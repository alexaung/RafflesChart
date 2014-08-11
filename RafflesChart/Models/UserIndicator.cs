using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RafflesChart.Models {
    public class UserIndicator {
        public int Id { get; set; }

        public String Code { get; set; }

        public ICollection<Scheme> Schemes { get; set; }

    }
}