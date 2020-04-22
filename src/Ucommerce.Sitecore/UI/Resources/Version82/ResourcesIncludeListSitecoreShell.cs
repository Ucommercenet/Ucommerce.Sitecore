using System.Web.UI;
using ClientDependency.Core;

namespace Ucommerce.Sitecore.UI.Resources.Version82
{
    /// <summary>
    /// Kentico implementation of the <see cref="IResourcesIncludeList"/> for the shell.
    /// </summary>
	[ClientDependency(ClientDependencyType.Css, "css/Sitecore8.2/SitecoreShell.css", "UCommerce", Priority = 200)]
	public class ResourcesIncludeListSitecoreShell : Version8.ResourcesIncludeListSitecoreShell
	{
		public override Control GetControl()
		{
			return this;
		}
	}
}
