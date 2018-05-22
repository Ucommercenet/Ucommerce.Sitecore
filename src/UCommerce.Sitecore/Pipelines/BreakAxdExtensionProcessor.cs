using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Pipelines.HttpRequest;

namespace UCommerce.Sitecore.Pipelines
{
	/// <summary>
	/// Breaks all the axd requests.
	/// </summary>
	/// <remarks>
	/// This is done because we need to allow our axd requests to get the context set.
	/// To do this we open for all axd requests.
	/// This is how we close them all down again.
	/// </remarks>
	public class BreakAxdExtensionProcessor : HttpRequestProcessor
	{
		/// <summary>
		/// Task called in the pipeline that breaks the request if it is has an axd extension.
		/// </summary>
		/// <param name="args">Object containing alot of information about the current HttpRequest.</param>
		public override void Process(HttpRequestArgs args)
		{
			if (Path.GetExtension(args.Context.Request.FilePath).Contains("axd"))
			{
				args.AbortPipeline();
			}
		}
	}
}
