using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.Extensions;
using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security.OAuth;
using Owin;
using Sitecore.Abstractions;
using Sitecore.Diagnostics;
using Sitecore.Owin.Authentication.Caching;
using Sitecore.Owin.Authentication.Configuration;
using Sitecore.Owin.Authentication.Services;
using Sitecore.Owin.Pipelines.Initialize;

namespace Ucommerce.Sitecore.Owin.Processors.Initialize
{
    public class JwtBearerAuthentication : global::Sitecore.Owin.Authentication.IdentityServer.Pipelines.Initialize.JwtBearerAuthentication
    {
        public JwtBearerAuthentication(FederatedAuthenticationConfiguration federatedAuthenticationConfiguration,
                                       BaseLog log,
                                       ApplicationUserResolver applicationUserResolver,
                                       TransformedIdentitiesCache transformedIdentitiesCache,
                                       BaseCacheManager cacheManager,
                                       BaseSettings settings)
            : base(federatedAuthenticationConfiguration, log, applicationUserResolver, transformedIdentitiesCache, cacheManager, settings) { }

        protected override void ProcessCore(InitializeArgs args)
        {
            Assert.ArgumentNotNull(args, nameof(args));
            Assert.ArgumentCondition(Issuers.Count > 0, "Issuers", "Collection Issuers should contain at least one item.");
            Assert.ArgumentCondition(Audiences.Count > 0, "Audiences", "Collection Audiences should contain at least one item.");
            for (var index = 0; index < Issuers.Count; ++index)
            {
                Issuers[index] = Issuers[index]
                    .ToLower(CultureInfo.InvariantCulture);
            }

            var app = args.App;
            var options = new JwtBearerAuthenticationOptions();
            options.AuthenticationType = AuthenticationType;
            options.Provider = new OAuthBearerAuthenticationProvider
            {
                OnValidateIdentity = ValidateIdentity
            };
            options.AllowedAudiences = Audiences;
            options.IssuerSecurityKeyProviders = GetIssuerSecurityKeyProviders(Issuers);

            var jwtFormat = options.TokenValidationParameters == null
                                ? new JwtFormat(options.AllowedAudiences, options.IssuerSecurityKeyProviders)
                                : new JwtFormat(options.TokenValidationParameters);
            if (options.TokenHandler != null)
            {
                jwtFormat.TokenHandler = options.TokenHandler;
            }

            var authenticationOptions = new OAuthBearerAuthenticationOptions();
            authenticationOptions.Realm = options.Realm;
            authenticationOptions.Provider = options.Provider;
            authenticationOptions.AccessTokenFormat = jwtFormat;
            authenticationOptions.AuthenticationMode = options.AuthenticationMode;
            authenticationOptions.AuthenticationType = options.AuthenticationType;
            authenticationOptions.Description = options.Description;
            var options1 = authenticationOptions;

            app.Use(typeof(MyOAuthBearerAuthenticationMiddleware), app, options1);
            app.UseStageMarker(PipelineStage.Authenticate);
        }

        private IEnumerable<IIssuerSecurityKeyProvider> GetIssuerSecurityKeyProviders(IEnumerable<string> issuers)
        {
            var type = typeof(global::Sitecore.Owin.Authentication.IdentityServer.Pipelines.Initialize.JwtBearerAuthentication);
            var method = type.GetMethod("GetIssuerSecurityKeyProviders", BindingFlags.NonPublic | BindingFlags.Instance);
            return method?.Invoke(this, new[] { issuers }) as IEnumerable<IIssuerSecurityKeyProvider>;
        }
    }

    public class MyOAuthBearerAuthenticationMiddleware :
        OAuthBearerAuthenticationMiddleware
    {
        public MyOAuthBearerAuthenticationMiddleware(OwinMiddleware next,
                                                     IAppBuilder app,
                                                     OAuthBearerAuthenticationOptions options)
            : base(next, app, options) { }

        public override Task Invoke(IOwinContext context)
        {
            if (context.Request.Path.Value.StartsWith("/api/v1", StringComparison.InvariantCultureIgnoreCase))
            {
                return Next.Invoke(context);
            }

            return base.Invoke(context);
        }
    }
}
