﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RafflesChart.Models {
    public class UserMarket : IUserFunction {
        [Key, Column(Order = 1)]
        public Guid UserId { get; set; }

        [Key, Column(Order = 2)]
        public string Market { get; set; }

        [NotMapped]
        string IUserFunction.FunctionName {
            get {
                return this.Market;
            }
            set {
                this.Market = value;
            }
        }
    }
}