using System;

namespace Ecoporto.CRM.Business.Helpers
{
    public static class Converters
    {
        public static string RawToGuid(string text)
        {
            byte[] bytes = ParseHex(text);
            Guid guid = new Guid(bytes);
            return guid.ToString("N").ToUpperInvariant();
        }

        static byte[] ParseHex(string text)
        {
            byte[] ret = new byte[text.Length / 2];

            for (int i = 0; i < ret.Length; i++)
            {
                ret[i] = Convert.ToByte(text.Substring(i * 2, 2), 16);
            }

            return ret;
        }

        public static string GuidToRaw(string text)
        {
            string result;
            if (string.IsNullOrWhiteSpace(text))
            {
                result = null;
            }
            else
            {
                Guid guid = new Guid(text);

                result = BitConverter.ToString(guid.ToByteArray()).Replace("-", "");
            }

            return result;
        }
    }
}
