using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProjectDriver_Client.Context;
using ProjectDriver_Client.Models;
using System.Collections.Generic;
using System.Linq;

namespace ProjectDriver_Client.Pages
{
    [RequireHttps]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class TripsModel : PageModel
    {
        [BindProperty]
        public FindTripModel? FindTripModel { get; set; }
        [BindProperty]
        public List<ScheduledTripsWithUserModel> TripsWithUser { get; set; } = new List<ScheduledTripsWithUserModel>();

        private readonly ILogger<TripsModel> logger;
        private ApplicationDbContext context;
        public TripsModel(ILogger<TripsModel> _logger , ApplicationDbContext _context)
        {
            logger = _logger;
            context = _context;
        }

        //Get trip from search
        public void OnGetTrips(FindTripModel findTripModel)
        {
            if (ModelState.IsValid)
            {
                TripsWithUser = context.ScheduledTrips.Where(t => t.From == findTripModel.From && t.To == findTripModel.To && EF.Functions.Like(t.Date.ToString(), $"%{findTripModel.Date.ToShortDateString()}%"))
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
                              ScheduledTrips = s
                          }).ToList();
            }
        }

        //Get user trips
        public void OnGetMyTrips()
        {
            if (ModelState.IsValid) 
            {
                if (User.Identity.IsAuthenticated)
                {
                    var user =  context.Users.Where(u => u.Number == long.Parse(User.Identity.Name))
                       .Select(i => i.ID)
                       .FirstOrDefault();
                    var tripsId = context.ScheduledTrips.Where(u => u.UserID == user).ToList();
                    List<SubscribedOnTripsModel> subs = new List<SubscribedOnTripsModel>();
                    for (int i = 0; i < tripsId.Count; i++)
                    {
                        var subUser = context.SubscribedOnTrips.Where(u => u.ScheduledTripsModelID == tripsId[i].ID).FirstOrDefault();
                        if (subUser != null)
                        {
                            subs.Add(subUser);
                        }

                    }
                    for (int i = 0; i < subs.Count; i++)
                    {
                        subs[i].User = context.Users.Where(u => u.ID == subs[i].UserID).FirstOrDefault();
                    }
                    TripsWithUser = context.ScheduledTrips.Where(u => u.UserID == user).Join(context.Users,
                        s => s.UserID,
                        u => u.ID,
                        (s, u) => new ScheduledTripsWithUserModel()
                        {
                            ID = u.ID,
                            Name = u.Name,
                            Date = u.Date,
                            Photo = u.Photo,
                            Rating = u.Rating,
                            CarMark = u.CarMark,
                            CarModel = u.CarModel,
                            CarImage = u.CarImage,
                            ScheduledTrips = s,
                            SubscribedOnTrips = subs
                        }).ToList();
                }
                else
                {
                    RedirectToPage("Login");
                }
            }
        }

        //Add new trip
        public void OnPostNewTrip(ScheduledTripsModel newTripModel)
        {
            if (ModelState.IsValid)
            {
                if (User.Identity.IsAuthenticated)
                {
                    var user = context.Users.Where(u => u.Number == long.Parse(User.Identity.Name) && u.CarModel != null && u.CarMark != null).FirstOrDefault();
                    if (user == null)
                    {
                        ModelState.AddModelError("", "Please, add car info first");
                        //Redirect to user page
                    }
                    newTripModel.UserID = user.ID;
                    context.ScheduledTrips.Add(newTripModel);
                    context.SaveChanges();
                }
            }
        }


        //Delete user trip by id
        public void OnGetDelTtip(int id)
        {
            if (ModelState.IsValid)
            {
                if (User.Identity.IsAuthenticated)
                {
                    var user =  context.Users.FirstOrDefault(u => u.Number == long.Parse(User.Identity.Name));
                    var trip =  context.ScheduledTrips.Where(i => i.ID == id && i.UserID == user.ID).FirstOrDefault();
                    if (trip == null)
                    {
                        ModelState.AddModelError("", "Can't find trip for current user");
                        //Show error
                    }
                    context.ScheduledTrips.Remove(trip);
                    context.SaveChangesAsync();
                }
            }
        }


        //Subscribe on trip
        public void OnGetSubOnTrip(int id)
        {
            if (ModelState.IsValid)
            {
                if (User.Identity.IsAuthenticated)
                {
                    var user = context.Users.FirstOrDefault(u => u.Number == long.Parse(User.Identity.Name));
                    int countSubs = context.SubscribedOnTrips.Where(t => t.ScheduledTripsModelID == id).ToList().Count();
                    var countSeats = context.ScheduledTrips.FirstOrDefault(s => s.ID == id);
                    if (countSubs >= countSeats.Seats)
                    {
                        ModelState.AddModelError("", "Not enought seats");
                        //Error
                    }
                    SubscribedOnTripsModel model = new SubscribedOnTripsModel()
                    {
                        ScheduledTripsModelID = id,
                        UserID = user.ID
                    };
                    context.SubscribedOnTrips.Add(model);
                    context.SaveChanges();
                }
            }
        }

        //Unsubscribe from trip
        public void OnGetUnsubOnTrip(int id)
        {
            if (ModelState.IsValid)
            {
                if (User.Identity.IsAuthenticated)
                {
                    var user = context.Users.FirstOrDefault(u => u.Number == long.Parse(User.Identity.Name));
                    SubscribedOnTripsModel model = new SubscribedOnTripsModel()
                    {
                        ScheduledTripsModelID = id,
                        UserID = user.ID
                    };
                    context.SubscribedOnTrips.Remove(model);
                    context.SaveChanges();
                }
            }
        }

        
        //User subscribed trips
        public void OnGetSubscribed()
        {
            if (ModelState.IsValid)
            {
                if (User.Identity.IsAuthenticated)
                {
                    var user = context.Users.Where(u => u.Number == long.Parse(User.Identity.Name))
                        .Select(i => i.ID)
                        .FirstOrDefault();
                    var subs = context.SubscribedOnTrips.Where(u => u.UserID == user).Join(context.Users,
                        s => s.UserID,
                        u => u.ID,
                    (s, u) => new SubscribedOnTripsModel()
                    {
                        ID = s.ID,
                        User = u,
                        UserID = s.UserID,
                        ScheduledTripsModelID = s.ID
                    }).ToList();

                    TripsWithUser = context.ScheduledTrips.Where(u => u.ID == u.SubscribedOnTrips
                    .Where(s => s.UserID == user)
                    .Select(t => t.ScheduledTripsModelID)
                    .FirstOrDefault() && u.SubscribedOnTrips
                    .Select(s => s.UserID)
                    .FirstOrDefault() == user)
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
                        }).ToList();
                }
                else
                {
                    RedirectToPage("Login");
                }
            }
            else
            {
                ModelState.AddModelError("", "Model state is not valid");
            }
        }
    }
}
