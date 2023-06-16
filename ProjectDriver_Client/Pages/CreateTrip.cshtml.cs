using DriveYOU_WebClient.Context;
using DriveYOU_WebClient.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace DriveYOU_WebClient.Pages
{
    public class CreateTripModel : PageModel
    {
        [BindProperty]
        public ScheduledTripsModel TripModel { get; set; }
        [BindProperty]
        public Models.ErrorModel ErrorModel { get; set; }

        private readonly ILogger<CreateTripModel> logger;
        private ApplicationDbContext context;
        public CreateTripModel(ILogger<CreateTripModel> _logger, ApplicationDbContext _context)
        {
            logger = _logger;
            context = _context;
        }

        public void OnPost()
        {
            if (ModelState.IsValid)
            {
                if (User.Identity.IsAuthenticated)
                {
                    var currentUser = context.Users.Where(u => u.Number == long.Parse(User.Identity.Name)).FirstOrDefault();
                    TripModel.UserID = currentUser.ID;
                    context.ScheduledTrips.Add(TripModel);
                    context.SaveChanges();
                }
                else
                {
                    ErrorModel = new Models.ErrorModel("Authentication error", "Authentication error: User not authenticated");
                }
            }
            else
            {
                ErrorModel = new Models.ErrorModel("Model error", "Model error: Model state is invalid");
            }
        }
    }
}
