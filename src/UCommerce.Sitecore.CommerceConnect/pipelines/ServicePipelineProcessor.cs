using System;
using Sitecore.Commerce.Pipelines;
using Sitecore.Commerce.Services;
using Sitecore.Diagnostics;

namespace UCommerce.Sitecore.CommerceConnect.Pipelines
{
    public abstract class ServicePipelineProcessor<TRequest, TResult> : PipelineProcessor<ServicePipelineArgs> where TRequest : ServiceProviderRequest where TResult : ServiceProviderResult
    {
        protected void CheckParametersAndSetupRequestAndResult(ServicePipelineArgs args, out TRequest request, out TResult result)
        {
            Assert.ArgumentNotNull(args, "args");
            Assert.ArgumentNotNull(args.Request, "args.Request");
            Assert.ArgumentNotNull(args.Result, "args.Result");
			Assert.ArgumentCondition(args.Request is TRequest, "args.Request", string.Format("args.Request is {0}, expected {1}", args.Request.GetType().Name, typeof(TRequest).Name));
			Assert.ArgumentCondition(args.Result is TResult, "args.Result", string.Format("args.Result is {0}, expected {1}", args.Result.GetType().Name, typeof(TResult).Name));

			request = (TRequest)args.Request;
            result = (TResult)args.Result;
        }

		protected bool IsFromUcommerce(TRequest request)
		{
			if (request == null)
			{
				throw new ArgumentNullException("request");
			}

			if (request.Properties.Contains("FromUCommerce"))
			{
				if ((bool)request.Properties["FromUCommerce"])
					return true;
			}

			return false;
		}
    }
}
