using Sitecore.Commerce.Entities.Prices;
using UCommerce.EntitiesV2;
using Price = Sitecore.Commerce.Entities.Prices.Price;

namespace UCommerce.Sitecore.CommerceConnect.Mapping.ToCommerceConnect
{
	public class OrderLineToPrice : IMapping<OrderLine, Price>
	{
		private readonly IMapValues<OrderLine, Price> _setPriceValuesFromOrderLine;

		public OrderLineToPrice(IMapValues<OrderLine, Price> setPriceValuesFromOrderLine)
		{
			_setPriceValuesFromOrderLine = setPriceValuesFromOrderLine;
		}

		public Price Map(OrderLine orderLine)
		{
			var price = new Price();
			_setPriceValuesFromOrderLine.MapValues(orderLine, price);
			return price;
		}
	}
}
