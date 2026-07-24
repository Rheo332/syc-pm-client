namespace syc_pm_client.DTOs
{
    public class PreLoginResponse
    {
        public string Pbkdf2Salt { get; set; } = "";
        public string PasswordSalt { get; set; } = "";
    }
}
