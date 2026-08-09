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
        }

        [RelayCommand]
        private async Task AddEntry()
        {
            _nav.Navigate<AddEntryPage>();
        }

        [RelayCommand]
        private async Task AddUser()
        {
            _nav.Navigate<AddUserPage>();
        }

        [RelayCommand]
        private void OpenRequests()
        {
            _nav.Navigate<RequestsPage>();
        }

        [RelayCommand]
        private void OpenMakeRequest()
        {
            _nav.Navigate<MakeRequestPage>();
        }

        [ObservableProperty]
        public partial ObservableCollection<Account>? Accounts { get; set; } = new();

        [ObservableProperty]
        public partial Account? SelectedAccount { get; set; }

        public async Task<bool> LoadDataAsync()
        {
            Accounts?.Clear();
            var pwEntries = await _pwEntryService.GetPwEntries();

            foreach (var entry in pwEntries)
            {
                Accounts?.Add(new Account
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