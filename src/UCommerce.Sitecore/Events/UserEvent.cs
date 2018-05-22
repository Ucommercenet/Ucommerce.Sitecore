using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCommerce.EntitiesV2;
using UCommerce.Infrastructure;
using UCommerce.Security;
using SitecoreExt = global::Sitecore;

namespace UCommerce.Sitecore.Events
{
	/// <summary>
	/// Class for handeling UserEvents in sitecore.
	/// </summary>
	/// <remarks>Resolve services inside methods as initialization isn't controlled by our DI container.</remarks>
	public class UserEvent
	{
		/// <summary>
		/// Method is called when a user is deleted.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		/// <remarks>Users must be deleted when they are deleted in sitecore. Otherwise it would be possible to get the roles of a previous user.</remarks>
		public void OnUserDeleted(object sender, EventArgs args)
		{
			var eventArgs = args as SitecoreExt.Events.SitecoreEventArgs;

			if (eventArgs == null) return;

			var userInfo = eventArgs.Parameters[0].ToString().Split('\\');

			if (userInfo.Length != 2) return;

			//Not possible to constructor inject as initizialitation is not controled by our DI container.
			var context = ObjectFactory.Instance.Resolve<ISitecoreContext>(); 
			
			var backEndDomainName = context.BackendDomainName;

			if (userInfo[0] != backEndDomainName) return;

			User user = User.FirstOrDefault(x => x.ExternalId == userInfo[1]);
		
			if (user == null) return;

			user.Delete();
		}
	}
}
