using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace RafflesChart.Models {
    public class Scheme {
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

        public bool ScannerFlag   {get;set;}
        public bool CustomIndicatorsFlag  {get;set;}
        public bool LiveFlag   {get;set;}
        public bool CIAddFlag   {get;set;}
        public bool ScannerAddFlag   {get;set;}
        public bool SignalAddFlag   {get;set;}
        public bool TrendAddFlag   {get;set;}
        public bool PatternAddFlag   {get;set;}


    }
}