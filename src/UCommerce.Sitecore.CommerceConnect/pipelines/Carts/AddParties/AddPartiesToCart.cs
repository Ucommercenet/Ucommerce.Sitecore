using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Sitecore.Commerce.Entities;
using Sitecore.Commerce.Entities.Carts;
using Sitecore.Commerce.Pipelines;
using Sitecore.Diagnostics;
using UCommerce.EntitiesV2;
using UCommerce.Infrastructure;
using UCommerce.Infrastructure.Configuration;
using UCommerce.Pipelines;
using UCommerce.Pipelines.AddAddress;
using UCommerce.Sitecore.CommerceConnect.Mapping;
using UCommerce.Sitecore.CommerceConnect.Pipelines.Orders;
using AddPartiesRequest = Sitecore.Commerce.Services.Carts.AddPartiesRequest;
using AddPartiesResult = Sitecore.Commerce.Services.Carts.AddPartiesResult;
using PipelineFactory = UCommerce.Pipelines.PipelineFactory;

namespace UCommerce.Sitecore.CommerceConnect.Pipelines.Carts.AddParties
{
    public class AddPartiesToCart : ServicePipelineProcessor<AddPartiesRequest, AddPartiesResult>
    {
        private readonly IBasketService _basketService;

        public AddPartiesToCart(IBasketService basketService)
        {
            _basketService = basketService;
        }

        public override void Process(ServicePipelineArgs args)
        {
            AddPartiesRequest request;
            AddPartiesResult result;
            using (new DisposableThreadLifestyleScope())
            {
                CheckParametersAndSetupRequestAndResult(args, out request, out result);

                Assert.ArgumentNotNull(request.Parties, "request.Parties");
                Assert.ArgumentNotNull(request.Cart, "request.Cart");

                IList<Party> parties = new List<Party>();

                foreach (var party in request.Parties)
                {
                    var addedParty = AddPartyToCart(party, request);
                    parties.Add(addedParty);
                }

                result.Parties = new ReadOnlyCollection<Party>(parties);
                var cart = UpdatedCart(request.Cart);

                SetAccountingCustomerPartyAndBuyerCustomerParty(cart);

                result.Cart = cart;
            }
        }

        private static void SetAccountingCustomerPartyAndBuyerCustomerParty(Cart cart)
        {
            foreach (var party in cart.Parties)
            {
                if (party.PartyId == "Billing")
                {
                    cart.AccountingCustomerParty = new CartParty()
                    {
                        ExternalId = party.ExternalId,
                        PartyID = party.PartyId
                    };
                }

                if (party.PartyId == Constants.DefaultShipmentAddressName)
                {
                    cart.BuyerCustomerParty = new CartParty()
                    {
                        ExternalId = party.ExternalId,
                        PartyID = party.PartyId
                    };
                }
            }
        }

        private Party AddPartyToCart(Party party, AddPartiesRequest request)
        {
            if (IsFromUcommerce(request))
            {
                return party;
            }

            var addAddressPipeline = PipelineFactory.Create<IPipelineArgs<AddAddressRequest, AddAddressResult>>("AddAddress");
            var orderAddressMapper = ObjectFactory.Instance.Resolve<IMapping<OrderAddress, Party>>();
            var purchaseOrder = _basketService.GetBasketByCartExternalId(request.Cart.ExternalId).PurchaseOrder;

            var addAddressPipelineArgs = new AddAddressPipelineArgs(new AddAddressRequest()
            {
                PurchaseOrder = purchaseOrder,
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
                PhoneNumber = party.PhoneNumber,
                Company = party.Company
            }, new AddAddressResult());

            addAddressPipelineArgs.Request.Properties["FromUCommerce"] = false;
            addAddressPipeline.Execute(addAddressPipelineArgs);
            party = orderAddressMapper.Map(addAddressPipelineArgs.Response.OrderAddress);

            if (party.PartyId == "Billing")
            {
                purchaseOrder.BillingAddress = addAddressPipelineArgs.Response.OrderAddress;
                purchaseOrder.Save();
            }


            return party;
        }

        private Cart UpdatedCart(Cart requestCart)
        {
            var purchaseOrderMapper = ObjectFactory.Instance.Resolve<IMapping<PurchaseOrder, Cart>>();
            var purchaseOrder = _basketService.GetBasketByCartExternalId(requestCart.ExternalId).PurchaseOrder;

            return purchaseOrderMapper.Map(purchaseOrder);
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
