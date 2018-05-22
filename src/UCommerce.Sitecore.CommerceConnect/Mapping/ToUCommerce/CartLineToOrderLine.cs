using Sitecore.Commerce.Entities.Carts;
using UCommerce.EntitiesV2;

namespace UCommerce.Sitecore.CommerceConnect.Mapping.ToUCommerce
{
	public class CartLineToOrderLine : IMapping<CartLine, OrderLine>
	{
		private readonly IMapValues<CartLine, OrderLine> _setOrderLineValuesFromCartLine;

		public CartLineToOrderLine(IMapValues<CartLine, OrderLine> setOrderLineValuesFromCartLine)
		{
			_setOrderLineValuesFromCartLine = setOrderLineValuesFromCartLine;
		}

		public OrderLine Map(CartLine target)
		{
			var orderLine = new OrderLine();
			_setOrderLineValuesFromCartLine.MapValues(target, orderLine);
			return orderLine;
		}
	}
}
