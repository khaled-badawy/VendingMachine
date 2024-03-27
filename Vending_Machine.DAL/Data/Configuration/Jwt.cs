namespace Vending_Machine.DAL
{
    public class Jwt
    {
        public string SecretKey { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int DurationInHours { get; set; }
    }
}
