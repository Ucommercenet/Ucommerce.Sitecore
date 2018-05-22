using System.Web.UI;
using ClientDependency.Core;

namespace UCommerce.Sitecore.UI.Resources.Version82
{
	[ClientDependency(ClientDependencyType.Css, "css/Sitecore8.2/Sitecore.css", "UCommerce", Priority = 200)]
	public class ResourcesIncludeListSitecore : Version8.ResourcesIncludeListSitecore
	{
		public override Control GetControl()
		{
			return this;
		}
	}
}
