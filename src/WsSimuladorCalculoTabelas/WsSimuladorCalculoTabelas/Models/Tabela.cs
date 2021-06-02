using System;

namespace WsSimuladorCalculoTabelas.Models
{
    public class Tabela
    {
        public int SimuladorId { get; set; }

        public int TabelaId { get; set; }

        public string Descricao { get; set; }

        public string Importador { get; set; }

        public string Despachante { get; set; }

        public string NVOCC { get; set; }

        public string Coloader { get; set; }

        public string CoColoader { get; set; }

        public string CoColoader2 { get; set; }

        public string Proposta { get; set; }

        public DateTime? DataInicio { get; set; }

        public DateTime? DataFinalValidade { get; set; }
    }
}