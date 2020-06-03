using System;
using log4net;
using Ucommerce.Infrastructure.Logging;

namespace Ucommerce.Sitecore.Logging
{
	/// <summary>
	/// Logs using the Sitecore log, via log4net.
	/// </summary>
	public class LoggingService : ILoggingService
	{
		private readonly ILog _logger;

		public LoggingService()
		{
			_logger = log4net.LogManager.GetLogger("UCommerce");
		}

		/// <summary>
		/// Logs the specified <paramref name="customMessage"/>.
		/// </summary>
		/// <param name="customMessage">The custom message.</param>
		public void Log<T>(string customMessage)
		{
			_logger.Info(customMessage);
		}

		/// <summary>
		/// Logs the specified exception.
		/// </summary>
		/// <param name="exception">The exception.</param>
		public void Log<T>(Exception exception)
		{
			var exceptionMessages = this.GetTypeLoadExceptionMessages(exception);
			foreach (var exceptionMessage in exceptionMessages)
			{
				_logger.Error(exceptionMessage);
			}

			Exception exToLog = exception;

			while (exToLog != null)
			{
				_logger.Error(exToLog.Message + "\n" + exToLog.TargetSite + "\n" + exToLog.StackTrace);
				exToLog = exToLog.InnerException;
			}
		}

		/// <summary>
		/// Logs the specified exception along with a custom message.
		/// </summary>
		/// <param name="exception">The exception.</param>
		/// <param name="customMessage">The custom message.</param>
		public void Log<T>(Exception exception, string customMessage)
		{
			_logger.Error(customMessage);

			Log<T>(exception);
		}
	}
}
