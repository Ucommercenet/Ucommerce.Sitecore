namespace UCommerce.Sitecore.Settings
{
    /// <summary>
    /// Settings controlling the behaviour of the Data Provider
    /// </summary>
    public class DataProviderSettings
    {
        /// <summary>
        /// Override this to true, to include the products category list in product items data in Sitecore.
        /// </summary>
        public bool IncludeProductCategoryRelationsData { get; set; }

        /// <summary>
        /// Override this to true, to include the products relations to other products in product items data in Sitecore.
        /// </summary>
        public bool IncludeProductRelationsData { get; set; }

        /// <summary>
        /// Override this to false, to exclude the product data items in Sitecore.
        /// </summary>
        public bool IncludeProductData { get; set; }

        public bool EnforceUniquenessOfCategoryNames { get; set; }
    }
}
