using System.Collections.Specialized;
using Sitecore.Configuration;
using Sitecore.Data.Items;
using Sitecore.Install.Framework;

namespace UCommerce.Sitecore.Installer.Steps
{
	/// <summary>
	/// Remove uCommerce Application, which was added until uCommerce version 7.2.0
	/// </summary>
	public class RemoveUCommerceApplicationLaunchButton : IPostStep
	{
		public void Run(ITaskOutput output, NameValueCollection metaData)
		{
			Item item = Factory.GetDatabase("core").GetItem("/sitecore/client/Applications/Launchpad/PageSettings/Buttons/ContentEditing/uCommerce");

			if (item != null)
			{
				item.Delete();
			}
		}
	}
}
