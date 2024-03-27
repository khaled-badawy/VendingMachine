namespace Vending_Machine.BL
{
    public class ReadProductDto
    {
        public int Id { get; set; }
        public int Amount { get; set; }
        public double Cost { get; set; }
        public string Name { get; set; } = string.Empty;
        public string SellerUserName { get; set; } = string.Empty;
    }
}
