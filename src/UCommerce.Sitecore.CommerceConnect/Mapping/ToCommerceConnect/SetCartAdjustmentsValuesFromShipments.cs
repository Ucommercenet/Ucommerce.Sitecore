using System.Collections.Generic;
using System.Linq;
using Sitecore.Commerce.Entities.Carts;
using UCommerce.EntitiesV2;

namespace UCommerce.Sitecore.CommerceConnect.Mapping.ToCommerceConnect
{
	public class SetCartAdjustmentsValuesFromShipments : IMapValues<ICollection<Shipment>, IList<CartAdjustment>>
	{
		private readonly IMapping<Shipment, CartAdjustment> _shipmentToAdjustment;

		public SetCartAdjustmentsValuesFromShipments(IMapping<Shipment, CartAdjustment> shipmentToAdjustment)
		{
			_shipmentToAdjustment = shipmentToAdjustment;
		}

		public void MapValues(ICollection<Shipment> source, IList<CartAdjustment> target)
		{
			((List<CartAdjustment>)target).AddRange(source.Select(shipment => _shipmentToAdjustment.Map(shipment)).ToList());
		}
	}
}
