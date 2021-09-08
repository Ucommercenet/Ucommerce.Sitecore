using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Sitecore;
using Sitecore.Collections;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Ucommerce.EntitiesV2;
using Ucommerce.EntitiesV2.Definitions;
using Ucommerce.EntitiesV2.Queries.Catalog;
using Ucommerce.Infrastructure;
using Ucommerce.Infrastructure.Globalization;
using Ucommerce.Infrastructure.Logging;
using Ucommerce.Sitecore.Extensions;
using Ucommerce.Sitecore.SitecoreDataProvider.Impl.SitecoreItems.Content;
using Ucommerce.Sitecore.SitecoreDataProvider.Impl.TemplateBuilders;
using Ucommerce.Tree;
using Ucommerce.Tree.Impl;
using Version = Sitecore.Data.Version;

namespace Ucommerce.Sitecore.SitecoreDataProvider.Impl.SitecoreItems
{
	internal class ContentNodeDataProvider : ISitecoreDataProvider
	{
		private ID _sitecoreEntryPoint;
		private readonly ILoggingService _log;
		private readonly ContentNodeSitecoreItemFactory _itemFactory;

		private readonly Dictionary<ID, ISitecoreItem> _sitecoreItems;
		private IDList _firstLevelIds;
		private VersionUriList _versions;

		private ProductCategoryTemplateBuilder _categoryValueProvider;
		private ProductCatalogTemplateBuilder _catalogValueProvider;

		private static readonly FieldListCache _cache = new FieldListCache(1024, ObjectFactory.Instance.Resolve<IDetectFullCategoryScan>());
		private static FieldListCache Cache { get { return _cache; } }

		private static readonly CacheStatistics _stats = new CacheStatistics("Catalog Field List Cache", 1000);
		private static CacheStatistics Stats { get { return _stats; } }

		private bool _weAreReady;
		private readonly object _lock = new object();

		private bool UseVerboseLogging
		{
			get { return false; }
		}

		public ContentNodeDataProvider(ILoggingService log, ContentNodeSitecoreItemFactory factory, IList<ITemplateFieldValueProvider> providers)
		{
			_log = log;
			_itemFactory = factory;

			_categoryValueProvider = providers.OfType<ProductCategoryTemplateBuilder>().First();
			_catalogValueProvider = providers.OfType<ProductCatalogTemplateBuilder>().First();

			_sitecoreItems = new Dictionary<ID, ISitecoreItem>();
		}

		public ID GetEntryIdInSitecoreTree()
		{
			return _sitecoreEntryPoint;
		}

		public void SetEntryIdInSitecoreTree(ID entryPoint)
		{
			_sitecoreEntryPoint = entryPoint;
		}

		public IDList GetFirstLevelIds()
		{
			MakeSureWeAreReady();
			return _firstLevelIds;
		}

		public bool IsOneOfOurSitecoreItems(ID id)
		{
			MakeSureWeAreReady();
			// We already have item in our dictionary.
			if (ItemIsContainedInDictionary(id)) { return true; }

			return false;
		}

		public ItemDefinition GetItemDefinition(ID id)
		{
			MakeSureWeAreReady();
			if (ItemIsContainedInDictionary(id))
			{
				var item = _sitecoreItems[id];
				if (UseVerboseLogging)
				{
					_log.Debug<ProductsDataProvider>(string.Format("ContentNodeDataProvider: GetItemDefinition() : {0} - {1} - {2}", id.Guid, item.GetType().Name, item.ItemDefinition.Name));
				}
				return item.ItemDefinition;
			}

			return null;
		}

		public bool HasChildren(ID id)
		{
			MakeSureWeAreReady();
			if (ItemIsContainedInDictionary(id))
			{
				var item = _sitecoreItems[id];
				if (UseVerboseLogging)
				{
					_log.Debug<ProductsDataProvider>(string.Format("ContentNodeDataProvider: HasChildren() : {0} - {1} - {2}", id.Guid, item.GetType().Name, item.ItemDefinition.Name));
				}
				return item.HasChildren();
			}

			return false;
		}

		public ID GetParentId(ID id)
		{
			MakeSureWeAreReady();
			if (ItemIsContainedInDictionary(id))
			{
				var item = _sitecoreItems[id];
				if (UseVerboseLogging)
				{
					_log.Debug<ProductsDataProvider>(string.Format("ContentNodeDataProvider: GetParentId() : {0} - {1} - {2}", id.Guid, item.GetType().Name, item.ItemDefinition.Name));
				}
				return item.ParentId;
			}

			return null;
		}

