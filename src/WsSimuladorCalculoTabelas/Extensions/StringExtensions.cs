using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace WsSimuladorCalculoTabelas.Extensions
{
    public static class StringExtensions
    {
        public static int ToInt(this string valor)
        {
            if (Int32.TryParse(valor, out _))
                return Convert.ToInt32(valor);

            return 0;
        }

        public static string Capitalize(this string valor)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(valor.ToLower());
        }

        public static string RemoverCaracteresEspeciais(this string valor)
        {
            return Regex.Replace(valor ?? string.Empty, "[^0-9a-zA-Z]+", "");
        }

        public static string SubstituirCaracteresEspeciais(this string valor, string caractereSubstituicao)
        {
            return Regex.Replace(valor ?? string.Empty, "[^0-9a-zA-Z]+", caractereSubstituicao);
        }

        public static string PPonto(this string valor)
        {
            return valor.Replace(".", "").Replace(",", ".");
        }

        public static string ObterPrimeiroNome(this string valor)
        {
            if (string.IsNullOrEmpty(valor))
                return string.Empty;

            var nome = valor.Split(' ');

            if (nome.Length > 0)
                return nome[0];

            return valor;
        }

        public static string RemoverUltimoCaractere(this string valor)
        {
            if (valor.Length > 1)
                return valor.Remove(valor.Length - 1);

            return string.Empty;
        }
    }
}