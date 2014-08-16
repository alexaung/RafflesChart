using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RafflesChart.Models {
    public class ActivateSchemeViewModel {
        public ICollection<SelectListItem> Schemes { get; set; }

        [DisplayName("Scheme")]
        public int SelectedSchemeId { get; set; }

        [DisplayName("Expired Date")]
        public DateTime? ExpiredDate { get; set; }

        public HttpPostedFileBase Users { get; set; }
    }
}