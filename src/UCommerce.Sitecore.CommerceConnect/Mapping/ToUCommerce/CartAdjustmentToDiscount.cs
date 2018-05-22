using Sitecore.Commerce.Entities.Carts;
using UCommerce.EntitiesV2;

namespace UCommerce.Sitecore.CommerceConnect.Mapping.ToUCommerce
{
	public class CartAdjustmentToDiscount : IMapping<CartAdjustment, Discount>
	{
		private readonly IMapValues<CartAdjustment, Discount> _setDiscountValuesFromCartAdjustment;

		public CartAdjustmentToDiscount(IMapValues<CartAdjustment, Discount> setDiscountValuesFromCartAdjustment)
		{
			_setDiscountValuesFromCartAdjustment = setDiscountValuesFromCartAdjustment;
		}

		public Discount Map(CartAdjustment target)
		{
			var discount = new Discount();
			_setDiscountValuesFromCartAdjustment.MapValues(target, discount);
			return discount;
		}
	}
}
