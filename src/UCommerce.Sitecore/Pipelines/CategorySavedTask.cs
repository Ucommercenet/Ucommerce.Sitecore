using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCommerce.EntitiesV2;
using UCommerce.Pipelines;

namespace UCommerce.Sitecore.Pipelines
{
	public class CategorySavedTask : IPipelineTask<Category>
	{
		private readonly ISitecoreContext _context;

		public CategorySavedTask(ISitecoreContext context)
		{
			_context = context;
		}

		public PipelineExecutionResult Execute(Category subject)
		{
			var provider = _context.DataProviderMaster;

			if (provider != null)
			{
				var task = new Task(() => provider.CategorySaved(subject));
				task.Start();
			}

			return PipelineExecutionResult.Success;
		}
	}
}
