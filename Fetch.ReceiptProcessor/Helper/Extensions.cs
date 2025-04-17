using System.Security.Cryptography;
using System.Text;

namespace Fetch.ReceiptProcessor.Helper;

public class Extensions
{
    public static string ComputeHash(string input)
    {
        using (var sha = SHA256.Create())
        {
            var bytes= Encoding.UTF8.GetBytes(input);
            var hashBytes = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hashBytes);
        }
    }
}