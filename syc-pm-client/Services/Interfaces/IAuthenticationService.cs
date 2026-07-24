using syc_pm_client.Models;
using System.Threading.Tasks;

namespace syc_pm_client.Services.Interfaces
{
    public interface IAuthenticationService
    {
        public Task<User> LoginAsync(string username, string password);
    }
}
