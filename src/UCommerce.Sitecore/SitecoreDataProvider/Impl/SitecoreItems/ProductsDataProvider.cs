using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using Sitecore;
using Sitecore.Collections;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using UCommerce.EntitiesV2;
using UCommerce.EntitiesV2.Definitions;
using UCommerce.EntitiesV2.Queries.Catalog;
using UCommerce.Extensions;
using UCommerce.Infrastructure;
using UCommerce.Infrastructure.Globalization;
using UCommerce.Infrastructure.Logging;
using UCommerce.Sitecore.Extensions;
using UCommerce.Sitecore.SitecoreDataProvider.Impl.SitecoreItems.Content;
using UCommerce.Sitecore.SitecoreDataProvider.Impl.SitecoreItems.Templates;
using UCommerce.Sitecore.SitecoreDataProvider.Impl.TemplateBuilders;
using Version = Sitecore.Data.Version;

namespace UCommerce.Sitecore.SitecoreDataProvider.Impl.SitecoreItems
{
	internal class ProductsDataProvider : ISitecoreDataProvider
	{
		private static object _prefetchlock = new object();
		private readonly ILoggingService _log;
		private readonly ProductNodeSitecoreItemFactory _itemFactory;

		private readonly ProductTemplatesBuilder _productValueProvider;
		private readonly ProductVariantTemplatesBuilder _variantValueProvider;

		private ID EntryPointId { get; set; }
		private readonly IDList _firstLevelIds = new IDList();
		private VersionUriList _versions;

		private readonly ConcurrentDictionary<ID, ISitecoreItem> _sitecoreItems;
		private FolderItem Root { get; set; }
		private readonly object _lock = new object();

		private bool WeAreReady { get; set; }

		private int PartitionDepth { get; set; }

		private static FieldListCache _cache = new FieldListCache(1024, ObjectFactory.Instance.Resolve<IDetectFullCategoryScan>());
		private static FieldListCache Cache { get { return _cache; } }

		private static CacheStatistics _stats = new CacheStatistics("Product Field List Cache", 1000);
		private static CacheStatistics Stats { get { return _stats; } }

		private bool UseVerboseLogging
		{
			get { return false; }
		}

		public ProductsDataProvider(ILoggingService log, ProductNodeSitecoreItemFactory itemFactory, IList<ITemplateFieldValueProvider> valueProviders)
		{
			_log = log;
			_itemFactory = itemFactory;
			_firstLevelIds.Add(FieldIds.Product.ProductsRootFolderId);

			_sitecoreItems = new ConcurrentDictionary<ID, ISitecoreItem>();

			_productValueProvider = valueProviders.OfType<ProductTemplatesBuilder>().First();
			_variantValueProvider = valueProviders.OfType<ProductVariantTemplatesBuilder>().First();

			WeAreReady = false;
		}

		public ID GetEntryIdInSitecoreTree()
		{
			return EntryPointId;
		}

		public void SetEntryIdInSitecoreTree(ID entryPoint)
		{
			EntryPointId = entryPoint;
		}

		public IDList GetFirstLevelIds()
		{
			return _firstLevelIds;
		}

		public bool IsOneOfOurSitecoreItems(ID id)
		{
			MakeSureWeAreReady();
			return _sitecoreItems.ContainsKey(id);
		}

		public ItemDefinition GetItemDefinition(ID id)
		{
			MakeSureWeAreReady();

			var item = _sitecoreItems[id];
			if (UseVerboseLogging)
			{
				_log.Log<ProductsDataProvider>(string.Format("ProductsDataProvider: GetItemDefinition() : {0} - {1} - {2}", id.Guid, item.GetType().Name, item.ItemDefinition.Name));
			}

			return item.ItemDefinition;
		}

		public IDList GetChildIds(ID id)
		{
			MakeSureWeAreReady();

			var item = _sitecoreItems[id];
			if (UseVerboseLogging)
			{
				_log.Log<ProductsDataProvider>(string.Format("ProductsDataProvider: GetChildIds() : {0} - {1} - {2}", id.Guid, item.GetType().Name, item.ItemDefinition.Name));
			}

			// Prefetch the products and variants, if the current item is a Bucket containing products.
			// Loading children on GetChildIds is killing the cache when indexing.
			//PreFetchProductAndVariantFieldData(item);

			return item.ChildIds();
		}

