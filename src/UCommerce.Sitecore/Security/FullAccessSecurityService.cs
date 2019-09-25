using System;
using System.Collections.Generic;
using UCommerce.EntitiesV2;
using UCommerce.Security;

namespace UCommerce.Sitecore.Security
{
	public class FullAccessSecurityService : ISecurityService
	{
		public IList<TEntity> Filter<TRole, TEntity>(IList<TEntity> unfilteredEntities)
		{
			return unfilteredEntities;
		}

		public bool UserCanAccess<TEntity>(TEntity entity)
		{
			return true;
		}

		public bool UserCanAccess<TRole, TEntity>(TEntity entity)
		{
			return true;
		}

        public bool UserCanAccess(Guid guid)
        {
            return true;
        }

        public bool UserIsInRole(Role role)
		{
			return true;
		}

		public User GetCurrentUser()
		{
			throw new NotImplementedException();
		}

		public bool UserCanAccess<TRole, TEntity>(TEntity entity, User user)
		{
			return true;
		}

		public bool UserHasRole<TRole, TEntity>(TEntity entity, User user)
		{
			return true;
		}
	}
}
