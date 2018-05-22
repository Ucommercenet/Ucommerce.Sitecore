using System.Collections.Generic;
using System.Collections.ObjectModel;
using Sitecore.Commerce.Entities.Carts;
using UCommerce.EntitiesV2;

namespace UCommerce.Sitecore.CommerceConnect.Mapping.ToCommerceConnect
{
	public class PurchaseOrderToAdjustments : IMapping<PurchaseOrder, IList<CartAdjustment>>
	{
		private readonly IMapValues<ICollection<Discount>, IList<CartAdjustment>> _setCartAdjustmentsValuesFromDiscounts;
		private readonly IMapValues<ICollection<Shipment>, IList<CartAdjustment>> _setCartAdjustmentsValuesFromShipments;
		private readonly IMapValues<ICollection<Payment>, IList<CartAdjustment>> _setCartAdjustmentsValuesFromPayments;

		public PurchaseOrderToAdjustments(IMapValues<ICollection<Discount>, IList<CartAdjustment>> setCartAdjustmentsValuesFromDiscounts,
			IMapValues<ICollection<Shipment>, IList<CartAdjustment>> setCartAdjustmentsValuesFromShipments,
			IMapValues<ICollection<Payment>, IList<CartAdjustment>> setCartAdjustmentsValuesFromPayments)
		{
			_setCartAdjustmentsValuesFromDiscounts = setCartAdjustmentsValuesFromDiscounts;
			_setCartAdjustmentsValuesFromShipments = setCartAdjustmentsValuesFromShipments;
			_setCartAdjustmentsValuesFromPayments = setCartAdjustmentsValuesFromPayments;
		}

		public IList<CartAdjustment> Map(PurchaseOrder target)
		{
			var cartAdjustments = new List<CartAdjustment>();
			_setCartAdjustmentsValuesFromDiscounts.MapValues(target.Discounts, cartAdjustments);
			_setCartAdjustmentsValuesFromShipments.MapValues(target.Shipments, cartAdjustments);
			_setCartAdjustmentsValuesFromPayments.MapValues(target.Payments, cartAdjustments);
			return new ReadOnlyCollection<CartAdjustment>(cartAdjustments);
		}
	}
}
