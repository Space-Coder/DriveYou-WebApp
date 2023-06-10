using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using ProjectDriver_Client.Context;

namespace ProjectDriver_Client.Pages
{
    public class TripDetailedModel : PageModel
    {
        private readonly ILogger<TripDetailedModel> logger;
        private ApplicationDbContext context;
        public TripDetailedModel(ILogger<TripDetailedModel> _logger, ApplicationDbContext _context)
        {
            logger = _logger;
            context = _context;
        }

        public void OnGet()
        {
        }
    }
}
