using DriveYOU_WebClient.Context;
using DriveYOU_WebClient.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace DriveYOU_WebClient.Pages
{
    [AllowAnonymous]
    [RequireHttps]
    public class RegisterModel : PageModel
    {
        [BindProperty]
        public Models.RegisterModel Model { get; set; }
        [BindProperty]
        public Models.ErrorModel ErrorModel { get; set; }

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

        public IActionResult OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                User newUser = context.Users.FirstOrDefault(u => u.Number == Model.Number || u.Email == Model.Email);
                if (newUser != null)
                {
                    ModelState.AddModelError("Registration", "User already registered");
                    ErrorModel = new Models.ErrorModel("Registration error", "Registration error: User already registered");
                }
                else
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
                    context.SaveChanges();
                    logger.LogInformation("User {0} / {1} sucessfully registered", Model.Email, Model.Number);
                    return RedirectToPage("/Home");
                }
            }
            else
            {
                ErrorModel = new Models.ErrorModel("Model error", "ModelState is not valid");
            }
            return null;
        }
    }
}
