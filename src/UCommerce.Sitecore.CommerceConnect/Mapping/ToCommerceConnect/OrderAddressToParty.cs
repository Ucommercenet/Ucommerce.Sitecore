using Sitecore.Commerce.Entities;
using UCommerce.EntitiesV2;

namespace UCommerce.Sitecore.CommerceConnect.Mapping.ToCommerceConnect
{
	public class OrderAddressToParty : IMapping<OrderAddress, Party>
	{
		private readonly IMapValues<OrderAddress, Party> _setPartyValuesFromOrderAddress;

		public OrderAddressToParty(IMapValues<OrderAddress, Party> setPartyValuesFromOrderAddress)
		{
			_setPartyValuesFromOrderAddress = setPartyValuesFromOrderAddress;
		}

		public Party Map(OrderAddress orderAddress)
		{
			var party = new Party();
			_setPartyValuesFromOrderAddress.MapValues(orderAddress, party);
			return party;
		}
	}
}
