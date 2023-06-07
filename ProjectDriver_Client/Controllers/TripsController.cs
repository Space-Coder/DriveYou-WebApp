using ProjectDriver_Client.Context;
using ProjectDriver_Client.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ProjectDriver_Client.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class TripsController : ControllerBase
    {
        private ApplicationDbContext db;
        private readonly ILogger<TripsController> _logger;
        public TripsController(ILogger<TripsController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            db = context;
        }

        [HttpGet("scheduled/my")]
        public async Task<IActionResult> GetMyScheduledTrips()
        {
            if (ModelState.IsValid)
            {
                if (User.Identity.IsAuthenticated)
                {
                    var user = await db.Users.Where(u => u.Number == long.Parse(User.Identity.Name))
                        .Select(i => i.ID)
                        .FirstOrDefaultAsync();                     
                    var tripsId = await db.ScheduledTrips.Where(u=>u.UserID == user).ToListAsync();
                    List<SubscribedOnTripsModel> subs = new List<SubscribedOnTripsModel>();
                    for (int i = 0; i < tripsId.Count; i++)
                    {
                        var subUser = await db.SubscribedOnTrips.Where(u => u.ScheduledTripsModelID == tripsId[i].ID).FirstOrDefaultAsync();
                        if (subUser != null)
                        {
                            subs.Add(subUser);
                        }

                    }
                    for (int i = 0; i < subs.Count; i++)
                    {
                        subs[i].User = await db.Users.Where(u=>u.ID == subs[i].UserID).FirstOrDefaultAsync();
                    }
                    var result = await db.ScheduledTrips.Where(u => u.UserID == user).Join(db.Users,
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
                        }).ToListAsync();
                    return new JsonResult(result);
                }
            }
            return BadRequest();
        }

        [HttpPost("scheduled")]
        public async Task<IActionResult> GetScheduledTrips(FindTripModel model)
        {
            if (ModelState.IsValid)
            {
                if (User.Identity.IsAuthenticated)
                {
                    var trips = await db.ScheduledTrips.Where(t=>t.From == model.From && t.To == model.To && EF.Functions.Like(t.Date.ToString(), $"%{model.Date.ToShortDateString()}%")).Join(db.Users,
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
                          }).ToListAsync();
                    return new JsonResult(trips);
                }
            }
            return BadRequest();
        }

        [HttpPost("scheduled/new")]
        public async Task<IActionResult> PostNewScheduledTrip(ScheduledTripsModel model)
        {
            if (ModelState.IsValid)
            {
                if (User.Identity.IsAuthenticated)
                {
                    var user = await db.Users.Where(u=>u.Number == long.Parse(User.Identity.Name) && u.CarModel != null && u.CarMark != null).FirstOrDefaultAsync();
                    if (user == null)
                    {
                        ModelState.AddModelError("", "Please, add car info first");
                        return NotFound(ModelState);
                    }
                    model.UserID = user.ID;
                    db.ScheduledTrips.Add(model);
                    await db.SaveChangesAsync();
                    return Ok();
                }
                return Unauthorized();
            }
            return BadRequest(ModelState);
        }

        [HttpDelete("scheduled/delete/{id}")]
        public async Task<IActionResult> DeleteScheduledTrip(int id)
        {
            if (ModelState.IsValid)
            {
                if (User.Identity.IsAuthenticated)
                {
                    var user = await db.Users.FirstOrDefaultAsync(u => u.Number == long.Parse(User.Identity.Name));
                    var trip = await db.ScheduledTrips.Where(i => i.ID == id && i.UserID == user.ID).FirstOrDefaultAsync();
                    if (trip == null)
                    {
                        ModelState.AddModelError("", "Can't find trip for current user");
                        return NotFound(ModelState);
                    }
                    db.ScheduledTrips.Remove(trip);
                    await db.SaveChangesAsync();
                    return Ok();
                }
            }
            return BadRequest();
        }

        [HttpGet("scheduled/subscribed")]
        public async Task<IActionResult> Subscribed()
        {
            if (ModelState.IsValid)
            {
                if (User.Identity.IsAuthenticated)
                {
                    var user = await db.Users.Where(u => u.Number == long.Parse(User.Identity.Name))
                        .Select(i => i.ID)
                        .FirstOrDefaultAsync();
                    var subs = await db.SubscribedOnTrips.Where(u=>u.UserID == user).Join(db.Users,
                        s => s.UserID,
                        u => u.ID,
                    (s, u) => new SubscribedOnTripsModel()
                    {
                        ID = s.ID,
                        User = u,
                        UserID = s.UserID,
                        ScheduledTripsModelID = s.ID
                    }).ToListAsync();
                    var result = await db.ScheduledTrips.Where(u => u.ID == u.SubscribedOnTrips
                    .Where(s=>s.UserID == user)
                    .Select(t=>t.ScheduledTripsModelID)
                    .FirstOrDefault() && u.SubscribedOnTrips
                    .Select(s=>s.UserID)
                    .FirstOrDefault() == user)
                        .Join(db.Users,
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
                        }).ToListAsync();
                    return new JsonResult(result);

                   
                }
                else
                {
                    return Unauthorized();
                }
            }
            else
            {
                ModelState.AddModelError("", "Model state is not valid");
            }
            return BadRequest(ModelState);
        }

        [HttpPost("scheduled/subscribe/{id}")]
        public async Task<IActionResult> SubscribeOnScheduledTrip(int id)
        {
            if (ModelState.IsValid)
            {
                if (User.Identity.IsAuthenticated)
                {
                    var user = await db.Users.FirstOrDefaultAsync(u => u.Number == long.Parse(User.Identity.Name));
                    int countSubs = db.SubscribedOnTrips.Where(t => t.ScheduledTripsModelID == id).ToList().Count();
                    var countSeats = await db.ScheduledTrips.FirstOrDefaultAsync(s => s.ID == id);
                    if (countSubs >= countSeats.Seats)
                    {
                        ModelState.AddModelError("", "Not enought seats");
                        return BadRequest(ModelState);
                    }
                    SubscribedOnTripsModel model = new SubscribedOnTripsModel()
                    {
                        ScheduledTripsModelID = id,
                        UserID = user.ID
                    };
                    db.SubscribedOnTrips.Add(model);
                    await db.SaveChangesAsync();
                    return Ok();
                }
            }
            return BadRequest();
        }

        [HttpPost("scheduled/unsubscribe/{id}")]
        public async Task<IActionResult> UnSubscribeOnScheduledTrip(int id)
        {
            if (ModelState.IsValid)
            {
                if (User.Identity.IsAuthenticated)
                {
                    var user = await db.Users.FirstOrDefaultAsync(u => u.Number == long.Parse(User.Identity.Name));
                    SubscribedOnTripsModel model = new SubscribedOnTripsModel()
                    {
                        ScheduledTripsModelID = id,
                        UserID = user.ID
                    };
                    db.SubscribedOnTrips.Remove(model);
                    await db.SaveChangesAsync();
                    return Ok();
                }
                return Unauthorized();
            }
            return BadRequest();
        }

        [HttpDelete("scheduled/end/{id}")]
        public async Task<IActionResult> EndScheduledTrip(int id)
        {
            if (ModelState.IsValid)
            {
                if (User.Identity.IsAuthenticated)
                {
                    var trip = await db.ScheduledTrips.FirstOrDefaultAsync(t => t.ID == id);
                    EndedTripsModel model = new EndedTripsModel()
                    {
                        TripID = id,
                        UserID = trip.UserID,
                        Date = trip.Date,
                        From = trip.From,
                        To = trip.To,
                        Cost = trip.Cost,
                        Comment = trip.Comment,
                    };
                    await db.AddAsync(model);
                    db.ScheduledTrips.Remove(trip);
                    await db.SaveChangesAsync();
                    return Ok();
                }
                return Unauthorized();
            }
            return BadRequest();
        }

        [HttpPost("scheduled/end/rate/{id}")]
        public async Task<IActionResult> RateEndedTrip(int id)
        {
            if (ModelState.IsValid)
            {
                if (User.Identity.IsAuthenticated)
                {
                    var user = await db.Users.FirstOrDefaultAsync(u => u.Number == long.Parse(User.Identity.Name));
                    var driver = await db.EndedTrips.FirstOrDefaultAsync(d => d.TripID == id);
                    UserReviewModel model = new UserReviewModel()
                    {
                        EndedTripsID = id,
                        UserID = user.ID,
                        ToID = driver.UserID,
                        Date = DateTime.Now,
                        Assessment = 4.5,
                        Comment = "Test"
                    };
                    await db.UserReviews.AddAsync(model);
                    await db.SaveChangesAsync();
                    return Ok();
                }
                return Unauthorized();
            }
            return BadRequest();
        }
    }
}
