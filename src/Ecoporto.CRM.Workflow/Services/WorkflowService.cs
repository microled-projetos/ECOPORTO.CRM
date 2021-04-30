using Ecoporto.CRM.Workflow.Models;
using RestSharp;
using System;

namespace Ecoporto.CRM.Workflow.Services
{
    public class WorkflowService
    {
        private readonly Token _token;

        public WorkflowService(Token token)
            => _token = token;

        public RetornoWorkflow EnviarParaAprovacao(CadastroWorkflow cadastroWorkflow)
        {
            RestClient client = new RestClient
            {
                BaseUrl = new Uri(Parametros.BaseUrl + "api/v1/WorkFlow/Cadastrar"),
                Timeout = 480000
            };

            RestRequest request = new RestRequest(Method.POST);

            request.AddParameter("Authorization", string.Format("Bearer " + _token.access_token), ParameterType.HttpHeader);
            request.AddHeader("Content-Type", "application/json");
            request.RequestFormat = DataFormat.Json;

            var body = new
            {
                id_Processo = Convert.ToInt32(cadastroWorkflow.Id_Processo),
                cadastroWorkflow.Id_Empresa,
                cadastroWorkflow.Autonum_Sistema_Processo_Requisitante,
                cadastroWorkflow.Usuario_Requisitante_Login,
                cadastroWorkflow.Usuario_Requisitante_Nome,
                cadastroWorkflow.Usuario_Requisitante_Email,
                cadastroWorkflow.ObjCampos
            };

            request.AddBody(body);

            var response = client.Execute<RetornoWorkflow>(request);

            return response.Data;
        }

        public RetornoHistoricoWorkflow ObterHistoricoWorkflow(int oportunidadeId, int processo, int? empresa = 1)
        {
            RestClient client = new RestClient
            {
                BaseUrl = new Uri(Parametros.BaseUrl + $"api/v1/WorkFlow/ConsultarHistoricoAprovacaoPorAutonumSistemaProcesso/?autonum={oportunidadeId}&id_Processo={processo}&id_Empresa={empresa}"),
                Timeout = 480000
            };

            RestRequest request = new RestRequest(Method.GET);

            request.AddParameter("Authorization", string.Format("Bearer " + _token.access_token), ParameterType.HttpHeader);
            request.AddHeader("Content-Type", "application/json");
            request.RequestFormat = DataFormat.Json;
           
            var response = client.Execute<RetornoHistoricoWorkflow>(request);

            return response.Data;
        }

        public RetornoFilaWorkflow ObterFilaWorkflow()
        {
            RestClient client = new RestClient
            {
                BaseUrl = new Uri(Parametros.BaseUrl + $"api/v1/WorkFlow/ConsultarFilaStatus/?id_Empresa=0"),
                Timeout = 480000
            };

            RestRequest request = new RestRequest(Method.GET);

            request.AddParameter("Authorization", string.Format("Bearer " + _token.access_token), ParameterType.HttpHeader);

            var response = client.Execute<RetornoFilaWorkflow>(request);

            return response.Data;
        }

        public RetornoOportunidadeAtualizada EnviarRetornoRegistroAtualizado(int id)
        {
            RestClient client = new RestClient
            {
                BaseUrl = new Uri(Parametros.BaseUrl + $"api/v1/WorkFlow/RetornoLeituraFilaStatusPorId/?id=" + id),
                Timeout = 480000
            };

            RestRequest request = new RestRequest(Method.GET);

            request.AddParameter("Authorization", string.Format("Bearer " + _token.access_token), ParameterType.HttpHeader);

            var response = client.Execute<RetornoOportunidadeAtualizada>(request);

            return response.Data;
        }        
    }
}
