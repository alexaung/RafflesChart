using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;

namespace RafflesChart.Models {
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser> {

        public DbSet<Event> Events { get; set; }

        public DbSet<UserMarket> UserMarkets { get; set; }

        public DbSet<UserIndicator> UserIndicators { get; set; }

        public DbSet<UserBullBearTest> UserBullBearTests { get; set; }

        public DbSet<UserBackTest> UserBackTests { get; set; }

        public DbSet<PatternScanner> PatternScanners { get; set; }

        public DbSet<Scanner> Scanners { get; set; }

        public DbSet<Scheme> Scheme { get; set; }

        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false) {
        }

        public static ApplicationDbContext Create() {
            return new ApplicationDbContext();
        }
    }
}