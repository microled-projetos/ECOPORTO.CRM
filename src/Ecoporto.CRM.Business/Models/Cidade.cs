using Ecoporto.CRM.Business.Enums;
using FluentValidation;

namespace Ecoporto.CRM.Business.Models
{
    public class Cidade : Entidade<Cidade>
    {
        public Cidade()
        {

        }
        
        public string Descricao { get; set; }

        public Estado Estado { get; set; }

        public override void Validar()
        {
            RuleFor(c => c.Descricao)
                .NotEmpty()
                .WithMessage("A descrição da Cidade é obrigatória");

            ValidationResult = Validate(this);
        }
    }
}