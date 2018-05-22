using Sitecore.Commerce.Entities.Carts;
using UCommerce.EntitiesV2;

namespace UCommerce.Sitecore.CommerceConnect.Mapping.ToUCommerce
{
	public class SetProductValuesFromCartProduct : IMapValues<CartProduct, Product>
	{
		public void MapValues(CartProduct source, Product target)
		{
			target.Sku = source.ProductId;

			if (source.GetPropertyValue("VariantSku") != null)
			{
				target.VariantSku = source.GetPropertyValue("VariantSku").ToString();
			}
		}
	}
}
