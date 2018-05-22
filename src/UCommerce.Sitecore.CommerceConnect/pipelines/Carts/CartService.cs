using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Commerce.Entities.Carts;
using Sitecore.Commerce.Services.Carts;
using UCommerce.EntitiesV2;
using UCommerce.Infrastructure;
using UCommerce.Infrastructure.Configuration;
using UCommerce.Sitecore.CommerceConnect.Mapping;

namespace UCommerce.Sitecore.CommerceConnect.Pipelines.Carts
{
    public class CartService : ICartService
    {
        public IList<CartBase> GetAll(GetCartsRequest getCartsRequest)
        {
            using (new DisposableThreadLifestyleScope())
            {
                var userIds = getCartsRequest.UserIds.ToList();
                var customerIds = getCartsRequest.CustomerIds.ToList();
                var names = getCartsRequest.Names.ToList();
                var statuses = getCartsRequest.Statuses;
                var isLocked = getCartsRequest.IsLocked;

                var repository = ObjectFactory.Instance.Resolve<IRepository<PurchaseOrder>>();

                var purchaseOrders =
                    repository.Select()
                        .Where(x => (!customerIds.Any() || customerIds.Contains(x.Customer.CustomerId.ToString())) &&
                                    (!userIds.Any() ||
                                     x.OrderProperties.Any(
                                         y => y.Key == "CommerceConnectUserIdentifier" && userIds.Contains(y.Value))) &&
                                    (!names.Any() || names.Contains(x.ProductCatalogGroup.DomainId)) &&
                                    x.OrderStatus.OrderStatusId == 1);

                var cartBases = new List<CartBase>();

                foreach (var purchaseOrder in purchaseOrders)
                {
                    cartBases.Add(
                        ObjectFactory.Instance.Resolve<IMapping<PurchaseOrder, CartBase>>("PurchaseOrderToCartBase")
                            .Map(purchaseOrder));
                }
                
                return cartBases.Distinct().ToList();
            }
        }

        public IQueryable<CartBase> GetByUserName(string userName, string shopName)
        {
            throw new NotImplementedException();
        }

        public Cart GetByCartId(string cartId, string shopName)
        {
            using (new DisposableThreadLifestyleScope())
            {
                Guid BasketId;
                var isGuid = Guid.TryParse(cartId, out BasketId);
                if (!isGuid) throw new ArgumentException("CardId was not a guid");

                var repository = ObjectFactory.Instance.Resolve<IRepository<PurchaseOrder>>();
                var purchaseOrder = repository.SingleOrDefault(x => x.BasketId == BasketId);

                if (purchaseOrder == null) return null;

                return ObjectFactory.Instance.Resolve<IMapping<PurchaseOrder, Cart>>("PurchaseOrderToCart").Map(purchaseOrder);
            }
        }
    }
}
