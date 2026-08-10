using CommunityToolkit.Mvvm.ComponentModel;
using syc_pm_client.Models;
using syc_pm_client.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace syc_pm_client.Viewmodels
{
    public partial class GiveAccessViewModel : ObservableObject
    {
        private readonly INavigationService _nav;
        private readonly HttpClient _http;
        private readonly IUserSessionService _userSession;
        private readonly IPwEntryService _pwEntryService;

        private List<PwEntry> _allEntries = [];

        public GiveAccessViewModel(INavigationService nav, HttpClient http, IUserSessionService userSession, IPwEntryService pwEntryService)
        {
            _nav = nav;
            _http = http;
            _userSession = userSession;
            _pwEntryService = pwEntryService;
            Users = [];
            AvailableEntries = [];
            GrantedEntries = [];

            _ = InitializeAsync();
        }

        private TargetUserDto? _selectedUser;
        public TargetUserDto? SelectedUser
        {
            get => _selectedUser;
            set
            {
                if (SetProperty(ref _selectedUser, value))
                {
                    _ = LoadUserAccessAsync(value);
                }
            }
        }

        public ObservableCollection<TargetUserDto> Users { get; }
        public ObservableCollection<PwEntry> AvailableEntries { get; }
        public ObservableCollection<PwEntry> GrantedEntries { get; }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        private string? _message;
        public string? Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        private async Task InitializeAsync()
        {
            try
            {
                IsBusy = true;
                Message = null;

                var currentUser = _userSession.CurrentUser;
                if (currentUser == null) return;
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", currentUser.Token);

                var usersResp = await _http.GetAsync("/api/users");
                if (usersResp.IsSuccessStatusCode)
                {
                    var usersList = await usersResp.Content.ReadFromJsonAsync<List<TargetUserDto>>();
                    if (usersList != null)
                    {
                        foreach (var u in usersList)
                        {
                            if (u.Username != currentUser.Username)
                            {
                                Users.Add(u);
                            }
                        }
                    }
                }

                _allEntries = await _pwEntryService.GetPwEntries();

            }
            catch (Exception ex)
            {
                Message = $"Failed to initialize: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task LoadUserAccessAsync(TargetUserDto? user)
        {
            AvailableEntries.Clear();
            GrantedEntries.Clear();
            Message = null;

            if (user == null) return;

            try
            {
                IsBusy = true;

                var currentUser = _userSession.CurrentUser;
                if (currentUser == null) return;
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", currentUser.Token);

                var accessResp = await _http.GetAsync($"/api/users/{user.Id}/access");
                if (accessResp.IsSuccessStatusCode)
                {
                    var grantedEntryIds = await accessResp.Content.ReadFromJsonAsync<List<Guid>>() ?? [];

                    foreach (var entry in _allEntries)
                    {
                        if (grantedEntryIds.Contains(entry.Id))
                        {
                            GrantedEntries.Add(entry);
                        }
                        else
                        {
                            AvailableEntries.Add(entry);
                        }
                    }
                }
                else
                {
                    Message = "Error fetching user access.";
                }
            }
            catch (Exception ex)
            {
                Message = $"Error: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task<bool> GrantAccessAsync(PwEntry entry)
        {
            if (SelectedUser == null) return false;

            try
            {
                IsBusy = true;
                Message = null;

                var currentUser = _userSession.CurrentUser;
                if (currentUser == null) return false;

                // simplification, assumption admin has EncryptedEntryKey
                var authUser = entry.AuthorizedUsers.FirstOrDefault(x => x.UserId == currentUser.Username || x.EncryptedEntryKey != null);
                authUser ??= entry.AuthorizedUsers.FirstOrDefault(); // fallback if Admin info not matching mapping

                if (authUser == null)
                {
                    Message = "Error: You do not have access to this entry.";
                    return false;
                }

                // decrypt local (only admin)
                using var rsaCurrent = RSA.Create();
                rsaCurrent.ImportPkcs8PrivateKey(Convert.FromBase64String(currentUser.PrivateKey), out _);
                var encryptedDEK = Convert.FromBase64String(authUser.EncryptedEntryKey);
                var dek = rsaCurrent.Decrypt(encryptedDEK, RSAEncryptionPadding.OaepSHA256);

                // encrypt for target
                using var rsaTarget = RSA.Create();
                byte[] targetPubKeyBytes;
                try
                {
                    targetPubKeyBytes = Convert.FromBase64String(SelectedUser.PublicKey);
                    rsaTarget.ImportSubjectPublicKeyInfo(targetPubKeyBytes, out _);
                }
                catch
                {
                    targetPubKeyBytes = Convert.FromBase64String(SelectedUser.PublicKey);
                    rsaTarget.ImportRSAPublicKey(targetPubKeyBytes, out _);
                }

                var newEncryptedEntryKey = rsaTarget.Encrypt(dek, RSAEncryptionPadding.OaepSHA256);
                var newEncryptedEntryKeyBase64 = Convert.ToBase64String(newEncryptedEntryKey);

                var payload = new
                {
                    targetUserId = SelectedUser.Id,
                    encryptedEntryKey = newEncryptedEntryKeyBase64
                };

                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", currentUser.Token);
                using var accessResp = await _http.PostAsJsonAsync($"/api/entries/{entry.Id}/access", payload);
                if (accessResp.IsSuccessStatusCode)
                {
                    AvailableEntries.Remove(entry);
                    GrantedEntries.Add(entry);
                    Message = $"Granted access to {entry.Title} for {SelectedUser.Username}.";
                    return true;
                }
                else
                {
                    Message = "Error: Failed to grant access.";
                    return false;
                }
            }
            catch (Exception ex)
            {
                Message = $"Error: {ex.Message}";
                return false;
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task<bool> RevokeAccessAsync(PwEntry entry)
        {
            if (SelectedUser == null) return false;

            try
            {
                IsBusy = true;
                Message = null;

                var currentUser = _userSession.CurrentUser;
                if (currentUser == null) return false;

                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", currentUser.Token);
                var revokeResp = await _http.DeleteAsync($"/api/entries/{entry.Id}/access/{SelectedUser.Id}");

                if (revokeResp.IsSuccessStatusCode)
                {
                    GrantedEntries.Remove(entry);
                    AvailableEntries.Add(entry);
                    Message = $"Revoked access to {entry.Title} for {SelectedUser.Username}.";
                    return true;
                }
                else
                {
                    Message = "Error: Failed to revoke access.";
                    return false;
                }
            }
            catch (Exception ex)
            {
                Message = $"Error: {ex.Message}";
                return false;
            }
            finally
            {
                IsBusy = false;
            }
        }
    }

    public class TargetUserDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = null!;
        public string PublicKey { get; set; } = null!;
    }
}
