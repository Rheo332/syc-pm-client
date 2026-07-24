namespace syc_pm_client.DTOs
{
    public class LoginResponse
    {
        public bool Success { get; set; }
        public string PublicKey { get; set; } = null!;
        public string EncryptedPrivateKey { get; set; } = null!;
        public string Token { get; set; } = null!;
        public string Message { get; set; } = string.Empty;
    }
}
