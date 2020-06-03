using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using Sitecore;
using Sitecore.Common;
using Sitecore.SecurityModel;
using Ucommerce.Security;
using Sitecore.Security.Authentication;
using Ucommerce.Infrastructure.Components.Windsor;
using Domain = Sitecore.Security.Domains.Domain;
using SitecoreUser = Sitecore.Security.Accounts.User;
using User = Ucommerce.EntitiesV2.User;


namespace Ucommerce.Sitecore.Security
{
	/// <summary>
	/// Integrates with the configured Sitecore Membership provider listing backend users for a particular domain.
	/// </summary>
	public class SitecoreUserService : IUserService
	{
		private readonly ISitecoreContext _sitecoreContext;
		private readonly IUserGroupService _userGroupService;
		private List<User> _allUsers;
		private static readonly object _lock = new object();

		[Mandatory]
		public ICurrentUserNameService CurrentUserNameService { get; set; }

		public SitecoreUserService(ISitecoreContext sitecoreContext, IUserGroupService userGroupService)
		{
			_sitecoreContext = sitecoreContext;
			_userGroupService = userGroupService;
		}

		protected User CurrentUser { get; set; }

		/// <summary>
		/// Get the <see cref="User"/> currently logged in.
		/// </summary>
		/// <returns></returns>
		/// <remarks>
		/// Defaults to the current <see cref="WindowsIdentity"/> if not running in a web context.
		/// </remarks>
		public virtual User GetCurrentUser()
		{
			if (CurrentUser == null)
				CurrentUser = GetUser(CurrentUserNameService.CurrentUserName);

			return CurrentUser;
		}


		/// <summary>
		/// Get the current culture configured for the user
		/// </summary>
		/// <returns></returns>
        public virtual CultureInfo GetCurrentUserCulture()
		{
			if (Context.Language != null)
			{
				return Context.Language.CultureInfo;
			}
			return AuthenticationManager.GetActiveUser().Profile.Culture;
        }

		/// <summary>
		/// Gets a <see cref="User"/> by <see cref="User.UserName"/>.
		/// </summary>
		/// <param name="userName">Name of the user.</param>
		/// <returns></returns>
		/// <remarks>Adds new user if user doesn't exist</remarks>
		public virtual User GetUser(string userName)
		{
            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentException("userName cannot be null or whitespace.");

			SitecoreUser sitecoreUser = GetUserInternal(userName);

            if (sitecoreUser == null)
                throw new NullReferenceException(string.Format("'sitecoreUser' is null. Looked for: '{0}'", userName));

			var name = sitecoreUser.LocalName;
			if(string.IsNullOrWhiteSpace(name))
                throw new ArgumentException(string.Format("The 'sitecoreUser.userName' cannot be null or whitespace. LocalName: {0}, Domain: {1}, AccountType: {2}, Name: {3}", sitecoreUser.LocalName, sitecoreUser.Domain, sitecoreUser.AccountType, sitecoreUser.Name));

			// Using static method on User to avoid circular
			// dependency between IRepository<T> and this service.
			// We need user info for audit in repo.
			var user = User.SingleOrDefault(x => x.ExternalId == name);

			return MapUser(sitecoreUser);
		}

        /// <summary>
        /// Wraps the functionallity of fetching a user from sitecore.
        /// </summary>
        /// <param name="username">name of the user.</param>
        /// <returns>A sitecoreUser</returns>
        /// <remarks>Sitecore users returned from a list returns the correct user name while getcurrentuser returns a user with lower casing.</remarks>
        protected virtual SitecoreUser GetUserInternal(string username)
        {
            var user = GetSitecoreUserInternal(username.ToLower());

            if (user == null && (Context.IsBackgroundThread || SitecoreSecurityIsDisabled()))
            {
                // When the context is a background thread, or the security is disabled,
                // Sitecore uses a dummy user called "Anonymous".
                // It is not a real user, so looking for it will fail.
                //
                // The intention is that a background thread is allowed to do everything.
                // And if the security is disabled, everything is allowed.
                //
                // To support this, we use the first administrator in the list
                // to stand in for the user in these cases.
                user = GetSitecoreUserInternal("admin");
            }

            return user;
        }

