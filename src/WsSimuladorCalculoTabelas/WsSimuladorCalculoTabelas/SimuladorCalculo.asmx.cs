using System;
using System.Linq;
using System.Web.Services;
using WsSimuladorCalculoTabelas.DAO;
using WsSimuladorCalculoTabelas.Models;
using WsSimuladorCalculoTabelas.Requests;
using WsSimuladorCalculoTabelas.Responses;
using WsSimuladorCalculoTabelas.Services;

namespace WsSimuladorCalculoTabelas
{
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]

    public class SimuladorCalculo : WebService
    {
        private readonly TabelasDAO _tabelasDAO;
        private readonly SimuladorDAO _simuladorDAO;
        private readonly OportunidadeDAO _oportunidadeDAO;

        public SimuladorCalculo()
        {
            _tabelasDAO = new TabelasDAO(true);
            _simuladorDAO = new SimuladorDAO(true);
            _oportunidadeDAO = new OportunidadeDAO();
        }

        [WebMethod(Description = "Retorna uma lista de tabelas de cobrança a partir dos parâmetros especificados")]
        public Response ObterTabelas(
            int clienteId,
            string clienteCnpj,
            int classeCliente,
            int simuladorId,
            bool crm,
            bool calculoAutomatico,
            int tabelaId)
        {
            try
            {
                var response = _tabelasDAO.ObterTabelas(
                    new TabelasRequest
                    {
                        ClienteId = clienteId,
                        ClienteCnpj = clienteCnpj,
                        ClasseCliente = classeCliente,
                        SimuladorId = simuladorId,
                        CRM = crm,
                        CalculoAutomatico = calculoAutomatico,
                        TabelaId = tabelaId
                    }).ToArray();

                return new Response
                {
                    Sucesso = true,
                    Lista = response
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    Sucesso = false,
                    Mensagem = ex.ToString()
                };
            }
        }

        [WebMethod(Description = "Calcula a estimativa dos valores das tabelas de cobranças especificadas")]
        public Response CalcularTabelas(int simuladorId, string tabelas, bool crm)
        {
            try
            {
                if (simuladorId == 0)
                    throw new Exception("ID do Simulador não informado");

                var service = new CalculoSimuladorService(false);

                int[] tabelasArray = Array.ConvertAll<string, int>(tabelas.Split(','), int.Parse);

                if (tabelasArray.Length == 0)
                    throw new Exception("Nenhuma Tabela de Cobrança informada");

                service.CalcularTabelas(new CalculoRequest
                {
                    SimuladorId = simuladorId,
                    Tabelas = tabelasArray,
                    CRM = crm
                });

                return new Response
                {
                    Sucesso = true
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    Sucesso = false,
                    Mensagem = ex.ToString()
                };
            }
        }

        [WebMethod(Description = "Retorna o ID do Cliente quando o Simulador não possui Classe")]
        public int ObterClienteSimulador(int simuladorId)
        {
            try
            {
                return _simuladorDAO.ObterClienteSimulador(simuladorId);
            }
            catch
            {
                return -1;
            }
        }

        [WebMethod(Description = "Gera um relatório Excel com a estimativa de valores")]
        public Response GerarRelatorioExcel(int simuladorId, bool comAnaliseDeDados, bool servicosComplementares, bool dadosDoCliente, bool somenteEstimativa, string dataPgtoInicial, string dataPgtoFinal, bool crm, bool vertical)
        {
            try
            {
                if (vertical)
                {
                    var excel = new CalculoTabelasExcelService();

                    var response = excel.GerarColuna(new GerarExcelFiltro
                    {
                        SimuladorId = simuladorId,
                        ComAnaliseDeDados = false,
                        ServicosComplementares = servicosComplementares,
                        DadosDoCliente = dadosDoCliente,
                        SomenteEstimativa = false,
                        DataPgtoInicial = dataPgtoInicial,
                        DataPgtoFinal = dataPgtoFinal,
                        CRM = crm
                    });

                    return response;                    
                }
                else
                {
                    var excel = new CalculoTabelasExcelService();

                    var response = excel.Gerar(new GerarExcelFiltro
                    {
                        SimuladorId = simuladorId,
                        ComAnaliseDeDados = comAnaliseDeDados,
                        ServicosComplementares = servicosComplementares,
                        DadosDoCliente = dadosDoCliente,
                        SomenteEstimativa = somenteEstimativa,
                        DataPgtoInicial = dataPgtoInicial,
                        DataPgtoFinal = dataPgtoFinal,
                        CRM = crm
                    });

                    return response;
                }
            }
            catch (Exception ex)
            {
                return new Response
                {
                    Sucesso = false,
                    Mensagem = $"Falha ao gerar a planilha. {ex.Message}"
                };
            }
        }

        [WebMethod(Description = "Inicia uma simulação a partir de uma Oportunidade")]
        public Response SimuladorOportunidade(int oportunidadeId, int simuladorParametroId, int modeloSimuladorId, int usuarioId)
        {
            try
            {
                var oportunidade = _oportunidadeDAO.ObterOportunidadePorId(oportunidadeId);

                if (oportunidade == null)
                {
                    return new Response
                    {
                        Sucesso = false,
                        Mensagem = $"Oportunidade {oportunidadeId} não encontrada"
                    };
                }

                var response = new SimuladorCRMService(oportunidadeId, simuladorParametroId, modeloSimuladorId, usuarioId)
                   .Iniciar();

                return new Response
                {
                    Sucesso = true,
                    Mensagem = response.Mensagem,
                    ArquivoId = response.ArquivoId,
                    Base64 = response.Base64,
                    NomeArquivo = response.NomeArquivo,
                    Hash = response.Hash,
                    TamanhoArquivo = response.TamanhoArquivo
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    Sucesso = false,
                    Mensagem = $"Falha ao simular a Oportunidade. {ex.Message}"
                };
            }
        }
    }
}