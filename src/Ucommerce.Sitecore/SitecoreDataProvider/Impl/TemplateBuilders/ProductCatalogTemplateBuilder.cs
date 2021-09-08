using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Items;
using Ucommerce.EntitiesV2;
using Ucommerce.Extensions;
using Ucommerce.Infrastructure;
using Ucommerce.Infrastructure.Globalization;
using Ucommerce.Infrastructure.Logging;
using Ucommerce.Pipelines;
using Ucommerce.Security;
using Ucommerce.Sitecore.Extensions;
using Ucommerce.Sitecore.Security;
using Ucommerce.Sitecore.SitecoreDataProvider.Impl.TemplateBuilders.BaseTemplates.TemplateBuilderExtentions;
using Ucommerce.Tree;
using Convert = System.Convert;

namespace Ucommerce.Sitecore.SitecoreDataProvider.Impl.TemplateBuilders
{
	internal class ProductCatalogTemplateBuilder : ITemplateBuilder
	{
		private readonly ILoggingService _loggingService;

		private readonly Dictionary<int, ID> _priceGroupIdToFieldIdMap;

		public ProductCatalogTemplateBuilder(ILoggingService loggingService)
		{
			_loggingService = loggingService;

			_priceGroupIdToFieldIdMap = new Dictionary<int, ID>();
		}

		public bool SaveItem(ITreeNodeContent node, ItemChanges changes)
		{
			if (!changes.HasFieldsChanged) return false;

			var catalog = ProductCatalog.Get(Convert.ToInt32(node.ItemId));
			if (catalog == null)
			{
				var message = string.Format("Product Catalog with id: {0} not found for ITreeNodeContent. ", node.ItemId);
				_loggingService.Debug<ProductCatalogTemplateBuilder>(message);
				throw new InvalidDataException(message);
			}

			foreach (FieldChange change in changes.FieldChanges) UpdateCatalogValuesFor(change, catalog);

			ObjectFactory.Instance.Resolve<IPipeline<ProductCatalog>>("SaveProductCatalog").Execute(catalog);

			return true;
		}

		private void UpdateCatalogValuesFor(FieldChange fieldChange, ProductCatalog catalog)
		{
			if (this.FieldBelongsToStatistics(fieldChange)) return;
			if (this.ValueDidNotChangeFor(fieldChange)) return;

			if (fieldChange.FieldID == FieldIds.Catalog.NameFieldId)
				UpdateName(fieldChange, catalog);
			else if (fieldChange.FieldID == FieldIds.Catalog.CatalogIdFieldId)
				return;
			else if (fieldChange.FieldID == FieldIds.Catalog.DisplayNameFieldId)
				UpdateDisplayName(fieldChange, catalog);
			else if (fieldChange.FieldID == FieldIds.Catalog.DefaultPriceGroupId)
				UpdatePriceGroup(fieldChange, catalog);
			else if (fieldChange.FieldID == FieldIds.Catalog.ShowPricesWithVatId)
				catalog.ShowPricesIncludingVAT = fieldChange.Value == "1";
			else if (fieldChange.Definition.Section.ID == FieldIds.Catalog.SectionAllowedPriceGroupsId)
				UpdateAllowedPriceGroups(fieldChange, catalog);
			else
				_loggingService.Debug<ProductCatalogTemplateBuilder>(
					string.Format("Could not find property: {0} for product catalog: {1}.", fieldChange.Definition.Key, catalog.Name));
		}

		private void UpdateName(FieldChange fieldChange, ProductCatalog catalog)
		{
			var existingCatalog = ProductCatalog.SingleOrDefault(x => x.Name == fieldChange.Value);

			if (existingCatalog == null)
				catalog.Name = fieldChange.Value;
			else
				_loggingService.Debug<ProductCatalogTemplateBuilder>(string.Format("Catalog: {0} already exists, skipping update.", fieldChange.Value));
		}

		private void UpdateDisplayName(FieldChange fieldChange, ProductCatalog catalog)
		{
			var description = catalog.GetDescription(fieldChange.Language.CultureInfo.ToString());

			if (description != null)
				description.DisplayName = fieldChange.Value;
		}

		private void UpdatePriceGroup(FieldChange fieldChange, ProductCatalog catalog)
		{
			ID id;
			if (ID.TryParse(fieldChange.Value, out id))
			{
				var priceGroup = PriceGroup.SingleOrDefault(x => x.Guid == id.Guid);

				if (priceGroup == null)
				{
					_loggingService.Debug<ProductCatalogGroupTemplateBuilder>(
						string.Format("Failed to update price group for catalog. Could not find price group with guid: {0}.", id.Guid));
					return;
				}
				catalog.UpdatePriceGroup(priceGroup);
			}
			else
			{
				_loggingService.Debug<ProductCatalogGroupTemplateBuilder>(
					string.Format("Failed to update price group for catalog. Could not find Sitecore ID for price group with id: {0}.", fieldChange.Value));
			}
		}

		private void UpdateAllowedPriceGroups(FieldChange fieldChange, ProductCatalog catalog)
		{
			var priceGroupId = _priceGroupIdToFieldIdMap.FirstOrDefault(x => x.Value == fieldChange.FieldID).Key;
			var priceGroup = PriceGroup.Get(priceGroupId);

			if (fieldChange.Value == "1")
				catalog.AddAllowedPriceGroup(priceGroup);
			else
				catalog.RemoveAllowedPriceGroup(priceGroup);
		}

		public bool Supports(ITreeNodeContent node)
		{
			switch (node.NodeType)
			{
				case Constants.DataProvider.NodeType.ProductCatalog:
					return true;
				default:
					return false;
			}
		}

