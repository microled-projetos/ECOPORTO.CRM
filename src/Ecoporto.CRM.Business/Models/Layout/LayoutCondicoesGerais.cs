using FluentValidation;

namespace Ecoporto.CRM.Business.Models
{
    public class LayoutCondicoesGerais : Entidade<LayoutCondicoesGerais>
    {
        public Cabecalho Cabecalho { get; set; }
        
        public string CondicoesGerais { get; set; }

        public LayoutCondicoesGerais()
        {

        }
        
        public LayoutCondicoesGerais(Cabecalho cabecalho, string condicoesGerais)
        {
            Cabecalho = cabecalho;
            CondicoesGerais = condicoesGerais;            
        }

        public override void Validar()
        {           
            RuleFor(c => c.CondicoesGerais)
                .NotEmpty()
                .WithMessage("Informe as condições gerais");

            ValidationResult = Validate(this);

            foreach (var erro in Cabecalho.ValidationResult.Errors)
                ValidationResult.Errors.Add(erro);            
        }
    }
}