		public IDList GetChildIds(ID id)
		{
			MakeSureWeAreReady();


			if (ItemIsContainedInDictionary(id))
			{
				var item = _sitecoreItems[id];

				if (UseVerboseLogging)
				{
					_log.Debug<ProductsDataProvider>(string.Format("ContentNodeDataProvider: GetChildIds() : {0} - {1} - {2}", id.Guid, item.GetType().Name, item.ItemDefinition.Name));
				}

				if (item is CategorySitecoreItem)
				{
					PopulateCacheWithChildrensFieldLists(item as CategorySitecoreItem);
				}

				if (item is CatalogSitecoreItem)
				{
					PopulateCacheWithChildrensFieldLists(item as CatalogSitecoreItem);
				}

				var childIds = item.ChildIds();
				return childIds;
			}

			return new IDList();
		}

		public FieldList GetFieldList(ID id, VersionUri version)
		{
			MakeSureWeAreReady();

			if (ItemIsContainedInDictionary(id))
			{
				var item = _sitecoreItems[id];

				if (UseVerboseLogging)
				{
					_log.Debug<ProductsDataProvider>(string.Format("ContentNodeDataProvider: GetFieldList() : {0} - {1} - {2}", id.Guid, item.GetType().Name, item.ItemDefinition.Name));
				}

				if (item is CatalogSitecoreItem)
				{
					return GetFieldListForCatalog(item as CatalogSitecoreItem, version);
				}

				if (item is CategorySitecoreItem)
				{
					return GetFieldListForCategory(item as CategorySitecoreItem, version);
				}

				return _sitecoreItems[id].GetFieldList(version);
			}

			return new FieldList();
		}

		private FieldList GetFieldListForCatalog(CatalogSitecoreItem item, VersionUri version)
		{
			var fields = Cache.Lookup(item.Id, version);
			if (fields != null)
			{
				if (UseVerboseLogging) _log.Debug<ContentNodeDataProvider>("cache hit");
				Stats.Hit();
				return fields;
			}
			if (UseVerboseLogging) _log.Debug<ContentNodeDataProvider>("cache miss");
			Stats.Miss();

			PopulateCacheWithFieldListsForAllVersions(item);

			fields = _cache.Lookup(item.Id, version);
			if (fields != null)
			{
				return fields;
			}

			return item.GetFieldList(version);
		}

		private FieldList GetFieldListForCategory(CategorySitecoreItem item, VersionUri version)
		{
			var fields = Cache.Lookup(item.Id, version);
			if (fields != null)
			{
				if (UseVerboseLogging) _log.Debug<ContentNodeDataProvider>("cache hit");
				Stats.Hit();
				return fields;
			}
			if (UseVerboseLogging) _log.Debug<ContentNodeDataProvider>("cache hit");
			Stats.Miss();

			PopulateCacheWithFieldListsForAllVersions(item);

			fields = _cache.Lookup(item.Id, version);
			if (fields != null)
			{
				return fields;
			}

			return item.GetFieldList(version);
		}

		private void PopulateCacheWithFieldListsForAllVersions(CategorySitecoreItem item)
		{
			var repository = ObjectFactory.Instance.Resolve<IRepository<Category>>();
			var category = repository.Select(new SingleCategoryQuery(int.Parse(item.Node.ItemId))).FirstOrDefault();
			if (category == null) { throw new Exception("Could not read category with id " + item.Node.ItemId); }

			foreach (VersionUri version in GetItemVersions())
			{
				var list = new FieldList();
				list.SafeAdd(FieldIDs.Icon, item.GetIcon());
				list.SafeAdd(FieldIDs.Sortorder, item.Node.SortOrder.ToString());
                list.SafeAdd(FieldIDs.DisplayName, item.Node.Name);
				_categoryValueProvider.AddDataFromCategory(list, version, category);

				//_log.Log<ContentNodeDataProvider>(string.Format("Storing: {0}", item.Id));
				Cache.Store(item.Id, version, list);
			}
		}

