using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
namespace RafflesChart.ViewModels
{
    public class NoteViewModel
    {
        [AllowHtml]
        public string NoteData { get; set; }
    }
}
