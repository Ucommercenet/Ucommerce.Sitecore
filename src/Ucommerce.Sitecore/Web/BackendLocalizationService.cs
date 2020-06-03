using Ucommerce.Presentation.Web.Pages;

namespace Ucommerce.Sitecore.Web
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
