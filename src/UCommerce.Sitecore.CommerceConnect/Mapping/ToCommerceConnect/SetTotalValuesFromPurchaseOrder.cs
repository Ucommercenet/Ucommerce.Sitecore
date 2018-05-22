using Sitecore.Commerce.Entities.Prices;
using UCommerce.EntitiesV2;

namespace UCommerce.Sitecore.CommerceConnect.Mapping.ToCommerceConnect
{
	public class SetTotalValuesFromPurchaseOrder : IMapValues<PurchaseOrder, Total>
	{
		private readonly IMapping<PurchaseOrder, TaxTotal> _purchaseOrderToTaxTotal;

		public SetTotalValuesFromPurchaseOrder(IMapping<PurchaseOrder, TaxTotal> purchaseOrderToTaxTotal)
		{
			_purchaseOrderToTaxTotal = purchaseOrderToTaxTotal;
		}

		public void MapValues(PurchaseOrder source, Total target)
		{
			target.Amount = source.OrderTotal.GetValueOrDefault();
			if(source.BillingCurrency!= null) target.CurrencyCode = source.BillingCurrency.ISOCode;
			target.TaxTotal = _purchaseOrderToTaxTotal.Map(source);
			//target.Description = 
		}
	}
}
