using System;
using Sitecore.Commerce.Pipelines;
using Sitecore.Commerce.Services.Payments;
using Sitecore.Diagnostics;
using UCommerce.EntitiesV2;
using UCommerce.Infrastructure;
using UCommerce.Infrastructure.Configuration;
using UCommerce.Transactions.Payments;

namespace UCommerce.Sitecore.CommerceConnect.Pipelines.Payments.GetPaymentServiceActionResult
{
	public class GetPaymentServiceActionResult : ServicePipelineProcessor<GetPaymentServiceActionResultRequest, GetPaymentServiceActionResultResult>
	{
		public override void Process(ServicePipelineArgs args)
		{
			GetPaymentServiceActionResultRequest request;
			GetPaymentServiceActionResultResult result;

			using (new DisposableThreadLifestyleScope())
			{
				CheckParametersAndSetupRequestAndResult(args, out request, out result);
				Assert.ArgumentNotNull(request.PaymentAcceptResultAccessCode, "args.Request.PaymentAcceptResultAccessCode");

				int paymentId = InterpretAccessCodeAsPaymentId(request.PaymentAcceptResultAccessCode);
				var payment = FindPaymentFromPaymentId(paymentId);

				result.AuthorizationResult = IsPaymentAuthorizedOrCaptured(payment) ? "OK" : "NOT OK";
			}
		}

		protected virtual int InterpretAccessCodeAsPaymentId(string accessCode)
		{
			int paymentId;
			if (int.TryParse(accessCode, out paymentId))
			{
				return paymentId;
			}

			throw new ArgumentException("The PaymentAcceptResultAccessCode passed, must be parsable to an integer.", "args.Request.PaymentAcceptResultAccessCode");
		}

		protected virtual Payment FindPaymentFromPaymentId(int paymentId)
		{
			var paymentRepository = ObjectFactory.Instance.Resolve<IRepository<Payment>>();
			var payment = paymentRepository.Get(paymentId);

			if (payment == null)
			{
				throw new ArgumentException("The PaymentAcceptResultAccessCode passed, must represent a valid PaymentId in uCommerce.", "args.Request.PaymentAcceptResultAccessCode");
			}

			return payment;
		}

		protected virtual bool IsPaymentAuthorizedOrCaptured(Payment payment)
		{
			return payment.PaymentStatus.PaymentStatusId == (int) PaymentStatusCode.Authorized ||
			       payment.PaymentStatus.PaymentStatusId == (int) PaymentStatusCode.Acquired;
		}
	}
}
