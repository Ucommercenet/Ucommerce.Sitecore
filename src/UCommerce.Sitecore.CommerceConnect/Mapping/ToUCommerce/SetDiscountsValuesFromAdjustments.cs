using System.Collections.Generic;
using System.Linq;
using Sitecore.Commerce.Entities.Carts;
using UCommerce.EntitiesV2;

namespace UCommerce.Sitecore.CommerceConnect.Mapping.ToUCommerce
{
	class SetDiscountsValuesFromAdjustments : IMapValues<IList<CartAdjustment>, IList<Discount>>
	{
		private readonly IMapping<CartAdjustment, Discount> _cartAdjustmentToDiscount;

		public SetDiscountsValuesFromAdjustments(IMapping<CartAdjustment, Discount> cartAdjustmentToDiscount)
		{
			_cartAdjustmentToDiscount = cartAdjustmentToDiscount;
		}

		public void MapValues(IList<CartAdjustment> source, IList<Discount> target)
		{
			((List<Discount>)target).AddRange(source.Select(cartAdjustment => _cartAdjustmentToDiscount.Map(cartAdjustment)).ToList());
		}
	}
}
