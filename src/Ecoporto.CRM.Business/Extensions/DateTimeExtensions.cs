using System;

namespace Ecoporto.CRM.Business.Extensions
{
    public static class DateTimeExtensions
    {
        public static string DataFormatada(this object valor)
        {
            if (valor == null)
                return string.Empty;

            if (DateTime.TryParse(valor.ToString(), out DateTime resultado))
            {
                if (resultado <= DateTime.MinValue)                
                    return string.Empty;
                
                return resultado.ToString("dd/MM/yyyy");
            }

            return string.Empty;
        }

        public static string DataHoraFormatada(this object valor)
        {
            if (valor == null)
                return string.Empty;

            if (DateTime.TryParse(valor.ToString(), out _))
                return Convert.ToDateTime(valor.ToString()).ToString("dd/MM/yyyy HH:mm");

            return string.Empty;
        }
    }
}