		public bool HasChildren(ID id)
		{
			MakeSureWeAreReady();

			var item = _sitecoreItems[id];
			if (UseVerboseLogging)
			{
				_log.Log<ProductsDataProvider>(string.Format("ProductsDataProvider: HasChildren() : {0} - {1} - {2}", id.Guid, item.GetType().Name, item.ItemDefinition.Name));
			}

			return item.HasChildren();
		}

		public ID GetParentId(ID id)
		{
			MakeSureWeAreReady();

			var item = _sitecoreItems[id];
			if (UseVerboseLogging)
			{
				_log.Log<ProductsDataProvider>(string.Format("ProductsDataProvider: GetParentId() : {0} - {1} - {2}", id.Guid, item.GetType().Name, item.ItemDefinition.Name));
			}

			return item.ParentId;
		}

		public FieldList GetFieldList(ID id, VersionUri version)
		{
			MakeSureWeAreReady();

			var item = _sitecoreItems[id];

			if (UseVerboseLogging)
			{
				_log.Log<ProductsDataProvider>(string.Format("ProductsDataProvider: GetFieldList() : {0} - {1} - {2}", id.Guid, item.GetType().Name, item.ItemDefinition.Name));
			}

			if (item is BucketFolderItem)
			{
				return item.GetFieldList(version);
			}

			var cachedFieldList = Cache.Lookup(id, version);
			if (cachedFieldList != null)
			{
				Stats.Hit();
				if (UseVerboseLogging) _log.Log<ProductsDataProvider>("Cache hit");
				return cachedFieldList;
			}

			Stats.Miss();
			if (UseVerboseLogging) _log.Log<ProductsDataProvider>("Cache miss");


			lock (_prefetchlock)
			{
				PreFetchProductAndVariantFieldData(IdOfFirstParentThatIsABucket(item));
			}

			//PreFetchSingleProductOrVariantData(item);

			cachedFieldList = Cache.Lookup(id, version);
			if (cachedFieldList != null)
			{
				return cachedFieldList;
			}

			return _sitecoreItems[id].GetFieldList(version);
		}

		private ISitecoreItem IdOfFirstParentThatIsABucket(ISitecoreItem item)
		{
            if (item.ParentId.Guid == item.Id.Guid)
            {
                throw new InvalidDataException($"Id and parent Id cannot be the same. Id of item: '{ item.Id.Guid }' ParentId: '{ item.ParentId.Guid}'. Item definition: '{ item.ItemDefinition.Name }'");
            }
			if (!_sitecoreItems.ContainsKey(item.ParentId)) return null;

			var parent = _sitecoreItems[item.ParentId];

			if (parent is BucketFolderItem) return parent;

			return IdOfFirstParentThatIsABucket(parent);
		}

		public VersionUriList GetItemVersions()
		{
			if (_versions == null)
			{
				var languages = LanguageManager.GetLanguages(Factory.GetDatabase(SitecoreConstants.SitecoreMasterDatabaseName)).Distinct();
				var versions = new VersionUriList();
				foreach (var language in languages)
				{
					versions.Add(language, Version.First);
				}

				_versions = versions;
			}

			return _versions;
		}

		public bool SaveItem(ItemDefinition item, ItemChanges changes)
		{
			MakeSureWeAreReady();
			return _sitecoreItems[item.ID].SaveItem(changes);
		}

		public void Clear()
		{
			Cache.Clear();
			WeAreReady = false;
		}

		private void InitializeData()
		{
			_log.Log<SystemDataProvider>("Initializing the Product Data Provider data");
			InitializeRoot();
			InitializeProducts();
		}

		#region Methods informing the DataProvider when uCommerce data has changed
		public void ProductSaved(Product product)
		{
			// Product might be new thus the structure
			// doesn't reflect the newly created product.
			WeAreReady = false;

			Cache.Evict(new ID(product.Guid));

			// The save event from uCommerce is called on the product family.
			// So any of the variants could have been changed.
			foreach (var variant in product.Variants)
			{
				Cache.Evict(new ID(variant.Guid));
			}
		}

