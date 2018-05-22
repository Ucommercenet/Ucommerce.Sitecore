using System.Reflection;
using System.Web;
using System.Web.SessionState;
using Sitecore.Analytics.Configuration;
using Sitecore.Analytics.Pipelines.StartAnalytics;
using UCommerce.Infrastructure;

namespace UCommerce.Sitecore.Web
{
    /// <summary>
    /// This class wraps an http handler to require session state information.
    /// </summary>
    /// <remarks>
    /// This marks all web requests through ServiceStack to require session state.
    /// Sitecore analytics require ASP.NET session state in order to work.
    /// </remarks>
    public class SessionHandlerDecorator : IHttpHandler, IRequiresSessionState
    {
        private IHttpHandler Handler { get; set; }

        public SessionHandlerDecorator(IHttpHandler handler)
        {
            Handler = handler;
        }

        public bool IsReusable
        {
            get { return Handler.IsReusable; }
        }

        public void ProcessRequest(HttpContext context)
        {
            if (AnalyticsIsEnabled())
            {
                StartAnalyticsPipeline.Run();
            }

            Handler.ProcessRequest(context);
        }
        
        /// <summary>
        /// Check if analytics is enabled across incompatible runtime Sitecore versions.
        /// </summary>
        /// <returns></returns>
        protected virtual bool AnalyticsIsEnabled()
        {
            var analyticsType = typeof(AnalyticsSettings);
            var analyticsEnabledMethod = analyticsType.GetMethod("get_Enabled");

            // Sitecore 7,8
            if (analyticsEnabledMethod != null)
            {
                if (analyticsEnabledMethod.ReturnType == typeof(bool))
                {
                    return (bool)analyticsEnabledMethod.Invoke(null, null);
                }
            }

            // Sitecore 9+
            var xdbSettingsType = Assembly.Load("Sitecore.Xdb.Configuration").GetType("Sitecore.Xdb.Configuration.XdbSettings");
            var xdbSettingsEnabledMethod = xdbSettingsType.GetMethod("get_Enabled");

            if (xdbSettingsEnabledMethod != null)
            {
                if (xdbSettingsEnabledMethod.ReturnType == typeof(bool))
                {
                    return (bool) xdbSettingsEnabledMethod.Invoke(null, null);
                }
            }

            return false;
        }
    }
}
