using Sitecore.Commerce.Entities.Prices;
using UCommerce.EntitiesV2;

namespace UCommerce.Sitecore.CommerceConnect.Mapping.ToCommerceConnect
{
	public class PurchaseOrderToTaxTotal : IMapping<PurchaseOrder, TaxTotal>
	{
		private readonly IMapValues<PurchaseOrder, TaxTotal> _setTaxTotalValuesFromPurchaseOrder;

		public PurchaseOrderToTaxTotal(IMapValues<PurchaseOrder, TaxTotal> setTaxTotalValuesFromPurchaseOrder)
		{
			_setTaxTotalValuesFromPurchaseOrder = setTaxTotalValuesFromPurchaseOrder;
		}

		public TaxTotal Map(PurchaseOrder purchaseOrder)
		{
			var taxTotal = new TaxTotal();
			_setTaxTotalValuesFromPurchaseOrder.MapValues(purchaseOrder, taxTotal);
			return taxTotal;
		}
	}
}
