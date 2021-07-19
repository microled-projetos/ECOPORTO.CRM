namespace WsSimuladorCalculoTabelas.Models
{
    public class ServicoCalculo
    {
        public int Id { get; set; }

        public int ServicoCalculoId { get; set; }

        public decimal BaseCalculo { get; set; }

        public string DescricaoBaseCalculo { get; set; }

        public int Lista { get; set; }

        public decimal PrecoUnitario { get; set; }

        public decimal PrecoMinimo { get; set; }

        public decimal Quantidade { get; set; }

        public decimal Parcela { get; set; }

        public string TipoServico { get; set; }

        public decimal ValorMDir { get; set; }

        public decimal ValorMEsq { get; set; }

        public string Margem { get; set; }

        public string TipoCarga { get; set; }
    }
}