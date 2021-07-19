namespace WsSimuladorCalculoTabelas.Models
{
    public class ServicoIPA
    {
        public int Id { get; set; }

        public string Descricao { get; set; }

        public decimal PrecoUnitario { get; set; }

        public decimal PrecoMinimo { get; set; }

        public string BaseCalculo { get; set; }
    }
}