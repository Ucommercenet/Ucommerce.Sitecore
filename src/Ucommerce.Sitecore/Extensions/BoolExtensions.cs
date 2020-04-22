namespace Ucommerce.Sitecore.Extensions
{
	public static class BoolExtensions
	{
		public static string ToSitecoreFormat(this bool b)
		{
			return b ? "1" : "0";
		}
	}
}
