using System.Collections.Generic;
using System.Collections.ObjectModel;
using Sitecore.Commerce.Entities.Carts;
using UCommerce.EntitiesV2;

namespace UCommerce.Sitecore.CommerceConnect.Mapping.ToCommerceConnect
{
	public class PaymentsToPayment : IMapping<ICollection<Payment>, IList<PaymentInfo>>
	{
		private readonly IMapValues<ICollection<Payment>, IList<PaymentInfo>> _setPaymentInfosValuesFromPayments;

		public PaymentsToPayment(IMapValues<ICollection<Payment>, IList<PaymentInfo>> setPaymentInfosValuesFromPayments)
		{
			_setPaymentInfosValuesFromPayments = setPaymentInfosValuesFromPayments;
		}

		public IList<PaymentInfo> Map(ICollection<Payment> payments)
		{
			var paymentInfos = new List<PaymentInfo>();
			_setPaymentInfosValuesFromPayments.MapValues(payments, paymentInfos);
			return new ReadOnlyCollection<PaymentInfo>(paymentInfos);
		}
	}
}
