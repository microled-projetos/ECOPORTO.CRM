using Ecoporto.CRM.Business.Enums;
using Ecoporto.CRM.Business.ValueObjects;
using FluentValidation;

namespace Ecoporto.CRM.Business.Models
{
    public class LayoutServicoMecanicaManual : Entidade<LayoutServicoMecanicaManual>
    {   
        public Cabecalho Cabecalho { get; set; }
        
        public int ServicoId { get; set; }

        public BaseCalculo BaseCalculo { get; set; }

        public ValorCarga ValorCarga { get; set; }

        public decimal AdicionalIMO { get; set; }

        public decimal Exercito { get; set; }

        public Moeda Moeda { get; set; }

        public decimal PesoMaximo { get; set; }

        public decimal AdicionalPeso { get; set; }

        public TipoTrabalho TipoTrabalho { get; set; }

        public TipoCarga TipoCarga { get; set; }

        public string DescricaoValor { get; set; }

        public LayoutServicoMecanicaManual()
        {

        }

        public LayoutServicoMecanicaManual(
            Cabecalho cabecalho, 
            int servicoId, 
            BaseCalculo baseCalculo, 
            ValorCarga valorCarga, 
            decimal adicionalIMO, 
            decimal exercito,
            Moeda moeda, 
            decimal pesoMaximo, 
            decimal adicionalPeso, 
            TipoTrabalho tipoTrabalho, 
            string descricaoValor)
        {
            Cabecalho = cabecalho;
            ServicoId = servicoId;
            BaseCalculo = baseCalculo;
            ValorCarga = valorCarga;
            AdicionalIMO = adicionalIMO;
            Exercito = exercito;
            Moeda = moeda;
            PesoMaximo = pesoMaximo;
            AdicionalPeso = adicionalPeso;
            TipoTrabalho = tipoTrabalho;
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

            RuleFor(c => c.AdicionalIMO)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Informe o Adicional IMO");

            RuleFor(c => c.Moeda)
                .IsInEnum()
                .WithMessage("Informe a Moeda");
            
            RuleFor(c => c.PesoMaximo)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Informe o Peso Máximo");

            RuleFor(c => c.AdicionalPeso)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Informe o Adicional de Peso");                     

            ValidationResult = Validate(this);

            foreach (var erro in Cabecalho.ValidationResult.Errors)
                ValidationResult.Errors.Add(erro);          
        }
    }
}
