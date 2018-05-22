using System.Collections.Generic;
using System.Collections.ObjectModel;
using Sitecore.Commerce.Entities.Carts;
using UCommerce.EntitiesV2;

namespace UCommerce.Sitecore.CommerceConnect.Mapping.ToCommerceConnect
{
	public class ShipmentsToShipping : IMapping<ICollection<Shipment>, IList<ShippingInfo>>
	{
		private readonly IMapValues<ICollection<Shipment>, IList<ShippingInfo>> _setShippingInfosValuesFromShipments;

		public ShipmentsToShipping(IMapValues<ICollection<Shipment>, IList<ShippingInfo>> setShippingInfosValuesFromShipments)
		{
			_setShippingInfosValuesFromShipments = setShippingInfosValuesFromShipments;
		}

		public IList<ShippingInfo> Map(ICollection<Shipment> shipments)
		{
			var shippingInfos = new List<ShippingInfo>();
			_setShippingInfosValuesFromShipments.MapValues(shipments, shippingInfos);
			return new ReadOnlyCollection<ShippingInfo>(shippingInfos);
		}
	}
}
