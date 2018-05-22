using System.Collections.Generic;
using Sitecore.Commerce.Entities;
using Sitecore.Commerce.Services.Carts;
using UCommerce.EntitiesV2;
using UCommerce.Infrastructure;
using UCommerce.Pipelines;
using UCommerce.Pipelines.AddAddress;
using UCommerce.Sitecore.CommerceConnect.Mapping;

namespace UCommerce.Sitecore.CommerceConnect.Pipelines.UCommerce.RunSitecoreCommerceAddAddressOrUpdateAddress
{
    public class RunSitecoreCommerceAddAddressOrUpdateAddress : IPipelineTask<IPipelineArgs<AddAddressRequest, AddAddressResult>>
	{
	    private readonly IMapping<OrderAddress, Party> _orderAddressMapper;
	    private readonly MappingLibraryInternal _mappingLibraryInternal;

	    public RunSitecoreCommerceAddAddressOrUpdateAddress(IMapping<OrderAddress, Party> orderAddressMapper, MappingLibraryInternal mappingLibraryInternal)
	    {
	        _orderAddressMapper = orderAddressMapper;
	        _mappingLibraryInternal = mappingLibraryInternal;
	    }

		public PipelineExecutionResult Execute(IPipelineArgs<AddAddressRequest, AddAddressResult> subject)
		{
			if (subject.Request.Properties.ContainsKey("FromUCommerce"))
				if (!(bool)subject.Request.Properties["FromUCommerce"]) return PipelineExecutionResult.Success;

			var cartServiceProvider = new CartServiceProvider();
			var cart = _mappingLibraryInternal.MapPurchaseOrderToCart(subject.Request.PurchaseOrder);

			var party = _orderAddressMapper.Map(subject.Response.OrderAddress);
			var partyList = new List<Party> { party };

			if (subject.Request.ExistingOrderAddress == null)
			{
				var addPartiesRequest = new AddPartiesRequest(cart, partyList);
                addPartiesRequest.Properties["FromUCommerce"] = true;
                var addPartiesResult = cartServiceProvider.AddParties(addPartiesRequest);

                return PipelineExecutionResult.Success;
			}

			var updatePartiesRequest = new UpdatePartiesRequest(cart, partyList);
            updatePartiesRequest.Properties["FromUCommerce"] = true;
            var updatePartiesResult = cartServiceProvider.UpdateParties(updatePartiesRequest);

			return PipelineExecutionResult.Success;
		}
	}
}
