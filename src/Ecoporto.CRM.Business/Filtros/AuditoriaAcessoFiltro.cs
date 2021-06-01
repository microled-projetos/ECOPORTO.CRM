using System;

namespace Ecoporto.CRM.Business.Filtros
{
    public class AuditoriaAcessoFiltro
    {
        public string Login { get; set; }

        public string De { get; set; }

        public string Ate { get; set; }

        public string IP { get; set; }

        public int? Externo { get; set; }

        public int? Sucesso { get; set; }
    }
}
