using Sitecore.Commerce.Entities.Prices;
using UCommerce.EntitiesV2;
using Price = Sitecore.Commerce.Entities.Prices.Price;

namespace UCommerce.Sitecore.CommerceConnect.Mapping.ToCommerceConnect
{
	public class SetPriceValuesFromOrderLine : IMapValues<OrderLine, Price>
	{
		public void MapValues(OrderLine source, Price target)
		{
			target.Amount = source.Price;
			target.PriceType = "List Price"; //or "Customer Price"
			target.CurrencyCode = source.PurchaseOrder.BillingCurrency.ISOCode;
			//Description = 
			//Conditions =  Used for break pricing and campaigns, where a specific price is only good when certain conditions are met (the customer has bought at least 5 products or the date is in the year 2013).
		}
	}
}
