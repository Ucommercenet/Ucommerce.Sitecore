using System.Collections.ObjectModel;
using System.Linq;
using Sitecore.Commerce.Entities.Customers;
using UCommerce.EntitiesV2;

namespace UCommerce.Sitecore.CommerceConnect.Mapping.ToCommerceConnect.Customer
{
    public class CustomerToCommerceCustomer : IMapping<EntitiesV2.Customer,CommerceCustomer>
    {
        private readonly IMapping<IAddress, CustomerParty> _addressToCustomerParty;
        private readonly IRepository<ProductCatalogGroup> _productCatalogGroupRepository;

        public CustomerToCommerceCustomer(IMapping<IAddress, CustomerParty> addressToCustomerParty, IRepository<ProductCatalogGroup> productCatalogGroupRepository)
        {
            _addressToCustomerParty = addressToCustomerParty;
            _productCatalogGroupRepository = productCatalogGroupRepository;
        }

        public CommerceCustomer Map(EntitiesV2.Customer target)
        {
            var commerceCustomer = new CommerceCustomer();

            commerceCustomer.Shops = new ReadOnlyCollection<string>(_productCatalogGroupRepository.Select().Select(x => x.Name).ToList());
            commerceCustomer.IsDisabled = false;
            commerceCustomer.Name = string.Format("{0} {1}", target.FirstName, target.LastName);
            
            commerceCustomer.Parties = new ReadOnlyCollection<CustomerParty>(target.Addresses.Select(x => _addressToCustomerParty.Map(x)).ToList());
            commerceCustomer.ExternalId = target.CustomerId.ToString();
            
            return commerceCustomer;
        }
    }
}
