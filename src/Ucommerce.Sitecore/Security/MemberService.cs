using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;
using Ucommerce.EntitiesV2;
using Ucommerce.Extensions;
using Ucommerce.Security;
using sitecoreExt = Sitecore;
using User = Sitecore.Security.Accounts.User;

namespace Ucommerce.Sitecore.Security
{
	/// <summary>
	/// Integrates with SiteCore members.
	/// </summary>
	public class MemberService : IMemberService
	{
		/// <summary>
		/// Gets all configured member groups from SiteCore.
		/// </summary>
		/// <returns></returns>
		public virtual IList<MemberGroup> GetMemberGroups()
		{
			return System.Web.Security.Roles.GetAllRoles().Select(x => new MemberGroup()
			{
				MemberGroupId = x,
				Name = x
			}).ToList();
		}

		public virtual bool IsLoggedIn()
		{
			return
				sitecoreExt.Context.User != null &&
				sitecoreExt.Context.User.IsAuthenticated;
		}

		public virtual bool IsMember(string emailAddress)
		{
			return !string.IsNullOrEmpty(Membership.GetUserNameByEmail(emailAddress));
		}

		public virtual Member GetCurrentMember()
		{
			var currentMembershipUser = Membership.GetUser();

			if (currentMembershipUser == null)
				return null;

			var member = sitecoreExt.Security.Accounts.User.Current;

			return ToMember(currentMembershipUser, member);
		}

		public virtual Member GetMemberFromEmail(string email)
		{
			var member = sitecoreExt.Security.Accounts.UserManager.GetUsers().FirstOrDefault(x => x.Profile.Email == email);

			if (member == null)
				return null;

			var membershipUser = Membership.GetUser(Membership.GetUserNameByEmail(email));

			if (membershipUser == null)
				return null;

			return ToMember(membershipUser, member);
		}

		public virtual Member GetMemberFromLoginName(string loginName)
		{
			var member = sitecoreExt.Security.Accounts.UserManager.GetUsers().SingleOrDefault(x => x.LocalName == loginName);

			if (member == null)
				return null;

			var membershipUser = Membership.GetUser(member.Domain.Name + "\\" + member.LocalName);

			if (membershipUser == null)
				return null;

			return ToMember(membershipUser, member);
		}

		protected virtual Member ToMember(MembershipUser membershipUser, User member)
		{
			return new Member()
			{
				MemberId = membershipUser.ProviderUserKey != null ? membershipUser.ProviderUserKey.ToString() : "",
				LoginName = member.DisplayName,
				Email = member.Profile.Email,
				Password = member.Profile.LegacyPassword
			};
		}

		public virtual EntitiesV2.Member MakeNew(string loginName, string password, string email, EntitiesV2.MemberType memberType, MemberGroup memberGroup)
		{
			// Getting role from sitecore
			var role = sitecoreExt.Security.Accounts.RolesInRolesManager.GetAllRoles(true).FirstOrDefault(x => x.Name == memberGroup.MemberGroupId) ??
					   sitecoreExt.Security.Accounts.Role.FromName(memberGroup.MemberGroupId);

			// Determining domain from role
			var domainName = role.Domain.Name;

			// Creating new sitecore user
			var newSitecoreUser = sitecoreExt.Security.Accounts.User.Create(domainName + "\\" + loginName, password);
			newSitecoreUser.Roles.Add(role);

			// Setting the profile on the new user
			newSitecoreUser.Profile.ProfileItemId = memberType.MemberTypeId;
			newSitecoreUser.Profile.Email = email;
			newSitecoreUser.Profile.Save();
			newSitecoreUser.Profile.LegacyPassword = password;

			var membershipUser = Membership.GetUser(domainName + "\\" + loginName);

			if (membershipUser == null)
				throw new Exception("Create user failed.");

			return ToMember(membershipUser, newSitecoreUser);
		}

		public virtual IList<EntitiesV2.MemberType> GetMemberTypes()
		{
			var dbCore = sitecoreExt.Configuration.Factory.GetDatabase(SitecoreConstants.SitecoreCoreDatabaseName);
			var profileItem = dbCore.GetItem(SitecoreConstants.SitecoreProfilesPath);

			var profiles = new List<MemberType>();

			if (profileItem != null && profileItem.HasChildren)
			{
				profileItem.Children.ForEach(x => profiles.Add(new MemberType()
				{
					MemberTypeId = x.ID.ToString(),
					Name = x.Name
				}));
			}

			return profiles;
		}
	}
}
