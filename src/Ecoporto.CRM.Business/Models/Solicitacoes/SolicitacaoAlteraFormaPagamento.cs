using Ecoporto.CRM.Business.Enums;
using FluentValidation;
using System;

namespace Ecoporto.CRM.Business.Models
{
    public class SolicitacaoAlteraFormaPagamento : Entidade<SolicitacaoAlteraFormaPagamento>
    {
        public SolicitacaoAlteraFormaPagamento()
        {

        }

        public SolicitacaoAlteraFormaPagamento(
            int solicitacaoId, 
            TipoPesquisa tipoPesquisa, 
            int tipoPesquisaNumero, 
            int lote, 
            int gr,
            decimal valor, 
            int? faturadoContraId, 
            string condicaoPagamentoId, 
            string emailNota, 
            int criadoPor)
        {
            SolicitacaoId = solicitacaoId;
            TipoPesquisa = tipoPesquisa;
            TipoPesquisaNumero = tipoPesquisaNumero;
            Lote = lote;
            Gr = gr;
            Valor = valor;
            FaturadoContraId = faturadoContraId;
            CondicaoPagamentoId = condicaoPagamentoId;
            EmailNota = emailNota;
            CriadoPor = criadoPor;
        }

        public int SolicitacaoId { get; set; }

        public TipoPesquisa TipoPesquisa { get; set; }

        public SolicitacaoComercial SolicitacaoComercial { get; set; }

        public int TipoPesquisaNumero { get; set; }

        public int Lote { get; set; }

        public int Gr { get; set; }

        public decimal Valor { get; set; }
        
        public int? FaturadoContraId { get; set; }

        public string FaturadoContraDescricao { get; set; }               

        public FormaPagamento FormaPagamento { get; set; }

        public string CondicaoPagamentoId { get; set; }

        public string EmailNota { get; set; }

        public string Proposta { get; set; }

        public DateTime FreeTime { get; set; }

        public string Periodo { get; set; }

        public string Cliente { get; set; }

        public string Indicador { get; set; }

        public int CriadoPor { get; set; }

        public void Alterar(SolicitacaoAlteraFormaPagamento solicitacao)
        {
            SolicitacaoId = solicitacao.SolicitacaoId;
            TipoPesquisa = solicitacao.TipoPesquisa;
            SolicitacaoComercial = solicitacao.SolicitacaoComercial;
            TipoPesquisaNumero = solicitacao.TipoPesquisaNumero;
            Lote = solicitacao.Lote;
            Valor = solicitacao.Valor;
            FaturadoContraId = solicitacao.FaturadoContraId;
            FormaPagamento = solicitacao.FormaPagamento;
            EmailNota = solicitacao.EmailNota;
            CriadoPor = solicitacao.CriadoPor;
        }

        public override void Validar()
        {
            RuleFor(c => c.TipoPesquisa)
                .IsInEnum()
                .WithMessage("Selecione o tipo de pesquisa");

            RuleFor(c => c.EmailNota)
                .NotEmpty()
                .WithMessage("Informe o Email para encaminhamento da Nota");

            RuleFor(c => c.CondicaoPagamentoId)
                .NotEmpty()
                .WithMessage("Informe a Condição de Pagamento");

            ValidationResult = Validate(this);
        }
    }
}
