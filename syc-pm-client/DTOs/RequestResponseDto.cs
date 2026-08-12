using System;
using System.Text.Json;

namespace syc_pm_client.DTOs
{
    public class RequestResponseDto : RequestDto
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }

        public string PayloadTitle
        {
            get
            {
                if (string.IsNullOrEmpty(Payload)) return string.Empty;
                try
                {
                    var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var entry = JsonSerializer.Deserialize<EntryPayload>(Payload, opts);
                    return entry?.Title ?? string.Empty;
                }
                catch
                {
                    return string.Empty;
                }
            }
        }
    }
}