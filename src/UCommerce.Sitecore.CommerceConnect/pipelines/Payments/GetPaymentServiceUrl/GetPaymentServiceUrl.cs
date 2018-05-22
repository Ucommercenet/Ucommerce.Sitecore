using System;
using System.Web;
using Sitecore.Commerce.Pipelines;
using Sitecore.Commerce.Services.Payments;
using UCommerce.Infrastructure.Configuration;

namespace UCommerce.Sitecore.CommerceConnect.Pipelines.Payments.GetPaymentServiceUrl
{
	public class GetPaymentServiceUrl : ServicePipelineProcessor<GetPaymentServiceUrlRequest, GetPaymentServiceUrlResult>
	{
		public override void Process(ServicePipelineArgs args)
		{
			GetPaymentServiceUrlRequest request;
			GetPaymentServiceUrlResult result;

			using (new DisposableThreadLifestyleScope())
			{
				CheckParametersAndSetupRequestAndResult(args, out request, out result);

				result.Url = BuildUrl();
			}
		}

		protected virtual string BuildUrl()
		{
			// {0} - PaymentMethodId
			// {1} - PaymentId
			var uri = new Uri("/{0}/{1}/PaymentRequest.axd", UriKind.RelativeOrAbsolute);
			var absoluteUri = new Uri(HttpContext.Current.Request.Url, uri);
			return absoluteUri.AbsoluteUri;
		}
	}
}
