using System.Web;
using Sitecore;
using Ucommerce.Infrastructure.Logging;
using Ucommerce.Security;

namespace Ucommerce.Sitecore.Security
{
	/// <summary>
	/// Integration with SiteCore authentication.
	/// </summary>
	public class AuthenticationService : IAuthenticationService
	{
		private readonly ISitecoreContext _sitecoreContext;
		private readonly ILoggingService _loggingService;

		public AuthenticationService(ISitecoreContext sitecoreContext, ILoggingService loggingService)
		{
			_sitecoreContext = sitecoreContext;
			_loggingService = loggingService;
		}

		public bool IsAuthenticated()
		{
			var user = Context.User;

			if (user == null)
			{
				_loggingService.Information<AuthenticationService>("User is null, and can't be authenticated.");
				return false;
			}

			if (!user.IsAuthenticated)
			{
				_loggingService.Information<AuthenticationService>(string.Format("User with name: {0} is not authenticated.",user.GetLocalName()));
				return false;
			}

			if (user.IsAuthenticated && !DomainNameAllowed())
			{
				_loggingService.Information<AuthenticationService>(string.Format("User: {0} on domain: {1} does not match the configured domain for authentication to uCommerce: {2}", user.LocalName, user.GetDomainName(), _sitecoreContext.BackendDomainName));
				return false;
			}

			return Context.User != null &&
			       Context.User.IsAuthenticated &&
			       DomainNameAllowed();
		}

		protected virtual bool DomainNameAllowed()
		{
			return Context.User.GetDomainName() == _sitecoreContext.BackendDomainName;
		}

		public void PromptForLogin()
		{
			var context = HttpContext.Current;
			context.Response.Redirect("/sitecore/login");
		}
	}
}