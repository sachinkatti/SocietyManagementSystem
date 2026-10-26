using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Application;
using SMS.Application.DTOs;
using SMS.Infrastructure;
using System.Security.Claims;

namespace SMS.UI.Controllers
{
    public class AccountController : Controller
    {
        private readonly SMS.Application.IAuthenticationService _authenticationService;
        private readonly IWebHostEnvironment _environment;
        private readonly IAuthenticationRepository _authenticationRepository;
        private readonly IPasswordHasherService _passwordHasherService;

        public AccountController(
            SMS.Application.IAuthenticationService authenticationService,
            IWebHostEnvironment environment,
            IAuthenticationRepository authenticationRepository,
            IPasswordHasherService passwordHasherService)
        {
            _authenticationService = authenticationService;
            _environment = environment;
            _authenticationRepository = authenticationRepository;
            _passwordHasherService = passwordHasherService;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                if (User.FindFirst("MustChangePassword")?.Value == "True")
                {
                    return RedirectToAction(nameof(ChangePassword));
                }

                return RedirectToAction("Index", "Home");
            }

            return View(new LoginDto());
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if (string.IsNullOrWhiteSpace(loginDto.Email) ||
                string.IsNullOrWhiteSpace(loginDto.Password))
            {
                ViewBag.ErrorMessage = "Email and password are required.";
                return View(loginDto);
            }

            var result = await _authenticationService.LoginAsync(loginDto);

            if (!result.Success)
            {
                ViewBag.ErrorMessage = result.Message;
                return View(loginDto);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, result.UserId.ToString()),
                new Claim(ClaimTypes.Name, result.FullName),
                new Claim(ClaimTypes.Email, result.Email),
                new Claim("TenantId", result.TenantId.ToString()),
                new Claim("MustChangePassword", result.MustChangePassword.ToString())
            };

            foreach (var role in result.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            foreach (var permission in result.Permissions)
            {
                claims.Add(new Claim("Permission", permission));
            }

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);

            var properties = new AuthenticationProperties
            {
                IsPersistent = loginDto.RememberMe,
                AllowRefresh = true
            };

            if (result.MustChangePassword)
            {
                properties.AllowRefresh = false;
            }

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                properties);

            if (result.MustChangePassword)
            {
                return RedirectToAction("ChangePassword");
            }

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login");
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }


        [Authorize]
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View(new ChangePasswordDto());
        }


        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var email = User.FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrWhiteSpace(email))
                return Challenge();

            var user = await _authenticationRepository.GetUserByEmailAsync(email);

            if (user == null || !user.IsActive)
                return Challenge();

            if (!_passwordHasherService.VerifyPassword(
                    user,
                    user.PasswordHash ?? string.Empty,
                    model.CurrentPassword))
            {
                ModelState.AddModelError(
                    nameof(model.CurrentPassword),
                    "Current password is incorrect.");

                return View(model);
            }

            if (model.CurrentPassword == model.NewPassword)
            {
                ModelState.AddModelError(
                    nameof(model.NewPassword),
                    "Your new password must be different from your current password.");

                return View(model);
            }

            var newHash = _passwordHasherService.HashPassword(
                user,
                model.NewPassword);

            var updated = await _authenticationRepository.ChangePasswordAsync(
                user.UserId,
                newHash);

            if (!updated)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Unable to update your password. Please try again.");

                return View(model);
            }

            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            TempData["SuccessMessage"] =
                "Password changed successfully. Please log in again.";

            return RedirectToAction(nameof(Login));
        }
    }
}