		public ID GetTemplateId(ITreeNodeContent node)
		{
			switch (node.NodeType)
			{
				case Constants.DataProvider.NodeType.ProductCatalog:
					return FieldIds.Catalog.ProductCatalogTemplateId;
				default:
					return null;
			}
		}

		public IEnumerable<ISitecoreItem> BuildTemplates()
		{
			var templateData = new List<ISitecoreItem>();
			var builder = new TemplateBuilder();

			builder.CreateTemplate("ProductCatalogTemplate", FieldIds.Catalog.ProductCatalogTemplateId, "Product Catalog Template", TemplateIDs.StandardTemplate);
			BuildCommonSection(builder, 10);
			BuildAllowedPriceGroupsSection(builder, 20);

			var template = builder.Build();

			template.AddToFieldList(FieldIDs.Icon,SitecoreConstants.UCommerceIconFolder + "/ui/map.png");
			template.SetRevision(Guid.NewGuid());

			templateData.Add(template);

			return templateData;
		}

		private void BuildCommonSection(TemplateBuilder builder, int sortOrder)
		{
			var resourceManager = ObjectFactory.Instance.Resolve<IResourceManager>();
			var userService = ObjectFactory.Instance.Resolve<IUserService>();
			builder.CreateSection(resourceManager.GetLocalizedText(userService.GetCurrentUserCulture(), "Tabs", "Common"), FieldIds.Catalog.SectionCommonId, sortOrder);

			builder.CreateTextField(FieldIds.Catalog.NameFieldId, "Name", 10);
			builder.CreateTextField(FieldIds.Catalog.CatalogIdFieldId, "Catalog id", 20);
			builder.CreateTextField(FieldIds.Catalog.DisplayNameFieldId, "Display name", 30);
			builder.CreateDropdownList(FieldIds.Catalog.DefaultPriceGroupId, "Default price group", 40, FieldIds.SystemContent.PriceGroupsFolderId);
			builder.CreateCheckBoxField(FieldIds.Catalog.ShowPricesWithVatId, "Show prices with VAT", 50);
		}

		private void BuildAllowedPriceGroupsSection(TemplateBuilder builder, int sortOrder)
		{
			builder.CreateSection("Allowed Price Groups", FieldIds.Catalog.SectionAllowedPriceGroupsId, sortOrder);

			var priceGroups = PriceGroup.Find(x => !x.Deleted).OrderBy(x => x.Name).ToList();
			_priceGroupIdToFieldIdMap.Clear();

			int priceGroupSortOrder = 10;
			foreach (var priceGroup in priceGroups)
			{
				ID priceGroupFieldId = priceGroup.GetOrCreateAllowedPriceGroupFieldId(FieldIds.Catalog.SectionAllowedPriceGroupsId);
				_priceGroupIdToFieldIdMap[priceGroup.PriceGroupId] = priceGroupFieldId;
				AddPriceGroupSpecificField(builder, priceGroup, priceGroupSortOrder, priceGroupFieldId);
				priceGroupSortOrder += 10;
			}

		}

		private void AddPriceGroupSpecificField(TemplateBuilder builder, PriceGroup priceGroup, int sortOrder, ID priceGroupFieldId)
		{
			builder.CreateCheckBoxField(priceGroupFieldId, priceGroup.Name, sortOrder);
		}

		public void AddFieldValues(ITreeNodeContent node, FieldList list, VersionUri version)
		{
			int categoryId = int.Parse(node.ItemId);
			AddDataFromCatalog(categoryId, list, version);
		}

		private string GetSecurityPermisionsFor(ProductCatalog catalog)
		{
            var valueBuilder = ObjectFactory.Instance.Resolve<ISecurityFieldValueBuilder>();
            var security = valueBuilder.BuildSecurityValue(catalog);

            return security;
        }

        internal void AddDataFromCatalog(int catalogId, FieldList list, VersionUri version)
		{
			var catalog = ProductCatalog.Get(catalogId);
			if (catalog == null) { return; }

			AddDataFromCatalog(catalog, list, version);
		}

		internal void AddDataFromCatalog(ProductCatalog catalog, FieldList list, VersionUri version)
		{
			if (catalog == null) { return; }

			list.SafeAdd(FieldIDs.Security, GetSecurityPermisionsFor(catalog));
			list.SafeAdd(FieldIds.Catalog.NameFieldId, catalog.Name);
			list.SafeAdd(FieldIds.Catalog.CatalogIdFieldId, catalog.ProductCatalogId.ToString());
			list.SafeAdd(FieldIds.Catalog.ShowPricesWithVatId, catalog.ShowPricesIncludingVAT.ToSitecoreFormat());
			list.SafeAdd(FieldIds.Catalog.DefaultPriceGroupId, catalog.PriceGroup.SitecoreId().ToString());
			list.SafeAdd(FieldIDs.Revision, catalog.Guid.Derived(catalog.ModifiedOn).ToString());

			var description = catalog.GetDescription(version.Language.Name);
			if (description != null)
			{
				list.SafeAdd(FieldIds.Catalog.DisplayNameFieldId, description.DisplayName);
			}

			var allowedPriceGroupIds = catalog.AllowedPriceGroups.ToList().Select(x => x.PriceGroupId).ToList();

			foreach (var priceGroupId in _priceGroupIdToFieldIdMap.Keys)
			{
				list.SafeAdd(_priceGroupIdToFieldIdMap[priceGroupId],
						 allowedPriceGroupIds.Contains(priceGroupId) ? true.ToSitecoreFormat() : false.ToSitecoreFormat());
			}
		}
	}
}
