namespace syc_pm_client.DTOs
{
    public class RequestDto
    {
        public string Type { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Payload { get; set; } = null!;
    }
}