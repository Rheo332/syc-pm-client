using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using syc_pm_client.Services.Interfaces;
using syc_pm_client.Views;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace syc_pm_client.Viewmodels
{
    public partial class AddUserViewModel : ObservableObject
    {
        private readonly INavigationService _nav;
        private readonly HttpClient _http;
        private readonly IUserSessionService _userSession;

        public AddUserViewModel(INavigationService nav, HttpClient http, IUserSessionService userSession)
        {
            _nav = nav;
            _http = http;
            _userSession = userSession;
        }

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SubmitCommand))]
        public partial string? Username { get; set; }

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SubmitCommand))]
        public partial string? Password { get; set; }

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

                // 1. Master Key Derivation (PBKDF2)
                byte[] pbkdf2Salt = new byte[16];
                RandomNumberGenerator.Fill(pbkdf2Salt);

                byte[] passwordBytes = Encoding.UTF8.GetBytes(Password!);
                byte[] masterKey = Rfc2898DeriveBytes.Pbkdf2(passwordBytes, pbkdf2Salt, 10000, HashAlgorithmName.SHA256, 32);

                // 2. Subkey Derivations (HKDF)
                byte[] authInfo = Encoding.UTF8.GetBytes("auth");
                byte[] authKey = HKDF.DeriveKey(HashAlgorithmName.SHA256, masterKey, 32, authInfo);

                byte[] dataInfo = Encoding.UTF8.GetBytes("data");
                byte[] dataKey = HKDF.DeriveKey(HashAlgorithmName.SHA256, masterKey, 32, dataInfo);

                // 3. Authentication Hash (HMAC)
                byte[] passwordSalt = new byte[16];
                RandomNumberGenerator.Fill(passwordSalt);

                byte[] passwordHash;
                using (var hmac = new HMACSHA256(passwordSalt))
                {
                    passwordHash = hmac.ComputeHash(authKey);
                }

                // 4. Public/Private Keypair Generation (RSA)
                using var rsa = RSA.Create(2048);
                byte[] publicKey = rsa.ExportSubjectPublicKeyInfo();
                byte[] privateKey = rsa.ExportPkcs8PrivateKey();

                // 5. Private Key Encryption (AES-GCM)
                byte[] nonce = new byte[12];
                RandomNumberGenerator.Fill(nonce);

                byte[] ciphertext = new byte[privateKey.Length];
                byte[] tag = new byte[16];

                using (var aesGcm = new AesGcm(dataKey, tag.Length))
                {
                    aesGcm.Encrypt(nonce, privateKey, ciphertext, tag);
                }

                byte[] encryptedPrivateKey = new byte[12 + 16 + ciphertext.Length];
                Buffer.BlockCopy(nonce, 0, encryptedPrivateKey, 0, 12);
                Buffer.BlockCopy(tag, 0, encryptedPrivateKey, 12, 16);
                Buffer.BlockCopy(ciphertext, 0, encryptedPrivateKey, 28, ciphertext.Length);

                // Send request
                var requestData = new
                {
                    username = Username,
                    passwordHash = Convert.ToBase64String(passwordHash),
                    passwordSalt = Convert.ToBase64String(passwordSalt),
                    pbkdf2Salt = Convert.ToBase64String(pbkdf2Salt),
                    publicKey = Convert.ToBase64String(publicKey),
                    encryptedPrivateKey = Convert.ToBase64String(encryptedPrivateKey)
                };

                var user = _userSession.CurrentUser;
                if (user != null)
                {
                    _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", user.Token);
                }

                using var response = await _http.PostAsJsonAsync("/api/users", requestData);

                if (response.IsSuccessStatusCode)
                {
                    _nav.Navigate<MainPage>();
                }
                else
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    ErrorMessage = $"Failed to add user. API returned {response.StatusCode}. {errorContent}";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to add user: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task Cancel()
        {
            _nav.Navigate<LoginPage>();
        }
    }
}
