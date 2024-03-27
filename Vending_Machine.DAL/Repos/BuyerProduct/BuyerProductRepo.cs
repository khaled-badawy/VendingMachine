
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Vending_Machine.DAL
{
    public class BuyerProductRepo : IBuyerProductRepo
    {
        private readonly VendingMachineContext _context;

        public BuyerProductRepo(VendingMachineContext context)
        {
            _context = context;
        }
        public void AddTransaction(BuyerProduct buyerProduct)
        {
            _context.BuyerProduct.Add(buyerProduct);
        }

        public List<BuyerProduct> GetBuyerProducts(int buyerId)
        {
            var buyerProductsFromDb = _context.BuyerProduct
                .Include(t => t.Product)
                .Where(t => t.BuyerId == buyerId)
                .ToList();
            if (buyerProductsFromDb.IsNullOrEmpty()) return new List<BuyerProduct>();
            return buyerProductsFromDb;
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}
