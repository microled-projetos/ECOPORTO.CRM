using FluentValidation;

namespace Ecoporto.CRM.Business.Models
{
    public class ClienteHubPort : Entidade<ClienteHubPort>
    {    
        public string Descricao { get; set; }

        public override void Validar()
        {
            RuleFor(c => c.Descricao)
                .NotEmpty()
                .WithMessage("A descrição do cliente é obrigatório");

            ValidationResult = Validate(this);
        }
    }
}
