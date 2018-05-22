using System;
using Sitecore.Commerce.Pipelines;
using Sitecore.Commerce.Services.Customers;

namespace UCommerce.Sitecore.CommerceConnect.Pipelines.Customers.CreateCustomer
{
    /// <summary>
    /// Handles creation of user in uCommerce called from commerce connect. 
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
	public class CreateCustomer : ServicePipelineProcessor<CustomerRequestWithCustomer, CustomerResultWithCart>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <remarks>We need to identify if we're called from uCommerce or external API's.</remarks>
        public override void Process(ServicePipelineArgs args)
        {
	        CustomerRequestWithCustomer request;
	        CustomerResultWithCart result;
			CheckParametersAndSetupRequestAndResult(args, out request, out result);

            if (IsFromUcommerce(request)) return;

            throw new InvalidOperationException("CreateCustomer cannot be called from outside uCommerce. Please use 'UCommerce.Transactions.TransactionLibraryInternal.EditCustomer()'");
        }
    }
}
