using Ecoporto.CRM.Business.Enums;
using System;

namespace Ecoporto.CRM.Business.DTO
{
    public class SolicitacaoDescontoDTO
    {
        public int Id { get; set; }

        public int SolicitacaoId { get; set; }

        public TipoPesquisa TipoPesquisa { get; set; }

        public StatusSolicitacao StatusSolicitacao { get; set; }

        public int TipoOperacao { get; set; }

        public string TipoPesquisaNumero { get; set; }

        public int MinutaGRId { get; set; }

        public decimal ValorGR { get; set; }

        public int Lote { get; set; }

        public string Reserva { get; set; }

        public int GR { get; set; }

        public int Minuta { get; set; }

        public int ClienteId { get; set; }

        public string RazaoSocial { get; set; }

        public string ClienteDescricao { get; set; }

        public string ClienteDocumento { get; set; }

        public int IndicadorId { get; set; }

        public int ClienteFaturamentoId { get; set; }

        public string ClienteFaturamentoDescricao { get; set; }

        public string IndicadorDescricao { get; set; }

        public string IndicadorDocumento { get; set; }

        public string Proposta { get; set; }

        public DateTime? VencimentoGR { get; set; }

        public DateTime? FreeTimeGR { get; set; }

        public int Periodo { get; set; }

        public FormaPagamento FormaPagamento { get; set; }

        public TipoDesconto TipoDesconto { get; set; }

        public decimal Desconto { get; set; }

        public decimal DescontoNoServico { get; set; }

        public decimal DescontoFinal { get; set; }

        public bool Geral => !PorServico;

        public bool PorServico { get; set; }

        public int ServicoFaturadoId { get; set; }

        public int ServicoId { get; set; }

        public decimal ServicoValor { get; set; }

        public string ServicoDescricao { get; set; }

        public decimal DescontoComImposto { get; set; }

        public DateTime? Vencimento { get; set; }

        public DateTime? FreeTime { get; set; }

        public decimal ValorCIF { get; set; }

        public DateTime DataCadastro { get; set; }

        public string CriadoPor { get; set; }

        public int CriadoPorId { get; set; }
    }
}
