using Sitecore.Configuration;
using Sitecore.Data.Items;
using UCommerce.Installer;

namespace UCommerce.Sitecore.Installer.InstallationSteps
{
    public class RemoveUCommerceApplicationLaunchButton : IInstallationStep
    {
        public void Execute()
        {
            Item item = Factory.GetDatabase("core").GetItem("/sitecore/client/Applications/Launchpad/PageSettings/Buttons/ContentEditing/uCommerce");

            if (item != null)
            {
                item.Delete();
            }
        }
    }
}