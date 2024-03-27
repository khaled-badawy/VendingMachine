using Microsoft.AspNetCore.Identity;

namespace Vending_Machine.DAL
{
    public class ApplicationUser : IdentityUser<int>
    {
        public string Role { get; set; } = string.Empty;
        public double Deposit { get; set; }
        public bool IsDeleted { get; set; }
        public List<Product>? products { get; set; }
        public List<BuyerProduct>? BuyerProducts { get; set; }
    }
}
