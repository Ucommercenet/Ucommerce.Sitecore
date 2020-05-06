using System.Collections.Specialized;
using Sitecore.Install.Framework;
using Ucommerce.Installer;

namespace Ucommerce.Sitecore.Installer.Steps
{
	public class InitializeObjectFactory : IPostStep
	{
		public void Run(ITaskOutput output, NameValueCollection metaData)
		{
			var initializer = new ObjectFactoryInitializer();
			initializer.InitializeObjectFactory();
		}
	}
}
