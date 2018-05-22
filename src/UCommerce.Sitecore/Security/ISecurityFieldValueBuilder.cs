using UCommerce.EntitiesV2;

namespace UCommerce.Sitecore.Security
{
    /// <summary>
    /// Service for building the Security field value determining the permissions for Ucommerce items in Sitecore.
    /// </summary>
    public interface ISecurityFieldValueBuilder
    {
        /// <summary>
        /// Builds the Security field value for a <see cref="ProductCatalogGroup"/>.
        /// </summary>
        /// <param name="shop">The shop to build the permissions for.</param>
        /// <returns>The string value representing the permissions.</returns>
        string BuildSecurityValue(ProductCatalogGroup shop);

        /// <summary>
        /// Builds the Security field value for a <see cref="ProductCatalog"/>.
        /// </summary>
        /// <param name="catalog">The catalog to build the permissions for.</param>
        /// <returns>The string value representing the permissions.</returns>
        string BuildSecurityValue(ProductCatalog catalog);
    }
}