		private void PopulateCacheWithFieldListsForAllVersions(CatalogSitecoreItem item)
		{
			var repository = ObjectFactory.Instance.Resolve<IRepository<ProductCatalog>>();
			var catalog = repository.Get(int.Parse(item.Node.ItemId));
			if (catalog == null) { throw new Exception("Could not read catalog with id " + item.Node.ItemId); }

			foreach (VersionUri version in GetItemVersions())
			{
				var list = new FieldList();
				list.SafeAdd(FieldIDs.Icon, item.GetIcon());
				list.SafeAdd(FieldIDs.Sortorder, item.Node.SortOrder.ToString());
                list.SafeAdd(FieldIDs.DisplayName, item.Node.Name);
                _catalogValueProvider.AddDataFromCatalog(catalog, list, version);

				//_log.Log<ContentNodeDataProvider>(string.Format("Storing: {0}", item.Id));
				Cache.Store(item.Id, version, list);
			}
		}

		public VersionUriList GetItemVersions()
		{
			if (_versions == null)
			{
				lock (_lock)
				{
					if (_versions == null)
					{
						var languages = LanguageManager.GetLanguages(Factory.GetDatabase(SitecoreConstants.SitecoreMasterDatabaseName)).Distinct();
						var versions = new VersionUriList();
						foreach (var language in languages)
						{
							//_log.Log<ContentNodeDataProvider>("Adding language to list of versions: " + language.CultureInfo.Name);
							versions.Add(language, Version.First);
						}

						_versions = versions;
					}
				}
			}

			return _versions;
		}

		public bool SaveItem(ItemDefinition item, ItemChanges changes)
		{
			MakeSureWeAreReady();
			if (ItemIsContainedInDictionary(item.ID))
			{
				return _sitecoreItems[item.ID].SaveItem(changes);
			}

			return false;
		}

		public void Clear()
		{
			Cache.Clear();
			_weAreReady = false;
		}

		#region Methods informing the DataProvider when uCommerce data has changed
		public void ProductSaved(Product product) { }

		public void ProductDeleted(Product product) { }

		public void VariantDeleted(Product variant) { }

		public void CategorySaved(Category category)
		{
			Clear();
			FireItemSavedEvent(category.Guid);
		}

		public void CatalogSaved(ProductCatalog catalog)
		{
			Clear();
			FireItemSavedEvent(catalog.Guid);
		}

		public void StoreSaved(ProductCatalogGroup store)
		{
			Clear();
			FireItemSavedEvent(store.Guid);
		}

		public void ProductDefinitionSaved(ProductDefinition definition) { }

		public void DefinitionSaved(IDefinition definition) { }

		public void DefinitionFieldSaved(IDefinitionField field) { }

		public void LanguageSaved(Language language)
		{
			_versions = null;
		}
		public void DataTypeSaved(DataType dataType) { }

	    public void PermissionsChanged()
	    {
	        EvictAllCatalogGroupsAndCatalogsFromSitecoresItemCache();
            Cache.Clear(); // Possible to be more accurate.
	    }

	    private void EvictAllCatalogGroupsAndCatalogsFromSitecoresItemCache()
	    {
	        foreach (var item in _sitecoreItems.Values)
	        {
	            if (item is CatalogSitecoreItem || item is ContentNodeSitecoreItem)
	            {
	                item.Id.EvictFromSitecoresItemCache();
	            }
	        }
	    }

	    #endregion Methods informing the DataProvider when uCommerce data has changed

		private bool ItemIsContainedInDictionary(ID id)
		{
			return _sitecoreItems.ContainsKey(id);
		}

		private void MakeSureWeAreReady()
		{
			if (!_weAreReady)
			{
				lock (_lock)
				{
					if (!_weAreReady)
					{
						Initialize();
						_weAreReady = true;
					}
				}
			}
		}

		private void Initialize()
		{
			var stopwatch = new Stopwatch();
			stopwatch.Start();

			_sitecoreItems.Clear();

			_versions = null;

			var treeService = ObjectFactory.Instance.Resolve<ITreeContentService>(SitecoreConstants.SitecoreDataProviderTreeServiceId);

			// Add main catalog node: "stores".
			var storeNode = treeService.GetChildren(Constants.DataProvider.NodeType.Root, "-1").First();
			var rootSiteCoreItem = _itemFactory.Create(storeNode, _sitecoreEntryPoint);
			_sitecoreItems[rootSiteCoreItem.Id] = rootSiteCoreItem;
			_firstLevelIds = new IDList { rootSiteCoreItem.Id };

			HydrateNodeStructure(rootSiteCoreItem);

			stopwatch.Stop();
			_log.Debug<ContentNodeDataProvider>(string.Format("ContentNodeDataProvider.Initialize(). {0} ms", stopwatch.ElapsedMilliseconds));
		}

