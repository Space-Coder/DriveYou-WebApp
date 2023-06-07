using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
namespace ProjectDriver_Client.Models
{
    public class SubscribedOnTripsModel
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public virtual User User { get; set; }
        public int ScheduledTripsModelID { get; set; }
        //public int ScheduledTripsWithUserModelID { get; set; }
        //public virtual ScheduledTripsModel ScheduledTrips { get; set; }

    }
}
