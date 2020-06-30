using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Resources.Media;
using Ucommerce.Tree;
using Ucommerce.Tree.Impl;

namespace Ucommerce.Sitecore.Content
{
	internal class SitecoreImageTreeService : ITreeContentService
	{
		private readonly ISitecoreContext _sitecoreContext;

		public SitecoreImageTreeService(ISitecoreContext sitecoreContext)
		{
			_sitecoreContext = sitecoreContext;
		}

		public virtual ITreeNodeContent GetRoot()
		{
			return new TreeNodeContent("Folder", ItemIDs.MediaLibraryRoot.ToString()) { AutoLoad = true };
		}

		public IList<ITreeNodeContent> GetChildren(string nodeType, int id)
		{
			throw new NotSupportedException();
		}

		public virtual IList<ITreeNodeContent> GetChildren(string nodeType, string id)
		{
			var item = _sitecoreContext.DatabaseForContent.GetItem(ID.Parse(id));
			var children = new List<ITreeNodeContent>();

			item.Children.ToList().ForEach(x => children.Add(CreateTreeNodeContent(x)));

			return children;
		}

		private ITreeNodeContent CreateTreeNodeContent(Item item)
        {
            string nodeType = string.Empty;
            nodeType = item.Template.Key == "media folder" ? "Folder" : "Image";
			return new TreeNodeContent(nodeType, item.ID.ToString())
			{
				ChildrenCount = item.Children.Count
			};
		}
	}
}
