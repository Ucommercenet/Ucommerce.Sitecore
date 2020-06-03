using System;
using Sitecore.Events;
using Ucommerce.Infrastructure.Configuration;

namespace Ucommerce.Sitecore.Events
{
	public class JobEvent
	{
		public virtual void OnJobEnded(Object sender, EventArgs e)
		{
			if (e is SitecoreEventArgs)
			{
				// Call the ManagedThreadLifestyle to release all the components associated with the current thread.
				// Then the next time this thread is used, it will get a new set of components.
				ManagedThreadLifestyle.ReleaseManagedThreadComponents();
			}
		}
	}
}
