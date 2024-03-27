using Serilog;
using Vending_Machine.DAL;

namespace Vending_Machine.BL
{
    public class ProductManager : IProductManager
    {
        private readonly IProductRepo _productRepo;

        public ProductManager(IProductRepo productRepo)
        {
            _productRepo = productRepo;
        }
        public int AddProduct(AddProductDto product, int sellerId)
        {
            var productToAdd = new Product
            {
                Amount = product.Amount,
                Cost = product.Cost,
                Name = product.Name,
                SellerId = sellerId,
                IsActive = true,
            };

            _productRepo.Add(productToAdd);
            _productRepo.SaveChanges();
            return productToAdd.Id;
        }
        public ReadProductDto? GetProduct(int productId)
        {
            var productFromDB =  _productRepo.Get(productId);
            if (productFromDB == null) return null;
            return new ReadProductDto
            {
                Amount = productFromDB.Amount,
                Cost = productFromDB.Cost,
                Name = productFromDB.Name,
                Id = productId,
                SellerUserName = productFromDB.Seller.UserName!
            };
        }
        public DeleteProductResponseModel DeleteProduct(int productId, int sellerId)
        {
            var deleteModel = new DeleteProductResponseModel();
            var productToRemove = _productRepo.Get(productId);
            if (productToRemove is null)
            {
                deleteModel.Message = "No product found !";
                return deleteModel;
            }
            if (productToRemove.SellerId != sellerId)
            {
                deleteModel.Message = "Only product owner could removes the product and can not be removed by another seller";
                return deleteModel;
            }

            try
            {
                _productRepo.Delete(productToRemove.Id);
                _productRepo.SaveChanges();
            }
            catch (Exception exception)
            {
                Log.Error(exception, $"Exception delete product : {exception.Message}");
                throw;
            }

            deleteModel.isDeleted = true;
            deleteModel.Message = "product removed succcessfully.";
            return deleteModel;
        }
        public EditProductResponseModel EditProduct(EditProductDto productDto, int sellerId)
        {
            var editModel = new EditProductResponseModel();
            var productToEdit = _productRepo.Get(productDto.ProductId);
            if (productToEdit is null)
            {
                editModel.Message = "No product found !";
                return editModel;
            }
            if (productToEdit.SellerId != sellerId)
            {
                editModel.Message = "Only product owner could ediits the product and can not be edited by another seller";
                return editModel;
            }

            productToEdit.Name = productDto.NewName;
            productToEdit.Cost = productDto.NewCost;
            productToEdit.Amount = productDto.NewAmount;
            _productRepo.SaveChanges();

            editModel.isUpdated = true;
            editModel.Message = "Product updated successfully";
            return editModel;
        }
    }
}
