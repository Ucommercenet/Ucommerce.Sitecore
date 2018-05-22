using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Sitecore.Commerce.Pipelines;
using Sitecore.Commerce.Services.Payments;
using UCommerce.Api;
using UCommerce.EntitiesV2;
using UCommerce.Infrastructure;
using UCommerce.Infrastructure.Configuration;
using UCommerce.Sitecore.CommerceConnect.Mapping;

namespace UCommerce.Sitecore.CommerceConnect.Pipelines.Payments.GetPaymentMethods
{
	public class GetPaymentMethods : ServicePipelineProcessor<GetPaymentMethodsRequest, GetPaymentMethodsResult>
	{
		public override void Process(ServicePipelineArgs args)
		{
			GetPaymentMethodsRequest request;
			GetPaymentMethodsResult result;

			using (new DisposableThreadLifestyleScope())
			{
				CheckParametersAndSetupRequestAndResult(args, out request, out result);

				if (string.IsNullOrEmpty(request.Properties["Country"] as string))
				{
					result.PaymentMethods = GetCommerceConnectPaymentMethods(null);
					return;
				}

				string countryName = (string)request.Properties["Country"];

				var countryRepository = ObjectFactory.Instance.Resolve<IRepository<Country>>();
				var country = countryRepository.Select(x => x.Name == countryName).FirstOrDefault();
				if (country == null)
				{
					throw new ArgumentException(string.Format("Could not find a country with name: \"{0}\", from Properties[\"Country\"]", countryName));
				}

				result.PaymentMethods = GetCommerceConnectPaymentMethods(country);
			}
		}

		private ReadOnlyCollection<global::Sitecore.Commerce.Entities.Payments.PaymentMethod> GetCommerceConnectPaymentMethods(Country country)
		{
			var mapper = ObjectFactory.Instance.Resolve<IMapping<PaymentMethod, global::Sitecore.Commerce.Entities.Payments.PaymentMethod>>("PaymentMethodToPaymentMethod");

			var paymentMethods = country == null ? TransactionLibrary.GetPaymentMethods() : TransactionLibrary.GetPaymentMethods(country);
			IList<global::Sitecore.Commerce.Entities.Payments.PaymentMethod> cCpaymentMethods = paymentMethods.Select(paymentMethod => mapper.Map(paymentMethod)).ToList();

			return new ReadOnlyCollection<global::Sitecore.Commerce.Entities.Payments.PaymentMethod>(cCpaymentMethods);
		}
	}
}
