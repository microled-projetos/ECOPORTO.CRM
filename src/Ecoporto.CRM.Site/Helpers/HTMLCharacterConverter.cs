using System;
using System.Collections.Generic;
using System.Text;

namespace Ecoporto.CRM.Site.Helpers
{
    public class HTMLCharacterConverter
    {
        private static Dictionary<char, string> dicionario = new Dictionary<char, string>();

        public HTMLCharacterConverter()
        {
            dicionario.Add('&', "&amp;");
            dicionario.Add('"', "&quot;");
            dicionario.Add('>', "&lt;");
            dicionario.Add('<', "&gt;");
            dicionario.Add('¡', "&iexcl;");
            dicionario.Add('¢', "&cent;");
            dicionario.Add('£', "&pound;");
            dicionario.Add('¤', "&curren;");
            dicionario.Add('¥', "&yen;");
            dicionario.Add('¦', "&brvbar;");
            dicionario.Add('§', "&sect;");
            dicionario.Add('¨', "&uml;");
            dicionario.Add('©', "&copy;");
            dicionario.Add('ª', "&ordf;");
            dicionario.Add('«', "&laquo;");
            dicionario.Add('¬', "&not;");
            dicionario.Add('\'', "&shy;");
            dicionario.Add('®', "&reg;");
            dicionario.Add('¯', "&macr;");
            dicionario.Add('°', "&deg;");
            dicionario.Add('±', "&plusmn;");
            dicionario.Add('²', "&sup2;");
            dicionario.Add('³', "&sup3;");
            dicionario.Add('´', "&acute;");
            dicionario.Add('µ', "&micro;");
            dicionario.Add('¶', "&para;");
            dicionario.Add('·', "&middot;");
            dicionario.Add('¸', "&cedil;");
            dicionario.Add('¹', "&sup1;");
            dicionario.Add('º', "&ordm;");
            dicionario.Add('»', "&raquo;");
            dicionario.Add('¼', "&frac14;");
            dicionario.Add('½', "&frac12;");
            dicionario.Add('¾', "&frac34;");
            dicionario.Add('¿', "&iquest;");
            dicionario.Add('À', "&Agrave;");
            dicionario.Add('Á', "&Aacute;");
            dicionario.Add('Â', "&Acirc;");
            dicionario.Add('Ã', "&Atilde;");
            dicionario.Add('Ä', "&Auml;");
            dicionario.Add('Å', "&Aring;");
            dicionario.Add('Æ', "&AElig;");
            dicionario.Add('Ç', "&Ccedil;");
            dicionario.Add('È', "&Egrave;");
            dicionario.Add('É', "&Eacute;");
            dicionario.Add('Ê', "&Ecirc;");
            dicionario.Add('Ë', "&Euml;");
            dicionario.Add('Ì', "&Igrave;");
            dicionario.Add('Í', "&Iacute;");
            dicionario.Add('Î', "&Icirc;");
            dicionario.Add('Ï', "&Iuml;");
            dicionario.Add('Ð', "&ETH;");
            dicionario.Add('Ñ', "&Ntilde;");
            dicionario.Add('Ò', "&Ograve;");
            dicionario.Add('Ó', "&Oacute;");
            dicionario.Add('Ô', "&Ocirc;");
            dicionario.Add('Õ', "&Otilde;");
            dicionario.Add('Ö', "&Ouml;");
            dicionario.Add('×', "&times;");
            dicionario.Add('Ø', "&Oslash;");
            dicionario.Add('Ù', "&Ugrave;");
            dicionario.Add('Ú', "&Uacute;");
            dicionario.Add('Û', "&Ucirc;");
            dicionario.Add('Ü', "&Uuml;");
            dicionario.Add('Ý', "&Yacute;");
            dicionario.Add('Þ', "&THORN;");
            dicionario.Add('ß', "&szlig;");
            dicionario.Add('à', "&agrave;");
            dicionario.Add('á', "&aacute;");
            dicionario.Add('â', "&acirc;");
            dicionario.Add('ã', "&atilde;");
            dicionario.Add('ä', "&auml;");
            dicionario.Add('å', "&aring;");
            dicionario.Add('æ', "&aelig;");
            dicionario.Add('ç', "&ccedil;");
            dicionario.Add('è', "&egrave;");
            dicionario.Add('é', "&eacute;");
            dicionario.Add('ê', "&ecirc;");
            dicionario.Add('ë', "&euml;");
            dicionario.Add('ì', "&igrave;");
            dicionario.Add('í', "&iacute;");
            dicionario.Add('î', "&icirc;");
            dicionario.Add('ï', "&iuml;");
            dicionario.Add(' ', "&eth;");
            dicionario.Add('ñ', "&ntilde;");
            dicionario.Add('ò', "&ograve;");
            dicionario.Add('ó', "&oacute;");
            dicionario.Add('ô', "&ocirc;");
            dicionario.Add('õ', "&otilde;");
            dicionario.Add('ö', "&ouml;");
            dicionario.Add('÷', "&divide;");
            dicionario.Add('ø', "&oslash;");
            dicionario.Add('ù', "&ugrave;");
            dicionario.Add('ú', "&uacute;");
            dicionario.Add('û', "&ucirc;");
            dicionario.Add('ü', "&uuml;");
            dicionario.Add('ý', "&yacute;");
            dicionario.Add('þ', "&thorn;");
            dicionario.Add('ÿ', "&yuml;");
        }

        public static string Encode(String pText)
        {
            StringBuilder htmlText = new StringBuilder();

            int ia = 0;

            Char c;

            for (int i = 0; i < pText.Length; i++)
            {
                c = pText[i];

                if (dicionario.ContainsKey(c))
                {
                    htmlText.Append(pText.Substring(ia, i - ia));
                    ia = i + 1;
                }
            }

            htmlText.Append(pText.Substring(ia));

            return htmlText.ToString();

        }

        public static String Decode(String pText)
        {
            string[] result = new string[2];

            result[0] = pText;
            result[1] = pText;

            foreach (KeyValuePair<char, string> item in dicionario)
            {
                if (result[1] != result[0])
                {
                    result[1] = result[0].Replace(item.Value, item.Key.ToString());
                    result[0] = result[1];
                }
                else
                {
                    result[0] = result[0].Replace(item.Value, item.Key.ToString());
                }
            }
            return result[0];
        }
    }
}