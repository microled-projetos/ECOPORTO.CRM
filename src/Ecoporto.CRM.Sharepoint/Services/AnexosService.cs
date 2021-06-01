using Ecoporto.CRM.Business.Helpers;
using Ecoporto.CRM.Sharepoint.Models;
using RestSharp;
using System;

namespace Ecoporto.CRM.Sharepoint.Services
{
    public class AnexosService
    {
        private readonly Token _token;

        public AnexosService(Token token)
            => _token = token;

        public RetornoServicoUpload EnviarArquivo(DadosArquivoUpload dadosArquivoUpload)
        {
            RestClient client = new RestClient
            {
                BaseUrl = new Uri(Parametros.BaseUrl + "api/v1/file/save"),
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
                BaseUrl = new Uri(Parametros.BaseUrl + "api/v1/file/getFileById"),
                Timeout = 180000
            };

            RestRequest request = new RestRequest(Method.POST);

            request.AddParameter("Authorization", string.Format("Bearer " + _token.access_token), ParameterType.HttpHeader);
            request.AddHeader("Content-Type", "application/json");
            request.RequestFormat = DataFormat.Json;

            var body = new
            {
                ID = Converters.RawToGuid(id),
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
                BaseUrl = new Uri(Parametros.BaseUrl + "api/v1/file/DeleteById"),
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
    }
}