using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Sitecore.Commerce.Entities.Carts;
using Sitecore.Commerce.Entities.Shipping;
using Sitecore.Commerce.Pipelines;
using Sitecore.Commerce.Services.Shipping;
using Sitecore.Diagnostics;
using UCommerce.EntitiesV2;
using UCommerce.Infrastructure;
using UCommerce.Infrastructure.Configuration;
using ShippingMethod = UCommerce.EntitiesV2.ShippingMethod;

namespace UCommerce.Sitecore.CommerceConnect.Pipelines.Shipping.GetPricesForShipments
{
	public class GetPricesForShipments : ServicePipelineProcessor<GetPricesForShipmentsRequest, GetPricesForShipmentsResult>
	{
		public override void Process(ServicePipelineArgs args)
		{
			GetPricesForShipmentsRequest request;
			GetPricesForShipmentsResult result;

			using (new DisposableThreadLifestyleScope())
			{
				CheckParametersAndSetupRequestAndResult(args, out request, out result);

				Assert.ArgumentNotNull(request.ShippingLookup, "args.Request.ShippingLookup");

				var shippingPrices = new List<ShippingPrice>();

				foreach (var shippingLookup in request.ShippingLookup)
				{
					shippingPrices.Add(GetShippingPrice(shippingLookup, request.Cart));
				}

				result.ShippingPrices = new ReadOnlyCollection<ShippingPrice>(shippingPrices);
			}
		}

		private ShippingPrice GetShippingPrice(ShippingLookup shippingLookup, Cart cart)
		{
			var shippingMethodId = ParseToInt(shippingLookup.MethodId);
			var shippingMethod = GetShippingMethodInternal(shippingMethodId);
			var currency = GetCurrencyCode(cart.CurrencyCode);

			return new ShippingPrice()
			{
				Amount = shippingMethod.GetPriceForCurrency(currency).Price,
				CurrencyCode = cart.CurrencyCode,
				LineItemIds = shippingLookup.LineItemIds,
				MethodId = shippingLookup.MethodId
			};
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
			int shippingMethodId;
			if (!int.TryParse(externalId, out shippingMethodId))
			{
				throw new ArgumentException(string.Format("Method id must to be a number, method id provided was \"{0}\".", externalId));
			}

			return shippingMethodId;
		}

		private ShippingMethod GetShippingMethodInternal(int shippingMethodId)
		{
			var shippingMethodRepository = ObjectFactory.Instance.Resolve<IRepository<ShippingMethod>>();
			var shippingMethod = shippingMethodRepository.Select(x => x.ShippingMethodId == shippingMethodId).FirstOrDefault();
			if (shippingMethod == null)
			{
				throw new ArgumentException(string.Format("No shipping method with id: {0} could be found.", shippingMethodId));
			}

			return shippingMethod;
		}
	}
}
