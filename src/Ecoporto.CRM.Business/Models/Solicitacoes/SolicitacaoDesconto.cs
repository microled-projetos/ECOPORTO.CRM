using Ecoporto.CRM.Business.Enums;
using FluentValidation;
using System;

namespace Ecoporto.CRM.Business.Models
{
    public class SolicitacaoDesconto : Entidade<SolicitacaoDesconto>
    {
        public SolicitacaoDesconto()
        {

        }

        public SolicitacaoDesconto(            
            int solicitacaoId,
            TipoPesquisa tipoPesquisa,
            string tipoPesquisaNumero,
            decimal valorGR, 
            int clienteId, 
            string razaoSocial,
            int indicadorId, 
            string proposta, 
            DateTime? vencimentoGR, 
            DateTime? freeTimeGR, 
            int periodo, 
            int lote,
            string reserva,
            FormaPagamento formaPagamento, 
            TipoDesconto tipoDesconto, 
            decimal valorDesconto, 
            decimal valorDescontoNoServico, 
            decimal valorDescontoFinal, 
            bool tipoDescontoPorServico, 
            int servicoFaturadoId,
            int servicoId,
            decimal servicoValor,
            decimal descontoComImposto,
            DateTime? vencimento, 
            DateTime? freeTime,
            int criadoPor)
        {            
            SolicitacaoId = solicitacaoId;
            TipoPesquisa = tipoPesquisa;
            TipoPesquisaNumero = tipoPesquisaNumero;
            ValorGR = valorGR;
            ClienteId = clienteId;
            RazaoSocial = razaoSocial;
            IndicadorId = indicadorId;
            Proposta = proposta;
            VencimentoGR = vencimentoGR;
            FreeTimeGR = freeTimeGR;
            Periodo = periodo;
            Lote = lote;
            Reserva = reserva;
            FormaPagamento = formaPagamento;
            TipoDesconto = tipoDesconto;
            ValorDesconto = valorDesconto;
            ValorDescontoNoServico = valorDescontoNoServico;
            ValorDescontoFinal = valorDescontoFinal;
            TipoDescontoPorServico = tipoDescontoPorServico;
            ServicoFaturadoId = servicoFaturadoId;
            ServicoId = servicoId;
            ServicoValor = servicoValor;
            DescontoComImposto = descontoComImposto;
            Vencimento = vencimento;
            FreeTime = freeTime;
            CriadoPor = criadoPor;
        }

        public int SolicitacaoId { get; set; }

        public TipoPesquisa TipoPesquisa { get; set; }

        public string TipoPesquisaNumero { get; set; }

        public int? SeqGR { get; set; }

        public decimal ValorGR { get; set; }

        public int ClienteId { get; set; }

        public string RazaoSocial { get; set; }

        public int IndicadorId { get; set; }

        public int ClienteFaturamentoId { get; set; }

        public string Proposta { get; set; }

        public DateTime? VencimentoGR { get; set; }

        public DateTime? FreeTimeGR { get; set; }

        public int Periodo { get; set; }

        public int Lote { get; set; }        

        public string Reserva { get; set; }

        public FormaPagamento FormaPagamento { get; set; }

        public TipoDesconto TipoDesconto { get; set; }

        public decimal ValorDesconto { get; set; }

        public decimal ValorDescontoNoServico { get; set; }

        public decimal ValorDescontoFinal { get; set; }

        public bool TipoDescontoPorServico { get; set; }

        public int ServicoFaturadoId { get; set; }

        public int ServicoId { get; set; }

        public decimal ServicoValor { get; set; }

        public decimal DescontoComImposto { get; set; }

        public int? Minuta { get; set; }

        public DateTime? Vencimento { get; set; }

        public DateTime? FreeTime { get; set; }

        public int CriadoPor { get; set; }

        public void Alterar(SolicitacaoDesconto desconto)
        {
            TipoPesquisa = desconto.TipoPesquisa;
            TipoPesquisaNumero = desconto.TipoPesquisaNumero;
            SeqGR = desconto.SeqGR;
            ValorGR = desconto.ValorGR;
            ClienteId = desconto.ClienteId;
            RazaoSocial = desconto.RazaoSocial;
            IndicadorId = desconto.IndicadorId;
            ClienteFaturamentoId = desconto.ClienteFaturamentoId;
            Proposta = desconto.Proposta;
            VencimentoGR = desconto.VencimentoGR;
            FreeTimeGR = desconto.FreeTimeGR;
            Periodo = desconto.Periodo;
            Lote = desconto.Lote;
            FormaPagamento = desconto.FormaPagamento;
            TipoDesconto = desconto.TipoDesconto;
            ValorDesconto = desconto.ValorDesconto;
            ValorDescontoNoServico = desconto.ValorDescontoNoServico;
            ValorDescontoFinal = desconto.ValorDescontoFinal;
            ServicoFaturadoId = desconto.ServicoFaturadoId;
            ServicoId = desconto.ServicoId;
            ServicoValor = desconto.ServicoValor;
            Vencimento = desconto.Vencimento;
            FreeTime = desconto.FreeTime;
            Minuta = desconto.Minuta;
            TipoDescontoPorServico = desconto.TipoDescontoPorServico;
            DescontoComImposto = desconto.DescontoComImposto;
        }

        public override void Validar()
        {
            RuleFor(c => c.TipoPesquisa)
               .IsInEnum()
               .WithMessage("Selecione o tipo de pesquisa");

            RuleFor(c => c.TipoDesconto)
               .IsInEnum()
               .WithMessage("Selecione o tipo de desconto");

            RuleFor(c => c.ValorDesconto)
               .GreaterThan(0)
               .WithMessage("Informe o valor do desconto");

            ValidationResult = Validate(this);
        }
    }
}
