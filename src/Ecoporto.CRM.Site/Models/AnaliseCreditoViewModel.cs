using Ecoporto.CRM.Business.DTO;
using Ecoporto.CRM.Business.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ecoporto.CRM.Site.Models
{
    public class AnaliseCreditoViewModel
    {
        public int ContaPesquisaId { get; set; }

        public string ContaPesquisaDescricao { get; set; }

        public bool Reprocessar { get; set; }

        [Display(Name = "Status Aprovação")]
        public string StatusAprovacao { get; set; }

        [Display(Name = "Total Dívida")]
        public string EcoportoTotalDivida { get; set; }

        [Display(Name = "Indadimplente")]
        public string EcoportoInadimplente { get; set; }

        [Display(Name = "Condição Pagamento")]
        public string CondicaoPagamentoId { get; set; }

        [Display(Name = "Condição Pagamento")]
        public string CondicaoPagamentoDescricao { get; set; }

        [Display(Name = "Limite Crédito")]
        public decimal LimiteCredito { get; set; }

        [Display(Name = "Observação")]
        public string Observacao { get; set; }

        public WsSPC.ConsultaSpcResponse ResultadoSPC { get; set; }

        public IEnumerable<Conta> ContasPesquisa { get; set; }

        public IEnumerable<CondicaoPagamentoFatura> CondicoesPagamento { get; set; }

        public IEnumerable<PendenciaFinanceiraDTO> PendenciasFinanceiras { get; set; }

        public IEnumerable<LimiteCreditoSpcDTO> SolicitacoesLimiteCredito { get; set; }
    
    }
}