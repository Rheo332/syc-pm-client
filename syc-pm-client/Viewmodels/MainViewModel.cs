using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using syc_pm_client.Services.Interfaces;
using syc_pm_client.Views;
using System;
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
        private async Task EditEntry()
        {
            if (SelectedAccount != null)
            {
                _nav.Navigate<MakeRequestPage>(page =>
                {
                    var vm = (MakeRequestViewModel)page.DataContext;
                    vm.RequestType = "Edit";
                    vm.TargetEntryId = SelectedAccount.Id.ToString();
                    vm.Title = SelectedAccount.Name;
                    vm.Url = SelectedAccount.URL;
                    vm.Username = SelectedAccount.Username;
                    vm.Password = SelectedAccount.Password;
                    vm.Description = SelectedAccount.Notes;
                });
            }
        }

        [RelayCommand]
        private async Task DeleteEntry()
        {
            if (SelectedAccount != null)
            {
                _nav.Navigate<MakeRequestPage>(page =>
                {
                    var vm = (MakeRequestViewModel)page.DataContext;
                    vm.RequestType = "Remove";
                    vm.TargetEntryId = SelectedAccount.Id.ToString();
                });
            }
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

        [RelayCommand]
        private void OpenGiveAccess()
        {
            _nav.Navigate<GiveAccessPage>();
        }

        [RelayCommand]
        private void CopyPassword()
        {
            if (SelectedAccount != null && !string.IsNullOrEmpty(SelectedAccount.Password))
            {
                var package = new Windows.ApplicationModel.DataTransfer.DataPackage();
                package.SetText(SelectedAccount.Password);
                Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(package);
            }
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
                    Id = entry.Id,
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
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string URL { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
    }
}