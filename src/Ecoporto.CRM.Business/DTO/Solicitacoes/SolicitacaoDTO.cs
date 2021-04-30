using Ecoporto.CRM.Business.Enums;

namespace Ecoporto.CRM.Business.DTO
{
    public class SolicitacaoDTO
    {
        public int Id { get; set; }

        public TipoSolicitacao TipoSolicitacao { get; set; }

        public StatusSolicitacao StatusSolicitacao { get; set; }

        public int UnidadeSolicitacao { get; set; }

        public string UnidadeSolicitacaoDescricao { get; set; }

        public AreaOcorrenciaSolicitacao AreaOcorrenciaSolicitacao { get; set; }

        public int TipoOperacao { get; set; }

        public string TipoOperacaoDescricao { get; set; }

        public string TipoOperacaoResumida { get; set; }

        public int OcorrenciaId { get; set; }

        public string Ocorrencia { get; set; }

        public string Justificativa { get; set; }

        public string Motivo { get; set; }

        public string Cliente { get; set; }

        public string Importador { get; set; }

        public decimal ValorDevido { get; set; }

        public decimal ValorCobrado { get; set; }

        public bool HabilitaValorDevido { get; set; }

        public decimal ValorCredito { get; set; }

        public string CriadoPor { get; set; }

        public int TotalLinhas { get; set; }
    }
}
