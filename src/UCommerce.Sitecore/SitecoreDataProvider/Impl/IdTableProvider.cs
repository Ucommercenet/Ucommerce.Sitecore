using System;
using Sitecore.Data;
using Sitecore.Data.DataProviders.Sql;
using Sitecore.Data.IDTables;
using Sitecore.Data.SqlServer;
using Sitecore.Diagnostics.PerformanceCounters;

namespace UCommerce.Sitecore.SitecoreDataProvider.Impl
{
	[Obsolete("This class is no longer used. It is left here, so that older configurations, that might reference this, do not break.")]
	public class IdTableProvider : SqlServerIDTable
	{
		public IdTableProvider(string connectionString, string cacheSize) : base(connectionString, cacheSize)
		{
			FillTheCacheInitially();
		}

		public IdTableProvider(SqlDataApi api, string cacheSize) : base(api, cacheSize)
		{
			FillTheCacheInitially();
		}

		private void FillTheCacheInitially()
		{
			using (DataProviderReader reader = Api.CreateReader("SELECT {0}ID{1}, {0}Prefix{1}, {0}Key{1}, {0}ParentID{1}, {0}CustomData{1} FROM {0}IDTable{1}"))
			{
				while (reader.Read())
				{
					ID id = Api.GetId(0, reader);
					string prefix = Api.GetString(1, reader);
					string key = Api.GetString(2, reader);
					ID parentId = Api.GetId(3, reader);
					string customData = Api.GetString(4, reader);

					var entry = new IDTableEntry(prefix, key, id, parentId, customData);
					Cache.Add(GetCacheKey(prefix, key), entry);
					DataCounters.PhysicalReads.Increment();
				}
			}
		}

		public override IDTableEntry GetID(string prefix, string key)
		{
			var entry1 = Cache[GetCacheKey(prefix, key)] as IDTableEntry;
			if (entry1 != null) return entry1;
			
			using (DataProviderReader reader = Api.CreateReader("SELECT {0}ID{1}, {0}ParentID{1}, {0}CustomData{1} FROM {0}IDTable{1} WHERE {0}Prefix{1} = {2}prefix{3} AND {0}Key{1} = {2}key{3}", (object)"prefix", (object)prefix, (object)"key", (object)key))
			{
				if (reader.Read())
				{
					ID id1 = Api.GetId(0, reader);
					ID id2 = Api.GetId(1, reader);
					string @string = Api.GetString(2, reader);
					var entry2 = new IDTableEntry(prefix, key, id1, id2, @string);
					Cache.Add(GetCacheKey(prefix, key), entry2);
					DataCounters.PhysicalReads.Increment();
					return entry2;
				}
			}
			return null;
		}
	}
}
