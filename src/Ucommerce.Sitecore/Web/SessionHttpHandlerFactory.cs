using System.Web;
using ServiceStack.WebHost.Endpoints;

namespace UCommerce.Sitecore.Web
{
	public class SessionHttpHandlerFactory : IHttpHandlerFactory
	{
		private readonly static ServiceStackHttpHandlerFactory Factory = new ServiceStackHttpHandlerFactory();

		public IHttpHandler GetHandler(HttpContext context, string requestType, string url, string pathTranslated)
		{
			var handler = Factory.GetHandler(context, requestType, url, pathTranslated);
			return handler == null ? null : new SessionHandlerDecorator(handler);
		}

		public void ReleaseHandler(IHttpHandler handler)
		{
			Factory.ReleaseHandler(handler);
		}

	}
}
