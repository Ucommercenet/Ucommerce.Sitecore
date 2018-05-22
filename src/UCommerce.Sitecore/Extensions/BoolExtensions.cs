using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCommerce.Sitecore.Extensions
{
	public static class BoolExtensions
	{
		public static string ToSitecoreFormat(this bool b)
		{
			return b ? "1" : "0";
		}
	}
}
