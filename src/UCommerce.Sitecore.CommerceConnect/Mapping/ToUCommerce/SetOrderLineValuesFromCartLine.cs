using System.Collections.Generic;
using Sitecore.Commerce.Entities.Carts;
using UCommerce.EntitiesV2;
using UCommerce.Infrastructure;

namespace UCommerce.Sitecore.CommerceConnect.Mapping.ToUCommerce
{
    public class SetOrderLineValuesFromCartLine : IMapValues<CartLine, OrderLine>
    {
        private readonly IMapping<IList<CartAdjustment>, IList<Discount>> _adjustMentsToDiscounts;

        public SetOrderLineValuesFromCartLine(IMapping<IList<CartAdjustment>, IList<Discount>> adjustMentsToDiscounts)
        {
            _adjustMentsToDiscounts = adjustMentsToDiscounts;
        }

        public void MapValues(CartLine source, OrderLine target)
        {
            target.Discounts = _adjustMentsToDiscounts.Map(source.Adjustments);
            target.Total = source.Total.Amount;
            target.Quantity = (int)source.Quantity;

            if (source.Product != null)
            {
                var cartProduct = source.Product;

                target.Sku = cartProduct.ProductId;
                target.Price = cartProduct.Price.Amount;
                target.VariantSku = cartProduct.Properties.GetPropertyOrDefault<string>("VariantSku", "");

                var productNameProperty = cartProduct.GetType().GetProperty("ProductName");

                if (productNameProperty != null)
                {
                    target.ProductName = (string)productNameProperty.GetValue(cartProduct);
                }
            }
        }
    }
}
