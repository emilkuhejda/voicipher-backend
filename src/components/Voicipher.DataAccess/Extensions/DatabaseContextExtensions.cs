using System;
using System.Linq;
using Voicipher.Common.Helpers;
using Voicipher.Domain.Models;

namespace Voicipher.DataAccess.Extensions
{
    public static class DatabaseContextExtensions
    {
        public static void SeedDatabase(this DatabaseContext context)
        {
            if (context.Administrators.Any())
                return;

            var password = PasswordHelper.CreateHash("VoicipherPass12!");
            var administrator = new Administrator
            {
                Id = Guid.NewGuid(),
                Username = "emil.kuhejda@gmail.com",
                FirstName = "Emil",
                LastName = "Kuhejda",
                PasswordHash = password.PasswordHash,
                PasswordSalt = password.PasswordSalt
            };

            context.Administrators.Add(administrator);
            context.SaveChanges();
        }
    }
}
