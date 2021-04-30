using Ecoporto.CRM.Business.Helpers;
using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Ecoporto.CRM.Business.Extensions
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

        public static decimal ToDBDecimal(this string valor)
        {
            if (Decimal.TryParse(valor, out _))
                return Convert.ToDecimal(valor, CultureInfo.InvariantCulture);

            return 0;
        }

        public static decimal ToPercentual(this string valor)
        {
            if (Decimal.TryParse(valor, out _))
                return Convert.ToDecimal(valor) / 100;

            return 0;
        }

        public static DateTime? ToNullabeDateTime(this string valor)
        {
            if (DateTimeHelpers.IsDate(valor))
                return Convert.ToDateTime(valor);

            return null;
        }

        public static double ToDouble(this string valor)
        {
            if (Double.TryParse(valor, out _))
                return Convert.ToDouble(valor);

            return 0;
        }

        public static bool ToBoolean(this string valor)
        {
            if (Boolean.TryParse(valor, out _))
                return true;

            return false;
        }

        public static string RemoverCaracteresEspeciais(this string valor)
        {
            return Regex.Replace(valor, @"\r\n?|\n", "");
        }

        public static string RemoverCaracteresEspeciaisDocumento(this string valor)
        {
            return valor.Replace(".", "").Replace("/", "").Replace("-", "");
        }

        public static string ToNumero(this string texto)
        {
            if (string.IsNullOrEmpty(texto))
                texto = "0";

            if (Decimal.TryParse(texto, out decimal resultado))
            {
                var casasDecimais = resultado.ObterCasasDecimais();

                return (casasDecimais == 0 || casasDecimais == 1)
                    ? string.Format("{0:N2}", resultado)
                    : string.Format("{0:N" + casasDecimais + "}", resultado);
            }

            return string.Empty;
        }

        public static string ToMoeda(this string texto)
        {
            if (string.IsNullOrEmpty(texto))
                texto = "0";

            if (Decimal.TryParse(texto, out decimal resultado))
            {
                var valorDecimal = Convert.ToDecimal(texto, CultureInfo.InvariantCulture);

                return string.Format("{0:C2}", valorDecimal);
            }

            return string.Empty;
        }

        public static byte[] ToByteArray(this string texto)
        {
            return Encoding.ASCII.GetBytes(texto);
        }

        public static int ObterCasasDecimais(this decimal n)
        {
            n = Math.Abs(n);
            n -= (int)n;
            var casasDecimais = 0;
            while (n > 0)
            {
                casasDecimais++;
                n *= 10;
                n -= (int)n;
            }
            return casasDecimais;
        }

        public static string CorrigirNomeArquivo(this string valor)
        {
            var valorCorrigido = valor;

            valorCorrigido = Regex.Replace(valorCorrigido, " ", "_");
            valorCorrigido = Regex.Replace(valorCorrigido, "/", "_");
            valorCorrigido = Regex.Replace(valorCorrigido, @"\\", "_");
            valorCorrigido = Regex.Replace(valorCorrigido, ":", "_");
            valorCorrigido = Regex.Replace(valorCorrigido, ",", "_");
            valorCorrigido = Regex.Replace(valorCorrigido, @"\.", "_");
            valorCorrigido = Regex.Replace(valorCorrigido, @"\*", "_");
            valorCorrigido = Regex.Replace(valorCorrigido, @"\?", "_");
            valorCorrigido = Regex.Replace(valorCorrigido, @"""", "_");
            valorCorrigido = Regex.Replace(valorCorrigido, "<", "_");
            valorCorrigido = Regex.Replace(valorCorrigido, ">", "_");
            valorCorrigido = Regex.Replace(valorCorrigido, "\n", "_");
            valorCorrigido = Regex.Replace(valorCorrigido, "\t", "_");
            valorCorrigido = Regex.Replace(valorCorrigido, @"\|", "_");
            valorCorrigido = Regex.Replace(valorCorrigido, "-", "_");
            valorCorrigido = Regex.Replace(valorCorrigido, @"\(", "_");
            valorCorrigido = Regex.Replace(valorCorrigido, @"\)", "_");
            valorCorrigido = Regex.Replace(valorCorrigido, "____", "_");
            valorCorrigido = Regex.Replace(valorCorrigido, "___", "_");
            valorCorrigido = Regex.Replace(valorCorrigido, "__", "_");

            return valorCorrigido;
        }

        public static string EscreverValorPorExtenso(this decimal valor)
        {
            if (valor <= 0)
                return string.Empty;
            else
            {
                string montagem = string.Empty;
                if (valor > 0 & valor < 1)
                {
                    valor *= 100;
                }
                string strValor = valor.ToString("000");
                int a = Convert.ToInt32(strValor.Substring(0, 1));
                int b = Convert.ToInt32(strValor.Substring(1, 1));
                int c = Convert.ToInt32(strValor.Substring(2, 1));
                if (a == 1) montagem += (b + c == 0) ? "Cem" : "Cento";
                else if (a == 2) montagem += "Duzentos";
                else if (a == 3) montagem += "Trezentos";
                else if (a == 4) montagem += "Quatrocentos";
                else if (a == 5) montagem += "Quinhentos";
                else if (a == 6) montagem += "Seiscentos";
                else if (a == 7) montagem += "Setecentos";
                else if (a == 8) montagem += "Oitocentos";
                else if (a == 9) montagem += "Novecentos";
                if (b == 1)
                {
                    if (c == 0) montagem += ((a > 0) ? " e " : string.Empty) + "Dez";
                    else if (c == 1) montagem += ((a > 0) ? " e " : string.Empty) + "Onze";
                    else if (c == 2) montagem += ((a > 0) ? " e " : string.Empty) + "Doze";
                    else if (c == 3) montagem += ((a > 0) ? " e " : string.Empty) + "Treze";
                    else if (c == 4) montagem += ((a > 0) ? " e " : string.Empty) + "Quatorze";
                    else if (c == 5) montagem += ((a > 0) ? " e " : string.Empty) + "Quinze";
                    else if (c == 6) montagem += ((a > 0) ? " e " : string.Empty) + "Dezesseis";
                    else if (c == 7) montagem += ((a > 0) ? " e " : string.Empty) + "Dezessete";
                    else if (c == 8) montagem += ((a > 0) ? " e " : string.Empty) + "Dezoito";
                    else if (c == 9) montagem += ((a > 0) ? " e " : string.Empty) + "Dezenove";
                }
                else if (b == 2) montagem += ((a > 0) ? " e " : string.Empty) + "Vinte";
                else if (b == 3) montagem += ((a > 0) ? " e " : string.Empty) + "Trinta";
                else if (b == 4) montagem += ((a > 0) ? " e " : string.Empty) + "Quarenta";
                else if (b == 5) montagem += ((a > 0) ? " e " : string.Empty) + "Cinquenta";
                else if (b == 6) montagem += ((a > 0) ? " e " : string.Empty) + "Sessenta";
                else if (b == 7) montagem += ((a > 0) ? " e " : string.Empty) + "Setenta";
                else if (b == 8) montagem += ((a > 0) ? " e " : string.Empty) + "Oitenta";
                else if (b == 9) montagem += ((a > 0) ? " e " : string.Empty) + "Noventa";
                if (strValor.Substring(1, 1) != "1" & c != 0 & montagem != string.Empty) montagem += " e ";
                if (strValor.Substring(1, 1) != "1")
                    if (c == 1) montagem += "Um";
                    else if (c == 2) montagem += "Dois";
                    else if (c == 3) montagem += "Três";
                    else if (c == 4) montagem += "Quatro";
                    else if (c == 5) montagem += "Cinco";
                    else if (c == 6) montagem += "Seis";
                    else if (c == 7) montagem += "Sete";
                    else if (c == 8) montagem += "Oito";
                    else if (c == 9) montagem += "Nove";
                return montagem;
            }
        }
    }
}
