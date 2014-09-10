using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RafflesChart.Controllers
{
    class SchemeUserViewModel
    {
        public IQueryable<string> UserMarkets { get; set; }

        public IQueryable<string> UserIndicators { get; set; }

        public IQueryable<string> UserBullBearTests { get; set; }

        public IQueryable<string> UserBackTests { get; set; }

        public IQueryable<string> UserPatternScanners { get; set; }

        public IQueryable<string> UserScanners { get; set; }
    }
}
