using System;
using System.Reflection;
using Sitecore.Analytics;
using Sitecore.Analytics.Model.Entities;
using UCommerce.Sitecore.UI.Resources;

namespace UCommerce.Sitecore.Sitecore75Compatability
{
    public class UpdateTrackerInformation
    {
        private readonly ISitecoreVersionResolver _sitecoreVersionResolver;

        public UpdateTrackerInformation(ISitecoreVersionResolver sitecoreVersionResolver)
        {
            _sitecoreVersionResolver = sitecoreVersionResolver;
        }

        public void Execute(string firstName, string lastName, string emailAddress, string phoneNumber, string addressName)
        {
            if (!Tracker.Enabled || addressName != "Billing")
            {
                return;
            }

            //Sitecore 9 https://briancaos.wordpress.com/2018/01/19/sitecore-9-tracker-current-session-identify-is-replaced-with-tracker-current-session-identifyas/
            //Added additional information which is the source identifier, where this contact is coming from.

            if (_sitecoreVersionResolver.IsEqualOrGreaterThan(new Version(9, 0)))
            {
                CallTrackerIdentify90(emailAddress);
            }
            else
            {
                CallTrackerIdentify82(emailAddress);
            }

            var personalInfo = Tracker.Current.Contact.GetFacet<IContactPersonalInfo>("Personal");
            personalInfo.FirstName = firstName;
            personalInfo.Surname = lastName;

            var phoneNumberInfo = Tracker.Current.Contact.GetFacet<IContactPhoneNumbers>("Phone Numbers");
            if (!phoneNumberInfo.Entries.Contains("Phone"))
                phoneNumberInfo.Entries.Create("Phone");
            phoneNumberInfo.Entries["Phone"].Number = phoneNumber;

            phoneNumberInfo.Preferred = "Phone";

            var emailInfo = Tracker.Current.Contact.GetFacet<IContactEmailAddresses>("Emails");
            if (!emailInfo.Entries.Contains("Preferred"))
            {
                emailInfo.Entries.Create("Preferred");
            }

            var email = emailInfo.Entries["Preferred"];
            email.SmtpAddress = emailAddress;
            emailInfo.Preferred = "Preferred";
        }

        private void CallTrackerIdentify82(string email)
        {
            Type trackerSessionType = Tracker.Current.Session.GetType();
            var identifyMethod = trackerSessionType.GetMethod("Identify");
            identifyMethod.Invoke(Tracker.Current.Session, new object[] { email });

        }

        private void CallTrackerIdentify90(string email)
        {
            //Tracker.Current.Session.Identify(source, knowidentifier);

            Type trackerSessionType = Tracker.Current.Session.GetType();
            var identifyMethod = trackerSessionType.GetMethod("Identify");
            identifyMethod.Invoke(Tracker.Current.Session, new object[] { "website", email});
        }
    }
}
