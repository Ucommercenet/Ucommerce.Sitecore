namespace UCommerce.Sitecore.CommerceConnect.Mapping
{
	public interface IMapping<in TTarget, out TSource>
	{
		TSource Map(TTarget target);
	}
}
