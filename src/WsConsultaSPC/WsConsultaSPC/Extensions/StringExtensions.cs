using System;

namespace WsConsultaSPC
{
    public static class StringExtensions
    {
        public static int ToInt(this string valor)
        {
            if (Int32.TryParse(valor, out _))
                return Convert.ToInt32(valor);

            return 0;
        }

        public static string RemoverCaracteresEspeciaisDocumento(this string valor)
        {
            return valor.Replace(".", "").Replace("/", "").Replace("-", "");
        }
    }
}