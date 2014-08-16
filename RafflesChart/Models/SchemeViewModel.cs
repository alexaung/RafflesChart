using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RafflesChart.Models {
    public class SchemeViewModel {
        public int Id { get; set; }

        public String Description { get; set; }

        public String Name { get; set; }        
        
        public string Markets { get; set; }

        public string Indicators { get; set; }
        

        [DisplayName("Bull Bear Tests")]
        public string BullBearTests { get; set; }
        
        [DisplayName("Back Tests")]
        public string BackTests { get; set; }
                
        [DisplayName("Pattern Scanners")]
        public string PatternScanners { get; set; }

        public string Scanners { get; set; }
    }
}