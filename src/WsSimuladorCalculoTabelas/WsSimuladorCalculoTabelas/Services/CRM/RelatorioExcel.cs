using Ecoporto.CRM.Business.Helpers;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using WsSimuladorCalculoTabelas.Configuracao;
using WsSimuladorCalculoTabelas.DAO;
using WsSimuladorCalculoTabelas.Enums;
using WsSimuladorCalculoTabelas.Extensions;
using WsSimuladorCalculoTabelas.Helpers;
using WsSimuladorCalculoTabelas.Models;
using WsSimuladorCalculoTabelas.Responses;

namespace WsSimuladorCalculoTabelas.Services
{
    public class RelatorioExcel
    {
        private readonly SimuladorDAO _simuladorDAO;
        private readonly ParceirosDAO _parceiroDAO;
        private readonly ImpostoDAO _impostoDAO;
        private readonly AnexosDAO _anexosDAO;
        private readonly ServicoDAO _servicoDAO;
        private readonly OportunidadeDAO _oportunidadeDAO;
        private readonly ParametrosDAO _parametrosDAO;
        private readonly ModeloDAO _modeloDAO;

        public RelatorioExcel()
        {
            _simuladorDAO = new SimuladorDAO(true);
            _parceiroDAO = new ParceirosDAO();
            _impostoDAO = new ImpostoDAO();
            _anexosDAO = new AnexosDAO();
            _oportunidadeDAO = new OportunidadeDAO();
            _servicoDAO = new ServicoDAO(true);
            _parametrosDAO = new ParametrosDAO();
            _modeloDAO = new ModeloDAO();
        }

        private readonly IDictionary<int, string> colunas = new Dictionary<int, string>()
        {
            { 1, "A" },
            { 2, "B" },
            { 3, "C" },
            { 4, "D" },
            { 5, "E" },
            { 6, "F" },
            { 7, "G" },
            { 8, "H" },
            { 9, "I" },
            { 10, "J" },
            { 11, "K" },
            { 12, "L" },
            { 13, "M" },
            { 14, "N" },
            { 15, "O" },
            { 16, "P" },
            { 17, "Q" },
            { 18, "R" },
            { 19, "S" },
            { 20, "T" },
            { 21, "U" },
            { 22, "V" },
            { 23, "W" },
            { 24, "X" },
            { 25, "Y" },
            { 26, "Z" },
        };

        public Response Gerar(GerarExcelFiltro filtro)
        {
            ExcelRange celula = null;
            Border borda = null;
            ExcelWorksheet excelWorksheet = null;
            ExcelWorksheet excelWorksheetFCL = null;

            Color corPadrao;

            Color verde20 = ColorTranslator.FromHtml("#F0FFF0");
            Color azul40 = ColorTranslator.FromHtml("#E0FFFF");

            var codigoServicoLista = new List<int>();

            int linha = 1;
            int coluna = 1;
            int totalColunas = 8;
            string tipoCliente = string.Empty;
            decimal subTotal_MD = 0.0M;
            decimal subTotal_ME = 0.0M;
            var valorImposto_MDIR = 0.0M;
            var valorImposto_MESQ = 0.0M;


            var subTotalMD_20 = 0.0M;
            var subTotalMD_40 = 0.0M;
            var subTotalME_20 = 0.0M;
            var subTotalME_40 = 0.0M;
            var valorImpostoFCL = 0.0M;

            IEnumerable<ServicoFixoVariavel> servicosSimulador;
            decimal impostoCalculoTicket = 0;
            string descricaoEmpresa = string.Empty;

            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                descricaoEmpresa = "Ecoporto Santos";
                corPadrao = ColorTranslator.FromHtml("#d8e4bc");
            }
            else
            {
                descricaoEmpresa = "Bandeirantes";
                corPadrao = ColorTranslator.FromHtml("#5DBCD2");
            }

