using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RafflesChart.Models {
    [System.ComponentModel.DataAnnotations.Schema.Table("UserBackTest")]
    public class UserBackTest : IUserFunction {

        [Key, Column(Order = 1)]
        public Guid UserId { get; set; }

        [Key, Column(Order = 2)]
        public string FormulaName { get; set; }

        [NotMapped]
        string IUserFunction.FunctionName {
            get {
                return this.FormulaName;
            }
            set {
                this.FormulaName = value;
            }
        }

    }
}