using System.Collections.Generic;
using Sitecore.Commerce.Entities.Carts;
using UCommerce.EntitiesV2;

namespace UCommerce.Sitecore.CommerceConnect.Mapping.ToUCommerce
{
	public class AdjustMentsToDiscounts : IMapping<IList<CartAdjustment>, IList<Discount>>
	{
		private readonly IMapValues<IList<CartAdjustment>, IList<Discount>> _setDiscountsValuesFromAdjustments;

		public AdjustMentsToDiscounts(IMapValues<IList<CartAdjustment>, IList<Discount>> setDiscountsValuesFromAdjustments)
		{
			_setDiscountsValuesFromAdjustments = setDiscountsValuesFromAdjustments;
		}

		public IList<Discount> Map(IList<CartAdjustment> target)
		{
			var discounts = new List<Discount>();
			_setDiscountsValuesFromAdjustments.MapValues(target, discounts);
			return discounts;
		}
	}
}
