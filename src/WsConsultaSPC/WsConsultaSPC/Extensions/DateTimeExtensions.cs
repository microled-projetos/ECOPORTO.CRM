using System;

namespace WsConsultaSPC
{
    public static class DateTimeExtensions
    {
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