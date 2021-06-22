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

		public void Debug<T>(string message)
		{
			_logger.Debug(message);
		}

        public void Debug<T>(string messageTemplate, params object[] propertyValues)
        {
            Ucommerce.Infrastructure.Logging.Capturing.MessageTemplateParser.TryParse(messageTemplate, propertyValues, out var message);
			_logger.Debug(message);
        }

        /// <summary>
		/// Logs the specified <paramref name="message"/>.
		/// </summary>
		/// <param name="message">The custom message.</param>
		public void Information<T>(string message)
		{
			_logger.Info(message);
		}

        public void Information<T>(string messageTemplate, params object[] propertyValues)
        {
            Ucommerce.Infrastructure.Logging.Capturing.MessageTemplateParser.TryParse(messageTemplate, propertyValues, out var message);
			_logger.Info(message);
        }

        /// <summary>
		/// Logs the specified exception.
		/// </summary>
		/// <param name="exception">The exception.</param>
		public void Error<T>(Exception exception)
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
		/// <param name="message">The custom message.</param>
		public void Error<T>(Exception exception, string message)
		{
			_logger.Error(message);

			Error<T>(exception);
		}

        public void Error<T>(Exception exception, string messageTemplate, params object[] propertyValues)
		{
			Ucommerce.Infrastructure.Logging.Capturing.MessageTemplateParser.TryParse(messageTemplate, propertyValues, out var message);
            _logger.Error(message, exception);
		}
    }
}
