using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DriveYOU_WebClient.Models
{
    public class ScheduledTripsModel
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public DateTime Date { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public int Seats { get; set; }
        public double Cost { get; set; }
        public string Comment { get; set; }
        public bool IsAnimals { get; set; }
        public bool IsSmoking { get; set; }
        public bool IsBagage { get; set; }
        public virtual List<SubscribedOnTripsModel> SubscribedOnTrips { get; set; }
    }
}
