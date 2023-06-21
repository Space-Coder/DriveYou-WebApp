using DriveYOU_WebClient.Context;
using DriveYOU_WebClient.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DriveYOU_WebClient.Pages
{
    [RequireHttps]
    [BindProperties]
    public class MyAccountModel : PageModel
    {
        private static User TempUserModel;
        public User UserModel { get; set; }
        public InputUserModel InputUserModel { get; set; }
        public InputPasswordModel InputPasswordModel { get; set; }
        public InputCarModel InputCarModel { get; set; }
        public int EndedTripsCount { get; set; }
        public IFormFile ImageFile { get; set; }
        public Models.MessageModel MessageModel { get; set; }

        private readonly ILogger<MyAccountModel> logger;
        private ApplicationDbContext context;

        public MyAccountModel(ILogger<MyAccountModel> _logger, ApplicationDbContext _context)
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
                    UserModel = context.Users.Where(u => u.Number == long.Parse(User.Identity.Name)).FirstOrDefault();
                    EndedTripsCount = context.EndedTrips.Where(t => t.UserID == UserModel.ID).Count();
                    TempUserModel = new User(UserModel);
                    RefreshModels(UserModel);
                }
                else
                {
                    MessageModel = new Models.MessageModel("Authentication error", "Authentication error: User not authenticated");
                    return RedirectToPage("Login");
                }
            }
            else
            {
                MessageModel = new Models.MessageModel("Model error", "ModelState is not valid");
            }
            return Page();
        }

        public async Task<IActionResult> OnGetLogout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToPage("Index");
        }

        public void OnPostUser()
        {
            if (ModelState.IsValid)
            {
                if (User.Identity.IsAuthenticated)
                {
                    if (ImageFile != null)
                    {
                        byte[] imageData = null;
                        using (var br = new BinaryReader(ImageFile.OpenReadStream()))
                        {
                            imageData = br.ReadBytes((int)ImageFile.Length);
                        }
                        InputUserModel.Photo = Convert.ToBase64String(imageData);
                    }
                    else
                    {
                        InputUserModel.Photo = TempUserModel.Photo;
                    }
                    SetProperties(InputUserModel);
                    UserModel = TempUserModel;
                    context.Users.Update(UserModel);
                    context.SaveChanges();
                    RefreshModels(UserModel);
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
        }



        public void OnPostPassword()
        {
            if (ModelState.IsValid)
            {
                if (User.Identity.IsAuthenticated)
                {
                    if (InputPasswordModel.OldPassword == TempUserModel.Password)
                    {
                        if (InputPasswordModel.Password == InputPasswordModel.ConfirmPassword)
                        {
                            SetProperties(InputPasswordModel);
                            UserModel = TempUserModel;
                            context.Users.Update(UserModel);
                            context.SaveChanges();
                            RefreshModels(UserModel);
                        }
                        else
                        {
                            MessageModel = new Models.MessageModel("New password", "New password and confirm are not mathed");
                        }
                    }
                    else
                    {
                        MessageModel = new Models.MessageModel("Data error", "Ivalid current password");
                    }
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
        }

        public void OnPostCar()
        {
            if (ModelState.IsValid)
            {
                if (User.Identity.IsAuthenticated)
                {
                    if (ImageFile != null)
                    {
                        byte[] imageData = null;
                        using (var br = new BinaryReader(ImageFile.OpenReadStream()))
                        {
                            imageData = br.ReadBytes((int)ImageFile.Length);
                        }
                        InputCarModel.CarImage = Convert.ToBase64String(imageData);
                    }
                    else
                    {
                        InputCarModel.CarImage = TempUserModel.CarImage;
                    }
                    SetProperties(InputCarModel);
                    UserModel = TempUserModel;
                    context.Users.Update(UserModel);
                    context.SaveChanges();
                    RefreshModels(UserModel);
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
        }

        private void SetProperties(object objModel)
        {
            PropertyInfo[] baseProps = TempUserModel.GetType().GetProperties();
            PropertyInfo[] objProps = objModel.GetType().GetProperties();

            foreach (var item in objProps)
            {
                var prop = baseProps.Where(p => p.Name == item.Name).FirstOrDefault();
                if (prop != null)
                {
                    if (objModel.GetType() == typeof(InputUserModel))
                    {
                        prop.SetValue(TempUserModel, item.GetValue(InputUserModel, null));
                    }
                    if (objModel.GetType() == typeof(InputPasswordModel))
                    {
                        prop.SetValue(TempUserModel, item.GetValue(InputPasswordModel, null));
                    }
                    if (objModel.GetType() == typeof(InputCarModel))
                    {
                        prop.SetValue(TempUserModel, item.GetValue(InputCarModel, null));
                    }

                }
            }
        }

        private void RefreshModels(User userModel)
        {
            InputUserModel = new InputUserModel(userModel);
            InputPasswordModel = new InputPasswordModel(userModel);
            InputCarModel = new InputCarModel(userModel);
        }
    }

    public class InputUserModel
    {
        public InputUserModel() { }
        public InputUserModel(User baseModel)
        {
            Number = baseModel.Number;
            Email = baseModel.Email;
            Name = baseModel.Name;
            Surname = baseModel.Surname;
            Date = baseModel.Date;
            Photo = baseModel.Photo;
        }
        public long Number { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime Date { get; set; }
        public string Photo { get; set; }
    }

    public class InputPasswordModel
    {
        public InputPasswordModel() { }
        public InputPasswordModel(User baseModel)
        {
            OldPassword = baseModel.Password;
        }
        public string OldPassword { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }

    public class InputCarModel
    {
        public InputCarModel() { }
        public InputCarModel(User baseModel)
        {
            CarMark = baseModel.CarMark;
            CarModel = baseModel.CarModel;
            CarImage = baseModel.CarImage;
        }
        public string CarModel { get; set; }
        public string CarMark { get; set; }
        public string CarImage { get; set; }
    }
}
