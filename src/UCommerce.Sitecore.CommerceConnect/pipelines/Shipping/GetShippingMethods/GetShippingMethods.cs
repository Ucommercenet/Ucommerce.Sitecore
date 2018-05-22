using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Sitecore.Commerce.Pipelines;
using Sitecore.Commerce.Services.Shipping;
using UCommerce.Api;
using UCommerce.EntitiesV2;
using UCommerce.Infrastructure;
using UCommerce.Infrastructure.Configuration;
using UCommerce.Sitecore.CommerceConnect.Mapping;

namespace UCommerce.Sitecore.CommerceConnect.Pipelines.Shipping.GetShippingMethods
{
	public class GetShippingMethods : ServicePipelineProcessor<GetShippingMethodsRequest, GetShippingMethodsResult>
	{
		public override void Process(ServicePipelineArgs args)
		{
			GetShippingMethodsRequest request;
			GetShippingMethodsResult result;
			using (new DisposableThreadLifestyleScope())
			{
				CheckParametersAndSetupRequestAndResult(args, out request, out result);

				if (request.Party == null || string.IsNullOrEmpty(request.Party.Country))
				{
					result.ShippingMethods = GetCommerceConnectShippingMethods(null);
					return;
				}

				var countryRepository = ObjectFactory.Instance.Resolve<IRepository<Country>>();
				var country = countryRepository.Select(x => x.Name == request.Party.Country).FirstOrDefault();
				if (country == null)
				{
					throw new ArgumentException(string.Format("Could not find a country with name: \"{0}\", from Party.Country", request.Party.Country));
				}

				result.ShippingMethods = GetCommerceConnectShippingMethods(country);
			}
		}

		private ReadOnlyCollection<global::Sitecore.Commerce.Entities.Shipping.ShippingMethod> GetCommerceConnectShippingMethods(Country country)
		{
			var mapper = ObjectFactory.Instance.Resolve<IMapping<ShippingMethod, global::Sitecore.Commerce.Entities.Shipping.ShippingMethod>>("ShippingMethodToShippingMethod");

			var shippingMethods = country == null ? TransactionLibrary.GetShippingMethods() : TransactionLibrary.GetShippingMethods(country);
			IList<global::Sitecore.Commerce.Entities.Shipping.ShippingMethod> cCshippingMethods = shippingMethods.Select(shippingMethod => mapper.Map(shippingMethod)).ToList();

			return new ReadOnlyCollection<global::Sitecore.Commerce.Entities.Shipping.ShippingMethod>(cCshippingMethods);
		}
	}
}
