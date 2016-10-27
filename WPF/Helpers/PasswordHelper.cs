using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace WPF.Helpers
{
    public static class PasswordHelper
    {
        public static string Hash(string value)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(value);
            var csp = new MD5CryptoServiceProvider();
            byte[] byteHash = csp.ComputeHash(bytes);

            StringBuilder sb = new StringBuilder();
            foreach (byte b in byteHash)
            {
                sb.Append(b.ToString("x2"));
            }

            return sb.ToString();
        }
    }
}
