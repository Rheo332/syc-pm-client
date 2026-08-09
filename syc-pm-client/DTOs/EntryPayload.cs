using System;

namespace syc_pm_client.DTOs
{
    public class EntryPayload
    {
        public Guid? EntryId { get; set; }
        public string Title { get; set; } = null!;
        public string Url { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string EncryptedPassword { get; set; } = null!;
        public string Description { get; set; } = null!;
    }
}