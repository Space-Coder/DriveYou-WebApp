using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DriveYOU_WebClient.Models
{
    public class EndedTripsModel
    {
        public int ID { get; set; }
        public int TripID { get; set; }
        public int UserID { get; set; }
        public User User { get; set; }
        public DateTime Date { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public double Cost { get; set; }
        public string Comment { get; set; }
        public bool IsAnimals { get; set; }
        public bool IsSmoking { get; set; }
        public bool IsBagage { get; set; }
        public List<UserReviewModel> UserReviews { get; set; }

    }
}
