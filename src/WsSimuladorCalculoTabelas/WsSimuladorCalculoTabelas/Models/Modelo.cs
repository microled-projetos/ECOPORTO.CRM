namespace WsSimuladorCalculoTabelas.Models
{
    public class Modelo
    {
        public int Id { get; set; }

        public string Descricao { get; set; }

        public bool IntegraChronos { get; set; }

        public bool Simular { get; set; }

        public bool Escalonado { get; set; }
    }
}