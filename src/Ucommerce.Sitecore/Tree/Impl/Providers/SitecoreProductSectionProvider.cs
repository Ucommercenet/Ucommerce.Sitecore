using System.Collections.Generic;
using System.Linq;
using Ucommerce.EntitiesV2;
using Ucommerce.EntitiesV2.Queries.Catalog;
using Ucommerce.Security;
using Ucommerce.Sitecore.SitecoreDataProvider;
using Ucommerce.Tree;
using Ucommerce.Tree.Impl.Providers;

namespace Ucommerce.Sitecore.Tree.Impl.Providers
{
	public class SitecoreProductSectionProvider : ProductSectionProvider
	{
		private readonly IRepository<ProductTreeView> _productTreeViewRepository;
		private readonly IDetectFullCategoryScan _fullCategoryScanDetecter;

		private static volatile Dictionary<int, List<ProductTreeView>> _allProductsForCategory;
		private static volatile Dictionary<int, List<ProductTreeView>> _allVariantsForProduct;

		private static readonly object _categoryLock = new object();
		private static readonly object _variantLock = new object();

		public SitecoreProductSectionProvider(IUserService userService, IRepository<ProductTreeView> productTreeViewRepository, IDetectFullCategoryScan fullCategoryScanDetecter)
			: base(userService, productTreeViewRepository)
		{
			_productTreeViewRepository = productTreeViewRepository;
			_fullCategoryScanDetecter = fullCategoryScanDetecter;
		}

		protected override IEnumerable<ProductTreeView> GetProductsForCategory(int categoryId)
		{
			FillOrEmptyTheProductsForCategoryCache();

			if (_fullCategoryScanDetecter.ThreadIsScanningFullCatalog && _allProductsForCategory != null)
			{
				if (_allProductsForCategory.ContainsKey(categoryId))
				{
					return _allProductsForCategory[categoryId];
				}
				return new List<ProductTreeView>(); // Miss in cache indicates that
			}

			return base.GetProductsForCategory(categoryId);
		}

		private void FillOrEmptyTheProductsForCategoryCache()
		{
			if (_fullCategoryScanDetecter.ThreadIsScanningFullCatalog && _allProductsForCategory == null)
			{
				// Hydrate cache
				lock (_categoryLock)
				{
					if (_allProductsForCategory == null)
					{
						var allProducts = _productTreeViewRepository.Select(new AllProductTreeViewQuery()).ToList();
						_allProductsForCategory = allProducts
							.GroupBy(p => p.ParentId)
							.ToDictionary(g => g.Key, g => g.ToList());
					}
				}
			}

			if (!_fullCategoryScanDetecter.FullCatalogScanInProgress && _allProductsForCategory != null)
			{
				// Clear cache
				_allProductsForCategory = null;
			}
		}

		protected override IEnumerable<ProductTreeView> GetVariantsForProducts(int productId)
		{
			FillOrEmptyCacheVariantsForProducts();

			if (_fullCategoryScanDetecter.ThreadIsScanningFullCatalog && _allVariantsForProduct != null)
			{
				if (_allVariantsForProduct.ContainsKey(productId))
				{
					return _allVariantsForProduct[productId];
				}
				return new List<ProductTreeView>(); // Miss indicates that the product does not have any variants.
			}

			return base.GetVariantsForProducts(productId);
		}

		private void FillOrEmptyCacheVariantsForProducts()
		{
			if (_fullCategoryScanDetecter.ThreadIsScanningFullCatalog && _allVariantsForProduct == null)
			{
				// Hydrate cache
				lock (_variantLock)
				{
					if (_allVariantsForProduct == null)
					{
						var allVariants = _productTreeViewRepository.Select(new AllProductVariantTreeViewQuery()).ToList();

						_allVariantsForProduct = allVariants
							.GroupBy(v => v.ParentId)
							.ToDictionary(g => g.Key, g => g.ToList());
					}
				}
			}


			if (!_fullCategoryScanDetecter.FullCatalogScanInProgress && _allVariantsForProduct != null)
			{
				// Clear cache
				_allVariantsForProduct = null;
			}
		}

		protected override IList<ITreeNodeContent> BuildProductItems(int categoryId)
		{
			return new List<ITreeNodeContent>();
			//if (_fullCategoryScanDetecter.ThreadIsScanningFullCatalog) return new List<ITreeNodeContent>();
			//return base.BuildProductItems(categoryId);
		}

		protected override IList<ITreeNodeContent> BuildProductVariants(int productId)
		{
			return new List<ITreeNodeContent>();
			//if (_fullCategoryScanDetecter.ThreadIsScanningFullCatalog) return new List<ITreeNodeContent>();
			//return base.BuildProductVariants(productId);
		}
	}
}
