using Ecoporto.CRM.Business.Enums;

namespace Ecoporto.CRM.Business.Filtros
{
    public class SolicitacoesFiltro
    {
        public int? Id { get; set; }

        public TipoSolicitacao? TipoSolicitacao { get; set; }

        public int? UnidadeSolicitacao { get; set; }

        public string De { get; set; }

        public string Ate { get; set; }

        public int? OcorrenciaId { get; set; }

        public int? CriadoPor { get; set; }

        public StatusSolicitacao? StatusSolicitacao { get; set; }

        public int? Lote { get; set; }

        public int? GR { get; set; }

        public int? Minuta { get; set; }

        public int? NotaFiscal { get; set; }
    }
}
