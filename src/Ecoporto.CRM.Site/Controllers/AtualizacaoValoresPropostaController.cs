using Ecoporto.CRM.Business.Enums;
using Ecoporto.CRM.Business.Extensions;
using Ecoporto.CRM.Business.Factory;
using Ecoporto.CRM.Business.Helpers;
using Ecoporto.CRM.Business.Interfaces.Repositorios;
using Ecoporto.CRM.Business.Models;
using Ecoporto.CRM.Site.Extensions;
using Ecoporto.CRM.Site.Helpers;
using Ecoporto.CRM.Site.Models;
using NLog;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace Ecoporto.CRM.Site.Controllers
{
    public class AtualizacaoValoresPropostaController : BaseController
    {
        private readonly ILayoutPropostaRepositorio _layoutPropostaRepositorio;

        public AtualizacaoValoresPropostaController(ILayoutPropostaRepositorio layoutPropostaRepositorio, ILogger logger) : base(logger)
        {
            _layoutPropostaRepositorio = layoutPropostaRepositorio;
        }

        [HttpPost]
        public ActionResult AtualizarTitulo(int linha, string descricao, int oportunidadeId, int tipoRegistro)
        {
            if (linha == 0 || oportunidadeId == 0)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var linhaAnterior = _layoutPropostaRepositorio.ObterLayoutEdicaoPorLinha(oportunidadeId, linha);

            if (linhaAnterior != null)
            {
                try
                {
                    _layoutPropostaRepositorio.AtualizarTitulos(linha, descricao, oportunidadeId, tipoRegistro);

                    if (linhaAnterior.Descricao != descricao)
                    {
                        _layoutPropostaRepositorio.GravarCampoPropostaAlterado(new OportunidadeAlteracaoLinhaProposta
                        {
                            OportunidadeId = oportunidadeId,
                            Linha = linha,
                            UsuarioId = User.ObterId(),
                            Propriedade = "Descricao"
                        });
                    }
                }
                catch
                {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
                }
            }

            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        [HttpPost]
        public ActionResult AtualizarArmazenagem(
            int linha,
            int oportunidadeId,
            string descricao,
            BaseCalculo baseCalculo,
            TipoCarga tipoCarga,
            int periodo,
            int qtdeDias,
            string valor,
            string valor20,
            string valor40,
            string adicionalArmazenagem,
            string adicionalGRC,
            string minimoGRC,
            string adicionalIMO,
            string exercito,
            string adicionalIMOGRC,
            string valorANVISA,
            string anvisaGRC,
            string target)
        {
            if (linha == 0 || oportunidadeId == 0)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var linhaAnterior = _layoutPropostaRepositorio.ObterLayoutEdicaoPorLinha(oportunidadeId, linha);

            if (tipoCarga == TipoCarga.CARGA_SOLTA || tipoCarga == TipoCarga.CARGA_BBK || tipoCarga == TipoCarga.CARGA_VEICULO)
            {
                valor20 = "0";
                valor40 = "0";
            }
            else
            {
                valor = "0";
            }

            if (linhaAnterior != null)
            {
                var layout = LayoutFactory.NovoLayoutArmazenagem(
                        linhaAnterior.ModeloId,
                        linha,
                        descricao,
                        valor.ToDecimal(),
                        valor20.ToDecimal(),
                        valor40.ToDecimal(),
                        tipoCarga,
                        linhaAnterior.ServicoId,
                        baseCalculo,
                        qtdeDias,
                        adicionalArmazenagem.ToDecimal(),
                        adicionalGRC.ToDecimal(),
                        minimoGRC.ToDecimal(),
                        adicionalIMO.ToDecimal(),
                        exercito.ToDecimal(),
                        adicionalIMOGRC.ToDecimal(),
                        valorANVISA.ToDecimal(),
                        anvisaGRC.ToDecimal(),
                        periodo,
                        linhaAnterior.Moeda,
                        linhaAnterior.DescricaoValor,
                        linhaAnterior.TipoDocumentoId,
                        linhaAnterior.BaseExcesso,
                        linhaAnterior.Margem,
                        linhaAnterior.ValorExcesso,
                        linhaAnterior.AdicionalPeso,
                        linhaAnterior.PesoLimite,
                        linhaAnterior.GrupoAtracacaoId,
                        linhaAnterior.ProRata,
                        linhaAnterior.Ocultar);

                var layoutTemp = LayoutFactory.NovoLayoutArmazenagem(
                        linhaAnterior.ModeloId,
                        linhaAnterior.Linha,
                        linhaAnterior.Descricao,
                        linhaAnterior.Valor,
                        linhaAnterior.Valor20,
                        linhaAnterior.Valor40,
                        linhaAnterior.TipoCarga,
                        linhaAnterior.ServicoId,
                        linhaAnterior.BaseCalculo,
                        linhaAnterior.QtdeDias,
                        linhaAnterior.AdicionalArmazenagem,
                        linhaAnterior.AdicionalGRC,
                        linhaAnterior.MinimoGRC,
                        linhaAnterior.AdicionalIMO,
                        linhaAnterior.Exercito,
                        linhaAnterior.AdicionalIMOGRC,
                        linhaAnterior.ValorANVISA,
                        linhaAnterior.AnvisaGRC,
                        linhaAnterior.Periodo,
                        linhaAnterior.Moeda,
                        linhaAnterior.DescricaoValor,
                        linhaAnterior.TipoDocumentoId,
                        linhaAnterior.BaseExcesso,
                        linhaAnterior.Margem,
                        linhaAnterior.ValorExcesso,
                        linhaAnterior.AdicionalPeso,
                        linhaAnterior.PesoLimite,
                        linhaAnterior.GrupoAtracacaoId,
                        linhaAnterior.ProRata,
                        linhaAnterior.Ocultar);

                try
                {
                    _layoutPropostaRepositorio.AtualizarArmazenagem(layout, linha, oportunidadeId);
                    GravarCamposAlterados(linha, oportunidadeId, layout, layoutTemp, target);

                    if (target == "QtdeDias" || target == "AdicionalArmazenagem" || target == "AdicionalGRC" || target == "MinimoGRC" || target == "AdicionalIMO" || target == "ValorANVISA" || target == "AnvisaGRC" || target == "AdicionalIMOGRC")
                    {
                        var proximasLinhas = _layoutPropostaRepositorio.ObterProximasLinhas(linhaAnterior.Linha, linhaAnterior.ModeloId, linhaAnterior.OportunidadeId);

                        List<int> linhasAtualizadas = new List<int>();

                        foreach (var proximo in proximasLinhas)
                        {
                            if (proximo.TipoRegistro == TipoRegistro.ARMAZENAGEM)
                            {
                                _layoutPropostaRepositorio.AtualizarCamposEmComumArmazenagem(layout, target, oportunidadeId, linha, proximo.Id);

                                var valoresAlterados = LayoutFactory.NovoLayoutArmazenagem(
                                    proximo.ModeloId,
                                    proximo.Linha,
                                    proximo.Descricao,
                                    proximo.Valor,
                                    proximo.Valor20,
                                    proximo.Valor40,
                                    proximo.TipoCarga,
                                    proximo.ServicoId,
                                    proximo.BaseCalculo,
                                    layout.QtdeDias,
                                    layout.AdicionalArmazenagem,
                                    layout.AdicionalGRC,
                                    layout.MinimoGRC,
                                    layout.AdicionalIMO,
                                    layout.Exercito,
                                    layout.AdicionalIMOGRC,
                                    layout.ValorANVISA,
                                    layout.AnvisaGRC,
                                    proximo.Periodo,
                                    proximo.Moeda,
                                    proximo.DescricaoValor,
                                    layout.TipoDocumentoId,
                                    layout.BaseExcesso,
                                    layout.Margem,
                                    layout.ValorExcesso,
                                    layout.AdicionalPeso,
                                    layout.PesoLimite,
                                    layout.GrupoAtracacaoId,
                                    layout.ProRata,
                                    linhaAnterior.Ocultar);

                                GravarCamposAlterados(proximo.Linha, proximo.OportunidadeId, valoresAlterados, proximo, target);

                                linhasAtualizadas.Add(proximo.Linha);
                            }
                            else
                            {
                                if (target != "AdicionalIMO" && target != "ValorANVISA")
                                    break;
                            }
                        }

                        if (linhasAtualizadas.Any())
                            return new HttpStatusCodeResult(HttpStatusCode.OK, string.Join(",", linhasAtualizadas.ToArray()));
                    }
                }
                catch (System.Exception ex)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
                }
            }

            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        [HttpPost]
        public ActionResult AtualizarArmazenagemCIF(
            int linha,
            int oportunidadeId,
            string descricao,
            BaseCalculo baseCalculo,
            TipoCarga tipoCarga,
            int periodo,
            int qtdeDias,
            string valor,
            string valor20,
            string valor40,
            string valorCif,
            string adicionalArmazenagem,
            string adicionalGRC,
            string minimoGRC,
            string adicionalIMO,
            string exercito,
            string adicionalIMOGRC,
            string valorANVISA,
            string anvisaGRC,
            string target)
        {
            if (linha == 0 || oportunidadeId == 0)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var linhaAnterior = _layoutPropostaRepositorio.ObterLayoutEdicaoPorLinha(oportunidadeId, linha);

            if (tipoCarga == TipoCarga.CARGA_SOLTA || tipoCarga == TipoCarga.CARGA_BBK || tipoCarga == TipoCarga.CARGA_VEICULO)
            {
                valor20 = "0";
                valor40 = "0";
            }
            else
            {
                valor = "0";
            }

            if (linhaAnterior != null)
            {
                var layout = LayoutFactory.NovoLayoutArmazenagemCIF(
                        linhaAnterior.ModeloId,
                        linha,
                        descricao,
                        valorCif.ToDecimal(),
                        valor.ToDecimal(),
                        valor20.ToDecimal(),
                        valor40.ToDecimal(),
                        tipoCarga,
                        linhaAnterior.ServicoId,
                        baseCalculo,
                        qtdeDias,
                        adicionalArmazenagem.ToDecimal(),
                        adicionalGRC.ToDecimal(),
                        minimoGRC.ToDecimal(),
                        adicionalIMO.ToDecimal(),
                        exercito.ToDecimal(),
                        adicionalIMOGRC.ToDecimal(),
                        valorANVISA.ToDecimal(),
                        anvisaGRC.ToDecimal(),
                        periodo,
                        linhaAnterior.Moeda,
                        linhaAnterior.DescricaoValor,
                        linhaAnterior.TipoDocumentoId,
                        linhaAnterior.BaseExcesso,
                        linhaAnterior.Margem,
                        linhaAnterior.ValorExcesso,
                        linhaAnterior.AdicionalPeso,
                        linhaAnterior.PesoLimite,
                        linhaAnterior.ProRata,
                        linhaAnterior.Ocultar);

                var layoutTemp = LayoutFactory.NovoLayoutArmazenagemCIF(
                        linhaAnterior.ModeloId,
                        linhaAnterior.Linha,
                        linhaAnterior.Descricao,
                        linhaAnterior.ValorCIF,
                        linhaAnterior.Valor,
                        linhaAnterior.Valor20,
                        linhaAnterior.Valor40,
                        linhaAnterior.TipoCarga,
                        linhaAnterior.ServicoId,
                        linhaAnterior.BaseCalculo,
                        linhaAnterior.QtdeDias,
                        linhaAnterior.AdicionalArmazenagem,
                        linhaAnterior.AdicionalGRC,
                        linhaAnterior.MinimoGRC,
                        linhaAnterior.AdicionalIMO,
                        linhaAnterior.Exercito,
                        linhaAnterior.AdicionalIMOGRC,
                        linhaAnterior.ValorANVISA,
                        linhaAnterior.AnvisaGRC,
                        linhaAnterior.Periodo,
                        linhaAnterior.Moeda,
                        linhaAnterior.DescricaoValor,
                        linhaAnterior.TipoDocumentoId,
                        linhaAnterior.BaseExcesso,
                        linhaAnterior.Margem,
                        linhaAnterior.ValorExcesso,
                        linhaAnterior.AdicionalPeso,
                        linhaAnterior.PesoLimite,
                        linhaAnterior.ProRata,
                        linhaAnterior.Ocultar);

                try
                {
                    _layoutPropostaRepositorio.AtualizarArmazenagemCIF(layout, linha, oportunidadeId);

                    GravarCamposAlterados(linha, oportunidadeId, layout, layoutTemp, target);

                    if (target == "QtdeDias" || target == "AdicionalArmazenagem" || target == "AdicionalGRC" || target == "MinimoGRC" || target == "AdicionalIMO" || target == "ValorANVISA" || target == "AnvisaGRC" || target == "AdicionalIMOGRC")
                    {
                        var proximasLinhas = _layoutPropostaRepositorio.ObterProximasLinhas(linhaAnterior.Linha, linhaAnterior.ModeloId, linhaAnterior.OportunidadeId);

                        List<int> linhasAtualizadas = new List<int>();

                        foreach (var proximo in proximasLinhas)
                        {
                            if (proximo.TipoRegistro == TipoRegistro.ARMAZENAGEM_CIF)
                            {
                                _layoutPropostaRepositorio.AtualizarCamposEmComumArmazenagemCIF(layout, target, oportunidadeId, linha, proximo.Id);

                                var valoresAlterados = LayoutFactory.NovoLayoutArmazenagemCIF(
                                    proximo.ModeloId,
                                    proximo.Linha,
                                    proximo.Descricao,
                                    layout.ValorCif,
                                    proximo.Valor,
                                    proximo.Valor20,
                                    proximo.Valor40,
                                    proximo.TipoCarga,
                                    proximo.ServicoId,
                                    proximo.BaseCalculo,
                                    layout.QtdeDias,
                                    layout.AdicionalArmazenagem,
                                    layout.AdicionalGRC,
                                    layout.MinimoGRC,
                                    layout.AdicionalIMO,
                                    layout.Exercito,
                                    layout.AdicionalIMOGRC,
                                    layout.ValorANVISA,
                                    layout.AnvisaGRC,
                                    proximo.Periodo,
                                    proximo.Moeda,
                                    proximo.DescricaoValor,
                                    layout.TipoDocumentoId,
                                    layout.BaseExcesso,
                                    layout.Margem,
                                    layout.ValorExcesso,
                                    layout.AdicionalPeso,
                                    layout.PesoLimite,
                                    layout.ProRata,
                                    linhaAnterior.Ocultar);

                                GravarCamposAlterados(proximo.Linha, proximo.OportunidadeId, valoresAlterados, proximo, target);

                                linhasAtualizadas.Add(proximo.Linha);
                            }
                            else
                            {
                                if (target != "AdicionalIMO" && target != "ValorANVISA")
                                    break;
                            }
                        }

                        if (linhasAtualizadas.Any())
                            return new HttpStatusCodeResult(HttpStatusCode.OK, string.Join(",", linhasAtualizadas.ToArray()));
                    }
                }
                catch (System.Exception ex)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
                }
            }

            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        [HttpPost]
        public ActionResult AtualizarArmazenagemMinimo(
            int linha,
            int oportunidadeId,
            string descricao,
            TipoCarga tipoCarga,
            BaseCalculo baseCalculo,
            string valorMinimo,
            string valorMinimo20,
            string valorMinimo40,
            string limiteBls,
            string descricaoValor)
        {
            if (linha == 0 || oportunidadeId == 0)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var linhaAnterior = _layoutPropostaRepositorio.ObterLayoutEdicaoPorLinha(oportunidadeId, linha);

            if (linhaAnterior != null)
            {
                var layout = LayoutFactory.NovoLayoutArmazenagemMinimo(
                    linhaAnterior.ModeloId,
                    linha,
                    descricao,
                    valorMinimo.ToDecimal(),
                    valorMinimo20.ToDecimal(),
                    valorMinimo40.ToDecimal(),
                    tipoCarga,
                    baseCalculo,
                    linhaAnterior.Margem,
                    linhaAnterior.ServicoId,
                    linhaAnterior.LinhaReferencia,
                    descricaoValor.RemoverCaracteresEspeciais(),
                    limiteBls.ToInt(),
                    linhaAnterior.Ocultar);

                var layoutTemp = LayoutFactory.NovoLayoutArmazenagemMinimo(
                    linhaAnterior.ModeloId,
                    linhaAnterior.Linha,
                    linhaAnterior.Descricao,
                    linhaAnterior.ValorMinimo,
                    linhaAnterior.ValorMinimo20,
                    linhaAnterior.ValorMinimo40,
                    linhaAnterior.TipoCarga,
                    linhaAnterior.BaseCalculo,
                    linhaAnterior.Margem,
                    linhaAnterior.ServicoId,
                    linhaAnterior.LinhaReferencia,
                    linhaAnterior.DescricaoValor,
                    linhaAnterior.LimiteBls,
                    linhaAnterior.Ocultar);

                try
                {
                    _layoutPropostaRepositorio.AtualizarArmazenagemMinimo(layout, linha, oportunidadeId);
                    GravarCamposAlterados(linha, oportunidadeId, layout, layoutTemp, string.Empty);
                }
                catch
                {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
                }
            }

            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        [HttpPost]
        public ActionResult AtualizarArmazenagemMinimoCIF(
            int linha,
            int oportunidadeId,
            string descricao,
            TipoCarga tipoCarga,
            string valorCif,
            string valorMinimo,
            string valorMinimo20,
            string valorMinimo40,
            string limiteBls,
            string descricaoValor)
        {
            if (linha == 0 || oportunidadeId == 0)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var linhaAnterior = _layoutPropostaRepositorio.ObterLayoutEdicaoPorLinha(oportunidadeId, linha);

            if (linhaAnterior != null)
            {
                var layout = LayoutFactory.NovoLayoutArmazenagemMinimoCIF(
                    linhaAnterior.ModeloId,
                    linha,
                    descricao,
                    valorCif.ToDecimal(),
                    valorMinimo.ToDecimal(),
                    valorMinimo20.ToDecimal(),
                    valorMinimo40.ToDecimal(),
                    tipoCarga,
                    linhaAnterior.ServicoId,
                    linhaAnterior.LinhaReferencia,
                    descricaoValor.RemoverCaracteresEspeciais(),
                    limiteBls.ToInt(),
                    linhaAnterior.Ocultar);

                var layoutTemp = LayoutFactory.NovoLayoutArmazenagemMinimoCIF(
                    linhaAnterior.ModeloId,
                    linhaAnterior.Linha,
                    linhaAnterior.Descricao,
                    linhaAnterior.ValorCIF,
                    linhaAnterior.ValorMinimo,
                    linhaAnterior.ValorMinimo20,
                    linhaAnterior.ValorMinimo40,
                    linhaAnterior.TipoCarga,
                    linhaAnterior.ServicoId,
                    linhaAnterior.LinhaReferencia,
                    linhaAnterior.DescricaoValor,
                    linhaAnterior.LimiteBls,
                    linhaAnterior.Ocultar);

                try
                {
                    _layoutPropostaRepositorio.AtualizarArmazenagemMinimoCIF(layout, linha, oportunidadeId);
                    GravarCamposAlterados(linha, oportunidadeId, layout, layoutTemp, string.Empty);
                }
                catch
                {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
                }
            }

            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        [HttpPost]
        public ActionResult AtualizarArmazenagemAllIn(
            int linha,
            int oportunidadeId,
            string descricao,
            BaseCalculo baseCalculo,
            Margem margem,
            int periodo,
            string descricaoPeriodo,
            string valor20,
            string valor40,
            string valorMinimo,
            string cifMinimo,
            string cifMaximo,
            string descricaoCif)
        {
            if (linha == 0 || oportunidadeId == 0)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var linhaAnterior = _layoutPropostaRepositorio.ObterLayoutEdicaoPorLinha(oportunidadeId, linha);

            if (linhaAnterior != null)
            {
                var layout = LayoutFactory.NovoLayoutArmazenagemAllIn(
                    linhaAnterior.ModeloId,
                    linha,
                    descricao,
                    valorMinimo.ToDecimal(),
                    valor20.ToDecimal(),
                    valor40.ToDecimal(),
                    cifMinimo.ToDecimal(),
                    cifMaximo.ToDecimal(),
                    descricaoCif,
                    linhaAnterior.ServicoId,
                    baseCalculo,
                    periodo,
                    descricaoPeriodo,
                    margem,
                    linhaAnterior.Moeda,
                    linhaAnterior.DescricaoValor,
                    linhaAnterior.TipoDocumentoId,
                    linhaAnterior.BaseExcesso,
                    linhaAnterior.ValorExcesso,
                    linhaAnterior.AdicionalPeso,
                    linhaAnterior.PesoLimite,
                    linhaAnterior.ProRata,
                    linhaAnterior.Ocultar);

                var layoutTemp = LayoutFactory.NovoLayoutArmazenagemAllIn(
                    linhaAnterior.ModeloId,
                    linhaAnterior.Linha,
                    linhaAnterior.Descricao,
                    linhaAnterior.ValorMinimo,
                    linhaAnterior.Valor20,
                    linhaAnterior.Valor40,
                    linhaAnterior.CifMinimo,
                    linhaAnterior.CifMaximo,
                    linhaAnterior.DescricaoCif,
                    linhaAnterior.ServicoId,
                    linhaAnterior.BaseCalculo,
                    linhaAnterior.Periodo,
                    linhaAnterior.DescricaoPeriodo,
                    linhaAnterior.Margem,
                    linhaAnterior.Moeda,
                    linhaAnterior.DescricaoValor,
                    linhaAnterior.TipoDocumentoId,
                    linhaAnterior.BaseExcesso,
                    linhaAnterior.ValorExcesso,
                    linhaAnterior.AdicionalPeso,
                    linhaAnterior.PesoLimite,
                    linhaAnterior.ProRata,
                    linhaAnterior.Ocultar);

                try
                {
                    _layoutPropostaRepositorio.AtualizarArmazenagemAllIn(layout, linha, oportunidadeId);
                    GravarCamposAlterados(linha, oportunidadeId, layout, layoutTemp, string.Empty);
                }
                catch
                {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
                }
            }

            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        [HttpPost]
        public ActionResult AtualizarServicoMargem(
          int linha,
          int oportunidadeId,
          string descricao,
          BaseCalculo baseCalculo,
          TipoCarga tipoCarga,
          string direita,
          string esquerda,
          string entreMargem,
          string imo,
          string exercito,
          string pesoMaximo,
          string adicionalPeso)
        {
            if (linha == 0 || oportunidadeId == 0)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var linhaAnterior = _layoutPropostaRepositorio.ObterLayoutEdicaoPorLinha(oportunidadeId, linha);

            if (linhaAnterior != null)
            {
                var layout = LayoutFactory.NovoLayoutServicoParaMargem(
                    linhaAnterior.ModeloId,
                    linha,
                    descricao,
                    linhaAnterior.ServicoId,
                    baseCalculo,
                    tipoCarga,
                    direita.ToDecimal(),
                    esquerda.ToDecimal(),
                    entreMargem.ToDecimal(),
                    imo.ToDecimal(),
                    exercito.ToDecimal(),
                    linhaAnterior.Moeda,
                    pesoMaximo.ToDecimal(),
                    adicionalPeso.ToDecimal(),
                    linhaAnterior.DescricaoValor,
                    linhaAnterior.TipoDocumentoId,
                    linhaAnterior.BaseExcesso,
                    linhaAnterior.ValorExcesso,
                    linhaAnterior.PesoLimite,
                    linhaAnterior.ProRata,
                    linhaAnterior.Ocultar);

                var layoutTemp = LayoutFactory.NovoLayoutServicoParaMargem(
                    linhaAnterior.ModeloId,
                    linhaAnterior.Linha,
                    linhaAnterior.Descricao,
                    linhaAnterior.ServicoId,
                    linhaAnterior.BaseCalculo,
                    linhaAnterior.TipoCarga,
                    linhaAnterior.ValorMargemDireita,
                    linhaAnterior.ValorMargemEsquerda,
                    linhaAnterior.ValorEntreMargens,
                    linhaAnterior.AdicionalIMO,
                    linhaAnterior.Exercito,
                    linhaAnterior.Moeda,
                    linhaAnterior.PesoMaximo,
                    linhaAnterior.AdicionalPeso,
                    linhaAnterior.DescricaoValor,
                    linhaAnterior.TipoDocumentoId,
                    linhaAnterior.BaseExcesso,
                    linhaAnterior.ValorExcesso,
                    linhaAnterior.PesoLimite,
                    linhaAnterior.ProRata,
                    linhaAnterior.Ocultar);

                try
                {
                    _layoutPropostaRepositorio.AtualizarServicoMargem(layout, linha, oportunidadeId);
                    GravarCamposAlterados(linha, oportunidadeId, layout, layoutTemp, string.Empty);
                }
                catch
                {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
                }
            }

            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        [HttpPost]
        public ActionResult AtualizarMinimoMargem(
          int linha,
          int oportunidadeId,
          string descricao,
          string direita,
          string esquerda,
          string entreMargem,
          string descricaoValor)
        {
            if (linha == 0 || oportunidadeId == 0)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var linhaAnterior = _layoutPropostaRepositorio.ObterLayoutEdicaoPorLinha(oportunidadeId, linha);

            if (linhaAnterior != null)
            {
                var layout = LayoutFactory.NovoLayoutMinimoParaMargem(
                    linhaAnterior.ModeloId,
                    linha,
                    descricao,
                    linhaAnterior.ServicoId,
                    direita.ToDecimal(),
                    esquerda.ToDecimal(),
                    entreMargem.ToDecimal(),
                    linhaAnterior.LinhaReferencia,
                    descricaoValor.RemoverCaracteresEspeciais(),
                    linhaAnterior.Ocultar);

                var layoutTemp = LayoutFactory.NovoLayoutMinimoParaMargem(
                    linhaAnterior.ModeloId,
                    linhaAnterior.Linha,
                    linhaAnterior.Descricao,
                    linhaAnterior.ServicoId,
                    linhaAnterior.ValorMargemDireita,
                    linhaAnterior.ValorMargemEsquerda,
                    linhaAnterior.ValorEntreMargens,
                    linhaAnterior.LinhaReferencia,
                    linhaAnterior.DescricaoValor,
                    linhaAnterior.Ocultar);

                try
                {
                    _layoutPropostaRepositorio.AtualizarMinimoMargem(layout, linha, oportunidadeId);
                    GravarCamposAlterados(linha, oportunidadeId, layout, layoutTemp, string.Empty);
                }
                catch
                {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
                }
            }

            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        [HttpPost]
        public ActionResult AtualizarServicoMecanicaManual(
            int linha,
            int oportunidadeId,
            string descricao,
            BaseCalculo baseCalculo,
            TipoCarga tipoCarga,
            string valor,
            string valor20,
            string valor40,
            string adicionalIMO,
            string exercito,
            string pesoMaximo,
            string adicionalPeso,
            TipoTrabalho tipoTrabalho,
            string descricaoValor)
        {
            if (linha == 0 || oportunidadeId == 0)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            if (tipoCarga == TipoCarga.CARGA_SOLTA || tipoCarga == TipoCarga.CARGA_BBK || tipoCarga == TipoCarga.CARGA_VEICULO)
            {
                valor20 = "0";
                valor40 = "0";
            }
            else
            {
                valor = "0";
            }

            var linhaAnterior = _layoutPropostaRepositorio.ObterLayoutEdicaoPorLinha(oportunidadeId, linha);

            if (linhaAnterior != null)
            {
                var layout = LayoutFactory.NovoLayoutServicoMecanicaManual(
                    linhaAnterior.ModeloId,
                    linha,
                    descricao,
                    valor.ToDecimal(),
                    valor20.ToDecimal(),
                    valor40.ToDecimal(),
                    linhaAnterior.ServicoId,
                    baseCalculo,
                    tipoCarga,
                    adicionalIMO.ToDecimal(),
                    exercito.ToDecimal(),
                    linhaAnterior.Moeda,
                    pesoMaximo.ToDecimal(),
                    adicionalPeso.ToDecimal(),
                    tipoTrabalho,
                    descricaoValor.RemoverCaracteresEspeciais(),
                    linhaAnterior.Ocultar);

                var layoutTemp = LayoutFactory.NovoLayoutServicoMecanicaManual(
                    linhaAnterior.ModeloId,
                    linhaAnterior.Linha,
                    linhaAnterior.Descricao,
                    linhaAnterior.Valor,
                    linhaAnterior.Valor20,
                    linhaAnterior.Valor40,
                    linhaAnterior.ServicoId,
                    linhaAnterior.BaseCalculo,
                    linhaAnterior.TipoCarga,
                    linhaAnterior.AdicionalIMO,
                    linhaAnterior.Exercito,
                    linhaAnterior.Moeda,
                    linhaAnterior.PesoMaximo,
                    linhaAnterior.AdicionalPeso,
                    linhaAnterior.TipoTrabalho,
                    linhaAnterior.DescricaoValor,
                    linhaAnterior.Ocultar);

                try
                {
                    _layoutPropostaRepositorio.AtualizarServicoMecanicaManual(layout, linha, oportunidadeId);
                    GravarCamposAlterados(linha, oportunidadeId, layout, layoutTemp, string.Empty);
                }
                catch
                {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
                }
            }

            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        [HttpPost]
        public ActionResult AtualizarMinimoMecanicaManual(
           int linha,
           int oportunidadeId,
           string descricao,
           string valorMinimo20,
           string valorMinimo40,
           string descricaoValor)
        {
            if (linha == 0 || oportunidadeId == 0)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var linhaAnterior = _layoutPropostaRepositorio.ObterLayoutEdicaoPorLinha(oportunidadeId, linha);

            if (linhaAnterior != null)
            {
                var layout = LayoutFactory.NovoLayoutMinimoMecanicaManual(
                    linhaAnterior.ModeloId,
                    linha,
                    descricao,
                    valorMinimo20.ToDecimal(),
                    valorMinimo40.ToDecimal(),
                    linhaAnterior.LinhaReferencia,
                    descricaoValor.RemoverCaracteresEspeciais(),
                    linhaAnterior.Ocultar);

                var layoutTemp = LayoutFactory.NovoLayoutMinimoMecanicaManual(
                    linhaAnterior.ModeloId,
                    linhaAnterior.Linha,
                    linhaAnterior.Descricao,
                    linhaAnterior.ValorMinimo20,
                    linhaAnterior.ValorMinimo40,
                    linhaAnterior.LinhaReferencia,
                    linhaAnterior.DescricaoValor,
                    linhaAnterior.Ocultar);

                try
                {
                    _layoutPropostaRepositorio.AtualizarMinimoMecanicaManual(layout, linha, oportunidadeId);
                    GravarCamposAlterados(linha, oportunidadeId, layout, layoutTemp, string.Empty);
                }
                catch
                {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
                }
            }

            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        [HttpPost]
        public ActionResult AtualizarServicoLiberacao(
          int linha,
          int oportunidadeId,
          string descricao,
          TipoCarga tipoCarga,
          string valor,
          string valor20,
          string valor40,
          string adicionalIMO,
          string exercito,
          string descricaoValor)
        {
            if (linha == 0 || oportunidadeId == 0)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var linhaAnterior = _layoutPropostaRepositorio.ObterLayoutEdicaoPorLinha(oportunidadeId, linha);

            if (linhaAnterior != null)
            {
                var layout = LayoutFactory.NovoLayoutServicoLiberacao(
                    linhaAnterior.ModeloId,
                    linha,
                    descricao,
                    valor.ToDecimal(),
                    valor20.ToDecimal(),
                    valor40.ToDecimal(),
                    tipoCarga,
                    linhaAnterior.ServicoId,
                    linhaAnterior.BaseCalculo,
                    linhaAnterior.Margem,
                    linhaAnterior.Reembolso,
                    linhaAnterior.Moeda,
                    descricaoValor.RemoverCaracteresEspeciais(),
                    linhaAnterior.TipoDocumentoId,
                    linhaAnterior.GrupoAtracacaoId,
                    adicionalIMO.ToDecimal(),
                    exercito.ToDecimal(),
                    linhaAnterior.Ocultar);

                var layoutTemp = LayoutFactory.NovoLayoutServicoLiberacao(
                    linhaAnterior.ModeloId,
                    linhaAnterior.Linha,
                    linhaAnterior.Descricao,
                    linhaAnterior.Valor,
                    linhaAnterior.Valor20,
                    linhaAnterior.Valor40,
                    linhaAnterior.TipoCarga,
                    linhaAnterior.ServicoId,
                    linhaAnterior.BaseCalculo,
                    linhaAnterior.Margem,
                    linhaAnterior.Reembolso,
                    linhaAnterior.Moeda,
                    linhaAnterior.DescricaoValor,
                    linhaAnterior.TipoDocumentoId,
                    linhaAnterior.GrupoAtracacaoId,
                    linhaAnterior.AdicionalIMO,
                    linhaAnterior.Exercito,
                    linhaAnterior.Ocultar);

                try
                {
                    _layoutPropostaRepositorio.AtualizarServicoLiberacao(layout, linha, oportunidadeId);
                    GravarCamposAlterados(linha, oportunidadeId, layout, layoutTemp, string.Empty);
                }
                catch
                {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
                }
            }

            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        [HttpPost]
        public ActionResult AtualizarServicosGerais(
          int linha,
          int oportunidadeId,
          string descricao,
          TipoCarga tipoCarga,
          string valor,
          string valor20,
          string valor40,
          string adicionalIMO,
          string exercito,
          BaseCalculo baseCalculo,
          string descricaoValor)
        {
            if (linha == 0 || oportunidadeId == 0)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            if (tipoCarga == TipoCarga.CARGA_SOLTA || tipoCarga == TipoCarga.CARGA_BBK || tipoCarga == TipoCarga.CARGA_VEICULO)
            {
                valor20 = "0";
                valor40 = "0";
            }
            else
            {
                valor = "0";
            }

            var linhaAnterior = _layoutPropostaRepositorio.ObterLayoutEdicaoPorLinha(oportunidadeId, linha);

            if (linhaAnterior != null)
            {
                var layout = LayoutFactory.NovoLayoutServicosGerais(
                    linhaAnterior.ModeloId,
                    linha,
                    descricao,
                    linhaAnterior.ServicoId,
                    valor.ToDecimal(),
                    valor20.ToDecimal(),
                    valor40.ToDecimal(),
                    adicionalIMO.ToDecimal(),
                    exercito.ToDecimal(),
                    tipoCarga,
                    baseCalculo,
                    linhaAnterior.Moeda,
                    descricaoValor.RemoverCaracteresEspeciais(),
                    linhaAnterior.TipoDocumentoId,
                    linhaAnterior.BaseExcesso,
                    linhaAnterior.ValorExcesso,
                    linhaAnterior.FormaPagamentoNVOCC,
                    linhaAnterior.Ocultar);

                var layoutTemp = LayoutFactory.NovoLayoutServicosGerais(
                    linhaAnterior.ModeloId,
                    linhaAnterior.Linha,
                    linhaAnterior.Descricao,
                    linhaAnterior.ServicoId,
                    linhaAnterior.Valor,
                    linhaAnterior.Valor20,
                    linhaAnterior.Valor40,
                    linhaAnterior.AdicionalIMO,
                    linhaAnterior.Exercito,
                    linhaAnterior.TipoCarga,
                    linhaAnterior.BaseCalculo,
                    linhaAnterior.Moeda,
                    linhaAnterior.DescricaoValor,
                    linhaAnterior.TipoDocumentoId,
                    linhaAnterior.BaseExcesso,
                    linhaAnterior.ValorExcesso,
                    linhaAnterior.FormaPagamentoNVOCC,
                    linhaAnterior.Ocultar);

                try
                {
                    _layoutPropostaRepositorio.AtualizarServicosGerais(layout, linha, oportunidadeId);
                    GravarCamposAlterados(linha, oportunidadeId, layout, layoutTemp, string.Empty);
                }
                catch
                {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
                }
            }

            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        [HttpPost]
        public ActionResult AtualizarPeriodoPadrao(
            int linha,
            int oportunidadeId,
            string descricao,
            TipoCarga tipoCarga,
            string valor,
            string valor20,
            string valor40,
            BaseCalculo baseCalculo,
            string descricaoValor)
        {
            if (linha == 0 || oportunidadeId == 0)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            if (tipoCarga == TipoCarga.CARGA_SOLTA || tipoCarga == TipoCarga.CARGA_BBK || tipoCarga == TipoCarga.CARGA_VEICULO)
            {
                valor20 = "0";
                valor40 = "0";
            }
            else
            {
                valor = "0";
            }

            var linhaAnterior = _layoutPropostaRepositorio.ObterLayoutEdicaoPorLinha(oportunidadeId, linha);

            if (linhaAnterior != null)
            {
                var layout = LayoutFactory.NovoLayoutPeriodoPadrao(
                    linhaAnterior.ModeloId,
                    linha,
                    descricao,
                    valor.ToDecimal(),
                    valor20.ToDecimal(),
                    valor40.ToDecimal(),
                    tipoCarga,
                    linhaAnterior.ServicoId,
                    baseCalculo,
                    linhaAnterior.QtdeDias,
                    linhaAnterior.Periodo,
                    descricaoValor.RemoverCaracteresEspeciais(),
                    linhaAnterior.Ocultar);

                var layoutTemp = LayoutFactory.NovoLayoutPeriodoPadrao(
                    linhaAnterior.ModeloId,
                    linhaAnterior.Linha,
                    linhaAnterior.Descricao,
                    linhaAnterior.Valor,
                    linhaAnterior.Valor20,
                    linhaAnterior.Valor40,
                    linhaAnterior.TipoCarga,
                    linhaAnterior.ServicoId,
                    linhaAnterior.BaseCalculo,
                    linhaAnterior.QtdeDias,
                    linhaAnterior.Periodo,
                    linhaAnterior.DescricaoValor,
                    linhaAnterior.Ocultar);

                try
                {
                    _layoutPropostaRepositorio.AtualizarPeriodoPadrao(layout, linha, oportunidadeId);
                    GravarCamposAlterados(linha, oportunidadeId, layout, layoutTemp, string.Empty);
                }
                catch
                {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
                }
            }

            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        [HttpPost]
        public ActionResult AtualizarHubPort(
          int linha,
          int oportunidadeId,
          string descricao,
          int origem,
          int destino,
          string valor,
          string descricaoValor,
          int formaPgto)
        {
            if (linha == 0 || oportunidadeId == 0)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var linhaAnterior = _layoutPropostaRepositorio.ObterLayoutEdicaoPorLinha(oportunidadeId, linha);

            if (linhaAnterior != null)
            {
                var layout = LayoutFactory.NovoLayoutServicoHubPort(
                    linhaAnterior.ModeloId,
                    linha,
                    descricao,
                    linhaAnterior.ServicoId,
                    linhaAnterior.BaseCalculo,
                    valor.ToDecimal(),
                    origem,
                    destino,
                    linhaAnterior.Moeda,
                    (FormaPagamento)formaPgto,
                    descricaoValor.RemoverCaracteresEspeciais(),
                    linhaAnterior.Ocultar);

                var layoutTemp = LayoutFactory.NovoLayoutServicoHubPort(
                    linhaAnterior.ModeloId,
                    linhaAnterior.Linha,
                    linhaAnterior.Descricao,
                    linhaAnterior.ServicoId,
                    linhaAnterior.BaseCalculo,
                    linhaAnterior.Valor,
                    linhaAnterior.Origem,
                    linhaAnterior.Destino,
                    linhaAnterior.Moeda,
                    linhaAnterior.FormaPagamentoNVOCC,
                    linhaAnterior.DescricaoValor,
                    linhaAnterior.Ocultar);

                try
                {
                    _layoutPropostaRepositorio.AtualizarHubPort(layout, linha, oportunidadeId);
                    GravarCamposAlterados(linha, oportunidadeId, layout, layoutTemp, string.Empty);
                }
                catch
                {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
                }
            }

            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        [HttpPost]
        public ActionResult AtualizarMinimoGeral(
          int linha,
          int oportunidadeId,
          string descricao,
          TipoCarga tipoCarga,
          BaseCalculo baseCalculo,
          string valorMin,
          string valorMin20,
          string valorMin40,
          string descricaoValor)
        {
            if (linha == 0 || oportunidadeId == 0)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var linhaAnterior = _layoutPropostaRepositorio.ObterLayoutEdicaoPorLinha(oportunidadeId, linha);

            if (linhaAnterior != null)
            {
                var layout = LayoutFactory.NovoLayoutMinimoGeral(
                    linhaAnterior.ModeloId,
                    linha,
                    descricao,
                    valorMin.ToDecimal(),
                    valorMin20.ToDecimal(),
                    valorMin40.ToDecimal(),
                    tipoCarga,
                     baseCalculo,
                    linhaAnterior.LinhaReferencia,
                    descricaoValor.RemoverCaracteresEspeciais(),
                    linhaAnterior.Ocultar);

                var layoutTemp = LayoutFactory.NovoLayoutMinimoGeral(
                    linhaAnterior.ModeloId,
                    linhaAnterior.Linha,
                    linhaAnterior.Descricao,
                    linhaAnterior.ValorMinimo,
                    linhaAnterior.ValorMinimo20,
                    linhaAnterior.ValorMinimo40,
                    linhaAnterior.TipoCarga,
                    linhaAnterior.BaseCalculo,
                    linhaAnterior.LinhaReferencia,
                    linhaAnterior.DescricaoValor,
                    linhaAnterior.Ocultar);

                try
                {
                    _layoutPropostaRepositorio.AtualizarMinimoGeral(layout, linha, oportunidadeId);
                    GravarCamposAlterados(linha, oportunidadeId, layout, layoutTemp, string.Empty);
                }
                catch
                {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
                }
            }

            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AtualizarCondicaoInicial(
          int linha,
          int oportunidadeId,
          string descricao)
        {
            if (linha == 0 || oportunidadeId == 0)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var linhaAnterior = _layoutPropostaRepositorio.ObterLayoutEdicaoPorLinha(oportunidadeId, linha);

            if (linhaAnterior != null)
            {
                var layout = LayoutFactory.NovoLayoutCondicoesIniciais(0, linha, string.Empty, descricao, linhaAnterior.Ocultar);

                try
                {
                    _layoutPropostaRepositorio.AtualizarCondicoesIniciais(layout, linha, oportunidadeId);

                    if (linhaAnterior.CondicoesIniciais != descricao)
                    {
                        _layoutPropostaRepositorio.GravarCampoPropostaAlterado(new OportunidadeAlteracaoLinhaProposta
                        {
                            OportunidadeId = oportunidadeId,
                            Linha = linha,
                            UsuarioId = User.ObterId(),
                            Propriedade = "CondicoesIniciais"
                        });
                    }
                }
                catch
                {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
                }
            }

            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AtualizarCondicaoGeral(
          int linha,
          int oportunidadeId,
          string descricao)
        {
            if (linha == 0 || oportunidadeId == 0)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var linhaAnterior = _layoutPropostaRepositorio.ObterLayoutEdicaoPorLinha(oportunidadeId, linha);

            if (linhaAnterior != null)
            {
                var layout = LayoutFactory.NovoLayoutCondicoesGerais(0, linha, string.Empty, descricao, linhaAnterior.Ocultar);

                try
                {
                    _layoutPropostaRepositorio.AtualizarCondicoesGerais(layout, linha, oportunidadeId);

                    if (linhaAnterior.CondicoesGerais != descricao)
                    {
                        _layoutPropostaRepositorio.GravarCampoPropostaAlterado(new OportunidadeAlteracaoLinhaProposta
                        {
                            OportunidadeId = oportunidadeId,
                            Linha = linha,
                            UsuarioId = User.ObterId(),
                            Propriedade = "CondicoesGerais"
                        });
                    }
                }
                catch
                {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
                }
            }

            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        [HttpPost]
        public ActionResult ExcluirLinhaProposta(int oportunidadeId, int linha)
        {
            try
            {
                var linhaBusca = _layoutPropostaRepositorio.ObterLayoutEdicaoPorLinha(oportunidadeId, linha);

                if (linhaBusca == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Linha não encontrada ou já excluída");

                _layoutPropostaRepositorio.ExcluirLinhaOportunidadeProposta(linhaBusca.OportunidadeId, linhaBusca.Linha);
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        private void GravarCamposAlterados(int linha, int oportunidadeId, object layout, object layoutTemp, string campo)
        {
            if (string.IsNullOrEmpty(campo))
            {
                foreach (var propriedade in layout.ObterPropriedades())
                {
                    ObjetoHelpers.GetValue(layoutTemp, propriedade, out object valorAntigo);
                    ObjetoHelpers.GetValue(layout, propriedade, out object valorNovo);

                    if (valorAntigo?.ToString().Trim() != valorNovo?.ToString().Trim())
                    {
                        _layoutPropostaRepositorio.GravarCampoPropostaAlterado(new OportunidadeAlteracaoLinhaProposta
                        {
                            OportunidadeId = oportunidadeId,
                            Linha = linha,
                            UsuarioId = User.ObterId(),
                            Propriedade = propriedade
                        });
                    }
                }
            }
            else
            {
                var propriedades = layout.ObterPropriedades().Where(c => c == campo);

                foreach (var propriedade in propriedades)
                {
                    ObjetoHelpers.GetValue(layoutTemp, propriedade, out object valorAntigo);
                    ObjetoHelpers.GetValue(layout, propriedade, out object valorNovo);

                    if (valorAntigo?.ToString().Trim() != valorNovo?.ToString().Trim())
                    {
                        _layoutPropostaRepositorio.GravarCampoPropostaAlterado(new OportunidadeAlteracaoLinhaProposta
                        {
                            OportunidadeId = oportunidadeId,
                            Linha = linha,
                            UsuarioId = User.ObterId(),
                            Propriedade = propriedade
                        });
                    }
                }
            }
        }

        [HttpPost]
        public ActionResult CadastrarFaixasBL(FaixasBLViewModel viewModel)
        {
            var faixaBL = new FaixaBL
            {
                LayoutId = viewModel.FaixaBLLayoutId,
                BLMinimo = viewModel.FaixasBLMinimo,
                BLMaximo = viewModel.FaixasBLMaximo,
                ValorMinimo = viewModel.FaixasBLValorMinimo
            };

            if (!Validar(faixaBL))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest,
                    faixaBL.ValidationResult
                        .Errors
                        .First()
                        .ToString());
            }

            _layoutPropostaRepositorio.CadastrarFaixasBL(faixaBL);

            var faixas = ObterFaixasBL(viewModel.FaixaBLLayoutId);

            return PartialView("_ConsultaFaixasBL", faixas);
        }

        [HttpPost]
        public ActionResult ExcluirFaixaBL(int id)
        {
            try
            {
                var faixaBLFBusca = _layoutPropostaRepositorio.ObterFaixaBLPorId(id);

                if (faixaBLFBusca == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Registro não encontrado ou já excluído");

                _layoutPropostaRepositorio.ExcluirFaixaBL(id);

                var faixas = ObterFaixasBL(faixaBLFBusca.OportunidadeLayoutId);

                return PartialView("_ConsultaFaixasBL", faixas);
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        private IEnumerable<FaixaBL> ObterFaixasBL(int oportunidadeId)
            => _layoutPropostaRepositorio.ObterFaixasBL(oportunidadeId);

        public PartialViewResult ObterFaixasBLJson(int oportunidadeId)
         => PartialView("_ConsultaFaixasBL", ObterFaixasBL(oportunidadeId));

        [HttpPost]
        public ActionResult CadastrarFaixasCIF(FaixasCIFViewModel viewModel)
        {
            var faixaCIF = new FaixaCIF
            {
                LayoutId = viewModel.FaixaCIFLayoutId,
                Minimo = viewModel.FaixasCIFMinimo,
                Maximo = viewModel.FaixasCIFMaximo,
                Valor20 = viewModel.FaixasCIFValor20,
                Valor40 = viewModel.FaixasCIFValor40,
                Margem = viewModel.FaixasCIFMargem,
                Descricao = viewModel.FaixasCIFDescricao
            };

            if (!Validar(faixaCIF))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest,
                    faixaCIF.ValidationResult
                        .Errors
                        .First()
                        .ToString());
            }

            _layoutPropostaRepositorio.CadastrarFaixasCIF(faixaCIF);

            var faixas = ObterFaixasCIF(viewModel.FaixaCIFLayoutId);

            return PartialView("_ConsultaFaixasCIF", faixas);
        }

        [HttpPost]
        public ActionResult ExcluirFaixaCIF(int id)
        {
            try
            {
                var faixaCIFBusca = _layoutPropostaRepositorio.ObterFaixaCIFPorId(id);

                if (faixaCIFBusca == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Registro não encontrado ou já excluído");

                _layoutPropostaRepositorio.ExcluirFaixaCIF(id);

                var faixas = ObterFaixasCIF(faixaCIFBusca.OportunidadeLayoutId);

                return PartialView("_ConsultaFaixasCIF", faixas);
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        private IEnumerable<FaixaCIF> ObterFaixasCIF(int oportunidadeId)
            => _layoutPropostaRepositorio.ObterFaixasCIF(oportunidadeId);

        public PartialViewResult ObterFaixasCIFJson(int oportunidadeId)
         => PartialView("_ConsultaFaixasCIF", ObterFaixasCIF(oportunidadeId));

        [HttpPost]
        public ActionResult CadastrarFaixasPeso(FaixasPesoViewModel viewModel)
        {
            var faixaPeso = new FaixaPeso
            {
                LayoutId = viewModel.FaixaPesoLayoutId,
                ValorInicial = viewModel.FaixasPesoValorInicial,
                ValorFinal = viewModel.FaixasPesoValorFinal,
                Preco = viewModel.FaixasPesoPreco
            };

            if (!Validar(faixaPeso))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest,
                    faixaPeso.ValidationResult
                        .Errors
                        .First()
                        .ToString());
            }

            _layoutPropostaRepositorio.CadastrarFaixasPeso(faixaPeso);

            var faixas = ObterFaixasPeso(viewModel.FaixaPesoLayoutId);

            return PartialView("_ConsultaFaixasPeso", faixas);
        }

        [HttpPost]
        public ActionResult ExcluirFaixaPeso(int id)
        {
            try
            {
                var faixaPesoBusca = _layoutPropostaRepositorio.ObterFaixaPesoPorId(id);

                if (faixaPesoBusca == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Registro não encontrado ou já excluído");

                _layoutPropostaRepositorio.ExcluirFaixaPeso(id);

                var faixas = ObterFaixasPeso(faixaPesoBusca.OportunidadeLayoutId);

                return PartialView("_ConsultaFaixasPeso", faixas);
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        private IEnumerable<FaixaPeso> ObterFaixasPeso(int oportunidadeId)
            => _layoutPropostaRepositorio.ObterFaixasPeso(oportunidadeId);

        public PartialViewResult ObterFaixasPesoJson(int oportunidadeId)
        => PartialView("_ConsultaFaixasPeso", ObterFaixasPeso(oportunidadeId));

        [HttpPost]
        public ActionResult CadastrarFaixasVolume(FaixasVolumeViewModel viewModel)
        {
            var faixaVolume = new FaixaVolume
            {
                LayoutId = viewModel.FaixaVolumeLayoutId,
                ValorInicial = viewModel.FaixasVolumeValorInicial,
                ValorFinal = viewModel.FaixasVolumeValorFinal,
                Preco = viewModel.FaixasVolumePreco
            };

            if (!Validar(faixaVolume))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest,
                    faixaVolume.ValidationResult
                        .Errors
                        .First()
                        .ToString());
            }

            _layoutPropostaRepositorio.CadastrarFaixasVolume(faixaVolume);

            var faixas = ObterFaixasVolume(viewModel.FaixaVolumeLayoutId);

            return PartialView("_ConsultaFaixasVolume", faixas);
        }

        [HttpPost]
        public ActionResult ExcluirFaixaVolume(int id)
        {
            try
            {
                var faixaVolumeBusca = _layoutPropostaRepositorio.ObterFaixasVolumePorId(id);

                if (faixaVolumeBusca == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Registro não encontrado ou já excluído");

                _layoutPropostaRepositorio.ExcluirFaixaVolume(id);

                var faixas = ObterFaixasVolume(faixaVolumeBusca.OportunidadeLayoutId);

                return PartialView("_ConsultaFaixasVolume", faixas);
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        private IEnumerable<FaixaVolume> ObterFaixasVolume(int oportunidadeId)
            => _layoutPropostaRepositorio.ObterFaixasVolume(oportunidadeId);

        public PartialViewResult ObterFaixasVolumeJson(int oportunidadeId)
       => PartialView("_ConsultaFaixasVolume", ObterFaixasVolume(oportunidadeId));
    }
}