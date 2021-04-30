using System;

namespace Ecoporto.CRM.Business.DTO
{
    public class AnexosNotasDTO
    {
        public int Id { get; set; }

        public string OportunidadeId { get; set; }

        public string Nota { get; set; }

        public string Descricao { get; set; }

        public string CriadoPor { get; set; }

        public DateTime DataCriacao { get; set; }
    }
}
