using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RafflesChart.Models
{
    public class Blog
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public Byte[] Content { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }
        public string Page { get; set; }
    }
}