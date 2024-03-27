namespace Vending_Machine.BL
{
    public class AddProductDto
    {
        public int Amount { get; set; }
        public double Cost { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
