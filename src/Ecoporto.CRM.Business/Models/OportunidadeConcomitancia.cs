using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecoporto.CRM.Business.Models
{
    public class OportunidadeConcomitancia
    {
        public bool Concomitante { get; set; }

        public string Mensagem { get; set; }

        public bool Bloqueia { get; set; }

        public string RedirectUrl { get; set; }
    }
}