using Ecoporto.CRM.Business.Enums;
using Ecoporto.CRM.Infra.Services;
using System;

namespace Ecoporto.CRM.Testes
{
    class Program
    {
        static void Main(string[] args)
        {
            //string wsSPCUrl = "https://treina.spc.org.br/spc/remoting/ws/consulta/consultaWebService";
            //string wsSPCUsuario = "400136";
            //string wsSPCSenha = "27072020";

            //SPCService service = new SPCService(wsSPCUrl, wsSPCUsuario, wsSPCSenha);

            //var retorno = service.Consultar(ClassificacaoFiscal.PJ, "02178451000149");

            var binding = new System.ServiceModel.BasicHttpsBinding();
            binding.MaxReceivedMessageSize = 65536 * 30;
            binding.Security.Transport.ClientCredentialType = System.ServiceModel.HttpClientCredentialType.Basic;
            var address = new System.ServiceModel.EndpointAddress("https://treina.spc.org.br/spc/remoting/ws/consulta/consultaWebService");
            using (var serv = new WsSPCTeste.consultaWebServiceClient(binding, address))
            {
                serv.ClientCredentials.UserName.UserName = "400136";
                serv.ClientCredentials.UserName.Password = "27072020";
                serv.Open();

                //var result = serv.sobreWebService();

                var filtro = new WsSPCTeste.FiltroConsulta
                {
                    codigoproduto = "198",
                    tipoconsumidor = WsSPCTeste.TipoPessoa.J,
                    tipoconsumidorSpecified=true,
                    documentoconsumidor = "02178451000149"
                };

                try
                {
                    var res = serv.consultar(filtro);
                    Console.ReadKey();
                }
                catch (Exception ex)
                {

                    throw;
                }
               
                //var res = serv.listarProdutos();

                Console.ReadKey();
            }


            //WsSPCTeste.consultaWebServiceClient ws = new WsSPCTeste.consultaWebServiceClient();
            //ws.ClientCredentials.UserName.UserName = "400136";
            //    ws.ClientCredentials.UserName.Password = "27072020";
            //var res = ws.consultar(new WsSPCTeste.FiltroConsulta
            //{
            //    codigoproduto = "198",
            //    tipoconsumidor = WsSPCTeste.TipoPessoa.J,
            //    documentoconsumidor = "02178451000149"
            //});

            Console.ReadKey();
        }
    }
}
