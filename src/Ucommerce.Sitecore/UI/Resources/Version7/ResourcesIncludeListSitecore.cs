using System.Web.UI;
using ClientDependency.Core;
using Ucommerce.Presentation.UI.Resources;

namespace Ucommerce.Sitecore.UI.Resources.Version7 {
	// uCommerce CSS
	[ClientDependency(ClientDependencyType.Javascript, "scripts/Shell/UCommerce.js", "Ucommerce")]
	[ClientDependency(ClientDependencyType.Javascript, "scripts/Shell/ui/jquery.js", "Ucommerce", Priority = 0)]
	[ClientDependency(ClientDependencyType.Javascript, "scripts/Shell/ui/jqueryui.js", "Ucommerce", Priority = 1)]
	[ClientDependency(ClientDependencyType.Javascript, "scripts/Gridster/gridster.min.js", "Ucommerce", Priority = 2)]
	[ClientDependency(ClientDependencyType.Javascript, "scripts/visjs/vis.min.js", "Ucommerce")]
	[ClientDependency(ClientDependencyType.Css, "css/sitecore/jquery-ui-1.8.16.custom.css", "Ucommerce")]
	[ClientDependency(ClientDependencyType.Css, "scripts/datatable/css/jquery.dataTables.css", "Ucommerce", Priority = 1)]
	[ClientDependency(ClientDependencyType.Css, "scripts/visjs/vis.min.css", "Ucommerce")]
	[ClientDependency(ClientDependencyType.Css, "scripts/Gridster/gridster.min.css", "Ucommerce")]
	[ClientDependency(ClientDependencyType.Css, "css/Sitecore/Common.css", "Ucommerce")]
	[ClientDependency(ClientDependencyType.Css, "css/Sitecore/Lists.css", "Ucommerce")]
	[ClientDependency(ClientDependencyType.Css, "css/Sitecore/PropertyPane.css", "Ucommerce")]
	[ClientDependency(ClientDependencyType.Css, "css/Sitecore/bootstrap-modal.css", "Ucommerce")]
	[ClientDependency(ClientDependencyType.Css, "css/Sitecore/ScrollingMenu.css", "Ucommerce")]
	[ClientDependency(ClientDependencyType.Css, "css/Sitecore/bootstrap-multiselect.css", "Ucommerce", Priority = 2)]
	[ClientDependency(ClientDependencyType.Css, "css/Sitecore/UI.css", "Ucommerce")]
	[ClientDependency(ClientDependencyType.Css, "css/Sitecore/TabView.css", "Ucommerce")]
	[ClientDependency(ClientDependencyType.Css, "css/fonts/css/font-awesome.min.css", "Ucommerce")]
	[ClientDependency(ClientDependencyType.Css, "css/fonts/css/font-awesome.animation.min.css", "Ucommerce")]
	[ClientDependency(ClientDependencyType.Css, "css/fonts/css/uCommerce-icon-font.css", "Ucommerce")]
	[ClientDependency(ClientDependencyType.Css, "css/Common/Widget.css", "Ucommerce")]
	[ClientDependency(ClientDependencyType.Css, "css/sitecore/Widget.css", "Ucommerce")]

	public class ResourcesIncludeListSitecore : Control, IResourcesIncludeList
	{
		public Control GetControl()
		{
			return this;
		}
	}
}
