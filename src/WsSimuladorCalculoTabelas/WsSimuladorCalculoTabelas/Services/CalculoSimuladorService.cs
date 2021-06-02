using System;
using System.Collections.Generic;
using System.Linq;
using WsSimuladorCalculoTabelas.DAO;
using WsSimuladorCalculoTabelas.Models;
using WsSimuladorCalculoTabelas.Requests;
using WsSimuladorCalculoTabelas.Responses;

namespace WsSimuladorCalculoTabelas.Services
{
    public class CalculoSimuladorService
    {
        private readonly TabelasDAO _tabelasDAO;
        private readonly SimuladorDAO _simuladorDAO;
        private readonly ServicoDAO _servicoDAO;

        public CalculoSimuladorService(bool crm)
        {
            _tabelasDAO = new TabelasDAO(crm);
            _simuladorDAO = new SimuladorDAO(crm);
            _servicoDAO = new ServicoDAO(crm);
        }

        public Response CalcularTabelas(CalculoRequest filtro)
        {
            decimal pesoTotalConteiner40 = 0.0M;
            decimal qtdeTotalConteiner40 = 0.0M;

            decimal pesoTotalCargasSolta = 0.0M;
            decimal qtdeTotalCargasSolta = 0.0M;
            int n_periodo = 0;

            List<string> margens = new List<string>() { "MDIR", "MESQ" };            

            if (filtro == null)
                throw new Exception("Nenhum filtro especificado");

            if (filtro.SimuladorId == 0)
                throw new Exception("Código do Simulador não informado");

            if (filtro.Tabelas.Length == 0)
                throw new Exception("Nenhuma Tabela de Cobrança especificada");

            _simuladorDAO.ExcluirCalculosAntigosSimuladorIPA(filtro.SimuladorId);

            var dadosSimulador = _simuladorDAO.ObterDetalhesSimuladorPorId(filtro.SimuladorId);

            if (dadosSimulador == null)
                throw new Exception("Não foi possível obter os dados do Simulador");

            var dadosConteiner20 = _simuladorDAO.ObterDadosCargaConteiner20(dadosSimulador.SimuladorId);
            var dadosConteiner40 = _simuladorDAO.ObterDadosCargaConteiner40(dadosSimulador.SimuladorId);
            var dadosCargasSolta = _simuladorDAO.ObterDadosCargaSolta(dadosSimulador.SimuladorId);

            dadosSimulador.AdicionarCargaConteiner(dadosConteiner20, dadosConteiner40);
            dadosSimulador.AdicionarCargaSolta(dadosCargasSolta);

            List<string> tipoCargas = dadosSimulador.Regime == "FCL" 
                ? new List<string>() { "SVAR20", "SVAR40" } 
                : new List<string>() { "CRGST" };

            var v20 = dadosSimulador.CargaConteiner20.Quantidade;
            var v40 = dadosSimulador.CargaConteiner40.Quantidade;

            foreach (var margem in margens)
            {
                dadosSimulador.Margem = margem;

                foreach (var tipoCarga in tipoCargas)
                {
                    dadosSimulador.TipoCarga = tipoCarga;

                    if (dadosSimulador.Regime == "FCL")
                    {
                        if (tipoCarga == "SVAR20")
                        {
                            dadosSimulador.CargaConteiner20.Quantidade = 1;
                            dadosSimulador.CargaConteiner40.Quantidade = 0;
                        }

                        if (tipoCarga == "SVAR40")
                        {
                            dadosSimulador.CargaConteiner20.Quantidade = 0;
                            dadosSimulador.CargaConteiner40.Quantidade = 1;
                        }

                        if (dadosSimulador.CargaConteiner20.Quantidade > 0 && dadosSimulador.CargaConteiner40.Quantidade == 0)
                        {
                            dadosSimulador.TipoCarga = "SVAR20";
                            dadosSimulador.TipoCargaSQL = " A.TIPO_CARGA = 'SVAR20' ";
                        }

                        if (dadosSimulador.CargaConteiner20.Quantidade == 0 && dadosSimulador.CargaConteiner40.Quantidade > 0)
                        {
                            dadosSimulador.TipoCarga = "SVAR40";
                            dadosSimulador.TipoCargaSQL = " A.TIPO_CARGA = 'SVAR40' ";
                        }

                        if (dadosSimulador.CargaConteiner20.Quantidade > 0 && dadosSimulador.CargaConteiner40.Quantidade > 0)
                        {
                            dadosSimulador.TipoCarga = "SVAR|SVAR20|SVAR40";
                            dadosSimulador.TipoCargaSQL = " (A.TIPO_CARGA = 'SVAR' OR A.TIPO_CARGA = 'SVAR20' OR A.TIPO_CARGA = 'SVAR40') ";
                        }

                        if (dadosSimulador.CargaConteiner20.Quantidade == 0 && dadosSimulador.CargaConteiner40.Quantidade == 0)
                        {
                            dadosSimulador.TipoCarga = "SVAR";
                            dadosSimulador.TipoCargaSQL = " A.TIPO_CARGA = 'SVAR' ";
                        }
                    }

                    if (dadosSimulador.Regime == "LCL")
                    {
                        dadosSimulador.TipoCarga = "CRGST";
                        dadosSimulador.TipoCargaSQL = " A.TIPO_CARGA ='CRGST' ";
                    }

                    decimal valorTicketMedio = 0;

                    foreach (var tabela in filtro.Tabelas)
                    {
                        var ticketMedioId = _simuladorDAO.GravarTicketMedio(new SimuladorTicketMedio
                        {
                            SimuladorId = dadosSimulador.SimuladorId,
                            ValorTicketMedio = valorTicketMedio,
                            ValorCif = dadosSimulador.ValorCifConteiner,
                            TipoDocumento = dadosSimulador.TipoDocumento,
                            LocalAtracacao = 0,
                            Regime = dadosSimulador.Regime,
                            TabelaId = tabela
                        });

                        IEnumerable<ServicoFixoVariavel> servicosFixos = new List<ServicoFixoVariavel>();

                        if (dadosSimulador.Regime == "LCL")
                        {
                            servicosFixos = _servicoDAO.ObterServicosLCLPorTabela(tabela, dadosSimulador.Margem);
                        }

                        if (dadosSimulador.Regime == "FCL")
                        {
                            servicosFixos = _servicoDAO.ObterServicosFCLPorTabela(tabela,dadosSimulador.Margem);
                        }

                        foreach (var servicoFixo in servicosFixos)
                        {
                            if (dadosSimulador.Regime == "LCL")
                            {
                                dadosSimulador.TipoCarga = "CRGST";
                                dadosSimulador.TipoCargaSQL = " A.TIPO_CARGA ='CRGST' ";

                                if (dadosSimulador.CargaSolta.Peso == 0)
                                {
                                    pesoTotalCargasSolta = 1000;
                                }

                                if (dadosSimulador.CargaSolta.Quantidade == 0)
                                {
                                    qtdeTotalCargasSolta = 1;
                                }

                                if (dadosSimulador.VolumeM3 == 0)
                                {
                                    dadosSimulador.VolumeM3 = 1;
                                }
                            }

                            if (dadosSimulador.Regime == "FCL")
                            {
                                if (dadosSimulador.CargaConteiner20.Quantidade > 0 && dadosSimulador.CargaConteiner40.Quantidade == 0)
                                {
                                    dadosSimulador.TipoCarga = "SVAR20";
                                    dadosSimulador.TipoCargaSQL = " (A.TIPO_CARGA ='SVAR20' OR A.TIPO_CARGA='SVAR') ";
                                }

                                if (dadosSimulador.CargaConteiner20.Quantidade == 0 && dadosSimulador.CargaConteiner40.Quantidade > 0)
                                {
                                    dadosSimulador.TipoCarga = "SVAR40";
                                    dadosSimulador.TipoCargaSQL = " (A.TIPO_CARGA ='SVAR40' OR A.TIPO_CARGA='SVAR') ";
                                }

                                if (dadosSimulador.CargaConteiner20.Quantidade > 0 && dadosSimulador.CargaConteiner40.Quantidade > 0)
                                {
                                    dadosSimulador.TipoCarga = "SVAR|SVAR20|SVAR40";
                                    dadosSimulador.TipoCargaSQL = " (A.TIPO_CARGA='SVAR' OR A.TIPO_CARGA='SVAR20' OR A.TIPO_CARGA='SVAR40') ";
                                }

                                if (dadosSimulador.CargaConteiner20.Quantidade == 0 && dadosSimulador.CargaConteiner40.Quantidade == 0)
                                {
                                    dadosSimulador.TipoCarga = "SVAR";
                                    dadosSimulador.TipoCargaSQL = " A.TIPO_CARGA = 'SVAR' ";
                                }

                                if (dadosSimulador.CargaConteiner20.Peso == 0 && dadosSimulador.CargaConteiner40.Peso == 0)
                                {
                                    pesoTotalConteiner40 = 1000;
                                }

                                if (dadosSimulador.CargaConteiner20.Quantidade == 0 && dadosSimulador.CargaConteiner40.Quantidade == 0)
                                {
                                    qtdeTotalConteiner40 = 1;
                                }
                            }

                            var servicos = _servicoDAO.ObterServicosFixos(
                                new ServicosFiltro
                                {
                                    Lista = tabela,
                                    TipoDocumento = dadosSimulador.TipoDocumento,
                                    Armador = dadosSimulador.Armador,
                                    ServicoId = servicoFixo.ServicoId,
                                    TipoCargaSQL = dadosSimulador.TipoCargaSQL,
                                    GrupoAtracacao = servicoFixo.GrupoAtracacaoId,
                                    Margem = dadosSimulador.Margem
                                });

                            if ((servicoFixo.GrupoAtracacaoId == 0) || (servicoFixo.ServicoId==1))
                            {
                                if (servicos.Count() == 0)
                                {
                                    servicos = _servicoDAO.ObterServicosFixos(
                                        new ServicosFiltro
                                        {
                                            Lista = tabela,
                                            TipoDocumento = dadosSimulador.TipoDocumento,
                                            Armador = dadosSimulador.Armador,
                                            ServicoId = servicoFixo.ServicoId,
                                            TipoCargaSQL = dadosSimulador.TipoCargaSQL,
                                            Margem = dadosSimulador.Margem
                                        });
                                }

                                if (servicos.Count() == 0)
                                {
                                    servicos = _servicoDAO.ObterServicosFixos(
                                        new ServicosFiltro
                                        {
                                            Lista = tabela,
                                            TipoDocumento = dadosSimulador.TipoDocumento,
                                            Armador = dadosSimulador.Armador,
                                            ServicoId = servicoFixo.ServicoId,
                                            TipoCargaSQL = dadosSimulador.TipoCargaSQL,
                                            Margem = "SVAR"
                                        });
                                }

                                if (servicos.Count() == 0)
                                {
                                    servicos = _servicoDAO.ObterServicosFixos(
                                       new ServicosFiltro
                                       {
                                           Lista = tabela,
                                           TipoDocumento = dadosSimulador.TipoDocumento,
                                           Armador = dadosSimulador.Armador,
                                           ServicoId = servicoFixo.ServicoId,
                                           TipoCargaSQL = dadosSimulador.TipoCargaSQL
                                       });
                                }
                                if (servicos.Count() == 0 && ((servicoFixo.FlagDesova && dadosSimulador.Regime == "LCL") || (servicoFixo.FlagDesova && dadosSimulador.Regime == "FCL") || servicoFixo.FlagFCL))
                                {
                                    if (dadosSimulador.CargaConteiner20.Quantidade > 0 && dadosSimulador.CargaConteiner40.Quantidade == 0)
                                    {
                                        dadosSimulador.TipoCarga = "SVAR20";
                                        dadosSimulador.TipoCargaSQL = " A.TIPO_CARGA ='SVAR20' ";
                                    }

                                    if (dadosSimulador.CargaConteiner20.Quantidade == 0 && dadosSimulador.CargaConteiner40.Quantidade > 0)
                                    {
                                        dadosSimulador.TipoCarga = "SVAR40";
                                        dadosSimulador.TipoCargaSQL = " A.TIPO_CARGA='SVAR40' ";
                                    }

                                    if (dadosSimulador.CargaConteiner20.Quantidade > 0 && dadosSimulador.CargaConteiner40.Quantidade > 0)
                                    {
                                        dadosSimulador.TipoCarga = "SVAR|SVAR20|SVAR40";
                                        dadosSimulador.TipoCargaSQL = " (A.TIPO_CARGA='SVAR' OR A.TIPO_CARGA='SVAR20' OR A.TIPO_CARGA='SVAR40') ";
                                    }

                                    if (dadosSimulador.CargaConteiner20.Quantidade == 0 && dadosSimulador.CargaConteiner40.Quantidade == 0)
                                    {
                                        dadosSimulador.TipoCarga = "SVAR";
                                        dadosSimulador.TipoCargaSQL = " A.TIPO_CARGA='SVAR' ";
                                    }

                                    servicos = _servicoDAO.ObterServicosFixos(
                                       new ServicosFiltro
                                       {
                                           Lista = tabela,
                                           TipoDocumento = dadosSimulador.TipoDocumento,
                                           Armador = dadosSimulador.Armador,
                                           ServicoId = servicoFixo.ServicoId,
                                           TipoCargaSQL = dadosSimulador.TipoCargaSQL,
                                           GrupoAtracacao = servicoFixo.GrupoAtracacaoId,
                                           Margem = dadosSimulador.Margem
                                       });
                                }

                                if (servicos.Count() == 0)
                                {
                                    servicos = _servicoDAO.ObterServicosFixos(
                                       new ServicosFiltro
                                       {
                                           Lista = tabela,
                                           TipoDocumento = dadosSimulador.TipoDocumento,
                                           Armador = dadosSimulador.Armador,
                                           ServicoId = servicoFixo.ServicoId,
                                           TipoCargaSQL = dadosSimulador.TipoCargaSQL,
                                           Margem = dadosSimulador.Margem
                                       });
                                }

                                if (servicos.Count() == 0)
                                {
                                    servicos = _servicoDAO.ObterServicosFixos(
                                       new ServicosFiltro
                                       {
                                           Lista = tabela,
                                           TipoDocumento = dadosSimulador.TipoDocumento,
                                           Armador = dadosSimulador.Armador,
                                           ServicoId = servicoFixo.ServicoId,
                                           TipoCargaSQL = dadosSimulador.TipoCargaSQL,
                                           Margem = "SVAR"
                                       });
                                }

                                if (servicos.Count() == 0)
                                {
                                    servicos = _servicoDAO.ObterServicosFixos(
                                       new ServicosFiltro
                                       {
                                           Lista = tabela,
                                           TipoDocumento = dadosSimulador.TipoDocumento,
                                           Armador = dadosSimulador.Armador,
                                           ServicoId = servicoFixo.ServicoId,
                                           TipoCargaSQL = dadosSimulador.TipoCargaSQL,
                                           Margem = "SVAR"
                                       });
                                }

                                if (servicos.Count() == 0)
                                {
                                    servicos = _servicoDAO.ObterServicosFixos(
                                       new ServicosFiltro
                                       {
                                           Lista = tabela,
                                           TipoDocumento = dadosSimulador.TipoDocumento,
                                           Armador = dadosSimulador.Armador,
                                           ServicoId = servicoFixo.ServicoId,
                                           TipoCargaSQL = " A.TIPO_CARGA = 'SVAR' ",
                                           Margem = dadosSimulador.Margem
                                       });
                                }

                                if (servicos.Count() == 0)
                                {
                                    servicos = _servicoDAO.ObterServicosFixos(
                                       new ServicosFiltro
                                       {
                                           Lista = tabela,
                                           TipoDocumento = dadosSimulador.TipoDocumento,
                                           Armador = dadosSimulador.Armador,
                                           ServicoId = servicoFixo.ServicoId,
                                           TipoCargaSQL = " A.TIPO_CARGA = 'SVAR' ",
                                           Margem = "SVAR"
                                       });
                                }

                                if (servicos.Count() == 0)
                                {
                                    servicos = _servicoDAO.ObterServicosFixos(
                                       new ServicosFiltro
                                       {
                                           Lista = tabela,
                                           TipoDocumento = dadosSimulador.TipoDocumento,
                                           Armador = dadosSimulador.Armador,
                                           ServicoId = servicoFixo.ServicoId,
                                           TipoCargaSQL = " A.TIPO_CARGA = 'SVAR' "
                                       });
                                }

                                if (servicos.Count() > 0)
                                {
                                    var servicoSimuladorId = _simuladorDAO.ObterServicoSimuladorId(filtro.SimuladorId, servicoFixo.ServicoId);

                                    if (servicoSimuladorId == 0)
                                    {
                                        servicoSimuladorId = _simuladorDAO.GravarServicoSimulador(filtro.SimuladorId, servicoFixo.ServicoId);
                                    }

                                    if (servicoSimuladorId > 0)
                                    {
                                        var ehConteiner = (dadosSimulador.Regime == "FCL");

                                        CalcularValorParcelaFixos(servicos, ehConteiner, servicoSimuladorId, dadosSimulador);
                                    }
                                }
                            }
                        }
                        // Calculo Serviços Variáveis

                        IEnumerable<ServicoFixoVariavel> servicosVariaveis = new List<ServicoFixoVariavel>();

                        servicosVariaveis = _servicoDAO.ObterServicosVariaveisPorTabela(tabela);

                        foreach (var servicoVariavel in servicosVariaveis)
                        {
                            if (dadosSimulador.Regime == "FCL")
                            {
                                if (dadosSimulador.CargaConteiner20.Quantidade > 0 && dadosSimulador.CargaConteiner40.Quantidade == 0)
                                {
                                    dadosSimulador.TipoCarga = "SVAR20";
                                    dadosSimulador.TipoCargaSQL = " A.TIPO_CARGA ='SVAR20' ";
                                }

                                if (dadosSimulador.CargaConteiner20.Quantidade == 0 && dadosSimulador.CargaConteiner40.Quantidade > 0)
                                {
                                    dadosSimulador.TipoCarga = "SVAR40";
                                    dadosSimulador.TipoCargaSQL = " A.TIPO_CARGA ='SVAR40' ";
                                }

                                if (dadosSimulador.CargaConteiner20.Quantidade > 0 && dadosSimulador.CargaConteiner40.Quantidade > 0)
                                {
                                    dadosSimulador.TipoCarga = "SVAR|SVAR20|SVAR40";
                                    dadosSimulador.TipoCargaSQL = " (A.TIPO_CARGA = 'SVAR' OR A.TIPO_CARGA = 'SVAR20' OR A.TIPO_CARGA = 'SVAR40') ";
                                }

                                if (dadosSimulador.CargaConteiner20.Quantidade == 0 && dadosSimulador.CargaConteiner40.Quantidade == 0)
                                {
                                    dadosSimulador.TipoCarga = "SVAR";
                                    dadosSimulador.TipoCargaSQL = " A.TIPO_CARGA='SVAR' ";
                                }
                            }

                            if (dadosSimulador.Regime == "LCL")
                            {
                                dadosSimulador.TipoCarga = "CRGST";
                                dadosSimulador.TipoCargaSQL = " A.TIPO_CARGA='CRGST' ";
                            }

                            for (int periodo = 1; periodo <= dadosSimulador.Periodos; periodo++)
                            {
                                n_periodo = periodo;                              
                                if (servicoVariavel.ServicoId ==295)
                                    {
                                    if (dadosSimulador.Periodos == 1) { n_periodo = 2; }
                                     }
                                var servicos = _servicoDAO.ObterServicosVariaveis(
                                    new ServicosFiltro
                                    {
                                        Lista = tabela,
                                        ServicoId = servicoVariavel.ServicoId,
                                        Periodo = n_periodo,
                                        TipoCargaSQL = "(" + dadosSimulador.TipoCargaSQL + " OR A.TIPO_CARGA = 'SVAR') ",
                                        TipoDocumento = dadosSimulador.TipoDocumento,
                                        Armador = dadosSimulador.Armador,
                                        Margem = dadosSimulador.Margem,
                                        GrupoAtracacao = servicoVariavel.GrupoAtracacaoId
                                    });

                                if (servicos.Count() == 0)
                                {
                                    servicos = _servicoDAO.ObterServicosVariaveis(
                                        new ServicosFiltro
                                        {
                                            Lista = tabela,
                                            ServicoId = servicoVariavel.ServicoId,
                                            TipoDocumento = dadosSimulador.TipoDocumento,
                                            Armador = dadosSimulador.Armador,
                                            Periodo = n_periodo,
                                            TipoCargaSQL = "(" + dadosSimulador.TipoCargaSQL + " OR A.TIPO_CARGA = 'SVAR') ",
                                            Margem = dadosSimulador.Margem,
                                            GrupoAtracacao = 0
                                        });
                                }

                                if (servicos.Count() == 0)
                                {
                                    servicos = _servicoDAO.ObterServicosVariaveis(
                                        new ServicosFiltro
                                        {
                                            Lista = tabela,
                                            ServicoId = servicoVariavel.ServicoId,
                                            Periodo = n_periodo,
                                            TipoDocumento = dadosSimulador.TipoDocumento,
                                            Armador = dadosSimulador.Armador,
                                            TipoCargaSQL = "(" + dadosSimulador.TipoCargaSQL + " OR A.TIPO_CARGA = 'SVAR') ",
                                            Margem = "SVAR",
                                            GrupoAtracacao = 0
                                        });
                                }

                                if (servicos.Count() == 0)
                                {
                                    servicos = _servicoDAO.ObterServicosVariaveis(
                                        new ServicosFiltro
                                        {
                                            Lista = tabela,
                                            ServicoId = servicoVariavel.ServicoId,
                                            Periodo = n_periodo,
                                            TipoCargaSQL = " A.TIPO_CARGA = 'SVAR' ",
                                            TipoDocumento = dadosSimulador.TipoDocumento,
                                            Armador = dadosSimulador.Armador,
                                            Margem = dadosSimulador.Margem,
                                            GrupoAtracacao = servicoVariavel.GrupoAtracacaoId
                                        });
                                }

                                if (servicos.Count() == 0)
                                {
                                    servicos = _servicoDAO.ObterServicosVariaveis(
                                        new ServicosFiltro
                                        {
                                            Lista = tabela,
                                            ServicoId = servicoVariavel.ServicoId,
                                            TipoDocumento = dadosSimulador.TipoDocumento,
                                            Armador = dadosSimulador.Armador,
                                            Periodo = n_periodo,
                                            TipoCargaSQL = " A.TIPO_CARGA = 'SVAR' ",
                                            Margem = dadosSimulador.Margem,
                                            GrupoAtracacao = 0
                                        });
                                }

                                if (servicos.Count() == 0)
                                {
                                    servicos = _servicoDAO.ObterServicosVariaveis(
                                        new ServicosFiltro
                                        {
                                            Lista = tabela,
                                            ServicoId = servicoVariavel.ServicoId,
                                            Periodo = n_periodo,
                                            TipoDocumento = dadosSimulador.TipoDocumento,
                                            Armador = dadosSimulador.Armador,
                                            TipoCargaSQL = " A.TIPO_CARGA = 'SVAR' ",
                                            Margem = "SVAR",
                                            GrupoAtracacao = 0
                                        });
                                }

                                if (servicos.Count() == 0)
                                {
                                    servicos = _servicoDAO.ObterServicosVariaveis(
                                        new ServicosFiltro
                                        {
                                            Lista = tabela,
                                            ServicoId = servicoVariavel.ServicoId,
                                            Periodo = n_periodo,
                                            TipoDocumento = dadosSimulador.TipoDocumento,
                                            Armador = dadosSimulador.Armador,
                                            TipoCargaSQL = " A.TIPO_CARGA = 'CPIER' ",
                                            Margem = dadosSimulador.Margem,
                                            GrupoAtracacao = 0
                                        });
                                }

                                if (servicos.Count() == 0 && dadosSimulador.Regime == "LCL")
                                {
                                    servicos = _servicoDAO.ObterServicosVariaveis(
                                        new ServicosFiltro
                                        {
                                            Lista = tabela,
                                            ServicoId = servicoVariavel.ServicoId,
                                            Periodo = n_periodo,
                                            TipoDocumento = dadosSimulador.TipoDocumento,
                                            Armador = dadosSimulador.Armador,
                                            TipoCargaSQL = " A.TIPO_CARGA = 'CPIER' ",
                                            Margem = "SVAR",
                                            GrupoAtracacao = 0
                                        });
                                }

                                if (servicos.Count() > 0)
                                {
                                    var servicoSimuladorId = _simuladorDAO.ObterServicoSimuladorId(filtro.SimuladorId, servicoVariavel.ServicoId);

                                    if (servicoSimuladorId == 0)
                                    {
                                        servicoSimuladorId = _simuladorDAO.GravarServicoSimulador(filtro.SimuladorId, servicoVariavel.ServicoId);
                                    }

                                    if (servicoSimuladorId > 0)
                                    {
                                        var ehConteiner = (dadosSimulador.Regime == "FCL");

                                        CalcularValorParcelaVariaveis(servicos, ehConteiner, servicoSimuladorId, dadosSimulador);
                                    }
                                }
                            }
                        }
                        
                    }
                }
                
            }

            return new Response
            {
                Sucesso = true,
                Mensagem = "Cálculo realizado com sucesso"
            };
        }

