using System.ComponentModel.DataAnnotations.Schema;

namespace Vending_Machine.DAL
{
    public class BuyerProduct
    {
        public int Id { get; set; }
        public int BuyerId { get; set; }
        public int ProductId { get; set; }
        public DateTime TransactionDate { get; set; }
        public int Amount { get; set; }
        [ForeignKey("BuyerId")]
        public virtual ApplicationUser Buyer { get; set; } = null!;
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; } = null!;

    }
}
