using System;
using System.Security.Cryptography;
using System.Text;

namespace Voicipher.Common.Helpers
{
    public static class PasswordHelper
    {
        public static (byte[] PasswordHash, byte[] PasswordSalt) CreateHash(string password)
        {
            if (password == null)
                throw new ArgumentNullException(nameof(password));

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Value cannot be empty or whitespace only string.", nameof(password));

            using (var hmac = new HMACSHA512())
            {
                return (hmac.ComputeHash(Encoding.UTF8.GetBytes(password)), hmac.Key);
            }
        }

        public static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null)
                return false;

            if (string.IsNullOrWhiteSpace(password))
                return false;

            if (storedHash.Length != 64)
                return false;

            if (storedSalt.Length != 128)
                return false;

            using (var hmac = new HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                for (var i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i])
                        return false;
                }
            }

            return true;
        }
    }
}
