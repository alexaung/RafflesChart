using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RafflesChart.Extensions {
    public static class TempDataExtensions {
        private class Keys {
            public const string ActivateErrorEmails = "scheme-activate-error-emails";
        }

        public static void AddErrorEmails(this TempDataDictionary temp, IEnumerable<string> emails) {
            temp[Keys.ActivateErrorEmails] = emails;
        }

        public static IEnumerable<string> GetErrorEmails(this TempDataDictionary temp) {
            var emails = (IEnumerable<string>)temp[Keys.ActivateErrorEmails] ?? Enumerable.Empty<string>();

            return emails;
        }
    }
}