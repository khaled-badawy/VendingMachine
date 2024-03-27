using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Vending_Machine.DAL;

namespace Vending_Machine.BL
{
    public class UserManagementManager : IUserManagementManager
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _config;
        private readonly Jwt _jwt;

        public UserManagementManager(UserManager<ApplicationUser> userManager, IConfiguration config, IOptions<Jwt> jwt)
        {
            _userManager = userManager;
            _config = config;
            _jwt = jwt.Value;
        }

        private async Task<JwtSecurityToken> CreateJwtTokenAsync(ApplicationUser user)
        {
            var claimsList = await _userManager.GetClaimsAsync(user);

            if (claimsList.IsNullOrEmpty())
            {
                claimsList = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role),
                    new Claim(ClaimTypes.GivenName , user.UserName!)
                };
                await _userManager.AddClaimsAsync(user, claimsList);
            }

            string keyString = _jwt.SecretKey;
            byte[] keyInBytes = Encoding.ASCII.GetBytes(keyString);
            var key = new SymmetricSecurityKey(keyInBytes);

            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var expiry = DateTime.Now.AddDays(_jwt.DurationInHours);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                expires: expiry,
                claims: claimsList,
                signingCredentials: signingCredentials
            );
            return jwtSecurityToken;
        }
        private async Task<IdentityResult> UpdateUserClaim(ApplicationUser user, Claim newClaim)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            Claim userClaim = userClaims.FirstOrDefault(c => c.Type == newClaim.Type)!;

            var removeClaimResult = await _userManager.RemoveClaimAsync(user, userClaim);
            if (!removeClaimResult.Succeeded) return removeClaimResult;

            var addClaimResult = await _userManager.AddClaimAsync(user, newClaim);
            return addClaimResult;
        }
        public async Task<AuthModel> CreateUserAsync(CreateUserDto createUserDto)
        {
            var authModel = new AuthModel();
            var isExisted = await _userManager.FindByNameAsync(createUserDto.UserName);
            if (isExisted is not null && !isExisted.IsDeleted)
            {
                authModel.Message = "User name is already existed.";
                return authModel;
            }
            if (isExisted is not null && isExisted.IsDeleted)
            {
                try
                {
                    await _userManager.RemovePasswordAsync(isExisted);
                    await _userManager.SetUserNameAsync(isExisted , createUserDto.UserName);
                    isExisted.Role = createUserDto.Role;
                    await _userManager.AddPasswordAsync(isExisted, createUserDto.Password);
                    isExisted.IsDeleted = false;
                    isExisted.Deposit = 0;
                    await _userManager.UpdateAsync(isExisted);

                    await UpdateUserClaim(isExisted , new Claim(ClaimTypes.Role, createUserDto.Role));
                    await UpdateUserClaim(isExisted , new Claim(ClaimTypes.GivenName, isExisted.UserName!));
                    var jwtToken = await CreateJwtTokenAsync(isExisted);

                    authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
                    authModel.TokenExpiration = jwtToken.ValidTo;
                    authModel.IsAuthenticated = true;
                    authModel.Message = "User created successfully";
                    return authModel;

                }
                catch (Exception ex)
                {
                    Log.Error(ex, $"Create a new user with deleted user name {ex.Message}");
                    throw;
                }
            }

            var newUser = new ApplicationUser
            {
                UserName = createUserDto.UserName,
                Role = createUserDto.Role,
                Deposit = 0,
                IsDeleted = false,
            };

            var createUserResult = await _userManager.CreateAsync(newUser, createUserDto.Password);
            if (!createUserResult.Succeeded)
            {
                StringBuilder errorString = new StringBuilder();
                foreach (var error in createUserResult.Errors)
                {
                    errorString.AppendLine(error.Description);
                }
                authModel.IsAuthenticated = false;
                authModel.Message = errorString.ToString();
                return authModel;
            }

            var jwtSecurityToken = await CreateJwtTokenAsync(newUser);

            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            authModel.TokenExpiration = jwtSecurityToken.ValidTo;
            authModel.IsAuthenticated = true;
            authModel.Message = "User created successfully";
            return authModel;
        }
        public async Task<AuthModel> LoginAsync(LoginDto loginDto)
        {
            var authModel = new AuthModel();
            var user = await _userManager.FindByNameAsync(loginDto.UserName);
            if (user == null || user.IsDeleted)
            {
                authModel.Message = "No user exists";
                return authModel;
            }
            var isLocked = await _userManager.IsLockedOutAsync(user);
            if (isLocked)
            {
                authModel.Message = "User is locked out try again later !";
                return authModel;
            }

            if (!await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                await _userManager.AccessFailedAsync(user);
                authModel.Message = "Wrong Password !";
                return authModel;
            }

            await _userManager.ResetAccessFailedCountAsync(user);
            var jwtSecurityToken = await CreateJwtTokenAsync(user);

            authModel.IsAuthenticated = true;
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            authModel.TokenExpiration = jwtSecurityToken.ValidTo;
            authModel.Message = "Successful login";
            return authModel;
        }
        public UserInfoDto GetUserInfo(ApplicationUser user)
        {
            return new UserInfoDto
            {
                UserName = user.UserName!,
                Role = user.Role,
                deposit = user.Deposit
            };
        }
        public async Task<AuthModel> RemoveUser(ApplicationUser user)
        {
            var authModel = new AuthModel();

            user.IsDeleted = true;
            var deletingResult =  await _userManager.UpdateAsync(user);
            if (!deletingResult.Succeeded)
            {
                StringBuilder errorString = new StringBuilder();
                foreach (var error in deletingResult.Errors)
                {
                    errorString.AppendLine(error.Description);
                }
                authModel.Message = errorString.ToString();
                return authModel;
            }
            authModel.IsAuthenticated= true;
            authModel.Message = "User Deleted successfully";
            return authModel;
        }
        public async Task<AuthModel> ChangePassword(ApplicationUser user, string oldPassword, string newPassword)
        {
            var authModel = new AuthModel();

            var isAuthenticated = await _userManager.CheckPasswordAsync(user, oldPassword);
            if (!isAuthenticated)
            {
                authModel.Message = "Wrong Password !";
                return authModel;
            }

            if (oldPassword == newPassword)
            {
                authModel.Message = "Please provide a new password rather than the old one.";
                return authModel;
            }
            var changePasswordResult = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
            if (!changePasswordResult.Succeeded)
            {
                StringBuilder errorString = new StringBuilder();
                foreach (var error in changePasswordResult.Errors)
                {
                    errorString.AppendLine(error.Description);
                }
                authModel.Message = errorString.ToString();
                return authModel;
            }

            authModel.IsAuthenticated = true;
            authModel.Message = "Password changed successfully.";
            return authModel;
        }
        public async Task<AuthModel> ChangeUserName(ApplicationUser user, string newUserName)
        {
            var authModel = new AuthModel();
            if (user.UserName == newUserName)
            {
                authModel.Message = "User name is already used. please provide a new user name rather than the old one,";
                return authModel;
            }
            var isExisted = await _userManager.FindByNameAsync(newUserName);
            if (isExisted is not null)
            {
                authModel.Message = "User name is already existed try another one";
                return authModel;
            }

            var updateUserNameResult = await _userManager.SetUserNameAsync(user, newUserName);
            if (!updateUserNameResult.Succeeded)
            {
                StringBuilder errorString = new StringBuilder();
                foreach (var error in updateUserNameResult.Errors)
                {
                    errorString.AppendLine(error.Description);
                }
                authModel.Message = errorString.ToString();
                return authModel;
            }
            await UpdateUserClaim(user, new Claim(ClaimTypes.GivenName, newUserName));
            await _userManager.UpdateAsync(user);

            var jwtSecurityToken = await CreateJwtTokenAsync(user);

            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            authModel.TokenExpiration = jwtSecurityToken.ValidTo;
            authModel.IsAuthenticated = true;
            authModel.Message = "User name updated successfully";
            return authModel;
        }
        public async Task<AuthModel> AddDeposit(ApplicationUser user, int deposit)
        {
            var authModel = new AuthModel();
            user.Deposit = user.Deposit + deposit;
            var updateDepositeResult = await _userManager.UpdateAsync(user);
            if (!updateDepositeResult.Succeeded)
            {
                StringBuilder errorString = new StringBuilder();
                foreach (var error in updateDepositeResult.Errors)
                {
                    errorString.AppendLine(error.Description);
                }
                authModel.Message = errorString.ToString();
                return authModel;
            }

            authModel.IsAuthenticated= true;
            authModel.Message = "Deposit added successfully";
            return authModel;
        }
        public async Task<AuthModel> ResetDeposit(ApplicationUser user)
        {
            var authModel = new AuthModel();
            user.Deposit = 0;
            var resetDepositResult = await _userManager.UpdateAsync(user);
            if (!resetDepositResult.Succeeded)
            {
                StringBuilder errorString = new StringBuilder();
                foreach (var error in resetDepositResult.Errors)
                {
                    errorString.AppendLine(error.Description);
                }
                authModel.Message = errorString.ToString();
                return authModel;
            }

            authModel.IsAuthenticated = true;
            authModel.Message = "Successful deposit reset";
            return authModel;
        }
    }
}
