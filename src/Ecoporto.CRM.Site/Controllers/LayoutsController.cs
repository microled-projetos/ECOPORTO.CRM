using Ecoporto.CRM.Business.Enums;
using Ecoporto.CRM.Business.Extensions;
using Ecoporto.CRM.Business.Factory;
using Ecoporto.CRM.Business.Interfaces.Repositorios;
using Ecoporto.CRM.Business.Models;
using Ecoporto.CRM.Site.Filtros;
using Ecoporto.CRM.Site.Models;
using Ecoporto.CRM.Site.Services;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Ecoporto.CRM.Site.Controllers
{
    [Authorize]
    public class LayoutsController : BaseController
    {
        private readonly ILayoutRepositorio _layoutRepositorio;
        private readonly ILayoutPropostaRepositorio _layoutPropostaRepositorio;
        private readonly IModeloRepositorio _modeloRepositorio;
        private readonly IServicoRepositorio _servicoRepositorio;
        private readonly IHubPortRepositorio _hubPortRepositorio;
        private readonly IOportunidadeRepositorio _oportunidadeRepositorio;
        private readonly IDocumentoRepositorio _documentoRepositorio;
        private readonly IGrupoAtracacaoRepositorio _grupoAtracacaoRepositorio;

        public LayoutsController(
            ILayoutRepositorio layoutRepositorio,
            ILayoutPropostaRepositorio layoutPropostaRepositorio,
            IModeloRepositorio modeloRepositorio,
            IServicoRepositorio servicoRepositorio,
            IHubPortRepositorio hubPortRepositorio,
            IOportunidadeRepositorio oportunidadeRepositorio,
            IDocumentoRepositorio documentoRepositorio,
            IGrupoAtracacaoRepositorio grupoAtracacaoRepositorio,
            ILogger logger) : base(logger)
        {
            _layoutRepositorio = layoutRepositorio;
            _layoutPropostaRepositorio = layoutPropostaRepositorio;
            _modeloRepositorio = modeloRepositorio;
            _servicoRepositorio = servicoRepositorio;
            _hubPortRepositorio = hubPortRepositorio;
            _oportunidadeRepositorio = oportunidadeRepositorio;
            _documentoRepositorio = documentoRepositorio;
            _grupoAtracacaoRepositorio = grupoAtracacaoRepositorio;
        }

        [HttpGet]
        [CanActivate(Roles = "Layouts:Acessar")]
        public ActionResult Index()
        {
            var modelos = _modeloRepositorio.ObterModelosAtivos();
            var servicos = _servicoRepositorio.ObterServicos();
            var clienteHubPort = _hubPortRepositorio.ObterClientesHubPort();
            var tiposDocumentos = _documentoRepositorio.ObterTiposDocumentos();
            var gruposAtracacao = _grupoAtracacaoRepositorio.ObterGruposAtracacao();

            return View(new LayoutViewModel
            {
                Modelos = modelos,
                Servicos = servicos,
                ClientesHubPort = clienteHubPort,
                TiposDocumentos = tiposDocumentos,
                GruposAtracacao = gruposAtracacao
            });
        }

        public ActionResult RetornarErros()
        {
            Response.StatusCode = (int)HttpStatusCode.BadRequest;

            return Json(new
            {
                erros = ModelState.Values.SelectMany(v => v.Errors)
            }, JsonRequestBehavior.AllowGet);
        }

        private void AtualizarLinhas(Cabecalho cabecalho)
        {
            if (_layoutRepositorio.LinhaJaCadastrada(cabecalho))
            {
                if (_layoutRepositorio.ExistemLinhasPosteriores(cabecalho))
                    _layoutRepositorio.AtualizarLinhasPosteriores(cabecalho);
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult CadastrarLayout(LayoutViewModel viewModel)
        {
            var cabecalho = new Cabecalho(viewModel.ModeloId, viewModel.Linha, viewModel.Descricao, viewModel.TipoRegistro, viewModel.Ocultar);

            try
            {
                switch (viewModel.TipoRegistro)
                {
                    case TipoRegistro.CONDICAO_INICIAL:

                        var layoutCondicoesIniciais = LayoutFactory.NovoLayoutCondicoesIniciais(
                            viewModel.ModeloId,
                            viewModel.Linha,
                            viewModel.Descricao,
                            viewModel.CondicoesIniciais,
                            viewModel.Ocultar);

                        if (!Validar(layoutCondicoesIniciais))
                            return RetornarErros();

                        AtualizarLinhas(cabecalho);

                        _layoutRepositorio.CadastrarLayoutCondicoesIniciais(layoutCondicoesIniciais);

                        break;

                    case TipoRegistro.TITULO_MASTER:

                        var layoutTituloMaster = new LayoutTituloMaster(cabecalho);

                        if (!Validar(layoutTituloMaster))
                            return RetornarErros();

                        AtualizarLinhas(cabecalho);

                        _layoutRepositorio.CadastrarLayoutTituloMaster(layoutTituloMaster);

                        break;

                    case TipoRegistro.TITULO:

                        var layoutTitulo = new LayoutTitulo(cabecalho);

                        if (!Validar(layoutTitulo))
                            return RetornarErros();

                        AtualizarLinhas(cabecalho);

                        _layoutRepositorio.CadastrarLayoutTitulo(layoutTitulo);

                        break;

                    case TipoRegistro.SUB_TITULO:

                        var layoutSubTitulo = new LayoutSubTitulo(cabecalho);

                        if (!Validar(layoutSubTitulo))
                            return RetornarErros();

                        AtualizarLinhas(cabecalho);

                        _layoutRepositorio.CadastrarLayoutSubTitulo(layoutSubTitulo);

                        break;

                    case TipoRegistro.SUB_TITULO_MARGEM:
                    case TipoRegistro.SUB_TITULO_MARGEM_D_E:

                        var layoutSubTituloMargem = new LayoutSubTituloMargem(cabecalho);

                        if (!Validar(layoutSubTituloMargem))
                            return RetornarErros();

                        AtualizarLinhas(cabecalho);

                        _layoutRepositorio.CadastrarLayoutSubTituloMargem(layoutSubTituloMargem);

                        break;

                    case TipoRegistro.SUB_TITULO_ALL_IN:

                        var layoutSubAllIn = new LayoutSubTituloAllIn(cabecalho);

                        if (!Validar(layoutSubAllIn))
                            return RetornarErros();

                        AtualizarLinhas(cabecalho);

                        _layoutRepositorio.CadastrarLayoutSubTituloAllIn(layoutSubAllIn);

                        break;

                    case TipoRegistro.ARMAZENAGEM:

                        var layoutArmazenagem = LayoutFactory.NovoLayoutArmazenagem(
                            viewModel.ModeloId,
                            viewModel.Linha,
                            viewModel.Descricao,
                            viewModel.ValorArmazenagem,
                            viewModel.Valor20Armazenagem,
                            viewModel.Valor40Armazenagem,
                            viewModel.TipoCargaArmazenagem,
                            viewModel.ServicoId,
                            viewModel.BaseCalculoArmazenagem,
                            viewModel.QtdeDiasArmazenagem,
                            viewModel.AdicionalArmazenagem,
                            viewModel.AdicionalGRC,
                            viewModel.MinimoGRC,
                            viewModel.AdicionalIMOArmazenagem,
                            viewModel.ExercitoArmazenagem,
                            viewModel.AdicionalIMOGRC,
                            viewModel.ValorANVISA,
                            viewModel.AnvisaGRC,
                            viewModel.PeriodoArmazenagem,
                            viewModel.MoedaArmazenagem,
                            viewModel.DescricaoValorArmazenagem,
                            viewModel.TipoDocumentoArmazenagem,
                            viewModel.BaseExcessoArmazenagem,
                            viewModel.MargemArmazenagem,
                            viewModel.ValorExcessoArmazenagem,
                            viewModel.AcrescimoPesoArmazenagem,
                            viewModel.PesoLimiteArmazenagem,
                            viewModel.GrupoAtracacaoArmazenagem,
                            viewModel.ProRataArmazenagem,
                            viewModel.Ocultar);

                        if (!Validar(layoutArmazenagem))
                            return RetornarErros();

                        AtualizarLinhas(cabecalho);

                        _layoutRepositorio.CadastrarLayoutArmazenagem(layoutArmazenagem);

                        break;

                    case TipoRegistro.ARMAZENAGEM_CIF:

                        var layoutArmazenagemCIF = LayoutFactory.NovoLayoutArmazenagemCIF(
                            viewModel.ModeloId,
                            viewModel.Linha,
                            viewModel.Descricao,
                            viewModel.CIFArmazenagemCIF,
                            viewModel.ValorArmazenagemCIF,
                            viewModel.Valor20ArmazenagemCIF,
                            viewModel.Valor40ArmazenagemCIF,
                            viewModel.TipoCargaArmazenagemCIF,
                            viewModel.ServicoId,
                            viewModel.BaseCalculoArmazenagemCIF,
                            viewModel.QtdeDiasArmazenagemCIF,
                            viewModel.AdicionalArmazenagemCIF,
                            viewModel.AdicionalGRCCIF,
                            viewModel.MinimoGRCCIF,
                            viewModel.AdicionalIMOArmazenagemCIF,
                            viewModel.ExercitoArmazenagemCIF,
                            viewModel.AdicionalIMOGRCCIF,
                            viewModel.ValorANVISACIF,
                            viewModel.AnvisaGRCCIF,
                            viewModel.PeriodoArmazenagemCIF,
                            viewModel.MoedaArmazenagemCIF,
                            viewModel.DescricaoValorArmazenagemCIF,
                            viewModel.TipoDocumentoArmazenagemCIF,
                            viewModel.BaseExcessoArmazenagemCIF,
                            viewModel.MargemArmazenagemCIF,
                            viewModel.ValorExcessoArmazenagemCIF,
                            viewModel.AcrescimoPesoArmazenagemCIF,
                            viewModel.PesoLimiteArmazenagemCIF,
                            viewModel.ProRataArmazenagemCIF,
                            viewModel.Ocultar);

                        if (!Validar(layoutArmazenagemCIF))
                            return RetornarErros();

                        AtualizarLinhas(cabecalho);

                        _layoutRepositorio.CadastrarLayoutArmazenagemCIF(layoutArmazenagemCIF);

                        break;

                    case TipoRegistro.ARMAZENAGEM_MINIMO:

                        var layoutArmazenagemMinimo = LayoutFactory.NovoLayoutArmazenagemMinimo(
                            viewModel.ModeloId,
                            viewModel.Linha,
                            viewModel.Descricao,
                            viewModel.ValorMinimoArmazenagemMin,
                            viewModel.ValorMinimo20ArmazenagemMin,
                            viewModel.ValorMinimo40ArmazenagemMin,
                            viewModel.TipoCargaArmazenagemMinimo,
                            viewModel.MargemArmazenagemMinimo,
                            viewModel.ServicoId,
                            viewModel.LinhaReferenciaArmazenagemMin,
                            viewModel.DescricaoValorArmazenagemMin,
                            viewModel.LimiteBls,
                            viewModel.Ocultar);

                        if (!Validar(layoutArmazenagemMinimo))
                            return RetornarErros();

                        AtualizarLinhas(cabecalho);

                        layoutArmazenagemMinimo.Id = _layoutRepositorio.CadastrarLayoutArmazenagemMinimo(layoutArmazenagemMinimo);

                        break;

                    case TipoRegistro.ARMAZENAGEM_MINIMO_CIF:

                        var layoutArmazenagemMinimoCIF = LayoutFactory.NovoLayoutArmazenagemMinimoCIF(
                            viewModel.ModeloId,
                            viewModel.Linha,
                            viewModel.Descricao,
                            viewModel.CIFArmazenagemMinimoCIF,
                            viewModel.ValorMinimoArmazenagemMinCIF,
                            viewModel.ValorMinimo20ArmazenagemMinCIF,
                            viewModel.ValorMinimo40ArmazenagemMinCIF,
                            viewModel.TipoCargaArmazenagemMinimoCIF,
                            viewModel.ServicoId,
                            viewModel.LinhaReferenciaArmazenagemMinCIF,
                            viewModel.DescricaoValorArmazenagemMinCIF,
                            viewModel.LimiteBlsCIF,
                            viewModel.Ocultar);

                        if (!Validar(layoutArmazenagemMinimoCIF))
                            return RetornarErros();

                        AtualizarLinhas(cabecalho);

                        layoutArmazenagemMinimoCIF.Id = _layoutRepositorio.CadastrarLayoutArmazenagemMinimoCIF(layoutArmazenagemMinimoCIF);

                        break;

                    case TipoRegistro.ARMAZENAMEM_ALL_IN:

                        var layoutArmazenagemAllIn = LayoutFactory.NovoLayoutArmazenagemAllIn(
                            viewModel.ModeloId,
                            viewModel.Linha,
                            viewModel.Descricao,
                            viewModel.ValorMinimoAllIn,
                            viewModel.Valor20ArmazenagemAllIn,
                            viewModel.Valor40ArmazenagemAllIn,
                            viewModel.CIFMinimoAllIn,
                            viewModel.CIFMaximoAllIn,
                            viewModel.DescricaoCifAllIn,
                            viewModel.ServicoId,
                            viewModel.BaseCalculoArmazenagemAllIn,
                            viewModel.PeriodoArmazenagemAllIn,
                            viewModel.DescricaoPeriodoAllIn,
                            viewModel.MargemArmAllIn,
                            viewModel.MoedaArmazenagemAllIn,
                            viewModel.DescricaoValorArmazenagemAllIn,
                            viewModel.TipoDocumentoAllIn,
                            viewModel.BaseExcessoAllIn,
                            viewModel.ValorExcessoAllIn,
                            viewModel.AcrescimoPesoAllIn,
                            viewModel.PesoLimiteAllIn,
                            viewModel.ProRataAllIn,
                            viewModel.Ocultar);

                        if (!Validar(layoutArmazenagemAllIn))
                            return RetornarErros();

                        AtualizarLinhas(cabecalho);

                        layoutArmazenagemAllIn.Id = _layoutRepositorio.CadastrarLayoutArmazenagemAllIn(layoutArmazenagemAllIn);

                        break;

                    case TipoRegistro.SERVIÇO_PARA_MARGEM:

                        var layoutServicoParaMargem = LayoutFactory.NovoLayoutServicoParaMargem(
                            viewModel.ModeloId,
                            viewModel.Linha,
                            viewModel.Descricao,
                            viewModel.ServicoId,
                            viewModel.BaseCalculoServMargem,
                            viewModel.TipoCargaServMargem,
                            viewModel.ValorMargemDireitaServMargem,
                            viewModel.ValorMargemEsquerdaServMargem,
                            viewModel.ValorEntreMargensServMargem,
                            viewModel.AdicionalIMOServMargem,
                            viewModel.ExercitoServMargem,
                            viewModel.MoedaServMargem,
                            viewModel.PesoMaximoServicoParaMargem,
                            viewModel.AdicionalPesoServicoParaMargem,
                            viewModel.DescricaoValorServMargem,
                            viewModel.TipoDocumentoServMargem,
                            viewModel.BaseExcessoServMargem,
                            viewModel.ValorExcessoServMargem,
                            viewModel.PesoLimiteServMargem,
                            viewModel.ProRataServMargem,
                            viewModel.Ocultar);

                        if (!Validar(layoutServicoParaMargem))
                            return RetornarErros();

                        AtualizarLinhas(cabecalho);

                        var id = _layoutRepositorio.CadastrarLayoutServicoParaMargem(layoutServicoParaMargem);

                        break;

                    case TipoRegistro.MINIMO_PARA_MARGEM:

                        var layoutMinimoParaMargem = LayoutFactory.NovoLayoutMinimoParaMargem(
                            viewModel.ModeloId,
                            viewModel.Linha,
                            viewModel.Descricao,
                            viewModel.ServicoId,
                            viewModel.ValorMinimoMargemDireita,
                            viewModel.ValorMinimoMargemEsquerda,
                            viewModel.ValorMinimoEntreMargens,
                            viewModel.LinhaReferenciaMinimoParaMargem,
                            viewModel.DescricaoValorMinimoMargem,
                            viewModel.Ocultar);

                        if (!Validar(layoutMinimoParaMargem))
                            return RetornarErros();

                        AtualizarLinhas(cabecalho);

                        _layoutRepositorio.CadastrarLayoutMinimoParaMargem(layoutMinimoParaMargem);

                        break;

                    case TipoRegistro.SERVICO_MECANICA_MANUAL:

                        var layoutServicoMecanicaManual = LayoutFactory.NovoLayoutServicoMecanicaManual(
                            viewModel.ModeloId,
                            viewModel.Linha,
                            viewModel.Descricao,
                            viewModel.ValorServMecManual,
                            viewModel.Valor20ServMecManual,
                            viewModel.Valor40ServMecManual,
                            viewModel.ServicoId,
                            viewModel.BaseCalculoServMecManual,
                            viewModel.TipoCargaServMecManual,
                            viewModel.AdicionalIMOServMecManual,
                            viewModel.ExercitoServMecManual,
                            viewModel.MoedaServMecMan,
                            viewModel.PesoMaximoServicoMecanicaManual,
                            viewModel.AdicionalPesoServicoMecanicaManual,
                            viewModel.TipoTrabalho,
                            viewModel.DescricaoValorServMecManual,
                            viewModel.Ocultar);

                        if (!Validar(layoutServicoMecanicaManual))
                            return RetornarErros();

                        AtualizarLinhas(cabecalho);

                        _layoutRepositorio.CadastrarLayoutServicoMecanicaManual(layoutServicoMecanicaManual);

                        break;

                    case TipoRegistro.MINIMO_MECANICA_MANUAL:

                        var layoutMinimoMecanicaManual = LayoutFactory.NovoLayoutMinimoMecanicaManual(
                            viewModel.ModeloId,
                            viewModel.Linha,
                            viewModel.Descricao,
                            viewModel.ValorMinimo20MecManual,
                            viewModel.ValorMinimo40MecManual,
                            viewModel.LinhaReferenciaMinimoMecanicaManual,
                            viewModel.DescricaoValorMinimoMecManual,
                            viewModel.Ocultar);

                        if (!Validar(layoutMinimoMecanicaManual))
                            return RetornarErros();

                        AtualizarLinhas(cabecalho);

                        _layoutRepositorio.CadastrarLayoutMinimoMecanicaManual(layoutMinimoMecanicaManual);

                        break;

                    case TipoRegistro.SERVICO_LIBERACAO:

                        var layoutServicoLiberacao = LayoutFactory.NovoLayoutServicoLiberacao(
                            viewModel.ModeloId,
                            viewModel.Linha,
                            viewModel.Descricao,
                            viewModel.ValorServLib,
                            viewModel.Valor20ServLib,
                            viewModel.Valor40ServLib,
                            viewModel.TipoCargaServLib,
                            viewModel.ServicoId,
                            viewModel.BaseCalculoServLib,
                            viewModel.MargemServLib,
                            viewModel.Reembolso,
                            viewModel.MoedaServLib,
                            viewModel.DescricaoValorServLib,
                            viewModel.TipoDocumentoServLib,
                            viewModel.GrupoAtracacaoServLiv,
                            viewModel.AdicionalIMOServLib,
                            viewModel.ExercitoServLib,
                            viewModel.Ocultar);

                        if (!Validar(layoutServicoLiberacao))
                            return RetornarErros();

                        AtualizarLinhas(cabecalho);

                        _layoutRepositorio.CadastrarLayoutServicoLiberacao(layoutServicoLiberacao);

                        break;

                    case TipoRegistro.SERVICO_HUBPORT:

                        var layoutServicoHubPort = LayoutFactory.NovoLayoutServicoHubPort(
                            viewModel.ModeloId,
                            viewModel.Linha,
                            viewModel.Descricao,
                            viewModel.ServicoId,
                            viewModel.BaseCalculoHubPort,
                            viewModel.ValorHubPort,
                            viewModel.Origem,
                            viewModel.Destino,
                            viewModel.MoedaHubPort,
                            viewModel.FormaPagamentoNVOCCHubPort,
                            viewModel.DescricaoValorHubPort,
                            viewModel.Ocultar);

                        if (!Validar(layoutServicoHubPort))
                            return RetornarErros();

                        AtualizarLinhas(cabecalho);

                        _layoutRepositorio.CadastrarLayoutServicoHubPort(layoutServicoHubPort);

                        break;

                    case TipoRegistro.GERAIS:

                        var layoutServicosGerais = LayoutFactory.NovoLayoutServicosGerais(
                            viewModel.ModeloId,
                            viewModel.Linha,
                            viewModel.Descricao,
                            viewModel.ServicoId,
                            viewModel.ValorServGerais,
                            viewModel.Valor20ServGerais,
                            viewModel.Valor40ServGerais,
                            viewModel.AdicionalIMOServicosGerais,
                            viewModel.ExercitoServicosGerais,
                            viewModel.TipoCargaServGerais,
                            viewModel.BaseCalculoServGerais,
                            viewModel.MoedaServGerais,
                            viewModel.DescricaoValorServicosGerais,
                            viewModel.TipoDocumentoGerais,
                            viewModel.BaseExcessoGerais,
                            viewModel.ValorExcessoGerais,
                            viewModel.FormaPagamentoNVOCCGerais,
                            viewModel.Ocultar);

                        if (!Validar(layoutServicosGerais))
                            return RetornarErros();

                        AtualizarLinhas(cabecalho);

                        _layoutRepositorio.CadastrarLayoutServicosGerais(layoutServicosGerais);

                        break;

                    case TipoRegistro.MINIMO_GERAL:

                        var layoutMinimoGeral = LayoutFactory.NovoLayoutMinimoGeral(
                            viewModel.ModeloId,
                            viewModel.Linha,
                            viewModel.Descricao,
                            viewModel.ValorMinimoGeral,
                            viewModel.ValorMinimo20Geral,
                            viewModel.ValorMinimo40Geral,
                            viewModel.TipoCargaMinimoGerais,
                            viewModel.LinhaReferenciaMinimoGeral,
                            viewModel.DescricaoValorMinGerais,
                            viewModel.Ocultar);

                        if (!Validar(layoutMinimoGeral))
                            return RetornarErros();

                        AtualizarLinhas(cabecalho);

                        _layoutRepositorio.CadastrarLayoutMinimoGeral(layoutMinimoGeral);

                        break;

                    case TipoRegistro.CONDICAO_GERAL:

                        var layoutCondicoesGerais = LayoutFactory.NovoLayoutCondicoesGerais(
                            viewModel.ModeloId,
                            viewModel.Linha,
                            viewModel.Descricao,
                            viewModel.CondicoesGerais,
                            viewModel.Ocultar);

                        if (!Validar(layoutCondicoesGerais))
                            return RetornarErros();

                        AtualizarLinhas(cabecalho);

                        _layoutRepositorio.CadastrarLayoutCondicoesGerais(layoutCondicoesGerais);

                        break;

                    case TipoRegistro.PERIODO_PADRAO:

                        var layoutPeriodoPadrao = LayoutFactory.NovoLayoutPeriodoPadrao(
                            viewModel.ModeloId,
                            viewModel.Linha,
                            viewModel.Descricao,
                            viewModel.ValorPeriodoPadrao,
                            viewModel.Valor20PeriodoPadrao,
                            viewModel.Valor40PeriodoPadrao,
                            viewModel.TipoCargaPeriodoPadrao,
                            viewModel.ServicoId,
                            viewModel.BaseCalculoPeriodoPadrao,
                            viewModel.QtdeDiasPeriodoPadrao,
                            viewModel.PeriodoPadrao,
                            viewModel.DescricaoValorPeriodoPadrao,
                            viewModel.Ocultar);

                        if (!Validar(layoutPeriodoPadrao))
                            return RetornarErros();

                        AtualizarLinhas(cabecalho);

                        _layoutRepositorio.CadastrarLayoutPeriodoPadrao(layoutPeriodoPadrao);

                        break;

                    default:
                        ModelState.AddModelError(string.Empty, "Selecione um Tipo de Registro");
                        return RetornarErros();
                }
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                ModelState.AddModelError(string.Empty, ex.Message);
                return RetornarErros();
            }

            var layouts = _layoutRepositorio.ObterLayouts(viewModel.ModeloId, false);

            return PartialView("_Consulta", layouts);
        }

        [HttpGet]
        [CanActivateAtualizarLayout]
        public ActionResult Atualizar(int? id, bool? proposta, int? oportunidadeId)
        {
            ILayoutBase repositorio = null;

            if (proposta.GetValueOrDefault())
                repositorio = _layoutPropostaRepositorio;
            else
                repositorio = _layoutRepositorio;

            if (id == null)
                return RedirectToAction(nameof(Index));

            var layout = repositorio.ObterLayoutPorId(id.Value);

            if (layout == null)
                RegistroNaoEncontrado();

            var viewModel = new LayoutViewModel();

            var modelos = _modeloRepositorio.ObterModelos();
            var servicos = _servicoRepositorio.ObterServicos();
            var clienteHubPort = _hubPortRepositorio.ObterClientesHubPort();
            var tiposDocumentos = _documentoRepositorio.ObterTiposDocumentos();
            var gruposAtracacao = _grupoAtracacaoRepositorio.ObterGruposAtracacao();

            switch (layout.TipoRegistro)
            {
                case TipoRegistro.ARMAZENAGEM:

                    viewModel = new LayoutViewModel
                    {
                        ServicoId = layout.ServicoId,
                        BaseCalculoArmazenagem = layout.BaseCalculo,
                        QtdeDiasArmazenagem = layout.QtdeDias,
                        ValorArmazenagem = layout.Valor,
                        Valor20Armazenagem = layout.Valor20,
                        Valor40Armazenagem = layout.Valor40,
                        AdicionalArmazenagem = layout.AdicionalArmazenagem,
                        AdicionalGRC = layout.AdicionalGRC,
                        MinimoGRC = layout.MinimoGRC,
                        AdicionalIMOArmazenagem = layout.AdicionalIMO,
                        ExercitoArmazenagem = layout.Exercito,
                        AdicionalIMOGRC = layout.AdicionalIMOGRC,
                        ValorANVISA = layout.ValorANVISA,
                        AnvisaGRC = layout.AnvisaGRC,
                        PeriodoArmazenagem = layout.Periodo,
                        MoedaArmazenagem = layout.Moeda,
                        DescricaoValorArmazenagem = layout.DescricaoValor,
                        TipoCargaArmazenagem = layout.TipoCarga,
                        TipoDocumentoArmazenagem = layout.TipoDocumentoId,
                        BaseExcessoArmazenagem = layout.BaseExcesso,
                        MargemArmazenagem = layout.Margem,
                        ValorExcessoArmazenagem = layout.ValorExcesso,
                        AcrescimoPesoArmazenagem = layout.AdicionalPeso,
                        PesoLimiteArmazenagem = layout.PesoLimite,
                        GrupoAtracacaoArmazenagem = layout.GrupoAtracacaoId,
                        ProRataArmazenagem = layout.ProRata,
                        OportunidadeId = oportunidadeId
                    };

                    break;

                case TipoRegistro.ARMAZENAGEM_CIF:

                    viewModel = new LayoutViewModel
                    {
                        ServicoId = layout.ServicoId,
                        BaseCalculoArmazenagemCIF = layout.BaseCalculo,
                        QtdeDiasArmazenagemCIF = layout.QtdeDias,
                        CIFArmazenagemCIF = layout.ValorCIF,
                        ValorArmazenagemCIF = layout.Valor,
                        Valor20ArmazenagemCIF = layout.Valor20,
                        Valor40ArmazenagemCIF = layout.Valor40,
                        AdicionalArmazenagemCIF = layout.AdicionalArmazenagem,
                        AdicionalGRCCIF = layout.AdicionalGRC,
                        MinimoGRCCIF = layout.MinimoGRC,
                        AdicionalIMOArmazenagemCIF = layout.AdicionalIMO,
                        ExercitoArmazenagemCIF = layout.Exercito,
                        AdicionalIMOGRCCIF = layout.AdicionalIMOGRC,
                        ValorANVISACIF = layout.ValorANVISA,
                        AnvisaGRCCIF = layout.AnvisaGRC,
                        PeriodoArmazenagemCIF = layout.Periodo,
                        MoedaArmazenagemCIF = layout.Moeda,
                        DescricaoValorArmazenagemCIF = layout.DescricaoValor,
                        TipoCargaArmazenagemCIF = layout.TipoCarga,
                        TipoDocumentoArmazenagemCIF = layout.TipoDocumentoId,
                        BaseExcessoArmazenagemCIF = layout.BaseExcesso,
                        MargemArmazenagemCIF = layout.Margem,
                        ValorExcessoArmazenagemCIF = layout.ValorExcesso,
                        AcrescimoPesoArmazenagemCIF = layout.AdicionalPeso,
                        PesoLimiteArmazenagemCIF = layout.PesoLimite,
                        ProRataArmazenagemCIF = layout.ProRata,
                        OportunidadeId = oportunidadeId
                    };

                    break;

                case TipoRegistro.ARMAZENAGEM_MINIMO:

                    viewModel = new LayoutViewModel
                    {
                        ServicoId = layout.ServicoId,
                        ValorMinimoArmazenagemMin = layout.ValorMinimo,
                        ValorMinimo20ArmazenagemMin = layout.ValorMinimo20,
                        ValorMinimo40ArmazenagemMin = layout.ValorMinimo40,
                        LinhaReferenciaArmazenagemMin = layout.LinhaReferencia,
                        DescricaoValorArmazenagemMin = layout.DescricaoValor,
                        LimiteBls = layout.LimiteBls,
                        TipoCargaArmazenagemMinimo = layout.TipoCarga,
                        MargemArmazenagemMinimo = layout.Margem,
                        OportunidadeId = oportunidadeId
                    };

                    break;

                case TipoRegistro.ARMAZENAGEM_MINIMO_CIF:

                    viewModel = new LayoutViewModel
                    {
                        ServicoId = layout.ServicoId,
                        CIFArmazenagemMinimoCIF = layout.ValorCIF,
                        ValorMinimoArmazenagemMinCIF = layout.ValorMinimo,
                        ValorMinimo20ArmazenagemMinCIF = layout.ValorMinimo20,
                        ValorMinimo40ArmazenagemMinCIF = layout.ValorMinimo40,
                        LinhaReferenciaArmazenagemMinCIF = layout.LinhaReferencia,
                        DescricaoValorArmazenagemMinCIF = layout.DescricaoValor,
                        LimiteBlsCIF = layout.LimiteBls,
                        TipoCargaArmazenagemMinimoCIF = layout.TipoCarga,
                        OportunidadeId = oportunidadeId
                    };

                    break;

                case TipoRegistro.ARMAZENAMEM_ALL_IN:

                    viewModel = new LayoutViewModel
                    {
                        ServicoId = layout.ServicoId,
                        BaseCalculoArmazenagemAllIn = layout.BaseCalculo,
                        PeriodoArmazenagemAllIn = layout.Periodo,
                        DescricaoPeriodoAllIn = layout.DescricaoPeriodo,
                        MargemArmAllIn = layout.Margem,
                        Valor20ArmazenagemAllIn = layout.Valor20,
                        Valor40ArmazenagemAllIn = layout.Valor40,
                        CIFMinimoAllIn = layout.CifMinimo,
                        CIFMaximoAllIn = layout.CifMaximo,
                        DescricaoCifAllIn = layout.DescricaoCif,
                        ValorMinimoAllIn = layout.ValorMinimo,
                        MoedaArmazenagemAllIn = layout.Moeda,
                        DescricaoValorArmazenagemAllIn = layout.DescricaoValor,
                        TipoDocumentoAllIn = layout.TipoDocumentoId,
                        BaseExcessoAllIn = layout.BaseExcesso,
                        ValorExcessoAllIn = layout.ValorExcesso,
                        AcrescimoPesoAllIn = layout.AdicionalPeso,
                        PesoLimiteAllIn = layout.PesoLimite,
                        ProRataAllIn = layout.ProRata,
                        OportunidadeId = oportunidadeId
                    };

                    break;

                case TipoRegistro.SERVIÇO_PARA_MARGEM:

                    viewModel = new LayoutViewModel
                    {
                        ServicoId = layout.ServicoId,
                        BaseCalculoServMargem = layout.BaseCalculo,
                        TipoCargaServMargem = layout.TipoCarga,
                        ValorMargemDireitaServMargem = layout.ValorMargemDireita,
                        ValorMargemEsquerdaServMargem = layout.ValorMargemEsquerda,
                        ValorEntreMargensServMargem = layout.ValorEntreMargens,
                        AdicionalIMOServMargem = layout.AdicionalIMO,
                        ExercitoServMargem = layout.Exercito,
                        MoedaServMargem = layout.Moeda,
                        PesoMaximoServicoParaMargem = layout.PesoMaximo,
                        AdicionalPesoServicoParaMargem = layout.AdicionalPeso,
                        DescricaoValorServMargem = layout.DescricaoValor,
                        TipoDocumentoServMargem = layout.TipoDocumentoId,
                        BaseExcessoServMargem = layout.BaseExcesso,
                        ValorExcessoServMargem = layout.ValorExcesso,
                        AcrescimoPesoServMargem = layout.AdicionalPeso,
                        PesoLimiteServMargem = layout.PesoLimite,
                        ProRataServMargem = layout.ProRata,
                        OportunidadeId = oportunidadeId
                    };

                    break;

                case TipoRegistro.MINIMO_PARA_MARGEM:

                    viewModel = new LayoutViewModel
                    {
                        ServicoId = layout.ServicoId,
                        ValorMinimoMargemDireita = layout.ValorMinimoMargemDireita,
                        ValorMinimoMargemEsquerda = layout.ValorMinimoMargemEsquerda,
                        ValorMinimoEntreMargens = layout.ValorMinimoEntreMargens,
                        LinhaReferenciaMinimoParaMargem = layout.LinhaReferencia,
                        DescricaoValorMinimoMargem = layout.DescricaoValor,
                        OportunidadeId = oportunidadeId
                    };

                    break;

                case TipoRegistro.SERVICO_MECANICA_MANUAL:

                    viewModel = new LayoutViewModel
                    {
                        ServicoId = layout.ServicoId,
                        BaseCalculoServMecManual = layout.BaseCalculo,
                        TipoCargaServMecManual = layout.TipoCarga,
                        ValorServMecManual = layout.Valor,
                        Valor20ServMecManual = layout.Valor20,
                        Valor40ServMecManual = layout.Valor40,
                        AdicionalIMOServMecManual = layout.AdicionalIMO,
                        ExercitoServMecManual = layout.Exercito,
                        MoedaServMecMan = layout.Moeda,
                        PesoMaximoServicoMecanicaManual = layout.PesoMaximo,
                        AdicionalPesoServicoMecanicaManual = layout.AdicionalPeso,
                        TipoTrabalho = layout.TipoTrabalho,
                        DescricaoValorServMecManual = layout.DescricaoValor,
                        OportunidadeId = oportunidadeId
                    };

                    break;

                case TipoRegistro.MINIMO_MECANICA_MANUAL:

                    viewModel = new LayoutViewModel
                    {
                        ValorMinimo20MecManual = layout.ValorMinimo20,
                        ValorMinimo40MecManual = layout.ValorMinimo40,
                        LinhaReferenciaMinimoMecanicaManual = layout.LinhaReferencia,
                        DescricaoValorMinimoMecManual = layout.DescricaoValor,
                        OportunidadeId = oportunidadeId
                    };

                    break;

                case TipoRegistro.SERVICO_LIBERACAO:

                    viewModel = new LayoutViewModel
                    {
                        ServicoId = layout.ServicoId,
                        BaseCalculoServLib = layout.BaseCalculo,
                        MargemServLib = layout.Margem,
                        Reembolso = layout.Reembolso,
                        ValorServLib = layout.Valor,
                        Valor20ServLib = layout.Valor20,
                        Valor40ServLib = layout.Valor40,
                        MoedaServLib = layout.Moeda,
                        TipoCargaServLib = layout.TipoCarga,
                        DescricaoValorServLib = layout.DescricaoValor,
                        TipoDocumentoServLib = layout.TipoDocumentoId,
                        GrupoAtracacaoServLiv = layout.GrupoAtracacaoId,
                        AdicionalIMOServLib = layout.AdicionalIMO,
                        ExercitoServLib = layout.Exercito,
                        OportunidadeId = oportunidadeId
                    };

                    break;

                case TipoRegistro.SERVICO_HUBPORT:

                    viewModel = new LayoutViewModel
                    {
                        ServicoId = layout.ServicoId,
                        BaseCalculoHubPort = layout.BaseCalculo,
                        ValorHubPort = layout.Valor,
                        Origem = layout.Origem,
                        Destino = layout.Destino,
                        MoedaHubPort = layout.Moeda,
                        FormaPagamentoNVOCCHubPort = layout.FormaPagamentoNVOCC,
                        DescricaoValorHubPort = layout.DescricaoValor,
                        OportunidadeId = oportunidadeId
                    };

                    break;

                case TipoRegistro.GERAIS:

                    viewModel = new LayoutViewModel
                    {
                        BaseCalculoServGerais = layout.BaseCalculo,
                        ValorServGerais = layout.Valor,
                        Valor20ServGerais = layout.Valor20,
                        Valor40ServGerais = layout.Valor40,
                        MoedaServGerais = layout.Moeda,
                        TipoCargaServGerais = layout.TipoCarga,
                        DescricaoValorServicosGerais = layout.DescricaoValor,
                        AdicionalIMOServicosGerais = layout.AdicionalIMO,
                        ExercitoServicosGerais = layout.Exercito,
                        TipoDocumentoGerais = layout.TipoDocumentoId,
                        BaseExcessoGerais = layout.BaseExcesso,
                        ValorExcessoGerais = layout.ValorExcesso,
                        FormaPagamentoNVOCCGerais = layout.FormaPagamentoNVOCC,
                        OportunidadeId = oportunidadeId
                    };

                    break;

                case TipoRegistro.MINIMO_GERAL:

                    viewModel = new LayoutViewModel
                    {
                        ValorMinimoGeral = layout.ValorMinimo,
                        ValorMinimo20Geral = layout.ValorMinimo20,
                        ValorMinimo40Geral = layout.ValorMinimo40,
                        LinhaReferenciaMinimoGeral = layout.LinhaReferencia,
                        TipoCargaMinimoGerais = layout.TipoCarga,
                        DescricaoValorMinGerais = layout.DescricaoValor,
                        OportunidadeId = oportunidadeId
                    };

                    break;

                case TipoRegistro.CONDICAO_GERAL:

                    viewModel = new LayoutViewModel
                    {
                        CondicoesGerais = layout.CondicoesGerais,
                        OportunidadeId = oportunidadeId
                    };

                    break;

                case TipoRegistro.CONDICAO_INICIAL:

                    viewModel = new LayoutViewModel
                    {
                        CondicoesIniciais = layout.CondicoesIniciais,
                        OportunidadeId = oportunidadeId
                    };

                    break;

                case TipoRegistro.PERIODO_PADRAO:

                    viewModel = new LayoutViewModel
                    {
                        ServicoId = layout.ServicoId,
                        BaseCalculoPeriodoPadrao = layout.BaseCalculo,
                        QtdeDiasPeriodoPadrao = layout.QtdeDias,
                        PeriodoPadrao = layout.Periodo,
                        ValorPeriodoPadrao = layout.Valor,
                        Valor20PeriodoPadrao = layout.Valor20,
                        Valor40PeriodoPadrao = layout.Valor40,
                        DescricaoValorPeriodoPadrao = layout.DescricaoValor,
                        TipoCargaPeriodoPadrao = layout.TipoCarga,
                        OportunidadeId = oportunidadeId
                    };

                    break;
            }

            viewModel.Modelos = modelos.Select(c => new Modelo
            {
                Id = c.Id,
                Descricao = c.Descricao
            });

            viewModel.Servicos = servicos.Select(c => new Servico
            {
                Id = c.Id,
                Descricao = c.Descricao
            });

            viewModel.ClientesHubPort = clienteHubPort;
            viewModel.TiposDocumentos = tiposDocumentos;
            viewModel.GruposAtracacao = gruposAtracacao;

            viewModel.TipoRegistro = layout.TipoRegistro;
            viewModel.ModeloId = layout.ModeloId;
            viewModel.Linha = layout.Linha;
            viewModel.Ocultar = layout.Ocultar;
            viewModel.Descricao = layout.Descricao;
            viewModel.ServicoId = layout.ServicoId;

            viewModel.Id = layout.Id;

            viewModel.LayoutProposta = proposta.GetValueOrDefault();

            return View(viewModel);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Atualizar(LayoutViewModel viewModel, int? id)
        {
            ILayoutBase repositorio = null;

            if (viewModel.LayoutProposta)
                repositorio = _layoutPropostaRepositorio;
            else
                repositorio = _layoutRepositorio;

            if (id == null)
                return RedirectToAction(nameof(Index));

            var layout = repositorio.ObterLayoutPorId(id.Value);

            if (layout == null)
                RegistroNaoEncontrado();

            var modelos = _modeloRepositorio.ObterModelos();
            var servicos = _servicoRepositorio.ObterServicos();
            var clienteHubPort = _hubPortRepositorio.ObterClientesHubPort();
            var tiposDocumentos = _documentoRepositorio.ObterTiposDocumentos();
            var gruposAtracacao = _grupoAtracacaoRepositorio.ObterGruposAtracacao();

            var cabecalho = new Cabecalho(viewModel.ModeloId, viewModel.Linha, viewModel.Descricao, viewModel.TipoRegistro, viewModel.Ocultar);

            switch (viewModel.TipoRegistro)
            {
                case TipoRegistro.CONDICAO_INICIAL:

                    var layoutCondicoesIniciais = LayoutFactory.NovoLayoutCondicoesIniciais(
                        viewModel.ModeloId,
                        viewModel.Linha,
                        viewModel.Descricao,
                        viewModel.CondicoesIniciais,
                        viewModel.Ocultar);

                    layoutCondicoesIniciais.Id = layout.Id;

                    Validar(layoutCondicoesIniciais);

                    if (!ModelState.IsValid)
                        return RetornarErros();

                    repositorio.AtualizarLayoutCondicoesIniciais(layoutCondicoesIniciais);

                    if (repositorio.LayoutProposta() && viewModel.OportunidadeId.HasValue)
                    {
                        var detalhesProposta = _oportunidadeRepositorio.ObterDetalhesProposta(viewModel.OportunidadeId.Value);

                        GravarLogAuditoria(TipoLogAuditoria.UPDATE,
                            detalhesProposta, viewModel.OportunidadeId.Value, "Condições Iniciais Atualizadas");
                    }

                    break;

                case TipoRegistro.TITULO_MASTER:

                    var layoutTituloMaster = new LayoutTituloMaster(cabecalho);

                    layoutTituloMaster.Id = layout.Id;

                    Validar(layoutTituloMaster);

                    if (!ModelState.IsValid)
                        return RetornarErros();

                    repositorio.AtualizarLayoutTituloMaster(layoutTituloMaster);

                    break;

                case TipoRegistro.TITULO:

                    var layoutTitulo = new LayoutTitulo(cabecalho);

                    layoutTitulo.Id = layout.Id;

                    Validar(layoutTitulo);

                    if (!ModelState.IsValid)
                        return RetornarErros();

                    repositorio.AtualizarLayoutTitulo(layoutTitulo);

                    break;

                case TipoRegistro.SUB_TITULO:

                    var layoutSubTitulo = new LayoutSubTitulo(cabecalho);

                    layoutSubTitulo.Id = layout.Id;

                    Validar(layoutSubTitulo);

                    if (!ModelState.IsValid)
                        return RetornarErros();

                    repositorio.AtualizarLayoutSubTitulo(layoutSubTitulo);

                    break;

                case TipoRegistro.SUB_TITULO_MARGEM:
                case TipoRegistro.SUB_TITULO_MARGEM_D_E:

                    var layoutSubTituloMargem = new LayoutSubTituloMargem(cabecalho);

                    layoutSubTituloMargem.Id = layout.Id;

                    Validar(layoutSubTituloMargem);

                    if (!ModelState.IsValid)
                        return RetornarErros();

                    repositorio.AtualizarLayoutSubTituloMargem(layoutSubTituloMargem);

                    break;

                case TipoRegistro.SUB_TITULO_ALL_IN:

                    var layoutSubTituloAllIn = new LayoutSubTituloAllIn(cabecalho);

                    layoutSubTituloAllIn.Id = layout.Id;

                    Validar(layoutSubTituloAllIn);

                    if (!ModelState.IsValid)
                        return RetornarErros();

                    repositorio.AtualizarLayoutSubTituloAllIn(layoutSubTituloAllIn);

                    break;

                case TipoRegistro.ARMAZENAGEM:

                    var layoutArmazenagem = LayoutFactory.NovoLayoutArmazenagem(
                        viewModel.ModeloId,
                        viewModel.Linha,
                        viewModel.Descricao,
                        viewModel.ValorArmazenagem,
                        viewModel.Valor20Armazenagem,
                        viewModel.Valor40Armazenagem,
                        viewModel.TipoCargaArmazenagem,
                        viewModel.ServicoId,
                        viewModel.BaseCalculoArmazenagem,
                        viewModel.QtdeDiasArmazenagem,
                        viewModel.AdicionalArmazenagem,
                        viewModel.AdicionalGRC,
                        viewModel.MinimoGRC,
                        viewModel.AdicionalIMOArmazenagem,
                        viewModel.ExercitoArmazenagem,
                        viewModel.AdicionalIMOGRC,
                        viewModel.ValorANVISA,
                        viewModel.AnvisaGRC,
                        viewModel.PeriodoArmazenagem,
                        viewModel.MoedaArmazenagem,
                        viewModel.DescricaoValorArmazenagem,
                        viewModel.TipoDocumentoArmazenagem,
                        viewModel.BaseExcessoArmazenagem,
                        viewModel.MargemArmazenagem,
                        viewModel.ValorExcessoArmazenagem,
                        viewModel.AcrescimoPesoArmazenagem,
                        viewModel.PesoLimiteArmazenagem,
                        viewModel.GrupoAtracacaoArmazenagem,
                        viewModel.ProRataArmazenagem,
                        viewModel.Ocultar);

                    layoutArmazenagem.Id = layout.Id;

                    Validar(layoutArmazenagem);

                    if (!ModelState.IsValid)
                        return RetornarErros();

                    repositorio.AtualizarLayoutArmazenagem(layoutArmazenagem);

                    break;

                case TipoRegistro.ARMAZENAGEM_CIF:

                    var layoutArmazenagemCIF = LayoutFactory.NovoLayoutArmazenagemCIF(
                        viewModel.ModeloId,
                        viewModel.Linha,
                        viewModel.Descricao,
                        viewModel.CIFArmazenagemCIF,
                        viewModel.ValorArmazenagemCIF,
                        viewModel.Valor20ArmazenagemCIF,
                        viewModel.Valor40ArmazenagemCIF,
                        viewModel.TipoCargaArmazenagemCIF,
                        viewModel.ServicoId,
                        viewModel.BaseCalculoArmazenagemCIF,
                        viewModel.QtdeDiasArmazenagemCIF,
                        viewModel.AdicionalArmazenagemCIF,
                        viewModel.ExercitoArmazenagemCIF,
                        viewModel.AdicionalGRCCIF,
                        viewModel.MinimoGRCCIF,
                        viewModel.AdicionalIMOArmazenagemCIF,
                        viewModel.AdicionalIMOGRCCIF,
                        viewModel.ValorANVISACIF,
                        viewModel.AnvisaGRCCIF,
                        viewModel.PeriodoArmazenagemCIF,
                        viewModel.MoedaArmazenagemCIF,
                        viewModel.DescricaoValorArmazenagemCIF,
                        viewModel.TipoDocumentoArmazenagemCIF,
                        viewModel.BaseExcessoArmazenagemCIF,
                        viewModel.MargemArmazenagemCIF,
                        viewModel.ValorExcessoArmazenagemCIF,
                        viewModel.AcrescimoPesoArmazenagemCIF,
                        viewModel.PesoLimiteArmazenagemCIF,
                        viewModel.ProRataArmazenagemCIF,
                        viewModel.Ocultar);

                    layoutArmazenagemCIF.Id = layout.Id;

                    Validar(layoutArmazenagemCIF);

                    if (!ModelState.IsValid)
                        return RetornarErros();

                    repositorio.AtualizarLayoutArmazenagemCIF(layoutArmazenagemCIF);

                    break;

                case TipoRegistro.ARMAZENAGEM_MINIMO:

                    var layoutArmazenagemMinimo = LayoutFactory.NovoLayoutArmazenagemMinimo(
                        viewModel.ModeloId,
                        viewModel.Linha,
                        viewModel.Descricao,
                        viewModel.ValorMinimoArmazenagemMin,
                        viewModel.ValorMinimo20ArmazenagemMin,
                        viewModel.ValorMinimo40ArmazenagemMin,
                        viewModel.TipoCargaArmazenagemMinimo,
                        viewModel.MargemArmazenagemMinimo,
                        viewModel.ServicoId,
                        viewModel.LinhaReferenciaArmazenagemMin,
                        viewModel.DescricaoValorArmazenagemMin,
                        viewModel.LimiteBls,
                        viewModel.Ocultar);

                    layoutArmazenagemMinimo.Id = layout.Id;

                    Validar(layoutArmazenagemMinimo);

                    if (!ModelState.IsValid)
                        return RetornarErros();

                    repositorio.AtualizarLayoutArmazenagemMinimo(layoutArmazenagemMinimo);

                    break;

                case TipoRegistro.ARMAZENAGEM_MINIMO_CIF:

                    var layoutArmazenagemMinimoCIF = LayoutFactory.NovoLayoutArmazenagemMinimoCIF(
                        viewModel.ModeloId,
                        viewModel.Linha,
                        viewModel.Descricao,
                        viewModel.CIFArmazenagemMinimoCIF,
                        viewModel.ValorMinimoArmazenagemMinCIF,
                        viewModel.ValorMinimo20ArmazenagemMinCIF,
                        viewModel.ValorMinimo40ArmazenagemMinCIF,
                        viewModel.TipoCargaArmazenagemMinimoCIF,
                        viewModel.ServicoId,
                        viewModel.LinhaReferenciaArmazenagemMinCIF,
                        viewModel.DescricaoValorArmazenagemMinCIF,
                        viewModel.LimiteBlsCIF,
                        viewModel.Ocultar);

                    layoutArmazenagemMinimoCIF.Id = layout.Id;

                    Validar(layoutArmazenagemMinimoCIF);

                    if (!ModelState.IsValid)
                        return RetornarErros();

                    repositorio.AtualizarLayoutArmazenagemMinimoCIF(layoutArmazenagemMinimoCIF);

                    break;

                case TipoRegistro.ARMAZENAMEM_ALL_IN:

                    var layoutArmazenagemAllIn = LayoutFactory.NovoLayoutArmazenagemAllIn(
                        viewModel.ModeloId,
                        viewModel.Linha,
                        viewModel.Descricao,
                        viewModel.ValorMinimoAllIn,
                        viewModel.Valor20ArmazenagemAllIn,
                        viewModel.Valor40ArmazenagemAllIn,
                        viewModel.CIFMinimoAllIn,
                        viewModel.CIFMaximoAllIn,
                        viewModel.DescricaoCifAllIn,
                        viewModel.ServicoId,
                        viewModel.BaseCalculoArmazenagemAllIn,
                        viewModel.PeriodoArmazenagemAllIn,
                        viewModel.DescricaoPeriodoAllIn,
                        viewModel.MargemArmAllIn,
                        viewModel.MoedaArmazenagemAllIn,
                        viewModel.DescricaoValorArmazenagemAllIn,
                        viewModel.TipoDocumentoAllIn,
                        viewModel.BaseExcessoAllIn,
                        viewModel.ValorExcessoAllIn,
                        viewModel.AcrescimoPesoAllIn,
                        viewModel.PesoLimiteAllIn,
                        viewModel.ProRataAllIn,
                        viewModel.Ocultar);

                    layoutArmazenagemAllIn.Id = layout.Id;

                    Validar(layoutArmazenagemAllIn);

                    if (!ModelState.IsValid)
                        return RetornarErros();

                    repositorio.AtualizarLayoutArmazenagemAllIn(layoutArmazenagemAllIn);

                    break;

                case TipoRegistro.SERVIÇO_PARA_MARGEM:

                    var layoutServicoParaMargem = LayoutFactory.NovoLayoutServicoParaMargem(
                        viewModel.ModeloId,
                        viewModel.Linha,
                        viewModel.Descricao,
                        viewModel.ServicoId,
                        viewModel.BaseCalculoServMargem,
                        viewModel.TipoCargaServMargem,
                        viewModel.ValorMargemDireitaServMargem,
                        viewModel.ValorMargemEsquerdaServMargem,
                        viewModel.ValorEntreMargensServMargem,
                        viewModel.AdicionalIMOServMargem,
                        viewModel.ExercitoServMargem,
                        viewModel.MoedaServMargem,
                        viewModel.PesoMaximoServicoParaMargem,
                        viewModel.AdicionalPesoServicoParaMargem,
                        viewModel.DescricaoValorServMargem,
                        viewModel.TipoDocumentoServMargem,
                        viewModel.BaseExcessoServMargem,
                        viewModel.ValorExcessoServMargem,
                        viewModel.PesoLimiteServMargem,
                        viewModel.ProRataServMargem,
                        viewModel.Ocultar);

                    layoutServicoParaMargem.Id = layout.Id;

                    Validar(layoutServicoParaMargem);

                    if (!ModelState.IsValid)
                        return RetornarErros();

                    repositorio.AtualizarLayoutServicoParaMargem(layoutServicoParaMargem);

                    break;

                case TipoRegistro.MINIMO_PARA_MARGEM:

                    var layoutMinimoParaMargem = LayoutFactory.NovoLayoutMinimoParaMargem(
                        viewModel.ModeloId,
                        viewModel.Linha,
                        viewModel.Descricao,
                        viewModel.ServicoId,
                        viewModel.ValorMinimoMargemDireita,
                        viewModel.ValorMinimoMargemEsquerda,
                        viewModel.ValorMinimoEntreMargens,
                        viewModel.LinhaReferenciaMinimoParaMargem,
                        viewModel.DescricaoValorMinimoMargem,
                        viewModel.Ocultar);

                    layoutMinimoParaMargem.Id = layout.Id;

                    Validar(layoutMinimoParaMargem);

                    if (!ModelState.IsValid)
                        return RetornarErros();

                    repositorio.AtualizarLayoutMinimoParaMargem(layoutMinimoParaMargem);

                    break;

                case TipoRegistro.SERVICO_MECANICA_MANUAL:

                    var layoutServicoMecanicaManual = LayoutFactory.NovoLayoutServicoMecanicaManual(
                        viewModel.ModeloId,
                        viewModel.Linha,
                        viewModel.Descricao,
                        viewModel.ValorServMecManual,
                        viewModel.Valor20ServMecManual,
                        viewModel.Valor40ServMecManual,
                        viewModel.ServicoId,
                        viewModel.BaseCalculoServMecManual,
                        viewModel.TipoCargaServMecManual,
                        viewModel.AdicionalIMOServMecManual,
                        viewModel.ExercitoServMecManual,
                        viewModel.MoedaServMecMan,
                        viewModel.PesoMaximoServicoMecanicaManual,
                        viewModel.AdicionalPesoServicoMecanicaManual,
                        viewModel.TipoTrabalho,
                        viewModel.DescricaoValorServMecManual,
                        viewModel.Ocultar);

                    layoutServicoMecanicaManual.Id = layout.Id;

                    Validar(layoutServicoMecanicaManual);

                    if (!ModelState.IsValid)
                        return RetornarErros();

                    repositorio.AtualizarLayoutServicoMecanicaManual(layoutServicoMecanicaManual);

                    break;

                case TipoRegistro.MINIMO_MECANICA_MANUAL:

                    var layoutMinimoMecanicaManual = LayoutFactory.NovoLayoutMinimoMecanicaManual(
                        viewModel.ModeloId,
                        viewModel.Linha,
                        viewModel.Descricao,
                        viewModel.ValorMinimo20MecManual,
                        viewModel.ValorMinimo40MecManual,
                        viewModel.LinhaReferenciaMinimoMecanicaManual,
                        viewModel.DescricaoValorMinimoMecManual,
                        viewModel.Ocultar);

                    layoutMinimoMecanicaManual.Id = layout.Id;

                    Validar(layoutMinimoMecanicaManual);

                    if (!ModelState.IsValid)
                        return RetornarErros();

                    repositorio.AtualizarLayoutMinimoMecanicaManual(layoutMinimoMecanicaManual);

                    break;

                case TipoRegistro.SERVICO_LIBERACAO:

                    var layoutServicoLiberacao = LayoutFactory.NovoLayoutServicoLiberacao(
                        viewModel.ModeloId,
                        viewModel.Linha,
                        viewModel.Descricao,
                        viewModel.ValorServLib,
                        viewModel.Valor20ServLib,
                        viewModel.Valor40ServLib,
                        viewModel.TipoCargaServLib,
                        viewModel.ServicoId,
                        viewModel.BaseCalculoServLib,
                        viewModel.MargemServLib,
                        viewModel.Reembolso,
                        viewModel.MoedaServLib,
                        viewModel.DescricaoValorServLib,
                        viewModel.TipoDocumentoServLib,
                        viewModel.GrupoAtracacaoServLiv,
                        viewModel.AdicionalIMOServLib,
                        viewModel.ExercitoServLib,
                        viewModel.Ocultar);

                    layoutServicoLiberacao.Id = layout.Id;

                    Validar(layoutServicoLiberacao);

                    if (!ModelState.IsValid)
                        return RetornarErros();

                    repositorio.AtualizarLayoutServicoLiberacao(layoutServicoLiberacao);

                    break;

                case TipoRegistro.SERVICO_HUBPORT:

                    var layoutServicoHubPort = LayoutFactory.NovoLayoutServicoHubPort(
                        viewModel.ModeloId,
                        viewModel.Linha,
                        viewModel.Descricao,
                        viewModel.ServicoId,
                        viewModel.BaseCalculoHubPort,
                        viewModel.ValorHubPort,
                        viewModel.Origem,
                        viewModel.Destino,
                        viewModel.MoedaHubPort,
                        viewModel.FormaPagamentoNVOCCHubPort,
                        viewModel.DescricaoValorHubPort,
                        viewModel.Ocultar);

                    layoutServicoHubPort.Id = layout.Id;

                    Validar(layoutServicoHubPort);

                    if (!ModelState.IsValid)
                        return RetornarErros();

                    repositorio.AtualizarLayoutServicoHubPort(layoutServicoHubPort);

                    break;

                case TipoRegistro.GERAIS:

                    var layoutServicosGerais = LayoutFactory.NovoLayoutServicosGerais(
                        viewModel.ModeloId,
                        viewModel.Linha,
                        viewModel.Descricao,
                        viewModel.ServicoId,
                        viewModel.ValorServGerais,
                        viewModel.Valor20ServGerais,
                        viewModel.Valor40ServGerais,
                        viewModel.AdicionalIMOServicosGerais,
                        viewModel.ExercitoServicosGerais,
                        viewModel.TipoCargaServGerais,
                        viewModel.BaseCalculoServGerais,
                        viewModel.MoedaServGerais,
                        viewModel.DescricaoValorServicosGerais,
                        viewModel.TipoDocumentoGerais,
                        viewModel.BaseExcessoGerais,
                        viewModel.ValorExcessoGerais,
                        viewModel.FormaPagamentoNVOCCGerais,
                        viewModel.Ocultar);

                    layoutServicosGerais.Id = layout.Id;

                    Validar(layoutServicosGerais);

                    if (!ModelState.IsValid)
                        return RetornarErros();

                    repositorio.AtualizarLayoutServicosGerais(layoutServicosGerais);

                    break;

                case TipoRegistro.MINIMO_GERAL:

                    var layoutMinimoGeral = LayoutFactory.NovoLayoutMinimoGeral(
                        viewModel.ModeloId,
                        viewModel.Linha,
                        viewModel.Descricao,
                        viewModel.ValorMinimoGeral,
                        viewModel.ValorMinimo20Geral,
                        viewModel.ValorMinimo40Geral,
                        viewModel.TipoCargaMinimoGerais,
                        viewModel.LinhaReferenciaMinimoGeral,
                        viewModel.DescricaoValorMinGerais,
                        viewModel.Ocultar);

                    layoutMinimoGeral.Id = layout.Id;

                    Validar(layoutMinimoGeral);

                    if (!ModelState.IsValid)
                        return RetornarErros();

                    repositorio.AtualizarLayoutMinimoGeral(layoutMinimoGeral);

                    break;

                case TipoRegistro.CONDICAO_GERAL:

                    var layoutCondicoesGerais = LayoutFactory.NovoLayoutCondicoesGerais(
                        viewModel.ModeloId,
                        viewModel.Linha,
                        viewModel.Descricao,
                        viewModel.CondicoesGerais,
                        viewModel.Ocultar);

                    layoutCondicoesGerais.Id = layout.Id;

                    Validar(layoutCondicoesGerais);

                    if (!ModelState.IsValid)
                        return RetornarErros();

                    repositorio.AtualizarLayoutCondicoesGerais(layoutCondicoesGerais);

                    if (repositorio.LayoutProposta() && viewModel.OportunidadeId.HasValue)
                    {
                        var detalhesProposta = _oportunidadeRepositorio.ObterDetalhesProposta(viewModel.OportunidadeId.Value);

                        GravarLogAuditoria(TipoLogAuditoria.UPDATE,
                            detalhesProposta, viewModel.OportunidadeId.Value, "Condições Gerais Atualizadas");
                    }

                    break;

                case TipoRegistro.PERIODO_PADRAO:

                    var layoutPeriodoPadrao = LayoutFactory.NovoLayoutPeriodoPadrao(
                            viewModel.ModeloId,
                            viewModel.Linha,
                            viewModel.Descricao,
                            viewModel.ValorPeriodoPadrao,
                            viewModel.Valor20PeriodoPadrao,
                            viewModel.Valor40PeriodoPadrao,
                            viewModel.TipoCargaPeriodoPadrao,
                            viewModel.ServicoId,
                            viewModel.BaseCalculoPeriodoPadrao,
                            viewModel.QtdeDiasPeriodoPadrao,
                            viewModel.PeriodoPadrao,
                            viewModel.DescricaoValorPeriodoPadrao,
                            viewModel.Ocultar);

                    layoutPeriodoPadrao.Id = layout.Id;

                    Validar(layoutPeriodoPadrao);

                    if (!ModelState.IsValid)
                        return RetornarErros();

                    repositorio.AtualizarLayoutPeriodoPadrao(layoutPeriodoPadrao);

                    break;

                default:
                    ModelState.AddModelError(string.Empty, "Selecione um Tipo de Registro");
                    return RetornarErros();
            }

            viewModel.Modelos = modelos.Select(c => new Modelo
            {
                Id = c.Id,
                Descricao = c.Descricao
            });

            viewModel.Servicos = servicos.Select(c => new Servico
            {
                Id = c.Id,
                Descricao = c.Descricao
            });

            viewModel.ClientesHubPort = clienteHubPort;
            viewModel.TiposDocumentos = tiposDocumentos;
            viewModel.GruposAtracacao = gruposAtracacao;

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Excluir(int id)
        {
            try
            {
                var layout = _layoutRepositorio.ObterLayoutPorId(id);

                if (layout == null)
                    RegistroNaoEncontrado();

                _layoutRepositorio.Excluir(id);
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Não foi possível excluir o registro");
            }

            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        public ActionResult ConsultarLayouts(int modeloId)
        {
            var layouts = _layoutRepositorio.ObterLayouts(modeloId, false);

            return PartialView("_Consulta", layouts);
        }

        private void DeleteCookie(string cookie)
        {
            Response.Cookies.Add(new HttpCookie(cookie)
            {
                Expires = DateTime.Now.AddDays(-1)
            });
        }

        public int ObterUltimaLinha(int modeloId) =>
            _layoutRepositorio.ObterUltimaLinha(modeloId);

        [HttpGet]
        public string MontaLayout(int modeloId, bool ocultar)
        {
            var layoutService = new LayoutService(_layoutRepositorio, _modeloRepositorio, _oportunidadeRepositorio);

            var linhas = _layoutRepositorio.ObterLayouts(modeloId, ocultar).ToList();

            return layoutService.MontarLayout(linhas, null);
        }

        [HttpPost]
        public ActionResult ExcluirLinha(string linha, string modelo)
        {
            if (string.IsNullOrEmpty(linha) || string.IsNullOrEmpty(modelo))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            try
            {
                _layoutRepositorio.ExcluirLinha(linha.ToInt(), modelo.ToInt());
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        [HttpGet]
        public string ObterTipoRegistroPorLinha(int linha, int modeloId)
        {
            return _layoutRepositorio.ObterTipoRegistroPorLinha(linha, modeloId);
        }

        [HttpGet]
        public ActionResult ObterCamposLayout(int tipo)
        {
            TipoRegistro tipoRegistro = (TipoRegistro)tipo;

            IDictionary<string, string> campos = new Dictionary<string, string>();

            //tipoDeDados:NomeCampoView / Valor exibição

            campos = new Dictionary<string, string>()
            {
                { "i:Modelo", "Identificação" },
                { "i:Linha", "Linha" },
                { "s:Descricao", "Descrição" }
            };

            switch (tipoRegistro)
            {
                case TipoRegistro.ARMAZENAGEM:

                    campos = new Dictionary<string, string>()
                    {
                        { "s:BaseCalculo", "Base Cálculo" },
                        { "i:QtdeDias", "Qtde  Dias" },
                        { "p:AdicionalArmazenagem", "Adicional Armazenagem" },
                        { "p:AdicionalGRC", "Adicional GRC" },
                        { "p:AnvisaGRC", "Anvisa GRC" },
                        { "f:MinimoGRC", "Mínimo GRC" },
                        { "p:AdicionalIMO", "Adicional IMO" },
                        { "p:AdicionalIMOGRC", "Adicional IMO GRC" },
                        { "f:ValorANVISA", "Valor ANVISA" },
                        { "f:ANVISAGRC", "Valor ANVISA GRC" },
                        { "i:Periodo", "Período" },
                        { "s:Moeda", "Moeda" },
                        { "s:DescricaoValor", "Descrição Valor" },
                        { "f:Valor", "Valor" },
                        { "f:Valor20", "Valor 20" },
                        { "f:Valor40", "Valor 40" },
                        { "s:TipoCarga", "Tipo Carga" },
                        { "p:Exercito", "Exercito" }
                    };

                    break;

                case TipoRegistro.ARMAZENAGEM_MINIMO:

                    campos = new Dictionary<string, string>()
                    {
                        { "i:ServicoId", "Serviço" },
                        { "s:DescricaoValor", "Descrição Valor" },
                        { "f:ValorMinimo", "Valor Mínimo" },
                        { "f:ValorMinimo20", "Valor Mínimo 20" },
                        { "f:ValorMinimo40", "Valor Mínimo 40" }
                    };

                    break;

                case TipoRegistro.ARMAZENAMEM_ALL_IN:

                    campos = new Dictionary<string, string>()
                    {
                        { "i:ServicoId", "Serviço" },
                        { "s:BaseCalculo", "Base Cálculo" },
                        { "i:Periodo", "Período" },
                        { "i:QtdeDias", "Qtde  Dias" },
                        { "s:DescricaoValor", "Descrição Valor" },
                        { "s:Moeda", "Moeda" },
                        { "f:Valor", "Valor" },
                        { "f:Valor20", "Valor 20" },
                        { "f:Valor40", "Valor 40" },
                        { "s:Margem", "Margem" }
                    };

                    break;

                case TipoRegistro.SERVIÇO_PARA_MARGEM:

                    campos = new Dictionary<string, string>()
                    {
                        { "i:ServicoId", "Serviço" },
                        { "s:BaseCalculo", "Base Cálculo" },
                        { "s:TipoCarga", "Tipo Carga" },
                        { "f:ValorMargemDireita", "Valor Margem Direita" },
                        { "f:ValorMargemEsquerda", "Valor Margem Esquerda" },
                        { "f:ValorEntreMargens", "Valor Entre Margens" },
                        { "f:PesoMaximo", "Peso Máximo" },
                        { "p:AdicionalIMO", "Adicional IMO" },
                        { "p:AdicionalPeso", "Adicional Peso" },
                        { "s:DescricaoValor", "Descrição Valor" },
                        { "s:Moeda", "Moeda" }
                    };

                    break;

                case TipoRegistro.MINIMO_PARA_MARGEM:

                    campos = new Dictionary<string, string>()
                    {
                        { "i:ServicoId", "Serviço" },
                        { "f:ValorMinimoMargemDireita", "Valor Mínimo Margem Direita" },
                        { "f:ValorMinimoMargemEsquerda", "Valor Mínimo Margem Esquerda" },
                        { "f:ValorMinimoEntreMargens", "Valor Mínimo Entre Margens" },
                        { "i:LinhaReferencia", "Linha Referência" },
                        { "s:DescricaoValor", "Descrição Valor" }
                    };

                    break;

                case TipoRegistro.SERVICO_MECANICA_MANUAL:

                    campos = new Dictionary<string, string>()
                    {
                        { "i:ServicoId", "Serviço" },
                        { "s:BaseCalculo", "Base Cálculo" },
                        { "f:Valor", "Valor" },
                        { "f:Valor20", "Valor 20" },
                        { "f:Valor40", "Valor 40" },
                        { "p:AdicionalIMO", "Adicional IMO" },
                        { "s:Moeda", "Moeda" },
                        { "f:PesoMaximo", "Peso Máximo" },
                        { "p:AdicionalPeso", "Adicional Peso" },
                        { "s:TipoTrabalho", "Tipo Trabalho" },
                        { "s:DescricaoValor", "Descrição Valor" }
                    };

                    break;

                case TipoRegistro.MINIMO_MECANICA_MANUAL:

                    campos = new Dictionary<string, string>()
                    {
                        { "f:ValorMinimo", "Valor Mínimo" },
                        { "f:ValorMinimo20", "Valor Mínimo 20" },
                        { "f:ValorMinimo40", "Valor Mínimo  40" },
                        { "i:LinhaReferencia", "Linha Referência" },
                        { "s:DescricaoValor", "Descrição Valor" }
                    };

                    break;

                case TipoRegistro.SERVICO_LIBERACAO:

                    campos = new Dictionary<string, string>()
                    {
                        { "i:ServicoId", "Serviço" },
                        { "s:BaseCalculo", "Base Cálculo" },
                        { "f:Valor", "Valor" },
                        { "f:Valor20", "Valor 20" },
                        { "f:Valor40", "Valor 40" },
                        { "s:Reembolso", "Reembolso" },
                        { "s:Moeda", "Moeda" },
                        { "p:AdicionalIMO", "Adicional IMO" },
                        { "s:DescricaoValor", "Descrição Valor" }
                    };

                    break;

                case TipoRegistro.SERVICO_HUBPORT:

                    campos = new Dictionary<string, string>()
                    {
                        { "i:ServicoId", "Serviço" },
                        { "s:BaseCalculo", "Base Cálculo" },
                        { "f:Valor", "Valor" },
                        { "s:Origem", "Origem" },
                        { "s:Destino", "Destino" },
                        { "s:Moeda", "Moeda" },
                        { "s:DescricaoValor", "Descrição Valor" }
                    };

                    break;

                case TipoRegistro.GERAIS:

                    campos = new Dictionary<string, string>()
                    {
                        { "i:ServicoId", "Serviço" },
                        { "s:BaseCalculo", "Base Cálculo" },
                        { "f:Valor", "Valor" },
                        { "f:Valor20", "Valor 20" },
                        { "f:Valor40", "Valor 40" },
                        { "s:Moeda", "Moeda" },
                        { "s:DescricaoValor", "Descrição Valor" }
                    };

                    break;

                case TipoRegistro.MINIMO_GERAL:

                    campos = new Dictionary<string, string>()
                    {
                        { "f:ValorMinimo", "Valor Mínimo" },
                        { "f:ValorMinimo20", "Valor Mínimo 20" },
                        { "f:ValorMinimo40", "Valor Mínimo 40" },
                        { "i:LinhaReferencia", "Linha Referência" },
                        { "s:DescricaoValor", "Descrição Valor" }
                    };

                    break;

                case TipoRegistro.CONDICAO_GERAL:

                    campos = new Dictionary<string, string>()
                    {
                        { "s:CondicoesGerais", "Condições Gerais" }
                    };

                    break;

                case TipoRegistro.CONDICAO_INICIAL:

                    campos = new Dictionary<string, string>()
                    {
                        { "s:CondicoesIniciais", "Condições Iniciais" }
                    };

                    break;

                default:
                    break;
            }

            return Json(campos.OrderBy(c => c.Value), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ObterCamposOportunidade()
        {
            //tipoDeDados:NomeCampoView / Valor exibição

            var campos = new Dictionary<string, string>()
            {
                { "s:Identificacao", "Identificação" },
                { "s:Descricao", "Descrição" },
                { "s:StatusOportunidade", "Status Oportunidade" },
                { "s:SucessoNegociacao", "Sucesso Negociação" },
                { "s:EstagioNegociacao", "Estágio Negociação" },
                { "s:TipoDeProposta", "Tipo de Proposta" },
                { "s:TipoServico", "Tipo de Serviço" },
                { "d:DataFechamento", "DataFechamento" },
                { "i:TabelaId", "ID Tabela" },
                { "d:DataInicio", "Data Início" },
                { "d:DataTermino", "Data Término" },
                { "f:Probabilidade", "Probabilidade" },
                { "s:ClassificacaoCliente", "Classificação Cliente" },
                { "s:Segmento", "Segmento" },
                { "s:Motivoperda", "Motivo Perda" },
                { "s:TipoNegocio", "Tipo Negócio" },
                { "s:TipoOperacaoOportunidade", "Tipo Operação Oportunidade" },
                { "s:Observacao", "Observação" },
                { "f:FaturamentoMensalLCL", "Faturamento Mensal LCL" },
                { "f:FaturamentoMensalFCL", "Faturamento Mensal FCL" },
                { "f:VolumeMensal", "Volume Mensal" },
                { "f:CIFMedio","CIF Médio" },
                { "s:PremioParceria", "Prêmio Parceria" },
                { "s:TipoOperacao", "Tipo Operação" },
                { "s:FormaPagamento", "Forma Pagamento" },
                { "i:DiasFreeTime", "Dias Free Time" },
                { "i:QtdeDias", "Qtde Dias" },
                { "i:Validade", "Validade" },
                { "s:TipoValidade", "Tipo Validade" },
                { "s:Conta", "Conta" },
                { "s:ContaDocumento", "Conta Documento" },
                { "s:ContaEndereco", "Conta Endereço" },
                { "s:ContaCidade", "Conta Cidade" },
                { "s:ContaBairro", "Conta Bairro" },                
                { "s:ContaEstado", "Conta Estado" },
                { "s:ContaComplemento", "Conta Complemento" },
                { "s:ContaCEP", "Conta CEP" },
                { "s:ContaTelefone", "Conta Telefone" },
                { "s:Contato", "Contato" },
                { "s:ContatoEmail", "Contato Email" },
                { "s:ContatoTelefone", "Contato Telefone" },
                { "s:ContatoDepartamento", "Contato Departamento" },
                { "s:ContatoCargo", "Contato Cargo" },
                { "s:Revisao", "Revisão Id" },
                { "s:RevisaoDescricao", "Revisão Descrição" },
                { "s:Mercadoria", "Mercadoria" },
                { "s:Modelo", "Modelo" },
                { "s:Vendedor", "Vendedor" },
                { "s:Imposto", "Imposto" },
                { "l:SubClientes [Importador]", "Sub Clientes (Importador)" },
                { "l:SubClientes [Despachante]", "Sub Clientes (Despachante)" },
                { "l:SubClientes [Freight_Forwarder]", "Sub Clientes (Freight Forwarder)" },
                { "l:SubClientes [Coloader]", "Sub Clientes (Coloader)" },
                { "l:SubClientes [Co_Coloader1]", "Sub Clientes (Co-Coloader 1)" },
                { "l:SubClientes [Co_Coloader2]", "Sub Clientes (Co-Coloader 2)" },
                { "l:GruposCNPJ", "Grupos CNPJ" },
                { "s:Lote", "Lote" },
                { "s:Conteiner", "Conteiner" },
                { "s:BL", "BL" },
                { "i:TabelaReferencia", "Tabela Ref. ID" },
            };

            return Json(campos.OrderBy(c => c.Value), JsonRequestBehavior.AllowGet);
        }
    }
}