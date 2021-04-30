using Ecoporto.CRM.Business.Enums;
using FluentValidation;
using System;

namespace Ecoporto.CRM.Business.Models
{
    public class SolicitacaoRestituicao : Entidade<SolicitacaoRestituicao>
    {
        public SolicitacaoRestituicao()
        {

        }

        public SolicitacaoRestituicao(
            int solicitacaoId, 
            TipoPesquisa tipoPesquisa,
            int tipoPesquisaNumero,
            int notaFiscalId,
            decimal valorNF, 
            int rps, 
            int lote, 
            string documento, 
            int favorecidoId, 
            string razaoSocial,
            int bancoId, 
            string agencia, 
            string contaCorrente, 
            string fornecedorSAP, 
            decimal valorAPagar,
            DateTime? dataVencimento,
            string observacoes,
            int criadoPor,
            SolicitacaoComercial solicitacaoComercial)
        {
            SolicitacaoId = solicitacaoId;
            TipoPesquisa = tipoPesquisa;
            TipoPesquisaNumero = tipoPesquisaNumero;
            NotaFiscalId = notaFiscalId;
            ValorNF = valorNF;
            RPS = rps;
            Lote = lote;
            Documento = documento;
            FavorecidoId = favorecidoId;
            RazaoSocial = razaoSocial;
            BancoId = bancoId;
            Agencia = agencia;
            ContaCorrente = contaCorrente;
            FornecedorSAP = fornecedorSAP;
            ValorAPagar = valorAPagar;
            DataVencimento = dataVencimento;
            Observacoes = observacoes;
            CriadoPor = criadoPor;
            SolicitacaoComercial = solicitacaoComercial;
        }

        public int SolicitacaoId { get; set; }

        public SolicitacaoComercial SolicitacaoComercial { get; set; }
        
        public TipoPesquisa TipoPesquisa { get; set; }

        public int TipoPesquisaNumero { get; set; }
        
        public int NotaFiscalId { get; set; }

        public int NFE { get; set; }

        public decimal ValorNF { get; set; }

        public int RPS { get; set; }

        public int Lote { get; set; }

        public string Documento { get; set; }

        public int FavorecidoId { get; set; }

        public string RazaoSocial { get; set; }

        public int BancoId { get; set; }

        public string Agencia { get; set; }

        public decimal ValorAPagar { get; set; }

        public string ContaCorrente { get; set; }

        public string FornecedorSAP { get; set; }

        public DateTime? DataVencimento { get; set; }       

        public string Observacoes { get; set; }

        public int CriadoPor { get; set; }

        public void Alterar(SolicitacaoRestituicao solicitacaoRestituicao)
        {
            TipoPesquisa = solicitacaoRestituicao.TipoPesquisa;
            TipoPesquisaNumero = solicitacaoRestituicao.TipoPesquisaNumero;
            NotaFiscalId = solicitacaoRestituicao.NotaFiscalId;
            NFE = solicitacaoRestituicao.NFE;
            ValorNF = solicitacaoRestituicao.ValorNF;
            RPS = solicitacaoRestituicao.RPS;
            Lote = solicitacaoRestituicao.Lote;
            Documento = solicitacaoRestituicao.Documento;
            FavorecidoId = solicitacaoRestituicao.FavorecidoId;
            RazaoSocial = solicitacaoRestituicao.RazaoSocial;
            BancoId = solicitacaoRestituicao.BancoId;
            Agencia = solicitacaoRestituicao.Agencia;
            ContaCorrente = solicitacaoRestituicao.ContaCorrente;
            FornecedorSAP = solicitacaoRestituicao.FornecedorSAP;
            ValorAPagar = solicitacaoRestituicao.ValorAPagar;
            DataVencimento = solicitacaoRestituicao.DataVencimento;
        }
      
        public override void Validar()
        {
            RuleFor(c => c.NotaFiscalId)
                .GreaterThan(0)
                .When(c => SolicitacaoComercial.UnidadeSolicitacao != 3)
                .When(c => SolicitacaoComercial.TipoOperacao != 4)
                .When(c => SolicitacaoComercial.TipoOperacao != 3)
                .When(c => SolicitacaoComercial.MotivoId != 14)
                .WithMessage("Nenhuma Nota Fiscal selecionada");

            RuleFor(c => c.FavorecidoId)
                .GreaterThan(0)
                .When(c => SolicitacaoComercial.TipoOperacao != 6)
                .WithMessage("Selecione o Favorecido");

            RuleFor(c => c.BancoId)
                .GreaterThan(0)
                .WithMessage("Selecione o Banco");

            RuleFor(c => c.Agencia)
                .NotEmpty()
                .WithMessage("A Agência é obrigatória");

            RuleFor(c => c.DataVencimento)
                .GreaterThan(c => DateTime.Now.Date)
                .When(c => c.DataVencimento != null && c.DataVencimento.Value != default(DateTime))
                .WithMessage("A Data de Vencimento deve ser superior a data atual");

            ValidationResult = Validate(this);
        }
    }
}
