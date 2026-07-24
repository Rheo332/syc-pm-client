using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using syc_pm_client.Services.Interfaces;
using syc_pm_client.Views;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace syc_pm_client.Viewmodels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly INavigationService _nav;
        private readonly IUserSessionService _userSession;

        public MainViewModel(INavigationService nav, IUserSessionService userSession)
        {
            _nav = nav;
            _userSession = userSession;

            // LoadDataAsync()
            LoadDummyData();
        }

        [RelayCommand]
        private async Task Logout()
        {
            _userSession.Logout();
            _nav.Navigate<LoginPage>();
        }

        [ObservableProperty]
        private ObservableCollection<Account> accounts = new();

        [ObservableProperty]
        private Account? selectedAccount;

        private void LoadDummyData()
        {

            for (int i = 0; i < 10; i++)
                Accounts.Add(new Account
                {
                    Name = $"Account {i + 1}",
                    Username = $"user{i + 1}",
                    Email = $"user{i + 1}@example.com",
                    Password = "Password",
                    URL = $"https://www.example.com/user{i + 1}",
                    Notes = $"Notes for Account {i + 1}"
                });
        }

        private async Task<bool> LoadDataAsync()
        {

            return true;
        }
    }

    public class Account
    {
        public string Name { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string URL { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
    }
}