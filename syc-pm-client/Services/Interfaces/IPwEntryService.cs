using syc_pm_client.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace syc_pm_client.Services.Interfaces
{
    public interface IPwEntryService
    {
        public Task<List<PwEntry>> GetPwEntries();
        public Task<bool> AddPwEntry(PwEntry entry);
        public Task<bool> UpdatePwEntry(Guid id, PwEntry entry);
        public Task<bool> DeletePwEntry(Guid id);
    }
}
