using FluentValidation;

namespace Ecoporto.CRM.Business.Models
{
    public class LayoutCondicoesIniciais : Entidade<LayoutCondicoesIniciais>
    {
        public Cabecalho Cabecalho { get; set; }
        
        public string CondicoesIniciais { get; set; }

        public LayoutCondicoesIniciais()
        {

        }
        
        public LayoutCondicoesIniciais(Cabecalho cabecalho, string condicoesIniciais)
        {
            Cabecalho = cabecalho;
            CondicoesIniciais = condicoesIniciais;            
        }

        public override void Validar()
        {
            RuleFor(c => c.CondicoesIniciais)
                .NotEmpty()
                .WithMessage("Informe as condições iniciais");

            ValidationResult = Validate(this);

            foreach (var erro in Cabecalho.ValidationResult.Errors)
                ValidationResult.Errors.Add(erro);            
        }
    }
}
