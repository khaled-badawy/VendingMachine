namespace Vending_Machine.BL
{
    public class AuthModel
    {
        public string? Message { get; set; }
        public bool IsAuthenticated { get; set; }
        public string? Token { get; set; }
        public DateTime? TokenExpiration { get; set; }
    }
}
