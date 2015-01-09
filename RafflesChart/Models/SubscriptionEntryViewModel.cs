using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RafflesChart.ViewModels
{
    public class SubscriptionEntryViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public string Detail { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }

        public string Status { get; set; }

        public string Description { get; set; }
    }
}