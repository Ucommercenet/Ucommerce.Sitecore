using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Items;
using UCommerce.Infrastructure;
using UCommerce.Security;
using UCommerce.Sitecore.Extensions;
using UCommerce.Sitecore.Security;
using UCommerce.Tree;

namespace UCommerce.Sitecore.SitecoreDataProvider.Impl.TemplateBuilders
{
	public class UCommerceTemplateBuilder : ITemplateBuilder
	{
		private readonly Guid _revision = new Guid("{B2D20329-5879-4B92-8952-938C32BE1F2B}");

		public ID GetTemplateId(ITreeNodeContent node)
		{
			return FieldIds.Common.UCommerceRootTemplateId;
		}

		public void AddFieldValues(ITreeNodeContent node, FieldList list, VersionUri version)
		{
			list.SafeAdd(FieldIDs.Security, ObjectFactory.Instance.Resolve<ISecurityService>().GetSecurityStringForNodes());			
		}

		public bool SaveItem(ITreeNodeContent node, ItemChanges changes)
		{
			return false;
		}

		public bool Supports(ITreeNodeContent node)
		{
			return node.NodeType == Constants.DataProvider.NodeType.Root;
		}

		public IEnumerable<ISitecoreItem> BuildTemplates()
		{
			var templateData = new List<ISitecoreItem>();
			var builder = new TemplateBuilder();

			builder.CreateTemplate("uCommerceTemplate", FieldIds.Common.UCommerceRootTemplateId, "uCommerce Template", TemplateIDs.StandardTemplate);

			var template = builder.Build();

			template.AddToFieldList(FieldIDs.Icon, SitecoreConstants.UCommerceIconFolder + "/ui/map.png");
			template.SetRevision(_revision);

			templateData.Add(template);

			return templateData;
		}
	}
}
