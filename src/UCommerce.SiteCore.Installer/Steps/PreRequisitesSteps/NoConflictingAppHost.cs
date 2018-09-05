using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.WebHost.Endpoints;
using UCommerce.Installer.Prerequisites;

namespace UCommerce.Sitecore.Installer.Steps.PreRequisitesSteps
{
	public class NoConflictingAppHost : IPrerequisitStep
	{
		public bool MeetsRequirement(out string information)
		{
			information = "";

			var existingAppHost = AppHostBase.Instance;
			
			if (existingAppHost == null) return true;

			var isOurAppHost = RegisteredAppHostIsUcommerceAppHost(out information);

			return isOurAppHost;
		}

		private bool RegisteredAppHostIsUcommerceAppHost(out string information)
		{
			information = "";

			var factoryPath = AppHostBase.Instance.Config.ServiceStackHandlerFactoryPath;
			
			var isOurfactoryPath = (factoryPath == "ucommerceapi");

			if (!isOurfactoryPath) information = string.Format("Found a conflicting apphost: {0}. Make sure no existing app hosts exists.", factoryPath);  

			return isOurfactoryPath;
		}
	}
}
