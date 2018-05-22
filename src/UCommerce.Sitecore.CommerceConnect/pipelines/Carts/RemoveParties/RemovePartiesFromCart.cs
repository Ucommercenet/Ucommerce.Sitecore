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
using UCommerce.Sitecore.CommerceConnect.Mapping;
using UCommerce.Sitecore.CommerceConnect.Pipelines.Orders;

namespace UCommerce.Sitecore.CommerceConnect.Pipelines.Carts.RemoveParties
{
    public class RemovePartiesFromCart : ServicePipelineProcessor<RemovePartiesRequest, RemovePartiesResult>
    {
        private readonly IBasketService _basketService;
        private readonly IMapping<PurchaseOrder, Cart> _purchaseOrderMapper;

        public RemovePartiesFromCart(IBasketService basketService)
        {
            _basketService = basketService;
            _purchaseOrderMapper = ObjectFactory.Instance.Resolve<IMapping<PurchaseOrder, Cart>>();
        }

        public override void Process(ServicePipelineArgs args)
        {
	        RemovePartiesRequest request;
	        RemovePartiesResult result;

			using (new DisposableThreadLifestyleScope())
            {
                CheckParametersAndSetupRequestAndResult(args, out request, out result);

                Assert.ArgumentNotNull(request.Parties, "request.parties");
                Assert.ArgumentNotNull(request.Cart, "request.Cart");

                IList<Party> parties = new List<Party>();

                foreach (var party in request.Parties)
                {
                    var addedParty = RemovePartyFromCart(party, request.Cart);
                    parties.Add(addedParty);
                }

                result.Parties = new ReadOnlyCollection<Party>(parties);
                result.Cart = UpdatedCart(request.Cart);
            }
        }

        private Party RemovePartyFromCart(Party party, Cart cart)
        {
            var purchaseOrder = _basketService.GetBasketByCartExternalId(cart.ExternalId).PurchaseOrder;

            var addressToRemove = purchaseOrder.OrderAddresses.FirstOrDefault(x => x.AddressName == party.PartyId);

            // No specific RemoveOrderAddress method?!
            if (addressToRemove != null)
            {
                purchaseOrder.OrderAddresses.Remove(addressToRemove);
            }

            return party;
        }

        private Cart UpdatedCart(Cart requestCart)
        {
            var purchaseOrder = _basketService.GetBasketByCartExternalId(requestCart.ExternalId).PurchaseOrder;
            return _purchaseOrderMapper.Map(purchaseOrder);
        }
    }
}
