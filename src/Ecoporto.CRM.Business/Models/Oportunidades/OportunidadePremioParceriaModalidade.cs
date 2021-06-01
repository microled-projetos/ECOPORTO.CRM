using Ecoporto.CRM.Business.Enums;
using FluentValidation;

namespace Ecoporto.CRM.Business.Models
{
    public class OportunidadePremioParceriaModalidade : Entidade<OportunidadePremioParceriaModalidade>
    {
        public OportunidadePremioParceriaModalidade()
        {

        }

        public OportunidadePremioParceriaModalidade(
            int oportunidadeId, 
            int oportunidadePremioId, 
            ModalidadesComissionamento modalidade)
        {
            OportunidadeId = oportunidadeId;
            OportunidadePremioId = oportunidadePremioId;
            Modalidade = modalidade;
        }

        public int OportunidadeId { get; set; }

        public int OportunidadePremioId { get; set; }
        
        public ModalidadesComissionamento Modalidade { get; set; }

        public override void Validar()
        {
            RuleFor(c => c.OportunidadeId)
                .GreaterThan(0)
                .WithMessage("Oportunidade não informada");

            RuleFor(c => c.OportunidadePremioId)
                .GreaterThan(0)
                .WithMessage("Prêmio Parceria não informado");

            RuleFor(c => c.Modalidade)
                .IsInEnum()
                .WithMessage("Modalidade não informadas");

            ValidationResult = Validate(this);
        }
    }
}
