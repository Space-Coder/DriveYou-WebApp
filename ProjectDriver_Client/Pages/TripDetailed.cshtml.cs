using DriveYOU_WebClient.Context;
using DriveYOU_WebClient.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace DriveYOU_WebClient.Pages
{
    [RequireHttps]
    public class TripDetailedModel : PageModel
    {
        private ScheduledTripsWithUserModel TempTripModel { get; set; }
        [BindProperty]
        public ScheduledTripsWithUserModel TripModel { get; set; }
        [BindProperty]
        public int UserID { get; set; }
        [BindProperty]
        public bool IsTripOwner { get; set; }
        [BindProperty]
        public bool IsSubscribed { get; set; }
        [BindProperty]
        public Models.MessageModel MessageModel { get; set; }

        private readonly ILogger<TripDetailedModel> logger;
        private ApplicationDbContext context;
        public TripDetailedModel(ILogger<TripDetailedModel> _logger, ApplicationDbContext _context)
        {
            logger = _logger;
            context = _context;
        }

        public IActionResult OnGet(int id)
        {
            if (ModelState.IsValid)
            {
                if (User.Identity.IsAuthenticated)
                {
                    var subs = context.SubscribedOnTrips.Where(u => u.ScheduledTripsModelID == id).Join(context.Users,
                       s => s.UserID,
                       u => u.ID,
                   (s, u) => new SubscribedOnTripsModel()
                   {
                       ID = s.ID,
                       User = u,
                       UserID = s.UserID,
                       ScheduledTripsModelID = s.ID
                   }).ToList();

                    TripModel = context.ScheduledTrips.Where(t => t.ID == id)
                        .Join(context.Users,
                        s => s.UserID,
                        u => u.ID,
                        (s, u) => new ScheduledTripsWithUserModel()
                        {
                            ID = u.ID,
                            Number = u.Number,
                            Name = u.Name,
                            Date = u.Date,
                            Photo = u.Photo,
                            Rating = u.Rating,
                            CarMark = u.CarMark,
                            CarModel = u.CarModel,
                            CarImage = u.CarImage,
                            ScheduledTrips = s,
                            SubscribedOnTrips = subs
                        }).FirstOrDefault();



                    var user = context.Users.Where(u => u.Number == long.Parse(User.Identity.Name)).FirstOrDefault();
                    var sub = context.SubscribedOnTrips.Where(t => t.ScheduledTripsModelID == id && t.UserID == user.ID).FirstOrDefault();
                    IsSubscribed = sub != null ? true : false;
                    if (user != null)
                    {
                        IsTripOwner = context.ScheduledTrips.Where(t => t.ID == id).FirstOrDefault().UserID == user.ID ? true : false;
                        UserID = user.ID;
                    }
                }
                else
                {
                    return RedirectToPage("Login");
                }
               
            }
            else
            {
                MessageModel = new Models.MessageModel("Model error", "ModelState is not valid");
            }
            return Page();
        }


        public IActionResult OnGetSubscribeUser(int id)
        {
            if (ModelState.IsValid)
            {
                if (User.Identity.IsAuthenticated)
                {
                    var user = context.Users.FirstOrDefault(u => u.Number == long.Parse(User.Identity.Name));
                    SubscribedOnTripsModel tripSubModel = new SubscribedOnTripsModel()
                    {
                        UserID = user.ID,
                        ScheduledTripsModelID = id,
                    };
                    context.SubscribedOnTrips.Add(tripSubModel);
                    context.SaveChanges();
                    return Redirect($"TripDetailed?id={id}");
                }
                else
                {
                    MessageModel = new Models.MessageModel("Authentication error", "Authentication error: User not authenticated");
                }
            }
            else
            {
                MessageModel = new Models.MessageModel("Model error", "ModelState is not valid");
            }
            return Page();
        }

        public IActionResult OnGetUnsubscribeUser(int id)
        {
            if (ModelState.IsValid)
            {
                if (User.Identity.IsAuthenticated)
                {
                    var user = context.Users.FirstOrDefault(u => u.Number == long.Parse(User.Identity.Name));
                    var tripToUnsub = context.SubscribedOnTrips.FirstOrDefault(t => t.UserID == user.ID && t.ScheduledTripsModelID == id);
                    context.SubscribedOnTrips.Remove(tripToUnsub);
                    context.SaveChanges();
                    return Redirect($"TripDetailed?id={id}");
                }
            }
            else
            {
                MessageModel = new Models.MessageModel("Model error", "ModelState is not valid");
            }
            return Page();
        }

        public IActionResult OnGetDelTrip(int id)
        {
            if (ModelState.IsValid)
            {
                if (User.Identity.IsAuthenticated)
                {
                    var tripToDelete = context.ScheduledTrips.Where(u => u.ID == id).FirstOrDefault();
                    context.ScheduledTrips.Remove(tripToDelete);
                    context.SaveChanges();
                    return RedirectToPage("Trips", "MyTrip");
                }
                else
                {
                    MessageModel = new Models.MessageModel("Model error", "ModelState is not valid");
                }
            }
            return Page();
        }
    }
}
