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
        private readonly IPwEntryService _pwEntryService;

        public MainViewModel(INavigationService nav, IUserSessionService userSession, IPwEntryService pwEntryService)
        {
            _nav = nav;
            _userSession = userSession;
            _pwEntryService = pwEntryService;

            // LoadDummyData();
        }

        [RelayCommand]
        private async Task Logout()
        {
            _userSession.Logout();
            _nav.Navigate<LoginPage>();
        }

        [RelayCommand]
        private async Task AddEntry()
        {
            _nav.Navigate<AddEntryPage>();
        }

        [ObservableProperty]
        private ObservableCollection<Account> accounts = new();

        [ObservableProperty]
        private Account? selectedAccount;

        /*private void LoadDummyData()
        {

            for (int i = 0; i < 10; i++)
                Accounts.Add(new Account
                {
                    Name = $"Account {i + 1}",
                    Username = $"user{i + 1}",
                    Password = "Password",
                    URL = $"https://www.example.com/user{i + 1}",
                    Notes = $"Notes for Account {i + 1}"
                });
        }*/

        public async Task<bool> LoadDataAsync()
        {
            Accounts.Clear();
            var pwEntries = await _pwEntryService.GetPwEntries();

            foreach (var entry in pwEntries)
            {
                Accounts.Add(new Account
                {
                    Name = entry.Title,
                    Username = entry.Username,
                    Password = entry.DecryptedPassword,
                    URL = entry.Url,
                    Notes = entry.Description
                });
            }

            return true;
        }
    }

    public class Account
    {
        public string Name { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string URL { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
    }
}