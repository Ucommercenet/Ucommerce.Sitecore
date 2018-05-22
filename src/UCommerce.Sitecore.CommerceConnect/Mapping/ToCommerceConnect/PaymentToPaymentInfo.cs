using UCommerce.EntitiesV2;
using UCommerce.Sitecore.CommerceConnect.Entities.Carts;

namespace UCommerce.Sitecore.CommerceConnect.Mapping.ToCommerceConnect
{
	public class PaymentToPaymentInfo : IMapping<Payment, PaymentInfoWithAmount>
	{
		private readonly IMapValues<Payment, PaymentInfoWithAmount> _setPaymentInfoValuesFromPayment;

		public PaymentToPaymentInfo(IMapValues<Payment, PaymentInfoWithAmount> setPaymentInfoValuesFromPayment)
		{
			_setPaymentInfoValuesFromPayment = setPaymentInfoValuesFromPayment;
		}

		public PaymentInfoWithAmount Map(Payment payment)
		{
			var paymentInfo = new PaymentInfoWithAmount();
			_setPaymentInfoValuesFromPayment.MapValues(payment, paymentInfo);
			return paymentInfo;
		}
	}
}
