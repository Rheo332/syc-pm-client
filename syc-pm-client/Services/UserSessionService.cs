using syc_pm_client.Models;
using syc_pm_client.Services.Interfaces;

namespace syc_pm_client.Services
{
    public class UserSessionService : IUserSessionService
    {
        public User? CurrentUser { get; private set; }

        public bool IsLoggedIn => CurrentUser != null;

        public void Login(User user)
        {
            CurrentUser = user;
        }

        public void Logout()
        {
            CurrentUser = null;
        }
    }
}
