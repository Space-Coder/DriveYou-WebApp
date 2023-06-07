using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProjectDriver_Client.Context;
using ProjectDriver_Client.Models;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

namespace ProjectDriver_Client.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    [RequireHttps]
    public class RegisterController : ControllerBase
    {
        private readonly ILogger<RegisterController> _logger;
        private ApplicationDbContext db;
        public RegisterController(ILogger<RegisterController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            db = context;
        }

        // POST api/<RegisterController>
        [HttpPost]
        public async Task<IActionResult> Login(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await db.Users.FirstOrDefaultAsync(u => u.Number == model.Number || u.Email == model.Email);
                if (user != null)
                {
                    ModelState.AddModelError("", "Аккаунт с таким номером или email-ом уже существует");
                    return BadRequest(ModelState);
                }
                else
                {
                    db.Add(new User()
                    {
                        Number = model.Number,
                        Email = model.Email,
                        Password = model.Password,
                        Name = model.Name,
                        Surname = model.Surname
                    });
                    await db.SaveChangesAsync();
                    return Ok();
                }
            }
            return BadRequest(ModelState);
        }
    }
}
