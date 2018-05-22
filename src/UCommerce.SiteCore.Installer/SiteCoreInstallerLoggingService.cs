using System;
using UCommerce.Installer;
using UCommerce.Installer.Extensions;
using log4net;

namespace UCommerce.Sitecore.Installer
{
	internal class SitecoreInstallerLoggingService : IInstallerLoggingService
	{
		private readonly ILog _logger;

		public SitecoreInstallerLoggingService()
		{
			_logger = LogManager.GetLogger("UCommerce");
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
			Exception exToLog = exception;

			while (exToLog != null)
			{
				_logger.Error(exToLog.FormatForLogging());
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
