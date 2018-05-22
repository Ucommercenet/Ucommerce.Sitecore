using System.Collections.Specialized;
using Sitecore.Install.Framework;
using UCommerce.Installer;

namespace UCommerce.Sitecore.Installer.Steps
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
