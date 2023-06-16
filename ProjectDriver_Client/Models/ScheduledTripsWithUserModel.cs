using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DriveYOU_WebClient.Models
{
    public class ScheduledTripsWithUserModel
    {
        public ScheduledTripsModel ScheduledTrips { get; set; }
        public int ID { get; set; }
        public long Number { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public string Photo { get; set; }
        public double Rating { get; set; }
        public string CarMark { get; set; }
        public string CarModel { get; set; }
        public string CarImage { get; set; }
        public virtual List<SubscribedOnTripsModel> SubscribedOnTrips { get; set; }

    }
}
