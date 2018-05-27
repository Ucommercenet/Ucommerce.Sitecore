﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCommerce.EntitiesV2;
using UCommerce.Pipelines;

namespace UCommerce.Sitecore.Pipelines
{
	public class ProductSavedTask : IPipelineTask<Product>
	{
		private readonly ISitecoreContext _context;

		public ProductSavedTask(ISitecoreContext context)
		{
			_context = context;
		}


		public PipelineExecutionResult Execute(Product subject)
		{
			var provider = _context.DataProviderMaster;

			if (provider != null)
			{
				var task = new Task(() => provider.ProductSaved(subject));
				task.Start();
			}

			return PipelineExecutionResult.Success;
		}
	}
}