using System.Threading.Tasks;

namespace syc_pm_client.Services.Interfaces
{
    public interface IAuthenticationService
    {
        Task<bool> LoginAsync(string username, string password);
    }
}
