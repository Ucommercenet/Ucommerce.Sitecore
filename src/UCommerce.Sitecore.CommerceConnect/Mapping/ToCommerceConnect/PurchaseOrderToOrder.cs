using Sitecore.Commerce.Entities.Carts;
using Sitecore.Commerce.Entities.Orders;
using UCommerce.EntitiesV2;

namespace UCommerce.Sitecore.CommerceConnect.Mapping.ToCommerceConnect
{
	public class PurchaseOrderToOrder : IMapping<PurchaseOrder, Order>
	{
		private readonly IMapValues<PurchaseOrder, Order> _setOrderFromPurchaseOrder;
		private readonly IMapValues<PurchaseOrder, CartBase> _setCartBaseValuesFromPurchaseOrder;
		private readonly IMapValues<PurchaseOrder, Cart> _setCartValuesFromPurchaseOrder;

		public PurchaseOrderToOrder(IMapValues<PurchaseOrder, Order> setOrderFromPurchaseOrder, IMapValues<PurchaseOrder, CartBase> setCartBaseValuesFromPurchaseOrder, IMapValues<PurchaseOrder, Cart> setCartValuesFromPurchaseOrder)
		{
			_setOrderFromPurchaseOrder = setOrderFromPurchaseOrder;
			_setCartBaseValuesFromPurchaseOrder = setCartBaseValuesFromPurchaseOrder;
			_setCartValuesFromPurchaseOrder = setCartValuesFromPurchaseOrder;
		}

		public Order Map(PurchaseOrder target)
		{
			var order = new Order();
			_setCartBaseValuesFromPurchaseOrder.MapValues(target, order);
			_setCartValuesFromPurchaseOrder.MapValues(target, order);
			_setOrderFromPurchaseOrder.MapValues(target,order);
			return order;
		}
	}
}
