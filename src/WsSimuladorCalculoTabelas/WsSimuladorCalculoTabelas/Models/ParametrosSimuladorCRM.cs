using WsSimuladorCalculoTabelas.Enums;

namespace WsSimuladorCalculoTabelas.Models
{
    public class ParametrosSimuladorCRM
    {
        public int Id { get; set; }

        public int ModeloSimuladorId { get; set; }

        public Regime Regime { get; set; }

        public int DocumentoId { get; set; }

        public string Margem { get; set; }

        public int Periodos { get; set; }

        public decimal VolumeM3 { get; set; }

        public decimal Peso { get; set; }

        public decimal CifConteiner { get; set; }

        public decimal CifCargaSolta { get; set; }

        public decimal ValorCif { get; set; }

        public int GrupoAtracacaoId { get; set; }

        public int Qtde20 { get; set; }

        public int Qtde40 { get; set; }

        public int NumeroLotes { get; set; }

        public string Observacoes { get; set; }
    }
}