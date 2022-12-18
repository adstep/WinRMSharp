using System.Security.Cryptography;

namespace WinRMSharp.Utils
{
    internal class Crypto
    {
        public static string ComputeSecurehash(string filePath)
        {
            using SHA1 sha1 = SHA1.Create();
            using FileStream fileStream = new FileStream(filePath, FileMode.Open);
            byte[] hashBytes = sha1.ComputeHash(fileStream);
            return BitConverter.ToString(hashBytes).Replace("-", string.Empty).ToLowerInvariant();
        }
    }
}
