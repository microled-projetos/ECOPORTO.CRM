namespace Ecoporto.CRM.Business.Models
{
    public class SimuladorCargaConteiner
    {
        public int Id { get; set; }

        public int SimuladorId { get; set; }

        public int Tamanho { get; set; }

        public decimal Peso { get; set; }

        public int Quantidade { get; set; }

        public int NumeroLotes { get; set; }

        public int UsuarioId { get; set; }
    }
}
