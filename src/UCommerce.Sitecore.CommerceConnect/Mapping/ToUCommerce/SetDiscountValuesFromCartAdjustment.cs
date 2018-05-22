using Sitecore.Commerce.Entities.Carts;
using UCommerce.EntitiesV2;

namespace UCommerce.Sitecore.CommerceConnect.Mapping.ToUCommerce
{
	public class SetDiscountValuesFromCartAdjustment : IMapValues<CartAdjustment, Discount>
	{
		public void MapValues(CartAdjustment source, Discount target)
		{
			target.AmountOffTotal = source.Amount;
			target.Description = source.Description;
		}
	}
}
