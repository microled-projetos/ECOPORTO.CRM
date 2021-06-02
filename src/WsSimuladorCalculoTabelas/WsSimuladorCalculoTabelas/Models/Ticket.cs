namespace WsSimuladorCalculoTabelas.Models
{
    public class Ticket
    {
        public int OportunidadeId { get; set; }
        public int TabelaId { get; set; }
        public decimal LCL { get; set; }
        public decimal FCL40 { get; set; }
        public decimal FCL20MD { get; set; }
        public decimal FCL20ME { get; set; }
        public decimal FCL40MD { get; set; }
    }
}