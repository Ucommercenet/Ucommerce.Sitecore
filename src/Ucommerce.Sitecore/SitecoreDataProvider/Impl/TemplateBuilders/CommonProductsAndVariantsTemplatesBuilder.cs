using System.Linq;
using Ucommerce.EntitiesV2;
using Ucommerce.EntitiesV2.Queries.Catalog;
using Ucommerce.Infrastructure;

namespace Ucommerce.Sitecore.SitecoreDataProvider.Impl.TemplateBuilders
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
