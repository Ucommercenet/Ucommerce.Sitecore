using System;
using System.Collections.Generic;
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Items;
using Ucommerce.Infrastructure;
using Ucommerce.Security;
using Ucommerce.Sitecore.Security;
using Ucommerce.Tree;
using Ucommerce.Sitecore.Extensions;

namespace Ucommerce.Sitecore.SitecoreDataProvider.Impl.TemplateBuilders
{
	public class StoresTemplateBuilder : ITemplateBuilder
	{
		private readonly Guid _revision = new Guid("{CE7E0A41-A0CD-4208-928E-0F2F77D72A32}");

		public ID GetTemplateId(ITreeNodeContent node)
		{
			return FieldIds.Common.UCommerceStoresTemplateId;
		}

		public void AddFieldValues(ITreeNodeContent node, FieldList list, VersionUri version)
		{
			list.SafeAdd(FieldIDs.Security,ObjectFactory.Instance.Resolve<ISecurityService>().GetSecurityStringForNodes());
		}

		public bool SaveItem(ITreeNodeContent node, ItemChanges changes)
		{
			return false;
		}

		public bool Supports(ITreeNodeContent node)
		{
			return node.NodeType == Constants.DataProvider.NodeType.Catalog;
		}

		public IEnumerable<ISitecoreItem> BuildTemplates()
		{
			var templateData = new List<ISitecoreItem>();
			var builder = new TemplateBuilder();

			builder.CreateTemplate("uCommerce stores Template", FieldIds.Common.UCommerceStoresTemplateId, "uCommerce stores Template", TemplateIDs.StandardTemplate);

			var template = builder.Build();

			template.AddToFieldList(FieldIDs.Icon, SitecoreConstants.UCommerceIconFolder + "/ui/map.png");
			template.SetRevision(_revision);
			templateData.Add(template);

			return templateData;
		}
	}
}
