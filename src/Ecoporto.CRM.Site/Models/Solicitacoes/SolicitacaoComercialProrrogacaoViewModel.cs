using Ecoporto.CRM.Business.DTO;
using Ecoporto.CRM.Business.Enums;
using Ecoporto.CRM.Business.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ecoporto.CRM.Site.Models
{
    public class SolicitacaoComercialProrrogacaoViewModel
    {
        public SolicitacaoComercialProrrogacaoViewModel()
        {
            Prorrogacoes = new List<SolicitacaoProrrogacaoDTO>();
            ProrrogacaoNotasFiscais = new List<NotaFiscal>();
            Contas = new List<Conta>();
        }

        public int ProrrogacaoId { get; set; }

        public int ProrrogacaoSolicitacaoId { get; set; }

        [Display(Name = "NFE")]
        public int ProrrogacaoNotaFiscal { get; set; }

        [Display(Name = "Nota Fiscal")]
        public int ProrrogacaoNotaFiscalId { get; set; }

        [Display(Name = "Valor NF")]
        public decimal ProrrogacaoValorNF { get; set; }

        [Display(Name = "Razão Social")]
        public string ProrrogacaoRazaoSocial { get; set; }
        
        [Display(Name = "Vencimento Original")]
        public DateTime? ProrrogacaoVencimentoOriginal { get; set; }

        [Display(Name = "Data Prorrogação")]
        public DateTime? ProrrogacaoDataProrrogacao { get; set; }

        [Display(Name = "Nº Prorrogação")]
        public int ProrrogacaoNumeroProrrogacao { get; set; }

        [Display(Name = "Isentar Juros")]
        public Boleano ProrrogacaoIsentarJuros { get; set; }

        public int? ProrrogacaoTipoOperacaoSolicitacao { get; set; }

        [Display(Name = "Valor Juros")]
        public decimal ProrrogacaoValorJuros { get; set; }

        [Display(Name = "Valor Total c/ Juros")]
        public decimal ProrrogacaoValorTotalComJuros { get; set; }

        [Display(Name = "Observações")]
        public string ProrrogacaoObservacoes { get; set; }

        [Display(Name = "Cliente")]
        public int? ProrrogacaoContaId { get; set; }

        public int ProrrogacaoUnidadeSolicitacao { get; set; }

        public StatusSolicitacao ProrrogacaoStatusSolicitacao { get; set; }

        public List<SolicitacaoProrrogacaoDTO> Prorrogacoes { get; set; }

        public List<NotaFiscal> ProrrogacaoNotasFiscais { get; set; }

        public List<Conta> Contas { get; set; }
    }
}