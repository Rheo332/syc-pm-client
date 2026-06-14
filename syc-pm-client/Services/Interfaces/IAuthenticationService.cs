using System.Threading.Tasks;

namespace syc_pm_client.Services.Interfaces
{
    public interface IAuthenticationService
    {
        /// <summary>
        /// Attempts to log in using the provided username and password.
        /// Returns true when status code OK (200) is returned from the server.
        /// Returns false when unauthorized (401) or other non-success status.
        /// Throws on network errors.
        /// </summary>
        Task<bool> LoginAsync(string username, string password);
    }
}
