using System;

namespace Ecoporto.CRM.Business.Helpers
{
    public class StringHelpers
    {
        public static bool IsInteger(string input)
        {
            if (double.TryParse(input, out double valor))
                return valor == (int)valor && valor >= int.MinValue && valor <= int.MaxValue;

            return false;
        }

        public static bool IsDecimal(string valor)
        {
            if (Decimal.TryParse(valor, out _))
                return true;

            return false;
        }

        public static bool IsNumero(string valor)
        {
            return IsDecimal(valor);
        }     
    }
}
