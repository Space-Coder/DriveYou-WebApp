using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using ProjectDriver_Client.Context;
using ProjectDriver_Client.Models;
using System;
using System.Linq;
using System.Text;

namespace ProjectDriver_Client.Pages
{
    public class MyAccountModel : PageModel
    {
        [BindProperty]
        public User UserModel { get; set; }
        [BindProperty]
        public int EndedTripsCount { get; set; }

        [BindProperty]
        public IFormFile ImageFile { get; set; } 

        private readonly ILogger<MyAccountModel> logger;
        private ApplicationDbContext context;

        public MyAccountModel(ILogger<MyAccountModel> _logger, ApplicationDbContext _context)
        {
            logger = _logger;
            context = _context;
        }


        public void OnGet()
        {
            if (ModelState.IsValid)
            {
                if (User.Identity.IsAuthenticated)
                {
                    UserModel = context.Users.Where(u => u.Number == long.Parse(User.Identity.Name))
                        .Include(u=>u.ScheduledTrips)
                        .Include(u=>u.SubscribedOnTrips)
                        .Include(u=>u.UserReviews)
                        .FirstOrDefault();
                    EndedTripsCount = context.EndedTrips.Where(t => t.UserID == UserModel.ID).Count();
                }
            }
        }


        public void OnPostCar()
        {
            if (this.ImageFile != null)
            {
                //UserModel.CarImage = Encoding.UTF8.GetBytes((byte)this.ImageFile);
            }
        }



    }
}
