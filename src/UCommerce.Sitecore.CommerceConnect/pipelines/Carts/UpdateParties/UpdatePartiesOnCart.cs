using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Sitecore.Commerce.Entities;
using Sitecore.Commerce.Entities.Carts;
using Sitecore.Commerce.Pipelines;
using Sitecore.Commerce.Services.Carts;
using Sitecore.Diagnostics;
using UCommerce.EntitiesV2;
using UCommerce.Infrastructure;
using UCommerce.Infrastructure.Configuration;
using UCommerce.Pipelines;
using UCommerce.Pipelines.AddAddress;
using UCommerce.Sitecore.CommerceConnect.Mapping;
using UCommerce.Sitecore.CommerceConnect.Pipelines.Orders;

namespace UCommerce.Sitecore.CommerceConnect.Pipelines.Carts.UpdateParties
{
	public class UpdatePartiesOnCart : ServicePipelineProcessor<UpdatePartiesRequest, UpdatePartiesResult>
	{
		private readonly IBasketService _basketService;

		public UpdatePartiesOnCart(IBasketService basketService)
		{
			_basketService = basketService;
		}

		public override void Process(ServicePipelineArgs args)
		{
			UpdatePartiesRequest request;
			UpdatePartiesResult result;

			using (new DisposableThreadLifestyleScope())
			{
				CheckParametersAndSetupRequestAndResult(args, out request, out result);

				Assert.ArgumentNotNull(request.Parties, "request.parties");
				Assert.ArgumentNotNull(request.Cart, "request.Cart");

				IList<Party> parties = new List<Party>();

				foreach (var party in request.Parties)
				{
					var addedParty = UpdatePartyOnCart(party, request);
					parties.Add(addedParty);
				}

				result.Parties = new ReadOnlyCollection<Party>(parties);
				result.Cart = UpdatedCart(request.Cart);
			}
		}

		private Party UpdatePartyOnCart(Party party, UpdatePartiesRequest request)
		{
			var purchaseOrder = _basketService.GetBasketByCartExternalId(request.Cart.ExternalId).PurchaseOrder;

			var addressToUpdate = purchaseOrder.OrderAddresses.FirstOrDefault(x => x.AddressName == party.PartyId);

			if (addressToUpdate != null)
			{
				party = UpdateOrderAddress(addressToUpdate, party, request);
			}

			return party;
		}

		private Cart UpdatedCart(Cart requestCart)
		{
			var purchaseOrderMapper = ObjectFactory.Instance.Resolve<IMapping<PurchaseOrder, Cart>>();
			var purchaseOrder = _basketService.GetBasketByCartExternalId(requestCart.ExternalId).PurchaseOrder;

			return purchaseOrderMapper.Map(purchaseOrder);
		}

		private Party UpdateOrderAddress(OrderAddress address, Party party, UpdatePartiesRequest request)
		{
			var orderAddressMapper = ObjectFactory.Instance.Resolve<IMapping<OrderAddress, Party>>();

			if (IsFromUcommerce(request))
			{
				return orderAddressMapper.Map(address);
			}

			var addAddressPipeline = PipelineFactory.Create<IPipelineArgs<AddAddressRequest, AddAddressResult>>("AddAddress");

			var addAddressPipelineArgs = new AddAddressPipelineArgs(new AddAddressRequest()
			{
				PurchaseOrder = address.PurchaseOrder,
				ExistingOrderAddress = address,
				AddressName = party.PartyId,
				FirstName = party.FirstName,
				LastName = party.LastName,
				EmailAddress = party.Email,
				Line1 = party.Address1,
				Line2 = party.Address2,
				PostalCode = party.ZipPostalCode,
				City = party.City,
				State = party.State,
				CountryId = FindCountryIdByName(party.Country),
				PhoneNumber = party.PhoneNumber
			}, new AddAddressResult());

			addAddressPipelineArgs.Request.Properties["FromUCommerce"] = false;
			addAddressPipeline.Execute(addAddressPipelineArgs);

			return orderAddressMapper.Map(addAddressPipelineArgs.Response.OrderAddress);
		}

		private int FindCountryIdByName(string countryName)
		{
			// Note! Very important not to initialize a repository in the constructor!
			// That would create a repository in the context that the Commerce Connect pipeline was created in.
			// Typically on a background thread... - Jesper

			var country = Country.All().FirstOrDefault(x => x.Name == countryName);

			if (country == null)
			{
				throw new ArgumentException("The country specified in the party to add could not be found in uCommerce: " + countryName);
			}

			return country.CountryId;
		}
	}
}
