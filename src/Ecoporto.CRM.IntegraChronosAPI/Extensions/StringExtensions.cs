using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Ecoporto.CRM.IntegraChronosAPI.Extensions
{
    public static class StringExtensions
    {
        public static int ToInt(this string valor)
        {
            if (Int32.TryParse(valor, out _))
                return Convert.ToInt32(valor);

            return 0;
        }

        public static decimal ToDecimal(this string valor)
        {
            if (Decimal.TryParse(valor, out _))
                return Convert.ToDecimal(valor);

            return 0;
        }
    }
}
