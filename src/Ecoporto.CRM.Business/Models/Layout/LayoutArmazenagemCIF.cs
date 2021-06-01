using Ecoporto.CRM.Business.Enums;
using Ecoporto.CRM.Business.ValueObjects;
using FluentValidation;

namespace Ecoporto.CRM.Business.Models
{
    public class LayoutArmazenagemCIF : Entidade<LayoutArmazenagemCIF>
    {
        public Cabecalho Cabecalho { get; set; }

        public int ServicoId { get; set; } 

        public BaseCalculo BaseCalculo { get; set; }

        public int QtdeDias { get; set; }

        public ValorCarga ValorCarga { get; set; }

        public decimal AdicionalArmazenagem { get; set; }

        public decimal AdicionalGRC { get; set; }

        public decimal MinimoGRC { get; set; }

        public decimal AdicionalIMO { get; set; }

        public decimal AdicionalIMOGRC { get; set; }

        public decimal ValorANVISA { get; set; }

        public decimal AnvisaGRC { get; set; }

        public int Periodo { get; set; }

        public Moeda Moeda { get; set; }

        public string DescricaoValor { get; set; }

        public int TipoDocumentoId { get; set; }

        public BaseCalculoExcesso BaseExcesso { get; set; }

        public Margem Margem { get; set; }

        public decimal ValorExcesso { get; set; }

        public decimal ValorCif { get; set; }

        public decimal AdicionalPeso { get; set; }

        public decimal PesoLimite { get; set; }

        public bool ProRata { get; set; }

        public LayoutArmazenagemCIF()
        {

        }

        public LayoutArmazenagemCIF(
            Cabecalho cabecalho, 
            int servicoId, 
            BaseCalculo baseCalculo, 
            int qtdeDias, 
            ValorCarga valorCarga, 
            decimal valorCIF,
            decimal adicionalArmazenagem, 
            decimal adicionalGRC, 
            decimal minimoGRC, 
            decimal adicionalIMO,
            decimal adicionalIMOGRC,
            decimal valorANVISA, 
            decimal anvisaGRC,
            int periodo, 
            Moeda moeda, 
            string descricaoValor,
            int tipoDocumentoId,
            BaseCalculoExcesso baseExcesso,
            Margem margem,
            decimal valorExcesso,
            decimal adicionalPeso,
            decimal pesoLimite,
            bool proRata)
        {
            Cabecalho = cabecalho;
            ServicoId = servicoId;
            BaseCalculo = baseCalculo;
            QtdeDias = qtdeDias;
            ValorCarga = valorCarga;
            ValorCif = valorCIF;
            AdicionalArmazenagem = adicionalArmazenagem;
            AdicionalGRC = adicionalGRC;
            MinimoGRC = minimoGRC;
            AdicionalIMO = adicionalIMO;
            AdicionalIMOGRC = adicionalIMOGRC;
            ValorANVISA = valorANVISA;
            AnvisaGRC = anvisaGRC;
            Periodo = periodo;
            Moeda = moeda;
            DescricaoValor = descricaoValor;
            TipoDocumentoId = tipoDocumentoId;
            BaseExcesso = baseExcesso;
            Margem = margem;
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

            RuleFor(c => c.Periodo)
               .GreaterThan(0)
               .WithMessage("Informe o número de Períodos");

            RuleFor(c => c.QtdeDias)
               .GreaterThan(0)
               .WithMessage("Informe a Quantidade de Dias");
           
            RuleFor(c => c.AdicionalArmazenagem)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Informe o Adicional armazenagem");

            RuleFor(c => c.AdicionalGRC)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Informe o Adicional GRC");

            RuleFor(c => c.MinimoGRC)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Informe o Mínimo GRC");

            RuleFor(c => c.AdicionalIMO)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Informe o Adicional IMO");

            RuleFor(c => c.ValorANVISA)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Informe o Valor Anvisa");

            RuleFor(c => c.Moeda)
                .IsInEnum()
                .WithMessage("Informe a Moeda");

            ValidationResult = Validate(this);

            foreach (var erro in Cabecalho.ValidationResult.Errors)
                ValidationResult.Errors.Add(erro);

            foreach (var erro in ValorCarga.ValidationResult.Errors)
                ValidationResult.Errors.Add(erro);            
        }        
    }
}