		private void HydrateNodeStructure(ContentNodeSitecoreItem mainStoresNode)
		{
			// Add stores.
			var stopwatch = new Stopwatch();
			stopwatch.Start();

			var storeRepository = ObjectFactory.Instance.Resolve<IRepository<StoreTreeView>>();
			var stores = storeRepository.Select(new AllStoresTreeViewQuery());

			var dictionaryOfStores = new Dictionary<int, ContentNodeSitecoreItem>();
			var dictionaryOfCatalogs = new Dictionary<int, ContentNodeSitecoreItem>();

			foreach (var store in stores)
			{
				var storeNode = ConvertToNode(store);
				var storeItem = _itemFactory.Create(storeNode, mainStoresNode.Id);

				mainStoresNode.AddItem(storeItem);

				_sitecoreItems[storeItem.Id] = storeItem;

				dictionaryOfStores[store.ProductCatalogGroupId] = storeItem;
			}

			// Add catalogs.
			var catalogRepository = ObjectFactory.Instance.Resolve<IRepository<CatalogTreeView>>();
			var catalogs = catalogRepository.Select(new AllCatalogsTreeViewQuery());

			foreach (var catalog in catalogs)
			{
				if (dictionaryOfStores.ContainsKey(catalog.ProductCatalogGroupId))
				{
					var catalogNode = ConvertToNode(catalog);
					var catalogItem = _itemFactory.Create(catalogNode, dictionaryOfStores[catalog.ProductCatalogGroupId].Id);

					dictionaryOfStores[catalog.ProductCatalogGroupId].AddItem(catalogItem);

					_sitecoreItems[catalogItem.Id] = catalogItem;

					dictionaryOfCatalogs[catalog.ProductCatalogId] = catalogItem;
				}
			}

			// Add categories.
			var categoriesRepository = ObjectFactory.Instance.Resolve<IRepository<CategoryTreeView>>();
			var categories = categoriesRepository.Select(new AllCategoriesTreeViewQuery()).ToList();

			var topLevelCategories =
				categories.Where(c => !c.ParentCategoryId.HasValue)
					.GroupBy(c => c.ProductCatalogId)
					.ToDictionary(g => g.Key, g => g.ToList());

			var lowerLevelCategories =
				categories.Where(c => c.ParentCategoryId.HasValue)
					.GroupBy(c => c.ParentCategoryId.Value)
					.ToDictionary(g => g.Key, g => g.ToList());

			foreach (var catalogId in topLevelCategories.Keys)
			{
				if (dictionaryOfCatalogs.ContainsKey(catalogId))
				{
					var cats = topLevelCategories[catalogId];

					bool hasSortOrder = cats.Any(c => c.SortOrder != 0);
					int nextSortOrder = 0;

					foreach (var category in cats)
					{
						var categoryNode = ConvertToNode(category);

						if (!hasSortOrder)
						{
							categoryNode.SortOrder = nextSortOrder++;
						}

						var categoryItem = _itemFactory.Create(categoryNode);

						var parent = dictionaryOfCatalogs[category.ProductCatalogId];

						parent.AddItem(categoryItem);

						_sitecoreItems[categoryItem.Id] = categoryItem;

						AddChildCategories(category.CategoryId, categoryItem, lowerLevelCategories);
					}
				}
			}

			stopwatch.Stop();
			_log.Debug<ContentNodeDataProvider>(string.Format("Adding categories to cache took {0} ms.", stopwatch.ElapsedMilliseconds));
		}

		private void AddChildCategories(int categoryId, ContentNodeSitecoreItem categoryItem, Dictionary<int, List<CategoryTreeView>> lowerLevelCategories)
		{
			if (lowerLevelCategories.ContainsKey(categoryId))
			{
				bool hasSortOrder = lowerLevelCategories[categoryId].Any(x => x.SortOrder != 0);
				var nextSortOrder = 0;

				foreach (var childCategory in lowerLevelCategories[categoryId])
				{
					var categoryNode = ConvertToNode(childCategory);

					if (!hasSortOrder)
						categoryNode.SortOrder = nextSortOrder++;

					var childItem = _itemFactory.Create(categoryNode);

					categoryItem.AddItem(childItem);

					_sitecoreItems[childItem.Id] = childItem;

					AddChildCategories(childCategory.CategoryId, childItem, lowerLevelCategories);
				}
			}
		}

