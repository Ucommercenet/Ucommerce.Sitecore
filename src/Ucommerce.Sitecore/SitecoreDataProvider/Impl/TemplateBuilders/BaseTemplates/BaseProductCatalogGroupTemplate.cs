using Sitecore;
using Sitecore.Data;
using Ucommerce.Content;
using Ucommerce.EntitiesV2;
using Ucommerce.Extensions;
using Ucommerce.Infrastructure;
using Ucommerce.Infrastructure.Globalization;
using Ucommerce.Security;
using Ucommerce.Sitecore.Extensions;
using Ucommerce.Sitecore.Security;
using Ucommerce.Sitecore.SitecoreDataProvider.Impl.SitecoreItems.Templates;

namespace Ucommerce.Sitecore.SitecoreDataProvider.Impl.TemplateBuilders.BaseTemplates
{
	internal class BaseProductCatalogGroupTemplate
	{
		public static ID ProductCatalogGroupBaseTemplateId { get { return ID.Parse("{ABCE4356-91B3-4995-9F8B-A0454592335D}"); } }

		public TemplateItem BuildBaseStoreTemplate()
		{
			var builder = new TemplateBuilder();

			builder.CreateTemplate("ProductCatalogGroupBaseTemplate", ProductCatalogGroupBaseTemplateId, "Product Catalog Group Base Template", TemplateIDs.StandardTemplate);

			BuildCommonSection(builder, 100);

			var template = builder.Build();

			template.AddToFieldList(FieldIDs.Icon, SitecoreConstants.UCommerceIconFolder + "/ui/map.png");

			return template;
		}

		private void BuildCommonSection(TemplateBuilder builder, int sortorder)
		{
			var resourceManager = ObjectFactory.Instance.Resolve<IResourceManager>();
			var userService = ObjectFactory.Instance.Resolve<IUserService>();
			builder.CreateSection(resourceManager.GetLocalizedText(userService.GetCurrentUserCulture(), "Tabs", "Common"), FieldIds.Store.SectionCommonId, sortorder);

			builder.CreateTextField(FieldIds.Store.NameFieldId, "Name", 1);
			builder.CreateTextField(FieldIds.Store.ProductCatalogGroupIdFieldId, "Product catalog group id", 2);
			builder.CreateDropdownList(FieldIds.Store.HostNameFieldId, "Host Name", 3, FieldIds.SystemContent.HostnamesFolderId);
			builder.CreateRichTextEditorField(FieldIds.Store.DescriptionFieldId, "Description", 4);
			builder.CreateDropdownList(FieldIds.Store.CurrencyFieldId, "Currency", 5, FieldIds.SystemContent.CurrenciesFolderId);
			builder.CreateDropdownList(FieldIds.Store.EmailProfileFieldId, "E-mail profile", 6, FieldIds.SystemContent.EmailProfilesFolderId);
			builder.CreateDropdownList(FieldIds.Store.OrderNumberSeriesFieldId, "Order number series", 7, FieldIds.SystemContent.OrderNumberSeriesFolderId);
			builder.CreateCheckBoxField(FieldIds.Store.ProductReviewRequiresApprovalFieldId, "Product reviews require approval", 8);
			builder.CreateCheckBoxField(FieldIds.Store.CreateCustomersAsMembersFieldId, "Create customers as members", 9);
			builder.CreateDropdownList(FieldIds.Store.MemberGroupFieldId, "Member group", 10, FieldIds.SystemContent.MemberGroupsFolderId);
			builder.CreateDropdownList(FieldIds.Store.MemberTypeFieldId, "Member type", 11, FieldIds.SystemContent.MemberTypeFolderId);
		}

		public void AddBaseFieldValues(ProductCatalogGroup store, FieldList list, VersionUri version)
		{
			var securityPermisions = GetSecurityPermisionsFor(store);
			list.SafeAdd(FieldIDs.Security,securityPermisions);
			list.SafeAdd(FieldIds.Store.NameFieldId, store.Name);
			list.SafeAdd(FieldIds.Store.ProductCatalogGroupIdFieldId, store.ProductCatalogGroupId.ToString());
			list.SafeAdd(FieldIds.Store.HostNameFieldId, GetSitecoreItemIdForDomainId(store.DomainId).ToString());
			list.SafeAdd(FieldIds.Store.DescriptionFieldId, store.Description);
			list.SafeAdd(FieldIds.Store.CurrencyFieldId, store.Currency.SitecoreId().ToString());
			list.SafeAdd(FieldIds.Store.EmailProfileFieldId, store.EmailProfile.SitecoreId().ToString());
			list.SafeAdd(FieldIds.Store.OrderNumberSeriesFieldId, ConvertOrderNumberSeriesIdToSitecoreId(store.OrderNumberSerie));
			list.SafeAdd(FieldIds.Store.ProductReviewRequiresApprovalFieldId, store.ProductReviewsRequireApproval.ToSitecoreFormat());
			list.SafeAdd(FieldIds.Store.CreateCustomersAsMembersFieldId, store.CreateCustomersAsMembers.ToSitecoreFormat());
			list.SafeAdd(FieldIds.Store.MemberGroupFieldId, ConvertMemberGroupIdToSitecoreId(store.MemberGroupId));
			list.SafeAdd(FieldIds.Store.MemberTypeFieldId, ConvertMemberTypeIdToSitecoreId(store.MemberTypeId));
			list.SafeAdd(FieldIDs.Revision, store.Guid.Derived(store.ModifiedOn).ToString());
		}

		private string ConvertOrderNumberSeriesIdToSitecoreId(OrderNumberSerie storeOrderNumberSerie)
		{
			if (storeOrderNumberSerie == null)
			{
				return string.Empty;
			}

			return storeOrderNumberSerie.SitecoreId().ToString();
		}

		private string GetSecurityPermisionsFor(ProductCatalogGroup store)
		{
		    var valueBuilder = ObjectFactory.Instance.Resolve<ISecurityFieldValueBuilder>();
            var security = valueBuilder.BuildSecurityValue(store);

            return security;
		}

		private ID GetSitecoreItemIdForDomainId(string domainId)
		{
			if (string.IsNullOrEmpty(domainId))
			{
				return FieldIds.SystemContent.DefaultHostnameId;
			}

			var domainService = ObjectFactory.Instance.Resolve<IDomainService>();
			var domain = domainService.GetDomain(domainId);

			if (domain == null)
			{
				return FieldIds.SystemContent.DefaultHostnameId;
			}

			return domain.SitecoreId();
		}

		private string ConvertMemberGroupIdToSitecoreId(string memberGroupId)
		{
			if (string.IsNullOrEmpty(memberGroupId) || memberGroupId == "-1") return string.Empty;
            return new ID(SitecoreConstants.MemberGroupItemBaseId.Derived(memberGroupId).ToString()).ToString();
		}

		private string ConvertMemberTypeIdToSitecoreId(string memberType)
		{
			if (string.IsNullOrEmpty(memberType) || memberType == "-1") return string.Empty;
			return new ID(SitecoreConstants.MemberTypeItemBaseId.Derived(memberType).ToString()).ToString();
		}
	}
}
