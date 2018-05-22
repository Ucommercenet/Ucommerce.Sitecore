using System.Linq;
using UCommerce.EntitiesV2;
using UCommerce.EntitiesV2.Queries.Catalog;
using UCommerce.Infrastructure;

namespace UCommerce.Sitecore.SitecoreDataProvider.Impl.TemplateBuilders
{
	public class CommonProductsAndVariantsTemplatesBuilder
	{
		protected Product ReadProductFromDatabase(int productId)
		{
			Product currentProduct = null;

			var productRepository = ObjectFactory.Instance.Resolve<IRepository<Product>>();
			currentProduct = productRepository.Select(new SingleProductQuery(productId)).FirstOrDefault();

			return currentProduct;
		}
	}
}
