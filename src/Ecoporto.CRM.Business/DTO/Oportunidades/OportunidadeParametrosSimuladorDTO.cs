namespace Ecoporto.CRM.Business.DTO
{
    public class OportunidadeParametrosSimuladorDTO
    {
        public int Id { get; set; }

        public int OportunidadeId { get; set; }

        public int ModeloId { get; set; }

        public string ModeloDescricao { get; set; }

        public string Regime { get; set; }

        public int DocumentoId { get; set; }

        public string TipoDocumento { get; set; }

        public string Armador { get; set; }

        public string Margem { get; set; }

        public int Periodos { get; set; }

        public decimal VolumeM3 { get; set; }

        public decimal Peso { get; set; }

        public decimal CifConteiner { get; set; }

        public decimal CifCargaSolta { get; set; }

        public decimal CifOportunidade { get; set; }

        public int GrupoAtracacaoId { get; set; }

        public string GrupoAtracacao { get; set; }

        public int NumeroLotes { get; set; }

        public int Qtde20 { get; set; }

        public int Qtde40 { get; set; }

        public string Observacoes { get; set; }
    }
}
