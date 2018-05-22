using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UCommerce.EntitiesV2;
using UCommerce.EntitiesV2.Queries.Catalog;
using UCommerce.Security;
using UCommerce.Sitecore.SitecoreDataProvider;
using UCommerce.Tree.Impl.Providers;

namespace UCommerce.Sitecore.Tree.Impl.Providers
{
	public class SitecoreCatalogSectionProvider : CatalogSectionProvider
	{
		private readonly IRepository<CategoryTreeView> _categoryTreeViewRepository;
		private readonly IDetectFullCategoryScan _detectFullCategoryScan;

		private static IEnumerable<CategoryTreeView> _fullStructure;
		private static object _lock = new object();

		public SitecoreCatalogSectionProvider(IUserService userService, ISecurityService securityService, IRepository<CategoryTreeView> categoryTreeViewRepository, bool considerProductsAsChildren, IDetectFullCategoryScan detectFullCategoryScan)
			: base(userService, securityService, categoryTreeViewRepository, considerProductsAsChildren)
		{
			_categoryTreeViewRepository = categoryTreeViewRepository;
			_detectFullCategoryScan = detectFullCategoryScan;
		}

		protected override IEnumerable<CategoryTreeView> ReadCategoryStructureForCatalog(int catalogId)
		{
			// Both bulk (indexing and publishing) operations and interactive operations calls this method for data.

			if (_detectFullCategoryScan.ThreadIsScanningFullCatalog && _fullStructure == null)
			{
				// The current thread is a bulk operation and the cache is not populated.
				HydrateCache();
			}

			if (!_detectFullCategoryScan.FullCatalogScanInProgress && _fullStructure != null)
			{
				// There are no bulk operations in progress and the cache is full.
				ClearCache();
			}

			if (_detectFullCategoryScan.ThreadIsScanningFullCatalog && _fullStructure != null)
			{
				// The current thread is a bulk operation, the cache is full, so return data from the cache.
				return _fullStructure.Where(x => x.ParentCategoryId == null && x.ProductCatalogId == catalogId);
			}

			// Interactive scenario or the cache is not set up.
			return base.ReadCategoryStructureForCatalog(catalogId);
		}

		protected override IEnumerable<CategoryTreeView> ReadCategoryStructureForCategory(int categoryId)
		{
			// Both bulk (indexing and publishing) operations and interactive operations calls this method for data.

			if (_detectFullCategoryScan.ThreadIsScanningFullCatalog && _fullStructure == null)
			{
				// The current thread is a bulk operation and the cache is not populated.
				HydrateCache();
			}

			if (!_detectFullCategoryScan.FullCatalogScanInProgress && _fullStructure != null)
			{
				// There are no bulk operations in progress and the cache is full.
				ClearCache();
			}

			if (_detectFullCategoryScan.ThreadIsScanningFullCatalog && _fullStructure != null)
			{
				// The current thread is a bulk operation, the cache is full, so return data from the cache.
				return _fullStructure.Where(x => x.ParentCategoryId == categoryId);
			}

			// Interactive scenario or the cache is not set up.
			return base.ReadCategoryStructureForCategory(categoryId);
		}

		private void HydrateCache()
		{
			lock (_lock)
			{
				if (_fullStructure == null)
				{
					_fullStructure = _categoryTreeViewRepository.Select(new AllCategoriesTreeViewQuery());
				}
			}
		}

		private void ClearCache()
		{
			_fullStructure = null;
		}

		protected override string GetLocalizedResourceText(string key, CultureInfo culture)
		{
			return base.GetLocalizedResourceText(key, CultureInfo.CreateSpecificCulture("en"));
		}
	}
}
