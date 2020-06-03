using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Sitecore;
using Sitecore.Caching;
using Sitecore.Collections;
using Sitecore.Data;
using Sitecore.Data.DataProviders;
using Sitecore.Data.Engines;
using Sitecore.Data.Items;
using Sitecore.Data.Templates;
using Sitecore.Reflection;
using Ucommerce.EntitiesV2;
using Ucommerce.EntitiesV2.Definitions;
using Ucommerce.Infrastructure;
using Ucommerce.Infrastructure.Globalization;
using Ucommerce.Infrastructure.Logging;
using Ucommerce.Sitecore.Extensions;
using Ucommerce.Sitecore.Logging;
using Ucommerce.Sitecore.Settings;
using Ucommerce.Sitecore.SitecoreDataProvider.Impl;
using Ucommerce.Sitecore.SitecoreDataProvider.Impl.SitecoreItems;
using Ucommerce.Sitecore.SitecoreDataProvider.Impl.SitecoreItems.Content;
using Ucommerce.Sitecore.SitecoreDataProvider.Impl.StandardTemplateFields;
using Ucommerce.Sitecore.SitecoreDataProvider.Impl.TemplateBuilders;

namespace Ucommerce.Sitecore.SitecoreDataProvider
{
	public class DataProviderMasterDatabase : DataProviderAllDatabases
	{
		private readonly ID _rootItemId = ID.Parse("{11111111-1111-1111-1111-111111111111}");
		private readonly ID _userDefinedTemplateFolderId = ID.Parse("{B29EE504-861C-492F-95A3-0D890B6FCA09}");
		private readonly ID _systemItemId = ID.Parse("{13D6D6C6-C50B-4BBD-B331-2B04F1A58F21}");

		private readonly object _lock = new object();

		private readonly ILoggingService _log;

		private TemplateCollection _data = null;

		private readonly ItemFieldsCache _fieldsCache = new ItemFieldsCache(ObjectFactory.Instance.Resolve<IDetectFullCategoryScan>());

		private readonly HashSet<ID> _itemsKnownToBelongToUs = new HashSet<ID>();

		private bool _hasResetTemplatesEngines; // Flag used to make sure we only reset the templates engine once.
	    private readonly DataProviderSettings _dataProviderSettings;

		/// <summary>
		/// Clears the internal cache of the templates collection and asks Sitecore for all templates to set it agian.
		/// </summary>
		public void ResetTemplatesCollection()
		{
			var sitecoreContext = ObjectFactory.Instance.Resolve<ISitecoreContext>();
			if (!sitecoreContext.ShouldPullTemplatesFromSitecore)
				return;

			var loggingService = ObjectFactory.Instance.Resolve<ILoggingService>();
			loggingService.Log<DataProviderMasterDatabase>("ResetTemplatesCollection called.");

			Stopwatch watch = new Stopwatch();
			watch.Start();

			_data = null;

			var templates =
				sitecoreContext.MasterDatabase.Engines.TemplateEngine.GetTemplates()
					.Where(x => x.Value.FullName.Contains("uCommerce definitions"))
					.Select(x => x.Value)
					.ToList();

			SetTemplatesCollection(templates);

			watch.Stop();
			loggingService.Log<DataProviderMasterDatabase>(string.Format("ResetTemplatesCollection took: {0} ms.", watch.ElapsedMilliseconds));
		}

		/// <summary>
		/// Sets the internal data cache with the list of templates provided.
		/// </summary>
		/// <param name="templates"></param>
		public void SetTemplatesCollection(IList<Template> templates)
		{
			if (templates != null)
			{
				_data = new TemplateCollection();
				foreach (var template in templates)
				{
					_data.Add(template);
				}
			}
		}

		protected virtual bool UseVerboseLogging
		{
			get
			{
				return false;
			}
		}

		// Data Providers.
		private ISitecoreDataProviderForTemplates _templateDataProvider;
		private List<ISitecoreDataProvider> _sitecoreDataProviders;
		private ISitecoreStandardTemplateFieldValueProvider _standardTemplateFieldValueProvider;
		private DataProviderAggregator _dataProviderAggregator;

