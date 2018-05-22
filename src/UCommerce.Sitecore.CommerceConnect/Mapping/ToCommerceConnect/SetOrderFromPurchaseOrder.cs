using Sitecore.Commerce.Entities.Orders;
using UCommerce.EntitiesV2;

namespace UCommerce.Sitecore.CommerceConnect.Mapping.ToCommerceConnect
{
	class SetOrderFromPurchaseOrder : IMapValues<PurchaseOrder, Order>
	{
		public void MapValues(PurchaseOrder source, Order target)
		{
			target.ExternalId = source.OrderGuid.ToString();
			target.OrderID = source.OrderId.ToString();
        }
	}
}
