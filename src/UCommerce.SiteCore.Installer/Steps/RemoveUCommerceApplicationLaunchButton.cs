using System.Collections.Specialized;
using Sitecore.Configuration;
using Sitecore.Install.Framework;

namespace Ucommerce.Sitecore.Installer.Steps
{
    /// <summary>
    /// Remove uCommerce Application, which was added until uCommerce version 7.2.0
    /// </summary>
    public class RemoveUCommerceApplicationLaunchButton : IPostStep
    {
        public void Run(ITaskOutput output, NameValueCollection metaData)
        {
            var item = Factory.GetDatabase("core")
                .GetItem("/sitecore/client/Applications/Launchpad/PageSettings/Buttons/ContentEditing/uCommerce");

            if (item != null)
            {
                item.Delete();
            }
        }
    }
}
