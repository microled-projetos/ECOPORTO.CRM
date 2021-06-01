using Ecoporto.CRM.Business.Enums;

namespace Ecoporto.CRM.Business.DTO
{
    public class ContaDTO
    {
        public int Id { get; set; }

        public string CriadoPor { get; set; }

        public string Descricao { get; set; }

        public string Documento { get; set; }

        public string NomeFantasia { get; set; }

        public string Vendedor { get; set; }

        public SituacaoCadastral SituacaoCadastral { get; set; }

        public Segmento Segmento { get; set; }

        public ClassificacaoFiscal ClassificacaoFiscal { get; set; }

        public int TotalLinhas { get; set; }
    }
}
