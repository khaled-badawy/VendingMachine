using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Vending_Machine.BL;
using Vending_Machine.DAL;

namespace Vending_Machine.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IBuyerProductManager _buyerProductManager;
        private readonly IProductManager _productManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProductController(IBuyerProductManager buyerProductManager,IProductManager productManager, UserManager<ApplicationUser> userManager)
        {
            _buyerProductManager = buyerProductManager;
            _productManager = productManager;
            _userManager = userManager;
        }

        #region Add Product

        [HttpPost]
        [Route("AddProduct")]
        [Authorize(Policy = "Sellers")]

        public async Task<ActionResult> Add(AddProductDto product)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
            {
                return NotFound();
            }
            var productId = _productManager.AddProduct(product, user.Id);
            return Ok(new { product_id = productId});
        }

        #endregion

        #region Get Product

        [HttpGet]
        [Route("GetProduct")]
        [Authorize]

        public ActionResult<ReadProductDto> Get(int productId)
        {
            var product = _productManager.GetProduct(productId);
            if (product is null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        #endregion

        #region Buy Product

        [HttpPost]
        [Route("Buy")]
        [Authorize(Policy = "Buyers")]

        public async Task<ActionResult<PurchasingReceiptModel>> BuyProduct(BuyProductDto buyProductDto)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
            {
                return NotFound();
            }

            var transactionModel = await _buyerProductManager.AddTransaction(user, buyProductDto.ProductId, buyProductDto.Amount);
            if (!transactionModel.IsPurchased) return BadRequest(transactionModel.Message);
            return Ok(transactionModel);
        }

        #endregion

        #region Delete Product


        [HttpDelete]
        [Route("DeleteProduct")]
        [Authorize(Policy = "Sellers")]

        public async Task<ActionResult<DeleteProductResponseModel>> DeleteProduct(int productId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
            {
                return NotFound();
            }

            var deleteModel = _productManager.DeleteProduct(productId,user.Id);
            return Ok(deleteModel);
        }

        #endregion

        #region Edit Product

        [HttpPut]
        [Route("EditProduct")]
        [Authorize(Policy = "Sellers")]

        public async Task<ActionResult<EditProductResponseModel>> EditProduct(EditProductDto productDto)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
            {
                return NotFound();
            }

            var editModel = _productManager.EditProduct(productDto , user.Id);
            return Ok(editModel);
        }

        #endregion
    }
}
