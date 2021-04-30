using FluentValidation;

namespace Ecoporto.CRM.Business.Models
{
    public class FaixaBL : Entidade<FaixaBL>
    {
        public FaixaBL()
        {

        }

        public FaixaBL(int layoutId, int bLMinimo, int bLMaximo, decimal valorMinimo)
        {
            LayoutId = layoutId;
            BLMinimo = bLMinimo;
            BLMaximo = bLMaximo;
            ValorMinimo = valorMinimo;

            Validar();
        }

        public int LayoutId { get; set; }

        public int OportunidadeLayoutId { get; set; }

        public int BLMinimo { get; set; }

        public int BLMaximo { get; set; }

        public decimal ValorMinimo { get; set; }

        public void Alterar(FaixaBL faixaBL)
        {
            BLMinimo = faixaBL.BLMinimo;
            BLMaximo = faixaBL.BLMaximo;
            ValorMinimo = faixaBL.ValorMinimo;
        }

        public override void Validar()
        {
            RuleFor(c => c.LayoutId)
               .GreaterThan(0)
               .WithMessage("ID do Layout não especificado");

            RuleFor(c => c.BLMinimo)
               .GreaterThan(0)
               .WithMessage("O BL Mínimo é obrigatório");

            RuleFor(c => c.BLMaximo)
               .GreaterThan(0)
               .WithMessage("O BL Máximo é obrigatório");            

            ValidationResult = Validate(this);
        }
    }
}
