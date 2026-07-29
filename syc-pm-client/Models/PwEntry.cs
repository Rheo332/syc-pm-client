using System.Collections.Generic;

namespace syc_pm_client.Models
{
    public class PwEntry
    {
        public string Title { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string EncryptedPassword { get; set; } = null!;
        public string Description { get; set; } = null!;
        public List<AuthorizedUser> AuthorizedUsers { get; set; } = [];
        public string DecryptedPassword { get; set; } = null!;
    }

    public class AuthorizedUser
    {
        public string PwEntryId { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public string EncryptedEntryKey { get; set; } = null!;
    }
}