        private void CalcularValorParcelaFixos(
            IEnumerable<ServicoFixoVariavel> servicos,
            bool ehConteiner,
            int servicoSimuladorId,
            Simulador tela)
        {
            decimal baseMaximo = 0.0M;
            decimal baseMinimo = 0.0M;
            decimal quantidade = 0.0M;
            decimal preco = 0.0M;
            decimal parcela = 0.0M;
            decimal baseCalculo = 0.0M;
            decimal precoUnitario = 0.0M;
            decimal precoCalculo = 0.0M;
            decimal pesoTotalConteiner = 0.0M;
            string descricaoBaseCalculo = string.Empty;

            if (ehConteiner)
            {
                foreach (var servico in servicos)
                {
                    baseMinimo = servico.PrecoMinimo;
                    baseMaximo = servico.PrecoMaximo;

                    if (servico.BaseCalculo == "UNID")
                    {
                        switch (tela.TipoCarga)
                        {
                            case "SVAR20":
                                quantidade = tela.CargaConteiner20.Quantidade;
                                break;
                            case "SVAR40":
                                quantidade = tela.CargaConteiner40.Quantidade;
                                break;
                            case "SVAR|SVAR20|SVAR40":
                            case "SVAR":
                                quantidade = tela.CargaConteiner20.Quantidade + tela.CargaConteiner40.Quantidade;
                                break;
                        }

                        quantidade = quantidade == 0 ? 1 : quantidade;

                        preco = servico.PrecoUnitario;
                        baseCalculo = quantidade;
                        descricaoBaseCalculo = "UNIDADE";

                        var valoresPorCif = _servicoDAO.ObterDetalhesServicoFixoPorFaixas(servico.ServicoFixoVariavelId, tela.ValorCifConteiner, "C");

                        if (valoresPorCif != null)
                        {
                            preco = valoresPorCif.Percentual;
                            baseMinimo = valoresPorCif.ValorMinimo;
                        }

                        precoUnitario = preco;
                    }

                    if (servico.BaseCalculo == "BL")
                    {
                        quantidade = 1;
                        preco = servico.PrecoUnitario;
                        baseCalculo = quantidade;
                        descricaoBaseCalculo = "BL";
                        precoUnitario = servico.PrecoUnitario;
                    }

                    if (servico.BaseCalculo == "VOL")
                    {
                        quantidade = tela.VolumeM3;
                        preco = servico.PrecoUnitario;
                        baseCalculo = quantidade;
                        descricaoBaseCalculo = "VOLUME M3";
                        precoUnitario = servico.PrecoUnitario;
                    }

                    if (servico.BaseCalculo == "VOLP")
                    {
                        quantidade = tela.VolumeM3;
                        descricaoBaseCalculo = "VOLUME M3";

                        switch (tela.TipoCarga)
                        {
                            case "SVAR20":
                                pesoTotalConteiner = tela.CargaConteiner20.Peso;
                                break;
                            case "SVAR40":
                                pesoTotalConteiner = tela.CargaConteiner40.Peso;
                                break;
                            case "SVAR|SVAR20|SVAR40":
                                pesoTotalConteiner = tela.CargaConteiner20.Peso + tela.CargaConteiner40.Peso;
                                break;
                            case "SVAR":
                                pesoTotalConteiner = tela.CargaConteiner20.Peso + tela.CargaConteiner40.Peso;

                                if (pesoTotalConteiner == 0)
                                    pesoTotalConteiner = 1000;

                                break;
                        }

                        if ((pesoTotalConteiner / 1000) > quantidade)
                        {
                            quantidade = pesoTotalConteiner / 1000;
                            descricaoBaseCalculo = "TONELAGEM";
                        }

                        preco = servico.PrecoUnitario;
                        baseCalculo = quantidade;
                        precoUnitario = servico.PrecoUnitario;
                    }

                    if (servico.BaseCalculo == "TON")
                    {
                        switch (tela.TipoCarga)
                        {
                            case "SVAR20":
                                pesoTotalConteiner = tela.CargaConteiner20.Peso;
                                break;
                            case "SVAR40":
                                pesoTotalConteiner = tela.CargaConteiner40.Peso;
                                break;
                            case "SVAR|SVAR20|SVAR40":
                                pesoTotalConteiner = tela.CargaConteiner20.Peso + tela.CargaConteiner40.Peso;
                                break;
                            case "SVAR":
                                pesoTotalConteiner = tela.CargaConteiner20.Peso + tela.CargaConteiner40.Peso;

                                if (pesoTotalConteiner == 0)
                                    pesoTotalConteiner = 1000;

                                break;
                        }

                        quantidade = pesoTotalConteiner / 1000;
                        preco = servico.PrecoUnitario;
                        baseCalculo = quantidade;
                        descricaoBaseCalculo = "TONELAGEM";
                        precoUnitario = servico.PrecoUnitario;
                    }

                    if (servico.BaseCalculo == "CIf" || servico.BaseCalculo == "CIFM")
                    {
                        quantidade = tela.ValorCifConteiner / 100;

                        var valoresPorCif = _servicoDAO.ObterDetalhesServicoFixoPorFaixas(servico.ServicoFixoVariavelId, tela.ValorCifConteiner, "C");

                        if (valoresPorCif != null)
                        {
                            preco = valoresPorCif.Percentual;
                            baseMinimo = valoresPorCif.ValorMinimo;
                        }
                        else
                        {
                            if (servico.PrecoUnitario > 0)
                            {
                                preco = servico.PrecoUnitario;
                            }
                        }

                        baseCalculo = tela.ValorCifConteiner;
                        descricaoBaseCalculo = "CIF";
                        precoUnitario = preco;
                    }

                    if (servico.BaseCalculo == "CIF0")
                    {
                        quantidade = (tela.ValorCifConteiner * 1.5M) / 100;

                        var valoresPorCif = _servicoDAO.ObterDetalhesServicoFixoPorFaixas(servico.ServicoFixoVariavelId, (tela.ValorCifConteiner * 1.5M), "C");

                        if (valoresPorCif != null)
                        {
                            preco = valoresPorCif.Percentual;
                            baseMinimo = valoresPorCif.ValorMinimo;
                        }
                        else
                        {
                            if (servico.PrecoUnitario > 0)
                            {
                                preco = servico.PrecoUnitario;
                            }
                        }

                        baseCalculo = tela.ValorCifConteiner * 1.5M;
                        descricaoBaseCalculo = "CIF";
                        precoUnitario = preco;
                    }

                    parcela = quantidade * preco;
                    precoCalculo = parcela;

                    if (parcela < baseMinimo && baseMinimo != 0 && quantidade != 0)
                    {
                        parcela = baseMinimo;
                    }

                    _simuladorDAO.GravarServicoCalculado(new ServicoCalculo
                    {
                        ServicoCalculoId = servicoSimuladorId,
                        BaseCalculo = baseCalculo,
                        Parcela = parcela,
                        Quantidade = quantidade,
                        PrecoUnitario = precoUnitario,
                        DescricaoBaseCalculo = descricaoBaseCalculo,
                        Lista = servico.Lista,
                        TipoServico = "F",
                        Margem = tela.Margem,
                        TipoCarga = tela.TipoCarga
                    });
                }
            }
            else
            {
                foreach (var servico in servicos)
                {
                    baseMinimo = servico.PrecoMinimo;

                    if (servico.BaseCalculo == "VOL")
                    {
                        quantidade = tela.VolumeM3;
                        preco = servico.PrecoUnitario;
                        baseCalculo = quantidade;
                        descricaoBaseCalculo = "VOLUME M3";
                        precoUnitario = servico.PrecoUnitario;
                    }

                    if (servico.BaseCalculo == "VOLP")
                    {
                        quantidade = tela.VolumeM3;
                        descricaoBaseCalculo = "VOLUME M3";

                        if ((tela.CargaSolta.Peso / 1000) > quantidade)
                        {
                            quantidade = tela.CargaSolta.Peso / 1000;
                            descricaoBaseCalculo = "TONELAGEM";
                        }

                        preco = servico.PrecoUnitario;
                        baseCalculo = quantidade;
                        precoUnitario = servico.PrecoUnitario;
                    }

                    if (servico.BaseCalculo == "UNID")
                    {
                        quantidade = tela.CargaSolta.Quantidade;
                        preco = servico.PrecoUnitario;
                        baseCalculo = quantidade;
                        descricaoBaseCalculo = "UNIDADE";
                        precoUnitario = servico.PrecoUnitario;
                    }

                    if (servico.BaseCalculo == "BL")
                    {
                        quantidade = 1;
                        preco = servico.PrecoUnitario;
                        baseCalculo = quantidade;
                        descricaoBaseCalculo = "BL";
                        precoUnitario = servico.PrecoUnitario;

                        tela.NumeroLotes = tela.NumeroLotes == 0 ? 1 : tela.NumeroLotes;

                        var valoresPorLotes = _servicoDAO.ObterDetalhesServicoFixoPorFaixas(servico.ServicoFixoVariavelId, tela.NumeroLotes, "B");

                        if (valoresPorLotes != null)
                        {
                            preco = valoresPorLotes.Percentual;
                            precoUnitario = valoresPorLotes.Percentual;
                            baseMinimo = valoresPorLotes.ValorMinimo;
                        }
                    }

                    if (servico.BaseCalculo == "TON")
                    {
                        quantidade = tela.CargaSolta.Peso / 1000;
                        preco = servico.PrecoUnitario;
                        baseCalculo = quantidade;
                        descricaoBaseCalculo = "TONELAGEM";
                        precoUnitario = servico.PrecoUnitario;
                    }

                    if (servico.BaseCalculo == "CIF" || servico.BaseCalculo == "CIFM")
                    {
                        quantidade = tela.ValorCifCargaSolta / 100;

                        var valoresPorFaixaCif = _servicoDAO.ObterDetalhesServicoFixoPorFaixas(servico.ServicoFixoVariavelId, tela.ValorCifCargaSolta, "C");

                        if (valoresPorFaixaCif != null)
                        {
                            preco = valoresPorFaixaCif.Percentual;
                            baseMinimo = valoresPorFaixaCif.ValorMinimo;
                        }
                        else
                        {
                            if (servico.PrecoUnitario > 0)
                            {
                                preco = servico.PrecoUnitario;
                            }
                        }

                        baseCalculo = tela.ValorCifCargaSolta;
                        descricaoBaseCalculo = "CIF";
                        precoUnitario = preco;
                    }

                    if (servico.BaseCalculo == "CIF0")
                    {
                        quantidade = ((tela.ValorCifCargaSolta * 1.5M) / 100);

                        var valoresPorFaixaCif = _servicoDAO.ObterDetalhesServicoFixoPorFaixas(servico.ServicoFixoVariavelId, (tela.ValorCifCargaSolta * 1.5M), "C");

                        if (valoresPorFaixaCif != null)
                        {
                            preco = valoresPorFaixaCif.Percentual;
                            baseMinimo = valoresPorFaixaCif.ValorMinimo;
                        }
                        else
                        {
                            if (servico.PrecoUnitario > 0)
                            {
                                preco = servico.PrecoUnitario;
                            }
                        }

                        baseCalculo = (tela.ValorCifCargaSolta * 1.5M);
                        descricaoBaseCalculo = "CIF";
                        precoUnitario = preco;
                    }

                    parcela = quantidade * preco;
                    precoCalculo = parcela;

                    if ((parcela < baseMinimo) && baseMinimo != 0 && quantidade != 0)
                    {
                        parcela = baseMinimo;
                    }

                    _simuladorDAO.GravarServicoCalculado(new ServicoCalculo
                    {
                        ServicoCalculoId = servicoSimuladorId,
                        BaseCalculo = baseCalculo,
                        Parcela = parcela,
                        Quantidade = quantidade,
                        PrecoUnitario = precoUnitario,
                        PrecoMinimo = baseMinimo,
                        DescricaoBaseCalculo = descricaoBaseCalculo,
                        Lista = servico.Lista,
                        TipoServico = "F",
                        Margem = tela.Margem,
                        TipoCarga = tela.TipoCarga
                    });
                }
            }
        }

