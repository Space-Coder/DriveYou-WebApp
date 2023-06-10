using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using ProjectDriver_Client.Context;

namespace ProjectDriver_Client.Pages
{
    public class AccountModel : PageModel
    {
        private readonly ILogger<AccountModel> logger;
        private ApplicationDbContext context;
        public AccountModel(ILogger<AccountModel> _logger, ApplicationDbContext _context)
        {
            logger = _logger;
            context = _context;
        }

        public void OnGet()
        {

        }
    }
}
