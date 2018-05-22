using Sitecore.Commerce.Entities.Carts;
using UCommerce.EntitiesV2;

namespace UCommerce.Sitecore.CommerceConnect.Mapping.ToCommerceConnect
{
	public class PaymentToAdjustment : IMapping<Payment, CartAdjustment>
	{
		private readonly IMapValues<Payment, CartAdjustment> _setCartAdjustmentValuesFromPayment;

		public PaymentToAdjustment(IMapValues<Payment, CartAdjustment> setCartAdjustmentValuesFromPayment)
		{
			_setCartAdjustmentValuesFromPayment = setCartAdjustmentValuesFromPayment;
		}

		public CartAdjustment Map(Payment target)
		{
			var cartAdjustment = new CartAdjustment();
			_setCartAdjustmentValuesFromPayment.MapValues(target, cartAdjustment);
			return cartAdjustment;
		}
	}
}
