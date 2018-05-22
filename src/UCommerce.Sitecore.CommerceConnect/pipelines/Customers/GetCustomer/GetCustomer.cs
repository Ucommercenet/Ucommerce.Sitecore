using System;
using Sitecore.Commerce.Entities.Customers;
using Sitecore.Commerce.Pipelines;
using Sitecore.Commerce.Services.Customers;
using UCommerce.EntitiesV2;
using UCommerce.Infrastructure;
using UCommerce.Sitecore.CommerceConnect.Mapping;

namespace UCommerce.Sitecore.CommerceConnect.Pipelines.Customers.GetCustomer
{
	public class GetCustomer : ServicePipelineProcessor<GetCustomerRequest, GetCustomerResult>
    {
        public override void Process(ServicePipelineArgs args)
        {
	        GetCustomerRequest request;
	        GetCustomerResult result;

			CheckParametersAndSetupRequestAndResult(args, out request, out result);

            var customer = ObjectFactory.Instance.Resolve<IRepository<Customer>>().Get(Convert.ToInt32(request.ExternalId));
            result.CommerceCustomer = ObjectFactory.Instance.Resolve<IMapping<Customer, CommerceCustomer>>().Map(customer);
        }
    }
}
