using System;
using System.Globalization;
using System.Linq;

namespace Ironclad.Extensions
{
    public static class StringExtensions
    {
        public static TextInfo myTI = new CultureInfo("en-US", false).TextInfo;
        public static int ToInt(this string input) =>
            input switch
            {
                null => throw new ArgumentNullException(nameof(input)),
                "" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
                _ => Convert.ToInt32(input)
            };
        public static decimal ToDec(this string input) =>
            input switch
            {
                null => throw new ArgumentNullException(nameof(input)),
                "" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
                _ => Convert.ToDecimal(input)
            };
        public static bool ToBool(this string input) =>
            input switch
            {
                null => throw new ArgumentNullException(nameof(input)),
                "" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
                _ => Convert.ToBoolean(input)
            };
        public static string Capitalize(this string input) =>
            input switch
            {
                null => throw new ArgumentNullException(nameof(input)),
                "" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
                _ => input.First().ToString().ToUpper() + input.Substring(1)
            };
        public static string Title(this string input) =>
            input switch
            {
                null => throw new ArgumentNullException(nameof(input)),
                "" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
                _ => myTI.ToTitleCase(input)
            };
        public static string Rem(this string mystring, string input1, string input2 = "|x|x|", string input3 = "|x|x|", string input4 = "|x|x|", string input5 = "|x|x|", string input6 = "|x|x|", string input7 = "|x|x|") =>
            input1 switch
            {
                null => throw new ArgumentNullException(nameof(input1)),
                "" => throw new ArgumentException($"{nameof(input1)} cannot be empty", nameof(input1)),
                _ => mystring.Replace(input1, "").Replace(input2, "").Replace(input3, "").Replace(input4, "").Replace(input5, "").Replace(input6, "").Replace(input7, "")
            };
        public static string DropBlank(this string mystring, string input1, string input2 = "|x|x|", string input3 = "|x|x|", string input4 = "|x|x|", string input5 = "|x|x|", string input6 = "|x|x|", string input7 = "|x|x|") =>
            input1 switch
            {
                null => throw new ArgumentNullException(nameof(input1)),
                "" => throw new ArgumentException($"{nameof(input1)} cannot be empty", nameof(input1)),
                _ => mystring.Replace(input1, " ").Replace(input2, " ").Replace(input3, " ").Replace(input4, " ").Replace(input5, " ").Replace(input6, " ").Replace(input7, " ")
            };

    }
}
