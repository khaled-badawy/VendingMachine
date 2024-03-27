using System.ComponentModel.DataAnnotations;

namespace Vending_Machine.BL
{
    public class AllowedRoleAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult("Role must be filled");
            }
            if (value != null)
            {
                string role = value.ToString()!;
                if (role != "buyer" && role != "seller")
                {
                    return new ValidationResult("Role must be either 'buyer' or 'seller'.");
                }
            }
            return ValidationResult.Success;
        }
    }
}
