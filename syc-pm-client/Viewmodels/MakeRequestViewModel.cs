using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using syc_pm_client.DTOs;
using syc_pm_client.Services;
using syc_pm_client.Services.Interfaces;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace syc_pm_client.Viewmodels
{
    public partial class MakeRequestViewModel : ObservableObject
    {
        private readonly IRequestService _requestService;
        private readonly INavigationService _nav;
        private readonly IUserSessionService _userSession;

        public MakeRequestViewModel(IRequestService requestService, INavigationService nav, IUserSessionService userSession)
        {
            _requestService = requestService;
            _nav = nav;
            _userSession = userSession;
        }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsTargetIdVisible))]
        [NotifyPropertyChangedFor(nameof(IsFormVisible))]
        public partial string? RequestType { get; set; } = "Add";

        public bool IsTargetIdVisible => RequestType != "Add";

        public bool IsFormVisible => RequestType != "Remove";

        [ObservableProperty]
        public partial string? TargetEntryId { get; set; }

        [ObservableProperty]
        public partial string? Title { get; set; }

        [ObservableProperty]
        public partial string? Url { get; set; }

        [ObservableProperty]
        public partial string? Username { get; set; }

        [ObservableProperty]
        public partial string? Password { get; set; }

        [RelayCommand]
        private void GeneratePassword()
        {
            Password = PasswordGenerator.GeneratePassword(16);
        }

        [ObservableProperty]
        public partial string? Description { get; set; }

        [ObservableProperty]
        public partial string? ErrorMessage { get; set; }

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

        public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

        [RelayCommand]
        private async Task Submit()
        {
            try
            {
                IsBusy = true;
                ErrorMessage = string.Empty;
                OnPropertyChanged(nameof(HasError));

                var adminPubKey = await _requestService.GetAdminPublicKey();
                var encryptedPwd = AdminCryptoHelper.EncryptPassword(Password!, adminPubKey);

                Guid? parsedId = null;
                if (Guid.TryParse(TargetEntryId, out var id))
                {
                    parsedId = id;
                }

                var payloadObj = new EntryPayload
                {
                    EntryId = parsedId,
                    Title = Title ?? string.Empty,
                    Url = Url ?? string.Empty,
                    Username = Username ?? string.Empty,
                    EncryptedPassword = encryptedPwd ?? string.Empty,
                    Description = Description ?? string.Empty
                };

                var request = new RequestDto
                {
                    Type = RequestType ?? string.Empty,
                    Username = _userSession.CurrentUser?.Username ?? "Unknown",
                    Payload = JsonSerializer.Serialize(payloadObj)
                };

                var success = await _requestService.CreateRequest(request);
                if (success)
                {
                    _nav.Navigate<Views.MainPage>();
                }
                else
                {
                    ErrorMessage = "Failed to submit request.";
                    OnPropertyChanged(nameof(HasError));
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private void Cancel()
        {
            _nav.Navigate<Views.MainPage>();
        }
    }
}