        /// <summary>
        /// Efficiently fetches a named user from Sitecore.
        /// </summary>
        /// <param name="username">The name of the user to find from Sitecore.</param>
        /// <returns>The matching Sitecore user. Or null if user was not found.</returns>
        protected virtual SitecoreUser GetSitecoreUserInternal(string username)
        {
            var domain = Domain.GetDomain(_sitecoreContext.BackendDomainName);

            int count;
            var siteCoreUsers = domain.GetUsersByName(0, 1, username, out count).ToList();

            return siteCoreUsers.FirstOrDefault();
        }

        /// <summary>
        /// Gets all users from sitecore, mapped as a <see cref="User"/>.
        /// </summary>
        /// <returns></returns>
        public virtual IList<User> GetAllUsers()
		{
			if (_allUsers == null)
			{
				var sitecoreUsers = GetSitecoreUsersInternal();

				var users = User.All().ToList();

				_allUsers = new List<User>();

				foreach (var scUser in sitecoreUsers)
				{
					var user = MapUser(scUser);

					_allUsers.Add(user);
				}
			}

			return _allUsers;
		}

		/// <summary>
		/// Returns a list of sitecoreusers for a configured domain.
		/// </summary>
		/// <remarks>
		/// This is a very expensive call!
		/// If there are around 8K users, it takes about 4 seconds on a developer computer.
		/// </remarks>
		/// <returns>List of Sitecore users</returns>
		protected virtual IEnumerable<SitecoreUser> GetSitecoreUsersInternal()
		{
			if (SitecoreUsers == null)
			{
				// Sites are available in web.config where you can look up the domain name for the backend, and retrieve the security domain that way.
				var domain = Domain.GetDomain(_sitecoreContext.BackendDomainName);

				var siteCoreUsers = domain.GetUsers().Where(x => x.LocalName != null);

				SitecoreUsers = siteCoreUsers.ToList();
			}

			return SitecoreUsers;
		}

		protected List<SitecoreUser> SitecoreUsers { get; set; }

		/// <summary>
		/// Converts a SiteCore user to a <see cref="User"/>.
		/// </summary>
		/// <param name="sitecoreUser">The Sitecore user.</param>
		/// <returns>UCommerce User representing the Sitecore user.</returns>
		protected virtual User MapUser(SitecoreUser sitecoreUser)
		{
			var user = GetOrCreateUser(sitecoreUser);

			user.UserName = sitecoreUser.LocalName;
			user.FirstName = sitecoreUser.LocalName;
			user.ExternalId = sitecoreUser.LocalName;
			user.IsAdmin = sitecoreUser.IsAdministrator;

			foreach (var userGroup in _userGroupService.GetAllUserGroups())
			{
				if(sitecoreUser.Roles.Any(x => x.LocalName == userGroup.ExternalId))
					user.UserGroups.Add(userGroup);
			}

			return user;
		}

		protected virtual User GetOrCreateUser(SitecoreUser sitecoreUser)
		{
			lock (_lock)
			{
                var user = User.SingleOrDefault(u => u.ExternalId.ToUpper() == sitecoreUser.LocalName.ToUpper());
				if (user == null)
				{
					user = new User(sitecoreUser.LocalName);
					user.ExternalId = sitecoreUser.LocalName;
					user.UserGroups = _userGroupService.GetAllUserGroups().Where(x => x.ExternalId.ToUpper() == sitecoreUser.LocalName.ToUpper()).ToList();
					if (user.ExternalId != null)
						user.Save();
				}
				return user;
			}

		}

		/// <summary>
		/// "This looks like the way to check if the security disabler is active." -- DotPeek
		/// </summary>
		/// <returns>true, if the security is disabled.</returns>
		protected virtual bool SitecoreSecurityIsDisabled()
		{
			return Switcher<SecurityState, SecurityState>.CurrentValue == SecurityState.Disabled;
		}
	}
}