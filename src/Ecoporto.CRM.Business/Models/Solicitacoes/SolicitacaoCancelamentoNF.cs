using Ecoporto.CRM.Business.Enums;
using FluentValidation;
using System;

namespace Ecoporto.CRM.Business.Models
{
    public class SolicitacaoCancelamentoNF : Entidade<SolicitacaoCancelamentoNF>
    {
        public SolicitacaoCancelamentoNF()
        {

        }

        public SolicitacaoCancelamentoNF(
            int solicitacaoId,
            TipoPesquisa tipoPesquisa, 
            int tipoPesquisaNumero, 
            int lote, 
            int notaFiscalId, 
            int nfe, 
            decimal valorNF, 
            int? contaId,
            string razaoSocial,
            FormaPagamento formaPagamento,            
            DateTime? dataEmissao, 
            decimal desconto, 
            decimal valorNovaNF, 
            decimal valorAPagar,
            DateTime? dataProrrogacao,
            int criadoPor,
            SolicitacaoComercial solicitacaoComercial)
        {
            SolicitacaoId = solicitacaoId;
            TipoPesquisa = tipoPesquisa;
            TipoPesquisaNumero = tipoPesquisaNumero;
            Lote = lote;
            NotaFiscalId = notaFiscalId;
            NFE = nfe;
            ValorNF = valorNF;
            RazaoSocial = razaoSocial;
            ContaId = contaId;
            FormaPagamento = formaPagamento;            
            DataEmissao = dataEmissao;
            Desconto = desconto;
            ValorNovaNF = valorNovaNF;
            ValorAPagar = valorAPagar;
            DataProrrogacao = dataProrrogacao;
            CriadoPor = criadoPor;
            SolicitacaoComercial = solicitacaoComercial;
        }

        public int SolicitacaoId { get; set; }

        public TipoPesquisa TipoPesquisa { get; set; }

        public SolicitacaoComercial SolicitacaoComercial { get; set; }

        public int TipoPesquisaNumero { get; set; }

        public int Lote { get; set; }

        public int NotaFiscalId { get; set; }

        public int NFE { get; set; }

        public decimal ValorNF { get; set; }

        public string RazaoSocial { get; set; }

        public int? ContaId { get; set; }

        public string ContaDescricao { get; set; }

        public FormaPagamento FormaPagamento { get; set; }

        public DateTime? DataEmissao { get; set; }

        public decimal Desconto { get; set; }

        public decimal ValorNovaNF { get; set; }

        public decimal ValorAPagar { get; set; }

        public DateTime? DataProrrogacao { get; set; }

        public int CriadoPor { get; set; }

        public void Alterar(SolicitacaoCancelamentoNF cancelamentoNF)
        {
            TipoPesquisa = cancelamentoNF.TipoPesquisa;
            TipoPesquisaNumero = cancelamentoNF.TipoPesquisaNumero;
            Lote = cancelamentoNF.Lote;
            NotaFiscalId = cancelamentoNF.NotaFiscalId;
            NFE = cancelamentoNF.NFE;
            ValorNF = cancelamentoNF.ValorNF;
            RazaoSocial = cancelamentoNF.RazaoSocial;
            ContaId = ContaId;
            FormaPagamento = cancelamentoNF.FormaPagamento;
            DataEmissao = cancelamentoNF.DataEmissao;
            Desconto = cancelamentoNF.Desconto;
            ValorNovaNF = cancelamentoNF.ValorNovaNF;
            ValorAPagar = cancelamentoNF.ValorAPagar;
            DataProrrogacao = cancelamentoNF.DataProrrogacao;
        }

        public override void Validar()
        {
            RuleFor(c => c.TipoPesquisa)
               .IsInEnum()
               .WithMessage("Selecione o tipo de pesquisa");

            RuleFor(c => c.NotaFiscalId)
               .GreaterThan(0)
               .When(c => SolicitacaoComercial.UnidadeSolicitacao != 3)
               .When(c => SolicitacaoComercial.TipoOperacao != 4)
               .When(c => SolicitacaoComercial.TipoOperacao != 3)
               .WithMessage("Nenhuma Nota Fiscal selecionada");

            ValidationResult = Validate(this);
        }
    }
}
