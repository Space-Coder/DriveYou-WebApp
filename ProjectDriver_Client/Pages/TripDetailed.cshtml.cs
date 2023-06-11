using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using ProjectDriver_Client.Context;
using ProjectDriver_Client.Models;
using System.Linq;

namespace ProjectDriver_Client.Pages
{
    public class TripDetailedModel : PageModel
    {
        [BindProperty]
        public ScheduledTripsWithUserModel TripModel { get; set; }
        private readonly ILogger<TripDetailedModel> logger;
        private ApplicationDbContext context;
        public TripDetailedModel(ILogger<TripDetailedModel> _logger, ApplicationDbContext _context)
        {
            logger = _logger;
            context = _context;
        }

        public void OnGet(int id = 1)
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
                        
        }
    }
}
