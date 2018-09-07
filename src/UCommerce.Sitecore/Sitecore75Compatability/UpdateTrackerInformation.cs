using Sitecore.Analytics;
using Sitecore.Analytics.Model.Entities;

namespace UCommerce.Sitecore.Sitecore75Compatability
{
    public class UpdateTrackerInformation
    {
        public void Execute(string firstName, string lastName, string emailAddress, string phoneNumber, string addressName)
        {
            if (!Tracker.Enabled || addressName != "Billing")
            {
                return;
            }

            dynamic currentSession = Tracker.Current.Session;
            currentSession.Identify(emailAddress);

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

    }
}