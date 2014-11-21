using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace RafflesChart.Models {
    public class ActivateFunctionViewModel {
        [DisplayName("Bull Bear Tests")]
        public string BullBearTests { get; set; }

        [DisplayName("Back Tests")]
        public string BackTests { get; set; }

        [DisplayName("Pattern Scanners")]
        public string PatternScanners { get; set; }

        public string Scanners { get; set; }

        public string Markets { get; set; }

        public string Indicators { get; set; }

        [DisplayName("Expired Date")]
        public DateTime? ExpiredDate { get; set; }

        public HttpPostedFileBase Users { get; set; }

        [DisplayName("Replace Records")]
        public bool ReplaceRecords { get; set; }
    }
}