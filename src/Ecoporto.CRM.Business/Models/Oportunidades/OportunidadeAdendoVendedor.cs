using FluentValidation;

namespace Ecoporto.CRM.Business.Models
{
    public class OportunidadeAdendoVendedor : Entidade<OportunidadeAdendoVendedor>
    {
        public int AdendoId { get; set; }

        public int VendedorId { get; set; }

        public override void Validar()
        {
            RuleFor(c => c.AdendoId)
               .GreaterThan(0)
               .WithMessage("Adendo não informado");

            RuleFor(c => c.VendedorId)
               .GreaterThan(0)
               .WithMessage("Vendedor não informado");

            ValidationResult = Validate(this);
        }
    }
}
