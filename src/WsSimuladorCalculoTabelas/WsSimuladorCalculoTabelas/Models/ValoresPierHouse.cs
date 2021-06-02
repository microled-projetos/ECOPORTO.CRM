using System;

namespace WsSimuladorCalculoTabelas.Models
{
    public class ValoresPierHouse
    {
        public string TabelaId { get; set; }

        public string Descricao { get; set; }

        public string Regime { get; set; }

        public string DataInicial { get; set; }

        public string DataFinal { get; set; }

        public string Status { get; set; }

        public string SeqGr { get; set; }

        public string BL { get; set; }

        public string Lotes { get; set; }

        public string DataPagamento { get; set; }

        public decimal Faturado { get; set; }
    }
}