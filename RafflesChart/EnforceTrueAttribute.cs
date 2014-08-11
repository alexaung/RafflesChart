using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RafflesChart {
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class EnforceTrueAttribute : ValidationAttribute {
        public override bool IsValid(object value) {
            return value != null && value is bool && (bool)value;
        }
    }
}