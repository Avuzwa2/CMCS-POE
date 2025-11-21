using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CMCS_Web.Services;
using CMCS_Web.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Sec = System.Security.Claims; // alias so "Claim" model and security Claim don't clash
using Microsoft.AspNetCore.Authorization;

namespace CMCS_Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register() => View();

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string email, string password, string? fullName, string role = "Lecturer")
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError(string.Empty, "Email and password are required.");
                return View();
            }

            try
            {
                var user = await _userService.CreateUserAsync(email.Trim(), password, role, fullName);
                // Auto login after registration
                await SignInUserAsync(user);
                // Redirect to a sensible default by role
                return user.Role switch
                {
                    "Coordinator" => RedirectToAction("Dashboard", "Coordinator"),
                    "Admin" => RedirectToAction("Index", "Admin"),
                    _ => RedirectToAction("Dashboard", "Lecturer")
                };
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View();
            }
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError(string.Empty, "Email and password are required.");
                return View();
            }

            var user = await _userService.AuthenticateAsync(email.Trim(), password);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                return View();
            }

            await SignInUserAsync(user);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            // Redirect by role (optional)
            return user.Role switch
            {
                "Coordinator" => RedirectToAction("Dashboard", "Coordinator"),
                "Admin" => RedirectToAction("Index", "Admin"),
                _ => RedirectToAction("Dashboard", "Lecturer")
            };
        }

        private async Task SignInUserAsync(User user)
        {
            // Build authentication claims using the alias Sec.Claim
            var claims = new List<Sec.Claim>
            {
                new Sec.Claim(Sec.ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Sec.Claim(Sec.ClaimTypes.Name, user.Email ?? string.Empty),
                new Sec.Claim(Sec.ClaimTypes.Role, user.Role ?? "Lecturer")
            };

            if (!string.IsNullOrEmpty(user.FullName))
                claims.Add(new Sec.Claim("FullName", user.FullName));

            var identity = new Sec.ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new Sec.ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }

        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Create", "Claims");
        }

        // Optional: profile page (requires authorization)
        [Authorize]
        [HttpGet]
        public IActionResult Profile()
        {
            return View();
        }
    }
}
