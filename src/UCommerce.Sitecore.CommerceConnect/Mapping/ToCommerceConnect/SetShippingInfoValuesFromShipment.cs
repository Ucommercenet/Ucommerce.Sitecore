using System.Collections.ObjectModel;
using System.Linq;
using Sitecore.Commerce.Entities.Carts;
using UCommerce.EntitiesV2;

namespace UCommerce.Sitecore.CommerceConnect.Mapping.ToCommerceConnect
{
	public class SetShippingInfoValuesFromShipment : IMapValues<Shipment, ShippingInfo>
	{
		public void MapValues(Shipment source, ShippingInfo target)
		{
			target.ExternalId = source.Id.ToString();
			target.LineIDs = new ReadOnlyCollection<string>(source.OrderLines.Select(x => x.OrderLineId.ToString()).ToList()); 
			target.PartyID = source.ShipmentAddress.Id.ToString();
			target.ShippingMethodID = source.ShippingMethod.ShippingMethodId.ToString();
			target.ShippingProviderID = source.ShippingMethod.ShippingMethodId.ToString();
		}
	}
}
