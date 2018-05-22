using System.Linq;
using Sitecore.Commerce.Contacts;
using Sitecore.Commerce.Services.Carts;
using SitecoreExt = Sitecore;
using Sitecore.Commerce.Entities.Carts;
using UCommerce.EntitiesV2;

namespace UCommerce.Sitecore.CommerceConnect.Mapping.ToCommerceConnect
{
	public class SetCartBaseValuesFromPurchaseOrder : IMapValues<PurchaseOrder, CartBase>
	{
		public void MapValues(PurchaseOrder purchaseOrder, CartBase cartBase)
		{
			cartBase.Status = GetOrderStatusFromProperties(purchaseOrder);

			if (IsCustomerAssigned(purchaseOrder))
				cartBase.CustomerId = purchaseOrder.Customer.CustomerId.ToString();

			var contactFactory = new ContactFactory();
			cartBase.UserId = contactFactory.GetContact();

			var cartBaseShopName = GetDomainForCart(purchaseOrder);
			cartBase.ShopName = cartBaseShopName;

			cartBase.ExternalId = purchaseOrder.BasketId.ToString();
			cartBase.IsLocked = false;
			cartBase.CartType = "Regular";

			cartBase.BuyerCustomerParty = GetBuyerCustomerParty(purchaseOrder);

			cartBase.AccountingCustomerParty = GetAccountingCustomerParty(purchaseOrder);
			
			cartBase.CurrencyCode = purchaseOrder.BillingCurrency.ISOCode;
		}

		protected virtual CartParty GetAccountingCustomerParty(PurchaseOrder purchaseOrder)
		{
		    if (purchaseOrder.BillingAddress == null) return null;

            return new CartParty()
            {
                ExternalId = purchaseOrder.BillingAddress.AddressId.ToString(),
                PartyID = purchaseOrder.BillingAddress.AddressName,
                Name = purchaseOrder.BillingAddress.AddressName
            };
        }

		protected virtual CartParty GetBuyerCustomerParty(PurchaseOrder purchaseOrder)
		{
		    var shippingAddress =
		        purchaseOrder.OrderAddresses.FirstOrDefault(x => x.AddressName == Constants.DefaultShipmentAddressName);
            if (shippingAddress == null) return null;

            return new CartParty()
            {
                ExternalId = shippingAddress.AddressId.ToString(),
                PartyID = shippingAddress.AddressName,
                Name = shippingAddress.AddressName
            };
        }

		private string GetDomainForCart(PurchaseOrder order)
		{
			return order.ProductCatalogGroup.DomainId;
		}

		private string GetOrderStatusFromProperties(PurchaseOrder purchaseOrder)
		{
			string orderStatusFromProperties = purchaseOrder["_cartStatus"];

			if (string.IsNullOrEmpty(orderStatusFromProperties))
			{
				purchaseOrder["_cartStatus"] = CartStatus.InProcess;
			}

			return purchaseOrder["_cartStatus"];
		}

		private bool IsProductCatalogGroupAssigned(PurchaseOrder purchaseOrder)
		{
			return purchaseOrder.ProductCatalogGroup != null;
		}

		private bool IsCustomerAssigned(PurchaseOrder purchaseOrder)
		{
			return purchaseOrder.Customer != null;
		}

		private bool IsOrderStatusAssigned(PurchaseOrder purchaseOrder)
		{
			return purchaseOrder.OrderStatus != null;
		}
	}
}
