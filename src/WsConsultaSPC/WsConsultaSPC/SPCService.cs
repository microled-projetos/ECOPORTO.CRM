using System;
using WsConsultaSPC.WsSPC;

namespace WsConsultaSPC
{
    public class SPCService
    {
        private readonly string _url;
        private readonly string _usuario;
        private readonly string _senha;

        public SPCService(string url, string usuario, string senha)
        {
            _url = url;
            _usuario = usuario;
            _senha = senha;
        }

        public WsSPC.ResultadoConsulta Consultar(TipoPessoaResponse tipo, string documento, string codigoProduto)
        {
            TipoPessoa tipoPessoa = tipo == TipoPessoaResponse.PessoaFisica
                ? WsSPC.TipoPessoa.F
                : WsSPC.TipoPessoa.J;

            var binding = new System.ServiceModel.BasicHttpsBinding();
            binding.MaxReceivedMessageSize = 65536 * 30;
            binding.Security.Transport.ClientCredentialType = System.ServiceModel.HttpClientCredentialType.Basic;
            var address = new System.ServiceModel.EndpointAddress(_url);
            using (var serv = new WsSPC.consultaWebServiceClient(binding, address))
            {
                serv.ClientCredentials.UserName.UserName = _usuario;
                serv.ClientCredentials.UserName.Password = _senha;
                serv.Open();

                var filtro = new WsSPC.FiltroConsulta
                {
                    codigoproduto = codigoProduto,
                    tipoconsumidor = tipoPessoa,
                    tipoconsumidorSpecified = true,
                    documentoconsumidor = documento
                };

                try
                {
                    return serv.consultar(filtro);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }
    }
}