namespace Ecoporto.CRM.Business.Models
{
    public class SimuladorCargaSolta
    {
        public int Id { get; set; }

        public int SimuladorId { get; set; }

        public int Quantidade { get; set; }

        public decimal VolumeM3 { get; set; }

        public decimal Peso { get; set; }

        public int UsuarioId { get; set; }
    }
}