		public void ProductDeleted(Product product)
		{
			// Product might be new thus the structure
			// doesn't reflect the newly created product.
			WeAreReady = false;

			Cache.Evict(new ID(product.Guid));

			// The save event from uCommerce is called on the product family.
			// So any of the variants could have been changed.
			foreach (var variant in product.Variants)
			{
				Cache.Evict(new ID(variant.Guid));
			}
		}

		public void VariantDeleted(Product variant)
		{
			// Product might be new thus the structure
			// doesn't reflect the newly created product.
			WeAreReady = false;

			Cache.Evict(new ID(variant.Guid));
		}


		public void CategorySaved(Category category) { }

		public void CatalogSaved(ProductCatalog catalog) { }

		public void StoreSaved(ProductCatalogGroup store) { }

		public void ProductDefinitionSaved(ProductDefinition definition) { }

		public void DefinitionSaved(IDefinition definition) { }

		public void DefinitionFieldSaved(IDefinitionField field) { }

		public void LanguageSaved(Language language)
		{
			_versions = null;
		}

		public void DataTypeSaved(DataType dataType) { }

        public void PermissionsChanged() { }
        #endregion Methods informing the DataProvider when uCommerce data has changed

        private void InitializeRoot()
		{
			_sitecoreItems.Clear();
			_bucketFolders = new Dictionary<string, BucketFolderItem>(); // Reset the list of buckets.

			Root = new FolderItem(FieldIds.Product.ProductsRootFolderId, "Products");
			Root.AddToFieldList(new ID("{D312103C-B36C-4CA5-864A-C85F9ABDA503}"), "1"); // IsBucket

			Root.ParentId = EntryPointId;

			_sitecoreItems[Root.Id] = Root;
		}

		private void InitializeProducts()
		{
			lock (_lock)
			{
				var stopwatch = new Stopwatch();
				stopwatch.Start();

				IList<ProductTreeView> productTreeViews = GetListOfProducts().ToList();

				var variants = productTreeViews
					.Where(x => x.ParentId != 0)
					.GroupBy(x => x.ParentId)
					.ToDictionary(g => g.Key, g => g.ToList());

				var parents = productTreeViews.Where(x => x.ParentId == 0).ToList();

				PartitionDepth = CalculatePartitionDepth(parents.Count);

				foreach (var parent in parents)
				{
					var productItem = _itemFactory.Create(parent, FieldIds.Product.ProductsRootFolderId);
					_sitecoreItems[productItem.Id] = productItem;

					BuildPartitionStructure(productItem);

					if (variants.ContainsKey(parent.ProductId))
					{
						foreach (var variantView in variants[parent.ProductId])
						{
							var variant = _itemFactory.Create(variantView, productItem.Id);
							productItem.AddItem(variant);
							_sitecoreItems[variant.Id] = variant;
						}
					}
				}

				stopwatch.Stop();
				_log.Log<ProductsDataProvider>(string.Format("ProductsDataProvider.InitializeData(). {0} ms", stopwatch.ElapsedMilliseconds));
			}
		}

		private IEnumerable<ProductTreeView> GetListOfProducts()
		{
			var repository = ObjectFactory.Instance.Resolve<IRepository<ProductTreeView>>();
			return repository.Select(new AllProductVariantTreeViewQuery()).ToList();
		}

		public ContentNodeSitecoreItem GetProductNodeSitecoreItem(ID id)
		{
			if (_sitecoreItems.ContainsKey(id))
			{
				return _sitecoreItems[id] as ContentNodeSitecoreItem;
			}
			else
			{
				WeAreReady = false;
				return null;
			}
		}

		private const int SweetSpotNumberOfProductsPerBuckerFolder = 100;
		Dictionary<string, BucketFolderItem> _bucketFolders = new Dictionary<string, BucketFolderItem>();

