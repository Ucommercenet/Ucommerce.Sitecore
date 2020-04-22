using System.Linq;
using Sitecore.Data.Templates;
using Ucommerce.Sitecore.SitecoreDataProvider.Impl.SitecoreItems.Templates;

namespace Ucommerce.Sitecore.SitecoreDataProvider.Impl
{
	internal class TemplateItemToStaticTemplateConverter
	{
		public Template Convert(TemplateItem templateItem, TemplateCollection owner)
		{
			var builder = templateItem.BuildTemplate(owner);

			foreach (SectionItem section in templateItem.Children.OfType<SectionItem>())
			{
				var sectionBuilder = section.Build(builder);
				foreach (FieldItem field in section.Children.OfType<FieldItem>())
				{
					field.Build(sectionBuilder);
				}
			}

			return builder.Template;
		}
	}
}
