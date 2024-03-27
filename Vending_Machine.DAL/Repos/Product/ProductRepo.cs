using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Vending_Machine.DAL
{
    public class ProductRepo : IProductRepo
    {
        private readonly VendingMachineContext _context;

        public ProductRepo(VendingMachineContext context)
        {
            _context = context;
        }
        public void Add(Product product)
        {
            try
            {
                _context.Product.Add(product);

            }
            catch (Exception exception)
            {
                Log.Error(exception, $"Exception thrown Add Product {exception.Message}");
                throw;
            }
        }
        public Product? Get(int productId)
        {
            return _context.Product
                .Include(p => p.Seller)
                .FirstOrDefault(p => p.Id == productId && p.IsActive == true);
        }

        public void Delete(int productId)
        {
            var productToRemove = Get(productId);
            productToRemove!.IsActive = false;
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

    }
}
