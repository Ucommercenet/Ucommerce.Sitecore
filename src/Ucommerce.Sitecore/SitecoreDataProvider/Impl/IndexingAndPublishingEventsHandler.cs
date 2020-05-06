using System;
using Sitecore.ContentSearch;
using Sitecore.Data;
using Sitecore.Events;
using Ucommerce.Infrastructure;
using Ucommerce.Infrastructure.Logging;

namespace Ucommerce.Sitecore.SitecoreDataProvider.Impl
{
	public class IndexingAndPublishingEventsHandler
	{
		public void IndexingStart(object sender, EventArgs args)
		{
			var databaseName = ExtractDatabaseNameFromIndexingEventArgs(args);

			if (databaseName.Equals("master", StringComparison.InvariantCultureIgnoreCase))
			{
				ObjectFactory.Instance.Resolve<ILoggingService>().Log<IndexingAndPublishingEventsHandler>("Indexing starting on master database.");
				DetectFullCategoryScan.IndexingOfMasterDatabaseInProgress = true;
			}
		}

		public void IndexingEnd(object sender, EventArgs args)
		{
			var databaseName = ExtractDatabaseNameFromIndexingEventArgs(args);

			if (databaseName.Equals("master", StringComparison.InvariantCultureIgnoreCase))
			{
				ObjectFactory.Instance.Resolve<ILoggingService>().Log<IndexingAndPublishingEventsHandler>("Indexing ending on master database.");
				DetectFullCategoryScan.IndexingOfMasterDatabaseInProgress = false;
			}
		}

		private string ExtractDatabaseNameFromIndexingEventArgs(EventArgs args)
		{
			var sitecoreArgs = args as SitecoreEventArgs;
			if (sitecoreArgs == null || sitecoreArgs.Parameters.Length < 2) return string.Empty;

			var uniqueId = sitecoreArgs.Parameters[1] as SitecoreItemUniqueId;
			if (uniqueId == null) return string.Empty;

			var itemUri = uniqueId.Value as ItemUri;
			if (itemUri == null) return string.Empty;

			return itemUri.DatabaseName;
		}
	}
}
