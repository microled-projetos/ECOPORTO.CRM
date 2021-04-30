using Ecoporto.CRM.Business.Enums;

namespace Ecoporto.CRM.Business.DTO
{
    public class AnexosDTO
    {
        public int Id { get; set; }

        public int IdProcesso { get; set; }

        public string Anexo { get; set; }

        public string DataCadastro { get; set; }

        public string CriadoPor { get; set; }

        public TipoAnexo TipoAnexo { get; set; }

        public string TipoAnexoDescricao { get; set; }

        public string Versao { get; set; }

        public string IdFile { get; set; }
    }
}
