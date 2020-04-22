using System.Web.UI;
using ClientDependency.Core;
using Ucommerce.Presentation.UI.Resources;

namespace Ucommerce.Sitecore.UI.Resources.Version8
{
	[ClientDependency(ClientDependencyType.Css, "css/Sitecore8/ucommerce-sitecore8.css", "UCommerce", Priority = 10)]
	[ClientDependency(ClientDependencyType.Css, "css/Sitecore8/bootstrap.min.css", "UCommerce", Priority = 10)]
	[ClientDependency(ClientDependencyType.Css, "css/fonts/css/font-awesome.min.css", "UCommerce", Priority = 10)]
	[ClientDependency(ClientDependencyType.Css, "css/fonts/css/uCommerce-icon-font.css", "UCommerce", Priority = 10)]
	public class ResourcesIncludeListSitecoreShell : Control, IResourcesIncludeList
	{
		public virtual Control GetControl() { return this; }
	}
}

