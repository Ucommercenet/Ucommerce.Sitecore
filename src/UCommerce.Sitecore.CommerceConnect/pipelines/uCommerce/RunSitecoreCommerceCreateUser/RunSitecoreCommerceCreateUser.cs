using Sitecore;
using Sitecore.Commerce.Services.Customers;
using UCommerce.Pipelines;
using UCommerce.Pipelines.CreateMember;

namespace UCommerce.Sitecore.CommerceConnect.Pipelines.UCommerce.RunSitecoreCommerceCreateUser
{
	public class RunSitecoreCommerceCreateUser : IPipelineTask<IPipelineArgs<CreateMemberRequest, CreateMemberResponse>>
	{
		public PipelineExecutionResult Execute(IPipelineArgs<CreateMemberRequest, CreateMemberResponse> subject)
		{
			if (subject.Request.Properties.ContainsKey("FromUCommerce"))
				if (!(bool)subject.Request.Properties["FromUCommerce"]) return PipelineExecutionResult.Success;

			var customerService = new CustomerServiceProvider();

			var createUserRequest = new CreateUserRequest(
												subject.Response.Member.LoginName, 
												subject.Response.Member.Password, 
												subject.Response.Member.Email,
												Context.GetSiteName()); 
			createUserRequest.Properties["FromUCommerce"] = true;
			
			customerService.CreateUser(createUserRequest);

			return PipelineExecutionResult.Success;
		}
	}
}
