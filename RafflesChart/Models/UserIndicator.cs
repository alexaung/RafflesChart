using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RafflesChart.Models {
    public class UserIndicator : IUserFunction {
        [Key, Column(Order = 1)]
        public Guid UserId { get; set; }

        [Key, Column(Order = 2)]
        public string Indicator { get; set; }

        [NotMapped]
        string IUserFunction.FunctionName {
            get {
                return this.Indicator;
            }
            set {
                this.Indicator = value;
            }
        }
    }
}