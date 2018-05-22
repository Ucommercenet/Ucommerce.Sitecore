using System.Collections.Generic;
using System.Linq;
using Sitecore.Commerce.Entities.Carts;
using UCommerce.EntitiesV2;

namespace UCommerce.Sitecore.CommerceConnect.Mapping.ToCommerceConnect
{
	public class SetPaymentInfosValuesFromPayments : IMapValues<ICollection<Payment>, IList<PaymentInfo>>
	{
		private readonly IMapping<Payment, PaymentInfo> _paymentToPaymentInfo;

		public SetPaymentInfosValuesFromPayments(IMapping<Payment, PaymentInfo> paymentToPaymentInfo)
		{
			_paymentToPaymentInfo = paymentToPaymentInfo;
		}
		public void MapValues(ICollection<Payment> source, IList<PaymentInfo> target)
		{
			((List<PaymentInfo>)target).AddRange(source.Select(payment => _paymentToPaymentInfo.Map(payment)).ToList());
		}
	}
}
