﻿using System.Web.UI;
using ClientDependency.Core;
using Ucommerce.Presentation.UI.Resources;

namespace Ucommerce.Sitecore.UI.Resources.Version8
{
	[ClientDependency(ClientDependencyType.Css, "css/Speak/ucommerce-speak.css", "Ucommerce")]
	public class ResourcesIncludeListSitecoreSpeak : ResourcesIncludeListSitecore, IResourcesIncludeList
	{
		public override Control GetControl()
		{
			return this;
		}
	}
}
