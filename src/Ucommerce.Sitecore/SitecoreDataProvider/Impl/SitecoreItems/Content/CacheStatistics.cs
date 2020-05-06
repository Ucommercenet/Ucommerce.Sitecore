using Ucommerce.Infrastructure;
using Ucommerce.Infrastructure.Logging;

namespace Ucommerce.Sitecore.SitecoreDataProvider.Impl.SitecoreItems.Content
{
	public class CacheStatistics
	{
		private string Name { get; set; }
		private int LoggingFrequency { get; set; }

		private int _hits;
		private int _misses;
		private int _frequencyCounter;

		public CacheStatistics(string name) : this(name, 10000) {}

		public CacheStatistics(string name, int loggingFrequency)
		{
			Name = name;
			LoggingFrequency = loggingFrequency;
		}

		public void Hit()
		{
			_hits++;
			CheckFrequency();
		}

		public void Miss()
		{
			_misses++;
			CheckFrequency();
		}

		private void CheckFrequency()
		{
			_frequencyCounter++;
			if (_frequencyCounter >= LoggingFrequency)
			{
				_frequencyCounter = 0;
				var log = ObjectFactory.Instance.Resolve<ILoggingService>();
				log.Log<CacheStatistics>(string.Format("'{0}' Hits:{1} Total:{2} Average: {3:00}%", Name, _hits, _hits + _misses, (_hits * 100) / (_hits + _misses)));
			}
		}
	}
}
