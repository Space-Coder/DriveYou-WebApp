using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DriveYOU_WebClient.Models
{
    public class UserReviewModel
    {
        public int ID { get; set; }
        public int EndedTripsID { get; set; }
        public int UserID { get; set; }
        public virtual User User { get; set; }
        public int ToID { get; set; }
        public DateTime Date { get; set; }
        public double Assessment { get; set; }
        public string Comment { get; set; }

    }
}
