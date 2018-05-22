using System.Web.UI;
using ClientDependency.Core;
using UCommerce.Presentation.UI.Resources;

namespace UCommerce.Sitecore.UI.Resources.Version7 {
	// uCommerce CSS
	[ClientDependency(ClientDependencyType.Javascript, "scripts/Shell/UCommerce.js", "UCommerce")]
	[ClientDependency(ClientDependencyType.Javascript, "scripts/Shell/ui/jquery.js", "UCommerce", Priority = 0)]
	[ClientDependency(ClientDependencyType.Javascript, "scripts/Shell/ui/jqueryui.js", "UCommerce", Priority = 1)]
	[ClientDependency(ClientDependencyType.Javascript, "scripts/Gridster/gridster.min.js", "UCommerce", Priority = 2)]
	[ClientDependency(ClientDependencyType.Javascript, "scripts/visjs/vis.min.js", "UCommerce")]
	[ClientDependency(ClientDependencyType.Css, "css/sitecore/jquery-ui-1.8.16.custom.css", "UCommerce")]
	[ClientDependency(ClientDependencyType.Css, "scripts/datatable/css/jquery.dataTables.css", "UCommerce", Priority = 1)]
	[ClientDependency(ClientDependencyType.Css, "scripts/visjs/vis.min.css", "UCommerce")]
	[ClientDependency(ClientDependencyType.Css, "scripts/Gridster/gridster.min.css", "UCommerce")]
	[ClientDependency(ClientDependencyType.Css, "css/Sitecore/Common.css", "UCommerce")]
	[ClientDependency(ClientDependencyType.Css, "css/Sitecore/Lists.css", "UCommerce")]
	[ClientDependency(ClientDependencyType.Css, "css/Sitecore/PropertyPane.css", "UCommerce")]
	[ClientDependency(ClientDependencyType.Css, "css/Sitecore/bootstrap-modal.css", "UCommerce")]
	[ClientDependency(ClientDependencyType.Css, "css/Sitecore/ScrollingMenu.css", "UCommerce")]
	[ClientDependency(ClientDependencyType.Css, "css/Sitecore/bootstrap-multiselect.css", "UCommerce", Priority = 2)]
	[ClientDependency(ClientDependencyType.Css, "css/Sitecore/UI.css", "UCommerce")]
	[ClientDependency(ClientDependencyType.Css, "css/Sitecore/TabView.css", "UCommerce")]
	[ClientDependency(ClientDependencyType.Css, "css/fonts/css/font-awesome.min.css", "UCommerce")]
	[ClientDependency(ClientDependencyType.Css, "css/fonts/css/font-awesome.animation.min.css", "UCommerce")]
	[ClientDependency(ClientDependencyType.Css, "css/fonts/css/uCommerce-icon-font.css", "UCommerce")]
	[ClientDependency(ClientDependencyType.Css, "css/Common/Widget.css", "UCommerce")]
	[ClientDependency(ClientDependencyType.Css, "css/sitecore/Widget.css", "UCommerce")]

	public class ResourcesIncludeListSitecore : Control, IResourcesIncludeList
	{
		public Control GetControl()
		{
			return this;
		}
	}
}
