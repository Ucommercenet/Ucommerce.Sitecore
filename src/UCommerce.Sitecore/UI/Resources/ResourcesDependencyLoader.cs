using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientDependency.Core.Controls;
using UCommerce.Presentation.Web;

namespace UCommerce.Sitecore.UI.Resources {
    public class ResourcesDependencyLoader : ClientDependencyLoader
    {
        public ResourcesDependencyLoader(IUrlResolver urlResolver)
        {
            AddPath("UCommerce", urlResolver.ResolveUrl(""));
            AddPath("shell", urlResolver.ResolveUrl(""));
        }
    }
}
