using Sitecore.Security.Authentication;
using UCommerce.Security;

namespace UCommerce.Sitecore.Security
{
    public class SitecoreUserNameService : ICurrentUserNameService
    {
        public string CurrentUserName
        {
            get
            {
                return AuthenticationManager.GetActiveUser().LocalName;                
            }
        }
    }
}