using Ecoporto.CRM.Business.DTO;
using Ecoporto.CRM.Business.Enums;
using Ecoporto.CRM.Business.Extensions;
using Ecoporto.CRM.Business.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ecoporto.CRM.Site.Models
{
    public class SolicitacaoComercialDescontoViewModel
    {
        public SolicitacaoComercialDescontoViewModel()
        {
            ServicosFaturamento = new List<ServicoFaturamento>();
            Contas = new List<Conta>();
            GRs = new List<GR>();
            Descontos = new List<SolicitacaoDescontoDTO>();
        }

        public int DescontoId { get; set; }

        public int DescontoSolicitacaoId { get; set; }

        [Display(Name = "Tipo Pesquisa")]
        public TipoPesquisa TipoPesquisaSolicitacaoDesconto { get; set; }

        [Display(Name = "Número")]
        public string DescontoTipoPesquisaNumero { get; set; }

        [Display(Name = "Lote")]
        public int DescontoLote { get; set; }

        [Display(Name = "Reserva")]
        public string DescontoReserva { get; set; }

        [Display(Name = "GR/Minuta")]
        public int DescontoGRMinutaId { get; set; }

        [Display(Name = "Valor")]
        public decimal DescontoValor { get; set; }

        [Display(Name = "Cliente")]
        public int DescontoClienteId { get; set; }

        public string ClienteFaturamentoId { get; set; }

        public string DescontoClienteDescricao { get; set; }

        [Display(Name = "Indicador")]
        public int DescontoIndicadorId { get; set; }

        public string DescontoIndicadorDescricao { get; set; }

        [Display(Name = "ID Tabela")]
        public string DescontoProposta { get; set; }

        [Display(Name = "Razão Social")]
        public string DescontoRazaoSocial { get; set; }

        [Display(Name = "Vencimento")]
        public DateTime? DescontoVencimentoGR { get; set; }

        [Display(Name = "Free Time")]
        public DateTime? DescontoFreeTimeGR { get; set; }

        [Display(Name = "Período")]
        public int DescontoPeriodo { get; set; }

        [Display(Name = "Forma Pagamento")]
        public FormaPagamento DescontoFormaPagamento { get; set; }

        public int? DescontoTipoOperacaoSolicitacao { get; set; }

        public StatusSolicitacao DescontoStatusSolicitacao { get; set; }

        public string DescontoDescricaoFormaPagamento => DescontoFormaPagamento.ToName();

        [Display(Name = "Tipo Desconto")]
        public TipoDesconto DescontoTipoDesconto { get; set; }

        [Display(Name = "Desconto")]
        public decimal DescontoValorDesconto { get; set; }

        [Display(Name = "Valor à Pagar")]
        public decimal DescontoValorAPagar { get; set; }

        [Display(Name = "Serviço Líquido")]
        public decimal DescontoValorDescontoNoServico { get; set; }

        [Display(Name = "Valor Final")]
        public decimal DescontoValorDescontoFinal { get; set; }

        [Display(Name = "Desconto c/ Imposto")]
        public decimal DescontoValorDescontoComImposto { get; set; }

        public bool DescontoTipoDescontoPorServico { get; set; }

        [Display(Name = "Serviço")]
        public int DescontoServicoId { get; set; }

        public int ServicoId { get; set; }

        public decimal ServicoValor { get; set; }

        [Display(Name = "Vencimento")]
        public DateTime? DescontoVencimento { get; set; }

        [Display(Name = "Free Time")]
        public DateTime? DescontoFreeTime { get; set; }

        public int DescontoUnidadeSolicitacao { get; set; }

        public List<ServicoFaturamento> ServicosFaturamento { get; set; }

        public List<Conta> Contas { get; set; }

        public List<GR> GRs { get; set; }

        public List<SolicitacaoDescontoDTO> Descontos { get; set; }
    }
}