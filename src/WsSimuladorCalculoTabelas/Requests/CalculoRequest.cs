namespace WsSimuladorCalculoTabelas.Requests
{
    public class CalculoRequest
    {
        public int SimuladorId { get; set; }

        public bool CRM { get; set; }

        public int[] Tabelas { get; set; }
    }
}