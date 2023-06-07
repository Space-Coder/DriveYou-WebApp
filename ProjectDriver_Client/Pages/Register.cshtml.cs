using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using ProjectDriver_Client.Context;
using ProjectDriver_Client.Models;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectDriver_Client.Pages
{
    [AllowAnonymous]
    [RequireHttps]
    public class RegisterModel : PageModel
    {
        [BindProperty]
        public Models.RegisterModel Model { get; set; }

        private readonly ILogger<RegisterModel> logger;
        private ApplicationDbContext context;
        public RegisterModel(ILogger<RegisterModel> _logger, ApplicationDbContext _context)
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
                    return RedirectToPage("/Home");
                }
            }
            return null;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                User newUser = context.Users.FirstOrDefault(u => u.Number == Model.Number || u.Email == Model.Email);
                if (newUser != null) 
                {
                    ModelState.AddModelError("Registration", "User already registered");
                }else
                {
                    context.Users.Add(
                        new User()
                        {
                            Number = Model.Number,
                            Email = Model.Email,
                            Password = Model.Password,
                            Name = Model.Name,
                            Surname = Model.Surname,
                        });
                    await context.SaveChangesAsync();
                    logger.LogInformation("User {0} / {1} sucessfully registered", Model.Email, Model.Number);
                    return RedirectToPage("/Home");
                }
            }
            return null;
        }
    }
}
