using syc_pm_client.DTOs;
using syc_pm_client.Models;
using syc_pm_client.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace syc_pm_client.Services
{
    public class PwEntryService : IPwEntryService
    {
        private readonly HttpClient _http;
        private readonly IUserSessionService _userSession;

        public PwEntryService(HttpClient http, IUserSessionService userSession)
        {
            _http = http;
            _userSession = userSession;
        }

        public async Task<List<PwEntry>> GetPwEntries()
        {
            if (_userSession.CurrentUser == null)
            {
                return new List<PwEntry>();
            }

            var user = _userSession.CurrentUser;

            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", user.Token);
            using var getPwEntriesResp = await _http.GetAsync("/api/entries");
            getPwEntriesResp.EnsureSuccessStatusCode();

            var getPwEntriesResponse = await getPwEntriesResp.Content.ReadFromJsonAsync<PwEntryResponse>();
            var pwEntries = new List<PwEntry>();
            using var rsa = RSA.Create();
            rsa.ImportPkcs8PrivateKey(Convert.FromBase64String(user.PrivateKey), out _);

            foreach (var entry in getPwEntriesResponse!.PwEntries)
            {
                var authUser = entry.AuthorizedUsers.FirstOrDefault();
                if (authUser == null) continue;

                var encryptedDEK = Convert.FromBase64String(authUser.EncryptedEntryKey);
                var dek = rsa.Decrypt(encryptedDEK, RSAEncryptionPadding.OaepSHA256);

                var encryptedPasswordBytes = Convert.FromBase64String(entry.EncryptedPassword);

                var nonce = new byte[12];
                Buffer.BlockCopy(encryptedPasswordBytes, 0, nonce, 0, 12);

                var tag = new byte[16];
                Buffer.BlockCopy(encryptedPasswordBytes, 12, tag, 0, 16);

                var ciphertextLength = encryptedPasswordBytes.Length - 12 - 16;
                var ciphertext = new byte[ciphertextLength];
                Buffer.BlockCopy(encryptedPasswordBytes, 28, ciphertext, 0, ciphertextLength);

                var decryptedPasswordBytes = new byte[ciphertextLength];

                using (var aesGcm = new AesGcm(dek, tag.Length))
                {
                    aesGcm.Decrypt(nonce, ciphertext, tag, decryptedPasswordBytes);
                }

                entry.DecryptedPassword = Encoding.UTF8.GetString(decryptedPasswordBytes);
                pwEntries.Add(entry);
            }

            return pwEntries;
        }

        public async Task<bool> AddPwEntry(PwEntry entry)
        {
            if (_userSession.CurrentUser == null)
            {
                return false;
            }
            var user = _userSession.CurrentUser;
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", user.Token);
            using var addPwEntryResp = await _http.PostAsJsonAsync("/api/entries", entry);
            addPwEntryResp.EnsureSuccessStatusCode();
            return true;
        }
    }
}
