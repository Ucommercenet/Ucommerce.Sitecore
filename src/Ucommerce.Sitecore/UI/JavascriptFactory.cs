using System.Text;
using Ucommerce.Presentation.UI;
using Ucommerce.Presentation.Web;

namespace Ucommerce.Sitecore.UI
{
    public class JavaScriptFactory : IJavaScriptFactory
    {
        private IUrlResolver UrlResolver { get; set; }

		private const string ClientMgrScript = "UCommerceClientMgr";

		public JavaScriptFactory(IUrlResolver urlResolver)
		{
			UrlResolver = urlResolver;
		}

        public string CreateJavascript(params string[] parts)
        {
            var builder = new StringBuilder();
            builder.AppendLine("<script type=\"text/javascript\">");

            builder.AppendLine("$(function() {");

            foreach (var part in parts)
            {
                if (string.IsNullOrWhiteSpace(part))
                    continue;

                builder.AppendLine(part);
            }

            builder.AppendLine("});");

            builder.AppendLine("</script>");

            return builder.ToString();
        }

        public string ContentFrameFunction(string redirectUrl)
        {
            return string.Format("{0}.contentFrame('{1}');", ClientMgrScript, UrlResolver.ResolveUrl(redirectUrl));
        }

        public string ChildNodeAddedFunction()
        {
            return string.Format("{0}.childNodeCreated();", ClientMgrScript);
        }

        public string RefreshTree()
        {
            return string.Format("{0}.refreshTree();", ClientMgrScript);
        }

	    public string RefreshChildren()
	    {
            return string.Format("{0}.refreshChildren();", ClientMgrScript);
	    }

		public string RefreshChildrenFor(string keyValuePairs)
		{
			return string.Format("{0}.refreshChildrenFor({1});",ClientMgrScript,keyValuePairs);
		}

		public string UpdateNodeText(string nodeId, string newNodeText)
        {
            return string.Format("{0}.updateNodeText('{1}', '{2}');", ClientMgrScript, nodeId, newNodeText);
        }

        public string UpdateCurrentNodeText(string newNodeText)
        {
            return string.Format("{0}.updateCurrentNodeText('{1}');", ClientMgrScript, newNodeText);
        }

        public string FindAndSelectNode(string nodeId)
        {
            return string.Format("{0}.findAndSelectNode('{1}');", ClientMgrScript, nodeId.Replace("'", "\\"));
        }

        public string CloseModalWindowFunction()
        {
            return string.Format("{0}.closeModalWindow();", ClientMgrScript);
        }

        public string CloseModalWindowFunction(string url)
        {
            return string.Format("{0}.closeModalWindow('{1}');", ClientMgrScript, url);
        }

        public string ShowSpeechBubbleFunction(bool ok, string header, string body)
        {
            var icon = ok ? "save" : "error";

            return string.Format("{0}.showSpeechBubble('{1}','{2}','{3}');", ClientMgrScript, icon, header.Replace("'", "\\'"), body.Replace("'", "\\'"));
        }

        public string OpenModalFunction(string url, string header, int width, int height)
        {
            return string.Format("{0}.openModal('{1}','{2}', {3}, {4});", ClientMgrScript, UrlResolver.ResolveUrl(url), header, width, height);
        }

        public string BuildRefreshScript(string redirectUrl)
        {
            return CreateJavascript(
                ContentFrameFunction(redirectUrl),
                ChildNodeAddedFunction(),
                CloseModalWindowFunction());

        }
    }
}
