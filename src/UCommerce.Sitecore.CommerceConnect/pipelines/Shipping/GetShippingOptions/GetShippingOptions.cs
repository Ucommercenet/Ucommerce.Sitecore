using System.Collections.Generic;
using System.Collections.ObjectModel;
using Sitecore.Commerce.Entities.Shipping;
using Sitecore.Commerce.Pipelines;
using Sitecore.Commerce.Services.Shipping;

namespace UCommerce.Sitecore.CommerceConnect.Pipelines.Shipping.GetShippingOptions
{
	public class GetShippingOptions : ServicePipelineProcessor<GetShippingOptionsRequest, GetShippingOptionsResult>
	{
		private readonly IList<ShippingOption> _shippingOptions; 

		public GetShippingOptions()
		{
			_shippingOptions = new List<ShippingOption>()
			{
				new ShippingOption()
				{
					Name = "ShipToAddress",
					ShippingOptionType = ShippingOptionType.ShipToAddress
				}
			};
		}

		public override void Process(ServicePipelineArgs args)
		{
			GetShippingOptionsRequest request;
			GetShippingOptionsResult result;

			CheckParametersAndSetupRequestAndResult(args, out request, out result);

			result.ShippingOptions = new ReadOnlyCollection<ShippingOption>(_shippingOptions);
		}
	}
}
