using System;
using System.Collections.Concurrent;
using System.Linq;
using UCommerce.EntitiesV2;
using UCommerce.EntitiesV2.Queries.Catalog;
using UCommerce.Infrastructure;

namespace UCommerce.Sitecore.SitecoreDataProvider.Impl
{
	public class CategoryIdToCategoryDefinitionIdMapperService : ICategoryIdToCategoryDefinitionIdMapperService
	{
		private readonly ConcurrentDictionary<int, int> _map = new ConcurrentDictionary<int, int>();
		private readonly object _lock = new object();

		public int MapToCategoryDefinitionId(int categoryId)
		{
			if (!_map.ContainsKey(categoryId))
			{
				lock (_lock)
				{
					if (!_map.ContainsKey(categoryId))
					{
						HydrateMap();
					}
				}
			}

			return _map[categoryId];
		}

		private void HydrateMap()
		{
			var repository = ObjectFactory.Instance.Resolve<IRepository<CategoryToCategoryDefinitionMap>>();
			var maps = repository.Select(new CategoryDefinitionIdQuery()).ToList();
			_map.Clear();

			foreach (var map in maps)
			{
				_map[map.CategoryId] = map.CategoryDefinitionId;
			}
		}
	}
}
