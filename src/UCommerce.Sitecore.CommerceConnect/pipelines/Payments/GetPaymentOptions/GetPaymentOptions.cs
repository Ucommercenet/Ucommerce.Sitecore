using System.Collections.Generic;
using System.Collections.ObjectModel;
using Sitecore.Commerce.Entities.Payments;
using Sitecore.Commerce.Pipelines;
using Sitecore.Commerce.Services.Payments;

namespace UCommerce.Sitecore.CommerceConnect.Pipelines.Payments.GetPaymentOptions
{
	public class GetPaymentOptions : ServicePipelineProcessor<GetPaymentOptionsRequest, GetPaymentOptionsResult>
    {
        private readonly IList<PaymentOption> _paymentOptions;

        public GetPaymentOptions()
        {
            _paymentOptions = new List<PaymentOption>()
            {
                new PaymentOption()
                {
                    Name = "None",
                    PaymentOptionType = PaymentOptionType.None
                }
            };
        }

        public override void Process(ServicePipelineArgs args)
        {
	        GetPaymentOptionsRequest request;
	        GetPaymentOptionsResult result;

			CheckParametersAndSetupRequestAndResult(args, out request, out result);
            result.PaymentOptions = new ReadOnlyCollection<PaymentOption>(_paymentOptions);
		}
    }
}
