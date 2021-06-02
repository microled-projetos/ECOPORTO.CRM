using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using WsSimuladorCalculoTabelas.Configuracao;
using WsSimuladorCalculoTabelas.DAO;
using WsSimuladorCalculoTabelas.Helpers;
using WsSimuladorCalculoTabelas.Models;
using WsSimuladorCalculoTabelas.Responses;
 using WsSimuladorCalculoTabelas.Enums;
 

 
namespace WsSimuladorCalculoTabelas.Services
{
    public class CalculoTabelasExcelService
    {
        private readonly SimuladorDAO _simuladorDAO;
        private readonly ParceirosDAO _parceiroDAO;
        private readonly ImpostoDAO _impostoDAO;
        private readonly AnexosDAO _anexosDAO;
        private readonly ServicoDAO _servicoDAO;

        public CalculoTabelasExcelService()
        {
            _simuladorDAO = new SimuladorDAO(false);
            _parceiroDAO = new ParceirosDAO();
            _impostoDAO = new ImpostoDAO();
            _anexosDAO = new AnexosDAO();
            _servicoDAO = new ServicoDAO(false);
        }

        public Response Gerar(GerarExcelFiltro filtro)
        {
            ExcelRange celula = null;
            Border borda = null;
            ExcelWorksheet excelWorksheet = null;

            var codigoServicoLista = new List<int>();

            int linha = 1;
            int coluna = 1;
            int totalColunas = 3;
            string tipoCliente = string.Empty;

            using (var pck = new ExcelPackage())
            {
                pck.Workbook.Properties.Author = "Ecoporto";
                pck.Workbook.Properties.Title = "Business Analysis";

                excelWorksheet = pck.Workbook.Worksheets.Add("Tabelas");

                var dadosSimulador = _simuladorDAO.ObterDetalhesSimuladorPorId(filtro.SimuladorId);

                if (dadosSimulador == null)
                    throw new Exception("Simulador não encontrado");

                var ehConteiner = (dadosSimulador.Regime == "FCL");

                var tabelas = _simuladorDAO.ObterTabelasCalculadas(dadosSimulador.SimuladorId);

                GravaCelula(new ExcelCelulaParametros("ID", true, true), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);

                foreach (var tabela in tabelas)
                {
                    GravaCelula(new ExcelCelulaParametros(tabela.Descricao, true, true), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);
                }

                GravaCelula(new ExcelCelulaParametros(string.Empty, true, true), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);

                excelWorksheet.Cells[linha, 2, linha, totalColunas].Merge = true;

                excelWorksheet.Cells[linha, 2, linha, totalColunas].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                excelWorksheet.Cells[linha, 2, linha, totalColunas].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                excelWorksheet.Cells[linha, 2, linha, totalColunas].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                excelWorksheet.Cells[linha, 2, linha, totalColunas].Style.Border.Right.Style = ExcelBorderStyle.Thin;


                PularLinhaResetaColuna(ref linha, ref coluna);

                switch (dadosSimulador.Classe)
                {
                    case 0:
                        tipoCliente = "CLIENTE ESPECÍFICO";
                        break;
                    case 1:
                        tipoCliente = "IMPORTADOR";
                        break;
                    case 2:
                        tipoCliente = "DESPACHANTE";
                        break;
                    case 3:
                        tipoCliente = "NVOCC";
                        break;
                    case 4:
                        tipoCliente = "COLOADER";
                        break;
                }

                GravaCelula(new ExcelCelulaParametros(tipoCliente, true, true, 38.25), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);

                foreach (var tabela in tabelas)
                {
                    var razaoSocial = _parceiroDAO.ObterRazaoSocial(filtro.DadosDoCliente, dadosSimulador.Classe, tabela.TabelaId);

                    if (razaoSocial != null)
                    {
                        GravaCelula(new ExcelCelulaParametros(razaoSocial.RazaoSocial, true, true), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);
                    }
                }

                GravaCelula(new ExcelCelulaParametros(string.Empty, true, true), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);

                excelWorksheet.Cells[linha, 2, linha, totalColunas].Merge = true;

                excelWorksheet.Cells[linha, 2, linha, totalColunas].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                excelWorksheet.Cells[linha, 2, linha, totalColunas].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                excelWorksheet.Cells[linha, 2, linha, totalColunas].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                excelWorksheet.Cells[linha, 2, linha, totalColunas].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                if (filtro.SomenteEstimativa == false)
                {
                    PularLinhaResetaColuna(ref linha, ref coluna);

                    GravaCelula(new ExcelCelulaParametros("SERVIÇOS", true, true, true), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);
                    GravaCelula(new ExcelCelulaParametros("VALOR MD", true, true), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);
                    GravaCelula(new ExcelCelulaParametros("VALOR ME", true, true), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);

                    for (int i = 1; i <= totalColunas; i++)
                    {
                        celula = excelWorksheet.Cells[linha, i];
                        celula.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        celula.Style.Fill.BackgroundColor.SetColor(Color.Silver);
                    }

                    PularLinhaResetaColuna(ref linha, ref coluna);

                    GravaCelula(new ExcelCelulaParametros($"ARMAZENAGEM {dadosSimulador.Regime}", true, false, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);

                    foreach (var tabela in tabelas)
                    {
                        var diasArmazenagem = _servicoDAO.ObterQuantidadeDias(tabela.TabelaId, ehConteiner);

                        GravaCelula(new ExcelCelulaParametros(diasArmazenagem, true, true), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);
                        GravaCelula(new ExcelCelulaParametros(string.Empty, true, true), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);
                    }

                    PularLinhaResetaColuna(ref linha, ref coluna);

                    for (int periodo = 1; periodo <= 4; periodo++)
                    {
                        coluna = 0;

                        foreach (var tabela in tabelas)
                        {
                            if (coluna == 0)
                            {
                                coluna = 1;
                                GravaCelula(new ExcelCelulaParametros($"{periodo}º Período", false, false, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);
                            }

                            var valoresArmazenagem = _servicoDAO.ObterValoresServicoArmazenagem(tabela.TabelaId, periodo, ehConteiner);

                            if (valoresArmazenagem != null)
                            {
                                if (valoresArmazenagem.BaseCalculo == "CIF" || valoresArmazenagem.BaseCalculo == "CIF0" || valoresArmazenagem.BaseCalculo == "CIFM")
                                {
                                    GravaCelula(new ExcelCelulaParametros(String.Format("{0:P2}", valoresArmazenagem.PrecoUnitario/100), false, true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda );
                                    GravaCelula(new ExcelCelulaParametros(String.Format("{0:P2}", valoresArmazenagem.PrecoUnitario/100), false, true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda );
                                }
                                else
                                {
                                    GravaCelula(new ExcelCelulaParametros(String.Format("{0:C2}", valoresArmazenagem.PrecoUnitario), false, true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda );
                                    GravaCelula(new ExcelCelulaParametros(String.Format("{0:C2}", valoresArmazenagem.PrecoUnitario), false, true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda );
                                }

                                if (periodo == 1)
                                    codigoServicoLista.Add(valoresArmazenagem.ServicoFixoVariavelId);
                            }
                            else
                            {
                                GravaCelula(new ExcelCelulaParametros(string.Empty, false, true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);
                                GravaCelula(new ExcelCelulaParametros(string.Empty, false, true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);

                                if (periodo == 1)
                                    codigoServicoLista.Add(0);
                            }
                        }

                        PularLinhaResetaColuna(ref linha, ref coluna);
                    }

                    coluna = 1;

                    GravaCelula(new ExcelCelulaParametros("Subsequentes", false, false, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);

                    coluna = 2;

                    foreach (var tabela in tabelas)
                    {
                        for (int periodo = dadosSimulador.Periodos + 4; periodo >= 1; periodo += -1)
                        {
                            var valoresArmazenagem = _servicoDAO.ObterValoresServicoArmazenagem(tabela.TabelaId, periodo, ehConteiner);

                            if (valoresArmazenagem != null)
                            {
                                if (valoresArmazenagem.BaseCalculo == "CIF" || valoresArmazenagem.BaseCalculo == "CIF0" || valoresArmazenagem.BaseCalculo == "CIFM")
                                {
                                    GravaCelula(new ExcelCelulaParametros(String.Format("{0:P2}", valoresArmazenagem.PrecoUnitario/100), false, true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);
                                    GravaCelula(new ExcelCelulaParametros(String.Format("{0:P2}", valoresArmazenagem.PrecoUnitario/100), false, true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);
                                }
                                else
                                {
                                    GravaCelula(new ExcelCelulaParametros(String.Format("{0:C2}", valoresArmazenagem.PrecoUnitario), false, true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);
                                    GravaCelula(new ExcelCelulaParametros(String.Format("{0:C2}", valoresArmazenagem.PrecoUnitario), false, true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);
                                }

                                break;
                            }
                        }
                    }

                    PularLinhaResetaColuna(ref linha, ref coluna);

                    GravaCelula(new ExcelCelulaParametros("Valor Mínimo de Cobrança", false, false, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);

                    foreach (var codigoServico in codigoServicoLista)
                    {
                        var precoMinimo = _servicoDAO.ObterPrecoMinimoServicoVariavelPorId(codigoServico, ehConteiner);

                        if (precoMinimo != null)
                        {
                            if (precoMinimo.Value > 0)
                            {
                                precoMinimo = precoMinimo.Value;
                            }

                            ServicoFixoVariavel minimoPorFaixa = null;

                            if (dadosSimulador.Regime == "FCL")
                                minimoPorFaixa = _servicoDAO.ObterDetalhesServicoVariavelPorFaixas(codigoServico, dadosSimulador.ValorCifConteiner, "C");
                            else
                                minimoPorFaixa = _servicoDAO.ObterDetalhesServicoVariavelPorFaixas(codigoServico, dadosSimulador.ValorCifCargaSolta, "C");

                            if (minimoPorFaixa != null)
                            {
                                precoMinimo = minimoPorFaixa.ValorMinimo;
                            }

                            if (dadosSimulador.Regime == "LCL")
                            {
                                dadosSimulador.NumeroLotes = dadosSimulador.NumeroLotes == 0 ? 1 : dadosSimulador.NumeroLotes;

                                minimoPorFaixa = _servicoDAO.ObterDetalhesServicoVariavelPorFaixasPeso(codigoServico, dadosSimulador.NumeroLotes, "SVAR40");

                                if (minimoPorFaixa != null)
                                {
                                    precoMinimo = minimoPorFaixa.ValorMinimo;
                                }
                                else
                                {
                                    minimoPorFaixa = _servicoDAO.ObterDetalhesServicoVariavelPorFaixasPeso(codigoServico, dadosSimulador.NumeroLotes, "SVAR20");

                                    if (minimoPorFaixa != null)
                                    {
                                        precoMinimo = minimoPorFaixa.ValorMinimo;
                                    }
                                    else
                                    {
                                        minimoPorFaixa = _servicoDAO.ObterDetalhesServicoVariavelPorFaixasPeso(codigoServico, dadosSimulador.NumeroLotes, "SVAR");

                                        if (minimoPorFaixa != null)
                                        {
                                            precoMinimo = minimoPorFaixa.ValorMinimo;
                                        }
                                    }
                                }
                            }
                        }

                        GravaCelula(new ExcelCelulaParametros(string.Format("{0:C2}", precoMinimo), true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);
                        GravaCelula(new ExcelCelulaParametros(string.Format("{0:C2}", precoMinimo), true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);
                    }

                    PularLinhaResetaColuna(ref linha, ref coluna);

                    bool cab = false;

                    foreach (var codigoServico in codigoServicoLista)
                    {
                        var minimoPorFaixa = _servicoDAO.ObterDetalhesServicoVariavelPorFaixasPeso(codigoServico, 3, "SVAR40");

                        if (minimoPorFaixa == null)
                        {
                            minimoPorFaixa = _servicoDAO.ObterDetalhesServicoVariavelPorFaixasPeso(codigoServico, 3, "SVAR20");
                        }

                        if (minimoPorFaixa != null)
                        {
                            if (minimoPorFaixa.ValorMinimo > 0)
                            {
                                if (cab == false)
                                {
                                    GravaCelula(new ExcelCelulaParametros("Acima de 02 B/L p/ CC 20' ou 40' e/ou por lote LCL", 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);
                                    cab = true;
                                }

                                GravaCelula(new ExcelCelulaParametros(string.Format("{0:C2}", minimoPorFaixa.ValorMinimo), true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);
                                GravaCelula(new ExcelCelulaParametros(string.Format("{0:C2}", minimoPorFaixa.ValorMinimo), true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);
                            }
                        }
                    }

                    if (cab)
                    {
                        PularLinhaResetaColuna(ref linha, ref coluna);
                    }

                    cab = false;

                    foreach (var codigoServico in codigoServicoLista)
                    {
                        var minimoPorFaixa = _servicoDAO.ObterDetalhesServicoVariavelPorFaixasPeso(codigoServico, 3, "SVAR20");

                        if (minimoPorFaixa == null && dadosSimulador.Regime == "FCL")
                        {
                            minimoPorFaixa = _servicoDAO.ObterDetalhesServicoVariavelPorFaixasPeso(codigoServico, 3, "SVAR20", 52, "CPIER");
                        }

                        if (minimoPorFaixa != null)
                        {
                            if (minimoPorFaixa.ValorMinimo > 0)
                            {
                                if (cab == false)
                                {
                                    GravaCelula(new ExcelCelulaParametros("Até 02 B/L por CC 20' (por lote)", 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);
                                    cab = true;
                                }
                                
                                GravaCelula(new ExcelCelulaParametros(string.Format("{0:C2}", minimoPorFaixa.ValorMinimo), true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);
                                GravaCelula(new ExcelCelulaParametros(string.Format("{0:C2}", minimoPorFaixa.ValorMinimo), true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);
                            }
                        }
                    }

                    if (cab)
                    {
                        PularLinhaResetaColuna(ref linha, ref coluna);
                    }

                    cab = false;

                    foreach (var codigoServico in codigoServicoLista)
                    {
                        var minimoPorFaixa = _servicoDAO.ObterDetalhesServicoVariavelPorFaixasPeso(codigoServico, 3, "SVAR40");

                        if (minimoPorFaixa == null && dadosSimulador.Regime == "FCL")
                        {
                            minimoPorFaixa = _servicoDAO.ObterDetalhesServicoVariavelPorFaixasPeso(codigoServico, 3, "SVAR40", 52, "CPIER");
                        }

                        if (minimoPorFaixa != null)
                        {
                            if (minimoPorFaixa.ValorMinimo > 0)
                            {
                                if (cab == false)
                                {
                                    GravaCelula(new ExcelCelulaParametros("Até 02 B/L por CC 40' (por lote)", 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);
                                    cab = true;
                                }

                                GravaCelula(new ExcelCelulaParametros(string.Format("{0:C2}", minimoPorFaixa.ValorMinimo), true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);
                                GravaCelula(new ExcelCelulaParametros(string.Format("{0:C2}", minimoPorFaixa.ValorMinimo), true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);
                            }
                        }
                    }

                    if (cab)
                    {
                        PularLinhaResetaColuna(ref linha, ref coluna);
                    }

                    cab = false;                    

                    GravaCelula(new ExcelCelulaParametros(string.Empty, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);

                    if (filtro.ServicosComplementares)
                    {
                        PularLinhaResetaColuna(ref linha, ref coluna);

                        GravaCelula(new ExcelCelulaParametros("SERVIÇOS COMPLEMENTARES", true, true, true), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);

                        for (int i = 1; i <= totalColunas; i++)
                        {
                            PintarLinha(ref excelWorksheet, ref celula, ref linha, i);
                        }

                        PularLinhaResetaColuna(ref linha, ref coluna);

                        var servicosSimuladorSemArmazenagem = _simuladorDAO.ObterServicosSemArmazenagemSimulador(dadosSimulador.SimuladorId, dadosSimulador.Regime);

                        foreach (var servico in servicosSimuladorSemArmazenagem)
                        {
                            GravaCelula(new ExcelCelulaParametros(servico.Descricao, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);

                            foreach (var tabela in tabelas)
                            {
                                var precoUnitario = _servicoDAO.ObterPrecoUnitarioPorTabelaEServico(tabela.TabelaId, servico.ServicoId);

                                GravaCelula(new ExcelCelulaParametros(string.Format("{0:C2}", precoUnitario), true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);
                            }

                            PularLinhaResetaColuna(ref linha, ref coluna);
                        }
                    }
                }

                PularLinhaResetaColuna(ref linha, ref coluna);

                var filtroSimulador = dadosSimulador.ObterFiltro();

                GravaCelula(new ExcelCelulaParametros($"Dados do Cálculo: {filtroSimulador}", true, 26, Color.Wheat), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);

                Merge(ref excelWorksheet, ref linha, 2);

                PularLinhaResetaColuna(ref linha, ref coluna);

                GravaCelula(new ExcelCelulaParametros($"ESTIMATIVA: {filtroSimulador}", true, true, true), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);

                for (int i = 1; i <= totalColunas; i++)
                {
                    PintarLinha(ref excelWorksheet, ref celula, ref linha, i);
                }

                PularLinhaResetaColuna(ref linha, ref coluna);

                var servicosSimulador = _simuladorDAO.ObterServicosSimulador(dadosSimulador.SimuladorId);

                foreach (var servico in servicosSimulador)
                {
                    GravaCelula(new ExcelCelulaParametros(servico.Descricao, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);

                    foreach (var tabela in tabelas)
                    {
                       //var valor = _servicoDAO.ObterValoresServicoSimuladorPorTabelaId(dadosSimulador.SimuladorId, tabela.TabelaId, servico.ServicoId, string.Empty);

                        GravaCelula(new ExcelCelulaParametros(string.Format("{0:C2}", servico.PrecoMinimoMDir ), true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);
                        GravaCelula(new ExcelCelulaParametros(string.Format("{0:C2}", servico.PrecoMinimoMEsq  ), true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);
                    }

                    PularLinhaResetaColuna(ref linha, ref coluna);
                }

                GravaCelula(new ExcelCelulaParametros("IMPOSTOS", 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);

                var taxaImposto = _impostoDAO.ObterTaxaImposto();

                taxaImposto = 1 - taxaImposto;

                foreach (var tabela in tabelas)
                {
                    var valorImposto = _simuladorDAO.ObterValorImposto(taxaImposto, dadosSimulador.SimuladorId, tabela.TabelaId, "MDIR");

                    GravaCelula(new ExcelCelulaParametros(string.Format("{0:C2}", valorImposto), true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);
                    var valorImpostoE = _simuladorDAO.ObterValorImposto(taxaImposto, dadosSimulador.SimuladorId, tabela.TabelaId, "MESQ");
                    GravaCelula(new ExcelCelulaParametros(string.Format("{0:C2}", valorImpostoE), true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);
                }

                PularLinhaResetaColuna(ref linha, ref coluna);

                GravaCelula(new ExcelCelulaParametros("Total:", true, 12.75, true), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);

                foreach (var tabela in tabelas)
                {
                    var valorComImposto = _simuladorDAO.ObterValorComImposto(taxaImposto, dadosSimulador.SimuladorId, tabela.TabelaId, "MDIR");

                    GravaCelula(new ExcelCelulaParametros(string.Format("{0:C2}", valorComImposto), true, 12.75, true), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);
                    var valorComImpostoE = _simuladorDAO.ObterValorComImposto(taxaImposto, dadosSimulador.SimuladorId, tabela.TabelaId, "MESQ");
                    GravaCelula(new ExcelCelulaParametros(string.Format("{0:C2}", valorComImpostoE), true, 12.75, true), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);
                }

                AjustarLarguraColuna(ref excelWorksheet, coluna: 1, larguraEmPixels: 50);

                for (int i = 2; i < totalColunas; i++)
                {
                    AjustarLarguraColuna(ref excelWorksheet, coluna: i, larguraEmPixels: 26);
                }

                excelWorksheet.Cells[1, totalColunas].Style.WrapText = true;

                if (excelWorksheet.Dimension != null)
                {
                    excelWorksheet.Cells[excelWorksheet.Dimension.Address].AutoFitColumns();
                }

                if (filtro.ComAnaliseDeDados == true)
                {
                    ResetaLinhaColuna(ref linha, ref coluna);

                    var colunas = new[] { "Tabela", "Regime", "ID Tabela", "Data Início", "Data Fim", "Tabela Ativa?", "Data Pagamento", "Faturado", "Lotes" };

                    var wsPier = pck.Workbook.Worksheets.Add("Pier");

                    var grupoNivel = 1;

                    for (int i = 0; i <= colunas.Count() - 1; i++)
                        GravaCelula(new ExcelCelulaParametros(colunas[i], true), ref wsPier, ref celula, ref linha, ref coluna, ref borda);

                    PularLinhaResetaColuna(ref linha, ref coluna);

                    var resumoPier = _simuladorDAO.ObterResumoPierHouse(dadosSimulador.SimuladorId, "PP", filtro.DataPgtoInicial, filtro.DataPgtoFinal);

                    foreach (var resumo in resumoPier)
                    {
                        GravaCelula(new ExcelCelulaParametros(resumo.Descricao, true, 12.75), ref wsPier, ref celula, ref linha, ref coluna, ref borda);

                        for (int i = 1; i <= 6; i++)
                            GravaCelula(new ExcelCelulaParametros(string.Empty, 12.75), ref wsPier, ref celula, ref linha, ref coluna, ref borda);

                        GravaCelula(new ExcelCelulaParametros(string.Format("{0:C2}", resumo.Faturado), true, 12.75), ref wsPier, ref celula, ref linha, ref coluna, ref borda);
                        GravaCelula(new ExcelCelulaParametros(resumo.Lotes.ToString(), true, 12.75), ref wsPier, ref celula, ref linha, ref coluna, ref borda);

                        PularLinhaResetaColuna(ref linha, ref coluna);

                        var valoresPierHouse = _simuladorDAO.ObterValoresPierHouse(dadosSimulador.SimuladorId, "PP", filtro.DataPgtoInicial, filtro.DataPgtoFinal, resumo.TabelaId);

                        foreach (var valor in valoresPierHouse)
                        {
                            wsPier.Row(linha).OutlineLevel = grupoNivel;
                            wsPier.Row(linha).Collapsed = true;

                            GravaCelula(new ExcelCelulaParametros(valor.Descricao, 12.75), ref wsPier, ref celula, ref linha, ref coluna, ref borda);
                            GravaCelula(new ExcelCelulaParametros(valor.Regime, 12.75), ref wsPier, ref celula, ref linha, ref coluna, ref borda);
                            GravaCelula(new ExcelCelulaParametros(valor.TabelaId, 12.75), ref wsPier, ref celula, ref linha, ref coluna, ref borda);
                            GravaCelula(new ExcelCelulaParametros(valor.DataInicial, 12.75), ref wsPier, ref celula, ref linha, ref coluna, ref borda);
                            GravaCelula(new ExcelCelulaParametros(valor.DataFinal, 12.75), ref wsPier, ref celula, ref linha, ref coluna, ref borda);
                            GravaCelula(new ExcelCelulaParametros(valor.Status, 12.75), ref wsPier, ref celula, ref linha, ref coluna, ref borda);
                            GravaCelula(new ExcelCelulaParametros(valor.DataPagamento, 12.75), ref wsPier, ref celula, ref linha, ref coluna, ref borda);
                            GravaCelula(new ExcelCelulaParametros(string.Format("{0:C2}", valor.Faturado), 12.75), ref wsPier, ref celula, ref linha, ref coluna, ref borda);
                            GravaCelula(new ExcelCelulaParametros(valor.Lotes, 12.75), ref wsPier, ref celula, ref linha, ref coluna, ref borda);

                            PularLinhaResetaColuna(ref linha, ref coluna);
                        }

                        grupoNivel += 1;
                    }

                    grupoNivel = 1;

                    if (wsPier.Dimension != null)
                    {
                        wsPier.Cells[wsPier.Dimension.Address].AutoFitColumns();
                    }

                    wsPier.Cells[$"A1:I{linha}"].AutoFilter = true;

                    ResetaLinhaColuna(ref linha, ref coluna);

                    var wsHouse = pck.Workbook.Worksheets.Add("House");

                    for (int i = 0; i <= colunas.Count() - 1; i++)
                        GravaCelula(new ExcelCelulaParametros(colunas[i], true), ref wsHouse, ref celula, ref linha, ref coluna, ref borda);

                    PularLinhaResetaColuna(ref linha, ref coluna);

                    var resumoHouse = _simuladorDAO.ObterResumoPierHouse(dadosSimulador.SimuladorId, "HH", filtro.DataPgtoInicial, filtro.DataPgtoFinal);

                    foreach (var resumo in resumoPier)
                    {
                        GravaCelula(new ExcelCelulaParametros(resumo.Descricao, true, 12.75), ref wsHouse, ref celula, ref linha, ref coluna, ref borda);

                        for (int i = 1; i <= 6; i++)
                            GravaCelula(new ExcelCelulaParametros(string.Empty, 12.75), ref wsHouse, ref celula, ref linha, ref coluna, ref borda);

                        GravaCelula(new ExcelCelulaParametros(string.Format("{0:C2}", resumo.Faturado), true, 12.75), ref wsHouse, ref celula, ref linha, ref coluna, ref borda);
                        GravaCelula(new ExcelCelulaParametros(resumo.Lotes.ToString(), true, 12.75), ref wsHouse, ref celula, ref linha, ref coluna, ref borda);

                        PularLinhaResetaColuna(ref linha, ref coluna);

                        var valoresPierHouse = _simuladorDAO.ObterValoresPierHouse(dadosSimulador.SimuladorId, "HH", filtro.DataPgtoInicial, filtro.DataPgtoFinal, resumo.TabelaId);

                        foreach (var valor in valoresPierHouse)
                        {
                            wsPier.Row(linha).OutlineLevel = grupoNivel;
                            wsPier.Row(linha).Collapsed = true;

                            GravaCelula(new ExcelCelulaParametros(valor.Descricao, 12.75), ref wsHouse, ref celula, ref linha, ref coluna, ref borda);
                            GravaCelula(new ExcelCelulaParametros(valor.Regime, 12.75), ref wsHouse, ref celula, ref linha, ref coluna, ref borda);
                            GravaCelula(new ExcelCelulaParametros(valor.TabelaId, 12.75), ref wsHouse, ref celula, ref linha, ref coluna, ref borda);
                            GravaCelula(new ExcelCelulaParametros(valor.DataInicial, 12.75), ref wsHouse, ref celula, ref linha, ref coluna, ref borda);
                            GravaCelula(new ExcelCelulaParametros(valor.DataFinal, 12.75), ref wsHouse, ref celula, ref linha, ref coluna, ref borda);
                            GravaCelula(new ExcelCelulaParametros(valor.Status, 12.75), ref wsHouse, ref celula, ref linha, ref coluna, ref borda);
                            GravaCelula(new ExcelCelulaParametros(valor.DataPagamento, 12.75), ref wsHouse, ref celula, ref linha, ref coluna, ref borda);
                            GravaCelula(new ExcelCelulaParametros(string.Format("{0:C2}", valor.Faturado), 12.75), ref wsHouse, ref celula, ref linha, ref coluna, ref borda);
                            GravaCelula(new ExcelCelulaParametros(valor.Lotes, 12.75), ref wsHouse, ref celula, ref linha, ref coluna, ref borda);

                            PularLinhaResetaColuna(ref linha, ref coluna);
                        }

                        grupoNivel += 1;
                    }

                    grupoNivel = 1;

                    if (wsPier.Dimension != null)
                    {
                        wsPier.Cells[wsPier.Dimension.Address].AutoFitColumns();
                    }

                    wsPier.Cells[$"A1:I{linha}"].AutoFilter = true;

                    ResetaLinhaColuna(ref linha, ref coluna);

                    var wsGeral = pck.Workbook.Worksheets.Add("Geral");

                    colunas = new[] { "Importador", "Despachante", "Tabela", "Regime", "ID Tabela", "SEQ_GR", "BL", "Tabela Data Início", "Tabela Data Fim", "Tabela Preço Ativa", "Data Pagamento", "Faturado", "Lotes" };

                    for (int i = 0; i <= colunas.Count() - 1; i++)
                        GravaCelula(new ExcelCelulaParametros(colunas[i], true), ref wsGeral, ref celula, ref linha, ref coluna, ref borda);

                    PularLinhaResetaColuna(ref linha, ref coluna);

                    var valoresGerais = _simuladorDAO.ObterValoresGerais(dadosSimulador.SimuladorId, filtro.DataPgtoInicial, filtro.DataPgtoFinal);

                    foreach (var valor in valoresGerais)
                    {
                        GravaCelula(new ExcelCelulaParametros(valor.Importador, 12.75), ref wsGeral, ref celula, ref linha, ref coluna, ref borda);
                        GravaCelula(new ExcelCelulaParametros(valor.Despachante, 12.75), ref wsGeral, ref celula, ref linha, ref coluna, ref borda);
                        GravaCelula(new ExcelCelulaParametros(valor.Descricao, 12.75), ref wsGeral, ref celula, ref linha, ref coluna, ref borda);
                        GravaCelula(new ExcelCelulaParametros(valor.Regime, 12.75), ref wsGeral, ref celula, ref linha, ref coluna, ref borda);
                        GravaCelula(new ExcelCelulaParametros(valor.TabelaId, 12.75), ref wsGeral, ref celula, ref linha, ref coluna, ref borda);
                        GravaCelula(new ExcelCelulaParametros(valor.SeqGr, 12.75), ref wsGeral, ref celula, ref linha, ref coluna, ref borda);
                        GravaCelula(new ExcelCelulaParametros(valor.BL, 12.75), ref wsGeral, ref celula, ref linha, ref coluna, ref borda);
                        GravaCelula(new ExcelCelulaParametros(valor.DataInicial, 12.75), ref wsGeral, ref celula, ref linha, ref coluna, ref borda);
                        GravaCelula(new ExcelCelulaParametros(valor.DataFinal, 12.75), ref wsGeral, ref celula, ref linha, ref coluna, ref borda);
                        GravaCelula(new ExcelCelulaParametros(valor.Status, 12.75), ref wsGeral, ref celula, ref linha, ref coluna, ref borda);
                        GravaCelula(new ExcelCelulaParametros(valor.DataPagamento, 12.75), ref wsGeral, ref celula, ref linha, ref coluna, ref borda);
                        GravaCelula(new ExcelCelulaParametros(string.Format("{0:C2}", valor.Faturado), 12.75), ref wsGeral, ref celula, ref linha, ref coluna, ref borda);
                        GravaCelula(new ExcelCelulaParametros(valor.Lotes, 12.75), ref wsGeral, ref celula, ref linha, ref coluna, ref borda);

                        PularLinhaResetaColuna(ref linha, ref coluna);
                    }

                    if (wsGeral.Dimension != null)
                    {
                        wsGeral.Cells[wsGeral.Dimension.Address].AutoFitColumns();
                    }

                    wsGeral.Cells[$"A1:K{linha}"].AutoFilter = true;

                    ResetaLinhaColuna(ref linha, ref coluna);

                    var wsDemonstrativo = pck.Workbook.Worksheets.Add("Demonstrativo");

                    colunas = new[] { "Tabela", "Valor estimado por lote LCL", "Valor real por lote LCL", "Pier", "Lotes Pier", "Valor real por lote LCL", "House", "Lotes House", "Valor Total (House e Pier)", "Lotes Total (House e Pier)" };

                    ResetaLinhaColuna(ref linha, ref coluna);

                    for (int i = 0; i <= colunas.Count() - 1; i++)
                        GravaCelula(new ExcelCelulaParametros(string.Empty, true), ref wsDemonstrativo, ref celula, ref linha, ref coluna, ref borda);

                    wsDemonstrativo.Cells[linha, 1, linha, colunas.Count()].Merge = true;

                    PularLinhaResetaColuna(ref linha, ref coluna);

                    for (int i = 0; i <= colunas.Count() - 1; i++)
                        GravaCelula(new ExcelCelulaParametros(string.Empty, true), ref wsDemonstrativo, ref celula, ref linha, ref coluna, ref borda);

                    wsDemonstrativo.Cells[linha, 1, linha, colunas.Count()].Merge = true;

                    PularLinhaResetaColuna(ref linha, ref coluna);

                    GravaCelula(new ExcelCelulaParametros("Resumo", true, true, true), ref wsDemonstrativo, ref celula, ref linha, ref coluna, ref borda);
                    wsDemonstrativo.Cells[linha, 1, linha, colunas.Count()].Merge = true;
                    PularLinhaResetaColuna(ref linha, ref coluna);

                    for (int i = 1; i <= 3; i++)
                        GravaCelula(new ExcelCelulaParametros(string.Empty, true, true, true), ref wsDemonstrativo, ref celula, ref linha, ref coluna, ref borda);

                    wsDemonstrativo.Cells[linha, 1, linha, 3].Merge = true;

                    coluna = 4;

                    if (StringHelpers.IsDate(filtro.DataPgtoInicial) && StringHelpers.IsDate(filtro.DataPgtoFinal))
                        GravaCelula(new ExcelCelulaParametros($"Valor pago no período de: {filtro.DataPgtoInicial} até: {filtro.DataPgtoFinal}"), ref wsDemonstrativo, ref celula, ref linha, ref coluna, ref borda);
                    else
                        GravaCelula(new ExcelCelulaParametros(string.Empty), ref wsDemonstrativo, ref celula, ref linha, ref coluna, ref borda);

                    for (int i = 1; i <= 6; i++)
                        GravaCelula(new ExcelCelulaParametros(string.Empty, true, true, true), ref wsDemonstrativo, ref celula, ref linha, ref coluna, ref borda);

                    wsDemonstrativo.Cells[linha, 4, linha, 10].Merge = true;

                    PularLinhaResetaColuna(ref linha, ref coluna);

                    for (int i = 0; i <= colunas.Count() - 1; i++)
                        GravaCelula(new ExcelCelulaParametros(string.Empty, true, true, true), ref wsDemonstrativo, ref celula, ref linha, ref coluna, ref borda);

                    wsDemonstrativo.Cells[linha, 1, linha, colunas.Count()].Merge = true;
                    PularLinhaResetaColuna(ref linha, ref coluna);

                    for (int i = 0; i <= colunas.Count() - 1; i++)
                        GravaCelula(new ExcelCelulaParametros(colunas[i], true, true, false), ref wsDemonstrativo, ref celula, ref linha, ref coluna, ref borda);

                    PularLinhaResetaColuna(ref linha, ref coluna);

                    var totalizacao = new ValoresGeraisTotais();

                    foreach (var tabela in tabelas)
                    {
                        GravaCelula(new ExcelCelulaParametros(tabela.Descricao), ref wsDemonstrativo, ref celula, ref linha, ref coluna, ref borda);

                        totalizacao.TotalValorEstimadoPorLoteLCLPier = _simuladorDAO.ObterTotalValorEstimadoPorLoteLCLPier(dadosSimulador.SimuladorId, tabela.TabelaId);
                        GravaCelula(new ExcelCelulaParametros(string.Format("{0:C2}", totalizacao.TotalValorEstimadoPorLoteLCLPier), true, 12.75), ref wsDemonstrativo, ref celula, ref linha, ref coluna, ref borda);

                        var valoresPier = _simuladorDAO.ObterResumoPierHouse(dadosSimulador.SimuladorId, "PP", filtro.DataPgtoInicial, filtro.DataPgtoFinal, tabela.TabelaId);

                        foreach (var pier in valoresPier)
                        {
                            totalizacao.TotalPier += pier.Faturado;
                            totalizacao.TotalLotesPier += pier.Lotes;
                        }

                        if (totalizacao.TotalLotesPier > 0)
                        {
                            totalizacao.TotalValorRealPorLoteLCLPier = totalizacao.TotalPier / totalizacao.TotalLotesPier;
                        }

                        GravaCelula(new ExcelCelulaParametros(string.Format("{0:C2}", totalizacao.TotalValorRealPorLoteLCLPier), true, 12.75), ref wsDemonstrativo, ref celula, ref linha, ref coluna, ref borda);
                        GravaCelula(new ExcelCelulaParametros(string.Format("{0:C2}", totalizacao.TotalPier), true, 12.75), ref wsDemonstrativo, ref celula, ref linha, ref coluna, ref borda);
                        GravaCelula(new ExcelCelulaParametros(totalizacao.TotalLotesPier.ToString(), true, 12.75), ref wsDemonstrativo, ref celula, ref linha, ref coluna, ref borda);

                        var valoresHouse = _simuladorDAO.ObterResumoPierHouse(dadosSimulador.SimuladorId, "HH", filtro.DataPgtoInicial, filtro.DataPgtoFinal, tabela.TabelaId);

                        foreach (var house in valoresHouse)
                        {
                            totalizacao.TotalHouse += house.Faturado;
                            totalizacao.TotalLotesHouse += house.Lotes;
                        }

                        if (totalizacao.TotalLotesHouse > 0)
                        {
                            totalizacao.TotalValorRealPorLoteLCLHouse = totalizacao.TotalHouse / totalizacao.TotalLotesHouse;
                        }

                        GravaCelula(new ExcelCelulaParametros(string.Format("{0:C2}", totalizacao.TotalValorRealPorLoteLCLHouse), true, 12.75), ref wsDemonstrativo, ref celula, ref linha, ref coluna, ref borda);
                        GravaCelula(new ExcelCelulaParametros(string.Format("{0:C2}", totalizacao.TotalHouse), true, 12.75), ref wsDemonstrativo, ref celula, ref linha, ref coluna, ref borda);
                        GravaCelula(new ExcelCelulaParametros(totalizacao.TotalLotesHouse.ToString(), true, 12.75), ref wsDemonstrativo, ref celula, ref linha, ref coluna, ref borda);

                        totalizacao.TotalValorPierHouse = totalizacao.TotalPier + totalizacao.TotalHouse;
                        totalizacao.TotalLotesPierHouse = totalizacao.TotalLotesPier + totalizacao.TotalLotesHouse;

                        GravaCelula(new ExcelCelulaParametros(string.Format("{0:C2}", totalizacao.TotalValorPierHouse), true, 12.75), ref wsDemonstrativo, ref celula, ref linha, ref coluna, ref borda);
                        GravaCelula(new ExcelCelulaParametros(totalizacao.TotalLotesPierHouse.ToString(), true, 12.75), ref wsDemonstrativo, ref celula, ref linha, ref coluna, ref borda);

                        PularLinhaResetaColuna(ref linha, ref coluna);

                        totalizacao.SomaValorEstimadoPorLoteLCLPier += totalizacao.TotalValorEstimadoPorLoteLCLPier;
                        totalizacao.SomaValorRealPorLoteLCLPier += totalizacao.TotalValorRealPorLoteLCLPier;
                        totalizacao.SomaPier += totalizacao.TotalPier;
                        totalizacao.SomaLotesPier += totalizacao.TotalLotesPier;
                        totalizacao.SomaValorRealPorLoteLCLHouse += totalizacao.TotalValorRealPorLoteLCLHouse;
                        totalizacao.SomaHouse += totalizacao.TotalHouse;
                        totalizacao.SomaLotesHouse += totalizacao.TotalLotesHouse;
                        totalizacao.SomaValorPierHouse += totalizacao.TotalValorPierHouse;
                        totalizacao.SomaLotesPierHouse += totalizacao.TotalLotesPierHouse;

                        totalizacao.SubTotalValorEstimadoPorLoteLCLPier += totalizacao.TotalValorEstimadoPorLoteLCLPier;
                        totalizacao.SubTotalValorRealPorLoteLCLPier += totalizacao.TotalValorRealPorLoteLCLPier;
                        totalizacao.SubTotalPier += totalizacao.TotalPier;
                        totalizacao.SubTotalLotesPier += totalizacao.TotalLotesPier;
                        totalizacao.SubTotalValorRealPorLoteLCLHouse += totalizacao.TotalValorRealPorLoteLCLHouse;
                        totalizacao.SubTotalHouse += totalizacao.TotalHouse;
                        totalizacao.SubTotalLotesHouse += totalizacao.TotalLotesHouse;
                        totalizacao.SubTotalValorPierHouse += totalizacao.TotalValorPierHouse;
                        totalizacao.SubTotalLotesPierHouse += totalizacao.TotalLotesPierHouse;

                        totalizacao.TotalValorEstimadoPorLoteLCLPier = 0;
                        totalizacao.TotalValorRealPorLoteLCLPier = 0;
                        totalizacao.TotalPier = 0;
                        totalizacao.TotalLotesPier = 0;
                        totalizacao.TotalValorRealPorLoteLCLHouse = 0;
                        totalizacao.TotalHouse = 0;
                        totalizacao.TotalLotesHouse = 0;
                        totalizacao.TotalValorPierHouse = 0;
                        totalizacao.TotalLotesPierHouse = 0;
                    }

                    GravaCelula(new ExcelCelulaParametros("Total: ", true, true, 12.75), ref wsDemonstrativo, ref celula, ref linha, ref coluna, ref borda);

                    GravaCelula(new ExcelCelulaParametros(string.Format("{0:C2}", Math.Round(totalizacao.SomaValorEstimadoPorLoteLCLPier, 2)), true, 12.75), ref wsDemonstrativo, ref celula, ref linha, ref coluna, ref borda);
                    GravaCelula(new ExcelCelulaParametros(string.Format("{0:C2}", Math.Round(totalizacao.SomaValorRealPorLoteLCLPier, 2)), true, 12.75), ref wsDemonstrativo, ref celula, ref linha, ref coluna, ref borda);
                    GravaCelula(new ExcelCelulaParametros(string.Format("{0:C2}", Math.Round(totalizacao.SomaPier, 2)), true, 12.75), ref wsDemonstrativo, ref celula, ref linha, ref coluna, ref borda);
                    GravaCelula(new ExcelCelulaParametros(totalizacao.SomaLotesPier.ToString(), true, 12.75), ref wsDemonstrativo, ref celula, ref linha, ref coluna, ref borda);
                    GravaCelula(new ExcelCelulaParametros(string.Format("{0:C2}", Math.Round(totalizacao.SomaValorRealPorLoteLCLHouse, 2)), true, 12.75), ref wsDemonstrativo, ref celula, ref linha, ref coluna, ref borda);
                    GravaCelula(new ExcelCelulaParametros(string.Format("{0:C2}", Math.Round(totalizacao.SomaHouse, 2)), true, 12.75), ref wsDemonstrativo, ref celula, ref linha, ref coluna, ref borda);
                    GravaCelula(new ExcelCelulaParametros(totalizacao.SomaLotesHouse.ToString(), true, 12.75), ref wsDemonstrativo, ref celula, ref linha, ref coluna, ref borda);
                    GravaCelula(new ExcelCelulaParametros(string.Format("{0:C2}", Math.Round(totalizacao.SomaValorPierHouse, 2)), true, 12.75), ref wsDemonstrativo, ref celula, ref linha, ref coluna, ref borda);
                    GravaCelula(new ExcelCelulaParametros(totalizacao.SomaLotesPierHouse.ToString(), true, 12.75), ref wsDemonstrativo, ref celula, ref linha, ref coluna, ref borda);

                    PularLinhaResetaColuna(ref linha, ref coluna);

                    GravaCelula(new ExcelCelulaParametros("Média Geral: ", true, true, 12.75), ref wsDemonstrativo, ref celula, ref linha, ref coluna, ref borda);
                    GravaCelula(new ExcelCelulaParametros(string.Empty, true, true, 12.75), ref wsDemonstrativo, ref celula, ref linha, ref coluna, ref borda);

                    GravaCelula(new ExcelCelulaParametros(string.Format("{0:C2}", Math.Round(totalizacao.SubTotalPier + totalizacao.SomaPier, 2) / ((Math.Round(totalizacao.SubTotalLotesPier + totalizacao.SomaLotesPier, 2)) > 0 ? (Math.Round(totalizacao.SubTotalLotesPier + totalizacao.SomaLotesPier, 2)) : 1)), true, 12.75), ref wsGeral, ref celula, ref linha, ref coluna, ref borda);
                    GravaCelula(new ExcelCelulaParametros(string.Format("{0:C2}", Math.Round(totalizacao.SubTotalPier + totalizacao.SomaPier, 2)), true, 12.75), ref wsDemonstrativo, ref celula, ref linha, ref coluna, ref borda);
                    GravaCelula(new ExcelCelulaParametros(string.Format("{0:N0}", Math.Round(totalizacao.SubTotalLotesPier + totalizacao.SomaLotesPier, 2)), true, 12.75), ref wsDemonstrativo, ref celula, ref linha, ref coluna, ref borda);
                    GravaCelula(new ExcelCelulaParametros(string.Empty, true, true, 12.75), ref wsDemonstrativo, ref celula, ref linha, ref coluna, ref borda);
                    GravaCelula(new ExcelCelulaParametros(string.Format("{0:C2}", Math.Round(totalizacao.SubTotalHouse + totalizacao.SomaHouse, 2)), true, 12.75), ref wsDemonstrativo, ref celula, ref linha, ref coluna, ref borda);
                    GravaCelula(new ExcelCelulaParametros(string.Format("{0:C2}", Math.Round(totalizacao.SubTotalLotesHouse + totalizacao.SomaLotesHouse, 2)), true, 12.75), ref wsDemonstrativo, ref celula, ref linha, ref coluna, ref borda);
                    GravaCelula(new ExcelCelulaParametros(string.Empty, true, true, 12.75), ref wsDemonstrativo, ref celula, ref linha, ref coluna, ref borda);
                    GravaCelula(new ExcelCelulaParametros(string.Empty, true, true, 12.75), ref wsDemonstrativo, ref celula, ref linha, ref coluna, ref borda);

                    if (wsDemonstrativo.Dimension != null)
                    {
                        wsDemonstrativo.Cells[wsDemonstrativo.Dimension.Address].AutoFitColumns();
                    }

                    wsDemonstrativo.Cells[$"A6:J{linha - 1}"].AutoFilter = true;
                }

                var nomeArquivo = $@"Simulador_{DateTime.Now.ToString("ddMMyyyyHHmmss")}";

                int tamanhoArquivo = 0;

                var dados = pck.GetAsByteArray();

                var arquivoId = 0;

                if (Configuracoes.BancoEmUso() == "ORACLE")
                {
                    arquivoId = IncluirAnexo(dados, filtro.SimuladorId, nomeArquivo, "xlsx", dadosSimulador.CriadoPor, ref tamanhoArquivo);
                }                     

                return new Response
                {
                    Sucesso = true,
                    Mensagem = "Relatório gerado com sucesso",
                    ArquivoId = arquivoId,
                    Base64 = Convert.ToBase64String(dados),
                    TamanhoArquivo = tamanhoArquivo
                };
            }
        }
        public Response GerarColuna(GerarExcelFiltro filtro)
        {
            ExcelRange celula = null;
            Border borda = null;
            ExcelWorksheet excelWorksheet = null;

            var codigoServicoLista = new List<int>();

            int linha = 1;
            int coluna = 1;
            int totalColunas = 3;
            string tipoCliente = string.Empty;

            using (var pck = new ExcelPackage())
            {
                pck.Workbook.Properties.Author = "Ecoporto";
                pck.Workbook.Properties.Title = "Business Analysis";

                excelWorksheet = pck.Workbook.Worksheets.Add("Tabelas");

                var dadosSimulador = _simuladorDAO.ObterDetalhesSimuladorPorId(filtro.SimuladorId);

                if (dadosSimulador == null)
                    throw new Exception("Simulador não encontrado");

                var ehConteiner = (dadosSimulador.Regime == "FCL");

                var tabelas = _simuladorDAO.ObterTabelasCalculadas(dadosSimulador.SimuladorId);

                GravaCelulaCol(new ExcelCelulaParametros("ID", true, true), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);

                foreach (var tabela in tabelas)
                {
                    GravaCelulaCol(new ExcelCelulaParametros(tabela.Descricao, true, true), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);
                }


                //excelWorksheet.Cells[linha, 2, linha, totalColunas].Merge = true;

                //excelWorksheet.Cells[linha, 2, linha, totalColunas].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                //excelWorksheet.Cells[linha, 2, linha, totalColunas].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                //excelWorksheet.Cells[linha, 2, linha, totalColunas].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                //excelWorksheet.Cells[linha, 2, linha, totalColunas].Style.Border.Right.Style = ExcelBorderStyle.Thin;


                PularColunaResetaLinha(ref linha, ref coluna);

                switch (dadosSimulador.Classe)
                {
                    case 0:
                        tipoCliente = "CLIENTE ESPECÍFICO";
                        break;
                    case 1:
                        tipoCliente = "IMPORTADOR";
                        break;
                    case 2:
                        tipoCliente = "DESPACHANTE";
                        break;
                    case 3:
                        tipoCliente = "NVOCC";
                        break;
                    case 4:
                        tipoCliente = "COLOADER";
                        break;
                }

                GravaCelulaCol(new ExcelCelulaParametros(tipoCliente, true, true, 38.25), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);

                foreach (var tabela in tabelas)
                {
                    var razaoSocial = _parceiroDAO.ObterRazaoSocial(filtro.DadosDoCliente, dadosSimulador.Classe, tabela.TabelaId);

                    if (razaoSocial != null)
                    {
                        GravaCelulaCol(new ExcelCelulaParametros(razaoSocial.RazaoSocial, true, true), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);
                    }
                }

         
                //excelWorksheet.Cells[linha, 2, linha, totalColunas].Merge = true;

                //excelWorksheet.Cells[linha, 2, linha, totalColunas].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                //excelWorksheet.Cells[linha, 2, linha, totalColunas].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                //excelWorksheet.Cells[linha, 2, linha, totalColunas].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                //excelWorksheet.Cells[linha, 2, linha, totalColunas].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                if (filtro.SomenteEstimativa == false)
                {
                    PularColunaResetaLinha(ref linha, ref coluna);

                    GravaCelulaCol(new ExcelCelulaParametros($"ARMAZENAGEM DIAS {dadosSimulador.Regime}", true, false, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);

                    foreach (var tabela in tabelas)
                    {
                        var diasArmazenagem = _servicoDAO.ObterQuantidadeDias(tabela.TabelaId, ehConteiner);

                        GravaCelulaCol(new ExcelCelulaParametros(diasArmazenagem, true, true), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);
                    }

                    PularColunaResetaLinha(ref linha, ref coluna);

                    for (int periodo = 1; periodo <= 4; periodo++)
                    {
                        GravaCelulaCol(new ExcelCelulaParametros($"{periodo}º Período ARMAZENAGEM ", false, false, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);
                        foreach (var tabela in tabelas)
                        {
                            var valoresArmazenagem = _servicoDAO.ObterValoresServicoArmazenagem(tabela.TabelaId, periodo, ehConteiner);

                            if (valoresArmazenagem != null)
                            {
                                if (valoresArmazenagem.BaseCalculo == "CIF" || valoresArmazenagem.BaseCalculo == "CIF0" || valoresArmazenagem.BaseCalculo == "CIFM")
                                {
                                    GravaCelulaCol(new ExcelCelulaParametros(String.Format("{0:P2}", valoresArmazenagem.PrecoUnitario), false, true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);
                                }
                                else
                                {
                                    GravaCelulaCol(new ExcelCelulaParametros(String.Format("{0:C2}", valoresArmazenagem.PrecoUnitario), false, true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);
                                }

                                if (periodo == 1)
                                    codigoServicoLista.Add(valoresArmazenagem.ServicoFixoVariavelId);
                            }
                            else
                            {
                                GravaCelulaCol(new ExcelCelulaParametros(string.Empty, false, true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);
                                if (periodo == 1)
                                    codigoServicoLista.Add(0);
                            }
                        }

                        PularColunaResetaLinha(ref linha, ref coluna);
                    }

                    linha = 1;

                    GravaCelulaCol(new ExcelCelulaParametros("Períodos Subsequentes", false, false, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);

 
                    foreach (var tabela in tabelas)
                    {
                        for (int periodo = dadosSimulador.Periodos + 4; periodo >= 1; periodo += -1)
                        {
                            var valoresArmazenagem = _servicoDAO.ObterValoresServicoArmazenagem(tabela.TabelaId, periodo, ehConteiner);

                            if (valoresArmazenagem != null)
                            {
                                if (valoresArmazenagem.BaseCalculo == "CIF" || valoresArmazenagem.BaseCalculo == "CIF0" || valoresArmazenagem.BaseCalculo == "CIFM")
                                {
                                    GravaCelulaCol(new ExcelCelulaParametros(String.Format("{0:P2}", valoresArmazenagem.PrecoUnitario), false, true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);
                                }
                                else
                                {
                                    GravaCelulaCol(new ExcelCelulaParametros(String.Format("{0:C2}", valoresArmazenagem.PrecoUnitario), false, true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);
                                }

                                break;
                            }
                        }
                    }

                    PularColunaResetaLinha(ref linha, ref coluna);

                    GravaCelulaCol(new ExcelCelulaParametros("Valor Mínimo de Cobrança", false, false, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);

                    foreach (var codigoServico in codigoServicoLista)
                    {
                        var precoMinimo = _servicoDAO.ObterPrecoMinimoServicoVariavelPorId(codigoServico, ehConteiner);

                        if (precoMinimo != null)
                        {
                            if (precoMinimo.Value > 0)
                            {
                                precoMinimo = precoMinimo.Value;
                            }

                            ServicoFixoVariavel minimoPorFaixa = null;

                            if (dadosSimulador.Regime == "FCL")
                                minimoPorFaixa = _servicoDAO.ObterDetalhesServicoVariavelPorFaixas(codigoServico, dadosSimulador.ValorCifConteiner, "C");
                            else
                                minimoPorFaixa = _servicoDAO.ObterDetalhesServicoVariavelPorFaixas(codigoServico, dadosSimulador.ValorCifCargaSolta, "C");

                            if (minimoPorFaixa != null)
                            {
                                precoMinimo = minimoPorFaixa.ValorMinimo;
                            }

                            if (dadosSimulador.Regime == "LCL")
                            {
                                dadosSimulador.NumeroLotes = dadosSimulador.NumeroLotes == 0 ? 1 : dadosSimulador.NumeroLotes;

                                minimoPorFaixa = _servicoDAO.ObterDetalhesServicoVariavelPorFaixasPeso(codigoServico, dadosSimulador.NumeroLotes, "SVAR40");

                                if (minimoPorFaixa != null)
                                {
                                    precoMinimo = minimoPorFaixa.ValorMinimo;
                                }
                                else
                                {
                                    minimoPorFaixa = _servicoDAO.ObterDetalhesServicoVariavelPorFaixasPeso(codigoServico, dadosSimulador.NumeroLotes, "SVAR20");

                                    if (minimoPorFaixa != null)
                                    {
                                        precoMinimo = minimoPorFaixa.ValorMinimo;
                                    }
                                    else
                                    {
                                        minimoPorFaixa = _servicoDAO.ObterDetalhesServicoVariavelPorFaixasPeso(codigoServico, dadosSimulador.NumeroLotes, "SVAR");

                                        if (minimoPorFaixa != null)
                                        {
                                            precoMinimo = minimoPorFaixa.ValorMinimo;
                                        }
                                    }
                                }
                            }
                        }

                        GravaCelulaCol(new ExcelCelulaParametros(string.Format("{0:C2}", precoMinimo), true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);
                    }

                    //PularColunaResetaLinha(ref linha, ref coluna);

                    bool cab = false;

                    foreach (var codigoServico in codigoServicoLista)
                    {
                        var minimoPorFaixa = _servicoDAO.ObterDetalhesServicoVariavelPorFaixasPeso(codigoServico, 3, "SVAR40");

                        if (minimoPorFaixa == null)
                        {
                            minimoPorFaixa = _servicoDAO.ObterDetalhesServicoVariavelPorFaixasPeso(codigoServico, 3, "SVAR20");
                        }

                        if (minimoPorFaixa != null)
                        {
                            if (minimoPorFaixa.ValorMinimo > 0)
                            {
                                if (cab == false)
                                {
                                    GravaCelulaCol(new ExcelCelulaParametros("Acima de 02 B/L p/ CC 20' ou 40' e/ou por lote LCL", 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);
                                    cab = true;
                                }

                                GravaCelulaCol(new ExcelCelulaParametros(string.Format("{0:C2}", minimoPorFaixa.ValorMinimo), true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);
                            }
                        }
                    }

                    if (cab)
                    {
                        PularColunaResetaLinha(ref linha, ref coluna);
                    }

                    cab = false;

                    foreach (var codigoServico in codigoServicoLista)
                    {
                        var minimoPorFaixa = _servicoDAO.ObterDetalhesServicoVariavelPorFaixasPeso(codigoServico, 3, "SVAR20");

                        if (minimoPorFaixa == null && dadosSimulador.Regime == "FCL")
                        {
                            minimoPorFaixa = _servicoDAO.ObterDetalhesServicoVariavelPorFaixasPeso(codigoServico, 3, "SVAR20", 52, "CPIER");
                        }

                        if (minimoPorFaixa != null)
                        {
                            if (minimoPorFaixa.ValorMinimo > 0)
                            {
                                if (cab == false)
                                {
                                    GravaCelulaCol(new ExcelCelulaParametros("Até 02 B/L por CC 20' (por lote)", 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);
                                    cab = true;
                                }

                                GravaCelulaCol(new ExcelCelulaParametros(string.Format("{0:C2}", minimoPorFaixa.ValorMinimo), true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);
                            }
                        }
                    }

                    if (cab)
                    {
                        PularColunaResetaLinha(ref linha, ref coluna);
                    }

                    cab = false;

                    foreach (var codigoServico in codigoServicoLista)
                    {
                        var minimoPorFaixa = _servicoDAO.ObterDetalhesServicoVariavelPorFaixasPeso(codigoServico, 3, "SVAR40");

                        if (minimoPorFaixa == null && dadosSimulador.Regime == "FCL")
                        {
                            minimoPorFaixa = _servicoDAO.ObterDetalhesServicoVariavelPorFaixasPeso(codigoServico, 3, "SVAR40", 52, "CPIER");
                        }

                        if (minimoPorFaixa != null)
                        {
                            if (minimoPorFaixa.ValorMinimo > 0)
                            {
                                if (cab == false)
                                {
                                    GravaCelulaCol(new ExcelCelulaParametros("Até 02 B/L por CC 40' (por lote)", 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);
                                    cab = true;
                                }

                                GravaCelulaCol(new ExcelCelulaParametros(string.Format("{0:C2}", minimoPorFaixa.ValorMinimo), true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);
                             }
                        }
                    }

                    if (cab)
                    {
                        PularColunaResetaLinha(ref linha, ref coluna);
                    }

                    cab = false;

                    if (filtro.ServicosComplementares)
                    {
                        //for (int i = 1; i <= totalColunas; i++)
                        //{
                        //    PintarLinha(ref excelWorksheet, ref celula, ref linha, i);
                        //}

                        PularColunaResetaLinha(ref linha, ref coluna);

                        var servicosSimuladorSemArmazenagem = _simuladorDAO.ObterServicosSemArmazenagemSimulador(dadosSimulador.SimuladorId, dadosSimulador.Regime);

                        foreach (var servico in servicosSimuladorSemArmazenagem)
                        {
                            GravaCelulaCol(new ExcelCelulaParametros(servico.Descricao, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);

                            foreach (var tabela in tabelas)
                            {
                                var precoUnitario = _servicoDAO.ObterPrecoUnitarioPorTabelaEServico(tabela.TabelaId, servico.ServicoId);

                                GravaCelulaCol(new ExcelCelulaParametros(string.Format("{0:C2}", precoUnitario), true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);
                            }

                            PularColunaResetaLinha(ref linha, ref coluna);
                        }
                    }
                }

              

                var filtroSimulador = dadosSimulador.ObterFiltro();

         
                //Merge(ref excelWorksheet, ref linha, 2);

          
                //for (int i = 1; i <= totalColunas; i++)
                //{
                //    PintarLinha(ref excelWorksheet, ref celula, ref linha, i);
                //}

                PularColunaResetaLinha(ref linha, ref coluna);

                var servicosSimulador = _simuladorDAO.ObterServicosSimulador(dadosSimulador.SimuladorId);

                foreach (var servico in servicosSimulador)
                {
                    GravaCelulaCol(new ExcelCelulaParametros(servico.Descricao, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);

                    foreach (var tabela in tabelas)
                    {
                        var valor = _servicoDAO.ObterValoresServicoSimuladorPorTabelaId(dadosSimulador.SimuladorId, tabela.TabelaId, servico.ServicoId, string.Empty);

                        GravaCelulaCol(new ExcelCelulaParametros(string.Format("{0:C2}", valor.ValorFinal), true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);
                     }

                    PularColunaResetaLinha(ref linha, ref coluna);
                }

                GravaCelulaCol(new ExcelCelulaParametros("IMPOSTOS", 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);

                var taxaImposto = _impostoDAO.ObterTaxaImposto();

                taxaImposto = 1 - taxaImposto;

                  
                
                

                foreach (var tabela in tabelas)
                {
                    var valorImposto = _simuladorDAO.ObterValorImposto(taxaImposto, dadosSimulador.SimuladorId, tabela.TabelaId, "MDIR");

                    GravaCelulaCol(new ExcelCelulaParametros(string.Format("{0:C2}", valorImposto), true, 12.75), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);
                 }
 
                PularColunaResetaLinha(ref linha, ref coluna);

                GravaCelulaCol(new ExcelCelulaParametros("Total:", true, 12.75, true), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);

                foreach (var tabela in tabelas)
                {
                    var valorComImposto = _simuladorDAO.ObterValorComImposto(taxaImposto, dadosSimulador.SimuladorId, tabela.TabelaId, "MDIR");

                    GravaCelulaCol(new ExcelCelulaParametros(string.Format("{0:C2}", valorComImposto), true, 12.75, true), ref excelWorksheet, ref celula, ref linha, ref coluna, ref borda);
                 }

                 AjustarLarguraColuna(ref excelWorksheet, coluna: 1, larguraEmPixels: 50);

                for (int i = 2; i < totalColunas; i++)
                {
                    AjustarLarguraColuna(ref excelWorksheet, coluna: i, larguraEmPixels: 26);
                }

                excelWorksheet.Cells[1, totalColunas].Style.WrapText = true;

                excelWorksheet.Row(1).Height = 200;               

                excelWorksheet.Cells[1, 1, 1, excelWorksheet.Dimension.Columns].Style.TextRotation = 90;

                if (excelWorksheet.Dimension != null)
                {
                    excelWorksheet.Cells[excelWorksheet.Dimension.Address].AutoFitColumns();
                }

                var nomeArquivo = $@"Simulador_{DateTime.Now.ToString("ddMMyyyyHHmmss")}";

                int tamanhoArquivo = 0;

                var dados = pck.GetAsByteArray();

                var arquivoId = 0;

                if (Configuracoes.BancoEmUso() == "ORACLE")
                {
                    arquivoId = IncluirAnexo(dados, filtro.SimuladorId, nomeArquivo, "xlsx", dadosSimulador.CriadoPor, ref tamanhoArquivo);
                }

                return new Response
                {
                    Sucesso = true,
                    Mensagem = "Relatório gerado com sucesso",
                    ArquivoId = arquivoId,
                    Base64 = Convert.ToBase64String(dados),
                    TamanhoArquivo = tamanhoArquivo
                };
            }
        }


        private int IncluirAnexo(byte[] bytes, int simuladorId, string nomeArquivo, string extensaoArquivo, int usuarioId, ref int tamanho)
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

                var anexoInclusaoId = _anexosDAO.IncluirAnexo(
                    new Anexo
                    {
                        IdProcesso = simuladorId,
                        Arquivo = dados.Name,
                        CriadoPor = usuarioId,
                        TipoAnexo = 7,
                        TipoDoc = 3,
                        IdArquivo = GuidHelpers.GuidToRaw(retornoUpload.Arquivo.id)
                    });

                tamanho = bytes.Length;

                return anexoInclusaoId;
            }

            return 0;
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
        private void PularColunaResetaLinha(ref int linha, ref int coluna)
        {
            linha = 1;
            coluna += 1;
        }

        private void ResetaLinhaColuna(ref int linha, ref int coluna)
        {
            linha = 1;
            coluna = 1;
        }

        private void GravaCelula(ExcelCelulaParametros parametros, ref ExcelWorksheet ws, ref ExcelRange celula, ref int linha, ref int coluna, ref Border borda, TipoCelulaExcel tipoCelulaExcel = TipoCelulaExcel.Texto)
        {
            coluna = coluna == 0 ? 1 : coluna;

            celula = ws.Cells[linha, coluna];

            if (tipoCelulaExcel == TipoCelulaExcel.Percentual)
            {
                celula.Style.Numberformat.Format = "#0.00%";

                if (string.IsNullOrEmpty(parametros.Valor))
                    celula.Value = 0;
                else
                    celula.Value = Convert.ToDecimal(parametros.Valor);
            }

            if (tipoCelulaExcel == TipoCelulaExcel.Numero)
            {
                celula.Style.Numberformat.Format = "#,##0.00";

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
                celula.Style.Numberformat.Format = "R$#,##0.00";

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
        private void GravaCelulaCol(ExcelCelulaParametros parametros, ref ExcelWorksheet ws, ref ExcelRange celula, ref int linha, ref int coluna, ref Border borda)
        {
            linha = linha == 0 ? 1 : linha;

            celula = ws.Cells[linha, coluna];

            celula.Value = parametros.Valor;

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

            linha += 1;
        }
    }
}