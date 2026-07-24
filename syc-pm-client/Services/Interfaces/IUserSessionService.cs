using syc_pm_client.Models;

namespace syc_pm_client.Services.Interfaces
{
    public interface IUserSessionService
    {
        public void Login(User user);
        public void Logout();
    }
}
