using Sitecore.Commerce.Entities.Prices;
using UCommerce.EntitiesV2;

namespace UCommerce.Sitecore.CommerceConnect.Mapping.ToCommerceConnect
{
	public class PurchaseOrderToTotal : IMapping<PurchaseOrder, Total>
	{
		private readonly IMapValues<PurchaseOrder, Total> _setTotalValuesFromPurchaseOrder;

		public PurchaseOrderToTotal(IMapValues<PurchaseOrder, Total> setTotalValuesFromPurchaseOrder)
		{
			_setTotalValuesFromPurchaseOrder = setTotalValuesFromPurchaseOrder;
		}

		public Total Map(PurchaseOrder purchaseOrder)
		{
			var total = new Total();
			_setTotalValuesFromPurchaseOrder.MapValues(purchaseOrder, total);
			return total;
		}
	}
}
