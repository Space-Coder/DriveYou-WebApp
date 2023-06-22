using DriveYOU_WebClient.Context;
using DriveYOU_WebClient.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using System.Linq;
using System.Threading.Tasks;

namespace DriveYOU_WebClient.Pages
{
    [RequireHttps]
    public class CreateTripModel : PageModel
    {
        [BindProperty]
        public bool isHaveCar { get; set; }
        [BindProperty]
        public ScheduledTripsModel TripModel { get; set; }
        [BindProperty]
        public Models.MessageModel MessageModel { get; set; }
        private readonly ILogger<CreateTripModel> logger;
        private ApplicationDbContext context;
        public CreateTripModel(ILogger<CreateTripModel> _logger, ApplicationDbContext _context)
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
                    var user = context.Users.Where(u => u.Number == long.Parse(User.Identity.Name)).FirstOrDefault();
                    if (user != null) 
                    {
                        if (!string.IsNullOrEmpty(user.CarMark))
                        {
                            isHaveCar = true;
                        }
                        else
                        {
                            isHaveCar = false;
                            MessageModel = new Models.MessageModel("CarException", "User has no car, please add car info");
                        }
                    }
                    else
                    {
                        MessageModel = new Models.MessageModel("ArgumentNullException", "User not found");
                    }
                }
                else
                {
                    MessageModel = new Models.MessageModel("Authentication error", "Authentication error: User not authenticated");
                }
            }
            return Page();
        }

        public IActionResult OnPost()
        {
            if (ModelState.IsValid)
            {
                if (User.Identity.IsAuthenticated)
                {
                    var currentUser = context.Users.Where(u => u.Number == long.Parse(User.Identity.Name)).FirstOrDefault();
                    if (string.IsNullOrEmpty(currentUser.CarMark)) 
                    {
                    }else
                    TripModel.UserID = currentUser.ID;
                    context.ScheduledTrips.Add(TripModel);
                    context.SaveChanges();
                    return RedirectToPage("Trips", "MyTrips");
                }
                else
                {
                    MessageModel = new Models.MessageModel("Authentication error", "Authentication error: User not authenticated");
                }
            }
            else
            {
                MessageModel = new Models.MessageModel("Model error", "Model error: Model state is invalid");
            }
            return Page();
        }
    }
}
