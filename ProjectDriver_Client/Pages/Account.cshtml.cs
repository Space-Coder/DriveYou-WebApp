using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProjectDriver_Client.Context;
using ProjectDriver_Client.Models;
using System.Linq;

namespace ProjectDriver_Client.Pages
{
    public class AccountModel : PageModel
    {
        [BindProperty]
        public User UserModel { get; set; }
        [BindProperty]
        public int EndedTripsCount { get; set; }

        private readonly ILogger<AccountModel> logger;
        private ApplicationDbContext context;
        public AccountModel(ILogger<AccountModel> _logger, ApplicationDbContext _context)
        {
            logger = _logger;
            context = _context;
        }

        public void OnGet(int id = 1)
        {
            UserModel = context.Users.Where(u => u.ID == id)
                .Include(u=>u.ScheduledTrips)
                .Include(u => u.SubscribedOnTrips)
                .Include(u => u.UserReviews)
                .FirstOrDefault();
            EndedTripsCount = context.EndedTrips.Where(u => u.UserID == id).Count();
        }
    }
}
