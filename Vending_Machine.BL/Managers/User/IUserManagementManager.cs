using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Vending_Machine.DAL;

namespace Vending_Machine.BL
{
    public interface IUserManagementManager
    {
        Task<AuthModel> CreateUserAsync(CreateUserDto createUserDto);
        Task<AuthModel> LoginAsync(LoginDto LoginDto);
        UserInfoDto GetUserInfo(ApplicationUser user);
        Task<AuthModel> RemoveUser(ApplicationUser user);
        Task<AuthModel> ChangePassword(ApplicationUser user , string oldPassword , string newPassword);
        Task<AuthModel> ChangeUserName(ApplicationUser user , string newUserName);
        Task<AuthModel> AddDeposit(ApplicationUser user , int deposit);
        Task<AuthModel> ResetDeposit(ApplicationUser user);
    }
}
