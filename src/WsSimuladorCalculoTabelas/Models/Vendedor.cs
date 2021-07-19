using System;

namespace WsSimuladorCalculoTabelas.Models
{
    public class Vendedor
    {
        public int Id { get; set; }

        public int TabelaId { get; set; }

        public string Nome { get; set; }

        public string CPF { get; set; }

        public DateTime InicioVigencia { get; set; }

        public DateTime? TerminoVigencia { get; set; }
    }
}