		private bool _breakEarlyUsingItemsKnownNotToBelongToUs = true;
		private readonly ConcurrentDictionary<Guid, Guid> _itemsNotOurOwnWeDeliverChildrenTo = new ConcurrentDictionary<Guid, Guid>();

		private bool _dependenciesHaveBeenSet;

		public DataProviderMasterDatabase()
		{
			_dependenciesHaveBeenSet = false;
			_log = new LoggingService();
            _dataProviderSettings = ObjectFactory.Instance.Resolve<DataProviderSettings>();
        }

		/// <summary>
		/// Part of the Sitecore Data Provider api.
		/// This method gets called for every single item in the sitecore tree. Remember: Everthing is Items!
		/// </summary>
		/// <remarks>
		/// It is essential to respond to all the items that are provided by this provider, and nothing else!
		/// This is necessary in order to "play nice" when sharing the database with other data providers.
		///
		/// That means that we should return <see cref="ItemDefinition"/> objects for:
		/// * Content nodes and all sub items.
		/// * Template nodes and all sub items. See below.
		/// We should NOT return Item definitions for:
		/// * the selected root node we hook our tree onto.
		///	    That particular ItemDefinition is provided by the standard data provider.
		///
		/// Sitecore uses two types of templates. Static and item-based.
		/// Static templates are the ones returned by the "GetTemplates()" method.
		/// Item-based are the ones returned by the "GetTemplateIds()" method.
		///
		/// Static templates are not displayed in the content editor.
		/// Item-based are displayed in the content editor.
		///
		/// We use the Item-based templates.
		/// </remarks>
		/// <param name="itemId">The ID of the item to perhaps return a ItemDefinition for.</param>
		/// <param name="context">The call context of the data provider api.</param>
		/// <returns>The appropriate ItemDefinition if the ID belongs to this data provider. Otherwise null.</returns>
		public override ItemDefinition GetItemDefinition(ID itemId, CallContext context)
		{
			if (!EntryGuardOkToProceed(context)) return null;
			if (IsThisItemKnownToNotBelongToUs(itemId, context)) return null;

			foreach (var sitecoreDataProvider in _sitecoreDataProviders)
			{
				if (sitecoreDataProvider.IsOneOfOurSitecoreItems(itemId))
				{
					if (UseVerboseLogging) _log.Log<DataProviderMasterDatabase>("GetItemDefinition called with 'itemId' = " + itemId);
					if (context != null) context.Abort();
					return sitecoreDataProvider.GetItemDefinition(itemId);
				}
			}

			return null;
		}

		/// <summary>
		/// Part of the Sitecore Data Provider api.
		///
		/// This method is responsible for populating the template fields
		/// with the actual data for ItemDefinitions belonging to this Data Provider.
		/// </summary>
		/// <param name="item">The <see cref="ItemDefinition"/> to provide field data for.</param>
		/// <param name="version">The version to deliver field data for.</param>
		/// <param name="context">The call context of the data provider api.</param>
		/// <returns>
		/// A field list with data for the fields in the template for the node,
		/// if the item belongs to this data provider. Otherwise null.
		/// </returns>
		public override FieldList GetItemFields(ItemDefinition item, VersionUri version, CallContext context)
		{
			if (!EntryGuardOkToProceed(context)) return null;
			if (IsThisItemKnownToNotBelongToUs(item.ID, context)) return null;

			var cachedData = _fieldsCache.Fetch(item.ID.Guid, version.Language.Name);
			if (cachedData != null)
			{
				return cachedData;
			}

			foreach (var sitecoreDataProvider in _sitecoreDataProviders)
			{
				if (sitecoreDataProvider.IsOneOfOurSitecoreItems(item.ID))
				{
					if (UseVerboseLogging) _log.Log<DataProviderMasterDatabase>("GetItemFields called with 'itemId' = " + item.ID + " Key = " + item.Key);

					RemoveItemFromSitecoresItemCache(item.ID, context.DataManager.Database);

					FieldList fieldValues = sitecoreDataProvider.GetFieldList(item.ID, version);

					GetItemFieldsForStandardTemplate(item, version, fieldValues);

					// Add empty context menu.
					fieldValues.SafeAdd(FieldIDs.ContextMenu, FieldIds.SystemContent.EmptyContextMenuNodeId.ToString());

					if (UseVerboseLogging)
					{
						foreach (KeyValuePair<ID, string> fieldValue in fieldValues)
						{
							_log.Log<DataProviderMasterDatabase>(string.Format("-- '{0}'. ID='{1}'. VersionUri='{2}'", fieldValue.Value, fieldValue.Key, version));
						}
					}

					context.Abort();
					_fieldsCache.Store(item.ID.Guid, version.Language.Name, fieldValues);
					return fieldValues;
				}
			}

			return null;
		}

