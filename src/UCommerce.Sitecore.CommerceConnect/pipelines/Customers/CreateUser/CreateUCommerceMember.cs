using Sitecore.Commerce.Data.Customers;
using Sitecore.Commerce.Pipelines;
using Sitecore.Commerce.Services.Customers;
using UCommerce.EntitiesV2;
using UCommerce.Infrastructure;
using UCommerce.Infrastructure.Configuration;
using UCommerce.Pipelines;
using UCommerce.Pipelines.CreateMember;
using UCommerce.Xslt;

namespace UCommerce.Sitecore.CommerceConnect.Pipelines.Customers.CreateUser
{
	public class CreateUCommerceMember : ServicePipelineProcessor<CreateUserRequest, CreateUserResult>
	{
		private readonly IUserRepository _userRepository;

		public CreateUCommerceMember(IUserRepository userRepository)
		{
			_userRepository = userRepository;
		}

		public override void Process(ServicePipelineArgs args)
		{
			CreateUserRequest request;
			CreateUserResult result;

			using (new DisposableThreadLifestyleScope())
			{
				CheckParametersAndSetupRequestAndResult(args, out request, out result);

				if (IsFromUcommerce(request))
				{
					result.CommerceUser = _userRepository.Get(request.UserName);
					return;
				}

				var createMemberPipeline = ObjectFactory.Instance.Resolve<IPipeline<IPipelineArgs<CreateMemberRequest, CreateMemberResponse>>>("CreateOrGetMember");

				var clientContext = ObjectFactory.Instance.Resolve<IClientContext>();
				var basket = clientContext.GetBasket(true);
				basket.PurchaseOrder.Customer = CreateCustomer(request);

				var createMemberRequest = new CreateMemberRequest
				{
					LoginName = request.UserName,
					Password = request.Password,
					Email = request.Email,
					MemberGroup = new MemberGroup(basket.PurchaseOrder.ProductCatalogGroup.MemberGroupId),
					MemberType = new MemberType(basket.PurchaseOrder.ProductCatalogGroup.MemberTypeId)
				};
				var createMemberResponse = new CreateMemberResponse();
				createMemberRequest.Properties["FromUCommerce"] = false;

				createMemberPipeline.Execute(new CreateMemberPipelineArgs(createMemberRequest, createMemberResponse));
				result.CommerceUser = _userRepository.Get(createMemberResponse.Member.LoginName);
			}
		}

		private Customer CreateCustomer(CreateUserRequest createUserRequest)
		{
			return new Customer
			{
				FirstName = createUserRequest.UserName,
				EmailAddress = createUserRequest.Email
			};
		}
	}
}
