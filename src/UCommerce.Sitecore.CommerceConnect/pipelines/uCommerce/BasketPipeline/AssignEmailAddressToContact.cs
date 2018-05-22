using System;
using Sitecore.Analytics.Configuration;
using Sitecore.Analytics.Data;
using Sitecore.Analytics.DataAccess;
using Sitecore.Analytics.Model;
using Sitecore.Analytics.Model.Entities;
using Sitecore.Analytics.Tracking;
using Sitecore.Commerce.Automation.MarketingAutomation;
using Sitecore.Commerce.Contacts;
using Sitecore.Configuration;
using Sitecore.Diagnostics;
using UCommerce.EntitiesV2;
using UCommerce.Pipelines;
using UCommerce.Security;

namespace UCommerce.Sitecore.CommerceConnect.Pipelines.UCommerce.BasketPipeline
{
	/// <summary>
	/// Assign emails to current contact when emails is known.
	/// </summary>
	public class AssignEmailAddressToContact : IPipelineTask<PurchaseOrder>
	{
		private readonly IMemberService _memberService;

		public AssignEmailAddressToContact(IMemberService memberService)
		{
			_memberService = memberService;
		}

		public PipelineExecutionResult Execute(PurchaseOrder purchaseOrder)
		{
			var contactRepositoryBase = Assert.ResultNotNull(Factory.CreateObject("contactRepository", true) as ContactRepositoryBase);

			var contactFactory = new ContactFactory();
			string userIdentifier = contactFactory.GetContact();
			var leaseOwner = new LeaseOwner(AnalyticsSettings.ClusterName, LeaseOwnerType.WebCluster);

			var contact = CommerceAutomationHelper.GetContact(userIdentifier);
			if (contact != null)
			{
				AddEmailAddressToContact(purchaseOrder, contact);
				contactRepositoryBase.SaveContact(contact, new ContactSaveOptions(true, leaseOwner, new TimeSpan?()));
			}
			
			return PipelineExecutionResult.Success;
		}

		private void AddEmailAddressToContact(PurchaseOrder purchaseOrder, Contact contact)
		{
			if (purchaseOrder.BillingAddress != null)
			{
				AddFacets(contact, purchaseOrder.BillingAddress.EmailAddress);
				return;
			}
		}

		/// <summary>
		/// Adds email facet to the Contact 
		/// </summary>
		/// <param name="contact">The Contact to add facets to</param><param name="emailAddress">The result containing details about the email</param>
		protected virtual void AddFacets(Contact contact, string emailAddress)
		{
			var facet = contact.GetFacet<IContactEmailAddresses>("Emails");
			facet.Preferred = "main";

			if (facet.Entries.Contains("main"))
			{
				facet.Entries["main"].SmtpAddress = emailAddress;
			}
			else
			{
				facet.Entries.Create("main").SmtpAddress = emailAddress;
			}
		}
	}
}
