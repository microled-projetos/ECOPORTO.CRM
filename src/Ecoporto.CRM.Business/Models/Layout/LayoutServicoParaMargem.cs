using Ecoporto.CRM.Business.Enums;
using FluentValidation;
using System.Collections.Generic;

namespace Ecoporto.CRM.Business.Models
{
    public class LayoutServicoParaMargem : Entidade<LayoutServicoParaMargem>
    {
        public Cabecalho Cabecalho { get; set; }
        
        public int ServicoId { get; set; }

        public BaseCalculo BaseCalculo { get; set; }

        public TipoCarga TipoCarga { get; set; }

        public decimal ValorMargemDireita { get; set; }

        public decimal ValorMargemEsquerda { get; set; }

        public decimal ValorEntreMargens { get; set; }

        public decimal AdicionalIMO { get; set; }

        public decimal Exercito { get; set; }

        public Moeda Moeda { get; set; }

        public decimal PesoMaximo { get; set; }

        public decimal AdicionalPeso { get; set; }

        public string DescricaoValor { get; set; }

        public int TipoDocumentoId { get; set; }

        public BaseCalculoExcesso BaseExcesso { get; set; }

        public decimal ValorExcesso { get; set; }

        public decimal PesoLimite { get; set; }

        public bool ProRata { get; set; }

        public IEnumerable<FaixaPeso> FaixasPeso { get; set; }

        public LayoutServicoParaMargem()
        {

        }

        public LayoutServicoParaMargem(
            Cabecalho cabecalho, 
            int servicoId, 
            BaseCalculo baseCalculo,
            TipoCarga tipoCarga,
            decimal valorMargemDireita, 
            decimal valorMargemEsquerda, 
            decimal valorEntreMargens, 
            decimal adicionalIMO, 
            decimal exercito,
            Moeda moeda, 
            decimal pesoMaximo, 
            decimal adicionalPeso, 
            string descricaoValor,
            int tipoDocumentoId,
            BaseCalculoExcesso baseExcesso,
            decimal valorExcesso,
            decimal pesoLimite,
            bool proRata)
        {
            Cabecalho = cabecalho;
            ServicoId = servicoId;
            BaseCalculo = baseCalculo;
            TipoCarga = tipoCarga;
            ValorMargemDireita = valorMargemDireita;
            ValorMargemEsquerda = valorMargemEsquerda;
            ValorEntreMargens = valorEntreMargens;
            AdicionalIMO = adicionalIMO;
            Exercito = exercito;
            Moeda = moeda;
            PesoMaximo = pesoMaximo;
            AdicionalPeso = adicionalPeso;
            DescricaoValor = descricaoValor;
            TipoDocumentoId = tipoDocumentoId;
            BaseExcesso = baseExcesso;
            ValorExcesso = valorExcesso;
            PesoLimite = pesoLimite;
            ProRata = proRata;

            FaixasPeso = new List<FaixaPeso>();            
        }

        public override void Validar()
        {        
            RuleFor(c => c.ServicoId)
               .GreaterThan(0)
               .WithMessage("Escolha o Serviço");
            
            RuleFor(c => c.BaseCalculo)
                .IsInEnum()
                .WithMessage("Escolha a Base Cálculo");

            RuleFor(c => c.TipoCarga)
               .IsInEnum()
               .WithMessage("Escolha o Tipo de Carga");

            RuleFor(c => c.ValorMargemDireita)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Informe o Valor Margem Direita");

            RuleFor(c => c.ValorMargemEsquerda)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Informe o Valor Margem Esquerda");

            RuleFor(c => c.ValorEntreMargens)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Informe o Valor Entre Margens");

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
