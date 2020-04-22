using System.Collections.Generic;
using System.Linq;
using Sitecore.Security.AccessControl;
using Sitecore.Security.Accounts;
using Sitecore.Security.Domains;
using Ucommerce.EntitiesV2;
using Ucommerce.Security;
using SitecoreUser = Sitecore.Security.Accounts.User;

namespace Ucommerce.Sitecore.Security
{
    /// <summary>
    /// Domain model layer: Class responsible for building the security field value converting Ucommerce security to Sitecore security.
    /// </summary>
    public class SecurityFieldValueBuilder : ISecurityFieldValueBuilder
    {
        private readonly ISitecoreContext _sitecoreContext;
        private readonly ISecurityService _securityService;
        private readonly IUserService _userService;

        private Dictionary<string, SitecoreUser> _sitecoreUsers;

        protected Dictionary<string, SitecoreUser> SitecoreUsers
        {
            get
            {
                if (_sitecoreUsers == null)
                {
                    _sitecoreUsers = GetSitecoreUsersInternal();
                }

                return _sitecoreUsers;
            }
        }

        public SecurityFieldValueBuilder(ISitecoreContext sitecoreContext, ISecurityService securityService, IUserService userService)
        {
            _sitecoreContext = sitecoreContext;
            _securityService = securityService;
            _userService = userService;
        }

        public virtual string BuildSecurityValue(ProductCatalogGroup shop)
        {
            AccessRuleCollection accessRulesForUsers = BuildAccessRuleCollectionForUsers(shop);

            var serializer = new AccessRuleSerializer();
            var value = serializer.Serialize(accessRulesForUsers);

            return value;
        }

        public virtual string BuildSecurityValue(ProductCatalog catalog)
        {
            AccessRuleCollection accessRulesForUsers = BuildAccessRuleCollectionForUsers(catalog);

            var serializer = new AccessRuleSerializer();
            var value = serializer.Serialize(accessRulesForUsers);

            return value;
        }

        protected virtual AccessRuleCollection BuildAccessRuleCollectionForUsers(ProductCatalogGroup group)
        {
            var result = new AccessRuleCollection();

            foreach (var user in _userService.GetAllUsers())
            {
                var sitecoreAccount = SitecoreUsers[user.ExternalId];
                bool hasAccess = _securityService.UserCanAccess<ProductCatalogGroupRole, ProductCatalogGroup>(group, user);

                var userCollection = BuildAccessRuleCollectionFull(sitecoreAccount, hasAccess);
                result.AddRange(userCollection);
            }

            return result;
        }

        protected virtual AccessRuleCollection BuildAccessRuleCollectionForUsers(ProductCatalog catalog)
        {
            var result = new AccessRuleCollection();

            foreach (var user in _userService.GetAllUsers())
            {
                var sitecoreAccount = SitecoreUsers[user.ExternalId];
                bool hasAccess = _securityService.UserCanAccess<ProductCatalogRole, ProductCatalog>(catalog, user);

                var userCollection = BuildAccessRuleCollection(sitecoreAccount, hasAccess);
                result.AddRange(userCollection);
            }

            return result;
        }

        protected virtual AccessRuleCollection BuildAccessRuleCollection(Account account, bool hasAccess)
        {
            var collection = new AccessRuleCollection();

            // Add read and write item permission, if the user or user group has access.
            collection.Add(AccessRule.Create(account, AccessRight.ItemRead, PropagationType.Any, hasAccess ? SecurityPermission.AllowAccess : SecurityPermission.DenyAccess));
            collection.Add(AccessRule.Create(account, AccessRight.ItemWrite, PropagationType.Any, hasAccess ? SecurityPermission.AllowAccess : SecurityPermission.DenyAccess));

            return collection;
        }

        protected virtual AccessRuleCollection BuildAccessRuleCollectionFull(Account account, bool hasAccess)
        {
            var collection = new AccessRuleCollection();

            // Add read and write item permission, if the user or user group has access.
            collection.Add(AccessRule.Create(account, AccessRight.ItemRead, PropagationType.Any, hasAccess ? SecurityPermission.AllowAccess : SecurityPermission.DenyAccess));
            collection.Add(AccessRule.Create(account, AccessRight.ItemWrite, PropagationType.Any, hasAccess ? SecurityPermission.AllowAccess : SecurityPermission.DenyAccess));

            // Deny all other rights besides Read and Write. These permissions will be inherited for the Catalogs and Categories.
            collection.Add(AccessRule.Create(account, AccessRight.ItemAdmin, PropagationType.Any, SecurityPermission.DenyAccess));
            collection.Add(AccessRule.Create(account, AccessRight.ItemCreate, PropagationType.Any, SecurityPermission.DenyAccess));
            collection.Add(AccessRule.Create(account, AccessRight.ItemDelete, PropagationType.Any, SecurityPermission.DenyAccess));
            collection.Add(AccessRule.Create(account, AccessRight.ItemRename, PropagationType.Any, SecurityPermission.DenyAccess));

            return collection;
        }

        protected virtual Dictionary<string, SitecoreUser> GetSitecoreUsersInternal()
        {
            // Sites are available in web.config where you can look up the domain name for the backend, and retrieve the security domain that way.
            var domain = Domain.GetDomain(_sitecoreContext.BackendDomainName);

            var siteCoreUsers = domain.GetUsers().Where(x => x.LocalName != null);

            var userList = siteCoreUsers.ToList();

            var result = new Dictionary<string, SitecoreUser>();

            foreach (var user in userList)
            {
                result[user.LocalName] = user;
            }

            return result;
        }
    }
}
