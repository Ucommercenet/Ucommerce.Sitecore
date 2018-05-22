using Sitecore.Caching;
using UCommerce.Pipelines;
using UCommerce.Pipelines.SaveUserGroupRoles;

namespace UCommerce.Sitecore.Pipelines
{
	public class ClearAccessResultCacheForUserGroupTask : IPipelineTask<IPipelineArgs<SaveUserGroupRolesRequest, SaveUserGroupRolesResponse>>
	{
		public PipelineExecutionResult Execute(IPipelineArgs<SaveUserGroupRolesRequest, SaveUserGroupRolesResponse> subject)
		{
			CacheManager.ClearAccessResultCache();

            return PipelineExecutionResult.Success;
		}
	}
}
