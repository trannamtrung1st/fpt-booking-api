using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FPTBooking.Business.Helpers
{
    public static class EmailHelper
    {
        public static bool IsStudent(this string email)
        {
            return Regex.IsMatch(email, "([a-zA-Z]{2}[0-9]{5,}?)@", RegexOptions.IgnoreCase);
        }

        public static string GetStudentCode(this string email)
        {
            var match = Regex.Match(email, "([a-zA-Z]{2}[0-9]+?)@", RegexOptions.IgnoreCase);
            var code = match.Groups[1].Value;
            return code;
        }

        public static string GetTeacherCode(this string email)
        {
            var match = Regex.Match(email, "(.+?)@", RegexOptions.IgnoreCase);
            var code = match.Groups[1].Value;
            return code;
        }

        public static bool IsFptEmail(this string email)
        {
            string[] emails = email.Trim().Split("@");
            if (emails.Length == 2)
                return emails[1] == AllowedEmailDomains.FPT_DOMAIN;
            return false;
        }

        public static ValueTuple<bool, bool, string> GetEmailInfo(this string email)
        {
            var isFptEmail = email.IsFptEmail();
            var isStudent = isFptEmail ? email.IsStudent() : false;
            var code = isStudent ? email.GetStudentCode() :
                (!isStudent ? email.GetTeacherCode() : null);
            return (isFptEmail, isStudent, code);
        }
    }
}
