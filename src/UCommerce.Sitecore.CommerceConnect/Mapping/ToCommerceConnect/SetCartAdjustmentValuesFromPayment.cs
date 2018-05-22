using System;
using Sitecore.Commerce.Entities.Carts;
using UCommerce.EntitiesV2;

namespace UCommerce.Sitecore.CommerceConnect.Mapping.ToCommerceConnect
{
	public class SetCartAdjustmentValuesFromPayment : IMapValues<Payment, CartAdjustment>
	{
		public void MapValues(Payment source, CartAdjustment target)
		{
			target.Amount = Convert.ToDecimal(source.FeeTotal);
			target.Description = source.PaymentMethodName;
			target.IsCharge = false; //if false = discount.
			//LineNumber =  used for sorting adjustments.
			//Percentage = 0 we can only use either Amount or Percentage
		}
	}
}
