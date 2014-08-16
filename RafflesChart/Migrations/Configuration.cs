namespace RafflesChart.Migrations
{
    using System;
    using System.Configuration;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Web.Security;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using RafflesChart.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            ContextKey = "RafflesChart.Models.ApplicationDbContext";
        }

        protected override void Seed(ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //            

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            if (!roleManager.RoleExists("Admin")) {
                var role = new IdentityRole();
                role.Name = "Admin";
                roleManager.Create(role);
            }

            var superAdminEmail = ConfigurationManager.AppSettings["SuperAdminEmail"] ?? "superadmin@gmail.com";

            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            var superAdmin = userManager.FindByEmail(superAdminEmail);
            if (superAdmin == null) {
                var user = new ApplicationUser() {
                    Name = superAdminEmail,
                    PhoneNumber = "123456",
                    PhoneNumberConfirmed = true,
                    UserName = superAdminEmail,
                    Email = superAdminEmail,
                    EmailConfirmed = true
                };                
                userManager.Create(user, "Super123!");
                userManager.AddToRole(user.Id, "Admin");
            }
        }
    }
}
