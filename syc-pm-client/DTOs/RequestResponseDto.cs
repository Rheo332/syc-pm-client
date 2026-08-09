using System;

namespace syc_pm_client.DTOs
{
    public class RequestResponseDto : RequestDto
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}