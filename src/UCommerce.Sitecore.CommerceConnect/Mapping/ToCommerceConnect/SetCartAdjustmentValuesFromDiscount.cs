using Sitecore.Commerce.Entities.Carts;
using UCommerce.EntitiesV2;

namespace UCommerce.Sitecore.CommerceConnect.Mapping.ToCommerceConnect
{
	public class SetCartAdjustmentValuesFromDiscount : IMapValues<Discount, CartAdjustment>
	{
		public void MapValues(Discount source, CartAdjustment target)
		{
			target.Amount = source.AmountOffTotal;
			target.Description = source.Description;
			target.IsCharge = false; //if false = discount.
			//LineNumber =  used for sorting adjustments.
			//Percentage = 0 we can only use either Amount or Percentage
		}
	}
}
