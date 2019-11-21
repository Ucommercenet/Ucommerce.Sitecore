using System.Linq;
using System.Reflection;
using System.Threading;
using Sitecore;
using Sitecore.Jobs;
using Sitecore.Publishing;

namespace UCommerce.Sitecore.SitecoreDataProvider.Impl
{
	public class DetectFullCategoryScan : IDetectFullCategoryScan
	{
		private readonly bool _activated;
		private readonly bool _testMode;

		private static bool _publishingInProgressState;

		public DetectFullCategoryScan()
		{
			_activated = true;
			_testMode = false;
		}

		public bool FullCatalogScanInProgress
		{
			get
			{
				if (!_activated) return false;
				if (_testMode) return true;
				return PublishingInProgress || IndexingInProgress;
			}
		}

		public bool ThreadIsScanningFullCatalog
		{
			get
			{
				if (!_activated) return false;
				if (_testMode) return true;

				return RequestPartOfPublishing || RequestPartOfIndexing;
			}
		}

		public static bool IndexingOfMasterDatabaseInProgress { get; set; }

		protected virtual bool IndexingInProgress
		{
			get
			{
				if (!_activated) return false;
				if (_testMode) return true;

				return IndexingOfMasterDatabaseInProgress;
			}
		}

		protected virtual bool PublishingInProgress
		{
			get
			{
				if (!_activated) return false;
				if (_testMode) return true;
				bool lockObtained = false;

				var jobManagerLock = GetJobManagerLockObject();
				try
				{
					if (jobManagerLock != null && Monitor.TryEnter(jobManagerLock))
					{
						lockObtained = true;
						var jobs = JobManager.GetJobs().Where(x => (x.Category == "publish") && !x.IsDone);

						foreach (var job in jobs)
						{
							if (job != null && job.Options != null && job.Options.Method != null && job.Options.Method.Object != null)
							{
								var publisher = job.Options.Method.Object as Publisher;
								if (publisher != null && publisher.Options.Mode == PublishMode.Full)
								{
									// Remember the state
									_publishingInProgressState = true;
									return true;
								}
							}
						}

						// Remember the state. We are not publishing.
						_publishingInProgressState = false;
					}
				}
				finally
				{
					if (jobManagerLock != null && lockObtained)
					{
						Monitor.Exit(jobManagerLock);
					}
				}

				// If we could not obtain a lock from the JobManager, we keep and return the current state.
				return _publishingInProgressState;
			}
		}

		protected virtual bool RequestPartOfIndexing
		{
			get { return IndexingInProgress; }
		}

		protected virtual bool RequestPartOfPublishing
		{
			get
			{
				if (!_activated) return false;
				if (_testMode) return true;
				
				var job = Context.Job;
				if (job != null && !job.IsDone && job.Category == "publish" && job.Options != null && job.Options.Method != null && job.Options.Method.Object != null)
				{
					var publisher = job.Options.Method.Object as Publisher;
					if (publisher != null && publisher.Options.Mode == PublishMode.Full)
					{
						return true;
					}
				}
				return false;
			}
		}

		private object GetJobManagerLockObject()
		{
			var field = typeof (JobManager).GetField("m_lock", BindingFlags.NonPublic | BindingFlags.Static);
			if (field == null) return null;

			return field.GetValue(null);
		}
	}
}
