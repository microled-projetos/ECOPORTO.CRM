using Ecoporto.CRM.Business.Enums;
using FluentValidation;
using System;

namespace Ecoporto.CRM.Business.Models
{
    public class OportunidadeAdendo : Entidade<OportunidadeAdendo>
    {
        public int OportunidadeId { get; set; }

        public TipoAdendo TipoAdendo { get; set; }

        public StatusAdendo StatusAdendo { get; set; }

        public DateTime DataCadastro { get; set; }

        public int CriadoPor { get; set; }

        public override void Validar()
        {
            RuleFor(c => c.OportunidadeId)
               .GreaterThan(0)
               .WithMessage("Oportunidade não informada");

            RuleFor(c => c.TipoAdendo)
               .IsInEnum()
               .WithMessage("O Tipo de Adendo é obrigatório");

            RuleFor(c => c.StatusAdendo)
               .IsInEnum()
               .WithMessage("O Status do Adendo é obrigatório");

            ValidationResult = Validate(this);
        }
    }
}
