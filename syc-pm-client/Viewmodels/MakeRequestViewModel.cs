using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using syc_pm_client.DTOs;
using syc_pm_client.Models;
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
        private readonly IPwEntryService _pwEntryService;
        private readonly INavigationService _nav;
        private readonly IUserSessionService _userSession;

        public MakeRequestViewModel(IRequestService requestService, IPwEntryService pwEntryService, INavigationService nav, IUserSessionService userSession)
        {
            _requestService = requestService;
            _pwEntryService = pwEntryService;
            _nav = nav;
            _userSession = userSession;
        }

        public bool IsAdmin => _userSession.IsAdmin;

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

                Guid? parsedId = null;
                if (Guid.TryParse(TargetEntryId, out var id))
                {
                    parsedId = id;
                }

                bool success = false;

                if (IsAdmin)
                {
                    if (RequestType == "Add")
                    {
                        success = await _pwEntryService.AddPwEntry(new PwEntry
                        {
                            Title = Title ?? string.Empty,
                            Url = Url ?? string.Empty,
                            Username = Username ?? string.Empty,
                            EncryptedPassword = Password ?? string.Empty,
                            Description = Description ?? string.Empty
                        });
                    }
                    else if (RequestType == "Edit" && parsedId.HasValue)
                    {
                        success = await _pwEntryService.UpdatePwEntry(parsedId.Value, new PwEntry
                        {
                            Title = Title ?? string.Empty,
                            Url = Url ?? string.Empty,
                            Username = Username ?? string.Empty,
                            EncryptedPassword = Password ?? string.Empty,
                            Description = Description ?? string.Empty
                        });
                    }
                    else if (RequestType == "Remove" && parsedId.HasValue)
                    {
                        success = await _pwEntryService.DeletePwEntry(parsedId.Value);
                    }
                }
                else
                {
                    var adminPubKey = await _requestService.GetAdminPublicKey();
                    var encryptedPwd = AdminCryptoHelper.EncryptPassword(Password!, adminPubKey);

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

                    success = await _requestService.CreateRequest(request);
                }

                if (success)
                {
                    _nav.Navigate<Views.MainPage>();
                }
                else
                {
                    ErrorMessage = IsAdmin ? "Failed to apply changes." : "Failed to submit request.";
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