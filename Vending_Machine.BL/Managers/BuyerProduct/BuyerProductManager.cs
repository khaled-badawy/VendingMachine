using Microsoft.AspNetCore.Identity;
using Serilog;
using Vending_Machine.DAL;

namespace Vending_Machine.BL
{
    public class BuyerProductManager : IBuyerProductManager
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IBuyerProductRepo _buyerProductRepo;
        private readonly IProductRepo _productRepo;

        public BuyerProductManager(UserManager<ApplicationUser> userManager, IBuyerProductRepo buyerProductRepo, IProductRepo productRepo)
        {
            _userManager = userManager;
            _buyerProductRepo = buyerProductRepo;
            _productRepo = productRepo;
        }
        public async Task<PurchasingReceiptModel> AddTransaction(ApplicationUser buyer, int productId, int amount)
        {
            var transactionModel = new PurchasingReceiptModel();
            var product = _productRepo.Get(productId);
            if (product == null)
            {
                transactionModel.Message = "No product Found !";
                return transactionModel;
            }
            if (amount > product.Amount)
            {
                transactionModel.Message = $"Only {product.Amount} products are available.";
                return transactionModel;
            }
            var totalCost = amount * product.Cost;
            if (totalCost > buyer.Deposit)
            {
                transactionModel.Message = $"Your account balance is {buyer.Deposit} and total cost is {totalCost} try to add {totalCost - buyer.Deposit} or more to your account";
                return transactionModel;
            }
            buyer.Deposit = buyer.Deposit - totalCost;
            await _userManager.UpdateAsync(buyer);

            product.Amount = product.Amount - amount;

            var productToBuy = new BuyerProduct
            {
                BuyerId = buyer.Id,
                ProductId = productId,
                Amount = amount,
                TransactionDate = DateTime.UtcNow,
            };

            try
            {
                _buyerProductRepo.AddTransaction(productToBuy);
                _buyerProductRepo.SaveChanges();
            }
            catch (Exception exception)
            {
                Log.Error(exception, $"Exception thrown Add Transaction : {exception.Message}");
                throw;
            }

            transactionModel.IsPurchased = true;
            transactionModel.Message = "Products Purchased Successfully";
            transactionModel.TransactionId = productToBuy.Id;
            transactionModel.TotlaPrice = totalCost;

            var changeCoins = CalculateChange(buyer.Deposit);
            transactionModel.ChangeCoins = changeCoins;

            var purchasedProducts = GetBuyerPurchasedProducts(buyer);
            transactionModel.PurchasedProducts = purchasedProducts;

            return transactionModel;
        }
        private List<PurchasedProductModel> GetBuyerPurchasedProducts(ApplicationUser buyer)
        {
            var purchasedProducts = _buyerProductRepo.GetBuyerProducts(buyer.Id);

            var productsList = purchasedProducts.Select(p => new PurchasedProductModel
            {
                ProductId = p.ProductId,
                ProductName = p.Product.Name,
                PurchasedQuantity = p.Amount,
                Cost = p.Product.Cost,
                TransactionDate = p.TransactionDate
            }).ToList();
            return productsList;
        }
        private Dictionary<int, int> CalculateChange(double? changeAmount)
        {
            int[] denominations = { 100, 50, 20, 10, 5 };
            Dictionary<int, int> change = new Dictionary<int, int>();

            foreach (int denomination in denominations)
            {
                change[denomination] = 0;
            }
            if (changeAmount is null)
            {
                return change;
            }

            foreach (int denomination in denominations)
            {
                if (changeAmount >= denomination)
                {
                    int count = (int)(changeAmount / denomination);
                    change[denomination] = count;
                    changeAmount = changeAmount - (count * denomination);
                }
            }

            return change;
        }
    }
}
