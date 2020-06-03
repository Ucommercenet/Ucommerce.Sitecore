using System.Web.UI;
using ClientDependency.Core;
using Ucommerce.Presentation.UI.Resources;

namespace Ucommerce.Sitecore.UI.Resources.Version8 {
	[ClientDependency(ClientDependencyType.Javascript, "scripts/Shell/ui/jquery.js", "Ucommerce", Priority = 0)]
	[ClientDependency(ClientDependencyType.Javascript, "scripts/Shell/ui/jqueryui.js", "Ucommerce", Priority = 1)]
	[ClientDependency(ClientDependencyType.Javascript, "scripts/visjs/vis.min.js", "Ucommerce")]
	[ClientDependency(ClientDependencyType.Javascript, "scripts/Shell/UCommerce.js", "Ucommerce")]

	// uCommerce CSS
	[ClientDependency(ClientDependencyType.Css, "scripts/datatable/css/jquery.dataTables.css", "Ucommerce", Priority = 1)]
	[ClientDependency(ClientDependencyType.Css, "css/sitecore8/jquery-ui-1.8.16.custom.css", "Ucommerce")]
	[ClientDependency(ClientDependencyType.Css, "scripts/visjs/vis.min.css", "Ucommerce")]
	[ClientDependency(ClientDependencyType.Css, "scripts/Gridster/gridster.min.css", "Ucommerce")]
	[ClientDependency(ClientDependencyType.Css, "css/Sitecore8/Common.css", "Ucommerce")]
	[ClientDependency(ClientDependencyType.Css, "css/Sitecore8/Lists.css", "Ucommerce")]
	[ClientDependency(ClientDependencyType.Css, "css/Sitecore8/bootstrap-multiselect.css", "Ucommerce")]
	[ClientDependency(ClientDependencyType.Css, "css/Sitecore8/ScrollingMenu.css", "Ucommerce")]
	[ClientDependency(ClientDependencyType.Css, "css/Sitecore8/PropertyPane.css", "Ucommerce")]
	[ClientDependency(ClientDependencyType.Css, "css/Sitecore8/TabView.css", "Ucommerce")]
	[ClientDependency(ClientDependencyType.Css, "css/Sitecore8/UI.css", "Ucommerce")]
	[ClientDependency(ClientDependencyType.Css, "css/fonts/css/font-awesome.min.css", "Ucommerce")]
	[ClientDependency(ClientDependencyType.Css, "css/fonts/css/font-awesome.animation.min.css", "Ucommerce")]
	[ClientDependency(ClientDependencyType.Css, "css/fonts/css/uCommerce-icon-font.css", "Ucommerce")]
    [ClientDependency(ClientDependencyType.Css, "css/Sitecore8/bootstrap-modal.css", "Ucommerce")]
	[ClientDependency(ClientDependencyType.Css, "css/Common/Widget.css", "Ucommerce", Priority = 1)]
	[ClientDependency(ClientDependencyType.Css, "css/sitecore8/Widget.css", "Ucommerce", Priority = 2)]

    public class ResourcesIncludeListSitecore : Control, IResourcesIncludeList {
		public virtual Control GetControl()
		{
			return this;
		}
    }
}
