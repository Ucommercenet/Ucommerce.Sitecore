using System.Web.UI;
using ClientDependency.Core;
using Ucommerce.Presentation.UI.Resources;

namespace Ucommerce.Sitecore.UI.Resources.Version7
{
	[ClientDependency(ClientDependencyType.Css, "css/Sitecore/ucommerce-sitecore.css", "Ucommerce")]
	[ClientDependency(ClientDependencyType.Css, "css/Sitecore/bootstrap.min.css", "Ucommerce")]
	public class ResourcesIncludeListSitecoreShell : Control, IResourcesIncludeList
	{
		public Control GetControl() { return this; }
	}
}
