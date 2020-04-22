using System;
using log4net;
using NHibernate;

namespace Ucommerce.Sitecore.Logging
{
	/// <summary>
	/// Implementation of an NHibernate logger factory, using Sitecore
	/// </summary>
	public class LoggerFactory : ILoggerFactory
	{
		public IInternalLogger LoggerFor(string keyName)
		{
			return new Logger(LogManager.GetLogger(keyName));
		}

		public IInternalLogger LoggerFor(Type type)
		{
			return new Logger(LogManager.GetLogger(type));
		}
	}
}
