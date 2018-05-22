namespace UCommerce.Sitecore.CommerceConnect.Mapping
{
	public interface IMapValues<in TSource, in TTarget>
	{
		void MapValues(TSource source, TTarget target);
	}
}
