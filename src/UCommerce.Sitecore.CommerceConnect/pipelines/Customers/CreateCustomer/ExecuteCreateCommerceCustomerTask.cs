using System;
using Sitecore.Commerce.Entities.Customers;
using Sitecore.Commerce.Services.Customers;
using UCommerce.EntitiesV2;
using UCommerce.Pipelines;
using UCommerce.Pipelines.CreateCustomer;
using UCommerce.Sitecore.CommerceConnect.Mapping;
using CreateCustomerRequest = UCommerce.Pipelines.CreateCustomer.CreateCustomerRequest;

namespace UCommerce.Sitecore.CommerceConnect.Pipelines.Customers.CreateCustomer
{
    public class ExecuteCreateCommerceCustomerTask : IPipelineTask<IPipelineArgs<CreateCustomerRequest, CreateCustomerResponse>>
    {
        private readonly IMapping<Customer, CommerceCustomer> _customerToCustomer;

        public ExecuteCreateCommerceCustomerTask(IMapping<Customer,CommerceCustomer> customerToCustomer)
        {
            _customerToCustomer = customerToCustomer;
        }

        public PipelineExecutionResult Execute(IPipelineArgs<CreateCustomerRequest, CreateCustomerResponse> subject)
        {
            if (subject.Response.Customer == null) throw new ArgumentException();

            var commerceCustomer = _customerToCustomer.Map(subject.Response.Customer);

            var customerService = new CustomerServiceProvider();

            var createCustomerRequest = new global::Sitecore.Commerce.Services.Customers.CreateCustomerRequest(commerceCustomer);
            createCustomerRequest.Properties["FromUCommerce"] = true;

            customerService.CreateCustomer(createCustomerRequest);
        
            return PipelineExecutionResult.Success;
        }
    }
}
