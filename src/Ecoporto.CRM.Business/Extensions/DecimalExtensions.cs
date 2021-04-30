using Ecoporto.CRM.Business.Enums;
using System;
using System.Globalization;

namespace Ecoporto.CRM.Business.Extensions
{
    public static class DecimalExtensions
    {
        public static string ToNumero(this decimal valor)
        {                      
            if (Decimal.TryParse(valor.ToString(), out decimal resultado))
            {
                var casasDecimais = resultado.ObterCasasDecimais();

                return (casasDecimais == 0 || casasDecimais == 1)
                    ? string.Format("{0:N2}", resultado)
                    : string.Format("{0:N" + casasDecimais + "}", resultado);
            }

            return string.Empty;
        }

        public static string ToMoeda(this decimal texto, Moeda moeda)
        {
            if (Decimal.TryParse(texto.ToString(), out decimal resultado))
            {                
                if (moeda == Moeda.REAL)
                    return string.Format("{0:C2}", resultado);

                return string.Format(CultureInfo.GetCultureInfo("en-US"), "U{0:C2}", resultado);
            }

            return string.Empty;
        }

        public static decimal ToPercentual(this decimal valor)
        {
            if (Decimal.TryParse(valor.ToString(), out decimal resultado))
                return valor / 100;

            return 0;
        }
    }
}
