using Ecoporto.CRM.Business.Enums;
using FluentValidation;
using System;

namespace Ecoporto.CRM.Business.Models
{
    public class OportunidadeProposta : Entidade<OportunidadeProposta>
    {
        public int OportunidadeId { get; set; }

        public TipoOperacao TipoOperacao { get; set; }

        public int ModeloId { get; set; }

        public FormaPagamento FormaPagamento { get; set; }

        public int DiasFreeTime { get; set; }

        public int QtdeDias { get; set; }

        public int Validade { get; set; }

        public TipoValidade TipoValidade { get; set; }

        public int VendedorId { get; set; }

        public int ImpostoId { get; set; }

        public DateTime? DataInicio { get; set; }

        public DateTime? DataTermino { get; set; }

        public bool Acordo { get; set; }

        public bool ParametroBL { get; set; }

        public bool ParametroLote { get; set; }

        public bool ParametroConteiner { get; set; }

        public bool ParametroIdTabela { get; set; }

        public string BL { get; set; }

        public string Lote { get; set; }

        public string Conteiner { get; set; }

        public bool HubPort { get; set; }

        public bool CobrancaEspecial { get; set; }

        public decimal DesovaParcial { get; set; }

        public decimal FatorCP { get; set; }

        public int PosicIsento { get; set; }

        public int? TabelaReferencia { get; set; }

        public override void Validar()
        
        {
            RuleFor(c => c.OportunidadeId)
                .GreaterThan(0)
                .WithMessage("Oportunidade não informada");

            RuleFor(c => c.TipoOperacao)
                .IsInEnum()
                .WithMessage("Selecione o Tipo de Operação");

            RuleFor(c => c.ModeloId)
                .GreaterThan(0)
                .WithMessage("Selecione um Modelo");

            RuleFor(c => c.FormaPagamento)
                .IsInEnum()
                .WithMessage("Selecione a Forma de Pagamento");
                     
            RuleFor(c => c.TipoValidade)
                .IsInEnum()
                .WithMessage("Informe o Tipo de Validade");

            RuleFor(c => c.Validade)
                .GreaterThan(0).WithMessage("A validade da proposta é obrigatória");

            RuleFor(c => c.Validade)
                .LessThanOrEqualTo(365).When(c => c.TipoValidade == TipoValidade.DIAS).WithMessage("A validade da proposta não pode ser superior a 365 dias");

            RuleFor(c => c.Validade)
                .LessThanOrEqualTo(12).When(c => c.TipoValidade == TipoValidade.MESES).WithMessage("A validade da proposta não pode ser superior a 12 meses");

            RuleFor(c => c.Validade)
                .LessThanOrEqualTo(1).When(c => c.TipoValidade == TipoValidade.ANOS).WithMessage("A validade da proposta não pode ser superior a 1 ano");

            RuleFor(c => c.VendedorId)
                .GreaterThan(0)
                .WithMessage("Informe o Vendedor");

            RuleFor(c => c.ImpostoId)
                .GreaterThan(0)
                .When(c => c.ParametroIdTabela ==false )
                .WithMessage("Informe o Imposto");

            RuleFor(c => c.QtdeDias)
               .GreaterThan(0)
                .When(c => c.FormaPagamento == FormaPagamento.FATURADO)
               .WithMessage("Informe a Quantidade de Dias");

            ValidationResult = Validate(this);
        }
    }
}
