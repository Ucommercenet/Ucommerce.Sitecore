using System;
using log4net;
using NHibernate;

namespace Ucommerce.Sitecore.Logging
{
	/// <summary>
	/// Wrapper class, that wraps a Sitecore log4net logger to an nHibernate internal logger.
	/// </summary>
	public class Logger : IInternalLogger
	{
		private readonly ILog _log;

		public Logger(ILog log)
		{
			_log = log;
		}

		public void Error(object message)
		{
			_log.Error(message);
		}

		public void Error(object message, Exception exception)
		{
			_log.Error(message, exception);
		}

		public void ErrorFormat(string format, params object[] args)
		{
			_log.Error(string.Format(format, args));
		}

		public void Fatal(object message)
		{
			_log.Fatal(message);
		}

		public void Fatal(object message, Exception exception)
		{
			_log.Fatal(message, exception);
		}

		public void Debug(object message)
		{
			_log.Debug(message);
		}

		public void Debug(object message, Exception exception)
		{
			_log.Debug(message, exception);
		}

		public void DebugFormat(string format, params object[] args)
		{
			_log.Debug(string.Format(format, args));
		}

		public void Info(object message)
		{
			_log.Info(message);
		}

		public void Info(object message, Exception exception)
		{
			_log.Info(message, exception);
		}

		public void InfoFormat(string format, params object[] args)
		{
			_log.Info(string.Format(format, args));
		}

		public void Warn(object message)
		{
			_log.Warn(message);
		}

		public void Warn(object message, Exception exception)
		{
			_log.Warn(message, exception);
		}

		public void WarnFormat(string format, params object[] args)
		{
			_log.Warn(string.Format(format, args));
		}

		public bool IsErrorEnabled
		{
			get { return _log.IsErrorEnabled; }
		}

		public bool IsFatalEnabled
		{
			get { return _log.IsFatalEnabled; }
		}

		public bool IsDebugEnabled
		{
			get { return _log.IsDebugEnabled; }
		}

		public bool IsInfoEnabled
		{
			get { return _log.IsInfoEnabled; }
		}

		public bool IsWarnEnabled
		{
			get { return _log.IsWarnEnabled; }
		}
	}
}
