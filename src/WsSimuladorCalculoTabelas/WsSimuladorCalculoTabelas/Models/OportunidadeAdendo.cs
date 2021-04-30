using Ecoporto.CRM.Business.Enums;
using System;
using WsSimuladorCalculoTabelas.Enums;

namespace WsSimuladorCalculoTabelas.Models
{
    public class OportunidadeAdendo 
    {
        public int Id { get; set; }

        public int OportunidadeId { get; set; }

        public TipoAdendo TipoAdendo { get; set; }

        public StatusAdendo StatusAdendo { get; set; }

        public DateTime DataCadastro { get; set; }

        public int CriadoPor { get; set; }
    }
}