		/// <summary>
		/// Part of the Sitecore Data Provider api.
		///
		/// This method is responsible for providing a list of child item ID's for a given item.
		/// This only returns child ids for nodes from this data provider, with one exception: The root node we hook into.
		/// </summary>
		/// <param name="item">The item to deliver children for.</param>
		/// <param name="context">The call context of the data provider api.</param>
		/// <returns>
		/// A list of ID's for the children nodes,
		/// if the item belongs to this data provider
		/// or is the root node.
		/// Otherwise null.
		/// </returns>
		public override IDList GetChildIDs(ItemDefinition item, CallContext context)
		{
			if (!EntryGuardOkToProceed(context)) return null;
			if (IsThisItemKnownToNotBelongToUs(item.ID, context) && !WeDeliverChildrenFor(item.ID)) return null;

			foreach (var sitecoreDataProvider in _sitecoreDataProviders)
			{
				if (sitecoreDataProvider.IsOneOfOurSitecoreItems(item.ID))
				{
					context.Abort();
					var childIds = sitecoreDataProvider.GetChildIds(item.ID);
					if (UseVerboseLogging)
					{
						_log.Log<DataProviderMasterDatabase>("GetChildIDs called with 'itemId' = " + item.ID + " Key = " + item.Key);
						foreach (var id in childIds)
						{
							_log.Log<DataProviderMasterDatabase>("  - " + id);
						}
					}

					foreach (ID childId in childIds)
					{
						_itemsKnownToBelongToUs.Add(childId);
					}

					return childIds;
				}

				if (item.ID == sitecoreDataProvider.GetEntryIdInSitecoreTree())
				{
					if (UseVerboseLogging) _log.Log<DataProviderMasterDatabase>("GetChildIDs called with 'itemId' = " + item.ID + " Key = " + item.Key + " Our first level.");
					var list = sitecoreDataProvider.GetFirstLevelIds();

					return list;
				}
			}

			return null;
		}

