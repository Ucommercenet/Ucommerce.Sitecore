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

        public void Error<T>(Exception exception)
        {
            Log("Error", ConsoleColor.Red, "Exception occured.", typeof(T), exception);
        }

        public void Error<T>(Exception exception, string message)
        {
            Log("Error", ConsoleColor.Red, message, typeof(T), exception);
        }

        public void Error<T>(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            Log("Error", ConsoleColor.Red, messageTemplate, typeof(T), exception);
        }

        public void Information<T>(string message)
        {
            Log("Information", ConsoleColor.Green, message, typeof(T));
        }

        public void Information<T>(string messageTemplate, params object[] propertyValues)
        {
            MessageTemplateParser.TryParse(messageTemplate, propertyValues, out var message);
            Log("Information", ConsoleColor.Green, message, typeof(T));
        }

        private void Log(string logLevel,
            ConsoleColor color,
            string message,
            Type type,
            Exception exception = null)
        {
            var originalColor = Console.ForegroundColor;

            _console.ForegroundColor = color;
            _console.Output.WriteLine($"[{logLevel}]");

            _console.ForegroundColor = originalColor;
            _console.Output.Write($"     {type.Name} - ");

            _console.ForegroundColor = color;

            if (exception == null)
            {
                _console.Output.WriteLine($"{message}");
            }
            else
            {
                _console.Error.WriteLine($"{message}");
                _console.Error.WriteLine($"     {exception.Message}{(exception.StackTrace != null ? $"\n\n{exception.StackTrace}" : "")}");
            }

            _console.ForegroundColor = originalColor;
        }
    }
}
