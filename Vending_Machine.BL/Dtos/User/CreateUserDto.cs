using System.ComponentModel.DataAnnotations;

namespace Vending_Machine.BL
{
    public class CreateUserDto
    {
        [Required]
        public string UserName { get; set; } = string.Empty;
        [AllowedRole]
        public string Role { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

    }
}
