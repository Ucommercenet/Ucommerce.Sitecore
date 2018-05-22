using System.Web.UI;
using ClientDependency.Core;

namespace UCommerce.Sitecore.UI.Resources.Version82
{
	[ClientDependency(ClientDependencyType.Css, "css/Sitecore8.2/SitecoreShell.css", "UCommerce", Priority = 200)]
	public class ResourcesIncludeListSitecoreSpeakShell : Version8.ResourcesIncludeListSitecoreSpeakShell
	{
		public override Control GetControl()
		{
			return this;
		}
	}
}
