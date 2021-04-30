using RestSharp;
using System;
using System.Configuration;
using WsSimuladorCalculoTabelas.Helpers;
using WsSimuladorCalculoTabelas.Models;

namespace WsSimuladorCalculoTabelas.Services
{
    public class UploadService
    {
        private readonly Token _token;

        public UploadService(Token token)
            => _token = token;

        public RetornoServicoUpload EnviarArquivo(DadosArquivoUpload dadosArquivoUpload)
        {
            RestClient client = new RestClient
            {
                BaseUrl = new Uri(string.Concat(ConfigurationManager.AppSettings["ApiAnexosUrl"].ToString(), "api/v1/file/save")),
                Timeout = 180000
            };

            RestRequest request = new RestRequest(Method.POST);

            request.AddParameter("Authorization", string.Format("Bearer " + _token.access_token), ParameterType.HttpHeader);
            request.AddHeader("Content-Type", "application/json");
            request.RequestFormat = DataFormat.Json;

            var body = new
            {
                name = dadosArquivoUpload.Name,
                extension = dadosArquivoUpload.Extension,
                system = dadosArquivoUpload.System,
                dataArray = dadosArquivoUpload.DataArray
            };

            request.AddBody(body);

            var response = client.Execute<RetornoServicoUpload>(request);

            return response.Data;
        }

        public RetornoServicoUpload ObterArquivo(string id)
        {
            RestClient client = new RestClient
            {
                BaseUrl = new Uri(string.Concat(ConfigurationManager.AppSettings["ApiAnexosUrl"].ToString(), "api/v1/file/getFileById")),
                Timeout = 180000
            };

            RestRequest request = new RestRequest(Method.POST);

            request.AddParameter("Authorization", string.Format("Bearer " + _token.access_token), ParameterType.HttpHeader);
            request.AddHeader("Content-Type", "application/json");
            request.RequestFormat = DataFormat.Json;

            var body = new
            {
                ID = GuidHelpers.RawToGuid(id),
                System = 3
            };

            request.AddBody(body);

            var response = client.Execute<RetornoServicoUpload>(request);

            return response.Data;
        }

        public RetornoServicoUpload ExcluirArquivo(string id)
        {
            RestClient client = new RestClient
            {
                BaseUrl = new Uri(string.Concat(ConfigurationManager.AppSettings["ApiAnexosUrl"].ToString(), "api/v1/file/DeleteById")),
                Timeout = 180000
            };

            RestRequest request = new RestRequest(Method.POST);

            request.AddParameter("Authorization", string.Format("Bearer " + _token.access_token), ParameterType.HttpHeader);
            request.AddHeader("Content-Type", "application/json");
            request.RequestFormat = DataFormat.Json;

            var body = new
            {
                ID = id,
                System = 3
            };

            request.AddBody(body);

            var response = client.Execute<RetornoServicoUpload>(request);

            return response.Data;
        }

        public static Token Autenticar()
        {
            RestClient client = new RestClient
            {
                BaseUrl = new Uri(string.Concat(ConfigurationManager.AppSettings["ApiAnexosUrl"].ToString(), "api/v1/Authenticate/Access?authentication=1&system=3&domain=ecoportocorp")),
                Timeout = 180000
            };

            var request = new RestRequest(Method.POST);

            string conteudo = string.Format("grant_type=password&username={0}&password={1}", ConfigurationManager.AppSettings["ApiAnexosUsuario"].ToString(), ConfigurationManager.AppSettings["ApiAnexosSenha"].ToString());

            request.AddParameter("application/x-www-form-urlencoded", conteudo, ParameterType.RequestBody);
            request.AddParameter("Content-Type", "application/x-www-form-urlencoded", ParameterType.HttpHeader);

            var response = client.Execute<Token>(request);

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
                return null;

            return response.Data;
        }
    }
}