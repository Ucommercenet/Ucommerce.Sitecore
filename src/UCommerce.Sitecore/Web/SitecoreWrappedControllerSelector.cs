using System;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using Sitecore.Services.Core;
using System.Net.Http;
using Sitecore.Services.Infrastructure.Web.Http.Dispatcher;

namespace UCommerce.Sitecore.Web
{
    public class SitecoreWrappedControllerSelector : DefaultHttpControllerSelector
    {
        private readonly IHttpControllerSelector _sitecoreHttpControllerSelector;

        public SitecoreWrappedControllerSelector(HttpConfiguration configuration, IControllerNameGenerator controllerNameGenerator) : base(configuration)
        {
            _sitecoreHttpControllerSelector = new NamespaceHttpControllerSelector(configuration, controllerNameGenerator);
            
        }

        public override HttpControllerDescriptor SelectController(HttpRequestMessage request)
        {
            try
            {
                return _sitecoreHttpControllerSelector.SelectController(request);
            }
            catch (HttpResponseException)
            {
                return base.SelectController(request);
            }
            catch (ArgumentNullException)
            {
                return base.SelectController(request);
            }
        }
    }
}
