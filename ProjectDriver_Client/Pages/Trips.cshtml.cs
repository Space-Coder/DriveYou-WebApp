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
    }
}
