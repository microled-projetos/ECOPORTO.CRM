using Ecoporto.CRM.Business.Enums;
using Ecoporto.CRM.Business.Models;
using FluentValidation;

namespace Ecoporto.CRM.Business.ValueObjects
{
    public class ValorCarga : Entidade<ValorCarga>
    {       
        public ValorCarga(decimal valor20, decimal valor40)
        {
            Valor20 = valor20;
            Valor40 = valor40;

            Validar();
        }

        public ValorCarga(decimal valor, decimal valor20, decimal valor40, TipoCarga tipoCarga)
        {
            Valor = valor;
            Valor20 = valor20;
            Valor40 = valor40;
            TipoCarga = tipoCarga;

            Validar();
        }

        public decimal Valor { get; set; }

        public decimal Valor20 { get; set; }

        public decimal Valor40 { get; set; }

        public TipoCarga TipoCarga { get; set; }

        public override void Validar()
        {
            ValidationResult = Validate(this);
        }
    }
}
