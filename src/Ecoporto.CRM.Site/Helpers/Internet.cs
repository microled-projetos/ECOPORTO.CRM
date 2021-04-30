using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;

namespace Ecoporto.CRM.Site.Helpers
{
    public class Internet
    {       
        public static bool AcessoViaInternet(out string i)
        {
            bool ret = false;

            i = "";

            try
            {
                i = HttpContext.Current.Request.UserHostAddress;

                if (string.IsNullOrEmpty(i))
                {
                    i = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                }

                if (string.IsNullOrEmpty(i))
                {
                    i = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }

                else
                {
                    i = i.Split(',')[0];
                }

                if (!string.IsNullOrEmpty(i) && (i.Trim() == "::1" || i.Trim().StartsWith("127.") || i.Trim().StartsWith("10.")))
                {
                    ret = false;
                }
                else if (!string.IsNullOrEmpty(i))
                {
                    string regex = @"(^192\.168\.([0-9]|[0-9][0-9]|[0-2][0-5][0-5])\.([0-9]|[0-9][0-9]|[0-2][0-5][0-5])$)|(^172\.([1][6-9]|[2][0-9]|[3][0-1])\.([0-9]|[0-9][0-9]|[0-2][0-5][0-5])\.([0-9]|[0-9][0-9]|[0-2][0-5][0-5])$)|(^10\.([0-9]|[0-9][0-9]|[0-2][0-5][0-5])\.([0-9]|[0-9][0-9]|[0-2][0-5][0-5])\.([0-9]|[0-9][0-9]|[0-2][0-5][0-5])$)";

                    var match = System.Text.RegularExpressions.Regex.Match(i.Trim(), regex, System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                    if (match.Success)
                    {
                        ret = false;
                    }
                    else
                        ret = true;
                }
                else
                    i = "";
            }
            catch
            {
                i = "";
                ret = false;
            }

            return ret;
        }

        public static bool ContemIP(string ipInicial, string ipFinal, string address)
        {
            long inicio = BitConverter.ToInt32(IPAddress.Parse(ipInicial)
                .GetAddressBytes()
                .Reverse()
                .ToArray(), 0);

            long fim = BitConverter.ToInt32(IPAddress.Parse(ipFinal)
                .GetAddressBytes()
                .Reverse()
                .ToArray(), 0);

            long ip = BitConverter.ToInt32(IPAddress.Parse(address)
                .GetAddressBytes()
                .Reverse()
                .ToArray(), 0);

            return ip >= inicio && ip <= fim;
        }
    }
}