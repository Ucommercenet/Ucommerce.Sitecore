using System;
using log4net;
using Ucommerce.Installer;
using Ucommerce.Sitecore.Installer.Extensions;

namespace Ucommerce.Sitecore.Installer
{
    internal class SitecoreInstallerLoggingService : IInstallerLoggingService
    {
        private readonly ILog _logger;

        public SitecoreInstallerLoggingService()
        {
            _logger = global::Sitecore.Diagnostics.LoggerFactory.GetLogger("UCommerce");
        }

        /// <summary>
        /// Logs the specified exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        public void Error<T>(Exception exception)
        {
            _logger.Error(exception);
            _logger.Error(exception.ToDeepString());
        }

        /// <summary>
        /// Logs the specified exception along with a custom message.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="customMessage">The custom message.</param>
        public void Error<T>(Exception exception, string customMessage)
        {
            Error<T>(exception);
        }

        public void Error<T>(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            MessageTemplateParser.TryParse(messageTemplate, propertyValues, out var message);
            Error<T>(exception, message);
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
            MessageTemplateParser.TryParse(messageTemplate, propertyValues, out var message);
            _logger.Info(message);
        }
    }
}
