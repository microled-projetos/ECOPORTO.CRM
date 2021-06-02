namespace WsSimuladorCalculoTabelas.Models
{
    public class ValoresGerais
    {
        public string TabelaId { get; set; }

        public string Descricao { get; set; }

        public string Importador { get; set; }

        public string Despachante { get; set; }

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