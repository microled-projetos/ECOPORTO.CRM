using Ecoporto.CRM.Workflow.Enums;
using FluentValidation;

namespace Ecoporto.CRM.Business.Models
{
    public class EnvioWorkflow : Entidade<EnvioWorkflow>
    {
        public EnvioWorkflow()
        {

        }

        public EnvioWorkflow(int processoId, Processo processo, string protocolo, string mensagem, int criadoPor)
        {
            ProcessoId = processoId;
            Processo = processo;
            Protocolo = protocolo;
            Mensagem = mensagem;
            CriadoPor = criadoPor;
        }

        public EnvioWorkflow(int processoId, int processoFilhoId, Processo processo, string protocolo, string mensagem, int criadoPor)
        {
            ProcessoId = processoId;
            ProcessoFilhoId = processoFilhoId;
            Processo = processo;
            Protocolo = protocolo;
            Mensagem = mensagem;
            CriadoPor = criadoPor;
        }

        public int ProcessoId { get; set; }

        public int ProcessoFilhoId { get; set; }

        public Processo Processo { get; set; }

        public string Protocolo { get; set; }

        public string Mensagem { get; set; }

        public string DataCadastro { get; set; }

        public int CriadoPor { get; set; }

        public bool Cancelado { get; set; }

        public override void Validar()
        {
            RuleFor(c => c.ProcessoId)
                .GreaterThan(0)
                .WithMessage("O processo é obrigatório");

            RuleFor(c => c.Protocolo)
                .NotEmpty()
                .WithMessage("O protocolo é obrigatório");

            RuleFor(c => c.Mensagem)
                .NotEmpty()
                .WithMessage("A Mensagem é obrigatória");

            RuleFor(c => c.Processo)
                .IsInEnum()
                .WithMessage("O processo é obrigatório");

            ValidationResult = Validate(this);
        }
    }
}
