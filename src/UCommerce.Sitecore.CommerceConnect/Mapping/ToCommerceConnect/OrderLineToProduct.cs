using Sitecore.Commerce.Entities.Carts;
using UCommerce.EntitiesV2;

namespace UCommerce.Sitecore.CommerceConnect.Mapping.ToCommerceConnect
{
	public class OrderLineToProduct : IMapping<OrderLine, CartProduct>
	{
		private readonly IMapValues<OrderLine, CartProduct> _setCartProductValuesFromOrderLine;
		public OrderLineToProduct(IMapValues<OrderLine, CartProduct> setCartProductValuesFromOrderLine)
		{
			_setCartProductValuesFromOrderLine = setCartProductValuesFromOrderLine;
		}

		public CartProduct Map(OrderLine orderLine)
		{
			var cartProduct = new CartProduct();
			_setCartProductValuesFromOrderLine.MapValues(orderLine, cartProduct);
			return cartProduct;
		}
	}
}
