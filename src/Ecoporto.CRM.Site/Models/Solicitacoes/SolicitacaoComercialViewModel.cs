using Ecoporto.CRM.Business.Enums;
using Ecoporto.CRM.Business.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Ecoporto.CRM.Site.Models
{
    public class SolicitacaoComercialViewModel
    {
        public SolicitacaoComercialViewModel()
        {
            SolicitacaoComercialMotivosViewModel = new SolicitacaoComercialMotivosViewModel();
            SolicitacaoComercialOcorrenciasViewModel = new SolicitacaoComercialOcorrenciasViewModel();
            SolicitacaoComercialCancelamentoNFViewModel = new SolicitacaoComercialCancelamentoNFViewModel();
            SolicitacaoComercialProrrogacaoViewModel = new SolicitacaoComercialProrrogacaoViewModel();
            SolicitacaoComercialRestituicaoViewModel = new SolicitacaoComercialRestituicaoViewModel();
            SolicitacaoComercialDescontoViewModel = new SolicitacaoComercialDescontoViewModel();
            SolicitacaoComercialAnexosViewModel = new SolicitacaoComercialAnexosViewModel();

            ValorDevido = HabilitaValorDevido ? ValorDevido : SolicitacaoComercialRestituicaoViewModel
                .Restituicoes.Sum(c => c.ValorNF).ToString("n2");
        }

        public int Id { get; set; }

        [Display(Name = "Tipo Solicitação")]
        public TipoSolicitacao TipoSolicitacao { get; set; }

        [Display(Name = "Status")]
        public StatusSolicitacao StatusSolicitacao { get; set; }

        //[Display(Name = "Unidade")]
        //public UnidadeSolicitacao UnidadeSolicitacao { get; set; }

        [Display(Name = "Unidade")]
        public int UnidadeSolicitacaoId { get; set; }

        [Display(Name = "Área que gerou a Ocorrência")]
        public AreaOcorrenciaSolicitacao AreaOcorrenciaSolicitacao { get; set; }

        //[Display(Name = "Tipo Operação")]
        //public TipoOperacaoSolicitacao TipoOperacao { get; set; }

        [Display(Name = "Tipo Operação")]
        public int? TipoOperacaoId { get; set; }

        [Display(Name = "Ocorrência / Ação")]
        public int? OcorrenciaId { get; set; }

        [Display(Name = "Motivo")]
        public int? MotivoId { get; set; }

        public string ValorDevido { get; set; }

        public string ValorCobrado { get; set; }

        public string ValorCredito { get; set; }

        public string Justificativa { get; set; }

        public bool HabilitaValorDevido { get; set; }

        public IEnumerable<SolicitacaoUnidade> UnidadesSolicitacao { get; set; }

        public IEnumerable<SolicitacaoTipoOperacao> TiposOperacaoSolicitacao { get; set; }

        public SolicitacaoComercialMotivosViewModel SolicitacaoComercialMotivosViewModel { get; set; }

        public SolicitacaoComercialOcorrenciasViewModel SolicitacaoComercialOcorrenciasViewModel { get; set; }

        public SolicitacaoComercialCancelamentoNFViewModel SolicitacaoComercialCancelamentoNFViewModel { get; set; }

        public SolicitacaoComercialProrrogacaoViewModel SolicitacaoComercialProrrogacaoViewModel { get; set; }

        public SolicitacaoComercialRestituicaoViewModel SolicitacaoComercialRestituicaoViewModel { get; set; }

        public SolicitacaoComercialDescontoViewModel SolicitacaoComercialDescontoViewModel { get; set; }

        public SolicitacaoComercialAnexosViewModel SolicitacaoComercialAnexosViewModel { get; set; }

        public SolicitacaoComercialAlteracaoFormaPagamentoViewModel SolicitacaoComercialAlteracaoFormaPagamentoViewModel { get; set; }
    }
}