using System.Collections.ObjectModel;
using System.Linq;
using Sitecore.Commerce.Entities;
using UCommerce.EntitiesV2;
using UCommerce.Sitecore.CommerceConnect.Entities.Carts;

namespace UCommerce.Sitecore.CommerceConnect.Mapping.ToCommerceConnect
{
	public class SetPaymentInfoValuesFromPayment : IMapValues<Payment, PaymentInfoWithAmount>
	{
		private readonly IMapping<OrderAddress, Party> _orderAddressToParty;

		public SetPaymentInfoValuesFromPayment(IMapping<OrderAddress, Party> orderAddressToParty)
		{
			_orderAddressToParty = orderAddressToParty;
		}

		public void MapValues(Payment source, PaymentInfoWithAmount target)
		{
			target.ExternalId = source.Id.ToString();
			target.LineIDs = new ReadOnlyCollection<string>(source.PurchaseOrder.OrderLines.Select(x => x.OrderLineId.ToString()).ToList());
			target.PaymentMethodID = source.PaymentMethod.PaymentMethodId.ToString();
			target.PartyID = _orderAddressToParty.Map(source.PurchaseOrder.BillingAddress).PartyId;
			target.Amount = source.Amount;
		}
	}
}
