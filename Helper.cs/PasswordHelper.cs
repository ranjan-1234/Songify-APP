using System;
using System.Security.Cryptography;

namespace Singer.Helpers   // ✅ Namespace must match here
{
    public static class PasswordHelper
    {
        // ✅ Create a secure password hash
        public static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;  // ✅ Generate random salt
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password)); // ✅ Generate hash
            }
        }


        // ✅ Verify the entered password against stored hash
        public static bool VerifyPassword(string password, byte[] storedHash, byte[] storedSalt)
        {
            using (var hmac = new HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return CryptographicEquals(computedHash, storedHash);
            }
        }

        // ✅ Secure comparison to prevent timing attacks
        private static bool CryptographicEquals(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
                return false;

            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i])
                    return false;
            }
            return true;
        }
    }
}
