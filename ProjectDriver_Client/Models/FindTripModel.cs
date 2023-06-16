using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
namespace DriveYOU_WebClient.Models
{
    public class FindTripModel
    {
        public string From { get; set; }
        public string To { get; set; }
        public DateTime Date { get; set; }
    }
}
