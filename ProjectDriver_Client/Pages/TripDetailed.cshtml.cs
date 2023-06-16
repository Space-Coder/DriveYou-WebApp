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
    public class TripDetailedModel : PageModel
    {
        [BindProperty]
        public ScheduledTripsWithUserModel TripModel { get; set; }
        [BindProperty]
        public int UserID { get; set; }
        [BindProperty]
        public bool IsTripOwner { get; set; }
        [BindProperty]
        public bool IsSubscribed { get; set; }
        [BindProperty]
        public Models.ErrorModel ErrorModel { get; set; }

        private readonly ILogger<TripDetailedModel> logger;
        private ApplicationDbContext context;
        public TripDetailedModel(ILogger<TripDetailedModel> _logger, ApplicationDbContext _context)
        {
            logger = _logger;
            context = _context;
        }

        public void OnGet(int id)
        {
            if (ModelState.IsValid)
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
                ErrorModel = new Models.ErrorModel("Model error", "ModelState is not valid");
            }
        }


        public void OnGetSubscribeUser(int id)
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
                }
                else
                {
                    ErrorModel = new Models.ErrorModel("Authentication error", "Authentication error: User not authenticated");
                }
            }
            else
            {
                ErrorModel = new Models.ErrorModel("Model error", "ModelState is not valid");
            }
        }

        public void OnGetUnsubscribeUser(int id)
        {
            if (ModelState.IsValid)
            {
                if (User.Identity.IsAuthenticated)
                {
                    var user = context.Users.FirstOrDefault(u => u.Number == long.Parse(User.Identity.Name));
                    var tripToUnsub = context.SubscribedOnTrips.FirstOrDefault(t => t.UserID == user.ID && t.ScheduledTripsModelID == id);
                    context.SubscribedOnTrips.Remove(tripToUnsub);
                    context.SaveChanges();
                }
            }
            else
            {
                ErrorModel = new Models.ErrorModel("Model error", "ModelState is not valid");
            }
        }

        public void OnGetDelTrip(int id)
        {
            if (ModelState.IsValid)
            {
                if (User.Identity.IsAuthenticated)
                {
                    var tripToDelete = context.ScheduledTrips.Where(u => u.ID == id).FirstOrDefault();
                    context.ScheduledTrips.Remove(tripToDelete);
                    context.SaveChanges();
                }
                else
                {
                    ErrorModel = new Models.ErrorModel("Model error", "ModelState is not valid");
                }
            }
        }
    }
}
