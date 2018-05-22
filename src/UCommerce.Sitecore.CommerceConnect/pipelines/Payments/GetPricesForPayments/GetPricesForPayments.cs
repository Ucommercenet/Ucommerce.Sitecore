using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Sitecore.Commerce.Entities.Carts;
using Sitecore.Commerce.Entities.Payments;
using Sitecore.Commerce.Pipelines;
using Sitecore.Commerce.Services.Payments;
using Sitecore.Diagnostics;
using UCommerce.EntitiesV2;
using UCommerce.Infrastructure;
using UCommerce.Infrastructure.Configuration;
using PaymentMethod = UCommerce.EntitiesV2.PaymentMethod;

namespace UCommerce.Sitecore.CommerceConnect.Pipelines.Payments.GetPricesForPayments
{
	public class GetPricesForPayments : ServicePipelineProcessor<GetPricesForPaymentsRequest, GetPricesForPaymentsResult>
	{
		public override void Process(ServicePipelineArgs args)
		{
			GetPricesForPaymentsRequest request;
			GetPricesForPaymentsResult result;

			using (new DisposableThreadLifestyleScope())
			{
				CheckParametersAndSetupRequestAndResult(args, out request, out result);

				Assert.ArgumentNotNull(request.PaymentLookup, "args.Request.PaymentLookup");

				var paymentPrices = new List<PaymentPrice>();

				foreach (var paymentLookup in request.PaymentLookup)
				{
					paymentPrices.Add(GetPaymentPrice(paymentLookup, request.Cart));
				}

				result.PaymentPrices = new ReadOnlyCollection<PaymentPrice>(paymentPrices);
			}
		}

		private PaymentPrice GetPaymentPrice(PaymentLookup paymentLookup, Cart cart)
		{
			var paymentMethodId = ParseToInt(paymentLookup.MethodId);
			var paymentMethod = GetPaymentMethodInternal(paymentMethodId);
			var currency = GetCurrencyCode(cart.CurrencyCode);

			var price = new PaymentPrice()
			{
				Amount = paymentMethod.GetFeeForCurrency(currency).Fee,
				CurrencyCode = cart.CurrencyCode,
				LineItemIds = paymentLookup.LineItemIds,
				MethodId = paymentLookup.MethodId
			};

			price.SetPropertyValue("FeePercent", paymentMethod.FeePercent);

			return price;
		}

		private Currency GetCurrencyCode(string currencyCode)
		{
			var currencyRepository = ObjectFactory.Instance.Resolve<IRepository<Currency>>();
			var currency = currencyRepository.Select(x => x.ISOCode == currencyCode).FirstOrDefault();

			if (currency == null)
			{
				throw new ArgumentException(string.Format("No currency with currency code: \"{0}\", from Cart.CurrencyCode", currencyCode));
			}

			return currency;
		}

		private int ParseToInt(string externalId)
		{
			int paymentMethodId;
			if (!int.TryParse(externalId, out paymentMethodId))
			{
				throw new ArgumentException(string.Format("Method id must be a number. The method id provided was \"{0}\".", externalId));
			}

			return paymentMethodId;
		}

		private PaymentMethod GetPaymentMethodInternal(int paymentMethodId)
		{
			var paymentMethodRepository = ObjectFactory.Instance.Resolve<IRepository<PaymentMethod>>();
			var paymentMethod = paymentMethodRepository.Select(x => x.PaymentMethodId == paymentMethodId).FirstOrDefault();
			if (paymentMethod == null)
			{
				throw new ArgumentException(string.Format("No payment method with id: {0} could be found.", paymentMethodId));
			}

			return paymentMethod;
		}
	}
}
