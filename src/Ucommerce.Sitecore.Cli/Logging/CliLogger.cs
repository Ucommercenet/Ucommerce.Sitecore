using System;
using CliFx.Infrastructure;
using Ucommerce.Infrastructure.Logging.Capturing;
using Ucommerce.Installer;

namespace Ucommerce.Sitecore.Cli.Logging
{
    public class CliLogger : IInstallerLoggingService
    {
        private readonly IConsole _console;

        public CliLogger(IConsole console)
        {
            _console = console;
        }

        public void Information<T>(string message)
        {
            _console.Output.WriteLine(message);
        }

        public void Information<T>(string messageTemplate, params object[] propertyValues)
        {
            MessageTemplateParser.TryParse(messageTemplate, propertyValues, out var message);
            _console.Output.WriteLine(message);
        }

        public void Error<T>(Exception exception)
        {
            _console.Output.WriteLine(exception.Message);
        }

        public void Error<T>(Exception exception, string message)
        {
            _console.Output.WriteLine(exception.Message);
        }

        public void Error<T>(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            _console.Output.WriteLine(exception.Message);
        }
    }
}