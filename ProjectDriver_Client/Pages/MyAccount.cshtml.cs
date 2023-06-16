using DriveYOU_WebClient.Context;
using DriveYOU_WebClient.Models;
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

namespace DriveYOU_WebClient.Pages
{
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
        public Models.ErrorModel ErrorModel { get; set; }

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
                    UserModel = context.Users.Where(u => u.Number == long.Parse(User.Identity.Name)).FirstOrDefault();
                    EndedTripsCount = context.EndedTrips.Where(t => t.UserID == UserModel.ID).Count();
                    InputUserModel = new InputUserModel(UserModel);
                    InputPasswordModel = new InputPasswordModel(UserModel);
                    InputCarModel = new InputCarModel(UserModel);

                    TempUserModel = new User(UserModel);
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
                    if (InputUserModel.Photo == null)
                    {
                        InputUserModel.Photo = TempUserModel.Photo;
                    }
                    SetProperties(InputUserModel);
                    UserModel = TempUserModel;

                    context.Users.Update(UserModel);
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



        public void OnPostPassword()
        {
            if (ModelState.IsValid)
            {
                if (User.Identity.IsAuthenticated)
                {
                    if (InputPasswordModel.Password == TempUserModel.Password)
                    {
                        SetProperties(InputPasswordModel);
                        context.Users.Update(TempUserModel);
                    }
                    else
                    {
                        ModelState.AddModelError("InvalidPassword", "Invalid current password");
                        ErrorModel = new Models.ErrorModel("Data error", "Ivalid current password");
                    }
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

                        SetProperties(InputCarModel);
                        UserModel = TempUserModel;
                        context.Users.Update(UserModel);
                        context.SaveChanges();
                    }
                    else
                    {
                        ErrorModel = new Models.ErrorModel("Car error", "Add car error: Please add car image");
                    }
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
            Password = baseModel.Password;
        }
        public string Password { get; set; }
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
