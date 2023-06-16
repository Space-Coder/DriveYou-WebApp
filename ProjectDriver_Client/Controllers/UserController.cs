using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using DriveYOU_WebClient.Context;
using DriveYOU_WebClient.Models;

namespace DriveYOU_WebClient.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [RequireHttps]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class UserController : ControllerBase
    {
        private ApplicationDbContext db;
        private readonly ILogger<UserController> _logger;
        public UserController(ILogger<UserController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            db = context;
        }

        // GET: api/<UserController>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Get()
        {
            if (ModelState.IsValid)
            {
                if (User.Identity.IsAuthenticated)
                {
                    return Ok();
                }
            }
            return BadRequest();
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMy()
        {
            if (ModelState.IsValid)
            {
                if (User.Identity.IsAuthenticated)
                {
                    var user = await db.Users.Where(u => u.Number == long.Parse(User.Identity.Name)).Select(u => u.ID).FirstOrDefaultAsync();
                    var reviews = await db.UserReviews.Where(u => u.ToID == user).Include(d => d.User).ToListAsync();
                    var result = await db.Users.Where(u => u.ID == user).Select(ui => new User
                    {
                        ID = ui.ID,
                        Number = ui.Number,
                        Password = ui.Password,
                        Email = ui.Email,
                        Name = ui.Name,
                        Surname = ui.Surname,
                        Date = ui.Date,
                        Photo = ui.Photo,
                        Rating = ui.Rating,
                        CarMark = ui.CarMark,
                        CarModel = ui.CarModel,
                        CarImage = ui.CarImage,
                        ScheduledTrips = ui.ScheduledTrips,
                        EndedTrips = ui.EndedTrips,
                        UserReviews = reviews,
                        SubscribedOnTrips = ui.SubscribedOnTrips,

                    }).FirstOrDefaultAsync();
                    return new JsonResult(result)
                    {
                        ContentType = "application/json;charset=utf-8",
                        StatusCode = 200,
                    };
                }
            }
            return BadRequest(ModelState);
        }

        [HttpPost("userphoto/set")]
        public async Task<IActionResult> SetUserImage(User model)
        {
            if (ModelState.IsValid)
            {
                if (User.Identity.IsAuthenticated)
                {
                    User user = await db.Users.Where(u => u.Number == long.Parse(User.Identity.Name)).FirstOrDefaultAsync();
                    user.Photo = model.Photo;
                    db.Users.Update(user);
                    await db.SaveChangesAsync();
                    return Ok();
                }
            }
            return BadRequest();
        }

        [HttpPost("carphoto/set")]
        public async Task<IActionResult> SetCarImage(User model)
        {
            if (ModelState.IsValid)
            {
                if (User.Identity.IsAuthenticated)
                {
                    User user = await db.Users.Where(u => u.Number == long.Parse(User.Identity.Name)).FirstOrDefaultAsync();
                    user.CarImage = model.CarImage;
                    if (string.IsNullOrWhiteSpace(model.CarMark))
                    {
                        user.CarMark = model.CarMark;
                    }
                    if (string.IsNullOrWhiteSpace(model.CarModel))
                    {
                        user.CarModel = model.CarModel;
                    }
                    db.Users.Update(user);
                    await db.SaveChangesAsync();
                    return Ok();
                }
            }
            return BadRequest();
        }

        // GET api/<UserController>/5
        //[Authorize]

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            if (ModelState.IsValid)
            {
                if (User.Identity.IsAuthenticated)
                {
                    var reviews = await db.UserReviews.Where(u => u.ToID == id).ToListAsync();
                    var result = await db.Users.Where(u => u.ID == id).Select(ui => new User
                    {
                        ID = ui.ID,
                        Number = ui.Number,
                        Password = ui.Password,
                        Email = ui.Email,
                        Name = ui.Name,
                        Surname = ui.Surname,
                        Date = ui.Date,
                        Photo = ui.Photo,
                        Rating = ui.Rating,
                        CarMark = ui.CarMark,
                        CarModel = ui.CarModel,
                        CarImage = ui.CarImage,
                        ScheduledTrips = ui.ScheduledTrips,
                        EndedTrips = ui.EndedTrips,
                        UserReviews = reviews,
                        SubscribedOnTrips = ui.SubscribedOnTrips,

                    }).FirstOrDefaultAsync();
                    return new JsonResult(result)
                    {
                        ContentType = "application/json;charset=utf-8",
                        StatusCode = 200,
                    };
                }
                else
                {
                    return Unauthorized();
                }
            }
            return BadRequest(ModelState);
        }

        // PUT api/<UserController>/5
        [HttpPut("{id}")]
        public IActionResult PutUpdateUserInfo(User user)
        {
            if (ModelState.IsValid)
            {
                if (User.Identity.IsAuthenticated)
                {
                    if (user != null)
                    {
                        db.Users.Update(user);
                        return Ok();
                    }
                    else
                    {
                        ModelState.AddModelError("Error_NotFound", "User not found!");
                        return NotFound(ModelState);
                    }
                }
                else
                {
                    ModelState.AddModelError("Error_Auth", "User is not authenticated!");
                    return BadRequest(ModelState);
                }
            }
            return BadRequest(ModelState);

        }

        // DELETE api/<UserController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (ModelState.IsValid)
            {
                if (User.Identity.IsAuthenticated)
                {
                    User delUser = db.Users.Find(id);
                    if (delUser != null)
                    {
                        ModelState.AddModelError("Error_NotFound", "User not found");
                        return NotFound(ModelState);
                    }
                    //TODO: Relocate user to "Deleted User's" Table
                    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    db.Users.Remove(delUser);
                    await db.SaveChangesAsync();
                    return Ok();
                }
            }
            return BadRequest(ModelState);
        }
    }
}
