using Sitecore.Commerce.Entities.Carts;
using UCommerce.EntitiesV2;

namespace UCommerce.Sitecore.CommerceConnect.Mapping.ToCommerceConnect
{
	public class DiscountToAdjustment : IMapping<Discount, CartAdjustment>
	{
		private readonly IMapValues<Discount, CartAdjustment> _setCartAdjustmentValuesFromDiscount;

		public DiscountToAdjustment(IMapValues<Discount, CartAdjustment> setCartAdjustmentValuesFromDiscount)
		{
			_setCartAdjustmentValuesFromDiscount = setCartAdjustmentValuesFromDiscount;
		}

		public CartAdjustment Map(Discount discount)
		{
			var cartAdjustment = new CartAdjustment();
			_setCartAdjustmentValuesFromDiscount.MapValues(discount, cartAdjustment);
			return cartAdjustment;
		}
	}
}
