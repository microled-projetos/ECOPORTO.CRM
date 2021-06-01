using Ecoporto.CRM.Business.Enums;
using FluentValidation;

namespace Ecoporto.CRM.Business.Models
{
    public class FaixaCIF : Entidade<FaixaCIF>
    {
        public FaixaCIF()
        {

        }

        public FaixaCIF(
            int layoutId, 
            decimal minimo, 
            decimal maximo, 
            Margem margem, 
            decimal valor20, 
            decimal valor40, 
            string descricao)
        {
            LayoutId = layoutId;
            Minimo = minimo;
            Maximo = maximo;
            Margem = margem;
            Valor20 = valor20;
            Valor40 = valor40;
            Descricao = descricao;
        }

        public int LayoutId { get; set; }

        public int OportunidadeLayoutId { get; set; }

        public decimal Minimo { get; set; }

        public decimal Maximo { get; set; }

        public Margem Margem { get; set; }

        public decimal Valor20 { get; set; }

        public decimal Valor40 { get; set; }

        public string Descricao { get; set; }

        public void Alterar(FaixaCIF faixaCIF)
        {
            Minimo = faixaCIF.Minimo;
            Maximo = faixaCIF.Maximo;
            Margem = faixaCIF.Margem;
            Valor20 = faixaCIF.Valor20;
            Valor40 = faixaCIF.Valor40;
            Descricao = faixaCIF.Descricao;
        }

        public override void Validar()
        {
            RuleFor(c => c.LayoutId)
               .GreaterThan(0)
               .WithMessage("ID do Layout não especificado");

            RuleFor(c => c.Minimo)
               .GreaterThan(0)
               .WithMessage("O CIF Mínimo é obrigatório");

            RuleFor(c => c.Maximo)
               .GreaterThan(0)
               .WithMessage("O CIF Máximo é obrigatório");

            RuleFor(c => c.Margem)
               .IsInEnum()
               .WithMessage("A Margem é obrigatória");

            RuleFor(c => c.Valor20)
               .GreaterThan(0)
               .WithMessage("O Valor 20 é obrigatório");

            RuleFor(c => c.Valor40)
               .GreaterThan(0)
               .WithMessage("O Valor 20 é obrigatório");

            RuleFor(c => c.Descricao)
               .NotNull()
               .WithMessage("A descrição é obrigatória");

            ValidationResult = Validate(this);
        }
    }
}
