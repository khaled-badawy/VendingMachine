using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Vending_Machine.BL;
using Vending_Machine.DAL;

namespace Vending_Machine.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserManagementManager _userManagementManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(IUserManagementManager userManagementManager , UserManager<ApplicationUser> userManager)
        {
            _userManagementManager = userManagementManager;
            _userManager = userManager;
        }


        #region Register

        [HttpPost]
        [Route("CreateUser")]

        public async Task<ActionResult<AuthModel>> CreateUser(CreateUserDto createUserDto)
        {
            var authModel = await _userManagementManager.CreateUserAsync(createUserDto);
            return Ok(authModel);
        }

        #endregion

        #region Login

        [HttpPost]
        [Route("Login")]

        public async Task<ActionResult<AuthModel>> Login(LoginDto loginDto)
        {
            var authModel = await _userManagementManager.LoginAsync(loginDto);
            return Ok(authModel);
        }

        #endregion

        #region Get User Data

        [HttpGet]
        [Route("GetInfo")]
        [Authorize]

        public async Task<ActionResult<UserInfoDto>> GetUserInfo()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
            {
                return NotFound();
            }

            var userInfo = _userManagementManager.GetUserInfo(user);
            return Ok(userInfo);
        }

        #endregion

        #region Delete User

        [HttpDelete]
        [Route("Delete")]
        [Authorize]

        public async Task<ActionResult<AuthModel>> DeleteUser()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
            {
                return NotFound();
            }

            var authModel = await _userManagementManager.RemoveUser(user);
            return Ok(authModel);
        }

        #endregion

        #region Change Password

        [HttpPut]
        [Route("ChangePassword")]
        [Authorize]

        public async Task<ActionResult<AuthModel>> ChangePassword(ChangePasswordDto changePasswordDto)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
            {
                return NotFound();
            }

            var authModel = await _userManagementManager.ChangePassword(user, changePasswordDto.OldPassword, changePasswordDto.NewPassword);
            return Ok(authModel);
        }

        #endregion

        #region Change User Name

        [HttpPut]
        [Route("ChangeName")]
        [Authorize]

        public async Task<ActionResult<AuthModel>> ChangeUserName(ChangeUserNameDto changeUserNameDto)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
            {
                return NotFound();
            }

            var authModel = await _userManagementManager.ChangeUserName(user, changeUserNameDto.NewUserName);
            return Ok(authModel);

        }

        #endregion

        #region Add Deposit

        [HttpPost]
        [Route("AddDeposit")]
        [Authorize(Policy = "Buyers")]

        public async Task<ActionResult<AuthModel>> AddDeposit(AddDepositDto addDepositDto)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
            {
                return NotFound();
            }

            var authModel = await _userManagementManager.AddDeposit(user, addDepositDto.Deposit);
            return Ok(authModel);
        }

        #endregion

        #region Reset Deposit

        [HttpPut]
        [Route("ResetDeposit")]
        [Authorize(Policy = "Buyers")]

        public async Task<ActionResult<AuthModel>> ResetDeposit()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
            {
                return NotFound();
            }

            var authModel = await _userManagementManager.ResetDeposit(user);
            return Ok(authModel);
        }

        #endregion
    }
}
