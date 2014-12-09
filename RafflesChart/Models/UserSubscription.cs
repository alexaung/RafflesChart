using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RafflesChart.Models
{
    public class UserSubscription
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public int SubscriptionId { get; set; }

        public string ItemName { get; set; }

        public int Month { get; set; }

        public decimal Price { get; set; }

        public string PaypalRef { get; set; }

        public DateTime CreatedDate { get; set; }

    }
}