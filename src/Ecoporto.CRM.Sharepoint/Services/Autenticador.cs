using Ecoporto.CRM.Sharepoint.Models;
using RestSharp;
using System;

namespace Ecoporto.CRM.Sharepoint.Services
{
    public class Autenticador
    {
        public static Token Autenticar()
        {
            RestClient client = new RestClient
            {
                BaseUrl = new Uri(Parametros.BaseUrl + "api/v1/Authenticate/Access?authentication=1&system=3&domain=ecoportocorp"),
                Timeout = 180000
            };

            var request = new RestRequest(Method.POST);

            string conteudo = string.Format("grant_type=password&username={0}&password={1}", Parametros.Usuario, Parametros.Senha);

            request.AddParameter("application/x-www-form-urlencoded", conteudo, ParameterType.RequestBody);
            request.AddParameter("Content-Type", "application/x-www-form-urlencoded", ParameterType.HttpHeader);

            var response = client.Execute<Token>(request);

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
                return null;

            return response.Data;
        }
    }
}
