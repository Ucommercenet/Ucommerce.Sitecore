using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Sitecore.Data;

namespace UCommerce.Sitecore.SitecoreDataProvider.Impl.SitecoreItems.Content
{
	public class FieldListCache
	{
		private readonly IDetectFullCategoryScan _detectFullCategoryScan;
		private int MaxNumberOfItems { get; set; }

		private ConcurrentDictionary<ID, ConcurrentDictionary<string, FieldList>> Data { get; set; }
		private ConcurrentQueue<ID> OrderedListOfAddedIds { get; set; }

		public FieldListCache(int itemSize, IDetectFullCategoryScan detectFullCategoryScan)
		{
			_detectFullCategoryScan = detectFullCategoryScan;
			MaxNumberOfItems = itemSize;
			Data = new ConcurrentDictionary<ID, ConcurrentDictionary<string, FieldList>>();
			OrderedListOfAddedIds = new ConcurrentQueue<ID>();
		}

		public FieldListCache(IDetectFullCategoryScan detectFullCategoryScan) : this(1024, detectFullCategoryScan) { }

		public FieldList Lookup(ID itemId, VersionUri version)
		{
			var versions = TryLookupVersionDictionary(itemId);

			if (versions == null) return null;

			FieldList fields;
			versions.TryGetValue(version.ToString(), out fields);

			return fields;
		}

		public void Store(ID itemId, VersionUri version, FieldList fields)
		{
			var versions = TryLookupVersionDictionary(itemId);

			if (versions == null)
			{
				versions = new ConcurrentDictionary<string, FieldList>();
				Data.TryAdd(itemId, versions);
				OrderedListOfAddedIds.Enqueue(itemId);

				// Only evict items, if there is no publishing or indexing in progress.
				while (!_detectFullCategoryScan.FullCatalogScanInProgress && OrderedListOfAddedIds.Count > MaxNumberOfItems)
				{
					ID idToEvict;
					if (OrderedListOfAddedIds.TryDequeue(out idToEvict))
					{
						Evict(idToEvict);
					}
				}
			}

			versions.TryAdd(version.ToString(), CopyFieldList(fields));
		}

		public bool ContainsItem(ID itemId)
		{
			return Data.ContainsKey(itemId);
		}

		private FieldList CopyFieldList(FieldList fields)
		{
			var copy = new FieldList();

			foreach (ID id in fields.GetFieldIDs())
			{
				copy.Add(id, fields[id]);
			}

			return copy;
		}

		public void Evict(ID itemId)
		{
			ConcurrentDictionary<string, FieldList> versions;
			Data.TryRemove(itemId, out versions);
		}

		private ConcurrentDictionary<string, FieldList> TryLookupVersionDictionary(ID id)
		{
			ConcurrentDictionary<string, FieldList> versions;
			Data.TryGetValue(id, out versions);
			return versions;
		}

		public IEnumerable<KeyValuePair<int, Guid>> FilterOnItemsNotInCache(List<KeyValuePair<int, Guid>> productIds)
		{
			foreach (var product in productIds)
			{
				var id = new ID(product.Value);
				if (!Data.ContainsKey(id)) yield return product;
			}
		}

		public void Clear()
		{
			Data.Clear();
            OrderedListOfAddedIds = new ConcurrentQueue<ID>();
		}
	}
}
