using System.Security.Cryptography;
using System.Text;

namespace Guestbook.Hashing
{
    public static class Hashing
    {
        public static string getHash(string text)
        {  
            using (var sha256 = SHA256.Create())
            {  
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(text));
                // Return the hashed string.  
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }
    }
}
