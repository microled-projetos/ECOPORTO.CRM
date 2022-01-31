using Ecoporto.CRM.Business.Enums;
using Ecoporto.CRM.Business.Models;
using FluentValidation;

namespace Ecoporto.CRM.Business.ValueObjects
{
    public class ValorCargaMinimo : Entidade<ValorCargaMinimo>
    {       
        public ValorCargaMinimo(decimal valorMinimo20, decimal valorMinimo40)
        {
            ValorMinimo20 = valorMinimo20;
            ValorMinimo40 = valorMinimo40;

            Validar();
        }

        public ValorCargaMinimo(decimal valorMinimo, decimal valorMinimo20, decimal valorMinimo40, TipoCarga tipoCarga, BaseCalculo baseCalculo)
        {
            ValorMinimo = valorMinimo;
            ValorMinimo20 = valorMinimo20;
            ValorMinimo40 = valorMinimo40;
            TipoCarga = tipoCarga;
            BaseCalculo = baseCalculo;

       Validar();
        }

        public decimal ValorMinimo { get; set; }

        public decimal ValorMinimo20 { get; set; }

        public decimal ValorMinimo40 { get; set; }

        public TipoCarga TipoCarga { get; set; }
        public BaseCalculo  BaseCalculo { get; set; }
        public override void Validar()
        {
            ValidationResult = Validate(this);
        }
    }
}
