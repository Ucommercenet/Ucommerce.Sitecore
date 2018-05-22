using System;
using Sitecore.Commerce.Entities.Carts;
using UCommerce.EntitiesV2;

namespace UCommerce.Sitecore.CommerceConnect.Mapping.ToCommerceConnect
{
	public class SetCartAdjustmentValuesFromShipment : IMapValues<Shipment, CartAdjustment>
	{
		public void MapValues(Shipment source, CartAdjustment target)
		{
			target.Amount = Convert.ToDecimal(source.ShipmentPrice);
			target.Description = source.ShipmentName;
			target.IsCharge = false; //if false = discount.
			//LineNumber =  used for sorting adjustments.
			//Percentage = 0 we can only use either Amount or Percentage
		}
	}
}
