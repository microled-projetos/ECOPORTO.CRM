using Ecoporto.CRM.Business.Enums;
using Ecoporto.CRM.Business.ValueObjects;
using FluentValidation;

namespace Ecoporto.CRM.Business.Models
{
    public class LayoutServicoHubPort : Entidade<LayoutServicoHubPort>
    {
        public Cabecalho Cabecalho { get; set; }
        
        public int ServicoId { get; set; }

        public BaseCalculo BaseCalculo { get; set; }

        public decimal Valor { get; set; }

        public int Origem { get; set; }

        public int Destino { get; set; }

        public Moeda Moeda { get; set; }

        public string DescricaoValor { get; set; }

        public FormaPagamento FormaPagamentoNVOCC { get; set; }

        public LayoutServicoHubPort()
        {

        }

        public LayoutServicoHubPort(
            Cabecalho cabecalho, 
            int servicoId, 
            BaseCalculo baseCalculo, 
            decimal valor, 
            int origem, 
            int destino, 
            Moeda moeda,
            FormaPagamento formaPagamentoNVOCC,
            string descricaoValor)
        {
            Cabecalho = cabecalho;
            ServicoId = servicoId;
            BaseCalculo = baseCalculo;
            Valor = valor;
            Origem = origem;
            Destino = destino;
            Moeda = moeda;
            FormaPagamentoNVOCC = formaPagamentoNVOCC;
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
            
            RuleFor(c => c.Origem)
                .GreaterThan(0)
                .WithMessage("Informe a Origem");

            RuleFor(c => c.Destino)
                .GreaterThan(0)
                .WithMessage("Informe o Destino");

            RuleFor(c => c.Moeda)
               .IsInEnum()
               .WithMessage("Escolha a Moeda");

            ValidationResult = Validate(this);

            foreach (var erro in Cabecalho.ValidationResult.Errors)
                ValidationResult.Errors.Add(erro);            
        }
    }
}
