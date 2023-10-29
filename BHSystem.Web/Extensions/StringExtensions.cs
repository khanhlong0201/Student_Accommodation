using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BHSystem.Web.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Mở rộng phương thức Contains để k phân biệt hoa thường
        /// </summary>
        /// <param name="source"></param>
        /// <param name="value"></param>
        /// <param name="comp"></param>
        /// <returns></returns>
        public static bool Contains(this string source, string value, StringComparison comp)
        {
            return source?.IndexOf(value, comp) >= 0;
        }

        // Regular expression used to validate a phone number.
        public const string motif = @"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$";

        public static bool IsPhoneNbr(this string number) => Regex.IsMatch(number, motif);
    }
}