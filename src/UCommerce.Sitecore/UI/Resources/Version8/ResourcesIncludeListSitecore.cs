using System.Web.UI;
using ClientDependency.Core;
using UCommerce.Presentation.UI.Resources;

namespace UCommerce.Sitecore.UI.Resources.Version8 {
	[ClientDependency(ClientDependencyType.Javascript, "scripts/Shell/ui/jquery.js", "UCommerce", Priority = 0)]
	[ClientDependency(ClientDependencyType.Javascript, "scripts/Shell/ui/jqueryui.js", "UCommerce", Priority = 1)]
	[ClientDependency(ClientDependencyType.Javascript, "scripts/visjs/vis.min.js", "UCommerce")]
	[ClientDependency(ClientDependencyType.Javascript, "scripts/Shell/UCommerce.js", "UCommerce")]

	// uCommerce CSS	
	[ClientDependency(ClientDependencyType.Css, "scripts/datatable/css/jquery.dataTables.css", "UCommerce", Priority = 1)]
	[ClientDependency(ClientDependencyType.Css, "css/sitecore8/jquery-ui-1.8.16.custom.css", "UCommerce")]
	[ClientDependency(ClientDependencyType.Css, "scripts/visjs/vis.min.css", "UCommerce")]
	[ClientDependency(ClientDependencyType.Css, "scripts/Gridster/gridster.min.css", "UCommerce")]
	[ClientDependency(ClientDependencyType.Css, "css/Sitecore8/Common.css", "UCommerce")]
	[ClientDependency(ClientDependencyType.Css, "css/Sitecore8/Lists.css", "UCommerce")]
	[ClientDependency(ClientDependencyType.Css, "css/Sitecore8/bootstrap-multiselect.css", "UCommerce")]
	[ClientDependency(ClientDependencyType.Css, "css/Sitecore8/ScrollingMenu.css", "UCommerce")]
	[ClientDependency(ClientDependencyType.Css, "css/Sitecore8/PropertyPane.css", "UCommerce")]
	[ClientDependency(ClientDependencyType.Css, "css/Sitecore8/TabView.css", "UCommerce")]
	[ClientDependency(ClientDependencyType.Css, "css/Sitecore8/UI.css", "UCommerce")]
	[ClientDependency(ClientDependencyType.Css, "css/fonts/css/font-awesome.min.css", "UCommerce")]
	[ClientDependency(ClientDependencyType.Css, "css/fonts/css/font-awesome.animation.min.css", "UCommerce")]
	[ClientDependency(ClientDependencyType.Css, "css/fonts/css/uCommerce-icon-font.css", "UCommerce")]
    [ClientDependency(ClientDependencyType.Css, "css/Sitecore8/bootstrap-modal.css", "UCommerce")]
	[ClientDependency(ClientDependencyType.Css, "css/Common/Widget.css", "UCommerce", Priority = 1)]
	[ClientDependency(ClientDependencyType.Css, "css/sitecore8/Widget.css", "UCommerce", Priority = 2)]

    public class ResourcesIncludeListSitecore : Control, IResourcesIncludeList {
		public virtual Control GetControl()
		{
			return this;
		}
    }
}
