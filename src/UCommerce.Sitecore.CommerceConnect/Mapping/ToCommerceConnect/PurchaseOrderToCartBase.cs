using Sitecore.Commerce.Entities.Carts;
using UCommerce.EntitiesV2;

namespace UCommerce.Sitecore.CommerceConnect.Mapping.ToCommerceConnect
{
	public class PurchaseOrderToCartBase : IMapping<PurchaseOrder, CartBase>
	{
		private readonly IMapValues<PurchaseOrder, CartBase> _setCartBaseValuesFromPurchaseOrder;

		public PurchaseOrderToCartBase(IMapValues<PurchaseOrder, CartBase> setCartBaseValuesFromPurchaseOrder)
		{
			_setCartBaseValuesFromPurchaseOrder = setCartBaseValuesFromPurchaseOrder;
		}
		public CartBase Map(PurchaseOrder target)
		{
			var cartBase = new CartBase();
			_setCartBaseValuesFromPurchaseOrder.MapValues(target, cartBase);
			return cartBase;
		}
	}
}
