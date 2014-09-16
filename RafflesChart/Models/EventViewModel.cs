using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace RafflesChart.Models {
    public class EventViewModel {
        public int AvailableEventId { get; set; }
        public string AvailableEventName { get; set; }
        public string AvailableEventLocation { get; set; }
        public string AvailableEventDescription { get; set; }
        public DateTime AvailableEventDate { get; set; }
        public bool Registered { get; set; }

        public IEnumerable<Registrant> Users { get; set; }
    }
}