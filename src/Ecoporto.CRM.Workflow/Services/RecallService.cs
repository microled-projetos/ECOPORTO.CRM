using Ecoporto.CRM.Workflow.Models;
using Newtonsoft.Json;
using RestSharp;
using System;

namespace Ecoporto.CRM.Workflow.Services
{
    public class RecallService
    {
        private readonly Token _token;

        public RecallService(Token token)
            => _token = token;

        public RetornoWorkflow Recall(CadastroRecall recall)
        {
            RestClient client = new RestClient
            {
                BaseUrl = new Uri(Parametros.BaseUrl + $"api/v1/WorkFlow/Cancelar"),
                Timeout = 180000
            };

            RestRequest request = new RestRequest(Method.POST);

            request.AddParameter("Authorization", string.Format("Bearer " + _token.access_token), ParameterType.HttpHeader);
            request.AddHeader("Content-Type", "application/json");
            request.RequestFormat = DataFormat.Json;

            request.AddBody(recall);

            var response = client.Execute<RetornoWorkflow>(request);

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                try
                {
                    var objErro = JsonConvert.DeserializeObject<RetornoWorkflow>(response.Content);

                    return new RetornoWorkflow
                    {
                        sucesso = false,
                        mensagem = objErro.mensagem
                    };
                }
                catch
                {
                }
            }

            return response.Data;
        }
    }
}
