namespace WsSimuladorCalculoTabelas.Models
{
    public class SimuladorTicketMedio
    {
        public int SimuladorId { get; set; }

        public decimal ValorTicketMedio { get; set; }

        public decimal ValorCif { get; set; }

        public string Regime { get; set; }

        public int TipoDocumento { get; set; }

        public int LocalAtracacao { get; set; }

        public int TabelaId { get; set; }
    }
}