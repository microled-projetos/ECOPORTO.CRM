using Ecoporto.CRM.Business.Enums;
using Ecoporto.CRM.Business.ValueObjects;
using FluentValidation;

namespace Ecoporto.CRM.Business.Models
{
    public class LayoutServicosGerais : Entidade<LayoutServicosGerais>
    {
        public Cabecalho Cabecalho { get; set; }

        public int ServicoId { get; set; }

        public BaseCalculo BaseCalculo { get; set; }

        public ValorCarga ValorCarga { get; set; }

        public Moeda Moeda { get; set; }

        public string DescricaoValor { get; set; }

        public decimal AdicionalIMO { get; set; }

        public decimal Exercito { get; set; }

        public int TipoDocumentoId { get; set; }

        public BaseCalculoExcesso BaseExcesso { get; set; }

        public decimal ValorExcesso { get; set; }

        public FormaPagamento FormaPagamentoNVOCC { get; set; }

        public LayoutServicosGerais()
        {

        }

        public LayoutServicosGerais(
            Cabecalho cabecalho,
            int servicoId,
            BaseCalculo baseCalculo, 
            ValorCarga valorCarga, 
            Moeda moeda, 
            string descricaoValor,
            decimal adicionalIMO,
            decimal exercito,
            int tipoDocumentoId,
            BaseCalculoExcesso baseExcesso,
            decimal valorExcesso,
            FormaPagamento formaPagamentoNVOCC)
        {
            Cabecalho = cabecalho;
            ServicoId = servicoId;
            BaseCalculo = baseCalculo;
            ValorCarga = valorCarga;
            Moeda = moeda;
            DescricaoValor = descricaoValor;
            AdicionalIMO = adicionalIMO;
            Exercito = exercito;
            TipoDocumentoId = tipoDocumentoId;
            BaseExcesso = baseExcesso;
            ValorExcesso = valorExcesso;
            FormaPagamentoNVOCC = formaPagamentoNVOCC;
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

            foreach (var erro in ValorCarga.ValidationResult.Errors)
                ValidationResult.Errors.Add(erro);            
        }
    }
}
