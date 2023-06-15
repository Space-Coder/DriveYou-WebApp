using Microsoft.EntityFrameworkCore;
using ProjectDriver_Client.Pages;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ProjectDriver_Client.Models
{
    public class User
    {
        public User() {}
        public User(User copyModel)
        {
            ID = copyModel.ID;
            Number = copyModel.Number;
            Password = copyModel.Password;
            Email = copyModel.Email;
            Name = copyModel.Name;
            Surname = copyModel.Surname;
            Date = copyModel.Date;
            Photo = copyModel.Photo;
            Rating = copyModel.Rating;
            CarMark = copyModel.CarMark;
            CarModel = copyModel.CarModel;
            CarImage = copyModel.CarImage;
            ScheduledTrips = copyModel.ScheduledTrips;
            EndedTrips = copyModel.EndedTrips;
            UserReviews = copyModel.UserReviews;
            SubscribedOnTrips = copyModel.SubscribedOnTrips;
        }
        /*    public User() { }
            public User(InputUserModel userModel, InputPasswordModel passwordModel, InputCarModel carModel)
            {
                if (userModel != null)
                {
                    Number = userModel.Number;
                    Email = userModel.Email;
                    Name = userModel.Name;
                    Surname = userModel.Surname;
                    Date = userModel.Date;
                    Photo = userModel.Photo;
                }
                if (passwordModel != null)
                {
                    Password = passwordModel.Password;
                }
                if (carModel != null)
                {
                    CarMark = carModel.CarMark;
                    CarModel = carModel.CarModel;
                    CarImage = carModel.CarImage;
                }
            }*/
        public int ID { get; set; }
        public long Number { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime Date { get; set; }
        public string Photo { get; set; }
        public double Rating { get; set; }
        public string CarMark { get; set; }
        public string CarModel { get; set; }
        public string CarImage { get; set; }
        public virtual List<ScheduledTripsModel> ScheduledTrips { get; set; }
        public virtual List<EndedTripsModel> EndedTrips { get; set; }
        public virtual List<UserReviewModel> UserReviews { get; set; }
        public virtual List<SubscribedOnTripsModel> SubscribedOnTrips { get; set; }
    }
}
