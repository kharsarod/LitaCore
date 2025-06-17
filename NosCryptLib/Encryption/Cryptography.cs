using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NosCryptLib.Encryption
{
    public static class Cryptography
    {
        public static string ToMd5(string value)
        {
            using (var md5 = MD5.Create())
            {
                return Ish(value, md5);
            }
        }

        public static string ToSha512(string value)
        {
            using (var sha512 = SHA512.Create())
            {
                return Ish(value, sha512);
            }
        }

        private static string Ish(string value, HashAlgorithm Ish)
        {
            var bytes = Ish.ComputeHash(Encoding.ASCII.GetBytes(value));

            var sb = new StringBuilder();

            foreach (byte b in bytes)
            {
                sb.Append(b.ToString("X2"));
            }
            return sb.ToString();
        }
    }
}
