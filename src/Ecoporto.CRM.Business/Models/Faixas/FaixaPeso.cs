using FluentValidation;

namespace Ecoporto.CRM.Business.Models
{
    public class FaixaPeso : Entidade<FaixaPeso>
    {
        public FaixaPeso()
        {

        }

        public FaixaPeso(int layoutId, decimal valorInicial, decimal valorFinal, decimal preco)
        {
            LayoutId = layoutId;
            ValorInicial = valorInicial;
            ValorFinal = valorFinal;
            Preco = preco;
        }

        public int LayoutId { get; set; }

        public int OportunidadeLayoutId { get; set; }

        public decimal ValorInicial { get; set; }

        public decimal ValorFinal { get; set; }

        public decimal Preco { get; set; }

        public void Alterar(FaixaPeso faixaPeso)
        {
            ValorInicial = faixaPeso.ValorInicial;
            ValorFinal = faixaPeso.ValorFinal;
            Preco = faixaPeso.Preco;
        }

        public override void Validar()
        {
            RuleFor(c => c.LayoutId)
               .GreaterThan(0)
               .WithMessage("ID do Layout não especificado");

            RuleFor(c => c.ValorInicial)
               .GreaterThan(0)
               .WithMessage("O Valor Inicial é obrigatório");

            RuleFor(c => c.ValorFinal)
               .GreaterThan(0)
               .WithMessage("O Valor Final é obrigatório");

            RuleFor(c => c.Preco)
               .GreaterThan(0)
               .WithMessage("O Preço é obrigatório");

            ValidationResult = Validate(this);
        }
    }
}
