using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Ucommerce.Extensions;

namespace Ucommerce.Sitecore.Installer.Extensions
{
    internal static class ExceptionExtensions
    {
        private static readonly string[] _excludedPropertyNames = { "Type", "Message", "InnerException", "InnerExceptions", "StackTrace", "Data" };

        public static Exception FindException(this Exception exception, Func<Exception, bool> predicate)
        {
            if (exception == null)
                throw new ArgumentNullException(nameof(exception));
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            if (predicate(exception))
                return exception;

            return exception.GetInnerExceptions()
                            .Select(iex => iex.FindException(predicate))
                            .FirstOrDefault();
        }

        public static Exception FindException(this Exception exception, IEnumerable<Type> exceptionTypes, Func<Exception, bool> predicate = null)
        {
            return exception.FindException(ex => exceptionTypes.Any(t => t.IsInstanceOfType(ex))
                                                 && (predicate == null || predicate(ex)));
        }

        public static Exception FindException(this Exception exception, Type exceptionType, Func<Exception, bool> predicate = null)
        {
            return exception.FindException(new[] { exceptionType }, predicate);
        }

        public static TException FindException<TException>(this Exception exception, Func<Exception, bool> predicate = null)
            where TException : Exception
        {
            return exception.FindException(new[] { typeof(TException) }, predicate) as TException;
        }

        public static Exception FindException<TException1, TException2>(this Exception exception, Func<Exception, bool> predicate = null)
            where TException1 : Exception
            where TException2 : Exception
        {
            return exception.FindException(new[] { typeof(TException1), typeof(TException2) }, predicate);
        }

        /// <summary>
        ///     Gets inner exception(s) and calls <see cref="UnwrapException" /> on them.
        /// </summary>
        public static IEnumerable<Exception> GetInnerExceptions(this Exception exception)
        {
            var innerExceptions = GetInnerExceptionsFromAggregateException(exception)
                                  ?? GetInnerExceptionsFromTargetInvocationException(exception);
            if (innerExceptions != null)
                return innerExceptions;

            if (exception.InnerException != null)
                return exception.InnerException.UnwrapException();

            return Enumerable.Empty<Exception>();
        }

        public static string ToDeepString(this Exception exception,
                                          [CallerFilePath] string callerFilePath = null,
                                          [CallerMemberName] string callerMemberName = null,
                                          [CallerLineNumber] int callerLineNumber = 0)
        {
            if (exception == null)
                return null;

            var textWriter = new StringWriter();

            // ReSharper disable ExplicitCallerInfoArgument
            exception.ToDeepString(textWriter, callerFilePath, callerMemberName, callerLineNumber);
            // ReSharper restore ExplicitCallerInfoArgument

            return textWriter.ToString();
        }

        public static void ToDeepString(this Exception exception,
                                        TextWriter textWriter,
                                        [CallerFilePath] string callerFilePath = null,
                                        [CallerMemberName] string callerMemberName = null,
                                        [CallerLineNumber] int callerLineNumber = 0)
        {
            try
            {
                textWriter.WriteLine("=== BEGIN EXCEPTION ===");
                if (!string.IsNullOrWhiteSpace(callerFilePath))
                {
                    textWriter.WriteLine("Catcher info: Member:   " + callerMemberName);
                    textWriter.WriteLine("              Location: " + callerFilePath + ":line " + callerLineNumber);
                }

                textWriter.WriteLine();
                exception.ToDeepString(textWriter, 0);

                textWriter.WriteLine("=== END EXCEPTION ===");
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch { }
        }

        /// <summary>
        ///     Unwraps exception until only non-wrapping exceptions are found.
        /// </summary>
        /// <remarks>
        ///     Wrapping exceptions are:
        ///     - <see cref="System.AggregateException" />
        ///     - <see cref="TargetInvocationException" />
        /// </remarks>
        public static IEnumerable<Exception> UnwrapException(this Exception exception)
        {
            var innerExceptions = GetInnerExceptionsFromAggregateException(exception)
                                  ?? GetInnerExceptionsFromTargetInvocationException(exception);
            if (innerExceptions != null)
                return innerExceptions;

            return new[] { exception };
        }

        private static IEnumerable<Exception> GetInnerExceptionsFromAggregateException(Exception exception)
        {
            var aggrEx = exception as AggregateException;

            return aggrEx?.InnerExceptions
                         .SelectMany(ex => ex.UnwrapException());
        }

        private static IEnumerable<Exception> GetInnerExceptionsFromTargetInvocationException(Exception exception)
        {
            var tiEx = exception as TargetInvocationException;

            return tiEx?.InnerException.UnwrapException();
        }

        private static IEnumerable<KeyValuePair<string, object>> GetProperties(Exception exception)
        {
            return exception.GetType()
                            .GetProperties()
                            .Where(p => p.CanRead)
                            .Where(p => !_excludedPropertyNames.Contains(p.Name))
                            .Select(p => new KeyValuePair<string, object>(p.Name,
                                                                          p.GetValue(exception, null)));
        }

        private static void ToDeepString(this Exception exception, TextWriter textWriter, int depth)
        {
            var indentation = "".PadLeft(3 * depth, ' ');
            if (depth != 0)
                textWriter.WriteLine(indentation + "=== BEGIN INNER EXCEPTION - level " + depth + " ===");

            WriteLine(textWriter, indentation, "Type", exception.GetType());
            WriteLine(textWriter, indentation, "Message", exception.Message);
            WriteLine(textWriter, indentation, "StackTrace", exception.StackTrace);
            WriteLine(textWriter,
                      indentation,
                      "Data",
                      exception.Data.Keys
                               .Cast<object>()
                               .Select(k => new
                               {
                                   Key = k,
                                   Value = exception.Data[k]
                               })
                               .Join("\n"));

            foreach (var property in GetProperties(exception))
                WriteLine(textWriter, indentation, property.Key, property.Value);

            var innerExceptions = GetInnerExceptions(exception);
            foreach (var innerException in innerExceptions)
                innerException.ToDeepString(textWriter, depth + 1);

            if (depth != 0)
                textWriter.WriteLine(indentation + "=== END INNER EXCEPTION - level " + depth + " ===");
        }

        private static void WriteLine(TextWriter textWriter, string indentation, string header, object value)
        {
            var sequence = value as IEnumerable;
            if (sequence != null && !(sequence is string))
            {
                WriteLine(textWriter,
                          indentation,
                          header,
                          sequence.Cast<object>()
                                  .Select(v => v + ""));
            }
            else
                WriteLine(textWriter, indentation, header, value + "");
        }

        private static void WriteLine(TextWriter textWriter, string indentation, string header, Type value)
        {
            WriteLine(textWriter,
                      indentation,
                      header,
                      value != null
                          ? value.FullName
                          : string.Empty);
        }

        private static void WriteLine(TextWriter textWriter, string indentation, string header, string value)
        {
            var lines = (value ?? string.Empty).Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            WriteLine(textWriter, indentation, header, lines);
        }

        private static void WriteLine(TextWriter textWriter, string indentation, string header, IEnumerable<string> values)
        {
            var lines = values.ToArray();

            var prefix = indentation + header + ": ";
            if (lines.Length == 1)
            {
                textWriter.WriteLine(prefix + lines[0]);
                return;
            }

            if (lines.Length > 1)
            {
                textWriter.WriteLine(prefix + lines[0]
                                         .Trim());
                indentation = "".PadLeft(prefix.Length, ' ');
                foreach (var line in lines.Skip(1))
                    textWriter.WriteLine(indentation + line.Trim());
            }
        }
    }
}
