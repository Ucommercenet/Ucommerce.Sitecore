using System.Collections.Generic;
using System.Linq;
using Sitecore;
using Sitecore.Data;
using Ucommerce.Tree;
using Ucommerce.Tree.Impl;

namespace Ucommerce.Sitecore.Content
{
	class SitecoreContentTreeService : ITreeContentService
	{
		private readonly ISitecoreContext _sitecoreContext;

		public SitecoreContentTreeService(ISitecoreContext sitecoreContext)
		{
			_sitecoreContext = sitecoreContext;
		}

		public virtual ITreeNodeContent GetRoot()
		{
			return new TreeNodeContent("folder", ItemIDs.ContentRoot.ToString()) { AutoLoad = true };
		}

		public virtual IList<ITreeNodeContent> GetChildren(string nodeType, string id)
		{
			var item = _sitecoreContext.DatabaseForContent.GetItem(ID.Parse(id));
			var children = new List<ITreeNodeContent>();

			item.Children.ToList().ForEach(x => children.Add(new TreeNodeContent("content", x.ID.ToString())));

			return children;
		}
	}
}
