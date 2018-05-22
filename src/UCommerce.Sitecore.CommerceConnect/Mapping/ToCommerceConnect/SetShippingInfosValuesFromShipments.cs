using System.Collections.Generic;
using System.Linq;
using Sitecore.Commerce.Entities.Carts;
using UCommerce.EntitiesV2;

namespace UCommerce.Sitecore.CommerceConnect.Mapping.ToCommerceConnect
{
	public class SetShippingInfosValuesFromShipments : IMapValues<ICollection<Shipment>, IList<ShippingInfo>>
	{
		private readonly IMapping<Shipment, ShippingInfo> _shipmentToShippingInfo;

		public SetShippingInfosValuesFromShipments(IMapping<Shipment, ShippingInfo> shipmentToShippingInfo)
		{
			_shipmentToShippingInfo = shipmentToShippingInfo;
		}

		public void MapValues(ICollection<Shipment> source, IList<ShippingInfo> target)
		{
			((List<ShippingInfo>)target).AddRange(source.Select(shipment => _shipmentToShippingInfo.Map(shipment)).ToList());
		}
	}
}
