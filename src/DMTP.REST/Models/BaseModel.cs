using DMTP.lib.Auth;

namespace DMTP.REST.Models
{
    public class BaseModel
    {
        public ApplicationUser CurrentUser;

        public BaseModel(ApplicationUser user)
        {
            CurrentUser = user;
        }
    }
}