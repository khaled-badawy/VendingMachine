using System.ComponentModel.DataAnnotations;

namespace Vending_Machine.BL
{
    [AttributeUsage(AttributeTargets.Property)]
    public class AllowedDepositAttribute : ValidationAttribute
    {
        private readonly int[] allowedValues = { 5, 10, 20, 50, 100 };

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value != null)
            {
                int deposit = int.Parse(value.ToString()!);
                if (!allowedValues.Contains(deposit))
                {
                    return new ValidationResult("deposit must be 5,10,20,50 or 100.");
                }
            }
            return ValidationResult.Success;
        }
    }
}
