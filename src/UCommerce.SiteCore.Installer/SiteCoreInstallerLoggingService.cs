using System;
using log4net;
using Ucommerce.Installer;
using Ucommerce.Installer.Extensions;

namespace Ucommerce.Sitecore.Installer
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
		public void Information<T>(string customMessage)
		{
			_logger.Info(customMessage);
		}

        public void Information<T>(string messageTemplate, params object[] propertyValues)
		{
			// TODO: Doing nothing here prevents an error on install. We don't have Ucommerce.Infrastructure loaded at the time where this would be used.
			// Infrastructure.Logging.Capturing.MessageTemplateParser.TryParse(messageTemplate, propertyValues, out var message);
   //          _logger.Info(message);
		}

        /// <summary>
		/// Logs the specified exception.
		/// </summary>
		/// <param name="exception">The exception.</param>
		public void Error<T>(Exception exception)
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
		public void Error<T>(Exception exception, string customMessage)
		{
			_logger.Error(customMessage);
			Error<T>(exception);
		}

        public void Error<T>(Exception exception, string messageTemplate, params object[] propertyValues)
		{
			// TODO: Doing nothing here prevents an error on install. We don't have Ucommerce.Infrastructure loaded at the time where this would be used.
			// Ucommerce.Infrastructure.Logging.Capturing.MessageTemplateParser.TryParse(messageTemplate, propertyValues, out var message);
   //          _logger.Error(message, exception);
		}
    }
}
