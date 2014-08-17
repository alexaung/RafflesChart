using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;

namespace RafflesChart.Models {
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser> {

        public DbSet<Event> Events { get; set; }
                
        public DbSet<Scheme> Schemes { get; set; }

        public DbSet<UserBackTest> UserBackTests { get; set; }

        public DbSet<UserBullBearTest> UserBullBearTests { get; set; }

        public DbSet<UserIndicator> UserIndicators { get; set; }
        
        public DbSet<UserMarket> UserMarkets { get; set; }
        
        public DbSet<UserPatternScanner> UserPatternScanners { get; set; }
        
        public DbSet<UserScanner> UserScanners { get; set; }
        
        public ApplicationDbContext()
            : base("LocalConnection", throwIfV1Schema: false)
        {        
        }

        public static ApplicationDbContext Create() {
            return new ApplicationDbContext();
        }
    }
}