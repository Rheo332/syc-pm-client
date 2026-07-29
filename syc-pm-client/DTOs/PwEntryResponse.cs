using syc_pm_client.Models;
using System.Collections.Generic;

namespace syc_pm_client.DTOs
{
    public class PwEntryResponse
    {
        public List<PwEntry> PwEntries { get; set; } = [];
    }
}
