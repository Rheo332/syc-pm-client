using syc_pm_client.DTOs;
using syc_pm_client.Models;
using syc_pm_client.Services.Interfaces;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace syc_pm_client.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly HttpClient _http;

        public AuthenticationService(HttpClient http)
        {
            _http = http;
        }

        public async Task<User> LoginAsync(string username, string password)
        {
            var preLoginPpayload = new { Username = username };
            using var preLoginResp = await _http.PostAsJsonAsync("/api/auth/prelogin", preLoginPpayload);
            preLoginResp.EnsureSuccessStatusCode();

            var preLoginResponse = await preLoginResp.Content.ReadFromJsonAsync<PreLoginResponse>();
            var pbkdf2Salt = Convert.FromBase64String(preLoginResponse!.Pbkdf2Salt);
            var passwordSalt = Convert.FromBase64String(preLoginResponse!.PasswordSalt);

            var masterKey = Rfc2898DeriveBytes.Pbkdf2(password, pbkdf2Salt, 10000, HashAlgorithmName.SHA256, 32);

            var authKey = HKDF.DeriveKey(HashAlgorithmName.SHA256, masterKey, 32, Encoding.UTF8.GetBytes("auth"));
            var dataKey = HKDF.DeriveKey(HashAlgorithmName.SHA256, masterKey, 32, Encoding.UTF8.GetBytes("data"));

            using var hmac = new HMACSHA256(passwordSalt);
            var finalHash = hmac.ComputeHash(authKey);

            var loginPpayload = new { Username = username, AuthHash = finalHash };
            using var loginResp = await _http.PostAsJsonAsync("/api/auth/login", loginPpayload);
            loginResp.EnsureSuccessStatusCode();

            var loginResponse = await loginResp.Content.ReadFromJsonAsync<LoginResponse>();

            var combinedData = Convert.FromBase64String(loginResponse!.EncryptedPrivateKey);

            var nonce = new byte[12];
            Buffer.BlockCopy(combinedData, 0, nonce, 0, 12);
            var tag = new byte[16];
            Buffer.BlockCopy(combinedData, 12, tag, 0, 16);

            var ciphertextLength = combinedData.Length - 12 - 16;
            var ciphertext = new byte[ciphertextLength];
            Buffer.BlockCopy(combinedData, 28, ciphertext, 0, ciphertextLength);

            var decryptedPrivateKeyBytes = new byte[ciphertextLength];

            using (var aesGcm = new AesGcm(dataKey, tag.Length))
            {
                aesGcm.Decrypt(nonce, ciphertext, tag, decryptedPrivateKeyBytes);
            }

            var user = new User { Username = username, PrivateKey = Convert.ToBase64String(decryptedPrivateKeyBytes), Token = loginResponse!.Token };

            return user;
        }
    }
}
