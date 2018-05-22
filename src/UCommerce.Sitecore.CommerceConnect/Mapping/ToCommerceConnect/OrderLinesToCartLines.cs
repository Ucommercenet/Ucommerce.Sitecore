using System.Collections.Generic;
using System.Collections.ObjectModel;
using Sitecore.Commerce.Entities.Carts;
using UCommerce.EntitiesV2;

namespace UCommerce.Sitecore.CommerceConnect.Mapping.ToCommerceConnect
{
	public class OrderLinesToCartLines : IMapping<ICollection<OrderLine>, IList<CartLine>>
	{
		private readonly IMapValues<ICollection<OrderLine>, IList<CartLine>> _setLinesValuesFromOrderLines;

		public OrderLinesToCartLines(IMapValues<ICollection<OrderLine>, IList<CartLine>> setLinesValuesFromOrderLines)
		{
			_setLinesValuesFromOrderLines = setLinesValuesFromOrderLines;
		}

		public IList<CartLine> Map(ICollection<OrderLine> orderLines)
		{
			var cartLines = new List<CartLine>();
			_setLinesValuesFromOrderLines.MapValues(orderLines, cartLines);
			return new ReadOnlyCollection<CartLine>(cartLines);
		}
	}
}