        private void CalcularValorParcelaVariaveis(
           IEnumerable<ServicoFixoVariavel> servicos,
           bool ehConteiner,
           int servicoSimuladorId,
           Simulador tela)
        {
            decimal baseMaximo = 0.0M;
            decimal baseMinimo = 0.0M;
            decimal quantidade = 0.0M;
            decimal preco = 0.0M;
            decimal parcela = 0.0M;
            decimal baseCalculo = 0.0M;
            decimal precoUnitario = 0.0M;
            decimal precoMinimo = 0.0M;
            decimal precoCalculo = 0.0M;
            decimal pesoTotalConteiner = 0.0M;
            decimal qtdeTotalConteiner = 0.0M;
            decimal pesoTotalCargaSolta = 0.0M;
            decimal qtdeTotalCargaSolta = 0.0M;
            string descricaoBaseCalculo = string.Empty;

            if (ehConteiner)
            {
                foreach (var servico in servicos)
                {
                    baseMaximo = servico.PrecoMaximo;
                    precoMinimo = servico.PrecoMinimo;
                    baseMinimo = precoMinimo;

                    if (servico.BaseCalculo == "PERI")
                    {
                        quantidade = 1;
                        preco = servico.PrecoUnitario;
                        descricaoBaseCalculo = "PERIODO";
                        baseCalculo = quantidade;
                    }

                    if (servico.BaseCalculo == "VOL")
                    {
                        quantidade = tela.VolumeM3;
                        preco = servico.PrecoUnitario;
                        descricaoBaseCalculo = "VOLUME M3";
                        baseCalculo = quantidade;
                    }

                    if (servico.BaseCalculo == "BL")
                    {
                        quantidade = 1;
                        preco = servico.PrecoUnitario;
                        baseCalculo = quantidade;
                        descricaoBaseCalculo = "BL";
                        precoUnitario = servico.PrecoUnitario;
                    }

                    if (servico.BaseCalculo == "VOLP")
                    {
                        quantidade = tela.VolumeM3;
                        baseCalculo = quantidade;

                        switch (tela.TipoCarga)
                        {
                            case "SVAR20":
                                pesoTotalConteiner = tela.CargaConteiner20.Peso;
                                break;
                            case "SVAR40":
                                pesoTotalConteiner = tela.CargaConteiner40.Peso;
                                break;
                            case "SVAR|SVAR20|SVAR40":
                                pesoTotalConteiner = tela.CargaConteiner20.Peso + tela.CargaConteiner40.Peso;
                                break;
                            case "SVAR":
                                pesoTotalConteiner = tela.CargaConteiner20.Peso + tela.CargaConteiner40.Peso;

                                if (pesoTotalConteiner == 0)
                                    pesoTotalConteiner = 1000;

                                break;
                        }

                        if ((pesoTotalConteiner / 1000) > quantidade)
                        {
                            quantidade = pesoTotalConteiner / 1000;
                            baseCalculo = quantidade;
                        }

                        preco = servico.PrecoUnitario;
                        descricaoBaseCalculo = "VOL/TONS";
                    }

                    if (servico.BaseCalculo == "UNID")
                    {
                        switch (tela.TipoCarga)
                        {
                            case "SVAR20":
                                qtdeTotalConteiner = tela.CargaConteiner20.Quantidade;
                                break;
                            case "SVAR40":
                                qtdeTotalConteiner = tela.CargaConteiner40.Quantidade;
                                break;
                            case "SVAR|SVAR20|SVAR40":
                                qtdeTotalConteiner = tela.CargaConteiner20.Quantidade + tela.CargaConteiner40.Quantidade;
                                break;
                        }

                        quantidade = qtdeTotalConteiner;
                        descricaoBaseCalculo = "UNIDADE";
                        baseCalculo = quantidade;

                        var valoresPorCif = _servicoDAO.ObterDetalhesServicoVariavelPorFaixas(servico.ServicoFixoVariavelId, tela.ValorCifConteiner, "C");

                        if (valoresPorCif != null)
                        {
                            preco = valoresPorCif.Percentual;
                            precoMinimo = valoresPorCif.ValorMinimo;
                        }
                        else
                        {
                            if (servico.PrecoUnitario > 0)
                            {
                                preco = servico.PrecoUnitario;
                            }
                        }
                    }

                    if (servico.BaseCalculo == "TON")
                    {
                        switch (tela.TipoCarga)
                        {
                            case "SVAR20":
                                pesoTotalConteiner = tela.CargaConteiner20.Peso;
                                break;
                            case "SVAR40":
                                pesoTotalConteiner = tela.CargaConteiner40.Peso;
                                break;
                            case "SVAR|SVAR20|SVAR40":
                                pesoTotalConteiner = tela.CargaConteiner20.Peso + tela.CargaConteiner40.Peso;
                                break;
                            case "SVAR":
                                pesoTotalConteiner = tela.CargaConteiner20.Peso + tela.CargaConteiner40.Peso;

                                if (pesoTotalConteiner == 0)
                                {
                                    pesoTotalConteiner = 1000;
                                }

                                break;
                        }

                        quantidade = pesoTotalConteiner / 1000;
                        preco = servico.PrecoUnitario;
                        descricaoBaseCalculo = "TONELAGEM";
                        baseCalculo = quantidade;
                    }

                    if (servico.BaseCalculo == "CIF" || servico.BaseCalculo == "CIFM")
                    {
                        descricaoBaseCalculo = "CIF";
                        quantidade = tela.ValorCifConteiner / 100;
                        baseCalculo = quantidade;

                        var valoresPorCif = _servicoDAO.ObterDetalhesServicoVariavelPorFaixas(servico.ServicoFixoVariavelId, tela.ValorCifConteiner, "C");

                        if (valoresPorCif != null)
                        {
                            preco = valoresPorCif.Percentual;
                            precoMinimo = valoresPorCif.ValorMinimo;
                        }
                        else
                        {
                            if (servico.PrecoUnitario > 0)
                            {
                                preco = servico.PrecoUnitario;
                            }
                        }

                        quantidade = quantidade;
                    }

                    if (servico.BaseCalculo == "CIF0")
                    {
                        descricaoBaseCalculo = "CIF";
                        quantidade = (tela.ValorCifConteiner * 1.5M) / 100;
                        baseCalculo = tela.ValorCifConteiner * 1.5M;

                        var valoresPorCif = _servicoDAO.ObterDetalhesServicoVariavelPorFaixas(servico.ServicoFixoVariavelId, tela.ValorCifConteiner * 1.5M, "C");

                        if (valoresPorCif != null)
                        {
                            preco = valoresPorCif.Percentual;
                            precoMinimo = valoresPorCif.ValorMinimo;
                        }
                        else
                        {
                            if (servico.PrecoUnitario > 0)
                            {
                                preco = servico.PrecoUnitario;
                            }
                        }
                    }

                    precoUnitario = preco;
                    parcela = quantidade * preco;
                    precoCalculo = parcela;

                    if (parcela < baseMinimo && baseMinimo > 0)
                    {
                        parcela = baseMinimo;
                    }

                    if (parcela > baseMaximo && baseMaximo > 0)
                    {
                        parcela = baseMaximo;
                    }

                    _simuladorDAO.GravarServicoCalculado(new ServicoCalculo
                    {
                        ServicoCalculoId = servicoSimuladorId,
                        BaseCalculo = baseCalculo,
                        Parcela = parcela,
                        Quantidade = quantidade,
                        PrecoUnitario = precoUnitario,
                        PrecoMinimo = precoMinimo,
                        DescricaoBaseCalculo = descricaoBaseCalculo,
                        Lista = servico.Lista,
                        TipoServico = "V",
                        Margem = tela.Margem,
                        TipoCarga = tela.TipoCarga
                    });
                }
            }
            else
            {
                foreach (var servico in servicos)
                {
                    precoMinimo = servico.PrecoMinimo;
                    baseMinimo = precoMinimo;

                    if (servico.BaseCalculo == "PERI")
                    {
                        quantidade = 1;
                        preco = servico.PrecoUnitario;
                        descricaoBaseCalculo = "PERIODO";
                        baseCalculo = quantidade;
                    }

                    if (servico.BaseCalculo == "VOL")
                    {
                        quantidade = tela.VolumeM3;
                        preco = servico.PrecoUnitario;
                        descricaoBaseCalculo = "VOLUME M3";
                        baseCalculo = quantidade;
                    }

                    if (servico.BaseCalculo == "BL")
                    {
                        quantidade = 1;
                        preco = servico.PrecoUnitario;
                        baseCalculo = quantidade;
                        descricaoBaseCalculo = "BL";
                        precoUnitario = servico.PrecoUnitario;
                    }

                    if (servico.BaseCalculo == "VOLP")
                    {
                        quantidade = tela.VolumeM3;
                        baseCalculo = quantidade;

                        if ((pesoTotalCargaSolta / 1000) > quantidade)
                        {
                            quantidade = pesoTotalCargaSolta / 1000;
                            baseCalculo = quantidade;
                        }

                        preco = servico.PrecoUnitario;
                        descricaoBaseCalculo = "VOL/PESO";
                    }

                    if (servico.BaseCalculo == "UNID")
                    {
                        quantidade = qtdeTotalCargaSolta;
                        descricaoBaseCalculo = "UNIDADE";
                        baseCalculo = quantidade;

                        var valoresPorCif = _servicoDAO.ObterDetalhesServicoVariavelPorFaixas(servico.ServicoFixoVariavelId, tela.ValorCifCargaSolta, "C");

                        if (valoresPorCif != null)
                        {
                            preco = valoresPorCif.Percentual;
                            precoMinimo = valoresPorCif.ValorMinimo;
                            baseMinimo = valoresPorCif.ValorMinimo;

                            if (preco == 0)
                            {
                                preco = valoresPorCif.PrecoUnitario;
                            }
                        }
                        else
                        {
                            if (servico.PrecoUnitario > 0)
                            {
                                preco = servico.PrecoUnitario;
                            }
                        }
                    }

                    if (servico.BaseCalculo == "TON")
                    {
                        quantidade = pesoTotalCargaSolta / 1000;
                        preco = servico.PrecoUnitario;
                        descricaoBaseCalculo = "TONELAGEM";
                        baseCalculo = quantidade;
                    }

                    if (servico.BaseCalculo == "CIF" || servico.BaseCalculo == "CIFM")
                    {
                        descricaoBaseCalculo = "CIF";
                        quantidade = tela.ValorCifCargaSolta / 100;
                        baseCalculo = quantidade;

                        var valoresPorCif = _servicoDAO.ObterDetalhesServicoVariavelPorFaixas(servico.ServicoFixoVariavelId, tela.ValorCifCargaSolta, "C");

                        if (valoresPorCif != null)
                        {
                            preco = valoresPorCif.Percentual;
                            precoMinimo = valoresPorCif.ValorMinimo;
                            baseMinimo = valoresPorCif.ValorMinimo;

                            if (preco == 0)
                            {
                                preco = valoresPorCif.PrecoUnitario;
                            }
                        }
                        else
                        {
                            if (servico.PrecoUnitario > 0)
                            {
                                preco = servico.PrecoUnitario;
                            }
                        }
                    }

                    if (servico.BaseCalculo == "CIF0")
                    {
                        descricaoBaseCalculo = "CIF";
                        quantidade = (tela.ValorCifCargaSolta * 1.5M) / 100;
                        baseCalculo = tela.ValorCifCargaSolta * 1.5M;

                        var valoresPorCif = _servicoDAO.ObterDetalhesServicoVariavelPorFaixas(servico.ServicoFixoVariavelId, tela.ValorCifCargaSolta * 1.5M, "C");

                        if (valoresPorCif != null)
                        {
                            preco = valoresPorCif.Percentual;
                            precoMinimo = valoresPorCif.ValorMinimo;
                            baseMinimo = valoresPorCif.ValorMinimo;

                            if (preco == 0)
                            {
                                preco = valoresPorCif.PrecoUnitario;
                            }
                        }
                        else
                        {
                            if (servico.PrecoUnitario > 0)
                            {
                                preco = servico.PrecoUnitario;
                            }
                        }
                    }

                    precoUnitario = preco;
                    parcela = quantidade * preco;
                    precoCalculo = parcela;

                    tela.NumeroLotes = tela.NumeroLotes == 0 ? 1 : tela.NumeroLotes;

                    var valorMinimo = _servicoDAO.ObterValorMinimo(servico.ServicoFixoVariavelId, "SVAR40", tela.NumeroLotes);

                    if (valorMinimo == null)
                    {
                        valorMinimo = _servicoDAO.ObterValorMinimo(servico.ServicoFixoVariavelId, "SVAR20", tela.NumeroLotes);

                        if (valorMinimo == null)
                        {
                            valorMinimo = _servicoDAO.ObterValorMinimo(servico.ServicoFixoVariavelId, "SVAR", tela.NumeroLotes);
                        }
                    }

                    if (valorMinimo != null)
                    {
                        precoMinimo = valorMinimo.Value;
                        baseMinimo = valorMinimo.Value;
                    }

                    if (parcela < baseMinimo && baseMinimo > 0)
                    {
                        parcela = baseMinimo;
                    }

                    if (parcela > baseMaximo && baseMaximo > 0)
                    {
                        parcela = baseMaximo;
                    }

                    _simuladorDAO.GravarServicoCalculado(new ServicoCalculo
                    {
                        ServicoCalculoId = servicoSimuladorId,
                        BaseCalculo = baseCalculo,
                        Parcela = parcela,
                        Quantidade = quantidade,
                        PrecoUnitario = precoUnitario,
                        PrecoMinimo = precoMinimo,
                        DescricaoBaseCalculo = descricaoBaseCalculo,
                        Lista = servico.Lista,
                        TipoServico = "V",
                        Margem = tela.Margem,
                        TipoCarga = tela.TipoCarga
                    });
                }
            }
        }
    }
}