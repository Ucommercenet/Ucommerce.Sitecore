using System;
using System.Linq;
using Sitecore.Commerce.Pipelines;
using Sitecore.Commerce.Services.Shipping;
using UCommerce.EntitiesV2;
using UCommerce.Infrastructure;
using UCommerce.Infrastructure.Configuration;
using UCommerce.Sitecore.CommerceConnect.Mapping;

namespace UCommerce.Sitecore.CommerceConnect.Pipelines.Shipping.GetShippingMethod
{
	public class GetShippingMethod : ServicePipelineProcessor<GetShippingMethodRequest, GetShippingMethodResult>
	{
		public override void Process(ServicePipelineArgs args)
		{
			GetShippingMethodRequest request;
			GetShippingMethodResult result;

			using (new DisposableThreadLifestyleScope())
			{
				CheckParametersAndSetupRequestAndResult(args, out request, out result);

				var shippingMethodId = ParseToInt(request.ExternalId);
				var shippingMethod = GetShippingMethodInternal(shippingMethodId);

				var mapper = ObjectFactory.Instance.Resolve<IMapping<ShippingMethod, global::Sitecore.Commerce.Entities.Shipping.ShippingMethod>>("ShippingMethodToShippingMethod");
				result.ShippingMethod = mapper.Map(shippingMethod);
			}
		}

		private int ParseToInt(string externalId)
		{
			int shippingMethodId;
			if (!int.TryParse(externalId, out shippingMethodId))
			{
				throw new ArgumentException(string.Format("External id must to be a number, external id provided was {0}.", externalId));
			}

			return shippingMethodId;
		}

		private ShippingMethod GetShippingMethodInternal(int shippingMethodId)
		{
			var shippingMethodRepository = ObjectFactory.Instance.Resolve<IRepository<ShippingMethod>>();
			var shippingMethod = shippingMethodRepository.Select(x => x.Id == shippingMethodId).FirstOrDefault();
			if (shippingMethod == null)
			{
				throw new ArgumentException(string.Format("No shipping method with external id: {0} could be found.", shippingMethodId));
			}

			return shippingMethod;
		}
	}
}
