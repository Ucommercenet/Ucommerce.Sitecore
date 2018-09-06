using System;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using Sitecore.Services.Core;
using System.Net.Http;
using System.Reflection;
using Sitecore.Services.Infrastructure.Web.Http.Dispatcher;
using UCommerce.Sitecore.Logging;
using UCommerce.Sitecore.UI.Resources;

namespace UCommerce.Sitecore.Web
{
    public class SitecoreWrappedControllerSelector : DefaultHttpControllerSelector
    {
        private readonly IHttpControllerSelector _sitecoreHttpControllerSelector;

        public SitecoreWrappedControllerSelector(HttpConfiguration configuration, IControllerNameGenerator controllerNameGenerator) : base(configuration)
        {
            SitecoreVersionResolver versionResolver = new SitecoreVersionResolver(new LoggingService());

            if (versionResolver.IsEqualOrGreaterThan(new Version(9, 0)))
            {
                _sitecoreHttpControllerSelector = GetControllerSelectorFromReflectionVersion90(configuration, controllerNameGenerator);
            }
            else
            {
                _sitecoreHttpControllerSelector = GetControllerSelectorFromReflectionVersion82(configuration, controllerNameGenerator);
            }
        }

        private IHttpControllerSelector GetControllerSelectorFromReflectionVersion90(HttpConfiguration configuration, IControllerNameGenerator controllerNameGenerator)
        {
            ConstructorInfo constructor = typeof(NamespaceHttpControllerSelector).GetConstructor(new[]
                {
                    typeof(HttpConfiguration),
                    typeof(IControllerNameGenerator),
                    typeof(IHttpControllerSelector)
                });

            return (IHttpControllerSelector)constructor.Invoke(new object[] { configuration, controllerNameGenerator, this });
        }

        private IHttpControllerSelector GetControllerSelectorFromReflectionVersion82(HttpConfiguration configuration, IControllerNameGenerator controllerNameGenerator)
        {
            ConstructorInfo constructor = typeof(NamespaceHttpControllerSelector).GetConstructor(new[]
            {
                typeof(HttpConfiguration),
                typeof(IControllerNameGenerator),
            });

            return (IHttpControllerSelector) constructor.Invoke(new object[] {configuration, controllerNameGenerator});
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
