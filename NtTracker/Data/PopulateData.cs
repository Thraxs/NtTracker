using System;
using System.Data.Entity;
using NtTracker.Models;

namespace NtTracker.Data
{
    public class PopulateData : CreateDatabaseIfNotExists<DataContext>
    {
        protected override void Seed(DataContext context)
        {
            // Admin
            var admin = new UserAccount()
            {
                IsAdmin = true,
                UserName = "admin",
                RegistrationDate = DateTime.Now.AddMinutes(-1),
                FailedLoginAttempts = 0
            };

            context.UserAccounts.Add(admin);
            context.SaveChanges();
        }
    }
}