using Ecoporto.CRM.Business.Enums;
using Ecoporto.CRM.Business.ValueObjects;
using FluentValidation;
using System.Collections.Generic;

namespace Ecoporto.CRM.Business.Models
{
    public class LayoutArmazenagemMinimo : Entidade<LayoutArmazenagemMinimo>
    {
        public Cabecalho Cabecalho { get; set; }
        
        public int ServicoId { get; set; }
        
        public ValorCargaMinimo ValorCarga { get; set; }

        public int LinhaReferencia { get; set; }

        public string DescricaoValor { get; set; }

        public int LimiteBls { get; set; }

        public Margem Margem { get; set; }

        public IEnumerable<FaixaBL> FaixasBL { get; set; }

        public LayoutArmazenagemMinimo()
        {

        }

        public LayoutArmazenagemMinimo(
            Cabecalho cabecalho, 
            int servicoId,
            ValorCargaMinimo valorCarga, 
            Margem margem,
            int linhaReferencia, 
            int limiteBls,
            string descricaoValor)
        {
            Cabecalho = cabecalho;
            ServicoId = servicoId;
            ValorCarga = valorCarga;
            Margem = margem;
            LinhaReferencia = linhaReferencia;
            LimiteBls = limiteBls;
            DescricaoValor = descricaoValor;

            FaixasBL = new List<FaixaBL>();            
        }

        public override void Validar()
        {          
            RuleFor(c => c.ServicoId)
               .GreaterThan(0)
               .WithMessage("Escolha o Serviço");

            ValidationResult = Validate(this);

            foreach (var erro in Cabecalho.ValidationResult.Errors)
                ValidationResult.Errors.Add(erro);

            foreach (var erro in ValorCarga.ValidationResult.Errors)
                ValidationResult.Errors.Add(erro);            
        }

    }
}
