using syc_pm_client.Services.Interfaces;
using System;
using System.Net.Http;
using System.Net.Http.Json;
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

        public async Task<bool> LoginAsync(string username, string password)
        {
            var payload = new { Username = username, Password = password };
            using var resp = await _http.PostAsJsonAsync("/api/auth/login", payload);

            if (resp.IsSuccessStatusCode)
                return true;

            if (resp.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return false;

            resp.EnsureSuccessStatusCode();
            return false;
        }
    }
}
