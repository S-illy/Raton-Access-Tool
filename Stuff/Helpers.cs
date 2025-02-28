using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Stuff
{
    public class Helpers
    {
        private static string STRING = "abcdefghijklmnopqrstuvwxyz";
        private static string INTEGER = "0123456789";

        private static Random charsRandom = new Random();
        private static Random lengthRandom = new Random();
        public static string Random(int length = 0)
        {
            if (length == 0) length = 30;
            lengthRandom.Next(1, length);
            string chars = STRING.ToUpper() + STRING + INTEGER;
            return new string(Enumerable.Repeat(chars, length).Select(s => s[charsRandom.Next(s.Length)]).ToArray());
        }
        public static string BytesToString(long byteCount)
        {
            string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" };
            if (byteCount == 0)
                return "0" + suf[0];
            long bytes = Math.Abs(byteCount);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 3);
            return (Math.Sign(byteCount) * num).ToString() + suf[place];
        }
        public static string ConvertToHex(byte[] bytes)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in bytes)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }
        public static string MD5_STRING(byte[] bytes)
        {
            MD5 md5 = MD5.Create();
            bytes = md5.ComputeHash(bytes, 0, bytes.Length);
            return ConvertToHex(bytes);
        }
        public static string SHA1_STRING(byte[] bytes)
        {
            SHA1 sha1 = SHA1.Create();
            bytes = sha1.ComputeHash(bytes, 0, bytes.Length);
            StringBuilder sb = new StringBuilder();
            return ConvertToHex(bytes);
        }
        public static string SHA256_STRING(byte[] bytes)
        {
            SHA256 sha256 = SHA256.Create();
            bytes = sha256.ComputeHash(bytes, 0, bytes.Length);
            StringBuilder sb = new StringBuilder();
            return ConvertToHex(bytes);
        }
    }
}
