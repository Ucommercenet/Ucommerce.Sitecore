using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCommerce.Sitecore.Extensions
{
	public static class DateTimeExtensions
	{
		public static string ToSitecoreFormat(this DateTime dateTime)
		{
			return dateTime.ToString("yyyyMMddTHHmmss");
		}
	}
}
