using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using RafflesChart.Models;

namespace RafflesChart.Extensions {
    public static class TempDataExtensions {
        private class Keys {
            public const string ActivateErrorEmails = "scheme-activate-error-emails";
            public const string UploadUserResult = "upload-user-result";
        }

        public static void AddErrorEmails(this TempDataDictionary temp, IEnumerable<string> emails) {
            temp[Keys.ActivateErrorEmails] = emails;
        }

        public static IEnumerable<string> GetErrorEmails(this TempDataDictionary temp) {
            var emails = (IEnumerable<string>)temp[Keys.ActivateErrorEmails] ?? Enumerable.Empty<string>();

            return emails;
        }


        public static void AddUploadUserResult(this TempDataDictionary temp, UploadUserResult result) {
            temp[Keys.UploadUserResult] = result;
        }

        public static UploadUserResult GetUploadUserResult(this TempDataDictionary temp) {
            var result = (UploadUserResult)temp[Keys.UploadUserResult] ?? new UploadUserResult() { ErrorUserEmails = Enumerable.Empty<string>() };

            return result;
        }
    }
}