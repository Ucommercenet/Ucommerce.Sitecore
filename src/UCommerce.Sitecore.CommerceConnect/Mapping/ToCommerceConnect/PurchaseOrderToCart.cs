using Sitecore.Commerce.Entities.Carts;
using UCommerce.EntitiesV2;

namespace UCommerce.Sitecore.CommerceConnect.Mapping.ToCommerceConnect
{
	class PurchaseOrderToCart : IMapping<PurchaseOrder, Cart>
	{
		private readonly IMapValues<PurchaseOrder, CartBase> _setCartBaseValuesFromPurchaseOrder;
		private readonly IMapValues<PurchaseOrder, Cart> _setCartValuesFromPurchaseOrder;

		public PurchaseOrderToCart(IMapValues<PurchaseOrder, CartBase> setCartBaseValuesFromPurchaseOrder, IMapValues<PurchaseOrder, Cart> setCartValuesFromPurchaseOrder)
		{
			_setCartBaseValuesFromPurchaseOrder = setCartBaseValuesFromPurchaseOrder;
			_setCartValuesFromPurchaseOrder = setCartValuesFromPurchaseOrder;
		}

		public Cart Map(PurchaseOrder target)
		{
			var cart = new Cart();
			_setCartBaseValuesFromPurchaseOrder.MapValues(target, cart);
			_setCartValuesFromPurchaseOrder.MapValues(target, cart);
			return cart;
		}
	}
}
