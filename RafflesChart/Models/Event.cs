using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RafflesChart.Models {
    public class Event {
        public int Id { get; set; }

        public String Name { get; set; }

        public DateTime Date { get; set; }

        public String Location { get; set; }

        public String Description { get; set; }
    }
    public class EventUser
    {
        [Key, Column(Order=1)]
        public int EventId { get; set; }
      [Key, Column(Order = 2)]
        public string UserEmail { get; set; }
    }

    public class EventGuestUser
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public String Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
    }
}