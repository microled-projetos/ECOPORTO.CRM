using FluentValidation;

namespace Ecoporto.CRM.Business.Models
{
    public class Imposto : Entidade<Imposto>
    {
        public Imposto()
        {

        }

        public Imposto(int id, string descricao)
        {
            Id = id;
            Descricao = descricao;
        }

        public string Descricao { get; set; }

        public override void Validar()
        {
            RuleFor(c => c.Descricao)
                .NotEmpty()
                .WithMessage("A descrição do imposto é obrigatória");

            ValidationResult = Validate(this);
        }
    }
}
