using System;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Pipelines;

namespace Ucommerce.Sitecore.Pipelines
{
	/// <summary>
	/// Allows uCommerce items to be tagged for DMS.
	/// </summary>
	public class CheckSectionAvailabilityUCommerce
	{
		/// <summary>
		/// The process.
		/// 
		/// </summary>
		/// <param name="args">The arguments.
		/// </param>
		public void Process(PipelineArgs args)
		{

			//Using dynamic to handle reflection for this type. 
			//Remarks: intelligence is not possible when using dynamic. Enable by using the below namespaces. 
			//Sitecore 8.1: using Sitecore.Pipelines.GetItemPersonalizationVisibility;
			//Sitecore 8.2: using Sitecore.Analytics.Pipelines.GetItemPersonalizationVisibility;
			dynamic changedObj = args;


			Assert.ArgumentNotNull(args, "args");
			if (changedObj.Item == null)
			{
				changedObj.Visible = false;
				changedObj.AbortPipeline();
			}
			else if (!changedObj.Visible) // Value has not been set to true previously in the pipeline.
			{
				changedObj.Visible = IsUCommerceCatalogItem(changedObj.Item);
			}
		}

		private bool IsUCommerceCatalogItem(Item item)
		{
			return item.Paths.Path.StartsWith(@"/sitecore/ucommerce/", StringComparison.OrdinalIgnoreCase);
		}
	}
}
