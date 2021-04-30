using System;

namespace Ecoporto.CRM.Business.DTO
{
    public class AuditoriaAcessosDTO
    {
        public int Id { get; set; }

        public string Login { get; set; }

        public DateTime DataHora { get; set; }

        public string IP { get; set; }

        public string Mensagem { get; set; }

        public bool Sucesso { get; set; }

        public bool Externo { get; set; }

        public int TotalLinhas { get; set; }
    }
}
