using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCommerce.EntitiesV2.Definitions;
using UCommerce.Pipelines;

namespace UCommerce.Sitecore.Pipelines
{
	public class DefinitionFieldSavedTask : IPipelineTask<IDefinitionField>
	{
		private readonly ISitecoreContext _context;

		public DefinitionFieldSavedTask(ISitecoreContext context)
		{
			_context = context;
		}

		public PipelineExecutionResult Execute(IDefinitionField subject)
		{
			var provider = _context.DataProviderMaster;

			if (provider != null)
			{
				provider.DefinitionFieldSaved(subject);
			}

			return PipelineExecutionResult.Success;
		}
	}
}
