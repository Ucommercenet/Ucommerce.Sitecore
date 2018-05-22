using System.Collections.Generic;
using System.Linq;
using Sitecore.Commerce.Entities.Carts;
using UCommerce.EntitiesV2;

namespace UCommerce.Sitecore.CommerceConnect.Mapping.ToCommerceConnect
{
	public class SetCartAdjustmentsValuesFromPayments : IMapValues<ICollection<Payment>, IList<CartAdjustment>>
	{
		private readonly IMapping<Payment, CartAdjustment> _paymentToAdjustment;

		public SetCartAdjustmentsValuesFromPayments(IMapping<Payment, CartAdjustment> paymentToAdjustment)
		{
			_paymentToAdjustment = paymentToAdjustment;
		}

		public void MapValues(ICollection<Payment> source, IList<CartAdjustment> target)
		{
			((List<CartAdjustment>)target).AddRange(source.Select(payment => _paymentToAdjustment.Map(payment)).ToList());
		}
	}
}
