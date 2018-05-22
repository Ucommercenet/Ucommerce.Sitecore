using System.Collections.Generic;
using System.Collections.ObjectModel;
using Sitecore.Commerce.Entities.Carts;
using UCommerce.EntitiesV2;

namespace UCommerce.Sitecore.CommerceConnect.Mapping.ToCommerceConnect
{
	public class OrderLineToAdjustments : IMapping<OrderLine, IList<CartAdjustment>>
	{
		private readonly IMapValues<ICollection<Discount>, IList<CartAdjustment>> _setCartAdjustmentsValuesFromDiscounts;

		public OrderLineToAdjustments(IMapValues<ICollection<Discount>, IList<CartAdjustment>> setCartAdjustmentsValuesFromDiscounts)
		{
			_setCartAdjustmentsValuesFromDiscounts = setCartAdjustmentsValuesFromDiscounts;
		}

		/*
		 * Adjustments for a CartLine. 
		 */
		public IList<CartAdjustment> Map(OrderLine target)
		{
			var cartAdjustments = new List<CartAdjustment>();
			_setCartAdjustmentsValuesFromDiscounts.MapValues(target.Discounts, cartAdjustments);
			return new ReadOnlyCollection<CartAdjustment>(cartAdjustments);
		}
	}
}
