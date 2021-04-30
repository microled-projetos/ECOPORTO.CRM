using FluentValidation;

namespace Ecoporto.CRM.Business.Models
{
    public class Cargo : Entidade<Cargo>
    {
        public Cargo()
        {

        }

        public Cargo(string descricao, bool vendedor)
        {
            Descricao = descricao;
            Vendedor = vendedor;
        }

        public void Alterar(Cargo cargo)
        {
            Descricao = cargo.Descricao;
            Vendedor = cargo.Vendedor;
        }

        public string Descricao { get; set; }

        public bool Vendedor { get; set; }

        public override void Validar()
        {
            RuleFor(c => c.Descricao)
               .NotNull()
               .WithMessage("A descrição do Cargo é obrigatória")
               .MinimumLength(3).WithMessage("Tamanho mínimo de 3 caracteres")
               .MaximumLength(150).WithMessage("Tamanho máximo de 150 caracteres");

            ValidationResult = Validate(this);
        }
    }
}
