using DriveYOU_WebClient.Context;
using DriveYOU_WebClient.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace DriveYOU_WebClient.Pages
{
    public class AccountModel : PageModel
    {
        [BindProperty]
        public User UserModel { get; set; }
        [BindProperty]
        public int EndedTripsCount { get; set; }
        [BindProperty]
        public Models.ErrorModel ErrorModel { get; set; }

        private readonly ILogger<AccountModel> logger;
        private ApplicationDbContext context;
        public AccountModel(ILogger<AccountModel> _logger, ApplicationDbContext _context)
        {
            logger = _logger;
            context = _context;
        }

        public void OnGet(int id)
        {
            if (ModelState.IsValid)
            {
                if (User.Identity.IsAuthenticated)
                {
                    UserModel = context.Users.Where(u => u.ID == id)
                .Include(u => u.ScheduledTrips)
                .Include(u => u.SubscribedOnTrips)
                .FirstOrDefault();
                    if (UserModel != null)
                    {
                        UserModel.UserReviews = context.UserReviews.Where(r => r.ToID == id)
                        .Include(u => u.User)
                        .ToList();
                    }
                    EndedTripsCount = context.EndedTrips.Where(u => u.UserID == id).Count();
                }
                else
                {
                    ErrorModel = new Models.ErrorModel("Authentication error", "Authentication error: User not authenticated");
                }
            }
            else
            {
                ErrorModel = new Models.ErrorModel("ModelState error", "ModelStat error: Model state is not valid");
            }
        }
    }
}
