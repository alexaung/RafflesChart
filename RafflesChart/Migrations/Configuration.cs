namespace RafflesChart.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using RafflesChart.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<RafflesChart.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            ContextKey = "RafflesChart.Models.ApplicationDbContext";
        }

        protected override void Seed(RafflesChart.Models.ApplicationDbContext context)
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

            context.UserMarkets.AddOrUpdate(u => u.Code,
                new UserMarket { Code = "SGX" },
                new UserMarket { Code = "HKEX" },
                new UserMarket { Code = "AMEX" },
                new UserMarket { Code = "NYSE" },
                new UserMarket { Code = "NASD" });

            context.UserIndicators.AddOrUpdate(u => u.Code,
                new UserIndicator { Code = "MACD" },
                new UserIndicator { Code = "Stochastic" });

            context.UserBullBearTests.AddOrUpdate(u => u.Code,
                new UserBullBearTest { Code = "MACD UP" });

            context.UserBackTests.AddOrUpdate(u => u.Code,
                new UserBackTest { Code = "Stochstic UP" });

            context.PatternScanners.AddOrUpdate(u => u.Code,
                new PatternScanner { Code = "Triangle" },
                new PatternScanner { Code = "Wedge" });

            context.Scanners.AddOrUpdate(u => u.Code,
                new Scanner { Code = "Breakout" },
                new Scanner { Code = "MACD" });
        }
    }
}
