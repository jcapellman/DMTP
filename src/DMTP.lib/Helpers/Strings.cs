using System;
using System.Linq;

namespace DMTP.lib.Helpers
{
    public static class Strings
    {
        public static string GenerateRandomString(int length = 20)
        {
            var random = new Random((int)DateTime.Now.Ticks);

            return new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", length).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}