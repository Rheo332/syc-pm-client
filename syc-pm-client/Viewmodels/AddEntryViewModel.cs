using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using syc_pm_client.Models;
using syc_pm_client.Services.Interfaces;
using syc_pm_client.Views;
using System;
using System.Threading.Tasks;

namespace syc_pm_client.Viewmodels
{
    public partial class AddEntryViewModel : ObservableObject
    {
        private readonly INavigationService _nav;
        private readonly IUserSessionService _userSession;
        private readonly IPwEntryService _pwEntryService;

        public AddEntryViewModel(INavigationService nav, IUserSessionService userSession, IPwEntryService pwEntryService)
        {
            _nav = nav;
            _userSession = userSession;
            _pwEntryService = pwEntryService;
        }

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SubmitCommand))]
        public partial string? Title { get; set; }

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SubmitCommand))]
        public partial string? Url { get; set; }

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SubmitCommand))]
        public partial string? Username { get; set; }

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SubmitCommand))]
        public partial string? Password { get; set; }

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SubmitCommand))]
        public partial string? Description { get; set; }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                SetProperty(ref _isBusy, value);
                SubmitCommand.NotifyCanExecuteChanged();
            }
        }

        private string? _errorMessage;
        public string? ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }
        private bool CanSubmit()
        {
            return !IsBusy
                && !string.IsNullOrWhiteSpace(Title)
                && !string.IsNullOrWhiteSpace(Url)
                && !string.IsNullOrWhiteSpace(Username)
                && !string.IsNullOrWhiteSpace(Password);
        }

        [RelayCommand(CanExecute = nameof(CanSubmit))]
        private async Task Submit()
        {
            try
            {
                IsBusy = true;
                ErrorMessage = null;

                var success = await _pwEntryService.AddPwEntry(new PwEntry
                {
                    Title = Title ?? string.Empty,
                    Url = Url ?? string.Empty,
                    Username = Username ?? string.Empty,
                    EncryptedPassword = Password ?? string.Empty,
                    Description = Description ?? string.Empty
                });

                if (success)
                {
                    _nav.Navigate<MainPage>();
                }
                else
                {
                    ErrorMessage = "Failed to add password entry.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to add password entry: {ex.Message}";
            }
        }
    }
}
