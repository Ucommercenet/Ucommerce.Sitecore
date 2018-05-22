using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCommerce.EntitiesV2;
using UCommerce.Infrastructure;
using UCommerce.Security;

namespace UCommerce.Sitecore.Security
{
	public static class SecurityExtensions
	{
		public static IList<User> GetUsersInRoleForEntity<TRoleType, TEntity>(this ISecurityService securityService, TEntity entity)
		{
			var userService = ObjectFactory.Instance.Resolve<IUserService>();
			return userService.GetAllUsers().Where(x => securityService.UserCanAccess<TRoleType, TEntity>(entity, x)).ToList();
		}

		public static IList<User> GetUserNotInRoleForEntity<TRoleType, TEntity>(this ISecurityService securityService, TEntity entity)
		{
			var userService = ObjectFactory.Instance.Resolve<IUserService>();
			return userService.GetAllUsers().Where(x => !securityService.UserCanAccess<TRoleType, TEntity>(entity, x)).ToList();
		}

		public static string GetPermissionsForUsersFor<TRoleType, TEntity>(this ISecurityService securityService, TEntity entity)
		{
			var permissions = new StringBuilder();

			var usersInRole = securityService.GetUsersInRoleForEntity<TRoleType, TEntity>(entity);
			var usersNotInRole = securityService.GetUserNotInRoleForEntity<TRoleType, TEntity>(entity);
			foreach (var user in usersInRole)
				permissions.Append(string.Format(@"au|sitecore\{0}|pe|+item:read|+item:write|", user.ExternalId));

			foreach (var user in usersNotInRole)
				permissions.Append(string.Format(@"au|sitecore\{0}|pe|-item:read|-item:write|", user.ExternalId));
			
			return permissions.ToString();
		}

		public static string GetCreatePermissionsForUsersFor<TRoleType, TEntity>(this ISecurityService securityService, TEntity entity)
		{
			var permissions = new StringBuilder();

			var usersInRole = securityService.GetUsersForEntityInRole<TRoleType, TEntity>(entity);
			var usersNotInRole = securityService.GetUsersNotForEntityInRole<TRoleType, TEntity>(entity);

			foreach (var user in usersInRole)
				permissions.Append(string.Format(@"au|sitecore\{0}|pe|+item:rename|+item:create|", user.ExternalId));

			foreach (var user in usersNotInRole)
				permissions.Append(string.Format(@"au|sitecore\{0}|pe|-item:rename|-item:create|", user.ExternalId));


			return permissions.ToString();
		}


		public static string GetDeletePermissionsForUsersFor<TRoleType, TEntity>(this ISecurityService securityService, TEntity entity)
		{
			var permissions = new StringBuilder();

			var usersInRole = securityService.GetUsersForEntityInRole<TRoleType, TEntity>(entity);
			var usersNotInRole = securityService.GetUsersNotForEntityInRole<TRoleType, TEntity>(entity);

			foreach (var user in usersInRole)
				permissions.Append(string.Format(@"au|sitecore\{0}|pe|+item:delete|", user.ExternalId));

			foreach (var user in usersNotInRole)
				permissions.Append(string.Format(@"au|sitecore\{0}|pe|-item:delete|", user.ExternalId));


			return permissions.ToString();
		}

		public static IList<User> GetUsersForEntityInRole<TRoleType, TEntity>(this ISecurityService securityService, TEntity entity)
		{
			var userService = ObjectFactory.Instance.Resolve<IUserService>();
			return userService.GetAllUsers().Where(x => securityService.UserHasRole<TRoleType, TEntity>(entity, x)).ToList();
		}

		public static IList<User> GetUsersNotForEntityInRole<TRoleType, TEntity>(this ISecurityService securityService, TEntity entity)
		{
			var userService = ObjectFactory.Instance.Resolve<IUserService>();
			return userService.GetAllUsers().Where(x => !securityService.UserHasRole<TRoleType, TEntity>(entity, x)).ToList();
		}

		public static string GetSecurityStringForNodes(this ISecurityService securityService)
		{
			var permissions = new StringBuilder();
			var users = ObjectFactory.Instance.Resolve<IUserService>().GetAllUsers();

			foreach (var user in users) permissions.Append(string.Format(@"au|sitecore\{0}|pe|+item:read|+item:write|", user.ExternalId));

			return permissions.ToString();
		}
	}
}