		private void BuildPartitionStructure(ContentNodeSitecoreItem product)
		{
			var guidAsString = product.Id.Guid.ToString("N").ToUpper(CultureInfo.InvariantCulture);

			FolderItem parent = Root;

			for (int i = 0; i < PartitionDepth; i++)
			{
				var bucketIndex = guidAsString.Substring(0, i + 1);
				if (!_bucketFolders.ContainsKey(bucketIndex))
				{
					var folder = new BucketFolderItem(new ID(Root.Id.Guid.Derived(bucketIndex)), bucketIndex);
					_bucketFolders[bucketIndex] = folder;
					_sitecoreItems[folder.Id] = folder;

					parent.AddItem(folder);
				}

				parent = _bucketFolders[bucketIndex];
			}

			parent.AddItem(product);
		}

		private void MakeSureWeAreReady()
		{
			if (!WeAreReady)
			{
				lock (_lock)
				{
					if (!WeAreReady)
					{
						InitializeData();
						WeAreReady = true;
					}
				}
			}
		}

		private int CalculatePartitionDepth(int numberOfProducts)
		{
			int products = numberOfProducts;
			int depth = 0;
			while (products > SweetSpotNumberOfProductsPerBuckerFolder)
			{
				depth++;
				products /= 16;
			}

			if (depth < 1)
			{
				depth = 1;
			}

			_log.Log<ProductsDataProvider>(string.Format("Calculated the partition depth to be {0}, based upon {1} products.", depth, numberOfProducts));
			return depth;
		}

		private void FireItemDeletedEvent(Guid id)
		{
			var sitecoreContext = ObjectFactory.Instance.Resolve<ISitecoreContext>();

			var item = sitecoreContext.MasterDatabase.GetItem(new ID(id));
			if (item != null)
			{
				item.Database.Engines.DataEngine.RaiseDeletedItem(item, item.ParentID, true);
				_log.Log<TemplateDataProvider>("Raised the DeletedItem event for : " + id);
			}
		}

		private void PreFetchSingleProductOrVariantData(ISitecoreItem item)
		{
			var productItem = item as ContentNodeSitecoreItem;
			if (productItem == null) return;

			int productId = int.Parse(productItem.Node.ItemId);
			var repository = ObjectFactory.Instance.Resolve<IRepository<Product>>();
			var product = repository.Select(new SingleProductQuery(productId)).FirstOrDefault();

			if (product == null) return;

			if (product.IsVariant)
			{
				AddVariantDataToCache(product);
			}
			else
			{
				AddProductDataToCache(product);
			}
		}

		private void PreFetchProductAndVariantFieldData(ISitecoreItem item)
		{
			var bucket = item as BucketFolderItem;
			if (bucket == null || !bucket.HoldsProducts) return;

			var productIds = new List<KeyValuePair<int, Guid>>();

			foreach (var child in bucket.Children.Cast<ContentNodeSitecoreItem>())
			{
				productIds.Add(new KeyValuePair<int, Guid>(int.Parse(child.Node.ItemId), child.Node.ItemGuid.Value));
				productIds.AddRange(child.Children.Cast<ContentNodeSitecoreItem>().Select(grandChild => new KeyValuePair<int, Guid>(int.Parse(grandChild.Node.ItemId), grandChild.Node.ItemGuid.Value)));
			}

			var filteredIds = Cache.FilterOnItemsNotInCache(productIds).ToList();

			if (filteredIds.Count == 0) return;

			var repository = ObjectFactory.Instance.Resolve<IRepository<Product>>();
			var products = repository.Select(new MultipleProductQuery(filteredIds.Select(p => p.Key).ToList(), true));

			foreach (var product in products)
			{
				if (product.IsVariant)
				{
					AddVariantDataToCache(product);
				}
				else
				{
					AddProductDataToCache(product);
				}
			}
		}

		private void AddProductDataToCache(Product product)
		{
			foreach (VersionUri version in GetItemVersions())
			{
				var list = new FieldList();
				_productValueProvider.AddFieldValuesForProduct(product, list, version);
			    list.SafeAdd(FieldIDs.DisplayName, product.Name);

				Cache.Store(new ID(product.Guid), version, list);
			}
		}

		private void AddVariantDataToCache(Product variant)
		{
			foreach (VersionUri version in GetItemVersions())
			{
				var list = new FieldList();
				_variantValueProvider.AddFieldValuesForProductVariant(variant, list, version);
                list.SafeAdd(FieldIDs.DisplayName, variant.Name);

                Cache.Store(new ID(variant.Guid), version, list);
			}
		}
	}
}
