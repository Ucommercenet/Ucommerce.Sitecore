using System.Collections.Generic;
using System.Linq;
using Sitecore.Commerce.Entities.Carts;
using UCommerce.EntitiesV2;

namespace UCommerce.Sitecore.CommerceConnect.Mapping.ToCommerceConnect
{
	public class SetCartAdjustmentsValuesFromDiscounts : IMapValues<ICollection<Discount>, IList<CartAdjustment>>
	{
		private readonly IMapping<Discount, CartAdjustment> _discountToAdjustment;

		public SetCartAdjustmentsValuesFromDiscounts(IMapping<Discount, CartAdjustment> discountToAdjustment)
		{
			_discountToAdjustment = discountToAdjustment;
		}

		public void MapValues(ICollection<Discount> source, IList<CartAdjustment> target)
		{
			((List<CartAdjustment>)target).AddRange(source.Select(discount => _discountToAdjustment.Map(discount)).ToList());
		}
	}
}
