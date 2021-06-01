using Ecoporto.CRM.Business.Enums;
using Ecoporto.CRM.Business.ValueObjects;
using FluentValidation;

namespace Ecoporto.CRM.Business.Models
{
    public class LayoutServicoLiberacao : Entidade<LayoutServicoLiberacao>
    {     
        public Cabecalho Cabecalho { get; set; }
        
        public int ServicoId { get; set; }

        public BaseCalculo BaseCalculo { get; set; }

        public Margem Margem { get; set; }

        public TipoCarga TipoCarga { get; set; }

        public ValorCarga ValorCarga { get; set; }

        public int Reembolso { get; set; }

        public Moeda Moeda { get; set; }
        
        public string DescricaoValor { get; set; }

        public int TipoDocumentoId { get; set; }

        public int GrupoAtracacaoId { get; set; }

        public decimal AdicionalIMO { get; set; }

        public decimal Exercito { get; set; }

        public LayoutServicoLiberacao()
        {

        }

        public LayoutServicoLiberacao(
            Cabecalho cabecalho, 
            int servicoId, 
            BaseCalculo baseCalculo,
            Margem margem,
            TipoCarga tipoCarga,
            ValorCarga valorCarga, 
            int reembolso, 
            Moeda moeda, 
            string descricaoValor,
            int tipoDocumentoId,
            int grupoAtracacaoId,
            decimal adicionalIMO,
            decimal exercito)
        {
            Cabecalho = cabecalho;
            ServicoId = servicoId;
            BaseCalculo = baseCalculo;
            Margem = margem;
            TipoCarga = tipoCarga;
            ValorCarga = valorCarga;
            Reembolso = reembolso;
            Moeda = moeda;
            DescricaoValor = descricaoValor;
            TipoDocumentoId = tipoDocumentoId;
            GrupoAtracacaoId = grupoAtracacaoId;
            AdicionalIMO = adicionalIMO;
            Exercito = exercito;
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
               .WithMessage("Escolha a Moeda");

            ValidationResult = Validate(this);

            foreach (var erro in Cabecalho.ValidationResult.Errors)
                ValidationResult.Errors.Add(erro);

            foreach (var erro in ValorCarga.ValidationResult.Errors)
                ValidationResult.Errors.Add(erro);            
        }
    }
}
