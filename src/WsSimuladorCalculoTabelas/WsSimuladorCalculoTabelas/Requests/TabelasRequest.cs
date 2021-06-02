namespace WsSimuladorCalculoTabelas.Requests
{
    public class TabelasRequest
    {
        public int ClienteId { get; set; }

        public string ClienteCnpj { get; set; }

        public int ClasseCliente { get; set; }

        public int SimuladorId { get; set; }

        public bool CRM { get; set; }

        public bool CalculoAutomatico { get; set; }

        public int TabelaId { get; set; }

        public string[] Cnpjs => ClienteCnpj.Split(',');
    }
}