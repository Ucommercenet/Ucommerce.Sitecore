using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using UCommerce.Presentation.Web;

namespace UCommerce.Sitecore.Web
{
	public class WebPath : IUrlResolver
	{
		public string ResolveUrl(string localUrl)
		{
			var url = VirtualPathUtility.ToAbsolute("~/sitecore modules/Shell/ucommerce");

			if (localUrl.StartsWith(url))
				return localUrl;

			return string.Format("{0}{1}", url, localUrl);
		}
	}
}