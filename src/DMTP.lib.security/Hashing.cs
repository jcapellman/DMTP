using System;
using System.Security.Cryptography;
using System.Text;

namespace DMTP.lib.Security
{
    public static class Hashing
    {
        public static string ToSHA1(this byte[] data)
        {
            if (data == null || data.Length == 0)
            {
                return null;
            }

            using (var sha = new SHA1Managed())
            {
                return BitConverter.ToString(sha.ComputeHash(data)).Replace("-", "");
            }
        }

        public static string ToSHA1(this string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return null;
            }

            using (var sha = new SHA1Managed())
            {
                return BitConverter.ToString(sha.ComputeHash(Encoding.ASCII.GetBytes(data))).Replace("-", "");
            }
        }
    }
}