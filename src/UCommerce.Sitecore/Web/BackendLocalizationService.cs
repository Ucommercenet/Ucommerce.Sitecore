using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Sitecore.Sites;
using UCommerce.Presentation.Web.Pages;
using UCommerce.Security;
using sitecoreExt = Sitecore;

namespace UCommerce.Sitecore.Web
{
    public class BackendLocalizationService : IBackendLocalizationService
    {
	    public void EnsureBackendCulture()
        {
			// NB! The current culture is set on the thread in the BeginHttpRequest pipeline.
			// So nothing to do here.
        }
    }
}
