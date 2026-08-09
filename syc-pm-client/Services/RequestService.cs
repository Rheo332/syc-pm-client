using syc_pm_client.DTOs;
using syc_pm_client.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace syc_pm_client.Services
{
    public class RequestService : IRequestService
    {
        private readonly HttpClient _http;
        private readonly IUserSessionService _userSession;

        public RequestService(HttpClient http, IUserSessionService userSession)
        {
            _http = http;
            _userSession = userSession;
        }

        private void SetAuthorizationHeader()
        {
            if (_userSession.CurrentUser != null)
            {
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _userSession.CurrentUser.Token);
            }
        }

        public async Task<bool> CreateRequest(RequestDto request)
        {
            SetAuthorizationHeader();
            var response = await _http.PostAsJsonAsync("/api/requests", request);
            return response.IsSuccessStatusCode;
        }

        public async Task<List<RequestResponseDto>> GetRequests()
        {
            SetAuthorizationHeader();
            try
            {
                var response = await _http.GetAsync("/api/requests");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<RequestResponseDto>>() ?? new List<RequestResponseDto>();
                }
            }
            catch { }
            return new List<RequestResponseDto>();
        }

        public async Task<bool> DeleteRequest(Guid id)
        {
            SetAuthorizationHeader();
            var response = await _http.DeleteAsync($"/api/requests/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<string> GetAdminPublicKey()
        {
            SetAuthorizationHeader();
            try
            {
                var response = await _http.GetAsync("/api/users/admin/publickey");
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<JsonElement>();
                    if (result.TryGetProperty("publicKey", out var pubKeyElem))
                    {
                        return pubKeyElem.GetString() ?? string.Empty;
                    }
                }
            }
            catch { }
            return string.Empty;
        }
    }
}