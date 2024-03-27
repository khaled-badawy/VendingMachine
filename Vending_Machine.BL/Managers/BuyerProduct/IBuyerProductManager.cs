using Vending_Machine.DAL;

namespace Vending_Machine.BL
{
    public interface IBuyerProductManager
    {
        Task<PurchasingReceiptModel> AddTransaction(ApplicationUser buyer, int productId, int amount);
    }
}
