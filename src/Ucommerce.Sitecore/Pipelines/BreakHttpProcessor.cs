using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Sitecore.Pipelines.HttpRequest;
using Sitecore.Xml;

namespace Ucommerce.Sitecore.Pipelines
{
	/// <summary>
	/// PipelineTask used in the sitecore HttpProcessor that breaks the pipeline if we're trying to
	/// call a webservice or other internal pages. Otherwise Sitecore will treat the request
	/// like every other and try to reslove the request to a contentpage.
	/// </summary>
	public class BreakHttpProcessor : HttpRequestProcessor
	{
		private readonly List<string> _breakOnTheseStrings = new List<string>();
		public void AddBreakOnThisPattern(XmlNode node)
		{
			var breakMatch = XmlUtil.GetAttribute("text", node);
			_breakOnTheseStrings.Add(breakMatch);
		}

		/// <summary>
		/// Task called in the pipeline that breaks the request if the request contains one of the configured patterns.
		/// </summary>
		/// <param name="args">Object containing alot of information about the current HttpRequest.</param>
		public override void Process(HttpRequestArgs args)
		{
			if (IsUcommerceInternalRequest(args))
			{
				args.AbortPipeline();
			}
		}

		/// <summary>
		/// Determines if this request matches one of the configured break strings.
		/// </summary>
		/// <param name="args">Object containing alot of information about the current HttpRequest.</param>
		/// <returns>True if the LocalPath contains one of the configured breakMatch'es.</returns>
		private bool IsUcommerceInternalRequest(HttpRequestArgs args)
		{
			return _breakOnTheseStrings.Any(breakMatch => args.LocalPath.ToLower().Contains(breakMatch));
		}
	}
}
