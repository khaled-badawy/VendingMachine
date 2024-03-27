using Vending_Machine.DAL;

namespace Vending_Machine.BL
{
    public interface IProductManager
    {
        int AddProduct(AddProductDto product , int sellerId);
        ReadProductDto? GetProduct(int productId);
        DeleteProductResponseModel DeleteProduct(int productId, int sellerId);
        EditProductResponseModel EditProduct(EditProductDto productDto , int sellerId);
    }
}
