using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace ProjectDriver_Client.Pages
{
    [RequireHttps]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class TripsModel : PageModel
    {
        private readonly ILogger<TripsModel> logger;
        public TripsModel(ILogger<TripsModel> _logger)
        {
            logger = _logger;
        }
        public IActionResult OnGet()
        {
            if (ModelState.IsValid)
            {
                if (User.Identity.IsAuthenticated)
                {
                    return null;
                }
                else
                {
                    return RedirectToPage("/Response/Unauthorized");
                }
            }
            else
            {
                return null;
            }
        }
    }
}
