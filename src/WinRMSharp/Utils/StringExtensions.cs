using System.Text;

namespace WinRMSharp.Utils
{
    internal static class StringExtensions
    {
        public static string EncodeBase64(this string value)
        {
            byte[] valueBytes = Encoding.UTF8.GetBytes(value);
            return Convert.ToBase64String(valueBytes);
        }

        public static string DecodeBase64(this string value)
        {
            byte[] valueBytes = Convert.FromBase64String(value);
            return Encoding.UTF8.GetString(valueBytes);
        }
    }
}
