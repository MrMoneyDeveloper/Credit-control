namespace eBaseApp.Migrations
{
    using eBaseApp.DataAccessLayer;
    using eBaseApp.Models;
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<eServicesDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(eServicesDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.

            var idManager = new IdentityManager(context);

            // JK.20140916a - Standard user roles for the system.
            if (!idManager.RoleExists("Super Administrators"))
                idManager.CreateRole("Super Administrators");

            if (!idManager.RoleExists("Administrators"))
                idManager.CreateRole("Administrators");

            //if (!idManager.RoleExists("Agents"))
            //    idManager.CreateRole("Agents");

            if (!idManager.RoleExists("Customers"))
                idManager.CreateRole("Customers");

            if (!idManager.RoleExists("Guests"))
                idManager.CreateRole("Guests");

            //if (!idManager.RoleExists("Financial Clerk"))
            //    idManager.CreateRole("Financial Clerk");

            var superAdmin = new SystemIdentityUser()
            {
                UserName = "SuperAdmin",
                Email = "siyanda.ngxonga@xetgroup.com",
                SystemUser = new SystemUser()
                {
                    FirstName = "Super",
                    LastName = "Admin",
                    UserName = "SuperAdmin",
                    EmailAddress = "siyanda.ngxonga@xetgroup.com"
                }
            };

            if (!idManager.UserExists(superAdmin.UserName))
            {
                idManager.CreateUser(superAdmin, "password");
                idManager.AddUserToRole(superAdmin.Id, "Super Administrators");
            }

            var customer = new SystemIdentityUser()
            {
                UserName = "JohnD",
                Email = "ngxongosiyanda@gmail.com",
                SystemUser = new SystemUser()
                {
                    FirstName = "John",
                    LastName = "Doe",
                    UserName = "JohnD",
                    EmailAddress = "ngxongosiyanda@gmail.com"
                }
            };

            if (!idManager.UserExists(customer.UserName))
            {
                idManager.CreateUser(customer, "password");
                idManager.AddUserToRole(customer.Id, "Customers");
            }

            context.SaveChanges();
        }
    }
}
