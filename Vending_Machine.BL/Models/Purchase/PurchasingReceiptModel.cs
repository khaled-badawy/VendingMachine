namespace Vending_Machine.BL
{
    public class PurchasingReceiptModel
    {
        public int TransactionId { get; set; }
        public double TotlaPrice { get; set; }
        public List<PurchasedProductModel> PurchasedProducts { get; set; } = new List<PurchasedProductModel>();
        public Dictionary<int, int> ChangeCoins { get; set; } = null!;
        public string? Message { get; set; }
        public bool IsPurchased { get; set; }
    }
}
