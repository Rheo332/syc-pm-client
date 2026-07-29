using syc_pm_client.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace syc_pm_client.Services.Interfaces
{
    public interface IPwEntryService
    {
        public Task<List<PwEntry>> GetPwEntries();
    }
}
