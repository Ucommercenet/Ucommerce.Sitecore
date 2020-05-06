using Sitecore.Security.Authentication;
using Ucommerce.Security;

namespace Ucommerce.Sitecore.Security
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