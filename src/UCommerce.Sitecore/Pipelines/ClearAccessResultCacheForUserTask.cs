using Sitecore.Caching;
using UCommerce.Pipelines;
using UCommerce.Pipelines.SaveUserRoles;

namespace UCommerce.Sitecore.Pipelines
{
	public class ClearAccessResultCacheForUserTask : IPipelineTask<IPipelineArgs<SaveUserRolesRequest, SaveUserRolesResponse>>
	{
		public PipelineExecutionResult Execute(IPipelineArgs<SaveUserRolesRequest, SaveUserRolesResponse> subject)
		{
			CacheManager.ClearAccessResultCache();
			return PipelineExecutionResult.Success;
		}
	}
}
