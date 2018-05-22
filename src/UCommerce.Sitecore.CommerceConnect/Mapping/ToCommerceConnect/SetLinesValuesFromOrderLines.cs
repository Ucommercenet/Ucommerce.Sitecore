using System.Collections.Generic;
using System.Linq;
using Sitecore.Commerce.Entities.Carts;
using UCommerce.EntitiesV2;

namespace UCommerce.Sitecore.CommerceConnect.Mapping.ToCommerceConnect
{
	public class SetLinesValuesFromOrderLines : IMapValues<ICollection<OrderLine>, IList<CartLine>>
	{
		private readonly IMapping<OrderLine, CartLine> _orderLineToCartLine;
		private readonly ISetOrderLineNumbersService _mappingService;

		public SetLinesValuesFromOrderLines(IMapping<OrderLine, CartLine> orderLineToCartLine, ISetOrderLineNumbersService mappingService)
		{
			_orderLineToCartLine = orderLineToCartLine;
			_mappingService = mappingService;
		}

		public void MapValues(ICollection<OrderLine> source, IList<CartLine> target)
		{
			((List<CartLine>)target).AddRange(source.Select(orderLine => _orderLineToCartLine.Map(orderLine)).ToList());

			_mappingService.SetOrderLineNumbersForCartLines(target);
		}
	}
}
