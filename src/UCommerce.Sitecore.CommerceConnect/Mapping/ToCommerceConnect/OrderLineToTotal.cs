using Sitecore.Commerce.Entities.Prices;
using UCommerce.EntitiesV2;

namespace UCommerce.Sitecore.CommerceConnect.Mapping.ToCommerceConnect
{
	public class OrderLineToTotal : IMapping<OrderLine, Total>
	{
		private readonly IMapValues<OrderLine, Total> _setTotalValuesFromOrderLine;

		public OrderLineToTotal(IMapValues<OrderLine, Total> setTotalValuesFromOrderLine)
		{
			_setTotalValuesFromOrderLine = setTotalValuesFromOrderLine;
		}

		public Total Map(OrderLine orderLine)
		{
			var total = new Total();
			_setTotalValuesFromOrderLine.MapValues(orderLine, total);
			return total;
		}
	}
}
