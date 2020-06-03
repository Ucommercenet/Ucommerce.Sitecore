using System.Collections.Concurrent;
using System.Linq;
using Ucommerce.EntitiesV2;
using Ucommerce.EntitiesV2.Queries.Catalog;
using Ucommerce.Infrastructure;

namespace Ucommerce.Sitecore.SitecoreDataProvider.Impl
{
	public class ProductDefinitionIdMapperService : IProductDefinitionIdMapperService
	{
		private ConcurrentDictionary<int, int> _map = new ConcurrentDictionary<int, int>();
		private readonly object _lock = new object();

		public int MapToProductDefinitionId(int productId)
		{
			if (!_map.ContainsKey(productId))
			{
				lock (_lock)
				{
					if (!_map.ContainsKey(productId))
					{
						HydrateMap();
					}
				}
			}

			return _map[productId];
		}

		private void HydrateMap()
		{
			var repository = ObjectFactory.Instance.Resolve<IRepository<ProductIdToProductDefinitionIdMap>>();
			var maps = repository.Select(new ProductDefinitionIdQuery()).ToList();
			_map.Clear();

			foreach (var map in maps)
			{
				_map[map.ProductId] = map.ProductDefinitionId;
			}
		}
	}
}
