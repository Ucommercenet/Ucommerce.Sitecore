using System.Collections.Generic;
using System.Collections.ObjectModel;
using Sitecore.Commerce.Entities;
using UCommerce.EntitiesV2;

namespace UCommerce.Sitecore.CommerceConnect.Mapping.ToCommerceConnect
{
	public class OrderAddressesToParties : IMapping<ICollection<OrderAddress>, IList<Party>>
	{
		private readonly IMapValues<ICollection<OrderAddress>, IList<Party>> _setPartiesValuesFromOrderAddresses;

		public OrderAddressesToParties(IMapValues<ICollection<OrderAddress>, IList<Party>> setPartiesValuesFromOrderAddresses)
		{
			_setPartiesValuesFromOrderAddresses = setPartiesValuesFromOrderAddresses;
		}

		public IList<Party> Map(ICollection<OrderAddress> orderAddresses)
		{
			var parties = new List<Party>();
			_setPartiesValuesFromOrderAddresses.MapValues(orderAddresses, parties);
			return new ReadOnlyCollection<Party>(parties);
		}
	}
}
