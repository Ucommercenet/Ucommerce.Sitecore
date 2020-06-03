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
using Sitecore.Data.Templates;
using Ucommerce.EntitiesV2;
using Ucommerce.EntitiesV2.Definitions;
using Ucommerce.Extensions;
using Ucommerce.Infrastructure;
using Ucommerce.Infrastructure.Globalization;
using Ucommerce.Infrastructure.Logging;
using Ucommerce.Sitecore.SitecoreDataProvider.Impl.SitecoreItems.Templates;
using Version = Sitecore.Data.Version;

namespace Ucommerce.Sitecore.SitecoreDataProvider.Impl.SitecoreItems
{
	internal class TemplateDataProvider : ISitecoreDataProviderForTemplates
	{
		private readonly ID EntryPointId;
		private readonly ILoggingService _log;
		private readonly ITemplateProvider _templateProvider;
		private const string _iconFolder = SitecoreConstants.UCommerceIconFolder;
		private FolderItem _topUcommerceFolder;

		private readonly Dictionary<ID, ISitecoreItem> _sitecoreItems;
		private IDList _idList = new IDList();
		private VersionUriList _versions;

		private bool _weAreReady;
		private object _lock = new object();

		public TemplateDataProvider(ILoggingService log, ID entryPointId, ITemplateProvider templateProvider)
		{
			_sitecoreItems = new Dictionary<ID, ISitecoreItem>();
			EntryPointId = entryPointId;
			_log = log;
			_templateProvider = templateProvider;

			Clear();
		}

		public void AddSitecoreItem(ISitecoreItem sitecoreItem)
		{
			_topUcommerceFolder.AddItem(sitecoreItem);
			AddItemsToDictionary(sitecoreItem);
		}

		public IDList GetTemplateIds()
		{
			MakeSureWeAreReady();
			return _idList;
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
			var ids = new IDList {_topUcommerceFolder.Id};
			return ids;
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
				var languages = LanguageManager.GetLanguages(Factory.GetDatabase(SitecoreConstants.SitecoreMasterDatabaseName)).Distinct();
				var versions = new VersionUriList();
				foreach (var language in languages)
				{
					_log.Log<TemplateDataProvider>("Adding language to list of versions: " + language.CultureInfo.Name);
					versions.Add(language, Version.First);
				}

				_versions = versions;
			}

			return _versions;
		}

		public bool SaveItem(ItemDefinition item, ItemChanges changes)
		{
			return false;
		}

		public void Clear()
		{
			lock (_lock)
			{
				_weAreReady = false;

				_versions = null;
				_sitecoreItems.Clear();
				_topUcommerceFolder = new FolderItem(FieldIds.Template.UCommerceTemplateFolderId, SitecoreConstants.UCommerceDynamicTemplatesFolderName)
				{
					ParentId = EntryPointId
				};

				_topUcommerceFolder.AddToFieldList(FieldIDs.Icon, _iconFolder + "/uCommerce-logo-symbol-small.png");
				AddItemsToDictionary(_topUcommerceFolder);
				_idList = new IDList();
			}
		}

		#region Methods informing the DataProvider when uCommerce data has changed
		public void ProductSaved(Product product) {}

		public void ProductDeleted(Product product) { }

		public void VariantDeleted(Product variant) { }

		public void CategorySaved(Category category) {}

		public void CatalogSaved(ProductCatalog catalog) {}

		public void StoreSaved(ProductCatalogGroup store) {}

		public void ProductDefinitionSaved(ProductDefinition definition)
		{
			Clear();

			var templateId = definition.Guid;
			var variantTemplateId = templateId.Derived("VariantTemplate");

			// Inform Sitecore of the potentially changed templates.
			FireItemSavedEvent(templateId);
			FireItemSavedEvent(variantTemplateId);
		}

		private void FireItemSavedEvent(Guid id)
		{
			var sitecoreContext = ObjectFactory.Instance.Resolve<ISitecoreContext>();

			var item = sitecoreContext.MasterDatabase.GetItem(new ID(id));
			if (item != null)
			{
				item.Database.Engines.DataEngine.RaiseSavedItem(item, true);
				_log.Log<TemplateDataProvider>("Raised the SavedItem event for : " + id);
			}
		}

		public void DefinitionSaved(IDefinition definition)
		{
			Clear();

			var templateId = definition.Guid;

			// Inform Sitecore of the potentially changed templates.
			FireItemSavedEvent(templateId);
		}

		public void DefinitionFieldSaved(IDefinitionField field)
		{
			Clear();

			var fieldId = field.Guid;

			// Inform Sitecore of the potentially changed templates.
			FireItemSavedEvent(fieldId);
		}

		public void LanguageSaved(Language language)
		{
			_versions = null;
		}

		public void DataTypeSaved(DataType dataType)
		{
			Clear();

			var fieldId = dataType.Guid;

			FireItemSavedEvent(fieldId);
		}

        public void PermissionsChanged() { }
        #endregion Methods informing the DataProvider when uCommerce data has changed

        public TemplateCollection GetTemplates()
		{
			MakeSureWeAreReady();

			var collection = new TemplateCollection();
			var converter = new TemplateItemToStaticTemplateConverter();

			foreach (ID id in _idList)
			{
				var item = _sitecoreItems[id] as Templates.TemplateItem;
				if (item != null)
				{
					collection.Add(converter.Convert(item, collection));
				}
			}
			return collection;
		}

		private void AddItemsToDictionary(ISitecoreItem sitecoreItem)
		{
			_sitecoreItems[sitecoreItem.Id] = sitecoreItem;

			if (sitecoreItem.IsTemplate())
			{
				_idList.Add(sitecoreItem.Id);
			}

			foreach (var child in sitecoreItem.Children)
			{
				AddItemsToDictionary(child);
			}
		}

		private void BuildTemplates()
		{
			var stopwatch = new Stopwatch();
			stopwatch.Start();

			_log.Log<TemplateDataProvider>("BuildTemplates() called.");
			Clear();

			var result = _templateProvider.GetTemplates();
			foreach (var sitecoreItem in result)
			{
				AddSitecoreItem(sitecoreItem);
			}

			stopwatch.Stop();
			_log.Log<TemplateDataProvider>(string.Format("TemplateDataProvider.BuildTemplates(). {0} ms", stopwatch.ElapsedMilliseconds));
		}

		private void MakeSureWeAreReady()
		{
			if (!_weAreReady)
			{
				lock (_lock)
				{
					if (!_weAreReady)
					{
						BuildTemplates();
						_weAreReady = true;
					}
				}
			}
		}
	}
}
