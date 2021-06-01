using Ecoporto.CRM.Business.Enums;
using Ecoporto.CRM.Business.ValueObjects;
using FluentValidation;

namespace Ecoporto.CRM.Business.Models
{
    public class LayoutPeriodoPadrao : Entidade<LayoutPeriodoPadrao>
    {
        public Cabecalho Cabecalho { get; set; }

        public int ServicoId { get; set; } 

        public BaseCalculo BaseCalculo { get; set; }

        public int QtdeDias { get; set; }

        public ValorCarga ValorCarga { get; set; }        

        public int Periodo { get; set; }

        public string DescricaoValor { get; set; }

        public LayoutPeriodoPadrao()
        {

        }

        public LayoutPeriodoPadrao(
            Cabecalho cabecalho, 
            int servicoId, 
            BaseCalculo baseCalculo, 
            int qtdeDias, 
            ValorCarga valorCarga,             
            int periodo,
            string descricaoValor)
        {
            Cabecalho = cabecalho;
            ServicoId = servicoId;
            BaseCalculo = baseCalculo;
            QtdeDias = qtdeDias;
            ValorCarga = valorCarga;
            Periodo = periodo;            
            DescricaoValor = descricaoValor;
        }

        public override void Validar()
        {            
            RuleFor(c => c.ServicoId)
               .GreaterThan(0)
               .WithMessage("Escolha o Serviço");
           
            RuleFor(c => c.BaseCalculo)
                .IsInEnum()
                .WithMessage("Escolha a Base Cálculo");

            RuleFor(c => c.Periodo)
               .GreaterThan(0)
               .WithMessage("Informe o número de Períodos");

            RuleFor(c => c.QtdeDias)
               .GreaterThan(0)
               .WithMessage("Informe a Quantidade de Dias");

            ValidationResult = Validate(this);

            foreach (var erro in Cabecalho.ValidationResult.Errors)
                ValidationResult.Errors.Add(erro);

            foreach (var erro in ValorCarga.ValidationResult.Errors)
                ValidationResult.Errors.Add(erro);            
        }        
    }
}