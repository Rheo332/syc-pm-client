using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using syc_pm_client.Services.Interfaces;
using syc_pm_client.Views;
using System;
using System.Threading.Tasks;

namespace syc_pm_client.Viewmodels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly INavigationService _nav;
        private readonly IAuthenticationService _auth;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
        public partial string? Username { get; set; }

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
        public partial string? Password { get; set; }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                SetProperty(ref _isBusy, value);
                LoginCommand.NotifyCanExecuteChanged();
            }
        }

        private string? _errorMessage;
        public string? ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public LoginViewModel(INavigationService nav, IAuthenticationService auth)
        {
            _nav = nav;
            _auth = auth;
        }

        private bool CanLogin()
        {
            return !IsBusy && !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password);
        }

        [RelayCommand(CanExecute = nameof(CanLogin))]
        private async Task Login()
        {
            try
            {
                IsBusy = true;
                ErrorMessage = null;

                var success = await _auth.LoginAsync(Username ?? string.Empty, Password ?? string.Empty);
                if (success)
                {
                    _nav.Navigate<MainPage>();
                }
                else
                {
                    ErrorMessage = "Invalid username or password.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Login failed: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }
    }


}
