using Ecoporto.CRM.Business.DTO;
using Ecoporto.CRM.Business.Enums;
using Ecoporto.CRM.Business.Extensions;
using Ecoporto.CRM.Business.Helpers;
using Ecoporto.CRM.Business.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Ecoporto.CRM.Site.Services
{
    public class LayoutService
    {
        private readonly ILayoutBase _layoutRepositorio;
        private readonly IModeloRepositorio _modeloRepositorio;
        private readonly IOportunidadeRepositorio _oportunidadeRepositorio;

        #region Estilos

        private const string COLUNA = "border:1px solid black;";
        private const string TABELA_PREVIEW = "border-collapse: collapse; margin-top: 10px; font-family: Verdana; font-size:9pt; width: 100%; margin-bottom: 10px;";
        private const string TABELA_CONDICOES = "margin-top: 10px; font-family: Verdana; width: 400px; margin-bottom: 10px;";
        private const string COLUNA_VALOR = COLUNA + "text-align: center; width: 105px;";
        private const string VALOR_UNICO = COLUNA + "text-align: center;";
        private const string TITULO_PREVIEW = COLUNA + "color: #005636; font-weight: bold; background-color: #EAF3DE;";
        private const string SEM_BORDA = "padding-left: 6px;border-left: 0px; border-right: 0px; border-bottom: 0px; background-color: #FFFFFF;";
        private const string CONDICOES = "padding-left: 6px;border-bottom: 0px; background-color: #FFFFFF;";
        private const string PARAGRAFO = "padding: 0; margin-top: 15px; margin-bottom: 15px; font-weight: bold; color: #005636;";
        private const string QUEBRA_PAGINA = "page-break-before: always;";
        private const string JUSTIFICADO = "text-align: justify;";
        private const string SEM_MARGENS = "margin:0pt;padding:0pt;";

        #endregion

        public LayoutService(
            ILayoutBase layoutRepositorio,
            IModeloRepositorio modeloRepositorio,
            IOportunidadeRepositorio oportunidadeRepositorio)
        {
            _layoutRepositorio = layoutRepositorio;
            _modeloRepositorio = modeloRepositorio;
            _oportunidadeRepositorio = oportunidadeRepositorio;
        }

        public string ObterCondicoesIniciais(IEnumerable<LayoutDTO> linhas, int? oportunidadeId)
        {
            var registro = linhas.Where(c => c.TipoRegistro == TipoRegistro.CONDICAO_INICIAL).FirstOrDefault();

            if (registro != null)
            {
                var linha = FormatarCamposCondicoesIniciais(registro, oportunidadeId.Value);

                var html = new HtmlBuilder();

                if (HttpContext.Current.User.IsInRole("Administrador") || HttpContext.Current.User.IsInRole("OportunidadesProposta:PropostaCondicoesIniciais"))
                {
                    html = html.AdicionaParagrafoComId("LinhaCondicoesIniciais", SEM_MARGENS, linha, registro.Id);
                }
                else
                {
                    html = html.AdicionaDiv(linha, SEM_MARGENS);
                }

                return html.Compilar().ToString();
            }

            return string.Empty;
        }

        public string MontarLayout(List<LayoutDTO> linhas, int? oportunidadeId)
        {
            var tabela = new StringBuilder();
            var quebra = false;
            var tipo = TipoRegistro.TITULO;
            var tipoAnterior = TipoRegistro.TITULO;
            var tipoCarga = TipoCarga.CONTEINER;
            var tipoMoeda = Moeda.REAL;
            var tipoRegistroAnterior = TipoRegistro.TITULO;

            if (linhas.Count() == 0)
                return string.Empty;

            var html = new HtmlBuilder()
                .AbreTabela(0, TABELA_PREVIEW);

            var contador = 0;

            foreach (var registro in linhas)
            {
                if (Enum.IsDefined(typeof(TipoCarga), registro.TipoCarga))
                    tipoCarga = registro.TipoCarga;

                if (Enum.IsDefined(typeof(Moeda), registro.Moeda))
                    tipoMoeda = registro.Moeda;

                tipo = registro.TipoRegistro;

                if (tipo != TipoRegistro.TITULO)
                    quebra = true;

                switch (tipo)
                {
                    case TipoRegistro.CONDICAO_INICIAL:

                        if (HttpContext.Current.User.IsInRole("Administrador") || HttpContext.Current.User.IsInRole("OportunidadesProposta:PropostaCondicoesIniciais"))
                        {
                            if (oportunidadeId.HasValue)
                            {
                                registro.CondicoesIniciais = FormatarCamposCondicoesIniciais(registro, oportunidadeId.Value);
                            }

                            html = html
                                .AbreLinha(CONDICOES, registro.Id)
                                .CriarColuna(5, CONDICOES, registro.CondicoesIniciais)
                            .FechaLinha();
                        }

                        break;

                    case TipoRegistro.TITULO_MASTER:

                        if (quebra == true)
                        {
                            html = html
                                .AbreLinha(registro.Id)
                                    .CriarColuna(5, SEM_BORDA, "&nbsp;", "sem-borda")
                                .FechaLinha();
                            quebra = false;
                        }

                        html = html
                            .AbreLinha(SEM_BORDA + QUEBRA_PAGINA, registro.Id, "sem-borda")
                                .CriarColuna(5, SEM_BORDA, $"<p style='{PARAGRAFO}'>&diams;&nbsp;{ registro.Descricao }</p>", "sem-borda")
                            .FechaLinha();

                        break;

                    case TipoRegistro.TITULO:

                        if (quebra == true)
                        {
                            html = html
                                .AbreLinha(registro.Id)
                                    .CriarColuna(5, SEM_BORDA, "&nbsp;", "sem-borda")
                                .FechaLinha();
                            quebra = false;
                        }

                        html = html
                            .AbreLinha(registro.Id)
                                .CriarColuna(5, TITULO_PREVIEW, registro.Descricao)
                            .FechaLinha();

                        break;

                    case TipoRegistro.SUB_TITULO:

                        if (tipoAnterior != TipoRegistro.TITULO)
                        {
                            html = html
                                .AbreLinha(registro.Id)
                                    .CriarColuna(5, SEM_BORDA, "&nbsp;", "sem-borda")
                                .FechaLinha();
                            quebra = false;
                        }

                        html = html
                            .AbreLinha(TITULO_PREVIEW, registro.Id)
                                .CriarColuna(3, COLUNA, registro.Descricao)
                                .CriarColuna(COLUNA_VALOR, "20'")
                                .CriarColuna(COLUNA_VALOR, "40'")
                            .FechaLinha();

                        break;

                    case TipoRegistro.SUB_TITULO_MARGEM:

                        if (tipoAnterior != TipoRegistro.TITULO)
                        {
                            html = html
                                .AbreLinha(registro.Id)
                                    .CriarColuna(4, SEM_BORDA, "&nbsp;", "sem-borda")
                                .FechaLinha();
                            quebra = false;
                        }

                        html = html
                            .AbreLinha(TITULO_PREVIEW, registro.Id)
                                .CriarColuna(2, COLUNA, registro.Descricao)
                                .CriarColuna(COLUNA_VALOR, "M Direita")
                                .CriarColuna(COLUNA_VALOR, "M Esquerda")
                                .CriarColuna(COLUNA_VALOR, "Entre Margem'")
                            .FechaLinha();

                        break;

                    case TipoRegistro.SUB_TITULO_MARGEM_D_E:

                        if (tipoAnterior != TipoRegistro.TITULO)
                        {
                            html = html
                                .AbreLinha(registro.Id)
                                    .CriarColuna(5, SEM_BORDA, "&nbsp;", "sem-borda")
                                .FechaLinha();
                            quebra = false;
                        }

                        html = html
                            .AbreLinha(TITULO_PREVIEW, registro.Id)
                                .CriarColuna(3, COLUNA, registro.Descricao)
                                .CriarColuna(COLUNA_VALOR, "M Direita")
                                .CriarColuna(COLUNA_VALOR, "M Esquerda")
                            .FechaLinha();

                        break;

                    case TipoRegistro.SUB_TITULO_ALL_IN:

                        if (tipoAnterior != TipoRegistro.TITULO)
                        {
                            html = html
                                .AbreLinha(registro.Id)
                                    .CriarColuna(5, SEM_BORDA, "&nbsp;", "sem-borda")
                                .FechaLinha();
                            quebra = false;
                        }

                        html = html
                            .AbreLinha(TITULO_PREVIEW, registro.Id)
                                .CriarColuna(COLUNA_VALOR, "Período")
                                .CriarColuna(COLUNA, "CIF")
                                .CriarColuna(COLUNA_VALOR, "Margem")
                                .CriarColuna(COLUNA_VALOR, "Valor 20'")
                                .CriarColuna(COLUNA_VALOR, "Valor 40'")
                            .FechaLinha();

                        break;

                    case TipoRegistro.ARMAZENAMEM_ALL_IN:

                        var faixaCif = $"De: {registro.CifMinimo.ToMoeda(registro.Moeda)} até {registro.CifMaximo.ToMoeda(registro.Moeda)}";

                        if (!string.IsNullOrEmpty(registro.DescricaoCif))
                            faixaCif = registro.DescricaoCif;

                        var periodo = $"{registro.Periodo.ToString()}º";

                        if (!string.IsNullOrEmpty(registro.DescricaoPeriodo))
                            periodo = registro.DescricaoPeriodo;

                        html = html
                            .AbreLinha(registro.Id)
                                .CriarColuna(COLUNA_VALOR, periodo)
                                .CriarColuna(COLUNA, faixaCif)
                                .CriarColuna(COLUNA_VALOR, registro.Margem.ToName())
                                .CriarColuna(COLUNA_VALOR, registro.Valor20.ToMoeda(registro.Moeda))
                                .CriarColuna(COLUNA_VALOR, registro.Valor40.ToMoeda(registro.Moeda))
                            .FechaLinha();

                        break;

                    case TipoRegistro.ARMAZENAGEM:
                    case TipoRegistro.ARMAZENAGEM_CIF:

                        html = html
                        .AbreLinha(registro.Id)
                            .CriarColuna(3, COLUNA, registro.Descricao);

                        if (tipoCarga == TipoCarga.CONTEINER)
                        {
                            if (registro.BaseCalculo == BaseCalculo.CIF)
                            {
                                if (registro.Valor20 == registro.Valor40)
                                {
                                    html = html
                                        .CriarColuna(2, VALOR_UNICO, $"{ registro.Valor20.ToNumero() } { registro.DescricaoValor}");
                                }
                                else
                                {
                                    html = html
                                        .CriarColuna(COLUNA_VALOR, $"{ registro.Valor20.ToNumero() } { registro.DescricaoValor}")
                                        .CriarColuna(COLUNA_VALOR, $"{ registro.Valor40.ToNumero() } { registro.DescricaoValor}");
                                }
                            }
                            else
                            {
                                if (registro.Valor20 == registro.Valor40)
                                {
                                    html = html
                                        .CriarColuna(2, VALOR_UNICO, $"{ registro.Valor20.ToMoeda(tipoMoeda) } { registro.DescricaoValor}");
                                }
                                else
                                {
                                    html = html
                                        .CriarColuna(COLUNA_VALOR, registro.Valor20.ToMoeda(tipoMoeda))
                                        .CriarColuna(COLUNA_VALOR, registro.Valor40.ToMoeda(tipoMoeda));
                                }
                            }
                        }
                        else if (tipoCarga == TipoCarga.CARGA_SOLTA || tipoCarga == TipoCarga.CARGA_BBK || tipoCarga == TipoCarga.CARGA_VEICULO)
                        {
                            if (registro.BaseCalculo == BaseCalculo.CIF)
                            {
                                html = html
                                    .CriarColuna(2, VALOR_UNICO, $"{ registro.Valor.ToNumero() } { registro.DescricaoValor}");
                            }
                            else
                            {
                                html = html
                                    .CriarColuna(2, VALOR_UNICO, $"{ registro.Valor.ToMoeda(tipoMoeda) } { registro.DescricaoValor}");
                            }
                        }
                        else
                        {
                            if (registro.BaseCalculo == BaseCalculo.CIF)
                            {
                                if (registro.Valor20 != registro.Valor40)
                                {
                                    html = html
                                        .CriarColuna(COLUNA_VALOR, $"{ registro.Valor20.ToNumero() } { registro.DescricaoValor}")
                                        .CriarColuna(COLUNA_VALOR, $"{ registro.Valor40.ToNumero() } { registro.DescricaoValor}");
                                }
                                else
                                {
                                    html = html
                                        .CriarColuna(2, VALOR_UNICO, $"{ registro.Valor20.ToNumero() } { registro.DescricaoValor}");
                                }
                            }
                            else
                            {
                                if (registro.Valor20 != registro.Valor40)
                                {
                                    html = html
                                        .CriarColuna(COLUNA_VALOR, registro.Valor20.ToMoeda(tipoMoeda))
                                        .CriarColuna(COLUNA_VALOR, registro.Valor40.ToMoeda(tipoMoeda));
                                }
                                else
                                {
                                    html = html
                                        .CriarColuna(2, VALOR_UNICO, $"{ registro.Valor20.ToMoeda(tipoMoeda) } { registro.DescricaoValor}");
                                }
                            }
                        }

                        html = html
                            .FechaLinha();

                        break;

                    case TipoRegistro.ARMAZENAGEM_MINIMO:
                    case TipoRegistro.ARMAZENAGEM_MINIMO_CIF:
                    case TipoRegistro.MINIMO_MECANICA_MANUAL:

                        html = html
                           .AbreLinha(registro.Id)
                               .CriarColuna(3, COLUNA, registro.Descricao);

                        if (registro.ValorMinimo > 0 && registro.ValorMinimo20 == 0 && registro.ValorMinimo40 == 0)
                        {
                            if (registro.ValorMinimo >= 0 && string.IsNullOrEmpty(registro.DescricaoValor))
                            {
                                html = html
                                    .CriarColuna(2, VALOR_UNICO, $"{ registro.ValorMinimo.ToMoeda(tipoMoeda) }");
                            }
                            else
                            {
                                if (registro.ValorMinimo == 0 && !string.IsNullOrEmpty(registro.DescricaoValor))
                                {
                                    html = html
                                        .CriarColuna(2, VALOR_UNICO, $"{registro.DescricaoValor}");
                                }
                                else
                                {
                                    html = html
                                        .CriarColuna(2, VALOR_UNICO, $"{ registro.ValorMinimo.ToMoeda(tipoMoeda) } {registro.DescricaoValor}");
                                }
                            }
                        }
                        else
                        {
                            if ((registro.ValorMinimo20 == registro.ValorMinimo40) && string.IsNullOrEmpty(registro.DescricaoValor))
                            {
                                html = html
                                    .CriarColuna(2, VALOR_UNICO, $"{ registro.ValorMinimo20.ToMoeda(tipoMoeda) } ");
                            }
                            else if ((registro.ValorMinimo20 != registro.ValorMinimo40) && string.IsNullOrEmpty(registro.DescricaoValor))
                            {
                                html = html
                                    .CriarColuna(COLUNA_VALOR, $"{ registro.ValorMinimo20.ToMoeda(tipoMoeda)  }")
                                    .CriarColuna(COLUNA_VALOR, $"{ registro.ValorMinimo40.ToMoeda(tipoMoeda)  }");
                            }
                            else
                            {
                                html = html
                                    .CriarColuna(2, VALOR_UNICO, $"{ registro.DescricaoValor } ");
                            }
                        }

                        html = html
                            .FechaLinha();

                        break;

                    case TipoRegistro.SERVIÇO_PARA_MARGEM:

                        for (int i = contador; i < linhas.Count; i--)
                        {
                            if (i > 0)
                            {
                                if (linhas[i].TipoRegistro == TipoRegistro.SUB_TITULO_MARGEM_D_E || linhas[i].TipoRegistro == TipoRegistro.SUB_TITULO_MARGEM)
                                {
                                    tipoRegistroAnterior = linhas[i].TipoRegistro;
                                    break;
                                }
                            }                          
                        }

                        if (tipoRegistroAnterior == TipoRegistro.SUB_TITULO_MARGEM_D_E)
                        {
                            html = html
                               .AbreLinha(registro.Id)
                                   .CriarColuna(3, COLUNA, registro.Descricao)
                                   .CriarColuna(COLUNA_VALOR, registro.ValorMargemDireita.ToMoeda(tipoMoeda))
                                   .CriarColuna(COLUNA_VALOR, registro.ValorMargemEsquerda.ToMoeda(tipoMoeda))
                               .FechaLinha();
                        }
                        else
                        {
                            html = html
                               .AbreLinha(registro.Id)
                                   .CriarColuna(2, COLUNA, registro.Descricao)
                                   .CriarColuna(COLUNA_VALOR, registro.ValorMargemDireita.ToMoeda(tipoMoeda))
                                   .CriarColuna(COLUNA_VALOR, registro.ValorMargemEsquerda.ToMoeda(tipoMoeda))
                                   .CriarColuna(COLUNA_VALOR, registro.ValorEntreMargens.ToMoeda(tipoMoeda))
                               .FechaLinha();
                        }

                        break;

                    case TipoRegistro.MINIMO_PARA_MARGEM:

                        for (int i = contador; i < linhas.Count; i--)
                        {
                            if (linhas[i].TipoRegistro == TipoRegistro.SUB_TITULO_MARGEM_D_E || linhas[i].TipoRegistro == TipoRegistro.SUB_TITULO_MARGEM)
                            {
                                tipoRegistroAnterior = linhas[i].TipoRegistro;
                                break;
                            }
                        }

                        if (tipoRegistroAnterior == TipoRegistro.SUB_TITULO_MARGEM_D_E)
                        {
                            html = html
                                .AbreLinha(registro.Id)
                                    .CriarColuna(3, COLUNA, registro.Descricao)
                                    .CriarColuna(COLUNA_VALOR, registro.ValorMinimoMargemDireita.ToMoeda(tipoMoeda))
                                    .CriarColuna(COLUNA_VALOR, registro.ValorMinimoMargemEsquerda.ToMoeda(tipoMoeda))
                                .FechaLinha();
                        }
                        else
                        {
                            html = html
                                  .AbreLinha(registro.Id)
                                      .CriarColuna(2, COLUNA, registro.Descricao)
                                      .CriarColuna(COLUNA_VALOR, registro.ValorMinimoMargemDireita.ToMoeda(tipoMoeda))
                                      .CriarColuna(COLUNA_VALOR, registro.ValorMinimoMargemEsquerda.ToMoeda(tipoMoeda))
                                      .CriarColuna(COLUNA_VALOR, registro.ValorMinimoEntreMargens.ToMoeda(tipoMoeda))
                                  .FechaLinha();
                        }

                        break;

                    case TipoRegistro.SERVICO_MECANICA_MANUAL:

                        html = html
                            .AbreLinha(registro.Id)
                                .CriarColuna(3, COLUNA, registro.Descricao);

                        if (registro.TipoCarga == TipoCarga.CARGA_SOLTA || registro.TipoCarga == TipoCarga.CARGA_BBK || registro.TipoCarga == TipoCarga.CARGA_VEICULO)
                        {
                            if (string.IsNullOrEmpty(registro.DescricaoValor))
                            {
                                html = html
                                    .CriarColuna(2, VALOR_UNICO, $"{ registro.Valor.ToMoeda(tipoMoeda) } ");
                            }
                            else
                            {
                                html = html
                                    .CriarColuna(2, VALOR_UNICO, $"{ registro.DescricaoValor } ");
                            }
                        }
                        else
                        {
                            if ((registro.Valor20 == registro.Valor40) && string.IsNullOrEmpty(registro.DescricaoValor))
                            {
                                html = html
                                    .CriarColuna(2, VALOR_UNICO, $"{ registro.Valor20.ToMoeda(tipoMoeda) } ");
                            }
                            else if ((registro.Valor20 != registro.Valor40) && string.IsNullOrEmpty(registro.DescricaoValor))
                            {
                                html = html
                                    .CriarColuna(COLUNA_VALOR, $"{ registro.Valor20.ToMoeda(tipoMoeda)  }")
                                    .CriarColuna(COLUNA_VALOR, $"{ registro.Valor40.ToMoeda(tipoMoeda)  }");
                            }
                            else
                            {
                                html = html
                                    .CriarColuna(2, VALOR_UNICO, $"{ registro.DescricaoValor } ");
                            }
                        }

                        html = html
                            .FechaLinha();

                        break;

                    case TipoRegistro.SERVICO_LIBERACAO:
                        html = html
                            .AbreLinha(registro.Id)
                                .CriarColuna(3, COLUNA, registro.Descricao)
                                .CriarColuna(COLUNA_VALOR, $"{ registro.Valor20.ToMoeda(tipoMoeda) } { registro.DescricaoValor  }")
                                .CriarColuna(COLUNA_VALOR, $"{ registro.Valor40.ToMoeda(tipoMoeda) } { registro.DescricaoValor  }")
                            .FechaLinha();
                        break;
                    case TipoRegistro.SERVICO_HUBPORT:

                        html = html
                          .AbreLinha(registro.Id)
                              .CriarColuna(4, COLUNA, registro.Descricao)
                              .CriarColuna(COLUNA_VALOR, $"{ registro.Valor.ToMoeda(tipoMoeda) } { registro.DescricaoValor  }")
                          .FechaLinha();

                        break;

                    case TipoRegistro.GERAIS:

                        html = html
                            .AbreLinha(registro.Id)
                                .CriarColuna(3, COLUNA, registro.Descricao);

                        if (tipoCarga == TipoCarga.CONTEINER || tipoCarga == TipoCarga.MUDANCA_REGIME)
                        {
                            if ((registro.Valor20 == registro.Valor40) && string.IsNullOrEmpty(registro.DescricaoValor))
                            {
                                html = html
                                    .CriarColuna(2, VALOR_UNICO, $"{ registro.Valor20.ToMoeda(tipoMoeda)  }");
                            }
                            else if ((registro.Valor20 == registro.Valor40) && !string.IsNullOrEmpty(registro.DescricaoValor))
                            {
                                if (registro.Valor20 == 0)
                                {
                                    html = html
                                        .CriarColuna(2, VALOR_UNICO, $" {registro.DescricaoValor}");
                                }
                                else
                                {
                                    html = html
                                        .CriarColuna(2, VALOR_UNICO, $"{ registro.Valor20.ToMoeda(tipoMoeda)  } {registro.DescricaoValor}");
                                }

                            }
                            else if ((registro.Valor20 != registro.Valor40) && string.IsNullOrEmpty(registro.DescricaoValor))
                            {
                                html = html
                                    .CriarColuna(COLUNA_VALOR, $"{ registro.Valor20.ToMoeda(tipoMoeda)  }")
                                    .CriarColuna(COLUNA_VALOR, $"{ registro.Valor40.ToMoeda(tipoMoeda)  }");
                            }
                            else
                            {
                                html = html
                                    .CriarColuna(2, VALOR_UNICO, $"{ registro.DescricaoValor  }");
                            }
                        }
                        else
                        {
                            if (registro.Valor > 0 && string.IsNullOrEmpty(registro.DescricaoValor))
                            {
                                html = html
                                    .CriarColuna(2, VALOR_UNICO, $"{ registro.Valor.ToMoeda(tipoMoeda) }");
                            }

                            if (registro.Valor > 0 && !string.IsNullOrEmpty(registro.DescricaoValor))
                            {
                                html = html
                                    .CriarColuna(2, VALOR_UNICO, $"{ registro.Valor.ToMoeda(tipoMoeda) } {registro.DescricaoValor}");
                            }

                            if (registro.Valor == 0 && string.IsNullOrEmpty(registro.DescricaoValor))
                            {
                                html = html
                                    .CriarColuna(2, VALOR_UNICO, new decimal(0).ToMoeda(tipoMoeda));
                            }

                            if (registro.Valor == 0 && !string.IsNullOrEmpty(registro.DescricaoValor))
                            {
                                if (registro.DescricaoValor.ToLower().Equals("sob consulta"))
                                {
                                    html = html
                                        .CriarColuna(2, VALOR_UNICO, $"{registro.DescricaoValor}");
                                }
                                else
                                {
                                    html = html
                                        .CriarColuna(2, VALOR_UNICO, $"{new decimal(0).ToMoeda(tipoMoeda)} {registro.DescricaoValor}");
                                }
                                
                            }
                        }

                        html = html
                            .FechaLinha();

                        break;
                    case TipoRegistro.MINIMO_GERAL:

                        html = html
                            .AbreLinha(registro.Id)
                                .CriarColuna(3, COLUNA, registro.Descricao);

                        if (tipoCarga == TipoCarga.CONTEINER)
                        {
                            if (registro.ValorMinimo20 > 0 && registro.ValorMinimo40 > 0)
                            {
                                if (registro.ValorMinimo20 != registro.ValorMinimo40)
                                {
                                    html = html
                                        .CriarColuna(COLUNA_VALOR, $"{ registro.ValorMinimo20.ToMoeda(tipoMoeda) } { registro.DescricaoValor  }")
                                        .CriarColuna(COLUNA_VALOR, $"{ registro.ValorMinimo40.ToMoeda(tipoMoeda) } { registro.DescricaoValor  }");
                                }
                                else
                                {
                                    html = html
                                        .CriarColuna(2, VALOR_UNICO, $"{ registro.ValorMinimo20.ToMoeda(tipoMoeda) } { registro.DescricaoValor  }");
                                }
                            }
                            else
                            {
                                if ((registro.ValorMinimo20 == registro.ValorMinimo40) && string.IsNullOrEmpty(registro.DescricaoValor))
                                {
                                    html = html
                                        .CriarColuna(2, VALOR_UNICO, $"{ registro.ValorMinimo20.ToMoeda(tipoMoeda) } ");
                                }
                                else if ((registro.ValorMinimo20 != registro.ValorMinimo40) && string.IsNullOrEmpty(registro.DescricaoValor))
                                {
                                    html = html
                                        .CriarColuna(COLUNA_VALOR, $"{ registro.ValorMinimo20.ToMoeda(tipoMoeda)  }")
                                        .CriarColuna(COLUNA_VALOR, $"{ registro.ValorMinimo40.ToMoeda(tipoMoeda)  }");
                                }
                                else
                                {
                                    html = html
                                        .CriarColuna(2, VALOR_UNICO, $"{ registro.DescricaoValor } ");
                                }
                            }
                        }
                        else if (tipoCarga == TipoCarga.CARGA_SOLTA || tipoCarga == TipoCarga.CARGA_BBK || tipoCarga == TipoCarga.CARGA_VEICULO)
                        {
                            html = html
                                .CriarColuna(2, VALOR_UNICO, $"{ registro.ValorMinimo.ToMoeda(tipoMoeda) } { registro.DescricaoValor  }");
                        }
                        else
                        {
                            if (registro.ValorMinimo20 != registro.ValorMinimo40)
                            {
                                html = html
                                    .CriarColuna(COLUNA_VALOR, registro.ValorMinimo20.ToMoeda(tipoMoeda))
                                    .CriarColuna(COLUNA_VALOR, registro.ValorMinimo40.ToMoeda(tipoMoeda));
                            }
                            else
                            {
                                html = html
                                    .CriarColuna(2, VALOR_UNICO, $"{ registro.ValorMinimo20.ToMoeda(tipoMoeda) } { registro.DescricaoValor}");
                            }
                        }

                        html = html
                            .FechaLinha();

                        break;
                    case TipoRegistro.PERIODO_PADRAO:

                        html = html
                            .AbreLinha(registro.Id)
                                .CriarColuna(3, COLUNA, registro.Descricao);

                        if (tipoCarga == TipoCarga.CONTEINER)
                        {
                            if ((registro.Valor20 == registro.Valor40) && string.IsNullOrEmpty(registro.DescricaoValor))
                            {
                                html = html
                                    .CriarColuna(2, VALOR_UNICO, $"{ registro.Valor20.ToMoeda(tipoMoeda)  }");
                            }
                            else if ((registro.Valor20 == registro.Valor40) && !string.IsNullOrEmpty(registro.DescricaoValor))
                            {
                                if (registro.Valor20 == 0)
                                {
                                    html = html
                                        .CriarColuna(2, VALOR_UNICO, $" {registro.DescricaoValor}");
                                }
                                else
                                {
                                    html = html
                                        .CriarColuna(2, VALOR_UNICO, $"{ registro.Valor20.ToMoeda(tipoMoeda)  } {registro.DescricaoValor}");
                                }

                            }
                            else if ((registro.Valor20 != registro.Valor40) && string.IsNullOrEmpty(registro.DescricaoValor))
                            {
                                html = html
                                    .CriarColuna(COLUNA_VALOR, $"{ registro.Valor20.ToMoeda(tipoMoeda)  }")
                                    .CriarColuna(COLUNA_VALOR, $"{ registro.Valor40.ToMoeda(tipoMoeda)  }");
                            }
                            else
                            {
                                html = html
                                    .CriarColuna(2, VALOR_UNICO, $"{ registro.DescricaoValor  }");
                            }
                        }
                        else if (tipoCarga == TipoCarga.CARGA_SOLTA || tipoCarga == TipoCarga.CARGA_BBK || tipoCarga == TipoCarga.CARGA_VEICULO)
                        {
                            if (registro.Valor > 0)
                            {
                                if (string.IsNullOrEmpty(registro.DescricaoValor))
                                {
                                    html = html
                                        .CriarColuna(2, VALOR_UNICO, $"{ registro.Valor.ToMoeda(tipoMoeda) }");
                                }
                                else
                                {
                                    html = html
                                        .CriarColuna(2, VALOR_UNICO, $"{ registro.Valor.ToMoeda(tipoMoeda) } {registro.DescricaoValor}");
                                }

                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(registro.DescricaoValor))
                                {
                                    html = html
                                        .CriarColuna(2, VALOR_UNICO, $"{registro.DescricaoValor}");
                                }
                                else
                                {
                                    html = html
                                        .CriarColuna(2, VALOR_UNICO, registro.Valor.ToMoeda(tipoMoeda));
                                }
                            }
                        }
                        else
                        {
                            if (registro.Valor20 != registro.Valor40)
                            {
                                html = html
                                    .CriarColuna(COLUNA_VALOR, registro.Valor20.ToMoeda(tipoMoeda))
                                    .CriarColuna(COLUNA_VALOR, registro.Valor40.ToMoeda(tipoMoeda));
                            }
                            else
                            {
                                html = html
                                    .CriarColuna(2, VALOR_UNICO, $"{ registro.Valor20.ToMoeda(tipoMoeda) } { registro.DescricaoValor}");
                            }
                        }

                        html = html
                            .FechaLinha();
                        break;
                    case TipoRegistro.CONDICAO_GERAL:

                        try
                        {
                            if (HttpContext.Current.User.IsInRole("Administrador") || HttpContext.Current.User.IsInRole("OportunidadesProposta:PropostaCondicoesGerais"))
                            {
                                if (oportunidadeId.HasValue)
                                    registro.OportunidadeId = oportunidadeId.Value;

                                registro.CondicoesGerais = FormatarCondicoesGerais(registro);

                                html = html
                                    .AbreLinha(CONDICOES, registro.Id)
                                    .CriarColuna(5, CONDICOES, registro.CondicoesGerais)
                                .FechaLinha();
                            }
                        }
                        catch
                        {
                            throw;
                        }

                        break;
                    default:
                        break;
                }

                tipoAnterior = tipo;
                tipoCarga = TipoCarga.CONTEINER;
                tipoMoeda = Moeda.REAL;
                tipoRegistroAnterior = TipoRegistro.TITULO;
                contador++;
            }

            html = html
                .FechaTabela();

            return html.Compilar().ToString();
        }

        public string ObterCondicoesGerais(IEnumerable<LayoutDTO> linhas)
        {
            var registro = linhas.Where(c => c.TipoRegistro == TipoRegistro.CONDICAO_GERAL).FirstOrDefault();

            if (registro != null)
            {
                var linha = FormatarCondicoesGerais(registro);

                var html = new HtmlBuilder();

                if (HttpContext.Current.User.IsInRole("Administrador") || HttpContext.Current.User.IsInRole("OportunidadesProposta:PropostaCondicoesGerais"))
                {
                    html = html.AdicionaParagrafoComId("LinhaCondicoesGerais", JUSTIFICADO, linha, registro.Id);
                }
                else
                {
                    html = html.AdicionaDiv(linha, JUSTIFICADO);
                }

                return html.Compilar().ToString();
            }

            return string.Empty;
        }

        private string FormatarCamposCondicoesIniciais(LayoutDTO registro, int id)
        {
            if (registro.CondicoesIniciais == null)
                return string.Empty;

            MatchCollection variaveis = Regex.Matches(registro.CondicoesIniciais, @"(?<=\{)[^}]*(?=\})");

            string valor = string.Empty;
            string valorExtenso = string.Empty;

            foreach (Match variavel in variaveis)
            {
                foreach (Capture capture in variavel.Captures)
                {
                    var variavelPura = LimparTagsHtml(capture.Value);

                    if (Regex.IsMatch(variavelPura, @"^[a-z]:[\s\S]"))
                    {
                        try
                        {
                            var conjunto = variavelPura.Split(':');

                            var tipo = conjunto[0].ToString();
                            var campo = conjunto[1].ToString();

                            bool contemExtenso = false;

                            if (campo.ToLower().Contains("[extenso]"))
                            {
                                contemExtenso = true;

                                campo = campo
                                    .Replace("[extenso]", string.Empty)
                                    .Replace("&nbsp;", string.Empty);
                            }

                            if (campo.Contains("SubClientes") || campo.Contains("GruposCNPJ"))
                            {
                                if (campo.Contains("SubClientes"))
                                {
                                    IEnumerable<ClientePropostaDTO> subClientes = null;

                                    var segmento = Regex.Match(campo, @"\[(.*?)\]");

                                    if (segmento.Success)
                                    {
                                        var nomeSegmento = segmento.Value
                                            .ToUpper()
                                            .Replace("[", string.Empty)
                                            .Replace("]", string.Empty);

                                        if (Enum.TryParse(nomeSegmento, out SegmentoSubCliente segmentoSubCliente))
                                        {
                                            subClientes = _oportunidadeRepositorio
                                                .ObterSubClientesDaPropostaPorSegmento(id, segmentoSubCliente);

                                            valor = string.Join("<br />", subClientes.Select(c => $"{c.Descricao} - {c.Documento}"));
                                        }
                                    }
                                }

                                if (campo.Contains("GruposCNPJ"))
                                {
                                    var clientesGrupo = _oportunidadeRepositorio.ObterClientesGrupoCNPJ(id);

                                    if (clientesGrupo != null)
                                    {
                                        valor = string.Join("<br />", clientesGrupo.Select(c => $"{c.Descricao} - {c.Documento}"));
                                    }
                                }
                            }
                            else
                            {
                                valor = _oportunidadeRepositorio.ObterValorPorCampo(campo, id);

                                if (StringHelpers.IsNumero(valor))
                                {
                                    if (tipo.Contains("i"))
                                    {
                                        valor = valor.ToInt().ToString();
                                    }

                                    if (tipo.Contains("f"))
                                    {
                                        valor = string.Format("{0:N2}", valor.ToDBDecimal());
                                    }
                                }

                                if (StringHelpers.IsNumero(valor) && contemExtenso)
                                    valor = $"{valor} ({valor.ToDecimal().EscreverValorPorExtenso()})";
                            }

                            registro.CondicoesIniciais = registro.CondicoesIniciais.Replace("{" + variavel + "}", valor);
                        }
                        catch (Exception)
                        {
                            valor = string.Empty;
                        }
                    }
                }
            }

            registro.CondicoesIniciais = registro.CondicoesIniciais
                           .Replace("{", string.Empty)
                           .Replace("}", string.Empty);

            return registro.CondicoesIniciais;
        }

        private string FormatarCondicoesGerais(LayoutDTO registro)
        {
            if (registro.CondicoesGerais == null)
                return string.Empty;

            MatchCollection variaveis = Regex.Matches(registro.CondicoesGerais, @"(?<=\{)[^}]*(?=\})");

            string valor = string.Empty;
            string valorExtenso = string.Empty;

            foreach (Match variavel in variaveis)
            {
                foreach (Capture capture in variavel.Captures)
                {
                    var variavelPura = LimparTagsHtml(capture.Value);

                    if (Regex.IsMatch(variavelPura, @"[0-9]:[a-z]:[\s\S]"))
                    {
                        var conjunto = variavelPura.Split(':');

                        var linha = conjunto[0].ToString();
                        var tipo = conjunto[1].ToString();
                        var campo = conjunto[2].ToString();

                        bool contemExtenso = false;

                        if (campo.ToLower().Contains("[extenso]"))
                        {
                            contemExtenso = true;

                            campo = campo
                                .Replace("[extenso]", string.Empty)
                                .Replace("&nbsp;", string.Empty);
                        }

                        try
                        {
                            if (registro.OportunidadeId > 0)
                            {
                                if (linha.ToInt() > 0)
                                    valor = _layoutRepositorio.ObterValorPorLinha(linha.ToInt(), registro.ModeloId, campo, registro.OportunidadeId);
                                else
                                    valor = _layoutRepositorio.ObterValorSemLinha(registro.ModeloId, campo, registro.OportunidadeId);
                            }
                            else
                            {
                                if (linha.ToInt() > 0)
                                    valor = _layoutRepositorio.ObterValorPorLinha(linha.ToInt(), registro.ModeloId, campo, registro.OportunidadeId);
                                else
                                    valor = _modeloRepositorio.ObterValorPorCampo(campo, registro.ModeloId);
                            }

                            if (StringHelpers.IsNumero(valor))
                            {
                                if (tipo.Contains("i"))
                                {
                                    valor = valor.ToInt().ToString();
                                }

                                if (tipo.Contains("f"))
                                {
                                    var valorBanco = valor.Replace(".", ",");

                                    valor = valorBanco.ToNumero();
                                }

                                if (tipo.Contains("p"))
                                {
                                    valor = valor.Replace(".", ",");

                                    valor = ((valor.ToDecimal() / 100) * 100).ToString();

                                    valor = valor.ToDecimal().ToString("0.##");
                                }
                            }

                            if (StringHelpers.IsNumero(valor) && contemExtenso)
                                valor = $"{valor} ({valor.ToDecimal().EscreverValorPorExtenso()})";

                            registro.CondicoesGerais = registro.CondicoesGerais.Replace("{" + variavelPura + "}", valor);
                        }
                        catch (Exception ex)
                        {
                            valor = string.Empty;
                        }
                    }
                }
            }

            registro.CondicoesGerais = registro.CondicoesGerais
                .Replace("{", string.Empty)
                .Replace("}", string.Empty);

            return registro.CondicoesGerais;
        }

        public string LimparTagsHtml(string texto)
        {
            var tags = new List<string>()
            {
                "<a>",
                "<abbr>",
                "<acronym>",
                "<address>",
                "<applet>",
                "<area>",
                "<article>",
                "<aside>",
                "<audio>",
                "<b>",
                "<base>",
                "<basefont>",
                "<bdi>",
                "<bdo>",
                "<big>",
                "<blockquote>",
                "<body>",
                "<br>",
                "<button>",
                "<canvas>",
                "<caption>",
                "<center>",
                "<cite>",
                "<code>",
                "<col>",
                "<colgroup>",
                "<command>",
                "<datalist>",
                "<dd>",
                "<del>",
                "<details>",
                "<dfn>",
                "<dir>",
                "<div>",
                "<dl>",
                "<dt>",
                "<em>",
                "<embed>",
                "<fieldset>",
                "<figcaption>",
                "<figure>",
                "<font>",
                "<footer>",
                "<form>",
                "<frame>",
                "<frameset>",
                "<h1>",
                "<h2>",
                "<h3>",
                "<h4>",
                "<h5>",
                "<h6>",
                "<head>",
                "<header>",
                "<hgroup>",
                "<hr>",
                "<html>",
                "<i>",
                "<iframe>",
                "<img>",
                "<input>",
                "<ins>",
                "<kbd>",
                "<keygen>",
                "<label>",
                "<legend>",
                "<li>",
                "<link>",
                "<map>",
                "<mark>",
                "<menu>",
                "<meta>",
                "<meter>",
                "<nav>",
                "<noframes>",
                "<noscript>",
                "<object>",
                "<ol>",
                "<optgroup>",
                "<option>",
                "<output>",
                "<p>",
                "<param>",
                "<pre>",
                "<progress>",
                "<q>",
                "<rp>",
                "<rt>",
                "<ruby>",
                "<s>",
                "<samp>",
                "<script>",
                "<section>",
                "<select>",
                "<small>",
                "<source>",
                "<span>",
                "<strike>",
                "<strong>",
                "<style>",
                "<sub>",
                "<summary>",
                "<sup>",
                "<table>",
                "<tbody>",
                "<td>",
                "<textarea>",
                "<tfoot>",
                "<th>",
                "<thead>",
                "<time>",
                "<title>",
                "<tr>",
                "<track>",
                "<tt>",
                "<u>",
                "<ul>",
                "<var>",
                "<video>",
                "<wbr>",
                "</a>",
                "</abbr>",
                "</acronym>",
                "</address>",
                "</applet>",
                "</area>",
                "</article>",
                "</aside>",
                "</audio>",
                "</b>",
                "</base>",
                "</basefont>",
                "</bdi>",
                "</bdo>",
                "</big>",
                "</blockquote>",
                "</body>",
                "</br>",
                "</button>",
                "</canvas>",
                "</caption>",
                "</center>",
                "</cite>",
                "</code>",
                "</col>",
                "</colgroup>",
                "</command>",
                "</datalist>",
                "</dd>",
                "</del>",
                "</details>",
                "</dfn>",
                "</dir>",
                "</div>",
                "</dl>",
                "</dt>",
                "</em>",
                "</embed>",
                "</fieldset>",
                "</figcaption>",
                "</figure>",
                "</font>",
                "</footer>",
                "</form>",
                "</frame>",
                "</frameset>",
                "</h1>",
                "</h2>",
                "</h3>",
                "</h4>",
                "</h5>",
                "</h6>",
                "</head>",
                "</header>",
                "</hgroup>",
                "</hr>",
                "</html>",
                "</i>",
                "</iframe>",
                "</img>",
                "</input>",
                "</ins>",
                "</kbd>",
                "</keygen>",
                "</label>",
                "</legend>",
                "</li>",
                "</link>",
                "</map>",
                "</mark>",
                "</menu>",
                "</meta>",
                "</meter>",
                "</nav>",
                "</noframes>",
                "</noscript>",
                "</object>",
                "</ol>",
                "</optgroup>",
                "</option>",
                "</output>",
                "</p>",
                "</param>",
                "</pre>",
                "</progress>",
                "</q>",
                "</rp>",
                "</rt>",
                "</ruby>",
                "</s>",
                "</samp>",
                "</script>",
                "</section>",
                "</select>",
                "</small>",
                "</source>",
                "</span>",
                "</strike>",
                "</strong>",
                "</style>",
                "</sub>",
                "</summary>",
                "</sup>",
                "</table>",
                "</tbody>",
                "</td>",
                "</textarea>",
                "</tfoot>",
                "</th>",
                "</thead>",
                "</time>",
                "</title>",
                "</tr>",
                "</track>",
                "</tt>",
                "</u>",
                "</ul>",
                "</var>",
                "</video>",
                "</wbr>",
                "&nbsp;"
            };

            foreach (var tag in tags)
            {
                texto = texto.Replace(tag, string.Empty);
            }

            return texto;
        }
    }
}