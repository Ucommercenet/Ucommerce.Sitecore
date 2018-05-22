using Sitecore.Commerce.Entities.Carts;
using UCommerce.EntitiesV2;

namespace UCommerce.Sitecore.CommerceConnect.Mapping.ToCommerceConnect
{
	public class OrderLineToCartLine : IMapping<OrderLine, CartLine>
	{
		private readonly IMapValues<OrderLine, CartLine> _setCartLineValuesFromOrderLine;

		public OrderLineToCartLine(IMapValues<OrderLine, CartLine> setCartLineValuesFromOrderLine)
		{
			_setCartLineValuesFromOrderLine = setCartLineValuesFromOrderLine;
		}

		public CartLine Map(OrderLine orderLine)
		{
			var cartLine = new CartLine();
			_setCartLineValuesFromOrderLine.MapValues(orderLine, cartLine);
			return cartLine;
		}
	}
}
