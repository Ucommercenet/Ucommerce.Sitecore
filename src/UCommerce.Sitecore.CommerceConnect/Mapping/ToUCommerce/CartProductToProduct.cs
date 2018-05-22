
using Sitecore.Commerce.Entities.Carts;
using UCommerce.EntitiesV2;

namespace UCommerce.Sitecore.CommerceConnect.Mapping.ToUCommerce
{
	public class CartProductToProduct : IMapping<CartProduct, Product>
	{
		private readonly IMapValues<CartProduct, Product> _setProductValuesFromCartProduct;

		public CartProductToProduct(IMapValues<CartProduct, Product> setProductValuesFromCartProduct)
		{
			_setProductValuesFromCartProduct = setProductValuesFromCartProduct;
		}

		public Product Map(CartProduct target)
		{
			var product = new Product();
			_setProductValuesFromCartProduct.MapValues(target, product);
			return product;
		}
	}
}
