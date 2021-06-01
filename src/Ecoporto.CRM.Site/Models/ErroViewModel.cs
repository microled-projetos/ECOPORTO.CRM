using System;

namespace Ecoporto.CRM.Site.Models
{
    public class ErroViewModel
    {
        public DateTime Data { get; set; }

        public string Ticket { get; set; }

        public string Exception { get; set; }
    }
}