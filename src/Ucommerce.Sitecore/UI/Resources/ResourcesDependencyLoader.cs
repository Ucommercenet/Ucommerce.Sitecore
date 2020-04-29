using ClientDependency.Core.Controls;
using Ucommerce.Presentation.Web;

namespace Ucommerce.Sitecore.UI.Resources {
    public class ResourcesDependencyLoader : ClientDependencyLoader
    {
        public ResourcesDependencyLoader(IUrlResolver urlResolver)
        {
            AddPath("Ucommerce", urlResolver.ResolveUrl(""));
            AddPath("shell", urlResolver.ResolveUrl(""));
        }
    }
}
