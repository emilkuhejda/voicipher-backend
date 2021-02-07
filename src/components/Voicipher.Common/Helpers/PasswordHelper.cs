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
    }
}
