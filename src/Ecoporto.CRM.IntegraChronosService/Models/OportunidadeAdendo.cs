using System;

namespace Ecoporto.CRM.IntegraChronosService
{
    public class OportunidadeAdendo 
    {
        public int Id { get; set; }

        public int OportunidadeId { get; set; }

        public DateTime DataCadastro { get; set; }

        public int CriadoPor { get; set; }
    }
}