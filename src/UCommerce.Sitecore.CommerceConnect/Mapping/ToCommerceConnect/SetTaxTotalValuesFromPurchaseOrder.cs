using Sitecore.Commerce.Entities.Prices;
using UCommerce.EntitiesV2;

namespace UCommerce.Sitecore.CommerceConnect.Mapping.ToCommerceConnect
{
	public class SetTaxTotalValuesFromPurchaseOrder : IMapValues<PurchaseOrder, TaxTotal>
	{
		public void MapValues(PurchaseOrder source, TaxTotal target)
		{
			target.Amount = source.TaxTotal.GetValueOrDefault();
			//target.Description = "";
			//target.Id = "";
			//TaxSubtotals =
		}
	}
}
