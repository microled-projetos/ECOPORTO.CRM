using Ecoporto.CRM.Business.Enums;
using System;
using FluentValidation;

namespace Ecoporto.CRM.Business.Models
{
    public class SolicitacaoProrrogacao : Entidade<SolicitacaoProrrogacao>
    {
        public SolicitacaoProrrogacao()
        {

        }

        public SolicitacaoProrrogacao(
            int solicitacaoId, 
            int notaFiscalId, 
            int nfe,
            decimal valorNF, 
            string razaoSocial, 
            int? contaId,
            DateTime? vencimentoOriginal, 
            DateTime? dataProrrogacao, 
            int numeroProrrogacao, 
            Boleano isentarJuros, 
            decimal valorJuros, 
            decimal valorTotalComJuros, 
            string observacoes,
            int criadoPor,
            SolicitacaoComercial solicitacaoComercial)
        {
            SolicitacaoId = solicitacaoId;
            NotaFiscalId = notaFiscalId;
            NFE = nfe;
            ValorNF = valorNF;
            RazaoSocial = razaoSocial;
            ContaId = contaId;
            VencimentoOriginal = vencimentoOriginal;
            DataProrrogacao = dataProrrogacao;
            NumeroProrrogacao = numeroProrrogacao;
            IsentarJuros = isentarJuros;
            ValorJuros = valorJuros;
            ValorTotalComJuros = valorTotalComJuros;
            Observacoes = observacoes;
            CriadoPor = criadoPor;
            SolicitacaoComercial = solicitacaoComercial;
        }

        public int SolicitacaoId { get; set; }

        public SolicitacaoComercial SolicitacaoComercial { get; set; }

        public int TipoOperacao { get; set; }

        public int NotaFiscalId { get; set; }

        public int NFE { get; set; }

        public DateTime DataEmissao { get; set; }

        public decimal ValorNF { get; set; }

        public int? ContaId { get; set; }

        public string ContaDescricao { get; set; }

        public string RazaoSocial { get; set; }

        public string Documento { get; set; }

        public DateTime? VencimentoOriginal { get; set; }

        public DateTime? DataProrrogacao { get; set; }

        public int NumeroProrrogacao { get; set; }

        public Boleano IsentarJuros { get; set; }

        public decimal ValorJuros { get; set; }

        public decimal ValorTotalComJuros { get; set; }

        public string Observacoes { get; set; }

        public int CriadoPor { get; set; }

        public void Alterar(SolicitacaoProrrogacao prorrogacao)
        {
            NotaFiscalId = prorrogacao.NotaFiscalId;
            NFE = prorrogacao.NFE;
            ValorNF = prorrogacao.ValorNF;
            RazaoSocial = prorrogacao.RazaoSocial;
            ContaId = prorrogacao.ContaId;
            VencimentoOriginal = prorrogacao.VencimentoOriginal;
            DataProrrogacao = prorrogacao.DataProrrogacao;
            NumeroProrrogacao = prorrogacao.NumeroProrrogacao;
            IsentarJuros = prorrogacao.IsentarJuros;
            ValorJuros = prorrogacao.ValorJuros;
            ValorTotalComJuros = prorrogacao.ValorTotalComJuros;
            Observacoes = prorrogacao.Observacoes;
        }

        public override void Validar()
        {
            RuleFor(c => c.NotaFiscalId)
               .GreaterThan(0)
               .When(c => SolicitacaoComercial.UnidadeSolicitacao != 3)
               .When(c => SolicitacaoComercial.TipoOperacao != 4)
               .When(c => SolicitacaoComercial.TipoOperacao != 3)
               .WithMessage("Nenhuma Nota Fiscal selecionada");

            RuleFor(c => c.VencimentoOriginal)          
                .Must(c => c != default(DateTime))
                .WithMessage("A Data de Vencimento é inválida");

            RuleFor(c => c.DataProrrogacao)
                .NotNull()
                .WithMessage("Informe a data de prorrogação")
                .Must(c => c != default(DateTime))
                .WithMessage("A Data de prorrogação é inválida");

            RuleFor(c => c.Observacoes)
                .MaximumLength(1000)
                .WithMessage("Observações devem ter no máximo 1000 caracteres");

            ValidationResult = Validate(this);
        }
    }
}
