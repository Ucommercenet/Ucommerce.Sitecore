using System;

namespace Ucommerce.Sitecore.Extensions
{
	public static class DateTimeExtensions
	{
		public static string ToSitecoreFormat(this DateTime dateTime)
		{
			return dateTime.ToString("yyyyMMddTHHmmss");
		}
	}
}