		private TreeNodeContent ConvertToNode(StoreTreeView store)
		{
			var node = new TreeNodeContent(Constants.DataProvider.NodeType.ProductCatalogGroup, store.ProductCatalogGroupId)
			{
				Name = store.Name,
				ItemGuid = store.Guid,
				SortOrder = store.ProductCatalogGroupId
			};

			return node;
		}

		private TreeNodeContent ConvertToNode(CatalogTreeView catalog)
		{
			var node = new TreeNodeContent(Constants.DataProvider.NodeType.ProductCatalog, catalog.ProductCatalogId)
			{
				Name = catalog.Name,
				ItemGuid = catalog.Guid,
				SortOrder = catalog.SortOrder
			};

			return node;
		}

		private TreeNodeContent ConvertToNode(CategoryTreeView category)
		{
			var node = new TreeNodeContent(Constants.DataProvider.NodeType.ProductCategory, category.CategoryId)
			{
				Name = category.Name,
				ItemGuid = category.Guid,
				SortOrder = category.SortOrder
			};

			return node;
		}

		private void FireItemSavedEvent(Guid id)
		{
			Cache.Evict(new ID(id));

			var sitecoreContext = ObjectFactory.Instance.Resolve<ISitecoreContext>();

			var item = sitecoreContext.MasterDatabase.GetItem(new ID(id));
			if (item != null)
			{
				item.Database.Engines.DataEngine.RaiseSavedItem(item, true);
				_log.Debug<TemplateDataProvider>("Raised the SavedItem event for : " + id);
			}
		}

		private void PopulateCacheWithChildrensFieldLists(CategorySitecoreItem categoryItem)
		{
			if (categoryItem == null) return;
			if (AreAllChildrenPresent(categoryItem)) return;

			int categoryId = int.Parse(categoryItem.Node.ItemId);

			var repository = ObjectFactory.Instance.Resolve<IRepository<Category>>();
			var categories = repository.Select(new ChildCategoryQuery(categoryId)).ToList();

			foreach (var category in categories)
			{
				if (!_sitecoreItems.ContainsKey(new ID(category.Guid))) { continue; } // Skip, if category does not exist anymore. This can happen, if the category is orphaned, in the database.

				var childItem = _sitecoreItems[new ID(category.Guid)] as ContentNodeSitecoreItem;
				if (childItem == null) { continue; }

				foreach (VersionUri version in GetItemVersions())
				{
					var list = new FieldList();
					list.SafeAdd(FieldIDs.Icon, childItem.GetIcon());
					list.SafeAdd(FieldIDs.Sortorder, childItem.Node.SortOrder.ToString());
                    list.SafeAdd(FieldIDs.DisplayName, childItem.Node.Name);
					_categoryValueProvider.AddDataFromCategory(list, version, category);

					//_log.Log<ContentNodeDataProvider>(string.Format("Storing: {0}", category.Guid));
					Cache.Store(new ID(category.Guid), version, list);
				}
			}
		}

		private void PopulateCacheWithChildrensFieldLists(CatalogSitecoreItem catalogItem)
		{
			if (catalogItem == null) return;
			if (AreAllChildrenPresent(catalogItem)) return;

			int catalogId = int.Parse(catalogItem.Node.ItemId);

			var catalog = ProductCatalog.Get(catalogId);

			var repository = ObjectFactory.Instance.Resolve<IRepository<Category>>();
			var categories = repository.Select(new CategoriesInCatalogQuery(catalog)).ToList();

			foreach (var category in categories)
			{
				var id = new ID(category.Guid);
				if (!_sitecoreItems.ContainsKey(id)) continue; // This can happen, if the parent of this category was deleted.

				var childItem = _sitecoreItems[id] as ContentNodeSitecoreItem;
				if (childItem == null) { throw new Exception("Could not find category in list of items. " + category.Guid); }

				foreach (VersionUri version in GetItemVersions())
				{
					var list = new FieldList();
					list.SafeAdd(FieldIDs.Icon, childItem.GetIcon());
					list.SafeAdd(FieldIDs.Sortorder, childItem.Node.SortOrder.ToString());
                    list.SafeAdd(FieldIDs.DisplayName, childItem.Node.Name);
                    _categoryValueProvider.AddDataFromCategory(list, version, category);

					//_log.Log<ContentNodeDataProvider>(string.Format("Storing: {0}", category.Guid));
					Cache.Store(new ID(category.Guid), version, list);
				}
			}
		}

		private bool AreAllChildrenPresent(ContentNodeSitecoreItem categoryItem)
		{
			return categoryItem.Children.All(child => Cache.ContainsItem(child.Id));
		}
	}
}
