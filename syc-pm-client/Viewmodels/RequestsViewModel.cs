using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using syc_pm_client.DTOs;
using syc_pm_client.Models;
using syc_pm_client.Services.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Threading.Tasks;

namespace syc_pm_client.Viewmodels
{
    public partial class RequestsViewModel : ObservableObject
    {
        private readonly IRequestService _requestService;
        private readonly IPwEntryService _pwEntryService;
        private readonly INavigationService _nav;
        private readonly IUserSessionService _userSession;

        [ObservableProperty]
        public partial ObservableCollection<RequestResponseDto> PendingRequests { get; set; } = new();

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                SetProperty(ref _isBusy, value);
            }
        }

        private RequestResponseDto? _selectedRequest;
        public RequestResponseDto? SelectedRequest
        {
            get => _selectedRequest;
            set
            {
                SetProperty(ref _selectedRequest, value);
                if (value != null)
                {
                    _ = OpenReviewDialog(value);
                    SelectedRequest = null; // reset
                }
            }
        }

        public RequestsViewModel(IRequestService requestService, IPwEntryService pwEntryService, INavigationService nav, IUserSessionService userSession)
        {
            _requestService = requestService;
            _pwEntryService = pwEntryService;
            _nav = nav;
            _userSession = userSession;
        }

        public async Task LoadRequestsAsync()
        {
            IsBusy = true;
            try
            {
                var reqs = await _requestService.GetRequests();
                PendingRequests.Clear();
                foreach (var r in reqs)
                {
                    PendingRequests.Add(r);
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task OpenReviewDialog(RequestResponseDto request)
        {
            EntryPayload? payload = null;
            try
            {
                payload = JsonSerializer.Deserialize<EntryPayload>(request.Payload, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch { }

            string contentText = payload != null
                ? $"Type: {request.Type}\nTitle: {payload.Title}\nURL: {payload.Url}\nUser: {payload.Username}\nDesc: {payload.Description}"
                : "Invalid payload";

            var dialog = new ContentDialog
            {
                Title = "Review Request",
                Content = contentText,
                PrimaryButtonText = "Approve",
                SecondaryButtonText = "Reject",
                CloseButtonText = "Cancel",
                XamlRoot = App.Host?.Services.GetService(typeof(MainWindow)) is MainWindow win ? win.Content.XamlRoot : null
            };

            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                await ApproveRequest(request, payload);
            }
            else if (result == ContentDialogResult.Secondary)
            {
                await RejectRequest(request);
            }
        }

        private async Task ApproveRequest(RequestResponseDto request, EntryPayload? payload)
        {
            IsBusy = true;
            try
            {
                bool dbSuccess = false;

                // Usually Admin private key would decypt here, but we are keeping it simple per instructions 
                // "The admin client app will need to utilize their local private key to get the raw password back"
                // For this simplistic setup, if admin private key decrypt is needed:
                string? rawPwd = null;
                if (!string.IsNullOrEmpty(payload?.EncryptedPassword) && _userSession.CurrentUser != null && !string.IsNullOrEmpty(_userSession.CurrentUser.PrivateKey))
                {
                    try
                    {
                        var privBytes = Convert.FromBase64String(_userSession.CurrentUser.PrivateKey);
                        using var rsa = System.Security.Cryptography.RSA.Create();
                        rsa.ImportPkcs8PrivateKey(privBytes, out _);
                        var pwdBytes = Convert.FromBase64String(payload.EncryptedPassword);
                        var decBytes = rsa.Decrypt(pwdBytes, System.Security.Cryptography.RSAEncryptionPadding.OaepSHA256);
                        rawPwd = System.Text.Encoding.UTF8.GetString(decBytes);
                    }
                    catch { rawPwd = "[Decryption Failed]"; }
                }

                if (request.Type == "Add")
                {
                    dbSuccess = await _pwEntryService.AddPwEntry(new PwEntry
                    {
                        Title = payload?.Title ?? string.Empty,
                        Url = payload?.Url ?? string.Empty,
                        Username = payload?.Username ?? string.Empty,
                        EncryptedPassword = rawPwd ?? string.Empty,
                        Description = payload?.Description ?? string.Empty
                    });
                }
                else if (request.Type == "Edit" && payload!.EntryId.HasValue)
                {
                    dbSuccess = await _pwEntryService.UpdatePwEntry(payload.EntryId.Value, new PwEntry
                    {
                        Title = payload?.Title ?? string.Empty,
                        Url = payload?.Url ?? string.Empty,
                        Username = payload?.Username ?? string.Empty,
                        EncryptedPassword = rawPwd ?? string.Empty,
                        Description = payload?.Description ?? string.Empty
                    });
                }
                else if (request.Type == "Remove" && payload!.EntryId.HasValue)
                {
                    dbSuccess = await _pwEntryService.DeletePwEntry(payload.EntryId.Value);
                }

                if (dbSuccess)
                {
                    await _requestService.DeleteRequest(request.Id);
                    PendingRequests.Remove(request);
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task RejectRequest(RequestResponseDto request)
        {
            IsBusy = true;
            try
            {
                var success = await _requestService.DeleteRequest(request.Id);
                if (success)
                {
                    PendingRequests.Remove(request);
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private void Back()
        {
            _nav.Navigate<Views.MainPage>();
        }
    }
}