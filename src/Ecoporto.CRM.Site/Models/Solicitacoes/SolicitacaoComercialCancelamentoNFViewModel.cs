using Ecoporto.CRM.Business.DTO;
using Ecoporto.CRM.Business.Enums;
using Ecoporto.CRM.Business.Extensions;
using Ecoporto.CRM.Business.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ecoporto.CRM.Site.Models
{
    public class SolicitacaoComercialCancelamentoNFViewModel
    {
        public SolicitacaoComercialCancelamentoNFViewModel()
        {
            CancelamentoNFNotasFiscais = new List<NotaFiscal>();
            CancelamentosNF = new List<SolicitacaoCancelamentoNFDTO>();
            Contas = new List<Conta>();
        }

        public int CancelamentoNFId { get; set; }

        public int CancelamentoNFSolicitacaoId { get; set; }

        [Display(Name = "Tipo Pesquisa")]
        public TipoPesquisa CancelamentoNFTipoPesquisa { get; set; }

        [Display(Name = "Número")]
        public int CancelamentoNFTipoPesquisaNumero { get; set; }

        [Display(Name = "Lote")]
        public int CancelamentoNFLote { get; set; }

        [Display(Name = "Reserva")]
        public int CancelamentoNFReserva { get; set; }

        [Display(Name = "Nota Fiscal")]
        public int CancelamentoNFNotaFiscalId { get; set; }

        [Display(Name = "Número NF")]
        public int CancelamentoNFNotaFiscal { get; set; }

        [Display(Name = "Valor NF")]
        public decimal CancelamentoNFValorNF { get; set; }

        [Display(Name = "Valor à Pagar")]
        public decimal CancelamentoValorAPagar { get; set; }

        [Display(Name = "Novo Valor à Pagar")]
        public decimal CancelamentoNovoValorAPagar { get; set; }

        [Display(Name = "Razão Social")]
        public int? CancelamentoNFContaId { get; set; }

        [Display(Name = "Razão Social")]
        public string CancelamentoNFRazaoSocial { get; set; }

        [Display(Name = "Forma Pagamento")]
        public FormaPagamento CancelamentoNFFormaPagamento { get; set; }

        [Display(Name = "Forma Pagamento")]
        public string DescricaoCancelamentoNFFormaPagamento => CancelamentoNFFormaPagamento.ToName();

        [Display(Name = "Data Emissão")]
        public DateTime? CancelamentoNFDataEmissao { get; set; }

        [Display(Name = "Desconto R$")]
        public decimal CancelamentoNFDesconto { get; set; }

        [Display(Name = "Valor Nova NF")]
        public decimal CancelamentoNFValorNovaNF { get; set; }

        [Display(Name = "Data Prorrogação")]
        public DateTime? CancelamentoNFDataProrrogacao { get; set; }

        public int CancelamentoNFUnidadeSolicitacao { get; set; }

        public int? CancelamentoTipoOperacaoSolicitacao { get; set; }

        public StatusSolicitacao CancelamentoStatusSolicitacao { get; set; }

        public List<NotaFiscal> CancelamentoNFNotasFiscais { get; set; }

        public List<SolicitacaoCancelamentoNFDTO> CancelamentosNF { get; set; }

        public List<Conta> Contas { get; set; }
    }
}