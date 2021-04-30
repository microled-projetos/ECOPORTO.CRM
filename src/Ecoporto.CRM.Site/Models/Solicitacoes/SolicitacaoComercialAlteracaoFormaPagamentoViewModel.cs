using Ecoporto.CRM.Business.DTO;
using Ecoporto.CRM.Business.Enums;
using Ecoporto.CRM.Business.Extensions;
using Ecoporto.CRM.Business.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ecoporto.CRM.Site.Models
{
    public class SolicitacaoComercialAlteracaoFormaPagamentoViewModel
    {
        public SolicitacaoComercialAlteracaoFormaPagamentoViewModel()
        {
            GRs = new List<GR>();
            Contas = new List<Conta>();
            CondicoesPagamentoFaturamento = new List<CondicaoPagamentoFatura>();
            SolicitacoesAlteracaoFormaPgto = new List<SolicitacaoFormaPagamentoDTO>();
        }

        public int AlteracaoFormaPagamentoId { get; set; }

        public int AlteracaoFormaPagamentoSolicitacaoId { get; set; }

        [Display(Name = "Tipo Pesquisa")]
        public TipoPesquisa AlteracaoFormaPagamentoTipoPesquisa { get; set; }

        [Display(Name = "Número")]
        public int AlteracaoFormaPagamentoTipoPesquisaNumero { get; set; }

        [Display(Name = "GR")]
        public int AlteracaoFormaPagamentoGrId { get; set; }

        [Display(Name = "Lote")]
        public int AlteracaoFormaPagamentoLote { get; set; }
        
        [Display(Name = "Valor")]
        public decimal AlteracaoFormaPagamentoValor { get; set; }
        
        [Display(Name = "Cliente")]
        public string AlteracaoFormaPagamentoCliente { get; set; }

        [Display(Name = "Indicador")]
        public string AlteracaoFormaPagamentoIndicador { get; set; }
        
        [Display(Name = "Forma Pagamento")]
        public FormaPagamento AlteracaoFormaPagamento { get; set; }

        [Display(Name = "Condição Pagamento")]
        public string AlteracaoFormaPagamentoCondicaoPagamentoId { get; set; }

        [Display(Name = "Forma Pagamento")]
        public string DescricaoAlteracaoFormaPagamento => AlteracaoFormaPagamento.ToName();

        [Display(Name = "ID Tabela")]
        public string AlteracaoFormaPagamentoProposta { get; set; }

        [Display(Name = "Free Time")]
        public DateTime? AlteracaoFormaPagamentoFreeTimeGR { get; set; }

        [Display(Name = "Período")]
        public int AlteracaoFormaPagamentoPeriodo { get; set; }

        [Display(Name = "Faturado Contra")]
        public int AlteracaoFormaPagamentoFaturadoContraId { get; set; }

        [Display(Name = "Faturado Contra")]
        public int AlteracaoFormaPagamentoFaturadoDescricao { get; set; }

        [Display(Name = "Encaminhar Para")]
        public string AlteracaoFormaPagamentoEncaminharPara { get; set; }

        public int AlteracaoFormaPagamentoUnidadeSolicitacao { get; set; }

        public int? AlteracaoFormaPagamentoTipoOperacaoSolicitacao { get; set; }

        public List<GR> GRs { get; set; }

        public List<Conta> Contas { get; set; }

        public List<CondicaoPagamentoFatura> CondicoesPagamentoFaturamento { get; set; }

        public List<SolicitacaoFormaPagamentoDTO> SolicitacoesAlteracaoFormaPgto { get; set; }
    }
}