using Ecoporto.CRM.Business.Enums;
using FluentValidation;
using System;

namespace Ecoporto.CRM.Business.Models
{
    public class Anexo : Entidade<Anexo>
    {
        public int IdProcesso { get; set; }

        public string Arquivo { get; set; }

        public DateTime DataCadastro { get; set; }

        public int CriadoPor { get; set; }

        public TipoAnexo TipoAnexo { get; set; }

        public int Versao { get; set; }

        public string IdArquivo { get; set; }

        public int TipoDoc { get; set; }

        public override void Validar()
        {
            RuleFor(c => c.Arquivo)
               .NotEmpty()
               .WithMessage("O anexo é obrigatório");

            ValidationResult = Validate(this);
        }
    }
}
