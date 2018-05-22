using System.Collections.Generic;
using System.Collections.ObjectModel;
using Sitecore.Commerce.Entities.Carts;
using Sitecore.Commerce.Entities.Prices;
using UCommerce.EntitiesV2;
using Price = Sitecore.Commerce.Entities.Prices.Price;

namespace UCommerce.Sitecore.CommerceConnect.Mapping.ToCommerceConnect
{
	public class SetCartProductValuesFromOrderLine : IMapValues<OrderLine, CartProduct>
	{
		private readonly IMapping<OrderLine, IList<CartAdjustment>> _orderLineToAdjustments;
		private readonly IMapping<OrderLine, Price> _orderLineToPrice;

		public SetCartProductValuesFromOrderLine(IMapping<OrderLine, IList<CartAdjustment>> orderLineToAdjustments, 
			IMapping<OrderLine, Price> orderLineToPrice)
		{
			_orderLineToAdjustments = orderLineToAdjustments;
			_orderLineToPrice = orderLineToPrice;
		}

		public void MapValues(OrderLine source, CartProduct target)
		{
			target.Adjustments = new ReadOnlyCollection<CartAdjustment>(_orderLineToAdjustments.Map(source));
			target.ProductId = source.Sku;

            target.Price = _orderLineToPrice.Map(source);

            if (!string.IsNullOrEmpty(source.VariantSku))
            {
                target.Properties.Add("VariantSku", source.VariantSku);
            }

            var productNameProperty = target.GetType().GetProperty("ProductName");

            if (productNameProperty != null)
            {
                productNameProperty.SetValue(target, source.ProductName, null);
            }
        }
	}
}
