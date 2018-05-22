using Sitecore.Commerce.Entities.Prices;
using UCommerce.EntitiesV2;

namespace UCommerce.Sitecore.CommerceConnect.Mapping.ToCommerceConnect
{
	public class SetTotalValuesFromOrderLine : IMapValues<OrderLine, Total>
	{
		private readonly IMapping<OrderLine, TaxTotal> _orderLineToTaxTotal;

		public SetTotalValuesFromOrderLine(IMapping<OrderLine, TaxTotal> orderLineToTaxTotal)
		{
			_orderLineToTaxTotal = orderLineToTaxTotal;
		}

		public void MapValues(OrderLine source, Total target)
		{
			target.Amount = source.Total.GetValueOrDefault();
			if (source.PurchaseOrder.BillingCurrency != null) target.CurrencyCode = source.PurchaseOrder.BillingCurrency.ISOCode;
			target.TaxTotal = _orderLineToTaxTotal.Map(source);
		}
	}
}
