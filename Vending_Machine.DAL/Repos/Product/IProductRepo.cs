namespace Vending_Machine.DAL
{
    public interface IProductRepo
    {
        void Add(Product product);
        Product? Get(int productId);
        void Delete(int productId);
        void SaveChanges();
    }
}
