using System.Web.Http;
using System.Web.Http.Dispatcher;
using Sitecore.Pipelines;
using Sitecore.Services.Core;
using UCommerce.Sitecore.Web;

namespace UCommerce.Sitecore.Pipelines
{
    public class WebApiConfig
    {
        public void Process(PipelineArgs args)
        {
            GlobalConfiguration.Configure(config =>
            {
                config.Services.Replace(typeof(IHttpControllerSelector), new SitecoreWrappedControllerSelector(config, new NamespaceQualifiedUniqueNameGenerator()));
            });

            GlobalConfiguration.Configuration.MapHttpAttributeRoutes();
        }
    }
}
