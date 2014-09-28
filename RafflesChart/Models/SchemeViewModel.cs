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

        [DisplayName("Scanner")]
        public bool ScannerFlag { get; set; }
        [DisplayName("Custom Indicators")]
        public bool CustomIndicatorsFlag { get; set; }

        [DisplayName("Live")]
        public bool LiveFlag { get; set; }

        [DisplayName("CI Add")]
        public bool CIAddFlag { get; set; }

        [DisplayName("Scanner Add")]
        public bool ScannerAddFlag { get; set; }

        [DisplayName("Signal Add")]
        public bool SignalAddFlag { get; set; }

        [DisplayName("Trend Add")]
        public bool TrendAddFlag { get; set; }

        [DisplayName("Pattern Add")]
        public bool PatternAddFlag { get; set; }
    }
}