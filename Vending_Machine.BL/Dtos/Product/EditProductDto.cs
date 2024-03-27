namespace Vending_Machine.BL
{
    public class EditProductDto
    {
        public int ProductId { get; set; }
        public int NewAmount { get; set; }
        public double NewCost { get; set; }
        public string NewName { get; set; } = string.Empty;
    }
}
