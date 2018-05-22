using UCommerce.EntitiesV2;

namespace UCommerce.Sitecore.CommerceConnect.Pipelines.Orders
{
    public interface IBasketService
    {
        Basket CreateBasket(bool internalRequest);
	    Basket GetBasketByCartExternalId(string externalId);
    }
}
