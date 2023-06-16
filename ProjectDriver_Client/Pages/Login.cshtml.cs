using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using DriveYOU_WebClient.Context;
using DriveYOU_WebClient.Models;

namespace DriveYOU_WebClient.Pages
{
    [AllowAnonymous]
    [RequireHttps]
    public class LoginModel : PageModel
    {
        [BindProperty]
        public Models.LoginModel Model { get; set; }
        [BindProperty]
        public Models.ErrorModel ErrorModel { get; set; }

        private readonly ILogger<LoginModel> logger;
        private ApplicationDbContext context;
        public LoginModel(ILogger<LoginModel> _logger, ApplicationDbContext _context)
        {
            logger = _logger;
            context = _context;
        }
        public IActionResult OnGet()
        {
            if (ModelState.IsValid)
            {
                if (User.Identity.IsAuthenticated)
                {
                    return RedirectToPage("/Account");
                }
            }
            return null;
        }

        public async Task<IActionResult> OnPost()
        {
            if (ModelState.IsValid)
            {
                User user = context.Users.FirstOrDefault(u => u.Number == Model.Number && u.Password == Model.Password);
                if (user != null)
                {
                    logger.LogInformation("User {0} is authenticated", user.Email);
                    return await Authenticate(user.Number.ToString());
                }
                else
                {
                    ErrorModel = new Models.ErrorModel("Authentication error", "Authentication error: Login or password is invalid");
                }
            }
            return null;
        }


        private async Task<IActionResult> Authenticate(string userName)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, "User")
            };
            ClaimsIdentity identity = new ClaimsIdentity(claims, "Cookies", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
            return RedirectToPage("/Index");
        }
    }
}
