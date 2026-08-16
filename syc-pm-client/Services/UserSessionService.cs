using syc_pm_client.Models;
using syc_pm_client.Services.Interfaces;
using System;

namespace syc_pm_client.Services
{
    public class UserSessionService : IUserSessionService
    {
        public event Action? OnSessionChanged;

        public User? CurrentUser { get; private set; }

        public bool IsAdmin => CurrentUser?.Username == "admin";

        public bool IsLoggedIn => CurrentUser != null;

        public void Login(User user)
        {
            CurrentUser = user;
            OnSessionChanged?.Invoke();
        }

        public void Logout()
        {
            CurrentUser = null;
            OnSessionChanged?.Invoke();
        }
    }
}
