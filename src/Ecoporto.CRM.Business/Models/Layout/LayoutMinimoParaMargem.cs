using FluentValidation;

namespace Ecoporto.CRM.Business.Models
{
    public class LayoutMinimoParaMargem : Entidade<LayoutMinimoParaMargem>
    {
        public Cabecalho Cabecalho { get; set; }
        
        public int ServicoId { get; set; }

        public decimal ValorMinimoMargemDireita { get; set; }

        public decimal ValorMinimoMargemEsquerda { get; set; }

        public decimal ValorMinimoEntreMargens { get; set; }

        public int LinhaReferencia { get; set; }

        public string DescricaoValor { get; set; }

        public LayoutMinimoParaMargem()
        {

        }
        
        public LayoutMinimoParaMargem(
            Cabecalho cabecalho, 
            int servicoId, 
            decimal valorMinimoMargemDireita, 
            decimal valorMinimoMargemEsquerda, 
            decimal valorMinimoEntreMargens, 
            int linhaReferencia, 
            string descricaoValor)
        {
            Cabecalho = cabecalho;
            ServicoId = servicoId;
            ValorMinimoMargemDireita = valorMinimoMargemDireita;
            ValorMinimoMargemEsquerda = valorMinimoMargemEsquerda;
            ValorMinimoEntreMargens = valorMinimoEntreMargens;
            LinhaReferencia = linhaReferencia;
            DescricaoValor = descricaoValor;          
        }

        public override void Validar()
        {            
            RuleFor(c => c.ServicoId)
               .GreaterThan(0)
               .WithMessage("Escolha o Serviço");
            
            RuleFor(c => c.ValorMinimoMargemDireita)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Informe o Valor Mínimo da Margem Direita");

            RuleFor(c => c.ValorMinimoMargemEsquerda)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Informe o Valor Mínimo da Margem Esquerda");

            RuleFor(c => c.ValorMinimoEntreMargens)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Informe o Valor Mínimo Entre Margens");

            RuleFor(c => c.LinhaReferencia)
                .GreaterThanOrEqualTo(0)
                .WithMessage("O número da linha de referência é obrigatório");

            ValidationResult = Validate(this);

            foreach (var erro in Cabecalho.ValidationResult.Errors)
                ValidationResult.Errors.Add(erro);            
        }
    }
}
