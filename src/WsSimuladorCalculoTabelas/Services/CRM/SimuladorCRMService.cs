using System;
using WsSimuladorCalculoTabelas.Configuracao;
using WsSimuladorCalculoTabelas.DAO;
using WsSimuladorCalculoTabelas.Enums;
using WsSimuladorCalculoTabelas.Models;
using WsSimuladorCalculoTabelas.Responses;

namespace WsSimuladorCalculoTabelas.Services
{
    public class SimuladorCRMService
    {
        private readonly OportunidadeDAO _oportunidadeDAO;
        private readonly ModeloDAO _modeloDAO;
        private readonly SimuladorDAO _simuladorDAO;
        private readonly TabelasDAO _tabelaDAO;
        private readonly ExportaTabelasService _exportaTabelasService;
        private readonly int _oportunidadeId;
        private readonly int _usuarioId;
        private readonly int _simuladorParametroId;
        private readonly int _modeloSimuladorId;

        public SimuladorCRMService(int oportunidadeId, int simuladorParametroId, int modeloSimuladorId, int usuarioId)
        {
            _oportunidadeDAO = new OportunidadeDAO();
            _modeloDAO = new ModeloDAO();
            _simuladorDAO = new SimuladorDAO(true);
            _tabelaDAO = new TabelasDAO(true);
            _exportaTabelasService = new ExportaTabelasService(oportunidadeId, true);
            _oportunidadeId = oportunidadeId;
            _usuarioId = usuarioId;
            _simuladorParametroId = simuladorParametroId;
            _modeloSimuladorId = modeloSimuladorId;
        }

        public Response Iniciar()
        {
            var oportunidade = _oportunidadeDAO.ObterOportunidadePorId(_oportunidadeId);

            if (oportunidade == null)
                throw new Exception($"Oportunidade {_oportunidadeId} não encontrada");

            var modeloOportunidade = _modeloDAO.ObterModeloPorId(oportunidade.ModeloId);

            if (modeloOportunidade == null)
                throw new Exception($"A Oportunidade {_oportunidadeId} não possui um modelo vinculado");

            var parametrosSimulador = _simuladorDAO.ObterParametroSimuladorCRMPorId(_simuladorParametroId);

            if (parametrosSimulador == null)
                throw new Exception($"Parâmetro não cadastrado");

            _simuladorDAO.ExcluirTabelasCobrancasPorOportunidade(oportunidade.Id);

            var tabelaCobranca = _simuladorDAO.CadastrarTabelaCobrancaCRM(oportunidade);

            _exportaTabelasService.ExportarServicos(tabelaCobranca);

            var calculo = new CalculoSimuladorService(true);

            var simuladorId = CriarSimulador(parametrosSimulador, oportunidade);

            calculo.CalcularTabelas(new Requests.CalculoRequest
            {
                CRM = true,
                SimuladorId = simuladorId,
                Tabelas = new[] {
                    tabelaCobranca
                }
            });

            Response excel;

            if (modeloOportunidade.Escalonado)
            {
                var relatorio = new RelatorioExcelEscalonado();

                excel = relatorio.Gerar(new GerarExcelFiltro
                {
                    CRM = true,
                    TabelaId = tabelaCobranca,
                    OportunidadeId = oportunidade.Id,
                    ComAnaliseDeDados = false,
                    ServicosComplementares = false,
                    DadosDoCliente = true,
                    SomenteEstimativa = false,
                    DataPgtoInicial = DateTime.Now.AddMonths(-6).ToString("dd/MM/yyyy"),
                    DataPgtoFinal = DateTime.Now.ToString("dd/MM/yyyy"),
                    SimuladorId = simuladorId,
                    ParametroSimuladorId = _simuladorParametroId,
                    ModeloSimuladorId = _modeloSimuladorId,
                    UsuarioSimuladorId = _usuarioId
                });
            }
            else
            {
                var relatorio = new RelatorioExcel();

               excel = relatorio.Gerar(new GerarExcelFiltro
                {
                    CRM = true,
                    TabelaId = tabelaCobranca,
                    OportunidadeId = oportunidade.Id,
                    ComAnaliseDeDados = false,
                    ServicosComplementares = false,
                    DadosDoCliente = true,
                    SomenteEstimativa = false,
                    DataPgtoInicial = DateTime.Now.AddMonths(-6).ToString("dd/MM/yyyy"),
                    DataPgtoFinal = DateTime.Now.ToString("dd/MM/yyyy"),
                    SimuladorId = simuladorId,
                    ParametroSimuladorId = _simuladorParametroId,
                    ModeloSimuladorId = _modeloSimuladorId,
                    UsuarioSimuladorId = _usuarioId
                });
            }

            if (Configuracoes.BancoEmUso() != "ORACLE")
            {
                var valoresTicket = _oportunidadeDAO.ObterValoresTicket(tabelaCobranca);

                if (valoresTicket != null)
                {
                    valoresTicket.OportunidadeId = oportunidade.Id;
                    valoresTicket.TabelaId = tabelaCobranca;

                    _oportunidadeDAO.AtualizarValoresTicket(valoresTicket);
                }
            }

            _simuladorDAO.ExcluirCalculosAntigosSimulador(simuladorId);

            return new Response
            {
                Sucesso = true,
                Mensagem = "Relatório gerado com sucesso",
                ArquivoId = excel.ArquivoId,
                Base64 = excel.Base64,
                Hash = excel.Hash,
                NomeArquivo = excel.NomeArquivo,
                TamanhoArquivo = excel.TamanhoArquivo
            };
        }

        public int CriarSimulador(ParametrosSimuladorCRM parametros, Oportunidade oportunidade)
        {
            var simulador = new Simulador
            {
                Descricao = "Simulador CRM"
            };

            if (parametros.Regime == Regime.LCL)
            {
                simulador.ValorCifCargaSolta = parametros.ValorCif;
                simulador.Regime = "1"; //LCL
            }
            else
            {
                simulador.ValorCifConteiner = parametros.ValorCif;
                simulador.Regime = "0"; //FCL
            }

            simulador.Periodos = parametros.Periodos;
            simulador.TipoDocumento = parametros.DocumentoId;
            simulador.VolumeM3 = parametros.VolumeM3;
            simulador.NumeroLotes = parametros.NumeroLotes;
            simulador.GrupoAtracacao = parametros.GrupoAtracacaoId;
            simulador.Qtde20 = parametros.Qtde20;
            simulador.Qtde40 = parametros.Qtde40;
            simulador.CriadoPor = _usuarioId;

            var simuladorId = _simuladorDAO.CadastrarSimulador(simulador);

            return simuladorId;
        }
    }
}