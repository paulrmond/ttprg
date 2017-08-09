using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using BCS.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using BCS.Models.Cryptography;

namespace BCS.Models
{
    // This is useful if you do not want to tear down the database each time you run the application.
    // public class ApplicationDbInitializer : DropCreateDatabaseAlways<ApplicationDbContext>
    // This example shows you how to create a new database if the Model changes
    public class ApplicationDbInitializer : DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            //DoYourSeedingHere(context);
            //base.Seed(context);
            var RoleManager1 = new RoleManager<ApplicationRole>(new RoleStore<ApplicationRole>(context));

            ApplicationRole approle1 = new ApplicationRole();
            approle1.Name = "System Administrator";
            approle1.RoleNumber = 1;
            var roleresult = RoleManager1.Create(approle1);

            ApplicationRole approle2 = new ApplicationRole();
            approle2.Name = "Super User";
            approle2.RoleNumber = 2;
            var roleresult2 = RoleManager1.Create(approle2);

            ApplicationRole approle3 = new ApplicationRole();
            approle3.Name = "Zone Head";
            approle3.RoleNumber = 3;
            var roleresult3 = RoleManager1.Create(approle3);

            ApplicationRole approle4 = new ApplicationRole();
            approle4.Name = "Department Head";
            approle4.RoleNumber = 4;
            var roleresult4 = RoleManager1.Create(approle4);

            ApplicationRole approle5 = new ApplicationRole();
            approle5.Name = "Finance Officer";
            approle5.RoleNumber = 5;
            var roleresult5 = RoleManager1.Create(approle5);

            ApplicationRole approle6 = new ApplicationRole();
            approle6.Name = "Billing Officer";
            approle6.RoleNumber = 6;
            var roleresult6 = RoleManager1.Create(approle6);

            ApplicationRole approle7 = new ApplicationRole();
            approle7.Name = "Cashier";
            approle7.RoleNumber = 7;
            var roleresult7 = RoleManager1.Create(approle7);

            ApplicationRole approle8 = new ApplicationRole();
            approle8.Name = "Meter Reader";
            approle8.RoleNumber = 8;
            var roleresult8 = RoleManager1.Create(approle8);


            // Out of the box approach
            // ctx.Users.AddOrUpdate(
            //     new ApplicationUser { Email = "foo@xyz.com", UserName = "foo@xyz.com" }
            //     );

            // Another approach
            Encrypt encrypt = new Encrypt("abcDEF123");
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            var userHO = new ApplicationUser() { LockoutEnabled = true, Status="Active", UserName = "HO", Email = "headoffice@abc.com", Division = "PEP", GivenName = "admin", LastName = "admin", MiddleName = "", PhoneNumber = "1234567", ZoneGroup = "01", LocalPass = encrypt.EncryptedValue };
            var passwordHO = "abcDEF123@";

            var userCEZ = new ApplicationUser() { LockoutEnabled = true, Status = "Active", UserName = "CEZ", Email = "caviteecozone@abc.com", Division = "PEP", GivenName = "admin", LastName = "admin", MiddleName = "", PhoneNumber = "1234567", ZoneGroup = "03", LocalPass = encrypt.EncryptedValue };
            var passwordCEZ = "abcDEF123@";

            var userBCEZ = new ApplicationUser() { LockoutEnabled = true, Status = "Active", UserName = "BCEZ", Email = "baguioecozone@abc.com", Division = "PEP", GivenName = "admin", LastName = "admin", MiddleName = "", PhoneNumber = "1234567", ZoneGroup = "06", LocalPass = encrypt.EncryptedValue };
            var password2BCEZ = "abcDEF123@";

            var userMEZ = new ApplicationUser() { LockoutEnabled = true, Status = "Active", UserName = "MEZ", Email = "mectanecozone@abc.com", Division = "PEP", GivenName = "admin", LastName = "admin", MiddleName = "", PhoneNumber = "1234567", ZoneGroup = "09", LocalPass = encrypt.EncryptedValue };
            var passwordMEZ = "abcDEF123@";

            var userSU = new ApplicationUser() { LockoutEnabled = false, Status = "Active", UserName = "SU", Email = "superuser@abc.com", Division = "PEP", GivenName = "admin", LastName = "admin", MiddleName = "", PhoneNumber = "1234567", ZoneGroup = "99", LocalPass = encrypt.EncryptedValue };
            var passwordSU = "abcDEF123@";

            var HOResult = UserManager.Create(userHO, passwordHO);
            var CEZResult = UserManager.Create(userCEZ, passwordCEZ);
            var BCEZResult = UserManager.Create(userBCEZ, password2BCEZ);
            var MEZResult = UserManager.Create(userMEZ, passwordMEZ);
            var SUResult = UserManager.Create(userSU, passwordSU);

            //Add User Admin to Role Administrator
            if (HOResult.Succeeded && CEZResult.Succeeded && BCEZResult.Succeeded && MEZResult.Succeeded && SUResult.Succeeded)
            {
                var resultHO = UserManager.AddToRole(userHO.Id, "System Administrator");
                var resultCEZ = UserManager.AddToRole(userCEZ.Id, "System Administrator");
                var resultBCEZ = UserManager.AddToRole(userBCEZ.Id, "System Administrator");
                var resultMEZ = UserManager.AddToRole(userMEZ.Id, "System Administrator");
                var resultSU = UserManager.AddToRole(userSU.Id, "Super User");
            }

            context.SaveChanges();
        }

    }
}