using Microsoft.EntityFrameworkCore;
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
