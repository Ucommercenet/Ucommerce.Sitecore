using System;
using Sitecore.Events;
using UCommerce.Infrastructure;
using UCommerce.Infrastructure.Logging;

namespace UCommerce.Sitecore.Events
{
    /// <summary>
    /// Sitecore event handler to be called when permissions change.
    /// </summary>
    public class PermissionsChangedTask
    {
        /// <summary>
        /// If present, inform the data provider, that permissions have changed.
        /// </summary>
        public void Process(object sender, EventArgs args)
        {
            LogEventName(args);
            var sitecoreContext = ObjectFactory.Instance.Resolve<ISitecoreContext>();

            var dataProvider = sitecoreContext.DataProviderMaster;

            if (dataProvider != null)
            {
                dataProvider.PermissionsChanged();
            }
        }

        private void LogEventName(EventArgs args)
        {
            var eventArgs = args as SitecoreEventArgs;

            if (eventArgs == null) return;

            var logger = ObjectFactory.Instance.Resolve<ILoggingService>();
            logger.Log<PermissionsChangedTask>("Event name: " + eventArgs.EventName);
        }
    }
}
