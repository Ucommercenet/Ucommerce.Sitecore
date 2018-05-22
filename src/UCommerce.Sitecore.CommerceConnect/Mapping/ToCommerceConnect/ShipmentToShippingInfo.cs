using Sitecore.Commerce.Entities.Carts;
using UCommerce.EntitiesV2;

namespace UCommerce.Sitecore.CommerceConnect.Mapping.ToCommerceConnect
{
	public class ShipmentToShippingInfo : IMapping<Shipment, ShippingInfo>
	{
		private readonly IMapValues<Shipment, ShippingInfo> _setShippingInfoValuesFromShipment;

		public ShipmentToShippingInfo(IMapValues<Shipment, ShippingInfo> setShippingInfoValuesFromShipment)
		{
			_setShippingInfoValuesFromShipment = setShippingInfoValuesFromShipment;
		}

		public ShippingInfo Map(Shipment shipment)
		{
			var shippingInfo = new ShippingInfo();
			_setShippingInfoValuesFromShipment.MapValues(shipment, shippingInfo);
			return shippingInfo;
		}
	}
}
