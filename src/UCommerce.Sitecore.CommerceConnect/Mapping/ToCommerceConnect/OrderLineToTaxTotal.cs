using Sitecore.Commerce.Entities.Prices;
using UCommerce.EntitiesV2;

namespace UCommerce.Sitecore.CommerceConnect.Mapping.ToCommerceConnect
{
	public class OrderLineToTaxTotal : IMapping<OrderLine, TaxTotal>
	{
		private readonly IMapValues<OrderLine, TaxTotal> _setTaxTotalValuesFromOrderLine;

		public OrderLineToTaxTotal(IMapValues<OrderLine, TaxTotal> setTaxTotalValuesFromOrderLine)
		{
			_setTaxTotalValuesFromOrderLine = setTaxTotalValuesFromOrderLine;
		}

		public TaxTotal Map(OrderLine orderLine)
		{
			var total = new TaxTotal();
			_setTaxTotalValuesFromOrderLine.MapValues(orderLine, total);
			return total;
		}
	}
}
