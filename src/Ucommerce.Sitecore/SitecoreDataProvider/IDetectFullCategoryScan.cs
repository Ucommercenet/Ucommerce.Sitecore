namespace Ucommerce.Sitecore.SitecoreDataProvider
{
	public interface IDetectFullCategoryScan
	{
		bool FullCatalogScanInProgress { get; }

		bool ThreadIsScanningFullCatalog { get; }
	}
}
