using System.Text;

namespace WinRMSharp.Utils
{
    internal static class StringExtensions
    {
        public static string EncodeBase64(this string value, Encoding encoding)
        {
            byte[] valueBytes = encoding.GetBytes(value);
            return Convert.ToBase64String(valueBytes);
        }

        public static string DecodeBase64(this string value, Encoding encoding)
        {
            byte[] valueBytes = Convert.FromBase64String(value);
            return encoding.GetString(valueBytes);
        }
    }
}
