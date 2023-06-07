using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProjectDriver_Client.Models;
using ProjectDriver_Client.Context;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace ProjectDriver_Client.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    [RequireHttps]
    public class LoginController : ControllerBase
    {
        private readonly ILogger<LoginController> _logger;
        private ApplicationDbContext db;
        public LoginController(ILogger<LoginController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            db = context;
        }

        // GET api/<LoginController>
        [HttpGet]
        public IActionResult Get()
        {
            if (User.Identity.Name != null && User.Identity.IsAuthenticated)
            {
                return Ok();
            }
            else
            {
                return Unauthorized();
            }
        }


        // POST api/<LoginController>/signin
        [HttpPost("SignIn")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await db.Users.FirstOrDefaultAsync(u => u.Number == model.Number && u.Password == model.Password);
                if (user != null)
                {
                    await Authenticate(model.Number.ToString());
                    return Ok();
                }
                else
                {
                    ModelState.AddModelError("", "Некоректные логин и(или) пароль");
                }
            }
            return BadRequest(ModelState);
        }


        [HttpPost("SignOut")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok();
        }

        private async Task Authenticate(string userName)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, "User")
            };
            ClaimsIdentity identity = new ClaimsIdentity(claims, "Cookies", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
            Console.WriteLine(this.User.Identity.IsAuthenticated.ToString());
        }
    }
}