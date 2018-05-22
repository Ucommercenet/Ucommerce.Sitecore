using System.Collections.Generic;
using System.Linq;
using Sitecore;
using Sitecore.Data;
using UCommerce.Sitecore.Extensions;
using UCommerce.Tree;

namespace UCommerce.Sitecore.SitecoreDataProvider.Impl
{
	/// <summary>
	/// This class is responsible for the handling of all the templates used by uCommerce in Sitecore.
	/// </summary>
	internal class TemplateProvider : ITemplateProvider
	{
		private ID AppearanceEditorFieldId { get { return ID.Parse("{D85DB4EC-FF89-4F9C-9E7C-A9E0654797FC}"); } }

		private readonly ITreeNodeTypeToUrlConverter _urlConverter;

		private readonly List<ITemplateBuilder> _templateBuilders;

		public TemplateProvider(IEnumerable<ITemplateBuilder> builders)
		{
			_templateBuilders = builders.ToList();
			_urlConverter = new SitecoreTreeNodeTypeToUrlConverter();
		}

		public IList<ISitecoreItem> GetTemplates()
		{
			return MakeBuildersBuildTemplates();
		}

		public FieldList GetFieldList(ITreeNodeContent node, VersionUri version)
		{
			var list = new FieldList();
			list.SafeAdd(FieldIDs.Icon, GetIconForNode(node));

			var builder = _templateBuilders.FirstOrDefault(x => x.Supports(node));

			if (builder == null)
			{
				list.SafeAdd(AppearanceEditorFieldId, GetEditorPathForNode(node));
			}
			else
			{
				builder.AddFieldValues(node, list, version);
			}
			return list;
		}

		private IList<ISitecoreItem> MakeBuildersBuildTemplates()
		{
			return _templateBuilders.SelectMany(builder => builder.BuildTemplates()).ToList();
		}

		private string GetIconForNode(ITreeNodeContent node)
		{
			return "/sitecore modules/Shell/ucommerce/shell/Content/Images/ui/" + node.Icon;
		}

		private string GetEditorPathForNode(ITreeNodeContent node)
		{
			string convertedUrl;

			if (_urlConverter.TryConvert(node, out convertedUrl))
			{
				if (string.IsNullOrEmpty(convertedUrl)) return string.Empty;
				convertedUrl = "/sitecore modules/Shell/" + convertedUrl;
				return convertedUrl;
			}

			return string.Empty;
		}
	}
}
