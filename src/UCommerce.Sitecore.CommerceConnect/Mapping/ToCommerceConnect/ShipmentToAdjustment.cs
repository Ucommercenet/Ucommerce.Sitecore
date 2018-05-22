using Sitecore.Commerce.Entities.Carts;
using UCommerce.EntitiesV2;

namespace UCommerce.Sitecore.CommerceConnect.Mapping.ToCommerceConnect
{
	public class ShipmentToAdjustment : IMapping<Shipment, CartAdjustment>
	{
		private readonly IMapValues<Shipment, CartAdjustment> _setCartAdjustmentValuesFromShipment;

		public ShipmentToAdjustment(IMapValues<Shipment, CartAdjustment> setCartAdjustmentValuesFromShipment)
		{
			_setCartAdjustmentValuesFromShipment = setCartAdjustmentValuesFromShipment;
		}

		public CartAdjustment Map(Shipment target)
		{
			var cartAdjustment = new CartAdjustment();
			_setCartAdjustmentValuesFromShipment.MapValues(target, cartAdjustment);
			return cartAdjustment;
		}
	}
}
