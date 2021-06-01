using Ecoporto.CRM.Business.Enums;
using FluentValidation;
using FluentValidation.Results;
using System;

namespace Ecoporto.CRM.IntegraChronosAPI.Models
{
    public class Processo : AbstractValidator<Processo>
    {
        public Processo()
        {

        }

        public Processo(int id_Processo, int tipo_Processo, int id_Workflow, int id_Etapa, int acao)
        {
            Id_Processo = id_Processo;
            Tipo_Processo = tipo_Processo;
            Id_Workflow = id_Workflow;
            Id_Etapa = id_Etapa;
            Acao = acao;

            Validar();
        }

        public int Id { get; set; }

        public int Id_Processo { get; set; }

        public int Tipo_Processo { get; set; }

        public int Id_Workflow { get; set; }

        public int Id_Etapa { get; set; }

        public int Acao { get; set; }

        public Status Status { get; set; }

        public string Motivo { get; set; }

        public DateTime? DataExecucao { get; set; }

        public ValidationResult ValidationResult { get; set; }

        public void Validar()
        {
            RuleFor(c => c.Id_Processo)
                .GreaterThan(0)
                .WithMessage("O ID do Processo é inválido");

            RuleFor(c => c.Id_Workflow)
                .GreaterThan(0)
                .WithMessage("O ID do Workflow é inválido");

            RuleFor(c => c.Id_Etapa)
                .GreaterThan(0)
                .WithMessage("O ID da Etapa é inválido");

            RuleFor(c => c.Tipo_Processo)
                .GreaterThanOrEqualTo(1)
                .LessThanOrEqualTo(10)
                .WithMessage("Tipo de Processo inválido");

            RuleFor(c => c.Acao)
                .GreaterThanOrEqualTo(1)
                .LessThanOrEqualTo(4)
                .WithMessage("Ação inválida");

            ValidationResult = Validate(this);
        }
    }
}