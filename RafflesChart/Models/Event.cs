using System;
using System.Collections.Generic;
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
}