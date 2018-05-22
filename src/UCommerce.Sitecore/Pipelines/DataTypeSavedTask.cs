using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCommerce.EntitiesV2;
using UCommerce.Pipelines;

namespace UCommerce.Sitecore.Pipelines
{
	public class DataTypeSavedTask : IPipelineTask<DataType>
	{
		private readonly ISitecoreContext _context;

		public DataTypeSavedTask(ISitecoreContext context)
		{
			_context = context;
		}

		public PipelineExecutionResult Execute(DataType subject)
		{
			var provider = _context.DataProviderMaster;

			if (provider != null)
			{
				provider.DataTypeSaved(subject);
			}

			return PipelineExecutionResult.Success;
		}
	}
}
