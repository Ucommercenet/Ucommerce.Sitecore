using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCommerce.EntitiesV2;
using UCommerce.Pipelines;

namespace UCommerce.Sitecore.Pipelines
{
	public class ProductDefinitionFieldSavedTask : IPipelineTask<ProductDefinitionField>
	{
		private readonly ISitecoreContext _context;

		public ProductDefinitionFieldSavedTask(ISitecoreContext context)
		{
			_context = context;
		}

		public PipelineExecutionResult Execute(ProductDefinitionField subject)
		{
			var provider = _context.DataProviderMaster;

			if (provider != null)
			{
				provider.ProductDefinitionSaved(subject.ProductDefinition);
			}

			return PipelineExecutionResult.Success;
		}
	}
}
