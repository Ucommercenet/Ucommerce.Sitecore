using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Sitecore.Commerce.Entities.Carts;
using Sitecore.Commerce.Entities.Prices;
using UCommerce.EntitiesV2;

namespace UCommerce.Sitecore.CommerceConnect.Mapping.ToCommerceConnect
{
	public class SetCartLineValuesFromOrderLine : IMapValues<OrderLine, CartLine>
	{
		private readonly IMapping<OrderLine, IList<CartAdjustment>> _orderLineToAdjustments;
		private readonly IMapping<OrderLine, CartProduct> _orderLineToProduct;
		private readonly IMapping<OrderLine, Total> _orderLineToTotal;

		public SetCartLineValuesFromOrderLine(IMapping<OrderLine, IList<CartAdjustment>> orderLineToAdjustments,
			IMapping<OrderLine, CartProduct> orderLineToProduct, IMapping<OrderLine, Total> orderLineToTotal)
		{
			_orderLineToAdjustments = orderLineToAdjustments;
			_orderLineToProduct = orderLineToProduct;
			_orderLineToTotal = orderLineToTotal;
		}

		public void MapValues(OrderLine source, CartLine target)
		{
			target.ExternalCartLineId = source.OrderLineId.ToString();
			target.Adjustments = new ReadOnlyCollection<CartAdjustment>(_orderLineToAdjustments.Map(source)); 
			target.Product = _orderLineToProduct.Map(source);
			target.Quantity = Convert.ToUInt32(source.Quantity);
			target.Total = _orderLineToTotal.Map(source);
			//SubLines = 
		}
	}
}
