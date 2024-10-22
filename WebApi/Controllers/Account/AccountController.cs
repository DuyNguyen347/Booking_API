using Application.Dtos.Requests.Account;
using Application.Dtos.Requests.Identity;
using Application.Features.Payment.Command;
using Application.Interfaces;
using Application.Interfaces.Merchant;
using Application.Interfaces.Services.Account;
using Application.Interfaces.Services.Identity;
using Domain.Constants;
using Domain.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Controllers.Account
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IUserService _userService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMerchantRepository _merchantRepository;

        public AccountController(IAccountService accountService, ICurrentUserService currentUserService, IUserService userService, IMerchantRepository merchantRepository)
        {
            _accountService = accountService;
            _currentUserService = currentUserService;
            _userService = userService;
            _merchantRepository = merchantRepository;
        }

        /// <summary>
        /// Change password
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
        {
            var result = await _accountService.ChangePasswordAsync(request, _currentUserService.UserName);
            return (result.Succeeded) ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Forgot password
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPasswordAsync(ForgotPasswordRequest request)
        {
            var result = await _userService.ForgotPasswordAsync(request);
            return (result.Succeeded) ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Reset password
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPasswordAsync(ResetPasswordRequest request)
        {
            var result = await _userService.ResetPasswordAsync(request);    
            return (result.Succeeded) ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Confirm Email
        /// </summary>
        [HttpGet("confirm-email")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            token = token.Replace(" ", "+");
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
            {
                return BadRequest("REQUIRED_TOKEN_AND_EMAIL");
            }
            var merchant = await _merchantRepository.Entities.FirstOrDefaultAsync();
            if (merchant == null) return BadRequest("NOT_FOUND_MERCHANT");
            var result = await _userService.ConfirmEmail(token, email);
            return (result.Succeeded) ? Redirect($"{merchant.MerchantWebLink}/confirm-email-success") : BadRequest(result);
        }


        /// <summary>
        /// Confirm Email by admin
        /// </summary>
        [Authorize(Roles = RoleConstants.AdministratorRole)]
        [HttpGet("admin-confirm-email")]
        public async Task<IActionResult> ConfirmEmailAdmin(string employeeNo)
        {
            if (string.IsNullOrEmpty(employeeNo))
            {
                return BadRequest("REQUIRED_EMAIL_OR_USERNAME");
            }
            var result = await _userService.ConfirmEmailAdmin(employeeNo);
            return (result.Succeeded) ? Ok(result) : BadRequest(result);
        }
    }
}