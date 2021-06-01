using Ecoporto.CRM.Business.Enums;
using FluentValidation;

namespace Ecoporto.CRM.Business.Models
{
    public class Mercadoria : Entidade<Mercadoria>
    {
        public Mercadoria()
        {

        }

        public Mercadoria(string descricao, Status status)
        {
            Descricao = descricao;
            Status = status;
        }

        public void Alterar(Mercadoria mercadoria)
        {
            Descricao = mercadoria.Descricao;
            Status = mercadoria.Status;
        }

        public string Descricao { get; set; }

        public Status Status { get; set; }

        public override void Validar()
        {
            RuleFor(c => c.Descricao)
               .NotNull()
               .WithMessage("A descrição da Mercadoria é obrigatória")
               .MinimumLength(3).WithMessage("Tamanho mínimo de 3 caracteres")
               .MaximumLength(150).WithMessage("Tamanho máximo de 150 caracteres");
            
            RuleFor(c => c.Status)
               .IsInEnum()
               .WithMessage("Informe o Status da Mercadoria");

            ValidationResult = Validate(this);
        }
    }
}
