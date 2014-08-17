using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace RafflesChart.Models {
    public class UploadUserResult {        
        public int SuccessUserCount { get; set; }

        public IEnumerable<string> ErrorUserEmails { get; set; }
    }
}