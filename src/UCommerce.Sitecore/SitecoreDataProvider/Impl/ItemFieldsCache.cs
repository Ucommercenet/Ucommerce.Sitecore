using System;
using System.Collections.Concurrent;
using Sitecore.Data;
using UCommerce.Sitecore.Extensions;
using UCommerce.Sitecore.SitecoreDataProvider.Impl.SitecoreItems.Content;

namespace UCommerce.Sitecore.SitecoreDataProvider.Impl
{
	public class ItemFieldsCache
	{
		private readonly IDetectFullCategoryScan _detectFullCategoryScan;
		private readonly ConcurrentDictionary<Guid, ConcurrentDictionary<string, FieldList>> _cache;
		private readonly ConcurrentQueue<Guid> _IdQueue;

		private const int MaxCapacity = 2048;

		private readonly object _lock = new object();
		
		private CacheStatistics Stats { get; set; }

		public ItemFieldsCache(IDetectFullCategoryScan detectFullCategoryScan)
		{
			_detectFullCategoryScan = detectFullCategoryScan;
			Stats = new CacheStatistics("ItemFieldsCache");
			_cache = new ConcurrentDictionary<Guid, ConcurrentDictionary<string, FieldList>>();
			_IdQueue = new ConcurrentQueue<Guid>();
		}

		public FieldList Fetch(Guid id, string language)
		{
			lock (_lock)
			{
				if (_cache.ContainsKey(id) && _cache[id].ContainsKey(language))
				{
					Stats.Hit();
					return _cache[id][language];
				}
				Stats.Miss();

				return null;
			}
		}

		public void Store(Guid id, string language, FieldList data)
		{
			FieldList clonedData;
			try
			{
				clonedData = data.Clone();
			}
			catch (InvalidOperationException)
			{
				// Can happen sometimes, when we enumerate over the data, that the data was modified.
				// In that case, simply skip the caching.
				return;
			}

			lock (_lock)
			{
				if (!_cache.ContainsKey(id))
				{
					_IdQueue.Enqueue(id);
					_cache[id] = new ConcurrentDictionary<string, FieldList>();

					// Only evict items, if there is no publishing or indexing in progress.
					while (!_detectFullCategoryScan.FullCatalogScanInProgress && _IdQueue.Count > MaxCapacity)
					{
						Guid evictId;
						if (_IdQueue.TryDequeue(out evictId))
						{
							EvictItem(evictId);
						}
					}
				}

				_cache[id][language] = clonedData;
			}
		}

		public void EvictItem(Guid id)
		{
			lock (_lock)
			{
				if (_cache.ContainsKey(id))
				{
					ConcurrentDictionary<string, FieldList> value;
					_cache.TryRemove(id, out value);
				}
			}
		}

		public void Clear()
		{
			lock (_lock)
			{
				_cache.Clear();
			}
		}
	}
}
