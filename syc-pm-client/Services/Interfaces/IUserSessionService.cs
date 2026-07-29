using syc_pm_client.Models;

namespace syc_pm_client.Services.Interfaces
{
    public interface IUserSessionService
    {
        User? CurrentUser { get; }
        void Login(User user);
        void Logout();
    }
}
