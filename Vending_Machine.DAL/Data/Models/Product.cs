using System.ComponentModel.DataAnnotations.Schema;

namespace Vending_Machine.DAL
{
    public class Product
    {
        public int Id { get; set; }
        public int Amount { get; set; }
        public double Cost { get; set; }
        public string Name { get; set; } = string.Empty;
        public int SellerId { get; set; }
        public bool IsActive { get; set; }

        [ForeignKey("SellerId")]
        public virtual ApplicationUser Seller { get; set; } = null!;
        public List<BuyerProduct>? BuyerProducts { get; set;} 

    }
}
