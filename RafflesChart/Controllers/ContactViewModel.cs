﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RafflesChart.Controllers
{
    public class ContactViewModel
    {
        public string From { get; set; }

        public string Subject { get; set; }

        public string Message { get; set; }

        public string Captcha { get; set; }
    }
}