		/// <summary>
		/// Part of the Sitecore Data Provider Api
		///
		/// Returns true, if the item has children.
		/// </summary>
		/// <param name="item">The item to check for children.</param>
		/// <param name="context">The call context.</param>
		/// <returns>True, if the item is a uCommerce item, and it has children.</returns>
		public override bool HasChildren(ItemDefinition item, CallContext context)
		{
			if (!EntryGuardOkToProceed(context)) return false;
			if (IsThisItemKnownToNotBelongToUs(item.ID, context) && !WeDeliverChildrenFor(item.ID)) return false;

			foreach (var sitecoreDataProvider in _sitecoreDataProviders)
			{
				if (sitecoreDataProvider.IsOneOfOurSitecoreItems(item.ID))
				{
					if (UseVerboseLogging) _log.Log<DataProviderMasterDatabase>("HasChildren called with 'itemId' = " + item.ID + " Key = " + item.Key);
					context.Abort();
					return sitecoreDataProvider.HasChildren(item.ID);
				}

				if (item.ID == sitecoreDataProvider.GetEntryIdInSitecoreTree())
				{
					if (UseVerboseLogging) _log.Log<DataProviderMasterDatabase>("HasChildren called with 'itemId' = " + item.ID + " Key = " + item.Key + " Our first level.");
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Part of the Data Provider api.
		///
		/// This implementation is taken straight from the Northwind example.
		/// </summary>
		/// <param name="item">The item to get the parent id for.</param>
		/// <param name="context">The call context of the data provider api.</param>
		/// <returns>The parent id, if known.</returns>
		public override ID GetParentID(ItemDefinition item, CallContext context)
		{
			if (!EntryGuardOkToProceed(context)) return null;
			if (IsThisItemKnownToNotBelongToUs(item.ID, context)) return null;

			foreach (var sitecoreDataProvider in _sitecoreDataProviders)
			{
				if (sitecoreDataProvider.IsOneOfOurSitecoreItems(item.ID))
				{
					var result = sitecoreDataProvider.GetParentId(item.ID);
					if (UseVerboseLogging) _log.Log<DataProviderMasterDatabase>("GetParentID called with 'itemId' = " + item.ID + " Key = " + item.Key + " Returns " + result);
					context.Abort();
					return result;
				}
			}

			return null;
		}

		/// <summary>
		/// Part of the Sitecore DataProvider API.
		/// Returns a list of ID's for the templates in the provider.
		/// </summary>
		/// <param name="context">The sitecore context.</param>
		/// <returns>A list of Sitecore template ID's</returns>
		public override IdCollection GetTemplateItemIds(CallContext context)
		{
			if (!EntryGuardOkToProceed(context))
			{
				_log.Log<DataProviderMasterDatabase>("GetTemplateItemIds called before we are ready to proceed. Returning null.");
				return null;
			}

			//_log.Log<DataProviderMasterDatabase>("GetTemplateItemIds called.");

			var result = new IdCollection();

			foreach (ID id in _templateDataProvider.GetTemplateIds())
			{
				result.Add(id);
				//_log.Log<DataProviderMasterDatabase>("-- Template ID: " + id);
			}

			return result;
		}

		public override TemplateCollection GetTemplates(CallContext context)
		{
			return _data;
		}

		/// <summary>
		/// Gets a list of the available versions of an item.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <param name="context">The call context of the data provider api.</param>
		/// <returns></returns>
		public override VersionUriList GetItemVersions(ItemDefinition item, CallContext context)
		{
			if (!EntryGuardOkToProceed(context)) return null;
			if (IsThisItemKnownToNotBelongToUs(item.ID, context)) return null;

			foreach (var provider in _sitecoreDataProviders)
			{
				if (provider.IsOneOfOurSitecoreItems(item.ID))
				{
					context.Abort();
					return provider.GetItemVersions();
				}
			}

			return null;
		}

		public override bool SaveItem(ItemDefinition item, ItemChanges changes, CallContext context)
		{
			if (!EntryGuardOkToProceed(context)) return false;
			if (IsThisItemKnownToNotBelongToUs(item.ID, context)) return false;

			if (UseVerboseLogging) _log.Log<DataProviderMasterDatabase>("SaveItem called with 'itemId' = " + item.ID + " Key = " + item.Key);

			foreach (var provider in _sitecoreDataProviders)
			{
				if (provider.IsOneOfOurSitecoreItems(item.ID))
				{
					provider.SaveItem(item, changes);
					SaveStandardTemplateFields(item, changes);

					if (UseVerboseLogging) _log.Log<DataProviderMasterDatabase>("SaveItem finished with 'itemId' = " + item.ID + " Key = " + item.Key);
					_fieldsCache.EvictItem(item.ID.Guid);
					return true;
				}
			}
			if (UseVerboseLogging) _log.Log<DataProviderMasterDatabase>("SaveItem finished with 'itemId' = " + item.ID + " Key = " + item.Key + " No item hit.");
			return false;
		}

		private bool EntryGuardOkToProceed(CallContext context)
		{
			if (context == null) return true;
			if (ThisIsNotTheDatabaseYouAreLookingFor(context)) return false;
			ResetTheTemplatesEngineIfTheTemplatesListIsEmpty(context.DataManager.Database.Engines.TemplateEngine);
			return (EnsureDependenciesHaveBeenSet());
		}

		/// <summary>
		/// Lazy setup of the dependencies
		/// </summary>
		/// <remarks>
		/// NB! NB! NB!
		///
		/// The lifestyle of the data provider is controlled by Sitecore. It is created at application start,
		/// and remains in memory throughout.
		///
		/// Therefore you MUST NOT use the ObjectFactory to create objects held by the data provider.
		/// The lifestyle of the created objects would be either "Thread" or "WebRequest". Keeping a
		/// reference longer than that is asking for trouble!
		/// </remarks>
		private bool EnsureDependenciesHaveBeenSet()
		{
			if (_dependenciesHaveBeenSet) return true;

			lock (_lock)
			{
				if (_dependenciesHaveBeenSet) return true;

				var systemDataProvider = CreateSystemDataProvider();

				// I break the "lifestyle" rule stated above.
				// I can get away with it in this case, because I simply retrive data from the object,
				// and then it is allowed to be disposed. It is not kept alive.
				var standardTemplateFieldValueConfiguration =
					ObjectFactory.Instance.Resolve<ISitecoreStandardTemplateFieldValueConfiguration>();

				_standardTemplateFieldValueProvider = new StandardTemplateFieldValueProvider(
					new StandardFieldValuePersistor(),
					standardTemplateFieldValueConfiguration.WhiteListTemplateIds,
					standardTemplateFieldValueConfiguration.BlackListFieldIds
				);

				var builders = CreateTemplateBuilders();

				_templateDataProvider = new TemplateDataProvider(_log, _userDefinedTemplateFolderId, new TemplateProvider(builders));
				_templateDataProvider.GetTemplateIds(); // Called to make sure the templates are ready. They are needed below.

				// Field value providers
				var fieldValueProviders = builders.ToList<ITemplateFieldValueProvider>();

				// Content node data provider.
				var sitecoreItemfactory = new ContentNodeSitecoreItemFactory(fieldValueProviders);

			    CreateProductAndContentNodeDataProviderAggregator(sitecoreItemfactory, fieldValueProviders);

			    _sitecoreDataProviders = new List<ISitecoreDataProvider>
					{
						_templateDataProvider,
						systemDataProvider,
						_dataProviderAggregator
					};
				SetupItemsWeDeliverChildrenFor();

				_dependenciesHaveBeenSet = true;
				_log.Log<DataProviderMasterDatabase>("Dependencies have been setup.");

				return true;
			}
		}

        /// <summary>
        /// Create an aggregator containing the product and content node data provider.
        /// Excludes product data provider if IncludeProductData setting is false.
        /// </summary>
        /// <param name="sitecoreItemfactory"></param>
        /// <param name="fieldValueProviders"></param>
	    private void CreateProductAndContentNodeDataProviderAggregator(ContentNodeSitecoreItemFactory sitecoreItemfactory,
	        List<ITemplateFieldValueProvider> fieldValueProviders)
	    {
	        IList<ISitecoreDataProvider> dataProviders = new List<ISitecoreDataProvider>();

	        var contentNodeDataProvider = new ContentNodeDataProvider(_log, sitecoreItemfactory, fieldValueProviders);
	        dataProviders.Add(contentNodeDataProvider);

	        if (_dataProviderSettings.IncludeProductData)
	        {
	            var productsDataProvider = new ProductsDataProvider(_log,
	                new ProductNodeSitecoreItemFactory(fieldValueProviders), fieldValueProviders);
	            dataProviders.Add(productsDataProvider);
	        }

	        // Create the aggregator provider for the products and the content.
	        _dataProviderAggregator = new DataProviderAggregator(_rootItemId.Guid, dataProviders, sitecoreItemfactory);
	    }

	    private void SetupItemsWeDeliverChildrenFor()
		{
			foreach (var provider in _sitecoreDataProviders)
			{
				var guid = provider.GetEntryIdInSitecoreTree().Guid;
				_itemsNotOurOwnWeDeliverChildrenTo[guid] = guid;
			}
		}

		private List<ITemplateBuilder> CreateTemplateBuilders()
		{
			// Setup templates
			var storeTemplateBuilder = new ProductCatalogGroupTemplateBuilder(_log);
			var categoryTemplateBuilder = new ProductCategoryTemplateBuilder(_log);
			var catalogTemplateBuilder = new ProductCatalogTemplateBuilder(_log);
			var uCommerceTemplateBuilder = new UCommerceTemplateBuilder();
			var storesTemplateBuilder = new StoresTemplateBuilder();

			var builders = new List<ITemplateBuilder>
				{
					uCommerceTemplateBuilder,
					storesTemplateBuilder,
					storeTemplateBuilder,
					catalogTemplateBuilder,
					categoryTemplateBuilder
				};

            // Include product and variant templates if product item data is included in the data provider.
		    if (_dataProviderSettings.IncludeProductData)
		    {
		        var productTemplateBuilder = new ProductTemplatesBuilder(_log);
		        var variantTemplateBuilder = new ProductVariantTemplatesBuilder(_log);

                builders.Add(productTemplateBuilder);
                builders.Add(variantTemplateBuilder);
		    }

		    return builders;
		}

		private ISitecoreDataProvider CreateSystemDataProvider()
		{
			var systemDataProvider = new SystemDataProvider(_log, _systemItemId);
			return systemDataProvider;
		}

		/// <summary>
		/// Return false, if the context is the Master database. (The one we are looking for)
		/// </summary>
		/// <remarks>
		/// Sitecore will call our dataprovider for both the "Master" and the "Core" databases.
		/// I do not understand why, since it is only registered as a dataprovider for the "Master" database.
		/// But because of this, we need to check the database context, before handling the call.
		/// </remarks>
		/// <param name="context">The current call context.</param>
		/// <returns>false, if the context is the "Master" database.</returns>
		protected virtual bool ThisIsNotTheDatabaseYouAreLookingFor(CallContext context)
		{
			if (context != null && context.DataManager != null && context.DataManager.Database != null)
			{
				if (context.DataManager.Database.Name == SitecoreConstants.SitecoreMasterDatabaseName)
				{
					// This is not not the database we are looking for. ;-)
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Removes the sitecore item from the ItemCache
		/// </summary>
		/// <remarks>
		/// Two of the cache levels for Sitecore is ItemCache and DataCache.
		/// The data level caches the data parts of the item.
		/// The item level caches the completed item.
		///
		/// So when we return new data parts, we need to push the item out of the item cache
		/// to make Sitecore hit the Data cache with the updated data, the next time the item
		/// is requested.
		/// </remarks>
		/// <param name="itemId">The item id to remove from the Item cache.</param>
		/// <param name="context">The context.</param>
		private void RemoveItemFromSitecoresItemCache(ID itemId, Database context)
		{
			if (context.DataManager.DataSource.ItemExists(itemId))
			{
				// The method called using reflection is no longer private, but for backwards compatibility
				// We still call it the voodoo way.
				ReflectionUtil.CallMethod(
					typeof(ItemCache), CacheManager.GetItemCache(context.DataManager.Database),
					"RemoveItem", true, true, new object[] { itemId });
				//_log.Log<DataProviderMasterDatabase>("Called remove item from cache using reflection.");
			}
		}

		private void ClearTheSitecoreCaches()
		{
			CacheManager.ClearAllCaches();

			//Code defensively as we're trying to call external code we have no control of.
			var database = ObjectFactory.Instance.Resolve<ISitecoreContext>().MasterDatabase;
			if (database != null)
				if (database.Engines != null)
					if (database.Engines.TemplateEngine != null)
						database.Engines.TemplateEngine.Reset();
		}

		private void GetItemFieldsForStandardTemplate(ItemDefinition item, VersionUri version, FieldList fieldValues)
		{
			_standardTemplateFieldValueProvider.AddFieldValues(item.ID, version, fieldValues);
		}

		private void SaveStandardTemplateFields(ItemDefinition item, ItemChanges changes)
		{
			_standardTemplateFieldValueProvider.SaveItem(item.ID, changes);
		}

		private bool IsThisItemKnownToNotBelongToUs(ID itemId, CallContext context)
		{
			if (context == null) return false;

			if ((context.CurrentResult != null) && !(context.CurrentResult is bool) && _itemsKnownToBelongToUs.Contains(itemId) &&
				   context.CurrentResult is IDList && ((IDList) context.CurrentResult).Count != 0)
			{
				_log.Log<DataProviderMasterDatabase>("Sitecore already added data to an item we know is ours. " + itemId);
				throw new InvalidOperationException("Sitecore already added data to an item we know is ours. " + itemId);
			}
			return _breakEarlyUsingItemsKnownNotToBelongToUs &&
			       (context.Aborted || ((context.CurrentResult != null) && !(context.CurrentResult is bool))) &&
				   context.CurrentResult is IDList && ((IDList) context.CurrentResult).Count != 0; //Dataproviders can return an empty list if they cannot provide any children for it.
		}

		private bool WeDeliverChildrenFor(ID itemId)
		{
			return _itemsNotOurOwnWeDeliverChildrenTo.ContainsKey(itemId.Guid);
		}

		#region Methods informing the DataProvider when uCommerce data has changed
		/// <summary>
		/// Call this method to inform the data provider that basic data has changed.
		/// </summary>
		public void DataChangedPleaseReinitialize()
		{
			_log.Log<DataProviderMasterDatabase>("Definitions have changed ...");
			if (_sitecoreDataProviders != null)
			{
				foreach (var provider in _sitecoreDataProviders)
				{
					provider.Clear();
				}
			}

			ClearTheSitecoreCaches();
			_fieldsCache.Clear();
		}

		/// <summary>
		/// Call this method to inform the data provider that a specific product has changed
		/// </summary>
		public void ProductSaved(Product product)
		{
			_log.Log<DataProviderMasterDatabase>("Product saved : " + product.Name);
			if (_sitecoreDataProviders != null)
			{
				foreach (var provider in _sitecoreDataProviders)
				{
					provider.ProductSaved(product);
				}
			}

			_fieldsCache.EvictItem(product.Guid);
			product.Guid.EvictFromSitecoresItemCache();

			FireItemSavedEvent(product.Guid);

			foreach (var variant in product.Variants)
			{
				_fieldsCache.EvictItem(variant.Guid);
				variant.Guid.EvictFromSitecoresItemCache();

				FireItemSavedEvent(variant.Guid);
			}
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

		public void ProductDeleted(Product product)
		{
			_log.Log<DataProviderMasterDatabase>("Product deleted : " + product.Name);
			if (_sitecoreDataProviders != null)
			{
				foreach (var provider in _sitecoreDataProviders)
				{
					provider.ProductDeleted(product);
				}
			}

			_fieldsCache.EvictItem(product.Guid);
			product.Guid.EvictFromSitecoresItemCache();

			FireItemDeletedEvent(product.Guid);

			foreach (var variant in product.Variants)
			{
				_fieldsCache.EvictItem(variant.Guid);
				variant.Guid.EvictFromSitecoresItemCache();

				FireItemDeletedEvent(variant.Guid);
			}
		}

		public void VariantDeleted(Product variant)
		{
			_log.Log<DataProviderMasterDatabase>("Variant deleted : " + variant.Name);
			if (_sitecoreDataProviders != null)
			{
				foreach (var provider in _sitecoreDataProviders)
				{
					provider.VariantDeleted(variant);
				}
			}

			_fieldsCache.EvictItem(variant.Guid);
			variant.Guid.EvictFromSitecoresItemCache();

			FireItemDeletedEvent(variant.Guid);
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

		/// <summary>
		/// Call this method to inform the data provider that a specific category has changed
		/// </summary>
		public void CategorySaved(Category category)
		{
			_log.Log<DataProviderMasterDatabase>("Category saved : " + category.Name);
			if (_sitecoreDataProviders != null)
			{
				foreach (var provider in _sitecoreDataProviders)
				{
					provider.CategorySaved(category);
				}
			}

			_fieldsCache.EvictItem(category.Guid);
			category.Guid.EvictFromSitecoresItemCache();
		}

		/// <summary>
		/// Call this method to inform the data provider that a specific catalog has changed
		/// </summary>
		public void CatalogSaved(ProductCatalog catalog)
		{
			_log.Log<DataProviderMasterDatabase>("Catalog saved : " + catalog.Name);

			if (_sitecoreDataProviders != null)
			{
				foreach (var provider in _sitecoreDataProviders)
				{
					provider.CatalogSaved(catalog);
				}
			}

			_fieldsCache.EvictItem(catalog.Guid);
			catalog.Guid.EvictFromSitecoresItemCache();
		}

		/// <summary>
		/// Call this method to inform the data provider that a specific store has changed
		/// </summary>
		public void StoreSaved(ProductCatalogGroup store)
		{
			_log.Log<DataProviderMasterDatabase>("Store saved : " + store.Name);

			if (_sitecoreDataProviders != null)
			{
				foreach (var provider in _sitecoreDataProviders)
				{
					provider.StoreSaved(store);
				}
			}

			_fieldsCache.EvictItem(store.Guid);
			store.Guid.EvictFromSitecoresItemCache();
		}

		/// <summary>
		/// Call this method to inform the data provider that a specific product definition has changed
		/// </summary>
		public void ProductDefinitionSaved(ProductDefinition definition)
		{
			_log.Log<DataProviderMasterDatabase>("ProductDefinition saved : " + definition.Name);
			if (_sitecoreDataProviders != null)
			{
				foreach (var provider in _sitecoreDataProviders)
				{
					provider.ProductDefinitionSaved(definition);
				}
			}

			ResetTemplatesCollection();
		}

		/// <summary>
		/// Call this method to inform the data provider that a specific definition has changed
		/// </summary>
		public void DefinitionSaved(IDefinition definition)
		{
			_log.Log<DataProviderMasterDatabase>("Definition saved : " + definition.Name);
			if (_sitecoreDataProviders != null)
			{
				foreach (var provider in _sitecoreDataProviders)
				{
					provider.DefinitionSaved(definition);
				}
			}

			ResetTemplatesCollection();
		}

		/// <summary>
		/// Call this method to inform the data provider that a specific definition field has changed
		/// </summary>
		public void DefinitionFieldSaved(IDefinitionField field)
		{
			_log.Log<DataProviderMasterDatabase>("Definition field saved : " + field.Name);
			if (_sitecoreDataProviders != null)
			{
				foreach (var provider in _sitecoreDataProviders)
				{
					provider.DefinitionFieldSaved(field);
				}
			}

			ResetTemplatesCollection();
		}

		public void LanguageSaved(Language language)
		{
			_log.Log<DataProviderMasterDatabase>("Language saved : " + language.LanguageName);
			if (_sitecoreDataProviders != null)
			{
				foreach (var provider in _sitecoreDataProviders)
				{
					provider.LanguageSaved(language);
				}
			}
		}

		public void DataTypeSaved(DataType dataType)
		{
			_log.Log<DataProviderMasterDatabase>("DataType saved : " + dataType.Name);
			if (_sitecoreDataProviders != null)
			{
				foreach (var provider in _sitecoreDataProviders)
				{
					provider.DataTypeSaved(dataType);
				}
			}

			ResetTemplatesCollection();
		}

        /// <summary>
        /// Call this method, when the Sitecore Security field needs to be recomputed.
        /// </summary>
	    public void PermissionsChanged()
	    {
	        _log.Log<DataProviderMasterDatabase>("PermissionsChanged() called.");
            _fieldsCache.Clear();

            if (_sitecoreDataProviders != null)
            {
                foreach (var provider in _sitecoreDataProviders)
                {
                    provider.PermissionsChanged();
                }
            }
        }
        #endregion Methods informing the DataProvider when uCommerce data has changed

        /// <summary>
        /// Reset the templates engine, if it is initialized but empty.
        /// </summary>
        /// <remarks>
        /// If an event as part of the add configuration code accesses templates, it can sometime happen,
        /// that the templates engine is started up, but without any templates in it. And it nevers resets.
        ///
        /// The symptom is that all items are displayed as "This item has no fields".
        /// </remarks>
        private void ResetTheTemplatesEngineIfTheTemplatesListIsEmpty(TemplateEngine templateEngine)
		{
			if (_hasResetTemplatesEngines) return;

			_hasResetTemplatesEngines = true;

			FieldInfo field = templateEngine.GetType().GetField("_templates", BindingFlags.NonPublic | BindingFlags.Instance);
			if (field == null) return;

			var templates = (TemplateDictionary) field.GetValue(templateEngine);

			if (templates != null && templates.Count == 0)
			{
				_log.Log<DataProviderMasterDatabase>("Resetting the template engine, because it is initialized but empty.");
				templateEngine.Reset();
			}
		}
	}
}
