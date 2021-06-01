using Ecoporto.CRM.Business.Enums;
using FluentValidation;

namespace Ecoporto.CRM.Business.Models
{
    public class OportunidadeAdendoFormaPagamento : Entidade<OportunidadeAdendoFormaPagamento>
    {
        public int AdendoId { get; set; }

        public int AnexoId { get; set; }

        public FormaPagamento FormaPagamento { get; set; }

        public override void Validar()
        {
            RuleFor(c => c.AdendoId)
               .GreaterThan(0)
               .WithMessage("Adendo não informado");

            RuleFor(c => c.FormaPagamento)
               .IsInEnum()
               .WithMessage("Forma Pagamento não informada");

            ValidationResult = Validate(this);
        }
    }
}
