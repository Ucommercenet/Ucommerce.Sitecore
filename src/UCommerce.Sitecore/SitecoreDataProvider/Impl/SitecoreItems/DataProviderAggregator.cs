using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Collections;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using UCommerce.EntitiesV2;
using UCommerce.EntitiesV2.Definitions;
using UCommerce.Infrastructure;
using UCommerce.Infrastructure.Globalization;
using UCommerce.Sitecore.SitecoreDataProvider.Impl.SitecoreItems.Content;
using UCommerce.Tree;
using Version = Sitecore.Data.Version;

namespace UCommerce.Sitecore.SitecoreDataProvider.Impl.SitecoreItems
{
	internal class DataProviderAggregator : ISitecoreDataProvider
	{
		private readonly ContentNodeSitecoreItemFactory _factory;
		private List<ISitecoreDataProvider> _providers;
		private ID _entryPoint;
		private IDList _firstLevelIds;
		private IDList _secondLevelIds;
		private ISitecoreItem _root;
		private ID _rootId = new ID("{3806A967-DD30-421F-A835-7D8DE80054B9}");

		private VersionUriList _versions;
		private readonly object _lock = new object();

		public DataProviderAggregator(Guid entryPoint, IEnumerable<ISitecoreDataProvider> providers, ContentNodeSitecoreItemFactory factory)
		{
			_factory = factory;
			_entryPoint = new ID(entryPoint);
			_providers = providers.ToList();

			_root = CreateRoot();

			_firstLevelIds = new IDList { _root.Id };

			_secondLevelIds = new IDList();
			foreach (var provider in _providers)
			{
				provider.SetEntryIdInSitecoreTree(_root.Id);
				var ids = provider.GetFirstLevelIds();
				foreach (ID id in ids)
				{
					_secondLevelIds.Add(id);
				}
			}

		}

		public ID GetEntryIdInSitecoreTree()
		{
			return _entryPoint;
		}

		public void SetEntryIdInSitecoreTree(ID entryPoint)
		{
			throw new NotImplementedException();
		}

		public IDList GetFirstLevelIds()
		{
			return _firstLevelIds;
		}

		public bool IsOneOfOurSitecoreItems(ID id)
		{
			if (id == _root.Id) { return true; }

			return _providers.Any(provider => provider.IsOneOfOurSitecoreItems(id));
		}

		public ItemDefinition GetItemDefinition(ID id)
		{
			if (id == _root.Id) { return _root.ItemDefinition; }

			return (from provider in _providers where provider.IsOneOfOurSitecoreItems(id) select provider.GetItemDefinition(id)).FirstOrDefault();
		}

		public IDList GetChildIds(ID id)
		{
			if (id == _entryPoint) { return _firstLevelIds; }
			if (id == _root.Id) { return _secondLevelIds; }

			return (from provider in _providers where provider.IsOneOfOurSitecoreItems(id) select provider.GetChildIds(id)).FirstOrDefault();
		}

		public bool HasChildren(ID id)
		{
			if (id == _root.Id) { return _root.HasChildren(); }

			return (from provider in _providers where provider.IsOneOfOurSitecoreItems(id) select provider.HasChildren(id)).FirstOrDefault();
		}

		public ID GetParentId(ID id)
		{
			if (id == _root.Id) { return _root.ParentId; }

			return (from provider in _providers where provider.IsOneOfOurSitecoreItems(id) select provider.GetParentId(id)).FirstOrDefault();
		}

		public FieldList GetFieldList(ID id, VersionUri version)
		{
			if (id == _root.Id) { return _root.GetFieldList(version); }

			return (from provider in _providers where provider.IsOneOfOurSitecoreItems(id) select provider.GetFieldList(id, version)).FirstOrDefault();
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
			if (item.ID == _root.Id) return false;

			return (from provider in _providers where provider.IsOneOfOurSitecoreItems(item.ID) select provider.SaveItem(item, changes)).FirstOrDefault();
		}

		public void Clear()
		{
			_versions = null;
			foreach (var provider in _providers)
			{
				provider.Clear();
			}
		}

		public void ProductSaved(Product product)
		{
			foreach (var provider in _providers)
			{
				provider.ProductSaved(product);
			}
		}

		public void ProductDeleted(Product product)
		{
			foreach (var provider in _providers)
			{
				provider.ProductDeleted(product);
			}
		}

		public void VariantDeleted(Product variant)
		{
			foreach (var provider in _providers)
			{
				provider.VariantDeleted(variant);
			}
		}

		public void CategorySaved(Category category)
		{
			foreach (var provider in _providers)
			{
				provider.CategorySaved(category);
			}
		}

		public void CatalogSaved(ProductCatalog catalog)
		{
			foreach (var provider in _providers)
			{
				provider.CatalogSaved(catalog);
			}
		}

		public void StoreSaved(ProductCatalogGroup store)
		{
			foreach (var provider in _providers)
			{
				provider.StoreSaved(store);
			}
		}

		public void ProductDefinitionSaved(ProductDefinition definition)
		{
			foreach (var provider in _providers)
			{
				provider.ProductDefinitionSaved(definition);
			}
		}

		public void DefinitionSaved(IDefinition definition)
		{
			foreach (var provider in _providers)
			{
				provider.DefinitionSaved(definition);
			}
		}

		public void DefinitionFieldSaved(IDefinitionField field)
		{
			foreach (var provider in _providers)
			{
				provider.DefinitionFieldSaved(field);
			}
		}

		public void LanguageSaved(Language language)
		{
			foreach (var provider in _providers)
			{
				provider.LanguageSaved(language);
			}
		}

		public void DataTypeSaved(DataType dataType)
		{
			foreach (var provider in _providers)
			{
				provider.DataTypeSaved(dataType);
			}
		}

	    public void PermissionsChanged()
	    {
            foreach (var provider in _providers)
            {
                provider.PermissionsChanged();
            }
        }

        private ISitecoreItem CreateRoot()
		{
			var treeService = ObjectFactory.Instance.Resolve<ITreeContentService>(SitecoreConstants.SitecoreDataProviderTreeServiceId);

			var root = treeService.GetRoot();
			var rootSiteCoreItem = _factory.Create(root, _entryPoint);
			rootSiteCoreItem.HasChildrenAlwaysTrue();

			return rootSiteCoreItem;
		}
	}
}