            using (var pck = new ExcelPackage())
            {
                pck.Workbook.Properties.Author = descricaoEmpresa;
                pck.Workbook.Properties.Title = "Simulador";

                var dadosSimulador = _simuladorDAO.ObterDetalhesSimuladorPorId(filtro.SimuladorId);

                if (dadosSimulador == null)
                    throw new Exception("Simulador não encontrado");

                var parametrosSimulador = _simuladorDAO.ObterParametroSimuladorCRMPorId(filtro.ParametroSimuladorId);

                if (parametrosSimulador == null)
                    throw new Exception("A Oportunidade não possui parâmetros cadastrados");

                var oportunidadeBusca = _oportunidadeDAO.ObterOportunidadePorId(filtro.OportunidadeId);

                if (oportunidadeBusca == null)
                    throw new Exception("A Oportunidade não encontrada");

                var modeloBusca = _modeloDAO.ObterModeloPorId(oportunidadeBusca.ModeloId);

                if (modeloBusca == null)
                    throw new Exception("A Oportunidade não possui um Modelo de Proposta vínculado");

                var taxaImposto = _impostoDAO.ObterTaxaImposto(oportunidadeBusca.ImpostoId);

                var validade = $"{oportunidadeBusca.Validade} {oportunidadeBusca.TipoValidade}";

                List<string> margens = new List<string>() { "MDIR", "MESQ" };

                List<string> tipoCargas = parametrosSimulador.Regime == Regime.FCL
                    ? new List<string>() { "SVAR20", "SVAR40" }
                    : new List<string>() { "CRGST","BBK","VEIC" };

                decimal valorTotalPlanilha = 0;

                var observacoesModelo = _oportunidadeDAO.ObterObservacoesModelo(filtro.ModeloSimuladorId);

                var tabelas = _simuladorDAO.ObterTabelasCalculadas(dadosSimulador.SimuladorId);

                var ehConteiner = false;
                if (parametrosSimulador.NumeroLotes == 0)
                {
                    parametrosSimulador.NumeroLotes = 1;
                }

                if (parametrosSimulador.Periodos == 0)
                {
                    parametrosSimulador.Periodos = 1;
                }
                #region LCL

                if (parametrosSimulador.Regime == Regime.LCL)
                {
                    excelWorksheet = pck.Workbook.Worksheets.Add("Simulador LCL");

                    GravaCelula(new ExcelCelulaParametros($"Simulador de Armazenagem de Carga LCL de Importação - {descricaoEmpresa}", true, true, 64.5), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);

                    excelWorksheet.Cells[linha, 1, linha, totalColunas].Merge = true;

                    PularLinhaResetaColuna(ref linha, ref coluna);

                    GravaCelula(new ExcelCelulaParametros("Forma Pagamento", true, 50), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);
                    GravaCelula(new ExcelCelulaParametros("À Vista", false, false, 50), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);

                    excelWorksheet.Cells[linha, 2, linha, totalColunas].Merge = true;
                    excelWorksheet.Cells[linha, 2, linha, totalColunas].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    excelWorksheet.Cells[linha, 2, linha, totalColunas].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    excelWorksheet.Cells[linha, 2, linha, totalColunas].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    excelWorksheet.Cells[linha, 2, linha, totalColunas].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                    PularLinhaResetaColuna(ref linha, ref coluna);

                    GravaCelula(new ExcelCelulaParametros("Tonelada (ton):", true, 12.75, true, false), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);
                    GravaCelula(new ExcelCelulaParametros(parametrosSimulador.Peso.ToString(), true, true, corPadrao), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Numero);

                    GravaCelula(new ExcelCelulaParametros("Metro (m3):", true, 12.75, true, false), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);
                    GravaCelula(new ExcelCelulaParametros(parametrosSimulador.VolumeM3.ToString(), true, true, corPadrao), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Numero);

                    var valoresMinimoCobranca = _servicoDAO.ObterValorMinimoCobranca(oportunidadeBusca.Id);

                    if (valoresMinimoCobranca.Any())
                    {
                        GravaCelula(new ExcelCelulaParametros("N. BL", false, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);
                        GravaCelula(new ExcelCelulaParametros("3", false, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);
                    }
                    else
                    {
                        GravaCelula(new ExcelCelulaParametros("", false, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);
                        GravaCelula(new ExcelCelulaParametros("", false, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);

                        excelWorksheet.Cells[linha, 5, linha, 6].Merge = true;
                    }

                    GravaCelula(new ExcelCelulaParametros("Períodos:", true, false, false), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);
                    GravaCelula(new ExcelCelulaParametros(parametrosSimulador.Periodos.ToString(), true, true, corPadrao), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Inteiro);

                    PularLinhaResetaColuna(ref linha, ref coluna);

                    string qtdeDias = string.Empty;
                    var freeTime = _oportunidadeDAO.ObterFreeTime(oportunidadeBusca.Id);

                    foreach (var tabela in tabelas)
                    {
                        qtdeDias = _servicoDAO.ObterQuantidadeDias(tabela.TabelaId, ehConteiner);
                    }

                    GravaCelula(new ExcelCelulaParametros($"Período: {qtdeDias} dias + {freeTime} para carregamento", true, true, 50), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);

                    excelWorksheet.Cells[linha, 1, linha, totalColunas].Merge = true;
                    excelWorksheet.Cells[linha, 1, linha, totalColunas].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    excelWorksheet.Cells[linha, 1, linha, totalColunas].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    excelWorksheet.Cells[linha, 1, linha, totalColunas].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    excelWorksheet.Cells[linha, 1, linha, totalColunas].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                    PularLinhaResetaColuna(ref linha, ref coluna);

                    if (filtro.SomenteEstimativa == false)
                    {
                        GravaCelula(new ExcelCelulaParametros("Cotação emitida em:", true, true, corPadrao), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);
                        excelWorksheet.Cells[linha, 1, linha, 1].Style.Border.Bottom.Style = ExcelBorderStyle.None;

                        GravaCelula(new ExcelCelulaParametros("CIF R$", true, true, corPadrao), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);

                        excelWorksheet.Cells[linha, 2, linha, totalColunas].Merge = true;
                        excelWorksheet.Cells[linha, 2, linha, totalColunas].Style.Border.Bottom.Style = ExcelBorderStyle.None;

                        PularLinhaResetaColuna(ref linha, ref coluna);

                        GravaCelula(new ExcelCelulaParametros(DateTime.Now.ToString("dd/MM/yyyy HH:mm"), true, true, corPadrao), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);

                        if (ehConteiner)
                        {
                            GravaCelula(new ExcelCelulaParametros(string.Format("{0:C2}", dadosSimulador.ValorCifConteiner), true, true, corPadrao), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);
                        }
                        else
                        {
                            GravaCelula(new ExcelCelulaParametros(string.Format("{0:C2}", dadosSimulador.ValorCifCargaSolta), true, true, corPadrao), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);
                        }

                        excelWorksheet.Cells[linha, 2, linha, totalColunas].Merge = true;

                        excelWorksheet.Cells[linha, 1, linha, totalColunas].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        excelWorksheet.Cells[linha, 1, linha, totalColunas].Style.Border.Bottom.Style = ExcelBorderStyle.None;
                        excelWorksheet.Cells[linha, 1, linha, totalColunas].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        excelWorksheet.Cells[linha, 1, linha, totalColunas].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        PularLinhaResetaColuna(ref linha, ref coluna);

                        GravaCelula(new ExcelCelulaParametros("Serviços", true, false, true), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);

                        foreach (var tabela in tabelas)
                        {
                            GravaCelula(new ExcelCelulaParametros("% MD", true, false, true), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);
                            GravaCelula(new ExcelCelulaParametros("% ME", true, false, true), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);
                            GravaCelula(new ExcelCelulaParametros("Valor Per", true, false, true), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);
                            GravaCelula(new ExcelCelulaParametros("Mínimo MD", true, false, true), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);
                            GravaCelula(new ExcelCelulaParametros("Mínimo ME", true, false, true), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);
                            //GravaCelula(new ExcelCelulaParametros("Qt. Per.'s", true, false, true), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);
                            //GravaCelula(new ExcelCelulaParametros("", true, false, true), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);
                            GravaCelula(new ExcelCelulaParametros("Valor - MD", true, false, true), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);
                            GravaCelula(new ExcelCelulaParametros("Valor - ME", true, true, true, corPadrao), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);
                        }

                        PularLinhaResetaColuna(ref linha, ref coluna);

                        for (int periodo = 1; periodo <= 3; periodo++)
                        {
                            coluna = 0;

                            foreach (var tabela in tabelas)
                            {
                                if (coluna == 0)
                                {
                                    coluna = 1;
                                    GravaCelula(new ExcelCelulaParametros($"{periodo}º Período", false, false, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);
                                }

                                var valoresArmazenagemMD = _servicoDAO.ObterValoresServicoArmazenagem(tabela.TabelaId, periodo, ehConteiner, "", "MDIR");

                                if (valoresArmazenagemMD == null)
                                    valoresArmazenagemMD = _servicoDAO.ObterValoresServicoArmazenagem(tabela.TabelaId, periodo, ehConteiner, "", "SVAR");

                                var valoresArmazenagemME = _servicoDAO.ObterValoresServicoArmazenagem(tabela.TabelaId, periodo, ehConteiner, "", "MESQ");

                                if (valoresArmazenagemME == null)
                                    valoresArmazenagemME = _servicoDAO.ObterValoresServicoArmazenagem(tabela.TabelaId, periodo, ehConteiner, "", "SVAR");

                                if (valoresArmazenagemMD == null && valoresArmazenagemME == null)
                                {
                                    GravaCelula(new ExcelCelulaParametros(String.Format("{0:P2}", 0), false, true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);
                                    GravaCelula(new ExcelCelulaParametros(String.Format("{0:P2}", 0), false, true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);
                                    GravaCelula(new ExcelCelulaParametros(String.Format("{0:C2}", 0), false, true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);
                                    GravaCelula(new ExcelCelulaParametros(String.Format("{0:C2}", 0), false, true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);
                                    GravaCelula(new ExcelCelulaParametros(String.Format("{0:C2}", 0), false, true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);
                                    //GravaCelula(new ExcelCelulaParametros("0", false, true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);
                                    GravaCelula(new ExcelCelulaParametros(String.Format("{0:C2}", 0), false, true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);
                                    GravaCelula(new ExcelCelulaParametros(String.Format("{0:C2}", 0), false, true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);

                                    PularLinhaResetaColuna(ref linha, ref coluna);

                                    continue;
                                }

                                var precoUnitarioMD = valoresArmazenagemMD.PrecoUnitario / 100;
                                var precoUnitarioME = valoresArmazenagemME.PrecoUnitario / 100;

                                var valorper = precoUnitarioMD * dadosSimulador.ValorCifCargaSolta;


                                var valorMinimoMD = _servicoDAO.ObterValorMinimo(valoresArmazenagemMD.ServicoFixoVariavelId, "SVAR40", valoresArmazenagemMD.LimiteBls);

                                if (valorMinimoMD == null)
                                {
                                    valorMinimoMD = _servicoDAO.ObterValorMinimo(valoresArmazenagemMD.ServicoFixoVariavelId, "SVAR20", valoresArmazenagemMD.LimiteBls);

                                    if (valorMinimoMD == null)
                                    {
                                        valorMinimoMD = _servicoDAO.ObterValorMinimo(valoresArmazenagemMD.ServicoFixoVariavelId, "SVAR", valoresArmazenagemMD.LimiteBls);

                                        if (valorMinimoMD == null)
                                        {
                                            valorMinimoMD = 0;
                                        }
                                    }
                                }

                                var valorMinimoME = _servicoDAO.ObterValorMinimo(valoresArmazenagemME.ServicoFixoVariavelId, "SVAR40", valoresArmazenagemMD.LimiteBls);

                                if (valorMinimoME == null)
                                {
                                    valorMinimoME = _servicoDAO.ObterValorMinimo(valoresArmazenagemME.ServicoFixoVariavelId, "SVAR20", valoresArmazenagemMD.LimiteBls);

                                    if (valorMinimoME == null)
                                    {
                                        valorMinimoME = _servicoDAO.ObterValorMinimo(valoresArmazenagemME.ServicoFixoVariavelId, "SVAR", valoresArmazenagemMD.LimiteBls);

                                        if (valorMinimoME == null)
                                        {
                                            valorMinimoME = 0;
                                        }
                                    }
                                }

                                if (valoresArmazenagemMD.BaseCalculo == "CIF" || valoresArmazenagemMD.BaseCalculo == "CIF0" || valoresArmazenagemMD.BaseCalculo == "CIFM")
                                {
                                    GravaCelula(new ExcelCelulaParametros(precoUnitarioMD.ToString(), false, true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Percentual);
                                    GravaCelula(new ExcelCelulaParametros(precoUnitarioME.ToString(), false, true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Percentual);

                                    GravaCelula(new ExcelCelulaParametros(String.Format("{0:N2}", valorper), false, true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                                    excelWorksheet.Cells[linha, coluna - 1].Formula = $"=C{linha}*B6";
                                    if (valoresMinimoCobranca.Any())
                                    {
                                        GravaCelula(new ExcelCelulaParametros(string.Empty, false, true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                                        excelWorksheet.Cells[linha, coluna - 1].Formula = $"IF(F3>2,M6,M7)";

                                        GravaCelula(new ExcelCelulaParametros(string.Empty, false, true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                                        excelWorksheet.Cells[linha, coluna - 1].Formula = $"IF(F3>2,M6,M7)";
                                    }
                                    else
                                    {
                                        GravaCelula(new ExcelCelulaParametros(valorMinimoMD.ToString(), false, true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                                        GravaCelula(new ExcelCelulaParametros(valorMinimoME.ToString(), false, true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                                    }
                                    //GravaCelula(new ExcelCelulaParametros(valoresArmazenagemMD.Periodo.ToString(), false, true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Inteiro);
                                    //GravaCelula(new ExcelCelulaParametros(string.Empty, false, true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);

                                    GravaCelula(new ExcelCelulaParametros(string.Empty, false, true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                                    excelWorksheet.Cells[linha, coluna - 1].Formula = $"IF((B{linha}*B6)>E{linha},(B{linha}*B6),E{linha})";

                                    GravaCelula(new ExcelCelulaParametros(string.Empty, false, true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                                    excelWorksheet.Cells[linha, coluna - 1].Formula = $"IF((C{linha}*B6)>F{linha},(C{linha}*B6),F{linha})";
                                }
                                else
                                {
                                    GravaCelula(new ExcelCelulaParametros(valoresArmazenagemMD.PrecoUnitario.ToString(), false, true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                                    GravaCelula(new ExcelCelulaParametros(valoresArmazenagemME.PrecoUnitario.ToString(), false, true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                                    GravaCelula(new ExcelCelulaParametros(String.Format("{0:N2}", 0), false, true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                                    if (valoresMinimoCobranca.Any())
                                    {
                                        GravaCelula(new ExcelCelulaParametros(string.Empty, false, true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                                        excelWorksheet.Cells[linha, coluna - 1].Formula = $"IF(F3>2,M6,M7)";

                                        GravaCelula(new ExcelCelulaParametros(string.Empty, false, true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                                        excelWorksheet.Cells[linha, coluna - 1].Formula = $"IF(F3>2,M6,M7)";

                                    }
                                    else
                                    {
                                        GravaCelula(new ExcelCelulaParametros(valorMinimoMD.ToString(), false, true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                                        GravaCelula(new ExcelCelulaParametros(valorMinimoME.ToString(), false, true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                                    }
                                    //GravaCelula(new ExcelCelulaParametros(string.Empty, false, true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);

                                    GravaCelula(new ExcelCelulaParametros(string.Empty, false, true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                                    excelWorksheet.Cells[linha, coluna - 1].Formula = $"IF((B{linha}*B6)>E{linha},(B{linha}*B6),E{linha})";

                                    GravaCelula(new ExcelCelulaParametros(string.Empty, false, true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                                    excelWorksheet.Cells[linha, coluna - 1].Formula = $"IF((C{linha}*B6)>F{linha},(C{linha}*B6),F{linha})";
                                }
                            }

                            PularLinhaResetaColuna(ref linha, ref coluna);
                        }

                        GravaCelula(new ExcelCelulaParametros("Subsequentes", false, false, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);

                        foreach (var tabela in tabelas)
                        {
                            for (int periodo = dadosSimulador.Periodos + 3; periodo >= 1; periodo += -1)
                            {
                                var valoresArmazenagemMD = _servicoDAO.ObterValoresServicoArmazenagem(tabela.TabelaId, periodo, ehConteiner, "", "MDIR");

                                if (valoresArmazenagemMD == null)
                                    valoresArmazenagemMD = _servicoDAO.ObterValoresServicoArmazenagem(tabela.TabelaId, periodo, ehConteiner, "", "SVAR");

                                var valoresArmazenagemME = _servicoDAO.ObterValoresServicoArmazenagem(tabela.TabelaId, periodo, ehConteiner, "", "MESQ");

                                if (valoresArmazenagemME == null)
                                    valoresArmazenagemME = _servicoDAO.ObterValoresServicoArmazenagem(tabela.TabelaId, periodo, ehConteiner, "", "SVAR");

                                if (valoresArmazenagemMD == null && valoresArmazenagemME == null)
                                {
                                    continue;
                                }

                                var precoUnitarioMD = valoresArmazenagemMD.PrecoUnitario / 100;
                                var precoUnitarioME = valoresArmazenagemME.PrecoUnitario / 100;

                                var valorper = precoUnitarioMD * dadosSimulador.ValorCifCargaSolta;

                                var valorMinimoMD = _servicoDAO.ObterValorMinimo(valoresArmazenagemMD.ServicoFixoVariavelId, "SVAR40", valoresArmazenagemMD.LimiteBls);

                                if (valorMinimoMD == null)
                                {
                                    valorMinimoMD = _servicoDAO.ObterValorMinimo(valoresArmazenagemMD.ServicoFixoVariavelId, "SVAR20", valoresArmazenagemMD.LimiteBls);

                                    if (valorMinimoMD == null)
                                    {
                                        valorMinimoMD = _servicoDAO.ObterValorMinimo(valoresArmazenagemMD.ServicoFixoVariavelId, "SVAR", valoresArmazenagemMD.LimiteBls);

                                        if (valorMinimoMD == null)
                                        {
                                            valorMinimoMD = 0;
                                        }
                                    }
                                }

                                var valorMinimoME = _servicoDAO.ObterValorMinimo(valoresArmazenagemME.ServicoFixoVariavelId, "SVAR40", valoresArmazenagemMD.LimiteBls);

                                if (valorMinimoME == null)
                                {
                                    valorMinimoME = _servicoDAO.ObterValorMinimo(valoresArmazenagemME.ServicoFixoVariavelId, "SVAR20", valoresArmazenagemMD.LimiteBls);

                                    if (valorMinimoME == null)
                                    {
                                        valorMinimoME = _servicoDAO.ObterValorMinimo(valoresArmazenagemME.ServicoFixoVariavelId, "SVAR", valoresArmazenagemMD.LimiteBls);

                                        if (valorMinimoME == null)
                                        {
                                            valorMinimoME = 0;
                                        }
                                    }
                                }

                                if (valoresArmazenagemMD.BaseCalculo == "CIF" || valoresArmazenagemMD.BaseCalculo == "CIF0" || valoresArmazenagemMD.BaseCalculo == "CIFM")
                                {
                                    GravaCelula(new ExcelCelulaParametros(precoUnitarioMD.ToString(), false, true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Percentual);
                                    GravaCelula(new ExcelCelulaParametros(precoUnitarioME.ToString(), false, true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Percentual);

                                    GravaCelula(new ExcelCelulaParametros(String.Format("{0:N2}", valorper), false, true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                                    excelWorksheet.Cells[linha, coluna - 1].Formula = $"=C{linha}*B6";

                                    if (valoresMinimoCobranca.Any())
                                    {
                                        GravaCelula(new ExcelCelulaParametros(string.Empty, false, true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                                        excelWorksheet.Cells[linha, coluna - 1].Formula = $"IF(F3>2,M6,M7)";

                                        GravaCelula(new ExcelCelulaParametros(string.Empty, false, true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                                        excelWorksheet.Cells[linha, coluna - 1].Formula = $"IF(F3>2,M6,M7)";
                                    }
                                    else
                                    {

                                        GravaCelula(new ExcelCelulaParametros(valorMinimoMD.ToString(), false, true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                                        GravaCelula(new ExcelCelulaParametros(valorMinimoME.ToString(), false, true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);

                                    }                                    //GravaCelula(new ExcelCelulaParametros(valoresArmazenagemMD.Periodo.ToString(), false, true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Inteiro);
                                    //GravaCelula(new ExcelCelulaParametros(string.Empty, false, true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);

                                    GravaCelula(new ExcelCelulaParametros(string.Empty, false, true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                                    excelWorksheet.Cells[linha, coluna - 1].Formula = $"IF((B{linha}*B6)>E{linha},(B{linha}*B6),E{linha})";

                                    GravaCelula(new ExcelCelulaParametros(string.Empty, false, true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                                    excelWorksheet.Cells[linha, coluna - 1].Formula = $"IF((C{linha}*B6)>F{linha},(C{linha}*B6),F{linha})";
                                }
                                else
                                {
                                    GravaCelula(new ExcelCelulaParametros(valoresArmazenagemMD.PrecoUnitario.ToString(), false, true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                                    GravaCelula(new ExcelCelulaParametros(valoresArmazenagemME.PrecoUnitario.ToString(), false, true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                                    GravaCelula(new ExcelCelulaParametros(String.Format("{0:N2}", 0), false, true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                                    if (valoresMinimoCobranca.Any())
                                    {
                                        GravaCelula(new ExcelCelulaParametros(string.Empty, false, true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                                        excelWorksheet.Cells[linha, coluna - 1].Formula = $"IF(F3>2,M6,M7)";

                                        GravaCelula(new ExcelCelulaParametros(string.Empty, false, true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                                        excelWorksheet.Cells[linha, coluna - 1].Formula = $"IF(F3>2,M6,M7)";
                                    }
                                    else
                                    {

                                        GravaCelula(new ExcelCelulaParametros(valorMinimoMD.ToString(), false, true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                                        GravaCelula(new ExcelCelulaParametros(valorMinimoME.ToString(), false, true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);

                                    }
                                    //GravaCelula(new ExcelCelulaParametros(string.Empty, false, true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);

                                    GravaCelula(new ExcelCelulaParametros(string.Empty, false, true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                                    excelWorksheet.Cells[linha, coluna - 1].Formula = $"IF((B{linha}*B6)>E{linha},(B{linha}*B6),E{linha})";

                                    GravaCelula(new ExcelCelulaParametros(string.Empty, false, true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                                    excelWorksheet.Cells[linha, coluna - 1].Formula = $"IF((C{linha}*B6)>F{linha},(C{linha}*B6),F{linha})";
                                }

                                break;

                            }
                        }

                        PularLinhaResetaColuna(ref linha, ref coluna);
                    }

                    var linhaPrimeiroServico = 0;

                    GravaCelula(new ExcelCelulaParametros($"ESTIMATIVA", true, true, true), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);

                    for (int i = 1; i <= totalColunas; i++)
                        PintarLinha(ref excelWorksheet, ref celula, ref linha, i);

                    PularLinhaResetaColuna(ref linha, ref coluna);

                    taxaImposto = 1 - taxaImposto;

                    servicosSimulador = _simuladorDAO.ObterServicosSimulador(dadosSimulador.SimuladorId,1);

                    foreach (var servico in servicosSimulador)
                    {
                        GravaCelula(new ExcelCelulaParametros(servico.Descricao, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);

                        if (linhaPrimeiroServico == 0)
                        {
                            linhaPrimeiroServico = linha;
                        }

                        foreach (var tabela in tabelas)
                        {
                            var valorMDir = _servicoDAO.ObterValoresServicoSimuladorPorTabelaId(dadosSimulador.SimuladorId, tabela.TabelaId, servico.ServicoId, "MDIR","CRGST");
                            var valorMEsq = _servicoDAO.ObterValoresServicoSimuladorPorTabelaId(dadosSimulador.SimuladorId, tabela.TabelaId, servico.ServicoId, "MESQ","CRGST");

                            string descricaoBaseCalculo = string.Empty;

                            descricaoBaseCalculo = $"{valorMDir.BaseCalculo} com mín de:";

                            if (servico.BaseCalculo == "BL")
                                descricaoBaseCalculo = "por lote";

                            if (servico.BaseCalculo == "CIF")
                                descricaoBaseCalculo = "Sobre o CIF com mín de:";

                            if (servico.BaseCalculo == "VOLUME M3")
                                descricaoBaseCalculo = "TON/M3 com mín de:";

                            if (servico.ServicoId != 52)
                            {
                                if (servico.BaseCalculo == "CIF")
                                {
                                    var valorMdirCif = valorMDir.PrecoUnitario / 100;
                                    var valorMEsqCif = valorMEsq.PrecoUnitario / 100;

                                    GravaCelula(new ExcelCelulaParametros(valorMdirCif.ToString(), false, true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Percentual);
                                    GravaCelula(new ExcelCelulaParametros(valorMEsqCif.ToString(), false, true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Percentual);
                                }
                                else
                                {
                                    if (servico.BaseCalculo == "BL")
                                    {
                                        GravaCelula(new ExcelCelulaParametros("", false, true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);
                                        GravaCelula(new ExcelCelulaParametros("", false, true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);
                                    }
                                    else
                                    {
                                        GravaCelula(new ExcelCelulaParametros(valorMDir.PrecoUnitario.ToString(), false, true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                                        GravaCelula(new ExcelCelulaParametros(valorMEsq.PrecoUnitario.ToString(), false, true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                                    }
                                }
                            }
                            else
                            {
                                GravaCelula(new ExcelCelulaParametros(string.Empty, false, true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);
                                GravaCelula(new ExcelCelulaParametros(string.Empty, false, true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);
                            }

                            GravaCelula(new ExcelCelulaParametros(descricaoBaseCalculo, false, true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);

                            if (servico.ServicoId == 52)
                            {
                                GravaCelula(new ExcelCelulaParametros(string.Empty, false, true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                                excelWorksheet.Cells[linha, coluna - 1].Formula = $"IF(H3=1,E8,IF(H3=2,E9,IF(H3=3,E10,IF(H3>=4,E11,0))))";

                                GravaCelula(new ExcelCelulaParametros(string.Empty, false, true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                                excelWorksheet.Cells[linha, coluna - 1].Formula = $"IF(H3=1,F8,IF(H3=2,F9,IF(H3=3,F10,IF(H3>=4,F11,0))))";
                            }
                            else
                            {
                                if (servico.BaseCalculo == "BL")
                                {
                                    GravaCelula(new ExcelCelulaParametros(valorMDir.PrecoUnitario.ToString(), false, true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                                    GravaCelula(new ExcelCelulaParametros(valorMEsq.PrecoUnitario.ToString(), false, true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                                }
                                else
                                {
                                    GravaCelula(new ExcelCelulaParametros(servico.PrecoMinimoMDir.ToString(), false, true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                                    GravaCelula(new ExcelCelulaParametros(servico.PrecoMinimoMEsq.ToString(), false, true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                                }
                            }

                            //GravaCelula(new ExcelCelulaParametros(string.Empty, false, true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);

                            if (servico.BaseCalculo == "CIF")
                            {
                                if (servico.ServicoId == 52)
                                {
                                    GravaCelula(new ExcelCelulaParametros(valorMDir.ValorFinal.ToString(), true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                                    excelWorksheet.Cells[linha, coluna - 1].Formula = $"IF(H3=0, 0, IF(H3=1, G8, IF(H3=2, G8+G9, IF(H3=3, G8+G9+G10, IF(H3=4, G8+G9+G10+G11, IF(H3>4,G8+G9+G10+G11+(G11*(H3-4))))))))";

                                    GravaCelula(new ExcelCelulaParametros(valorMEsq.ValorFinal.ToString(), true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                                    excelWorksheet.Cells[linha, coluna - 1].Formula = $"IF(H3=0, 0, IF(H3=1, H8, IF(H3=2, H8+H9, IF(H3=3, H8+H9+H10, IF(H3=4, H8+H9+H10+H11, IF(H3>4,H8+H9+H10+H11+(H11*(H3-4))))))))";
                                }
                                else
                                {
                                    if (servico.ServicoId == 295)
                                    {
                                        GravaCelula(new ExcelCelulaParametros(valorMDir.ValorFinal.ToString(), true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                                        excelWorksheet.Cells[linha, coluna - 1].Formula = $"IF(B{linha}*(B6 * 1.5)>E{linha},B{linha}*(B6*1.5),E{linha})*(IF(H3>0,H3-1,0))";

                                        GravaCelula(new ExcelCelulaParametros(valorMDir.ValorFinal.ToString(), true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                                        excelWorksheet.Cells[linha, coluna - 1].Formula = $"IF(C{linha}*(B6 * 1.5)>F{linha},C{linha}*(B6*1.5),F{linha})*(IF(H3>0,H3-1,0))";
                                    }
                                    else
                                    {
                                        GravaCelula(new ExcelCelulaParametros(valorMDir.ValorFinal.ToString(), true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                                        excelWorksheet.Cells[linha, coluna - 1].Formula = $"IF(B{linha}*B6>E{linha},B{linha}*B6,E{linha})*H3";

                                        GravaCelula(new ExcelCelulaParametros(valorMEsq.ValorFinal.ToString(), true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                                        excelWorksheet.Cells[linha, coluna - 1].Formula = $"IF(C{linha}*B6>F{linha},C{linha}*B6,F{linha})*H3";
                                    }
                                }
                            }
                            else
                            {
                                if (servico.BaseCalculo == "VOLUME" || servico.BaseCalculo == "VOLUME M3")
                                {
                                    GravaCelula(new ExcelCelulaParametros(string.Empty, false, true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                                    excelWorksheet.Cells[linha, coluna - 1].Formula = $"IF(B{linha}*IF(D3>B3,D3,B3)>E{linha},B{linha}*IF(D3>B3,D3,B3),E{linha})";

                                    GravaCelula(new ExcelCelulaParametros(string.Empty, false, true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                                    excelWorksheet.Cells[linha, coluna - 1].Formula = $"IF(C{linha}*IF(D3>B3,D3,B3)>F{linha},C{linha}*IF(D3>B3,D3,B3),F{linha})";
                                }
                                else if (servico.BaseCalculo == "BL")
                                {
                                    GravaCelula(new ExcelCelulaParametros(string.Empty, false, true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                                    excelWorksheet.Cells[linha, coluna - 1].Formula = $"=E{linha}";

                                    GravaCelula(new ExcelCelulaParametros(string.Empty, false, true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                                    excelWorksheet.Cells[linha, coluna - 1].Formula = $"=F{linha}";
                                }
                                else if (servico.BaseCalculo == "TON")
                                {
                                    GravaCelula(new ExcelCelulaParametros(string.Empty, false, true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                                    excelWorksheet.Cells[linha, coluna - 1].Formula = $"IF(B{linha}*IF(D3>B3,D3,B3)>E{linha},B{linha}*IF(D3>B3,D3,B3),E{linha})";

                                    GravaCelula(new ExcelCelulaParametros(string.Empty, false, true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                                    excelWorksheet.Cells[linha, coluna - 1].Formula = $"IF(C{linha}*IF(D3>B3,D3,B3)>F{linha},C{linha}*IF(D3>B3,D3,B3),F{linha})";
                                }
                            }

                            subTotal_MD += valorMDir.ValorFinal;
                            subTotal_ME += valorMEsq.ValorFinal;
                        }

                        PularLinhaResetaColuna(ref linha, ref coluna);
                    }

                    var linhaSubTotal = 0;
                    var linhaTributo = 0;

                    GravaCelula(new ExcelCelulaParametros("Sub Total:", true, 12.75, true), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);

                    foreach (var tabela in tabelas)
                    {
                        for (int i = 1; i <= 5; i++)
                            GravaCelula(new ExcelCelulaParametros(string.Empty, true, 12.75, true), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);

                        GravaCelula(new ExcelCelulaParametros(string.Empty, true, 12.75, true), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                        excelWorksheet.Cells[linha, coluna - 1].Formula = $"SUM(G{linhaPrimeiroServico}:G{linha - 1})";

                        GravaCelula(new ExcelCelulaParametros(string.Empty, true, 12.75, true), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                        excelWorksheet.Cells[linha, coluna - 1].Formula = $"SUM(H{linhaPrimeiroServico}:H{linha - 1})";

                        if (linhaSubTotal == 0)
                            linhaSubTotal = linha;
                    }

                    PularLinhaResetaColuna(ref linha, ref coluna);

                    GravaCelula(new ExcelCelulaParametros("Tributos (repasse):", true, 12.75, false), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);

                    valorImposto_MDIR = 0.0M;
                    valorImposto_MESQ = 0.0M;

                    if (oportunidadeBusca.ImpostoId != 2)
                    {
                        foreach (var tabela in tabelas)
                        {
                            valorImposto_MDIR = _simuladorDAO.ObterValorImposto(taxaImposto, dadosSimulador.SimuladorId, tabela.TabelaId, "MDIR","CRGST");
                            valorImposto_MESQ = _simuladorDAO.ObterValorImposto(taxaImposto, dadosSimulador.SimuladorId, tabela.TabelaId, "MESQ","CRGST");
                            decimal valorPorcentagem_MDIR = 0;
                            decimal valorPorcentagem_MESQ = 0;
                            if (subTotal_MD > 0)
                            {
                                valorPorcentagem_MDIR = Math.Round(valorImposto_MDIR / subTotal_MD, 6);
                            }
                            if (subTotal_ME > 0)
                            {
                             valorPorcentagem_MESQ = Math.Round(valorImposto_MESQ / subTotal_ME, 6);
                            }
                            if (valorPorcentagem_MDIR == 0)
                            {
                                valorPorcentagem_MDIR = (1 / taxaImposto) - 1;
                            }

                            if (valorPorcentagem_MESQ == 0)
                            {
                                valorPorcentagem_MESQ = (1 / taxaImposto) - 1;
                            }

                            GravaCelula(new ExcelCelulaParametros(valorPorcentagem_MDIR.ToString(), true, 12.75, false), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Percentual);
                            impostoCalculoTicket = valorPorcentagem_MDIR;

                            for (int i = 1; i <= 4; i++)
                                GravaCelula(new ExcelCelulaParametros(string.Empty, true, 12.75, false), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);

                            GravaCelula(new ExcelCelulaParametros(string.Empty, true, 12.75, true), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                            excelWorksheet.Cells[linha, coluna - 1].Formula = $"=(G{linhaSubTotal}*B{linha})";

                            GravaCelula(new ExcelCelulaParametros(string.Empty, true, 12.75, true), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                            excelWorksheet.Cells[linha, coluna - 1].Formula = $"=(H{linhaSubTotal}*B{linha})";

                            if (linhaTributo == 0)
                                linhaTributo = linha;
                        }
                    }
                    else
                    {
                        GravaCelula(new ExcelCelulaParametros(string.Empty, true, 12.75, false), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Percentual);

                        for (int i = 1; i <= 4; i++)
                            GravaCelula(new ExcelCelulaParametros(string.Empty, true, 12.75, false), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);

                        GravaCelula(new ExcelCelulaParametros(string.Empty, true, 12.75, false), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                        GravaCelula(new ExcelCelulaParametros(string.Empty, true, 12.75, false), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                    }

                    PularLinhaResetaColuna(ref linha, ref coluna);

                    GravaCelula(new ExcelCelulaParametros("Total:", true, 12.75, true), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);

                    foreach (var tabela in tabelas)
                    {
                        for (int i = 1; i <= 5; i++)
                            GravaCelula(new ExcelCelulaParametros(string.Empty, true, 12.75, true), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);

                        GravaCelula(new ExcelCelulaParametros(String.Empty, true, 12.75, true), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                        excelWorksheet.Cells[linha, coluna - 1].Formula = $"SUM(G{linhaSubTotal}:G{linhaTributo})";

                        GravaCelula(new ExcelCelulaParametros(string.Empty, true, 12.75, true), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                        excelWorksheet.Cells[linha, coluna - 1].Formula = $"SUM(H{linhaSubTotal}:H{linhaTributo})";
                    }

                    AjustarLarguraColuna(ref excelWorksheet, coluna: 1, larguraEmPixels: 50);

                    for (int i = 1; i <= totalColunas; i++)
                        AjustarLarguraColuna(ref excelWorksheet, coluna: i, larguraEmPixels: 26);

                    excelWorksheet.Cells[1, totalColunas].Style.WrapText = true;

                    if (excelWorksheet.Dimension != null)
                        excelWorksheet.Cells[excelWorksheet.Dimension.Address].AutoFitColumns();

                    excelWorksheet.Cells[$"G5:H{linha}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    excelWorksheet.Cells[$"G5:H{linha}"].Style.Fill.BackgroundColor.SetColor(corPadrao);

                    AdicionarImagem(excelWorksheet, 1, 1, "");

                    for (int i = 1; i <= 2; i++)
                        PularLinhaResetaColuna(ref linha, ref coluna);

                    GravaCelula(new ExcelCelulaParametros("Serviços Complementares:", true, false, false), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);

                    excelWorksheet.Cells[linha, 1, linha, totalColunas].Merge = true;

                    excelWorksheet.Cells[linha, 1, linha, totalColunas].Style.Border.Top.Style = ExcelBorderStyle.None;
                    excelWorksheet.Cells[linha, 1, linha, totalColunas].Style.Border.Bottom.Style = ExcelBorderStyle.None;
                    excelWorksheet.Cells[linha, 1, linha, totalColunas].Style.Border.Left.Style = ExcelBorderStyle.None;
                    excelWorksheet.Cells[linha, 1, linha, totalColunas].Style.Border.Right.Style = ExcelBorderStyle.None;

                    for (int i = 1; i <= 2; i++)
                        PularLinhaResetaColuna(ref linha, ref coluna);

                    var servicosComplementares = _simuladorDAO.ObterServicosComplementares(parametrosSimulador.ModeloSimuladorId, filtro.OportunidadeId, 1);

                    foreach (var servicoComplementar in servicosComplementares)
                    {
                        var baseCalculoServicoComplementar = string.Empty;

                        switch (servicoComplementar.BaseCalculo.ToUpper())
                        {
                            case "TON":
                                baseCalculoServicoComplementar = "por tonelagem";
                                break;
                            case "VOL":
                                baseCalculoServicoComplementar = "por volume";
                                break;
                            case "VOLP":
                                baseCalculoServicoComplementar = "por volume/tonelagem";
                                break;
                            case "BL":
                                baseCalculoServicoComplementar = "por lote";
                                break;
                            case "CIF":
                                baseCalculoServicoComplementar = "por CIF";
                                break;
                        }

                        var minimoServicoComplementar = string.Empty;

                        if (servicoComplementar.PrecoMinimo > 0)
                            minimoServicoComplementar = $"com mínimo de {string.Format("{0:C2}", servicoComplementar.PrecoMinimo)}";

                        GravaCelula(new ExcelCelulaParametros(string.Concat(servicoComplementar.Descricao.Capitalize(), ": ", string.Format("{0:C2}", servicoComplementar.PrecoUnitario), " ", baseCalculoServicoComplementar, " ", minimoServicoComplementar), false, false, false), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);

                        excelWorksheet.Cells[linha, 1, linha, totalColunas].Merge = true;

                        excelWorksheet.Cells[linha, 1, linha, totalColunas].Style.Border.Top.Style = ExcelBorderStyle.None;
                        excelWorksheet.Cells[linha, 1, linha, totalColunas].Style.Border.Bottom.Style = ExcelBorderStyle.None;
                        excelWorksheet.Cells[linha, 1, linha, totalColunas].Style.Border.Left.Style = ExcelBorderStyle.None;
                        excelWorksheet.Cells[linha, 1, linha, totalColunas].Style.Border.Right.Style = ExcelBorderStyle.None;

                        PularLinhaResetaColuna(ref linha, ref coluna);
                    }

                    foreach (var tabela in tabelas)
                    {
                        var adicionalArmazenagem = _servicoDAO.ObterAdicionalArmazenagem(tabela.TabelaId, 1);

                        GravaCelula(new ExcelCelulaParametros($"Adicional Armazenagem: { adicionalArmazenagem.ToString("0.##") }% a partir do segundo período sobre 1,5 do CIF por período;", false, false, false), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);

                        excelWorksheet.Cells[linha, 1, linha, totalColunas].Merge = true;

                        excelWorksheet.Cells[linha, 1, linha, totalColunas].Style.Border.Top.Style = ExcelBorderStyle.None;
                        excelWorksheet.Cells[linha, 1, linha, totalColunas].Style.Border.Bottom.Style = ExcelBorderStyle.None;
                        excelWorksheet.Cells[linha, 1, linha, totalColunas].Style.Border.Left.Style = ExcelBorderStyle.None;
                        excelWorksheet.Cells[linha, 1, linha, totalColunas].Style.Border.Right.Style = ExcelBorderStyle.None;

                        PularLinhaResetaColuna(ref linha, ref coluna);



                        var adicionaisAnvisa = _servicoDAO.ObterAdicionaisAnvisa(tabela.TabelaId, 1);

                        if (adicionaisAnvisa.Any())
                        {
                            foreach (var adicionalAnvisa in adicionaisAnvisa)
                            {
                                GravaCelula(new ExcelCelulaParametros($"Adicional ANVISA {adicionalAnvisa.Descricao.Capitalize()}: { adicionalAnvisa.ValorAnvisa.ToString("0.##") }%", false, false, false), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);

                                excelWorksheet.Cells[linha, 1, linha, totalColunas].Merge = true;

                                excelWorksheet.Cells[linha, 1, linha, totalColunas].Style.Border.Top.Style = ExcelBorderStyle.None;
                                excelWorksheet.Cells[linha, 1, linha, totalColunas].Style.Border.Bottom.Style = ExcelBorderStyle.None;
                                excelWorksheet.Cells[linha, 1, linha, totalColunas].Style.Border.Left.Style = ExcelBorderStyle.None;
                                excelWorksheet.Cells[linha, 1, linha, totalColunas].Style.Border.Right.Style = ExcelBorderStyle.None;

                                PularLinhaResetaColuna(ref linha, ref coluna);
                            }
                        }
                        else
                        {
                            GravaCelula(new ExcelCelulaParametros($"Adicional ANVISA: 0%", false, false, false), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);

                            excelWorksheet.Cells[linha, 1, linha, totalColunas].Merge = true;

                            excelWorksheet.Cells[linha, 1, linha, totalColunas].Style.Border.Top.Style = ExcelBorderStyle.None;
                            excelWorksheet.Cells[linha, 1, linha, totalColunas].Style.Border.Bottom.Style = ExcelBorderStyle.None;
                            excelWorksheet.Cells[linha, 1, linha, totalColunas].Style.Border.Left.Style = ExcelBorderStyle.None;
                            excelWorksheet.Cells[linha, 1, linha, totalColunas].Style.Border.Right.Style = ExcelBorderStyle.None;

                            PularLinhaResetaColuna(ref linha, ref coluna);
                        }

                        var adicionais = _servicoDAO.ObterAdicionais(tabela.TabelaId, 1);

                        if (adicionais.Any())
                        {
                            foreach (var adicional in adicionais)
                            {
                                GravaCelula(new ExcelCelulaParametros($"Adicional IMO {adicional.Descricao.Capitalize()}: { adicional.ValorAcrescimo.ToString("0.##") }%", false, false, false), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);

                                excelWorksheet.Cells[linha, 1, linha, totalColunas].Merge = true;

                                excelWorksheet.Cells[linha, 1, linha, totalColunas].Style.Border.Top.Style = ExcelBorderStyle.None;
                                excelWorksheet.Cells[linha, 1, linha, totalColunas].Style.Border.Bottom.Style = ExcelBorderStyle.None;
                                excelWorksheet.Cells[linha, 1, linha, totalColunas].Style.Border.Left.Style = ExcelBorderStyle.None;
                                excelWorksheet.Cells[linha, 1, linha, totalColunas].Style.Border.Right.Style = ExcelBorderStyle.None;

                                PularLinhaResetaColuna(ref linha, ref coluna);
                            }
                        }
                        else
                        {
                            GravaCelula(new ExcelCelulaParametros($"Adicional IMO: 0%", false, false, false), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);

                            excelWorksheet.Cells[linha, 1, linha, totalColunas].Merge = true;

                            excelWorksheet.Cells[linha, 1, linha, totalColunas].Style.Border.Top.Style = ExcelBorderStyle.None;
                            excelWorksheet.Cells[linha, 1, linha, totalColunas].Style.Border.Bottom.Style = ExcelBorderStyle.None;
                            excelWorksheet.Cells[linha, 1, linha, totalColunas].Style.Border.Left.Style = ExcelBorderStyle.None;
                            excelWorksheet.Cells[linha, 1, linha, totalColunas].Style.Border.Right.Style = ExcelBorderStyle.None;

                            PularLinhaResetaColuna(ref linha, ref coluna);
                        }

                        GravaCelula(new ExcelCelulaParametros("Produtos controlados pelo Exército, Polícias Federal e Civil: Adicional de 35 % sobre os itens: Armazenagem e Gerenciamento de Risco;", false, false, false), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);

                        excelWorksheet.Cells[linha, 1, linha, totalColunas].Merge = true;

                        excelWorksheet.Cells[linha, 1, linha, totalColunas].Style.Border.Top.Style = ExcelBorderStyle.None;
                        excelWorksheet.Cells[linha, 1, linha, totalColunas].Style.Border.Bottom.Style = ExcelBorderStyle.None;
                        excelWorksheet.Cells[linha, 1, linha, totalColunas].Style.Border.Left.Style = ExcelBorderStyle.None;
                        excelWorksheet.Cells[linha, 1, linha, totalColunas].Style.Border.Right.Style = ExcelBorderStyle.None;
                    }

                    if (!string.IsNullOrEmpty(observacoesModelo))
                    {
                        PularLinhaResetaColuna(ref linha, ref coluna);
                        PularLinhaResetaColuna(ref linha, ref coluna);

                        var linhasObs = observacoesModelo.Split('\n');

                        foreach (var linhaObs in linhasObs)
                        {
                            if (linhaObs.Contains("!"))
                            {
                                var servico = Regex.Matches(linhaObs, @"(?<=\!)[^}]*(?=\!)");

                                if (servico.Count > 0)
                                {
                                    var capture = servico.Cast<Match>().FirstOrDefault();

                                    var descricao = linhaObs.Replace(capture.Value, "");
                                    var valor = capture.Value;

                                    GravaCelula(new ExcelCelulaParametros(descricao.Replace("!!", ""), false, false, false), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);
                                    GravaCelula(new ExcelCelulaParametros(string.Empty, false, false, false), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);
                                    GravaCelula(new ExcelCelulaParametros(string.Empty, false, false, false), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);

                                    excelWorksheet.Cells[linha, 1, linha, 3].Merge = true;

                                    GravaCelula(new ExcelCelulaParametros(valor, false, false, false), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);

                                    excelWorksheet.Cells[linha, 4, linha, totalColunas].Merge = true;

                                    excelWorksheet.Cells[linha, 1, linha, totalColunas].Style.Border.Top.Style = ExcelBorderStyle.None;
                                    excelWorksheet.Cells[linha, 1, linha, totalColunas].Style.Border.Bottom.Style = ExcelBorderStyle.None;
                                    excelWorksheet.Cells[linha, 1, linha, totalColunas].Style.Border.Left.Style = ExcelBorderStyle.None;
                                    excelWorksheet.Cells[linha, 1, linha, totalColunas].Style.Border.Right.Style = ExcelBorderStyle.None;

                                    PularLinhaResetaColuna(ref linha, ref coluna);
                                }
                            }
                            else
                            {
                                GravaCelula(new ExcelCelulaParametros(linhaObs, false, false, false), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);

                                excelWorksheet.Cells[linha, 1, linha, totalColunas].Merge = true;

                                excelWorksheet.Cells[linha, 1, linha, totalColunas].Style.Border.Top.Style = ExcelBorderStyle.None;
                                excelWorksheet.Cells[linha, 1, linha, totalColunas].Style.Border.Bottom.Style = ExcelBorderStyle.None;
                                excelWorksheet.Cells[linha, 1, linha, totalColunas].Style.Border.Left.Style = ExcelBorderStyle.None;
                                excelWorksheet.Cells[linha, 1, linha, totalColunas].Style.Border.Right.Style = ExcelBorderStyle.None;

                                PularLinhaResetaColuna(ref linha, ref coluna);
                            }

                            excelWorksheet.Cells[linha, 1, linha, totalColunas].Style.WrapText = true;
                        }
                    }

                    for (int i = 1; i <= 2; i++)
                        PularLinhaResetaColuna(ref linha, ref coluna);

                    GravaCelula(new ExcelCelulaParametros("Validade:", true, false, false), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);

                    excelWorksheet.Cells[linha, 1, linha, totalColunas].Merge = true;

                    excelWorksheet.Cells[linha, 1, linha, totalColunas].Style.Border.Top.Style = ExcelBorderStyle.None;
                    excelWorksheet.Cells[linha, 1, linha, totalColunas].Style.Border.Bottom.Style = ExcelBorderStyle.None;
                    excelWorksheet.Cells[linha, 1, linha, totalColunas].Style.Border.Left.Style = ExcelBorderStyle.None;
                    excelWorksheet.Cells[linha, 1, linha, totalColunas].Style.Border.Right.Style = ExcelBorderStyle.None;

                    for (int i = 1; i <= 2; i++)
                        PularLinhaResetaColuna(ref linha, ref coluna);

                    GravaCelula(new ExcelCelulaParametros($"Cotação válida por 10 dias a contar da data de emissão", false, false, false), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);

                    excelWorksheet.Cells[linha, 1, linha, totalColunas].Merge = true;

                    excelWorksheet.Cells[linha, 1, linha, totalColunas].Style.Border.Top.Style = ExcelBorderStyle.None;
                    excelWorksheet.Cells[linha, 1, linha, totalColunas].Style.Border.Bottom.Style = ExcelBorderStyle.None;
                    excelWorksheet.Cells[linha, 1, linha, totalColunas].Style.Border.Left.Style = ExcelBorderStyle.None;
                    excelWorksheet.Cells[linha, 1, linha, totalColunas].Style.Border.Right.Style = ExcelBorderStyle.None;

                    PularLinhaResetaColuna(ref linha, ref coluna);
                    GravaCelula(new ExcelCelulaParametros("Após recebermos a confirmação desta cotação, a proposta formal será registrada em sistema para posterior análise, aceite e assinatura do cliente", false, false, false), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);

                    excelWorksheet.Cells[linha, 1, linha, totalColunas].Merge = true;

                    excelWorksheet.Cells[linha, 1, linha, totalColunas].Style.Border.Top.Style = ExcelBorderStyle.None;
                    excelWorksheet.Cells[linha, 1, linha, totalColunas].Style.Border.Bottom.Style = ExcelBorderStyle.None;
                    excelWorksheet.Cells[linha, 1, linha, totalColunas].Style.Border.Left.Style = ExcelBorderStyle.None;
                    excelWorksheet.Cells[linha, 1, linha, totalColunas].Style.Border.Right.Style = ExcelBorderStyle.None;

                    PularLinhaResetaColuna(ref linha, ref coluna);
                    GravaCelula(new ExcelCelulaParametros("Valores estarão vigentes para processos entrados No terminal em data igual ou posterior à da emissão da proposta assinada pelo cliente", false, false, false), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);

                    excelWorksheet.Cells[linha, 1, linha, totalColunas].Merge = true;

                    excelWorksheet.Cells[linha, 1, linha, totalColunas].Style.Border.Top.Style = ExcelBorderStyle.None;
                    excelWorksheet.Cells[linha, 1, linha, totalColunas].Style.Border.Bottom.Style = ExcelBorderStyle.None;
                    excelWorksheet.Cells[linha, 1, linha, totalColunas].Style.Border.Left.Style = ExcelBorderStyle.None;
                    excelWorksheet.Cells[linha, 1, linha, totalColunas].Style.Border.Right.Style = ExcelBorderStyle.None;

                    if (valoresMinimoCobranca.Any())
                    {
                        var valorMinimoSemTamanho = valoresMinimoCobranca.Any(c => c.ValorMinimo > 0);

                        var linhaCifEscalonado = 5;
                        var colunaCifEscalonado = 11;

                        if (valorMinimoSemTamanho)
                        {
                            GravaCelula(new ExcelCelulaParametros("", false, true, corPadrao), ref excelWorksheet, ref celula, ref linhaCifEscalonado, ref colunaCifEscalonado, ref borda);
                            GravaCelula(new ExcelCelulaParametros("", true, true, true, corPadrao), ref excelWorksheet, ref celula, ref linhaCifEscalonado, ref colunaCifEscalonado, ref borda);
                            GravaCelula(new ExcelCelulaParametros("Valor", true, true, true, corPadrao), ref excelWorksheet, ref celula, ref linhaCifEscalonado, ref colunaCifEscalonado, ref borda);

                            linhaCifEscalonado++;
                            colunaCifEscalonado = 11;

                            foreach (var minimo in valoresMinimoCobranca)
                            {
                                GravaCelula(new ExcelCelulaParametros(minimo.Descricao, false, false, false), ref excelWorksheet, ref celula, ref linhaCifEscalonado, ref colunaCifEscalonado, ref borda);
                                GravaCelula(new ExcelCelulaParametros("", false, false, false), ref excelWorksheet, ref celula, ref linhaCifEscalonado, ref colunaCifEscalonado, ref borda);
                                GravaCelula(new ExcelCelulaParametros(minimo.ValorMinimo.ToString(), false, false, false), ref excelWorksheet, ref celula, ref linhaCifEscalonado, ref colunaCifEscalonado, ref borda, TipoCelulaExcel.Moeda);

                                linhaCifEscalonado++;
                                colunaCifEscalonado = 11;
                            }
                        }
                        else
                        {
                            GravaCelula(new ExcelCelulaParametros("", false, true, corPadrao), ref excelWorksheet, ref celula, ref linhaCifEscalonado, ref colunaCifEscalonado, ref borda);
                            GravaCelula(new ExcelCelulaParametros("20", true, true, true, corPadrao), ref excelWorksheet, ref celula, ref linhaCifEscalonado, ref colunaCifEscalonado, ref borda);
                            GravaCelula(new ExcelCelulaParametros("40", true, true, true, corPadrao), ref excelWorksheet, ref celula, ref linhaCifEscalonado, ref colunaCifEscalonado, ref borda);

                            linhaCifEscalonado++;
                            colunaCifEscalonado = 11;

                            foreach (var minimo in valoresMinimoCobranca)
                            {
                                GravaCelula(new ExcelCelulaParametros(minimo.Descricao, false, false, false), ref excelWorksheet, ref celula, ref linhaCifEscalonado, ref colunaCifEscalonado, ref borda);
                                GravaCelula(new ExcelCelulaParametros(minimo.ValorMinimo20.ToString(), false, false, false), ref excelWorksheet, ref celula, ref linhaCifEscalonado, ref colunaCifEscalonado, ref borda, TipoCelulaExcel.Moeda);
                                GravaCelula(new ExcelCelulaParametros(minimo.ValorMinimo40.ToString(), false, false, false), ref excelWorksheet, ref celula, ref linhaCifEscalonado, ref colunaCifEscalonado, ref borda, TipoCelulaExcel.Moeda);

                                linhaCifEscalonado++;
                                colunaCifEscalonado = 11;
                            }
                        }
                    }

                    if (excelWorksheet.Dimension != null)
                    {
                        excelWorksheet.Cells[excelWorksheet.Dimension.Address].AutoFitColumns();
                    }

                    excelWorksheet.View.ShowGridLines = false;
                   // excelWorksheet.Workbook.Calculate();
                }

                #endregion

                #region FCL

                if (parametrosSimulador.Regime == Regime.FCL)
                {
                    ehConteiner = true;

                    linha = 1;
                    coluna = 1;
                    totalColunas = 14;
                    subTotal_MD = 0.0M;
                    subTotal_ME = 0.0M;

                    excelWorksheetFCL = pck.Workbook.Worksheets.Add("Simulador FCL");

                    GravaCelula(new ExcelCelulaParametros($"Simulador de Armazenagem de Carga FCL de Importação - {descricaoEmpresa}", true, true, 64.5), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);

                    excelWorksheetFCL.Cells[linha, 1, linha, totalColunas].Merge = true;

                    PularLinhaResetaColuna(ref linha, ref coluna);

                    GravaCelula(new ExcelCelulaParametros("Forma Pagamento", true, 50), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);
                    GravaCelula(new ExcelCelulaParametros("À Vista", false, false, 50), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);

                    excelWorksheetFCL.Cells[linha, 2, linha, totalColunas].Merge = true;
                    excelWorksheetFCL.Cells[linha, 2, linha, totalColunas].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    excelWorksheetFCL.Cells[linha, 2, linha, totalColunas].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    excelWorksheetFCL.Cells[linha, 2, linha, totalColunas].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    excelWorksheetFCL.Cells[linha, 2, linha, totalColunas].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                    PularLinhaResetaColuna(ref linha, ref coluna);

                    GravaCelula(new ExcelCelulaParametros("Tonelada (ton):", true, 12.75, true, false), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);
                    GravaCelula(new ExcelCelulaParametros(string.Format("{0:N2}", parametrosSimulador.Peso), true, true, corPadrao), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);

                    GravaCelula(new ExcelCelulaParametros("Metro (m3):", true, 12.75, true, false), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);
                    GravaCelula(new ExcelCelulaParametros(string.Format("{0:N2}", parametrosSimulador.VolumeM3), true, true, corPadrao), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);

                    GravaCelula(new ExcelCelulaParametros("Valor base para Cálculo:", true, 12.75, true, false), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);
                    GravaCelula(new ExcelCelulaParametros(parametrosSimulador.NumeroLotes.ToString(), true, true, corPadrao), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);

                    GravaCelula(new ExcelCelulaParametros("Períodos:", true, false, false), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);
                    GravaCelula(new ExcelCelulaParametros(parametrosSimulador.Periodos.ToString(), true, true, corPadrao), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Inteiro);

                    GravaCelula(new ExcelCelulaParametros("'20:", true, 0, true, false), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);
                    GravaCelula(new ExcelCelulaParametros("1", true, true, corPadrao), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Inteiro);

                    GravaCelula(new ExcelCelulaParametros("'40:", true, 0, true, false), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);
                    GravaCelula(new ExcelCelulaParametros("1", true, true, corPadrao), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Inteiro);

                    PularLinhaResetaColuna(ref linha, ref coluna);

                    string qtdeDias = string.Empty;
                    var freeTime = _oportunidadeDAO.ObterFreeTime(oportunidadeBusca.Id);

                    foreach (var tabela in tabelas)
                    {
                        qtdeDias = _servicoDAO.ObterQuantidadeDias(tabela.TabelaId, ehConteiner);
                    }

                    GravaCelula(new ExcelCelulaParametros($"Período: {qtdeDias} dias + {freeTime} para carregamento", true, true, 50), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);

                    excelWorksheetFCL.Cells[linha, 1, linha, totalColunas].Merge = true;
                    excelWorksheetFCL.Cells[linha, 1, linha, totalColunas].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    excelWorksheetFCL.Cells[linha, 1, linha, totalColunas].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    excelWorksheetFCL.Cells[linha, 1, linha, totalColunas].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    excelWorksheetFCL.Cells[linha, 1, linha, totalColunas].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                    PularLinhaResetaColuna(ref linha, ref coluna);

                    GravaCelula(new ExcelCelulaParametros("Cotação emitida em:", true, true, corPadrao), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);
                    excelWorksheetFCL.Cells[linha, 1, linha, 1].Style.Border.Bottom.Style = ExcelBorderStyle.None;

                    GravaCelula(new ExcelCelulaParametros("CIF R$", true, true, corPadrao), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);

                    excelWorksheetFCL.Cells[linha, 2, linha, totalColunas].Merge = true;
                    excelWorksheetFCL.Cells[linha, 2, linha, totalColunas].Style.Border.Bottom.Style = ExcelBorderStyle.None;

                    PularLinhaResetaColuna(ref linha, ref coluna);

                    GravaCelula(new ExcelCelulaParametros(DateTime.Now.ToString("dd/MM/yyyy HH:mm"), true, true, corPadrao), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);

                    if (ehConteiner)
                    {
                        GravaCelula(new ExcelCelulaParametros(string.Format("{0:C2}", dadosSimulador.ValorCifConteiner), true, true, corPadrao), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);
                    }
                    else
                    {
                        GravaCelula(new ExcelCelulaParametros(string.Format("{0:C2}", dadosSimulador.ValorCifCargaSolta), true, true, corPadrao), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);
                    }

                    excelWorksheetFCL.Cells[linha, 2, linha, totalColunas].Merge = true;

                    excelWorksheetFCL.Cells[linha, 1, linha, totalColunas].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    excelWorksheetFCL.Cells[linha, 1, linha, totalColunas].Style.Border.Bottom.Style = ExcelBorderStyle.None;
                    excelWorksheetFCL.Cells[linha, 1, linha, totalColunas].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    excelWorksheetFCL.Cells[linha, 1, linha, totalColunas].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    PularLinhaResetaColuna(ref linha, ref coluna);

                    GravaCelula(new ExcelCelulaParametros(string.Empty, true, 12.75, true, false), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);
                    GravaCelula(new ExcelCelulaParametros(string.Empty, true, 12.75, true, false), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);
                    GravaCelula(new ExcelCelulaParametros("20", true, true, true, verde20), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);
                    GravaCelula(new ExcelCelulaParametros(string.Empty, true, 12.75, true, false), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);
                    GravaCelula(new ExcelCelulaParametros(string.Empty, true, 12.75, true, false), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);
                    GravaCelula(new ExcelCelulaParametros(string.Empty, true, 12.75, true, false), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);
                    GravaCelula(new ExcelCelulaParametros("40", true, true, true, azul40), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);
                    GravaCelula(new ExcelCelulaParametros(string.Empty, true, 12.75, true, false), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);
                    GravaCelula(new ExcelCelulaParametros(string.Empty, true, 12.75, true, false), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);
                    GravaCelula(new ExcelCelulaParametros(string.Empty, true, 12.75, true, false), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);
                    GravaCelula(new ExcelCelulaParametros("MARGEM DIREITA", true, true, true), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);
                    GravaCelula(new ExcelCelulaParametros(string.Empty, true, 12.75, true, false), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);
                    GravaCelula(new ExcelCelulaParametros("MARGEM ESQUERDA", true, true, true), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);
                    GravaCelula(new ExcelCelulaParametros(string.Empty, true, 12.75, true, false), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);

                    excelWorksheetFCL.Cells[linha, 3, linha, 6].Merge = true;
                    excelWorksheetFCL.Cells[linha, 7, linha, 10].Merge = true;
                    excelWorksheetFCL.Cells[linha, 11, linha, 12].Merge = true;
                    excelWorksheetFCL.Cells[linha, 13, linha, 14].Merge = true;

                    PularLinhaResetaColuna(ref linha, ref coluna);

                    GravaCelula(new ExcelCelulaParametros(string.Empty, true, true, false), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);
                    GravaCelula(new ExcelCelulaParametros("Base de Cobrança", true, true, true), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);
                    GravaCelula(new ExcelCelulaParametros("Qtde Per.", true, true, true, verde20), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);
                    GravaCelula(new ExcelCelulaParametros("%", true, true, true, verde20), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);
                    GravaCelula(new ExcelCelulaParametros("Valor Per.", true, true, true, verde20), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);
                    GravaCelula(new ExcelCelulaParametros("Mínimo", true, true, true, verde20), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);
                    GravaCelula(new ExcelCelulaParametros("Qtde Per.", true, true, true, azul40), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);
                    GravaCelula(new ExcelCelulaParametros("%", true, true, true, azul40), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);
                    GravaCelula(new ExcelCelulaParametros("Valor Per.", true, true, true, azul40), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);
                    GravaCelula(new ExcelCelulaParametros("Mínimo", true, true, true, azul40), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);
                    GravaCelula(new ExcelCelulaParametros("20'", true, true, true), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);
                    GravaCelula(new ExcelCelulaParametros("40'", true, true, true), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);
                    GravaCelula(new ExcelCelulaParametros("20'", true, true, true), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);
                    GravaCelula(new ExcelCelulaParametros("40'", true, true, true), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);

                    PularLinhaResetaColuna(ref linha, ref coluna);

                    var valorper20 = 0.0M;
                    var valorper40 = 0.0M;

                    for (int periodo = 1; periodo <= 3; periodo++)
                    {
                        coluna = 0;

                        foreach (var tabela in tabelas)
                        {
                            var valoresArmazenagem20 = _servicoDAO.ObterValoresServicoArmazenagem(tabela.TabelaId, periodo, ehConteiner, "SVAR20");

                            if (valoresArmazenagem20 == null)
                            {
                                valoresArmazenagem20 = _servicoDAO.ObterValoresServicoArmazenagem(tabela.TabelaId, periodo, ehConteiner, "SVAR");
                            }

                            if (valoresArmazenagem20 == null)
                                continue;

                            if (coluna == 0)
                            {
                                coluna = 1;
                                GravaCelula(new ExcelCelulaParametros($"Armazenagem {periodo}º Período", false, false, 12.75), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);
                            }

                            var precoUnitario20 = 0.0M;

                            if (valoresArmazenagem20.BaseCalculo == "CIF" || valoresArmazenagem20.BaseCalculo == "CIF0" || valoresArmazenagem20.BaseCalculo == "CIFM")
                            {
                                precoUnitario20 = valoresArmazenagem20.PrecoUnitario / 100;
                                valorper20 = (valoresArmazenagem20.PrecoUnitario / 100) * dadosSimulador.ValorCifConteiner;
                            }
                            else
                            {
                                precoUnitario20 = valoresArmazenagem20.PrecoUnitario;
                                valorper20 = valoresArmazenagem20.PrecoUnitario;

                            }

                            if (valoresArmazenagem20.BaseCalculo == "UNI")
                            {
                                GravaCelula(new ExcelCelulaParametros("unidade", false, true, 12.75), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);
                            }
                            else
                            {
                                GravaCelula(new ExcelCelulaParametros(valoresArmazenagem20.BaseCalculo, false, true, 12.75), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);
                            }

                            if (valoresArmazenagem20.BaseCalculo == "CIF" || valoresArmazenagem20.BaseCalculo == "CIF0" || valoresArmazenagem20.BaseCalculo == "CIFM")
                            {
                                GravaCelula(new ExcelCelulaParametros(valoresArmazenagem20.Periodo.ToString(), true, true, verde20), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);
                                GravaCelula(new ExcelCelulaParametros(precoUnitario20.ToString(), true, true, verde20), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Percentual);

                                GravaCelula(new ExcelCelulaParametros(string.Empty, true, true, verde20), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                                excelWorksheetFCL.Cells[linha, coluna - 1].Formula = $"=(D{linha}*B6)";

                                GravaCelula(new ExcelCelulaParametros(valoresArmazenagem20.PrecoMinimo.ToString(), true, true, verde20), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                            }
                            else
                            {
                                GravaCelula(new ExcelCelulaParametros(valoresArmazenagem20.Periodo.ToString(), true, true, verde20), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);
                                GravaCelula(new ExcelCelulaParametros("", true, true, verde20), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);
                                GravaCelula(new ExcelCelulaParametros(String.Format("{0:C2}", valoresArmazenagem20.PrecoUnitario), true, true, verde20), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);
                                GravaCelula(new ExcelCelulaParametros(String.Format("{0:C2}", valoresArmazenagem20.PrecoMinimo), true, true, verde20), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);
                            }

                            var valoresArmazenagem40 = _servicoDAO.ObterValoresServicoArmazenagem(tabela.TabelaId, periodo, ehConteiner, "SVAR40");

                            if (valoresArmazenagem40 == null)
                            {
                                valoresArmazenagem40 = _servicoDAO.ObterValoresServicoArmazenagem(tabela.TabelaId, periodo, ehConteiner, "SVAR");
                            }
                            var precoUnitario40 = 0.0M;
                            if (valoresArmazenagem40.BaseCalculo == "CIF" || valoresArmazenagem40.BaseCalculo == "CIF0" || valoresArmazenagem40.BaseCalculo == "CIFM")
                            {

                                precoUnitario40 = valoresArmazenagem40.PrecoUnitario / 100;
                                valorper40 = (valoresArmazenagem40.PrecoUnitario / 100) * dadosSimulador.ValorCifConteiner;
                            }
                            else
                            {
                                precoUnitario40 = valoresArmazenagem40.PrecoUnitario;
                                valorper40 = valoresArmazenagem40.PrecoUnitario;

                            }
                            if (valoresArmazenagem40.BaseCalculo == "CIF" || valoresArmazenagem40.BaseCalculo == "CIF0" || valoresArmazenagem40.BaseCalculo == "CIFM")
                            {
                                GravaCelula(new ExcelCelulaParametros(valoresArmazenagem40.Periodo.ToString(), true, true, azul40), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);
                                GravaCelula(new ExcelCelulaParametros(precoUnitario40.ToString(), true, true, azul40), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Percentual);

                                GravaCelula(new ExcelCelulaParametros(string.Empty, true, true, azul40), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                                excelWorksheetFCL.Cells[linha, coluna - 1].Formula = $"=(H{linha}*B6)";

                                GravaCelula(new ExcelCelulaParametros(valoresArmazenagem40.PrecoMinimo.ToString(), true, true, azul40), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                            }
                            else
                            {
                                GravaCelula(new ExcelCelulaParametros(valoresArmazenagem40.Periodo.ToString(), true, true, azul40), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);
                                GravaCelula(new ExcelCelulaParametros("", true, true, azul40), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);
                                GravaCelula(new ExcelCelulaParametros(String.Format("{0:C2}", valoresArmazenagem40.PrecoUnitario), true, true, azul40), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);
                                GravaCelula(new ExcelCelulaParametros(String.Format("{0:C2}", valoresArmazenagem40.PrecoMinimo), true, true, azul40), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);
                            }

                            GravaCelula(new ExcelCelulaParametros(string.Empty, true, true, verde20), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                            excelWorksheetFCL.Cells[linha, coluna - 1].Formula = $"IF(E{linha}>F{linha},E{linha},F{linha})";

                            GravaCelula(new ExcelCelulaParametros(string.Empty, true, true, verde20), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                            excelWorksheetFCL.Cells[linha, coluna - 1].Formula = $"IF(I{linha}>J{linha},I{linha},J{linha})";

                            GravaCelula(new ExcelCelulaParametros(string.Empty, true, true, verde20), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                            excelWorksheetFCL.Cells[linha, coluna - 1].Formula = $"IF(E{linha}>F{linha},E{linha},F{linha})";

                            GravaCelula(new ExcelCelulaParametros(string.Empty, true, true, verde20), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                            excelWorksheetFCL.Cells[linha, coluna - 1].Formula = $"IF(I{linha}>J{linha},I{linha},J{linha})";

                            PularLinhaResetaColuna(ref linha, ref coluna);
                        }
                    }

                    valorper20 = 0.0M;
                    valorper40 = 0.0M;

                    GravaCelula(new ExcelCelulaParametros("Subsequentes", false, false, 12.75), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);

                    foreach (var tabela in tabelas)
                    {
                        for (int periodo = 3; periodo >= 1; periodo += -1)
                        {
                            var valoresArmazenagem20 = _servicoDAO.ObterValoresServicoArmazenagem(tabela.TabelaId, periodo, ehConteiner, "SVAR20");

                            if (valoresArmazenagem20 != null)
                            {
                                var precoUnitario20 = 0.0M;

                                if (valoresArmazenagem20.BaseCalculo == "CIF" || valoresArmazenagem20.BaseCalculo == "CIF0" || valoresArmazenagem20.BaseCalculo == "CIFM")
                                {
                                    precoUnitario20 = valoresArmazenagem20.PrecoUnitario / 100;
                                    valorper20 = (valoresArmazenagem20.PrecoUnitario / 100) * dadosSimulador.ValorCifConteiner;
                                }
                                else
                                {
                                    precoUnitario20 = valoresArmazenagem20.PrecoUnitario;
                                    valorper20 = valoresArmazenagem20.PrecoUnitario;
                                }

                                if (valoresArmazenagem20.BaseCalculo == "UNI")
                                {
                                    GravaCelula(new ExcelCelulaParametros("unidade", false, true, 12.75), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);
                                }
                                else
                                {
                                    GravaCelula(new ExcelCelulaParametros(valoresArmazenagem20.BaseCalculo, false, true, 12.75), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);
                                }

                                if (valoresArmazenagem20.BaseCalculo == "CIF" || valoresArmazenagem20.BaseCalculo == "CIF0" || valoresArmazenagem20.BaseCalculo == "CIFM")
                                {
                                    GravaCelula(new ExcelCelulaParametros("...", true, true, verde20), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);
                                    GravaCelula(new ExcelCelulaParametros(precoUnitario20.ToString(), true, true, verde20), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Percentual);

                                    GravaCelula(new ExcelCelulaParametros(string.Empty, true, true, verde20), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                                    excelWorksheetFCL.Cells[linha, coluna - 1].Formula = $"=(D{linha}*B6)";

                                    GravaCelula(new ExcelCelulaParametros(valoresArmazenagem20.PrecoMinimo.ToString(), true, true, verde20), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                                }
                                else
                                {
                                    GravaCelula(new ExcelCelulaParametros("...", true, true, verde20), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);
                                    GravaCelula(new ExcelCelulaParametros("", true, true, verde20), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);
                                    GravaCelula(new ExcelCelulaParametros(String.Format("{0:C2}", valoresArmazenagem20.PrecoUnitario), true, true, verde20), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);
                                    GravaCelula(new ExcelCelulaParametros(valoresArmazenagem20.PrecoMinimo.ToString(), true, true, verde20), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                                }
                            }

                            if (valoresArmazenagem20 == null)
                                continue;

                            var valoresArmazenagem40 = _servicoDAO.ObterValoresServicoArmazenagem(tabela.TabelaId, periodo, ehConteiner, "SVAR40");

                            if (valoresArmazenagem40 != null)
                            {
                                var precoUnitario40 = 0.0M;
                                if (valoresArmazenagem20.BaseCalculo == "CIF" || valoresArmazenagem20.BaseCalculo == "CIF0" || valoresArmazenagem20.BaseCalculo == "CIFM")
                                {
                                    precoUnitario40 = valoresArmazenagem40.PrecoUnitario / 100;
                                    valorper40 = (valoresArmazenagem40.PrecoUnitario / 100) * dadosSimulador.ValorCifConteiner;
                                }
                                else
                                {
                                    precoUnitario40 = valoresArmazenagem40.PrecoUnitario;
                                    valorper40 = valoresArmazenagem40.PrecoUnitario;

                                }

                                if (valoresArmazenagem40.BaseCalculo == "CIF" || valoresArmazenagem40.BaseCalculo == "CIF0" || valoresArmazenagem40.BaseCalculo == "CIFM")
                                {
                                    GravaCelula(new ExcelCelulaParametros("...", true, true, azul40), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);
                                    GravaCelula(new ExcelCelulaParametros(precoUnitario40.ToString(), true, true, azul40), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Percentual);

                                    GravaCelula(new ExcelCelulaParametros(string.Empty, true, true, azul40), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                                    excelWorksheetFCL.Cells[linha, coluna - 1].Formula = $"=(H{linha}*B6)";

                                    GravaCelula(new ExcelCelulaParametros(valoresArmazenagem40.PrecoMinimo.ToString(), true, true, azul40), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                                }
                                else
                                {
                                    GravaCelula(new ExcelCelulaParametros("...", true, true, azul40), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);
                                    GravaCelula(new ExcelCelulaParametros("", true, true, azul40), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);
                                    GravaCelula(new ExcelCelulaParametros(String.Format("{0:C2}", valoresArmazenagem40.PrecoUnitario), true, true, azul40), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);
                                    GravaCelula(new ExcelCelulaParametros(valoresArmazenagem40.PrecoMinimo.ToString(), true, true, azul40), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                                }
                            }

                            GravaCelula(new ExcelCelulaParametros(string.Empty, true, true, verde20), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                            excelWorksheetFCL.Cells[linha, coluna - 1].Formula = $"IF(E{linha}>F{linha},E{linha},F{linha})";


                            GravaCelula(new ExcelCelulaParametros(string.Empty, true, true, verde20), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                            excelWorksheetFCL.Cells[linha, coluna - 1].Formula = $"IF(I{linha}>J{linha},I{linha},J{linha})";

                            GravaCelula(new ExcelCelulaParametros(string.Empty, true, true, verde20), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                            excelWorksheetFCL.Cells[linha, coluna - 1].Formula = $"IF(E{linha}>F{linha},E{linha},F{linha})";

                            GravaCelula(new ExcelCelulaParametros(string.Empty, true, true, verde20), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                            excelWorksheetFCL.Cells[linha, coluna - 1].Formula = $"IF(I{linha}>J{linha},I{linha},J{linha})";

                            if (valoresArmazenagem20 != null || valoresArmazenagem40 != null)
                            {
                                break;
                            }

                        }
                    }

                    PularLinhaResetaColuna(ref linha, ref coluna);

                    GravaCelula(new ExcelCelulaParametros(string.Empty, true), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);

                    for (int i = 1; i < totalColunas; i++)
                        GravaCelula(new ExcelCelulaParametros(string.Empty, true), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);

                    PularLinhaResetaColuna(ref linha, ref coluna);

                    taxaImposto = _impostoDAO.ObterTaxaImposto(oportunidadeBusca.ImpostoId);

                    taxaImposto = 1 - taxaImposto;

                    servicosSimulador = _simuladorDAO.ObterServicosSimulador(dadosSimulador.SimuladorId,2);

                    if (servicosSimulador.Count() == 0)
                        PularLinhaResetaColuna(ref linha, ref coluna);

                    var linhaPrimeiroServico = 0;

                    foreach (var servico in servicosSimulador)
                    {
                        if (linhaPrimeiroServico == 0)
                        {
                            linhaPrimeiroServico = linha;
                        }

                        GravaCelula(new ExcelCelulaParametros(servico.Descricao, 12.75), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);

                        foreach (var tabela in tabelas)
                        {
                            var descricaoBaseCalculo = servico.BaseCalculo != "BL"
                                ? servico.BaseCalculo
                                : "por lote";

                            GravaCelula(new ExcelCelulaParametros(descricaoBaseCalculo, false, true, 12.75), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);

                            if (servico.ServicoId == 45)
                            {
                                GravaCelula(new ExcelCelulaParametros(string.Empty, true, 12.75), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);

                                var perc20 = _simuladorDAO.ObterPercentualServicos(dadosSimulador.SimuladorId, 45, 20);

                                GravaCelula(new ExcelCelulaParametros(perc20.ToString(), true, 12.75), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Percentual);

                                for (int i = 1; i <= 6; i++)
                                    GravaCelula(new ExcelCelulaParametros(string.Empty, true, 12.75), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);
                            }
                            else if (servico.ServicoId == 295)
                            {
                                GravaCelula(new ExcelCelulaParametros(string.Empty, true, 12.75), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);

                                var perc20 = _simuladorDAO.ObterPercentualServicos(dadosSimulador.SimuladorId, 295, 20);

                                GravaCelula(new ExcelCelulaParametros(perc20.ToString(), true, 12.75), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Percentual);

                                GravaCelula(new ExcelCelulaParametros(string.Empty, true, 12.75), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);

                                var valorMinimo20 = _servicoDAO.ObterValorMinimoAdicionalArmazenagem(oportunidadeBusca.Id, "SVAR20");

                                GravaCelula(new ExcelCelulaParametros(valorMinimo20?.ToString() ?? string.Empty, true, 12.75), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                                GravaCelula(new ExcelCelulaParametros(string.Empty, true, 12.75), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);

                                var perc40 = _simuladorDAO.ObterPercentualServicos(dadosSimulador.SimuladorId, 295, 40);

                                GravaCelula(new ExcelCelulaParametros(perc40.ToString(), true, 12.75), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Percentual);

                                GravaCelula(new ExcelCelulaParametros(string.Empty, true, 12.75), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);


                                var valorMinimo40 = _servicoDAO.ObterValorMinimoAdicionalArmazenagem(oportunidadeBusca.Id, "SVAR40");

                                GravaCelula(new ExcelCelulaParametros(valorMinimo40?.ToString() ?? string.Empty, true, 12.75), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                            }
                            else
                            {
                                for (int i = 1; i <= 8; i++)
                                    GravaCelula(new ExcelCelulaParametros(string.Empty, true, 12.75), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);
                            }

                            foreach (var margem in margens)
                            {
                                foreach (var tipoCarga in tipoCargas)
                                {
                                    var valor = _servicoDAO.ObterValoresServicoSimuladorPorTabelaId(dadosSimulador.SimuladorId, tabela.TabelaId, servico.ServicoId, margem, tipoCarga);

                                    if (valor != null)
                                    {
                                        var valorper = valor.ValorFinal;

                                        var valorunt = valor.PrecoUnitario;

                                        GravaCelula(new ExcelCelulaParametros(valorper.ToString(), true, 12.75), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);

                                        if (descricaoBaseCalculo == "CIF")
                                        {
                                            if (servico.ServicoId == 52)
                                            {
                                                if (margem.ToString() == "MDIR")
                                                {
                                                    if (tipoCarga.ToString() == "SVAR20")
                                                    {
                                                        excelWorksheetFCL.Cells[linha, coluna - 1].Formula = $"IF(H3=1, K9, IF(H3=2, K9+K10, IF(H3=3, K9+K10+K11, IF(H3=4, K9+K10+K11+K12, IF(H3>4,K9+K10+K11+K12+(K12*(H3-4))  )))))";
                                                    }
                                                    if (tipoCarga.ToString() == "SVAR40")
                                                    {
                                                        excelWorksheetFCL.Cells[linha, coluna - 1].Formula = $"IF(H3=1, L9, IF(H3=2, L9+L10, IF(H3=3, L9+L10+L11, IF(H3=4, L9+L10+L11+L12, IF(H3>4,L9+L10+L11+L12+(L12*(H3-4)))))))";
                                                    }
                                                }
                                                if (margem.ToString() == "MESQ")
                                                {
                                                    if (tipoCarga.ToString() == "SVAR20")
                                                    {
                                                        excelWorksheetFCL.Cells[linha, coluna - 1].Formula = $"IF(H3=1, M9, IF(H3=2, M9+M10, IF(H3=3, M9+M10+M11, IF(H3=4, M9+M10+M11+M12, IF(H3>4,M9+M10+M11+M12+(M12*(H3-4)))))))";
                                                    }
                                                    if (tipoCarga.ToString() == "SVAR40")
                                                    {
                                                        excelWorksheetFCL.Cells[linha, coluna - 1].Formula = $"IF(H3=1, N9, IF(H3=2, N9+N10, IF(H3=3, N9+N10+N11, IF(H3=4, N9+N10+N11+N12, IF(H3>4,N9+N10+N11+N12+(N12*(H3-4)))))))";
                                                    }
                                                }

                                            }

                                            if (servico.ServicoId == 295)
                                            {
                                                if (tipoCarga.ToString() == "SVAR20")
                                                {
                                                    excelWorksheetFCL.Cells[linha, coluna - 1].Formula = $"IF(D{linha}*(B6 * 1.5)>F{linha},D{linha}*(B6*1.5),F{linha})*(IF(H3>0,H3-1,0))";
                                                }
                                                if (tipoCarga.ToString() == "SVAR40")
                                                {
                                                    excelWorksheetFCL.Cells[linha, coluna - 1].Formula = $"IF(H{linha}*(B6 * 1.5)>J{linha},H{linha}*(B6*1.5),J{linha})*(IF(H3>0,H3-1,0))";
                                                }
                                            }

                                            if (servico.ServicoId == 45)
                                            {
                                                excelWorksheetFCL.Cells[linha, coluna - 1].Formula = $"D{linha}*B6";

                                                excelWorksheetFCL.Cells[linha, coluna - 1].Formula = $"D{linha}*B6";
                                            }
                                        }
                                        else if (descricaoBaseCalculo == "UNIDADE")
                                        {
                                            if (tipoCarga == "SVAR20")
                                            {
                                                excelWorksheetFCL.Cells[linha, coluna - 1].Formula = $"=({valorper.ToString("n2").PPonto()}*J3)";
                                            }

                                            if (tipoCarga == "SVAR40")
                                            {
                                                excelWorksheetFCL.Cells[linha, coluna - 1].Formula = $"=({valorper.ToString("n2").PPonto()}*L3)";
                                            }
                                        }
                                        else if (descricaoBaseCalculo == "VOLUME")
                                        {
                                            excelWorksheetFCL.Cells[linha, coluna - 1].Formula = $"=({valorper}*D3)";
                                        }
                                        else if (descricaoBaseCalculo == "TONELAGEM")
                                        {
                                            excelWorksheetFCL.Cells[linha, coluna - 1].Formula = $"=({valorper}*B3)";
                                        }

                                        if (margem == "MDIR" && tipoCarga == "SVAR20")
                                            subTotalMD_20 += valorper;

                                        if (margem == "MDIR" && tipoCarga == "SVAR40")
                                            subTotalMD_40 += valorper;

                                        if (margem == "MESQ" && tipoCarga == "SVAR20")
                                            subTotalME_20 += valorper;

                                        if (margem == "MESQ" && tipoCarga == "SVAR40")
                                            subTotalME_40 += valorper;
                                    }
                                    else
                                    {
                                        GravaCelula(new ExcelCelulaParametros(string.Empty, true, 12.75), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                                    }
                                }
                            }
                        }

                        PularLinhaResetaColuna(ref linha, ref coluna);
                    }

                    GravaCelula(new ExcelCelulaParametros("Sub Total", true), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);

                    GravaCelula(new ExcelCelulaParametros(string.Empty, true), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);
                    GravaCelula(new ExcelCelulaParametros(string.Empty, true), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);
                    GravaCelula(new ExcelCelulaParametros(string.Empty, true), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);
                    GravaCelula(new ExcelCelulaParametros(string.Empty, true), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);
                    GravaCelula(new ExcelCelulaParametros(string.Empty, true), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);
                    GravaCelula(new ExcelCelulaParametros(string.Empty, true), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);
                    GravaCelula(new ExcelCelulaParametros(string.Empty, true), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);
                    GravaCelula(new ExcelCelulaParametros(string.Empty, true), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);
                    GravaCelula(new ExcelCelulaParametros(string.Empty, true), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);

                    GravaCelula(new ExcelCelulaParametros(String.Empty, true, 12.75, true), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                    excelWorksheetFCL.Cells[linha, coluna - 1].Formula = $"SUM(K{linhaPrimeiroServico}:K{linha - 1})";

                    GravaCelula(new ExcelCelulaParametros(String.Empty, true, 12.75, true), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                    excelWorksheetFCL.Cells[linha, coluna - 1].Formula = $"SUM(L{linhaPrimeiroServico}:L{linha - 1})";

                    GravaCelula(new ExcelCelulaParametros(String.Empty, true, 12.75, true), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                    excelWorksheetFCL.Cells[linha, coluna - 1].Formula = $"SUM(M{linhaPrimeiroServico}:M{linha - 1})";

                    GravaCelula(new ExcelCelulaParametros(String.Empty, true, 12.75, true), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                    excelWorksheetFCL.Cells[linha, coluna - 1].Formula = $"SUM(N{linhaPrimeiroServico}:N{linha - 1})";

                    var linhaSubTotal = linha;

                    PularLinhaResetaColuna(ref linha, ref coluna);

                    var linhaTributos = 0;

                    GravaCelula(new ExcelCelulaParametros("Tributos", false), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);

                    if (oportunidadeBusca.ImpostoId != 2)
                    {
                        linhaTributos = linha;

                        foreach (var tabela in tabelas)
                        {
                            valorImpostoFCL = _simuladorDAO.ObterValorImposto(taxaImposto, dadosSimulador.SimuladorId, tabela.TabelaId, "MDIR", "SVAR20");

                            var valorPorcentagemFCL = valorImpostoFCL / (subTotalMD_20 == 0 ? 1 : subTotalMD_20);
                            if ( valorPorcentagemFCL == 0)
                            {
                                valorPorcentagemFCL = (1 / taxaImposto) - 1;
                            }
                            GravaCelula(new ExcelCelulaParametros(string.Format("{0:P2}", valorPorcentagemFCL), true, 12.75, false), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);
                        }

                        for (int i = 1; i <= 8; i++)
                            GravaCelula(new ExcelCelulaParametros(string.Empty, false), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);

                        foreach (var tabela in tabelas)
                        {
                            GravaCelula(new ExcelCelulaParametros(string.Empty, true, 12.75, true), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                            excelWorksheetFCL.Cells[linha, coluna - 1].Formula = $"=(K{linhaSubTotal}*B{linha})";

                            GravaCelula(new ExcelCelulaParametros(string.Empty, true, 12.75, true), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                            excelWorksheetFCL.Cells[linha, coluna - 1].Formula = $"=(L{linhaSubTotal}*B{linha})";

                            GravaCelula(new ExcelCelulaParametros(string.Empty, true, 12.75, true), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                            excelWorksheetFCL.Cells[linha, coluna - 1].Formula = $"=(M{linhaSubTotal}*B{linha})";

                            GravaCelula(new ExcelCelulaParametros(string.Empty, true, 12.75, true), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                            excelWorksheetFCL.Cells[linha, coluna - 1].Formula = $"=(N{linhaSubTotal}*B{linha})";
                        }
                    }
                    else
                    {
                        GravaCelula(new ExcelCelulaParametros(string.Empty, true, 12.75, false), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);

                        for (int i = 1; i <= 8; i++)
                            GravaCelula(new ExcelCelulaParametros(string.Empty, false), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);

                        GravaCelula(new ExcelCelulaParametros(string.Empty, true, 12.75, false), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                        GravaCelula(new ExcelCelulaParametros(string.Empty, true, 12.75, false), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                        GravaCelula(new ExcelCelulaParametros(string.Empty, true, 12.75, false), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                        GravaCelula(new ExcelCelulaParametros(string.Empty, true, 12.75, false), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                    }

                    PularLinhaResetaColuna(ref linha, ref coluna);

                    GravaCelula(new ExcelCelulaParametros("Total", true), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);

                    GravaCelula(new ExcelCelulaParametros(string.Empty, true), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);
                    GravaCelula(new ExcelCelulaParametros(string.Empty, true), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);
                    GravaCelula(new ExcelCelulaParametros(string.Empty, true), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);
                    GravaCelula(new ExcelCelulaParametros(string.Empty, true), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);
                    GravaCelula(new ExcelCelulaParametros(string.Empty, true), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);
                    GravaCelula(new ExcelCelulaParametros(string.Empty, true), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);
                    GravaCelula(new ExcelCelulaParametros(string.Empty, true), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);
                    GravaCelula(new ExcelCelulaParametros(string.Empty, true), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);
                    GravaCelula(new ExcelCelulaParametros(string.Empty, true), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);

                    GravaCelula(new ExcelCelulaParametros(String.Empty, true, 12.75, true), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                    excelWorksheetFCL.Cells[linha, coluna - 1].Formula = $"SUM(K{linhaSubTotal}:K{linha - 1})";

                    GravaCelula(new ExcelCelulaParametros(String.Empty, true, 12.75, true), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                    excelWorksheetFCL.Cells[linha, coluna - 1].Formula = $"SUM(L{linhaSubTotal}:L{linha - 1})";

                    GravaCelula(new ExcelCelulaParametros(String.Empty, true, 12.75, true), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                    excelWorksheetFCL.Cells[linha, coluna - 1].Formula = $"SUM(M{linhaSubTotal}:M{linha - 1})";

                    GravaCelula(new ExcelCelulaParametros(String.Empty, true, 12.75, true), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda, TipoCelulaExcel.Moeda);
                    excelWorksheetFCL.Cells[linha, coluna - 1].Formula = $"SUM(N{linhaSubTotal}:N{linha - 1})";


                    AjustarLarguraColuna(ref excelWorksheetFCL, coluna: 1, larguraEmPixels: 60);

                    for (int i = 2; i <= 10; i++)
                        AjustarLarguraColuna(ref excelWorksheetFCL, coluna: i, larguraEmPixels: 12);

                    for (int i = 11; i <= totalColunas; i++)
                        AjustarLarguraColuna(ref excelWorksheetFCL, coluna: i, larguraEmPixels: 20);


                    PularLinhaResetaColuna(ref linha, ref coluna);

                    GravaCelula(new ExcelCelulaParametros("Serviços Complementares:", true, false, false), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);

                    excelWorksheetFCL.Cells[linha, 1, linha, totalColunas].Merge = true;

                    excelWorksheetFCL.Cells[linha, 1, linha, totalColunas].Style.Border.Top.Style = ExcelBorderStyle.None;
                    excelWorksheetFCL.Cells[linha, 1, linha, totalColunas].Style.Border.Bottom.Style = ExcelBorderStyle.None;
                    excelWorksheetFCL.Cells[linha, 1, linha, totalColunas].Style.Border.Left.Style = ExcelBorderStyle.None;
                    excelWorksheetFCL.Cells[linha, 1, linha, totalColunas].Style.Border.Right.Style = ExcelBorderStyle.None;

                    for (int i = 1; i <= 2; i++)
                        PularLinhaResetaColuna(ref linha, ref coluna);

                    var servicosComplementares = _simuladorDAO.ObterServicosComplementares(parametrosSimulador.ModeloSimuladorId, filtro.OportunidadeId, 2);

                    foreach (var servicoComplementar in servicosComplementares)
                    {
                        var baseCalculoServicoComplementar = string.Empty;

                        switch (servicoComplementar.BaseCalculo.ToUpper())
                        {
                            case "TON":
                                baseCalculoServicoComplementar = "por tonelagem";
                                break;
                            case "VOL":
                                baseCalculoServicoComplementar = "por volume";
                                break;
                            case "VOLP":
                                baseCalculoServicoComplementar = "por volume/tonelagem";
                                break;
                            case "BL":
                                baseCalculoServicoComplementar = "por lote";
                                break;
                            case "CIF":
                                baseCalculoServicoComplementar = "por CIF";
                                break;
                            case "SVAR20":
                                baseCalculoServicoComplementar = "por Contêiner de 20";
                                break;
                            case "SVAR40":
                                baseCalculoServicoComplementar = "por Contêiner de 40";
                                break;
                            case "SVAR":
                                baseCalculoServicoComplementar = "por Contêiner";
                                break;
                            case "CPIER":
                                baseCalculoServicoComplementar = "por Contêiner";
                                break;
                        }

                        var minimoServicoComplementar = string.Empty;

                        if (servicoComplementar.PrecoMinimo > 0)
                            minimoServicoComplementar = $"com mínimo de {string.Format("{0:C2}", servicoComplementar.PrecoMinimo)}";

                        GravaCelula(new ExcelCelulaParametros(string.Concat(servicoComplementar.Descricao.Capitalize(), ": ", string.Format("{0:C2}", servicoComplementar.PrecoUnitario), " ", baseCalculoServicoComplementar, " ", minimoServicoComplementar), false, false, false), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);

                        excelWorksheetFCL.Cells[linha, 1, linha, totalColunas].Merge = true;

                        excelWorksheetFCL.Cells[linha, 1, linha, totalColunas].Style.Border.Top.Style = ExcelBorderStyle.None;
                        excelWorksheetFCL.Cells[linha, 1, linha, totalColunas].Style.Border.Bottom.Style = ExcelBorderStyle.None;
                        excelWorksheetFCL.Cells[linha, 1, linha, totalColunas].Style.Border.Left.Style = ExcelBorderStyle.None;
                        excelWorksheetFCL.Cells[linha, 1, linha, totalColunas].Style.Border.Right.Style = ExcelBorderStyle.None;

                        PularLinhaResetaColuna(ref linha, ref coluna);
                    }

                    foreach (var tabela in tabelas)
                    {
                        var adicionalArmazenagem = _servicoDAO.ObterAdicionalArmazenagem(tabela.TabelaId, 2);

                        GravaCelula(new ExcelCelulaParametros($"Adicional Armazenagem: { adicionalArmazenagem.ToString("0.##") }% a partir do segundo período sobre 1,5 do CIF por período;", false, false, false), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);

                        excelWorksheetFCL.Cells[linha, 1, linha, totalColunas].Merge = true;

                        excelWorksheetFCL.Cells[linha, 1, linha, totalColunas].Style.Border.Top.Style = ExcelBorderStyle.None;
                        excelWorksheetFCL.Cells[linha, 1, linha, totalColunas].Style.Border.Bottom.Style = ExcelBorderStyle.None;
                        excelWorksheetFCL.Cells[linha, 1, linha, totalColunas].Style.Border.Left.Style = ExcelBorderStyle.None;
                        excelWorksheetFCL.Cells[linha, 1, linha, totalColunas].Style.Border.Right.Style = ExcelBorderStyle.None;

                        PularLinhaResetaColuna(ref linha, ref coluna);

                        var adicionaisAnvisa = _servicoDAO.ObterAdicionaisAnvisa(tabela.TabelaId, 2);

                        foreach (var adicionalAnvisa in adicionaisAnvisa)
                        {
                            GravaCelula(new ExcelCelulaParametros($"Adicional ANVISA {adicionalAnvisa.Descricao.Capitalize()}: { adicionalAnvisa.ValorAnvisa.ToString("0.##") }%", false, false, false), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);

                            excelWorksheetFCL.Cells[linha, 1, linha, totalColunas].Merge = true;

                            excelWorksheetFCL.Cells[linha, 1, linha, totalColunas].Style.Border.Top.Style = ExcelBorderStyle.None;
                            excelWorksheetFCL.Cells[linha, 1, linha, totalColunas].Style.Border.Bottom.Style = ExcelBorderStyle.None;
                            excelWorksheetFCL.Cells[linha, 1, linha, totalColunas].Style.Border.Left.Style = ExcelBorderStyle.None;
                            excelWorksheetFCL.Cells[linha, 1, linha, totalColunas].Style.Border.Right.Style = ExcelBorderStyle.None;

                            PularLinhaResetaColuna(ref linha, ref coluna);
                        }

                        var adicionais = _servicoDAO.ObterAdicionais(tabela.TabelaId, 2);

                        foreach (var adicional in adicionais)
                        {
                            GravaCelula(new ExcelCelulaParametros($"Adicional IMO {adicional.Descricao.Capitalize()}: { adicional.ValorAcrescimo.ToString("0.##") }%", false, false, false), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);

                            excelWorksheetFCL.Cells[linha, 1, linha, totalColunas].Merge = true;

                            excelWorksheetFCL.Cells[linha, 1, linha, totalColunas].Style.Border.Top.Style = ExcelBorderStyle.None;
                            excelWorksheetFCL.Cells[linha, 1, linha, totalColunas].Style.Border.Bottom.Style = ExcelBorderStyle.None;
                            excelWorksheetFCL.Cells[linha, 1, linha, totalColunas].Style.Border.Left.Style = ExcelBorderStyle.None;
                            excelWorksheetFCL.Cells[linha, 1, linha, totalColunas].Style.Border.Right.Style = ExcelBorderStyle.None;

                            PularLinhaResetaColuna(ref linha, ref coluna);
                        }

                        GravaCelula(new ExcelCelulaParametros("Produtos controlados pelo Exército, Polícias Federal e Civil: Adicional de 35 % sobre os itens: Armazenagem e Gerenciamento de Risco;", false, false, false), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);

                        excelWorksheetFCL.Cells[linha, 1, linha, totalColunas].Merge = true;

                        excelWorksheetFCL.Cells[linha, 1, linha, totalColunas].Style.Border.Top.Style = ExcelBorderStyle.None;
                        excelWorksheetFCL.Cells[linha, 1, linha, totalColunas].Style.Border.Bottom.Style = ExcelBorderStyle.None;
                        excelWorksheetFCL.Cells[linha, 1, linha, totalColunas].Style.Border.Left.Style = ExcelBorderStyle.None;
                        excelWorksheetFCL.Cells[linha, 1, linha, totalColunas].Style.Border.Right.Style = ExcelBorderStyle.None;
                    }

                    if (!string.IsNullOrEmpty(observacoesModelo))
                    {
                        PularLinhaResetaColuna(ref linha, ref coluna);
                        PularLinhaResetaColuna(ref linha, ref coluna);

                        var linhasObs = observacoesModelo.Split('\n');

                        foreach (var linhaObs in linhasObs)
                        {
                            if (linhaObs.Contains("!"))
                            {
                                var servico = Regex.Matches(linhaObs, @"(?<=\!)[^}]*(?=\!)");

                                if (servico.Count > 0)
                                {
                                    var capture = servico.Cast<Match>().FirstOrDefault();

                                    var descricao = linhaObs.Replace(capture.Value, "");
                                    var valor = capture.Value;

                                    GravaCelula(new ExcelCelulaParametros(descricao.Replace("!!", ""), false, false, false), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);
                                    GravaCelula(new ExcelCelulaParametros(string.Empty, false, false, false), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);
                                    GravaCelula(new ExcelCelulaParametros(string.Empty, false, false, false), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);

                                    excelWorksheetFCL.Cells[linha, 1, linha, 3].Merge = true;

                                    GravaCelula(new ExcelCelulaParametros(valor, false, false, false), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);

                                    excelWorksheetFCL.Cells[linha, 4, linha, totalColunas].Merge = true;

                                    excelWorksheetFCL.Cells[linha, 1, linha, totalColunas].Style.Border.Top.Style = ExcelBorderStyle.None;
                                    excelWorksheetFCL.Cells[linha, 1, linha, totalColunas].Style.Border.Bottom.Style = ExcelBorderStyle.None;
                                    excelWorksheetFCL.Cells[linha, 1, linha, totalColunas].Style.Border.Left.Style = ExcelBorderStyle.None;
                                    excelWorksheetFCL.Cells[linha, 1, linha, totalColunas].Style.Border.Right.Style = ExcelBorderStyle.None;

                                    PularLinhaResetaColuna(ref linha, ref coluna);
                                }
                            }
                            else
                            {
                                GravaCelula(new ExcelCelulaParametros(linhaObs, false, false, false), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);

                                excelWorksheetFCL.Cells[linha, 1, linha, totalColunas].Merge = true;

                                excelWorksheetFCL.Cells[linha, 1, linha, totalColunas].Style.Border.Top.Style = ExcelBorderStyle.None;
                                excelWorksheetFCL.Cells[linha, 1, linha, totalColunas].Style.Border.Bottom.Style = ExcelBorderStyle.None;
                                excelWorksheetFCL.Cells[linha, 1, linha, totalColunas].Style.Border.Left.Style = ExcelBorderStyle.None;
                                excelWorksheetFCL.Cells[linha, 1, linha, totalColunas].Style.Border.Right.Style = ExcelBorderStyle.None;

                                PularLinhaResetaColuna(ref linha, ref coluna);
                            }

                            excelWorksheetFCL.Cells[linha, 1, linha, totalColunas].Style.WrapText = true;
                        }
                    }

                    for (int i = 1; i <= 2; i++)
                        PularLinhaResetaColuna(ref linha, ref coluna);

                    GravaCelula(new ExcelCelulaParametros("Validade:", true, false, false), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);

                    excelWorksheetFCL.Cells[linha, 1, linha, totalColunas].Merge = true;

                    excelWorksheetFCL.Cells[linha, 1, linha, totalColunas].Style.Border.Top.Style = ExcelBorderStyle.None;
                    excelWorksheetFCL.Cells[linha, 1, linha, totalColunas].Style.Border.Bottom.Style = ExcelBorderStyle.None;
                    excelWorksheetFCL.Cells[linha, 1, linha, totalColunas].Style.Border.Left.Style = ExcelBorderStyle.None;
                    excelWorksheetFCL.Cells[linha, 1, linha, totalColunas].Style.Border.Right.Style = ExcelBorderStyle.None;

                    for (int i = 1; i <= 2; i++)
                        PularLinhaResetaColuna(ref linha, ref coluna);

                    GravaCelula(new ExcelCelulaParametros($"Cotação válida por 10 dias a contar da data de emissão", false, false, false), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);

                    excelWorksheetFCL.Cells[linha, 1, linha, totalColunas].Merge = true;

                    excelWorksheetFCL.Cells[linha, 1, linha, totalColunas].Style.Border.Top.Style = ExcelBorderStyle.None;
                    excelWorksheetFCL.Cells[linha, 1, linha, totalColunas].Style.Border.Bottom.Style = ExcelBorderStyle.None;
                    excelWorksheetFCL.Cells[linha, 1, linha, totalColunas].Style.Border.Left.Style = ExcelBorderStyle.None;
                    excelWorksheetFCL.Cells[linha, 1, linha, totalColunas].Style.Border.Right.Style = ExcelBorderStyle.None;

                    PularLinhaResetaColuna(ref linha, ref coluna);
                    GravaCelula(new ExcelCelulaParametros("Após recebermos a confirmação desta cotação, a proposta formal será registrada em sistema para posterior análise, aceite e assinatura do cliente", false, false, false), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);

                    excelWorksheetFCL.Cells[linha, 1, linha, totalColunas].Merge = true;

                    excelWorksheetFCL.Cells[linha, 1, linha, totalColunas].Style.Border.Top.Style = ExcelBorderStyle.None;
                    excelWorksheetFCL.Cells[linha, 1, linha, totalColunas].Style.Border.Bottom.Style = ExcelBorderStyle.None;
                    excelWorksheetFCL.Cells[linha, 1, linha, totalColunas].Style.Border.Left.Style = ExcelBorderStyle.None;
                    excelWorksheetFCL.Cells[linha, 1, linha, totalColunas].Style.Border.Right.Style = ExcelBorderStyle.None;

                    PularLinhaResetaColuna(ref linha, ref coluna);
                    GravaCelula(new ExcelCelulaParametros("Valores estarão vigentes para processos entrados No terminal em data igual ou posterior à da emissão da proposta assinada pelo cliente", false, false, false), ref excelWorksheetFCL, ref celula, ref linha, ref coluna, ref borda);

                    excelWorksheetFCL.Cells[linha, 1, linha, totalColunas].Merge = true;

                    excelWorksheetFCL.Cells[linha, 1, linha, totalColunas].Style.Border.Top.Style = ExcelBorderStyle.None;
                    excelWorksheetFCL.Cells[linha, 1, linha, totalColunas].Style.Border.Bottom.Style = ExcelBorderStyle.None;
                    excelWorksheetFCL.Cells[linha, 1, linha, totalColunas].Style.Border.Left.Style = ExcelBorderStyle.None;
                    excelWorksheetFCL.Cells[linha, 1, linha, totalColunas].Style.Border.Right.Style = ExcelBorderStyle.None;

                    excelWorksheetFCL.View.ShowGridLines = false;
                 //   excelWorksheetFCL.Workbook.Calculate(); 

                    excelWorksheetFCL.View.ShowGridLines = false;

                    AdicionarImagem(excelWorksheetFCL, 1, 1, "");

                    if (excelWorksheetFCL.Dimension != null)
                    {
                        //  excelWorksheetFCL.Cells[excelWorksheetFCL.Dimension.Address].AutoFitColumns();
                    }

                }
                #endregion

                var parametros = _parametrosDAO.ObterParametros();

                var nomeArquivo = $@"Simulador_{oportunidadeBusca.Identificacao}_{parametrosSimulador.Regime.ToName()}";

                int tamanhoArquivo = 0;
                var hash = string.Empty;
                var arquivoId = 0;

                var valorImposto = valorImpostoFCL / (subTotalMD_20 == 0 ? 1 : subTotalMD_20);

                if (parametrosSimulador.Regime == Regime.LCL)
                {
                    valorImposto = impostoCalculoTicket / (subTotalMD_20 == 0 ? 1 : subTotalMD_20);
                }

                var valorTotalTicketMdir = _oportunidadeDAO.ObterValorTotalSimulador(filtro.TabelaId, "MDIR", parametrosSimulador.Regime);
                var valorTotalImpostoMdir = valorTotalTicketMdir * valorImposto;

                var valorTotalTicketMesq = _oportunidadeDAO.ObterValorTotalSimulador(filtro.TabelaId, "MESQ", parametrosSimulador.Regime);
                var valorTotalImpostoMesq = valorTotalTicketMesq * valorImposto;

                var valorTotalTicket = 0.0M;

                if (parametrosSimulador.Regime == Regime.FCL)
                {
                    //    valorTotalTicket = valorTotalTicketMdir + valorTotalImpostoMdir + valorTotalTicketMesq + valorTotalImpostoMesq;
                    valorTotalTicket = valorTotalTicketMdir + valorTotalImpostoMdir ;
                }
                else
                {
                    valorTotalTicket = valorTotalTicketMdir + valorTotalImpostoMdir;
                }
                
                if (parametros.AtualizaValorTicket && modeloBusca.Simular)
                {
                    _oportunidadeDAO.AtualizarValorTicket(oportunidadeBusca.Id, valorTotalTicket, parametrosSimulador.Regime);
                }

                var dados = pck.GetAsByteArray();

                hash = IncluirAnexo(dados, oportunidadeBusca.Id, nomeArquivo, "xlsx", dadosSimulador.CriadoPor, ref tamanhoArquivo);

                if (parametros.AnexarSimulador && modeloBusca.Simular)
                {
                    arquivoId = _anexosDAO.IncluirAnexo(
                        new Anexo
                        {
                            IdProcesso = oportunidadeBusca.Id,
                            Arquivo = string.Concat(nomeArquivo, ".xlsx"),
                            CriadoPor = filtro.UsuarioSimuladorId,
                            TipoAnexo = 7,
                            TipoDoc = 3,
                            IdArquivo = hash
                        });

                    var anexosOportunidade = _anexosDAO.ObterAnexosOportunidadePorTipo(oportunidadeBusca.Id, Ecoporto.CRM.Business.Enums.TipoAnexo.RELATORIO_SIMULADOR);

                    foreach (var anexo in anexosOportunidade)
                    {
                        if (anexo.Arquivo.ToLower().StartsWith("simulador_") && anexo.Id != arquivoId)
                        {
                            var fileId = Converters.RawToGuid(anexo.IdArquivo);

                            var token = UploadService.Autenticar();

                            if (token == null)
                                throw new Exception("Não foi possível se autenticar no serviço de Anexos");

                            var retornoUpload = new UploadService(token)
                                .ExcluirArquivo(fileId);

                            if (retornoUpload.success)
                            {
                                _anexosDAO.Excluir(anexo.Id);
                            }
                        }
                    }
                }

                return new Response
                {
                    Sucesso = true,
                    Mensagem = "Relatório gerado com sucesso",
                    ArquivoId = arquivoId,
                    Hash = hash,
                    NomeArquivo = nomeArquivo,
                    Base64 = Convert.ToBase64String(dados),
                    TamanhoArquivo = tamanhoArquivo
                };
            }
        }

        private string IncluirAnexo(byte[] bytes, int oportunidadeId, string nomeArquivo, string extensaoArquivo, int usuarioId, ref int tamanho)
        {
            if (bytes != null && bytes.Length > 0)
            {
                var token = UploadService.Autenticar();

                if (token == null)
                    throw new Exception("Não foi possível se autenticar no serviço de Anexos");

                var dados = new DadosArquivoUpload
                {
                    Name = string.Concat(nomeArquivo, ".", extensaoArquivo),
                    Extension = extensaoArquivo,
                    System = 3,
                    DataArray = Convert.ToBase64String(bytes)
                };

                var retornoUpload = new UploadService(token)
                    .EnviarArquivo(dados);

                if (!retornoUpload.success)
                    throw new Exception(string.Concat("Retorno API anexos: ", retornoUpload.message));

                tamanho = bytes.Length;

                return GuidHelpers.GuidToRaw(retornoUpload.Arquivo.id);
            }

            return string.Empty;
        }

        private void AjustarLarguraColuna(ref ExcelWorksheet ws, int coluna, int larguraEmPixels)
        {
            ws.Column(coluna).Width = larguraEmPixels;
        }

        private void Merge(ref ExcelWorksheet ws, ref int linha, int total)
        {
            ws.Cells[linha, 1, linha, total + 1].Merge = true;
            ws.Cells[linha, 1].Style.WrapText = true;
        }

        private void PintarLinha(ref ExcelWorksheet ws, ref ExcelRange celula, ref int linha, int coluna)
        {
            celula = ws.Cells[linha, coluna];
            celula.Style.Fill.PatternType = ExcelFillStyle.Solid;
            celula.Style.Fill.BackgroundColor.SetColor(Color.Silver);
        }
        private void PularLinhaResetaColuna(ref int linha, ref int coluna)
        {
            linha += 1;
            coluna = 1;
        }

        private void ResetaLinhaColuna(ref int linha, ref int coluna)
        {
            linha = 1;
            coluna = 1;
        }

        private void AdicionarImagem(ExcelWorksheet ws, int linha, int coluna, string caminhoImagem)
        {
            Bitmap logotipo = null;

            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                logotipo = new Bitmap(Path.Combine(HttpContext.Current.Server.MapPath("~/"), "logoEcoporto.png"));
            }
            else
            {
                logotipo = new Bitmap(Path.Combine(HttpContext.Current.Server.MapPath("~/"), "logoBandeirantes.png"));
            }

            if (logotipo != null)
            {
                ExcelPicture excelImage = ws.Drawings.AddPicture("Logotipo", logotipo);
                excelImage.SetPosition(0, 0, 0, 0);
            }
        }

        public int Pixel2MTU(int pixels)
        {
            int mtus = pixels * 9525;
            return mtus;
        }

        private void GravaCelula(ExcelCelulaParametros parametros, ref ExcelWorksheet ws, ref ExcelRange celula, ref int linha, ref int coluna, ref Border borda, TipoCelulaExcel tipoCelulaExcel = TipoCelulaExcel.Texto, string mascara = "")
        {
            coluna = coluna == 0 ? 1 : coluna;

            celula = ws.Cells[linha, coluna];

            if (tipoCelulaExcel == TipoCelulaExcel.Percentual)
            {
                if (string.IsNullOrEmpty(mascara))
                    celula.Style.Numberformat.Format = "#0.00####%";
                else
                    celula.Style.Numberformat.Format = mascara;

                if (string.IsNullOrEmpty(parametros.Valor))
                    celula.Value = 0;
                else
                    celula.Value = Convert.ToDecimal(parametros.Valor);
            }

            if (tipoCelulaExcel == TipoCelulaExcel.Percentual4Casas)
            {
                if (string.IsNullOrEmpty(mascara))
                    celula.Style.Numberformat.Format = "#0.0000%";
                else
                    celula.Style.Numberformat.Format = mascara;

                if (string.IsNullOrEmpty(parametros.Valor))
                    celula.Value = 0;
                else
                    celula.Value = Convert.ToDecimal(parametros.Valor);
            }

            if (tipoCelulaExcel == TipoCelulaExcel.Numero)
            {
                if (string.IsNullOrEmpty(mascara))
                    celula.Style.Numberformat.Format = "#,##0.00";
                else
                    celula.Style.Numberformat.Format = mascara;

                if (string.IsNullOrEmpty(parametros.Valor))
                    celula.Value = 0;
                else
                    celula.Value = Convert.ToDecimal(parametros.Valor);
            }

            if (tipoCelulaExcel == TipoCelulaExcel.Inteiro)
            {
                if (string.IsNullOrEmpty(parametros.Valor))
                    celula.Value = 0;
                else
                    celula.Value = Convert.ToInt32(parametros.Valor);
            }

            if (tipoCelulaExcel == TipoCelulaExcel.Moeda)
            {
                if (string.IsNullOrEmpty(mascara))
                    celula.Style.Numberformat.Format = "R$#,##0.00";
                else
                    celula.Style.Numberformat.Format = mascara;

                if (string.IsNullOrEmpty(parametros.Valor))
                    celula.Value = 0;
                else
                    celula.Value = Convert.ToDecimal(parametros.Valor);
            }

            if (tipoCelulaExcel == TipoCelulaExcel.Texto)
            {
                celula.Value = parametros.Valor;
            }

            if (parametros.Centraliza)
                celula.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            if (parametros.AlinhaDireita)
                celula.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            celula.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            var fonte = celula.Style.Font;
            fonte.Name = "Arial";
            fonte.Size = 10;

            fonte.Bold = parametros.Negrito;

            if (parametros.CorDeFundo)
            {
                celula.Style.Fill.PatternType = ExcelFillStyle.Solid;

                if (parametros.Cor.HasValue)
                    celula.Style.Fill.BackgroundColor.SetColor(parametros.Cor.Value);
                else
                    celula.Style.Fill.BackgroundColor.SetColor(Color.Silver);
            }

            borda = celula.Style.Border;
            borda.Top.Style = parametros.TipoBorda;
            borda.Bottom.Style = parametros.TipoBorda;
            borda.Left.Style = parametros.TipoBorda;
            borda.Right.Style = parametros.TipoBorda;

            if (parametros.Altura > 0)
                ws.Row(linha).Height = parametros.Altura;

            coluna += 1;
        }

        public double MeasureTextHeight(string text, ExcelFont font, int width)
        {
            if (string.IsNullOrEmpty(text)) return 0.0;
            var bitmap = new Bitmap(1, 1);
            var graphics = Graphics.FromImage(bitmap);

            var pixelWidth = Convert.ToInt32(width * 7.5);  //7.5 pixels per excel column width
            var drawingFont = new Font(font.Name, font.Size);
            var size = graphics.MeasureString(text, drawingFont, pixelWidth);

            //72 DPI and 96 points per inch.  Excel height in points with max of 409 per Excel requirements.
            return Math.Min(Convert.ToDouble(size.Height) * 72 / 96, 409);
        }
    }
}