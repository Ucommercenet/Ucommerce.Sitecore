using System.Globalization;
using System.Threading;
using Sitecore;
using Sitecore.Pipelines.HttpRequest;
using UCommerce.Sitecore.SitecoreDataProvider;

namespace UCommerce.Sitecore.Pipelines
{
	public class SetCurrentThreadCultureToCurrentLanguage : HttpRequestProcessor
	{
		public override void Process(HttpRequestArgs args)
		{
			if (Context.Language == null) return;

			// Defines the resources to be used when displaying content.
			SetCurrentUiCulture();

			// Defines the culture used when formatting dates, times and numbers.
			SetCurrentCulture();
		}

		private void SetCurrentUiCulture()
		{
			var cultureInfo = Context.Language.CultureInfo;
			Thread.CurrentThread.CurrentUICulture = cultureInfo;
		}

		private void SetCurrentCulture()
		{
			var cultureInfo = Context.Language.CultureInfo;
			if (Context.IsLoggedIn)
			{
				Thread.CurrentThread.CurrentCulture = Context.User.Profile.Culture;
			}
			else
			{
				Thread.CurrentThread.CurrentCulture = cultureInfo;
			}
		}
	}
}
