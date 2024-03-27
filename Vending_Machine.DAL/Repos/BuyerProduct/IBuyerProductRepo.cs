namespace Vending_Machine.DAL
{
    public interface IBuyerProductRepo
    {
        void AddTransaction(BuyerProduct buyerProduct);
        List<BuyerProduct> GetBuyerProducts(int buyerId);
        void SaveChanges();
    }
}
