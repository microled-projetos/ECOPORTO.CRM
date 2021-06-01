using Ecoporto.CRM.Business.Enums;
using FluentValidation;

namespace Ecoporto.CRM.Business.Models
{
    public class LayoutArmazenagemAllIn : Entidade<LayoutArmazenagemAllIn>
    {
        public Cabecalho Cabecalho { get; set; }
        
        public int ServicoId { get; set; }
        
        public BaseCalculo BaseCalculo { get; set; }

        public int Periodo { get; set; }

        public string DescricaoPeriodo { get; set; }

        public decimal CifMinimo { get; set; }

        public decimal CifMaximo { get; set; }

        public string DescricaoCif { get; set; }

        public Margem Margem { get; set; }

        public decimal Valor20 { get; set; }

        public decimal Valor40 { get; set; }

        public decimal ValorMinimo { get; set; }

        public Moeda Moeda { get; set; }

        public string DescricaoValor { get; set; }

        public int TipoDocumentoId { get; set; }

        public BaseCalculoExcesso BaseExcesso { get; set; }

        public decimal ValorExcesso { get; set; }

        public decimal AdicionalPeso { get; set; }

        public decimal PesoLimite { get; set; }

        public bool ProRata { get; set; }

        public LayoutArmazenagemAllIn()
        {

        }

        public LayoutArmazenagemAllIn(
            Cabecalho cabecalho, 
            int servicoId, 
            BaseCalculo baseCalculo, 
            int periodo, 
            string descricaoPeriodo,
            decimal cifMinimo,
            decimal cifMaximo,
            string descricaoCif,
            Margem margem,
            decimal valor20,
            decimal valor40,
            decimal valorMinimo,
            Moeda moeda, 
            string descricaoValor,
            int tipoDocumentoId,
            BaseCalculoExcesso baseExcesso,
            decimal valorExcesso,
            decimal adicionalPeso,
            decimal pesoLimite,
            bool proRata)
        {
            Cabecalho = cabecalho;
            ServicoId = servicoId;
            BaseCalculo = baseCalculo;
            Periodo = periodo;
            DescricaoPeriodo = descricaoPeriodo;
            CifMinimo = cifMinimo;
            CifMaximo = cifMaximo;
            DescricaoCif = descricaoCif;
            Margem = margem;
            Valor20 = valor20;
            Valor40 = valor40;
            ValorMinimo = valorMinimo;
            Moeda = moeda;
            DescricaoValor = descricaoValor;
            TipoDocumentoId = tipoDocumentoId;
            BaseExcesso = baseExcesso;
            ValorExcesso = valorExcesso;
            AdicionalPeso = adicionalPeso;
            PesoLimite = pesoLimite;
            ProRata = proRata;
        }

        public override void Validar()
        {            
            RuleFor(c => c.ServicoId)
               .GreaterThan(0)
               .WithMessage("Escolha o Serviço");

            RuleFor(c => c.BaseCalculo)
                .IsInEnum()
                .WithMessage("Escolha a Base Cálculo");    

            RuleFor(c => c.Moeda)
                .IsInEnum()
                .WithMessage("Informe a Moeda");

            ValidationResult = Validate(this);

            foreach (var erro in Cabecalho.ValidationResult.Errors)
                ValidationResult.Errors.Add(erro);          
        }
    }
}
