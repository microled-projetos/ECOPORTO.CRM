namespace WsSimuladorCalculoTabelas.Models
{
    public class FaixaCIF
    {
        public int Id { get; set; }

        public int ServicoVariavelId { get; set; }

        public decimal ValorInicial { get; set; }

        public decimal ValorFinal { get; set; }

        public string TipoCarga { get; set; }

        public int Linha { get; set; }
    }
}