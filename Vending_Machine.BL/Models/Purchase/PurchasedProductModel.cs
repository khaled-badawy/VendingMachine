namespace Vending_Machine.BL
{
    public class PurchasedProductModel
    {
        public int ProductId { get; set; }
        public double Cost { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int PurchasedQuantity { get; set; }
        public DateTime TransactionDate { get; set; }

    }
}
