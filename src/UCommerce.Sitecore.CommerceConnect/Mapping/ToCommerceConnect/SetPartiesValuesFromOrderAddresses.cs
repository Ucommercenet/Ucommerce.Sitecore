using System.Collections.Generic;
using System.Linq;
using Sitecore.Commerce.Entities;
using UCommerce.EntitiesV2;

namespace UCommerce.Sitecore.CommerceConnect.Mapping.ToCommerceConnect
{
	public class SetPartiesValuesFromOrderAddresses : IMapValues<ICollection<OrderAddress>, IList<Party>>
	{
		private readonly IMapping<OrderAddress, Party> _orderAddressToParty;

		public SetPartiesValuesFromOrderAddresses(IMapping<OrderAddress, Party> orderAddressToParty)
		{
			_orderAddressToParty = orderAddressToParty;
		}

		public void MapValues(ICollection<OrderAddress> source, IList<Party> target)
		{
			((List<Party>)target).AddRange(source.Select(orderAddress => _orderAddressToParty.Map(orderAddress)).ToList());
		}
	}
}
