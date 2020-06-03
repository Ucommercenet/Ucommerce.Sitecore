using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Sitecore.Collections;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Ucommerce.Content;
using Ucommerce.EntitiesV2;
using Ucommerce.EntitiesV2.Definitions;
using Ucommerce.Infrastructure;
using Ucommerce.Infrastructure.Globalization;
using Ucommerce.Infrastructure.Logging;
using Ucommerce.Security;
using Version = Sitecore.Data.Version;

namespace Ucommerce.Sitecore.SitecoreDataProvider.Impl.SitecoreItems
{
	internal class SystemDataProvider : ISitecoreDataProvider
	{
		private readonly ILoggingService _log;

		private ID EntryPointId { get; set; }

		private readonly Dictionary<ID, ISitecoreItem> _sitecoreItems;
		private IDList _idList = new IDList();
		private VersionUriList _versions;

		private bool _weAreReady = false;
		private object _lock = new object();

		public SystemDataProvider(ILoggingService log, ID entryPointId)
		{
			_log = log;
			EntryPointId = entryPointId;

			_sitecoreItems = new Dictionary<ID, ISitecoreItem>();
		}

		public ID GetEntryIdInSitecoreTree()
		{
			return EntryPointId;
		}

		public void SetEntryIdInSitecoreTree(ID entryPoint)
		{
			throw new NotImplementedException();
		}

		public IDList GetFirstLevelIds()
		{
			MakeSureWeAreReady();
			return _idList;
		}

		public bool IsOneOfOurSitecoreItems(ID id)
		{
			MakeSureWeAreReady();
			return _sitecoreItems.ContainsKey(id);
		}

		public ItemDefinition GetItemDefinition(ID id)
		{
			MakeSureWeAreReady();
			return _sitecoreItems[id].ItemDefinition;
		}

		public IDList GetChildIds(ID id)
		{
			MakeSureWeAreReady();
			return _sitecoreItems[id].ChildIds();
		}

		public bool HasChildren(ID id)
		{
			MakeSureWeAreReady();
			return _sitecoreItems[id].HasChildren();
		}

		public ID GetParentId(ID id)
		{
			MakeSureWeAreReady();
			return _sitecoreItems[id].ParentId;
		}

		public FieldList GetFieldList(ID id, VersionUri version)
		{
			MakeSureWeAreReady();
			return _sitecoreItems[id].GetFieldList(version);
		}

		public VersionUriList GetItemVersions()
		{
			MakeSureWeAreReady();
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
							_log.Log<TemplateDataProvider>("Adding language to list of versions: " + language.CultureInfo.Name);
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
			return false;
		}

		private void MakeSureWeAreReady()
		{
			if (!_weAreReady)
			{
				lock (_lock)
				{
					if (!_weAreReady)
					{
						InitializeData();
						_weAreReady = true;
					}
				}
			}
		}

		public void Clear()
		{
			_weAreReady = false;
		}

		#region Methods informing the DataProvider when uCommerce data has changed
		public void ProductSaved(Product product) {}

		public void ProductDeleted(Product product) { }

		public void VariantDeleted(Product variant) { }

		public void CategorySaved(Category category) {}

		public void CatalogSaved(ProductCatalog catalog) {}

		public void StoreSaved(ProductCatalogGroup store) {}

		public void ProductDefinitionSaved(ProductDefinition definition) {}

		public void DefinitionSaved(IDefinition definition) {}

		public void DefinitionFieldSaved(IDefinitionField field) {}

		public void LanguageSaved(Language language)
		{
			_versions = null;
		}

		public void DataTypeSaved(DataType dataType)
		{
			Clear();
		}

        public void PermissionsChanged() { }
        #endregion Methods informing the DataProvider when uCommerce data has changed

        private void AddSitecoreItem(ISitecoreItem item)
		{
			item.ParentId = EntryPointId;
			_idList.Add(item.Id);
			AddItemsToDictionary(item);
		}

		private void AddItemsToDictionary(ISitecoreItem sitecoreItem)
		{
			_sitecoreItems[sitecoreItem.Id] = sitecoreItem;

			foreach (var child in sitecoreItem.Children)
			{
				AddItemsToDictionary(child);
			}
		}

		private void InitializeData()
		{
			_log.Log<SystemDataProvider>("Initializing the System Data Provider data");
			var stopwatch = new Stopwatch();
			stopwatch.Start();

			_versions = null;
			_sitecoreItems.Clear();
			_idList = new IDList();

			var memberService = ObjectFactory.Instance.Resolve<IMemberService>();
			var domainService = ObjectFactory.Instance.Resolve<IDomainService>();

			if (memberService == null || domainService == null)
			{
				throw new InvalidOperationException("Could not get the dependencies!");
			}

			var systemNodesContentProvider = new SystemNodesContentProvider(domainService, memberService);

			foreach (var item in systemNodesContentProvider.GetSystemData())
			{
				AddSitecoreItem(item);
			}

			stopwatch.Stop();
			_log.Log<SystemDataProvider>(string.Format("SystemDataProvider.InitializeData(). {0} ms", stopwatch.ElapsedMilliseconds));
		}
	}
}
