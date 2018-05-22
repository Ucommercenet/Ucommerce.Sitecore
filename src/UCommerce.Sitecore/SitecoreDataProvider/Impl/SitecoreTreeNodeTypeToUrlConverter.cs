using UCommerce.Tree;

namespace UCommerce.Sitecore.SitecoreDataProvider.Impl
{
	internal class SitecoreTreeNodeTypeToUrlConverter : ITreeNodeTypeToUrlConverter
	{
		public bool TryConvert(ITreeNodeContent treeNodeContent, out string url)
		{
			url = string.Empty;

			switch (treeNodeContent.NodeType)
			{
				case "productCatalogGroup":
					url = "ucommerce/catalog/editproductcataloggroup.aspx?ucid=" + treeNodeContent.ItemId; break;
				case "productCatalog":
					url = "ucommerce/catalog/editproductcatalog.aspx?ucid=" + treeNodeContent.ItemId; break;
				case "productCategory":
					url = "ucommerce/catalog/editcategory.aspx?ucid=" + treeNodeContent.ItemId; break;
				case "product":
					url = "ucommerce/catalog/editproduct.aspx?ucid=" + treeNodeContent.ItemId; break;
				case "catalogSearch":
					url = "ucommerce/catalog/searchproduct.aspx?ucid=-1"; break;

				case "orderGroup":
					url = "ucommerce/orders/viewordergroup.aspx?ucid=" + treeNodeContent.ItemId; break;
				case "order":
					url = "ucommerce/orders/editorder.aspx?ucid=" + treeNodeContent.ItemId; break;
				case "ordersSearch":
					url = "ucommerce/orders/search.aspx?ucid=-1"; break;

				case "campaign":
					url = "ucommerce/marketing/editcampaign.aspx?ucid=" + treeNodeContent.ItemId; break;
				case "campaignItem":
					url = "ucommerce/marketing/editcampaignitem.aspx?ucid=" + treeNodeContent.ItemId + "&parent=" + treeNodeContent.ItemContextId; break;

				case "productanalytics":
					url = "ucommerce/analytics/ProductAnalytics.aspx"; break;
				case "orderanalytics":
					url = "ucommerce/analytics/OrderAnalytics.aspx"; break;

				case "settingsCatalogPriceGroup":
					url = "ucommerce/settings/catalog/editpricegroup.aspx?ucid=" + treeNodeContent.ItemId; break;
				case "settingsCatalogProductRelation":
					url = "ucommerce/settings/catalog/editproductrelationtype.aspx?ucid=" + treeNodeContent.ItemId; break;

				case "shippingMethod":
					url = "ucommerce/settings/orders/editshippingmethod.aspx?ucid=" + treeNodeContent.ItemId; break;
				case "paymentMethod":
					url = "ucommerce/settings/orders/editpaymentmethod.aspx?ucid=" + treeNodeContent.ItemId; break;
				case "country":
					url = "ucommerce/settings/orders/editCountry.aspx?ucid=" + treeNodeContent.ItemId; break;
				case "currency":
					url = "ucommerce/settings/orders/editCurrency.aspx?ucid=" + treeNodeContent.ItemId; break;
				case "orderNumber":
					url = "ucommerce/settings/orders/editOrderNumberSerie.aspx?ucid=" + treeNodeContent.ItemId; break;

				case "settingsEmailProfiles":
					url = "ucommerce/settings/email/profiles"; break;
				case "settingsEmailTypes":
					url = "ucommerce/settings/email/types"; break;
				case "settingsEmailProfile":
					url = "ucommerce/settings/email/editemailProfile.aspx?ucid=" + treeNodeContent.ItemId; break;
				case "settingsEmailProfileType":
					url = "ucommerce/settings/email/editEmailProfileType.aspx?ucid=" + treeNodeContent.ItemId + "&parent=" + treeNodeContent.ItemContextId; break;
				case "settingsEmailType":
					url = "ucommerce/settings/email/editemailtype.aspx?ucid=" + treeNodeContent.ItemId; break;

				case "settingsDefinitionsProductDefinition":
					url = "ucommerce/settings/definitions/editproductdefinition.aspx?ucid=" + treeNodeContent.ItemId; break;
				case "settingsDefinitionsProductDefinitionField":
					url = "ucommerce/settings/definitions/editproductdefinitionfield.aspx?ucid=" + treeNodeContent.ItemId + "&parent=" + treeNodeContent.ItemContextId; break;
				case "settingsDefinitionsDynamicDefinition":
					url = "ucommerce/settings/definitions/editdefinition.aspx?ucid=" + treeNodeContent.ItemId; break;
				case "settingsDefinitionsDynamicDefinitionField":
					url = "ucommerce/settings/definitions/editdefinitionfield.aspx?ucid=" + treeNodeContent.ItemId + "&parent=" + treeNodeContent.ItemContextId; break;
				case "settingsDefinitionsDataType":
					url = "ucommerce/settings/definitions/editdatatype.aspx?ucid=" + treeNodeContent.ItemId; break;
				case "settingsDefinitionsDataTypeEnum":
					url = "ucommerce/settings/definitions/editDataTypeEnum.aspx?ucid=" + treeNodeContent.ItemId; break;
				case "settingsSecurityUser":
					url = "ucommerce/settings/security/EditUserAccess.aspx?ucid=" + treeNodeContent.ItemId; break;
				case "settingsSecurityUserGroup":
					url = "ucommerce/settings/security/EditUserGroupAccess.aspx?ucid=" + treeNodeContent.ItemId; break;

				default:
					return false;
			}

			return true;
		}
	}
}
