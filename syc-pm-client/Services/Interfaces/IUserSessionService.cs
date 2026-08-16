using System;
using syc_pm_client.Models;

namespace syc_pm_client.Services.Interfaces
{
    public interface IUserSessionService
    {
        event Action OnSessionChanged;
        User? CurrentUser { get; }
        bool IsAdmin { get; }
        void Login(User user);
        void Logout();
    }
}
