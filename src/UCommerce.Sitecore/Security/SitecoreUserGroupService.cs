using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Mapping;
using Sitecore.Security.Domains;
using UCommerce.EntitiesV2;
using UCommerce.Infrastructure;
using UCommerce.Security;
using Role = Sitecore.Security.Accounts.Role;

namespace UCommerce.Sitecore.Security
{
	public class SitecoreUserGroupService : IUserGroupService
	{
		private readonly ISitecoreContext _sitecoreContext;
		private static readonly object _lock = new object();

		protected IList<UserGroup> _userGroups; 

		//We cannot inject IRepository in the constructor right now as it gives circular dependency on SessionProvider.
		//See https://trello.com/c/ZMP3hY8A/446-move-currentuser-and-currentusername-from-iuserservice-to-iauthenticationservice for more information
		//To fix this we need to introduce a breaking change on the SessionProvider which we're not interested in before a major version.
		public SitecoreUserGroupService(ISitecoreContext sitecoreContext)
		{
			_sitecoreContext = sitecoreContext;
		}

		/// <summary>
		/// Gets all user groups.
		/// </summary>
		/// <returns></returns>
		public virtual IList<UserGroup> GetAllUserGroups()
		{
			if (_userGroups != null)
				return _userGroups;

			Domain domain = Domain.GetDomain(_sitecoreContext.BackendDomainName);

			IList<Role> sitecoreDomainRoles = domain.GetRoles().ToList();

			IList<UserGroup> existingUserGroups = UserGroup.All().ToList();

			//If number of roles matches in both ends we assume everything is up to date.
			if (sitecoreDomainRoles.Count == existingUserGroups.Count) 
			{
				_userGroups = sitecoreDomainRoles.Select(x => MapExternalUserGroupToInternalUserGroup(x.LocalName)).ToList();
				return _userGroups;
			}

			lock (_lock)
			{
				var roleNamesNotCreatedAsUserGroups = sitecoreDomainRoles.Where(x => existingUserGroups.All(y => y.ExternalId != x.LocalName)).Select(x => x.LocalName).ToList();

				var newGroups = MapExternalUserGroupsToInternalUserGroups(roleNamesNotCreatedAsUserGroups);

				ObjectFactory.Instance.Resolve<IRepository<UserGroup>>().Save(newGroups);

				var allGroups = existingUserGroups.Concat(newGroups).ToList();

				_userGroups = allGroups;

				return _userGroups;	
			}
		}

		/// <summary>
		/// Maps a list of <see cref="UserGroup"/> given a list of names.
		/// </summary>
		/// <param name="userGroupNamesNotCreatedAsUserGroups">The name of the external user group name.</param>
		/// <returns></returns>
		private IList<UserGroup> MapExternalUserGroupsToInternalUserGroups(IEnumerable<string> userGroupNamesNotCreatedAsUserGroups)
		{
			IList<UserGroup> newGroups = new List<UserGroup>();

			foreach (var roleNameNotCreatedAsUserGroup in userGroupNamesNotCreatedAsUserGroups)
			{
				newGroups.Add(MapExternalUserGroupToInternalUserGroup(roleNameNotCreatedAsUserGroup));	
			}

			return newGroups;
		}

		/// <summary>
		/// Maps a <see cref="UserGroup"/> given a name.
		/// </summary>
		/// <param name="userGroupName">The name of the external user group name.</param>
		/// <returns></returns>
		protected virtual UserGroup MapExternalUserGroupToInternalUserGroup(string userGroupName)
		{
			var userGroup = new UserGroup()
			{
				ExternalId = userGroupName,
				Name = userGroupName
			};

			return userGroup;
		}

		/// <summary>
		/// Gets a <see cref="UserGroup"/> by <see cref="UserGroup.Name"/>.
		/// </summary>
		/// <param name="externalId">Name of the user group.</param>
		/// <returns></returns>
		public virtual UserGroup GetUserGroup(string externalId)
		{
			UserGroup userGroup = UserGroup.SingleOrDefault(x => x.ExternalId.ToUpper() == externalId.ToUpper());

		    if (userGroup != null)
		    {
		        userGroup.Name = userGroup.ExternalId;
		        return userGroup;
		    }

            Domain sitecoreDomain = Domain.GetDomain(_sitecoreContext.BackendDomainName);
		
			Role sitecoreUserGroup = sitecoreDomain.GetRoles().FirstOrDefault(x => x.DisplayName == externalId);

			if (sitecoreUserGroup == null)
				throw new NullReferenceException(string.Format("'sitecore role' is null. For: '{0}'", externalId));

			var newUserGroup = MapExternalUserGroupToInternalUserGroup(externalId);
			newUserGroup.Save();
			
			return newUserGroup;
		}
	}
}
