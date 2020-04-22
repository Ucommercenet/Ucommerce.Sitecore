using System.Web.UI;
using ClientDependency.Core;
using Ucommerce.Presentation.UI.Resources;

namespace Ucommerce.Sitecore.UI.Resources.Version7
{
	[ClientDependency(ClientDependencyType.Css, "css/Sitecore/ucommerce-sitecore.css", "UCommerce")]
	[ClientDependency(ClientDependencyType.Css, "css/Sitecore/bootstrap.min.css", "UCommerce")]
	public class ResourcesIncludeListSitecoreShell : Control, IResourcesIncludeList
	{
		public Control GetControl() { return this; }
	}
}
