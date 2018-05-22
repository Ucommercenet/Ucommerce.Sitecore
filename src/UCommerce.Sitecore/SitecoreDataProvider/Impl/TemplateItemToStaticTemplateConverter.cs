using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Data.Templates;
using UCommerce.Sitecore.SitecoreDataProvider.Impl.SitecoreItems.Templates;

namespace UCommerce.Sitecore.SitecoreDataProvider.Impl
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
