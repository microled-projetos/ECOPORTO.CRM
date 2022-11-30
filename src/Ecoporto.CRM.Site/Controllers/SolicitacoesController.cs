using Ecoporto.CRM.Business.DTO;
using Ecoporto.CRM.Business.Enums;
using Ecoporto.CRM.Business.Extensions;
using Ecoporto.CRM.Business.Filtros;
using Ecoporto.CRM.Business.Helpers;
using Ecoporto.CRM.Business.Interfaces.Repositorios;
using Ecoporto.CRM.Business.Models;
using Ecoporto.CRM.Infra.Datatables;
using Ecoporto.CRM.Sharepoint.Models;
using Ecoporto.CRM.Site.Extensions;
using Ecoporto.CRM.Site.Filtros;
using Ecoporto.CRM.Site.Models;
using Ecoporto.CRM.Site.Services;
using Ecoporto.CRM.Workflow.Enums;
using Ecoporto.CRM.Workflow.Models;
using Ecoporto.CRM.Workflow.Services;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace Ecoporto.CRM.Site.Controllers
{
    [Authorize]
    public class SolicitacoesController : BaseController
    {
        private readonly ISolicitacoesRepositorio _solicitacaoRepositorio;
        private readonly IMotivosRepositorio _motivosRepositorio;
        private readonly IOcorrenciasRepositorio _ocorrenciasRepositorio;
        private readonly INotaFiscalRepositorio _notaFiscalRepositorio;
        private readonly IContaRepositorio _contaRepositorio;
        private readonly IServicoFaturamentoRepositorio _servicoFaturamentoRepositorio;
        private readonly IParametrosRepositorio _parametrosRepositorio;
        private readonly IGRRepositorio _grRepositorio;
        private readonly IMinutaRepositorio _minutaRepositorio;
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IParceiroRepositorio _parceiroRepositorio;
        private readonly ILoteRepositorio _loteRepositorio;
        private readonly IBancoRepositorio _bancoRepositorio;
        private readonly IWorkflowRepositorio _workflowRepositorio;
        private readonly IBookingRepositorio _bookingRepositorio;
        private readonly IAnexoRepositorio _anexoRepositorio;
        private readonly ICondicaoPagamentoFaturaRepositorio _condicaoPagamentoFaturaRepositorio;

        public SolicitacoesController(
            ISolicitacoesRepositorio solicitacaoRepositorio,
            IMotivosRepositorio motivosRepositorio,
            IOcorrenciasRepositorio ocorrenciasRepositorio,
            INotaFiscalRepositorio notaFiscalRepositorio,
            IContaRepositorio contaRepositorio,
            IServicoFaturamentoRepositorio servicoFaturamentoRepositorio,
            IParametrosRepositorio parametrosRepositorio,
            IBancoRepositorio bancoRepositorio,
            IGRRepositorio grRepositorio,
            IUsuarioRepositorio usuarioRepositorio,
            IParceiroRepositorio parceiroRepositorio,
            ILoteRepositorio loteRepositorio,
            IMinutaRepositorio minutaRepositorio,
            IWorkflowRepositorio workflowRepositorio,
            IBookingRepositorio bookingRepositorio,
            IAnexoRepositorio anexoRepositorio,
            ICondicaoPagamentoFaturaRepositorio condicaoPagamentoFaturaRepositorio,
            ILogger logger) : base(logger)
        {
            _solicitacaoRepositorio = solicitacaoRepositorio;
            _motivosRepositorio = motivosRepositorio;
            _ocorrenciasRepositorio = ocorrenciasRepositorio;
            _notaFiscalRepositorio = notaFiscalRepositorio;
            _contaRepositorio = contaRepositorio;
            _servicoFaturamentoRepositorio = servicoFaturamentoRepositorio;
            _parametrosRepositorio = parametrosRepositorio;
            _grRepositorio = grRepositorio;
            _minutaRepositorio = minutaRepositorio;
            _usuarioRepositorio = usuarioRepositorio;
            _parceiroRepositorio = parceiroRepositorio;
            _loteRepositorio = loteRepositorio;
            _bancoRepositorio = bancoRepositorio;
            _workflowRepositorio = workflowRepositorio;
            _bookingRepositorio = bookingRepositorio;
            _anexoRepositorio = anexoRepositorio;
            _condicaoPagamentoFaturaRepositorio = condicaoPagamentoFaturaRepositorio;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [UsuarioExternoFilter]
        public ActionResult Consultar(JQueryDataTablesParamViewModel Params, SolicitacoesFiltro filtro)
        {
            SolicitacoesUsuarioExternoFiltro filtroUsuarioExterno = null;

            if (ViewBag.UsuarioExternoId != null)
            {
                var clientesVinculados = _usuarioRepositorio.ObterVinculosContas(ViewBag.UsuarioExternoId);

                if (clientesVinculados != null)
                {
                    filtroUsuarioExterno = new SolicitacoesUsuarioExternoFiltro(
                        ViewBag.UsuarioExternoId,
                        User.IsInRole("SolicitacoesCancelamento:Acessar"),
                        User.IsInRole("SolicitacoesDesconto:Acessar"),
                        User.IsInRole("SolicitacoesProrrogacao:Acessar"),
                        User.IsInRole("SolicitacoesRestituicao:Acessar"),
                        clientesVinculados);
                }
            }

            IEnumerable<dynamic> solicitacoes;
            int totalFiltro = 0;

            if (filtroUsuarioExterno == null)
            {
                solicitacoes = _solicitacaoRepositorio
                    .ObterSolicitacoes(Params.Pagina, Params.iDisplayLength, filtro, Params.OrderBy, out totalFiltro)
                    .Select(c => new
                    {
                        c.Id,
                        TipoSolicitacao = c.TipoSolicitacao.ToName(),
                        StatusSolicitacao = c.StatusSolicitacao.ToName(),
                        UnidadeSolicitacao = c.UnidadeSolicitacaoDescricao,
                        c.Ocorrencia,
                        c.Cliente,
                        AreaOcorrenciaSolicitacao = c.AreaOcorrenciaSolicitacao.ToName(),
                        TipoOperacao = c.TipoOperacaoDescricao
                    });
            }
            else
            {
                solicitacoes = _solicitacaoRepositorio
                    .ObterSolicitacoesAcessoExterno(Params.Pagina, Params.iDisplayLength, filtro, Params.OrderBy, out totalFiltro, filtroUsuarioExterno)
                    .Select(c => new
                    {
                        c.Id,
                        TipoSolicitacao = c.TipoSolicitacao.ToName(),
                        StatusSolicitacao = c.StatusSolicitacao.ToName(),
                        UnidadeSolicitacao = c.UnidadeSolicitacaoDescricao,
                        c.Ocorrencia,
                        c.Cliente,
                        AreaOcorrenciaSolicitacao = c.AreaOcorrenciaSolicitacao.ToName(),
                        TipoOperacao = c.TipoOperacaoDescricao
                    });
            }

            var totalRegistros = _solicitacaoRepositorio.ObterTotalSolicitacoes();

            var resultado = new
            {
                sEcho = Params.sEcho,
                iTotalRecords = totalRegistros,
                iTotalDisplayRecords = totalFiltro,
                aaData = solicitacoes
            };

            return Json(resultado, JsonRequestBehavior.AllowGet);
        }

        public void PopularUnidadesSolicitacao(SolicitacaoComercialViewModel viewModel)
        {
            var unidades = _solicitacaoRepositorio
                .ObterUnidadesSolicitacao();

            if (User.IsInRole("UsuarioExterno"))
            {
                unidades = unidades.Where(c => c.Id != 3);
            }

            viewModel.UnidadesSolicitacao = unidades.ToList();
        }

        public void PopularTiposOperacaoSolicitacao(SolicitacaoComercialViewModel viewModel)
        {
            var tiposOperacao = _solicitacaoRepositorio
               .ObterTiposOperacaoSolicitacao();

            if (User.IsInRole("UsuarioExterno"))
            {
                tiposOperacao = tiposOperacao.Where(c => c.Id == 1 || c.Id == 3);
            }

            viewModel.TiposOperacaoSolicitacao = tiposOperacao.ToList();
        }

        [HttpGet]
        public ActionResult Cadastrar(int? conta)
        {
            var solicitacaoComercialMotivosViewModel = new SolicitacaoComercialMotivosViewModel();

            var solicitacaoComercialOcorrenciasViewModel = new SolicitacaoComercialOcorrenciasViewModel();

            var solicitacaoComercialProrrogacaoViewModel = new SolicitacaoComercialProrrogacaoViewModel();

            var solicitacaoComercialRestituicaoViewModel = new SolicitacaoComercialRestituicaoViewModel();

            var solicitacaoComercialDescontoViewModel = new SolicitacaoComercialDescontoViewModel();

            var viewModel = new SolicitacaoComercialViewModel
            {
                SolicitacaoComercialMotivosViewModel = solicitacaoComercialMotivosViewModel,
                SolicitacaoComercialOcorrenciasViewModel = solicitacaoComercialOcorrenciasViewModel,
                SolicitacaoComercialProrrogacaoViewModel = solicitacaoComercialProrrogacaoViewModel,
                SolicitacaoComercialRestituicaoViewModel = solicitacaoComercialRestituicaoViewModel,
                SolicitacaoComercialDescontoViewModel = solicitacaoComercialDescontoViewModel,
                StatusSolicitacao = StatusSolicitacao.NOVO
            };

            PopularUnidadesSolicitacao(viewModel);
            PopularTiposOperacaoSolicitacao(viewModel);

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Cadastrar([Bind(Include = "TipoSolicitacao, StatusSolicitacao, UnidadeSolicitacaoId, OcorrenciaId, AreaOcorrenciaSolicitacao, TipoOperacaoId, MotivoId, Justificativa")] SolicitacaoComercialViewModel viewModel)
        {
            var solicitacao = new SolicitacaoComercial(
                viewModel.TipoSolicitacao,
                viewModel.UnidadeSolicitacaoId,
                viewModel.AreaOcorrenciaSolicitacao,
                viewModel.TipoOperacaoId,
                viewModel.OcorrenciaId,
                viewModel.MotivoId,
                viewModel.Justificativa,
                User.ObterId(),
                User.IsInRole("UsuarioExterno"));

            solicitacao.AdicionarMotivos(
                PopularMotivosPorTipoSolicitacao(viewModel.TipoSolicitacao));

            solicitacao.AdicionarOcorrencias(
                PopularOcorrenciasPorTipoSolicitacao(viewModel.TipoSolicitacao));
             
            viewModel.StatusSolicitacao = StatusSolicitacao.NOVO;
            if (string.IsNullOrEmpty(viewModel.TipoOperacaoId.ToString()))
            {
                viewModel.TipoOperacaoId = 0;
                ModelState.AddModelError("TipoOperacao", "Selecione o Tipo de Operação");
            }
            if (viewModel.TipoOperacaoId.ToString()=="0")
            {
                ModelState.AddModelError("TipoOperacao", "Selecione o Tipo de Operação");
            }
            if (viewModel.TipoOperacaoId.ToString() != "0")
            {
                if (ModelState.IsValid)
                {
                    if (Validar(solicitacao))
                    {
                        solicitacao.Id = _solicitacaoRepositorio.Cadastrar(solicitacao);

                        TempData["Sucesso"] = true;

                        GravarLogAuditoria(TipoLogAuditoria.INSERT, solicitacao);

                        return RedirectToAction(nameof(Atualizar), new { solicitacao.Id });
                    }
                }
            }
            viewModel.SolicitacaoComercialMotivosViewModel.Motivos = solicitacao.Motivos;

            viewModel.SolicitacaoComercialOcorrenciasViewModel.Ocorrencias = solicitacao.Ocorrencias;

            PopularTiposPesquisa(solicitacao);
            PopularUnidadesSolicitacao(viewModel);
            PopularTiposOperacaoSolicitacao(viewModel);

            return View(viewModel);
        }

        private IEnumerable<dynamic> ObterTiposPesquisa(Func<TipoPesquisa, bool> expressao)
        {
            return Enum.GetValues(typeof(TipoPesquisa))
                .Cast<TipoPesquisa>()
                .Where(expressao)
                .Select(v => new
                {
                    Id = ((int)v).ToString(),
                    Descricao = v.ToString()
                });
        }

        private void PopularTiposPesquisa(SolicitacaoComercial solicitacao)
        {
            List<dynamic> tiposPesquisa = null;

            if (solicitacao.TipoSolicitacao == TipoSolicitacao.OUTROS)
            {
                tiposPesquisa = ObterTiposPesquisa(c => c == TipoPesquisa.GR || c == TipoPesquisa.LOTE).ToList();
            }
            else
            {
                if (solicitacao.TipoOperacao == 1)
                {
                    tiposPesquisa = ObterTiposPesquisa(c => c == TipoPesquisa.GR
                        || c == TipoPesquisa.LOTE || c == TipoPesquisa.NF || c == TipoPesquisa.BL).ToList();
                }
                else if (solicitacao.TipoOperacao == 2)
                {
                    tiposPesquisa = ObterTiposPesquisa(c => c == TipoPesquisa.BOOKING
                        || c == TipoPesquisa.MINUTA || c == TipoPesquisa.NF).ToList();
                }
                else if (solicitacao.TipoOperacao == 4)
                {
                    tiposPesquisa = ObterTiposPesquisa(c => c == TipoPesquisa.FATURA).ToList();
                }
                else if (solicitacao.TipoOperacao == 3)
                {
                    tiposPesquisa = ObterTiposPesquisa(c => c == TipoPesquisa.BOOKING
                        || c == TipoPesquisa.GR || c == TipoPesquisa.NF).ToList();
                }
                else
                {
                    tiposPesquisa = ObterTiposPesquisa(c => c == TipoPesquisa.GR
                        || c == TipoPesquisa.LOTE || c == TipoPesquisa.NF).ToList();
                }
            }

            ViewBag.TipoPesquisa = new MultiSelectList(tiposPesquisa, "Id", "Descricao");
        }

        private void ObterDadosUsuario(SolicitacaoComercial solicitacao)
        {
            var usuario = _usuarioRepositorio.ObterUsuarioPorId(solicitacao.CriadoPor);

            if (usuario != null)
            {
                ViewBag.UsuarioCriacao = usuario.Externo ? usuario.LoginExterno : usuario.Login;
                ViewBag.DataCriacao = solicitacao.DataCriacao.DataHoraFormatada();
            }
        }

        [HttpGet]
        [UsuarioExternoFilter]
        public ActionResult Atualizar(int? id)
        {
            Session["FontePagadoraId"] = id.HasValue;

            if (id == null)
                return RedirectToAction(nameof(Index));

            var solicitacao = _solicitacaoRepositorio.ObterSolicitacaoPorId(id.Value);

            if (solicitacao == null)
                RegistroNaoEncontrado();

            var viewModel = new SolicitacaoComercialViewModel
            {
                Id = solicitacao.Id,
                TipoSolicitacao = solicitacao.TipoSolicitacao,
                UnidadeSolicitacaoId = solicitacao.UnidadeSolicitacao,
                StatusSolicitacao = solicitacao.StatusSolicitacao,
                AreaOcorrenciaSolicitacao = solicitacao.AreaOcorrenciaSolicitacao,
                TipoOperacaoId = solicitacao.TipoOperacao,
                ValorDevido = solicitacao.ValorDevido.ToString("n2"),
                ValorCobrado = solicitacao.ValorCobrado.ToString("n2"),
                ValorCredito = solicitacao.ValorCredito.ToString("n2"),
                HabilitaValorDevido = solicitacao.HabilitaValorDevido,
                Justificativa = solicitacao.Justificativa
            };

            if (ViewBag.UsuarioExternoId != null)
            {
                var clientesVinculados = _usuarioRepositorio.ObterVinculosContas((int)ViewBag.UsuarioExternoId).ToList();

                if (clientesVinculados != null)
                {
                    var cnpjsVinculados = clientesVinculados.Select(c => c.ContaDocumento);

                    switch (solicitacao.TipoSolicitacao)
                    {
                        case TipoSolicitacao.CANCELAMENTO_NF:

                            var cancelamentos = _solicitacaoRepositorio
                                .ObterCancelamentosNF(solicitacao.Id);

                            if (solicitacao.CriadoPor != (int)ViewBag.UsuarioExternoId)
                            {
                                var existeCancelamento = cancelamentos
                                    .Where(c => cnpjsVinculados.Contains(c.Documento) || cnpjsVinculados.Contains(c.IndicadorDocumento)).Any();

                                if (existeCancelamento == false)
                                {
                                    return RedirectToAction(nameof(Index));
                                }
                            }

                            break;
                        case TipoSolicitacao.DESCONTO:

                            var descontos = _solicitacaoRepositorio
                                .ObterDescontos(solicitacao.Id);

                            if (solicitacao.CriadoPor != (int)ViewBag.UsuarioExternoId)
                            {
                                var existeDesconto = descontos
                                    .Where(c => cnpjsVinculados.Contains(c.IndicadorDocumento) || cnpjsVinculados.Contains(c.ClienteDocumento)).Any();

                                if (existeDesconto == false)
                                {
                                    return RedirectToAction(nameof(Index));
                                }
                            }

                            break;
                        case TipoSolicitacao.PRORROGACAO_BOLETO:

                            var prorrogacoes = _solicitacaoRepositorio
                                .ObterProrrogacoes(solicitacao.Id);

                            if (solicitacao.CriadoPor != (int)ViewBag.UsuarioExternoId)
                            {
                                var existeProrrogacao = prorrogacoes
                                    .Where(c => cnpjsVinculados.Contains(c.Documento)).Any();

                                if (existeProrrogacao == false)
                                {
                                    return RedirectToAction(nameof(Index));
                                }
                            }

                            break;
                        case TipoSolicitacao.RESTITUICAO:

                            var restituicoes = _solicitacaoRepositorio
                                .ObterRestituicoes(solicitacao.Id);

                            if (solicitacao.CriadoPor != (int)ViewBag.UsuarioExternoId)
                            {
                                var existeRestituicao = restituicoes
                                    .Where(c => cnpjsVinculados.Contains(c.FavorecidoDocumento)).Any();

                                if (existeRestituicao == false)
                                {
                                    return RedirectToAction(nameof(Index));
                                }
                            }

                            break;
                        case TipoSolicitacao.OUTROS:
                            break;
                    }
                }
            }

            PopularTiposPesquisa(solicitacao);
            PopularUnidadesSolicitacao(viewModel);
            PopularTiposOperacaoSolicitacao(viewModel);

            var solicitacaoComercialMotivosViewModel = new SolicitacaoComercialMotivosViewModel();

            var solicitacaoComercialOcorrenciasViewModel = new SolicitacaoComercialOcorrenciasViewModel();

            var solicitacaoComercialCancelamentoNFViewModel = new SolicitacaoComercialCancelamentoNFViewModel
            {
                CancelamentoNFSolicitacaoId = solicitacao.Id,
                CancelamentoNFUnidadeSolicitacao = solicitacao.UnidadeSolicitacao,
                CancelamentoTipoOperacaoSolicitacao = solicitacao.TipoOperacao,
                CancelamentoStatusSolicitacao = solicitacao.StatusSolicitacao
            };

            var solicitacaoComercialProrrogacaoViewModel = new SolicitacaoComercialProrrogacaoViewModel
            {
                ProrrogacaoSolicitacaoId = solicitacao.Id,
                ProrrogacaoUnidadeSolicitacao = solicitacao.UnidadeSolicitacao,
                ProrrogacaoTipoOperacaoSolicitacao = solicitacao.TipoOperacao,
                ProrrogacaoStatusSolicitacao = solicitacao.StatusSolicitacao
            };

            var solicitacaoComercialRestituicaoViewModel = new SolicitacaoComercialRestituicaoViewModel
            {
                RestituicaoSolicitacaoId = solicitacao.Id,
                RestituicaoUnidadeSolicitacao = solicitacao.UnidadeSolicitacao,
                RestituicaoTipoOperacaoSolicitacao = solicitacao.TipoOperacao,
                RestituicaoStatusSolicitacao = solicitacao.StatusSolicitacao,
            };

            var solicitacaoComercialDescontoViewModel = new SolicitacaoComercialDescontoViewModel
            {
                DescontoSolicitacaoId = solicitacao.Id,
                DescontoUnidadeSolicitacao = solicitacao.UnidadeSolicitacao,
                DescontoTipoOperacaoSolicitacao = solicitacao.TipoOperacao,
                DescontoStatusSolicitacao = solicitacao.StatusSolicitacao
            };

            var solicitacaoComercialAnexosViewModel = new SolicitacaoComercialAnexosViewModel
            {
                AnexoSolicitacaoId = solicitacao.Id
            };

            var solicitacaoComercialAlteracaoFormaPagamentoViewModel = new SolicitacaoComercialAlteracaoFormaPagamentoViewModel
            {
                AlteracaoFormaPagamentoSolicitacaoId = solicitacao.Id,
                AlteracaoFormaPagamentoUnidadeSolicitacao = solicitacao.UnidadeSolicitacao,
                AlteracaoFormaPagamentoTipoOperacaoSolicitacao = solicitacao.TipoOperacao
            };

            viewModel.SolicitacaoComercialMotivosViewModel = solicitacaoComercialMotivosViewModel;
            viewModel.SolicitacaoComercialOcorrenciasViewModel = solicitacaoComercialOcorrenciasViewModel;
            viewModel.SolicitacaoComercialCancelamentoNFViewModel = solicitacaoComercialCancelamentoNFViewModel;
            viewModel.SolicitacaoComercialProrrogacaoViewModel = solicitacaoComercialProrrogacaoViewModel;
            viewModel.SolicitacaoComercialRestituicaoViewModel = solicitacaoComercialRestituicaoViewModel;
            viewModel.SolicitacaoComercialDescontoViewModel = solicitacaoComercialDescontoViewModel;
            viewModel.SolicitacaoComercialAnexosViewModel = solicitacaoComercialAnexosViewModel;
            viewModel.SolicitacaoComercialAlteracaoFormaPagamentoViewModel = solicitacaoComercialAlteracaoFormaPagamentoViewModel;

            viewModel.SolicitacaoComercialMotivosViewModel.Motivos = PopularMotivosPorTipoSolicitacao(viewModel.TipoSolicitacao);
            viewModel.SolicitacaoComercialOcorrenciasViewModel.Ocorrencias = PopularOcorrenciasPorTipoSolicitacao(viewModel.TipoSolicitacao);

            viewModel.OcorrenciaId = solicitacao.OcorrenciaId;
            viewModel.MotivoId = solicitacao.MotivoId;

            PopularCancelamentosNF(solicitacaoComercialCancelamentoNFViewModel);
            PopularProrrogacoes(solicitacaoComercialProrrogacaoViewModel);
            PopularRestituicao(solicitacaoComercialRestituicaoViewModel);
            PopularDescontos(solicitacaoComercialDescontoViewModel);
            PopularBancos(solicitacaoComercialRestituicaoViewModel);
            PopularAnexos(solicitacaoComercialAnexosViewModel);
            PopularAlteracoesFormaPgto(solicitacaoComercialAlteracaoFormaPagamentoViewModel);
            PopularCondicoesPagamento(solicitacaoComercialAlteracaoFormaPagamentoViewModel);

            ObterDadosUsuario(solicitacao);

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Atualizar([Bind(Include = "Id, TipoSolicitacao, StatusSolicitacao, UnidadeSolicitacaoId, OcorrenciaId, AreaOcorrenciaSolicitacao, TipoOperacaoId, MotivoId, Justificativa")] SolicitacaoComercialViewModel viewModel, int? id)
        {
            if (id == null)
                return RedirectToAction(nameof(Index));

            var solicitacao = _solicitacaoRepositorio.ObterSolicitacaoPorId(id.Value);

            if (solicitacao == null)
                RegistroNaoEncontrado();

            if (viewModel.UnidadeSolicitacaoId != solicitacao.UnidadeSolicitacao)
            {
                if (viewModel.TipoSolicitacao == TipoSolicitacao.CANCELAMENTO_NF)
                {
                    var registrosCancelamento = _solicitacaoRepositorio.ObterCancelamentosNF(solicitacao.Id);

                    if (registrosCancelamento.Any())
                        ModelState.AddModelError(string.Empty, "Não é permitido a alteração de Unidade para Solicitações de Cancelamento que já contém registros");

                    viewModel.SolicitacaoComercialCancelamentoNFViewModel.CancelamentoNFSolicitacaoId = solicitacao.Id;

                    PopularCancelamentosNF(viewModel.SolicitacaoComercialCancelamentoNFViewModel);
                }
                else if (viewModel.TipoSolicitacao == TipoSolicitacao.DESCONTO)
                {
                    var registrosDesconto = _solicitacaoRepositorio.ObterDescontos(solicitacao.Id);

                    if (registrosDesconto.Any())
                        ModelState.AddModelError(string.Empty, "Não é permitido a alteração de Unidade para Solicitações de Desconto que já contém registros");

                    viewModel.SolicitacaoComercialDescontoViewModel.DescontoSolicitacaoId = solicitacao.Id;

                    PopularDescontos(viewModel.SolicitacaoComercialDescontoViewModel);
                }
                else if (viewModel.TipoSolicitacao == TipoSolicitacao.PRORROGACAO_BOLETO)
                {
                    var registrosProrrogacao = _solicitacaoRepositorio.ObterProrrogacoes(solicitacao.Id);

                    if (registrosProrrogacao.Any())
                        ModelState.AddModelError(string.Empty, "Não é permitido a alteração de Unidade para Solicitações de Prorrogação que já contém registros");

                    viewModel.SolicitacaoComercialProrrogacaoViewModel.ProrrogacaoSolicitacaoId = solicitacao.Id;

                    PopularProrrogacoes(viewModel.SolicitacaoComercialProrrogacaoViewModel);
                }
                else if (viewModel.TipoSolicitacao == TipoSolicitacao.RESTITUICAO)
                {
                    var registrosRestituicao = _solicitacaoRepositorio.ObterRestituicoes(solicitacao.Id);

                    if (registrosRestituicao.Any())
                        ModelState.AddModelError(string.Empty, "Não é permitido a alteração de Unidade para Solicitações de Restituição que já contém registros");

                    viewModel.SolicitacaoComercialRestituicaoViewModel.RestituicaoSolicitacaoId = solicitacao.Id;

                    PopularRestituicao(viewModel.SolicitacaoComercialRestituicaoViewModel);
                    PopularBancos(viewModel.SolicitacaoComercialRestituicaoViewModel);
                }
            }

            if (!User.IsInRole("Administrador"))
            {
                if (solicitacao.CriadoPor != User.ObterId())
                {
                    ModelState.AddModelError(string.Empty, "A edição da solicitação é permitida apenas pelo usuário de criação.");

                    PopularTiposPesquisa(solicitacao);
                    PopularUnidadesSolicitacao(viewModel);
                    PopularTiposOperacaoSolicitacao(viewModel);
                    ObterDadosUsuario(solicitacao);

                    return View(viewModel);
                }
            }

            solicitacao.Alterar(new SolicitacaoComercial(
                viewModel.TipoSolicitacao,
                viewModel.UnidadeSolicitacaoId,
                viewModel.AreaOcorrenciaSolicitacao,
                viewModel.TipoOperacaoId,
                viewModel.OcorrenciaId,
                viewModel.MotivoId,
                viewModel.Justificativa,
                solicitacao.CriadoPor,
                false));

            if (ModelState.IsValid && Validar(solicitacao))
            {
                _solicitacaoRepositorio.Atualizar(solicitacao);

                GravarLogAuditoria(TipoLogAuditoria.UPDATE, solicitacao);

                return RedirectToAction(nameof(Atualizar), new { solicitacao.Id });
            }

            viewModel.SolicitacaoComercialOcorrenciasViewModel
                    .Ocorrencias = PopularOcorrenciasPorTipoSolicitacao(viewModel.TipoSolicitacao);

            viewModel.SolicitacaoComercialMotivosViewModel
                .Motivos = PopularMotivosPorTipoSolicitacao(viewModel.TipoSolicitacao);

            PopularTiposPesquisa(solicitacao);
            PopularUnidadesSolicitacao(viewModel);
            PopularTiposOperacaoSolicitacao(viewModel);

            PopularAnexos(viewModel.SolicitacaoComercialAnexosViewModel);

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Excluir(int id)
        {
            try
            {
                var solicitacao = _solicitacaoRepositorio.ObterSolicitacaoPorId(id);

                if (solicitacao == null)
                    RegistroNaoEncontrado();

                if (!User.IsInRole("Administrador"))
                {
                    if (solicitacao.CriadoPor != User.ObterId())
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"A Solicitação só pode ser excluída pelo usuário de criação ({solicitacao.CriadoPor})");
                }

                if (solicitacao.StatusSolicitacao != StatusSolicitacao.NOVO && solicitacao.StatusSolicitacao != StatusSolicitacao.REJEITADO)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"Exclusão não permitida para Solicitações com status igual {solicitacao.StatusSolicitacao.ToName()}");

                _solicitacaoRepositorio.Excluir(solicitacao.Id, solicitacao.TipoSolicitacao);

                GravarLogAuditoria(TipoLogAuditoria.DELETE, solicitacao);
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        private void PopularCancelamentosNF(SolicitacaoComercialCancelamentoNFViewModel solicitacaoComercialCancelamentoNFViewModel)
        {
            var cancelamentosNF = _solicitacaoRepositorio.ObterCancelamentosNF(solicitacaoComercialCancelamentoNFViewModel.CancelamentoNFSolicitacaoId);
            solicitacaoComercialCancelamentoNFViewModel.CancelamentosNF = cancelamentosNF.ToList();
        }

        private void PopularAlteracoesFormaPgto(SolicitacaoComercialAlteracaoFormaPagamentoViewModel solicitacaoComercialAlteracaoFormaPagamentoViewModel)
        {
            var alteracoesFormasPgto = _solicitacaoRepositorio.ObterAlteracoesFormaPagamento(solicitacaoComercialAlteracaoFormaPagamentoViewModel.AlteracaoFormaPagamentoSolicitacaoId);
            solicitacaoComercialAlteracaoFormaPagamentoViewModel.SolicitacoesAlteracaoFormaPgto = alteracoesFormasPgto.ToList();
        }

        private void PopularCondicoesPagamento(SolicitacaoComercialAlteracaoFormaPagamentoViewModel solicitacaoComercialAlteracaoFormaPagamentoViewModel)
        {
            solicitacaoComercialAlteracaoFormaPagamentoViewModel.CondicoesPagamentoFaturamento = _condicaoPagamentoFaturaRepositorio
                .ObterCondicoesPagamento()
                .ToList();
        }

        [HttpPost]
        public ActionResult CadastrarCancelamentoNF([Bind(Include = "CancelamentoNFId, CancelamentoNFSolicitacaoId, CancelamentoNFTipoPesquisa, CancelamentoNFTipoPesquisaNumero, CancelamentoNFLote, CancelamentoNFNotaFiscalId, CancelamentoNFNotaFiscal, CancelamentoNFValorNF, CancelamentoNFRazaoSocial, CancelamentoNFContaId, CancelamentoNFFormaPagamento, CancelamentoNFDataEmissao, CancelamentoNFDesconto, CancelamentoNFValorNovaNF, CancelamentoValorAPagar, CancelamentoNFDataProrrogacao")] SolicitacaoComercialCancelamentoNFViewModel viewModel)
        {
            if (viewModel.CancelamentoNFSolicitacaoId == 0)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var solicitacao = _solicitacaoRepositorio.ObterSolicitacaoPorId(viewModel.CancelamentoNFSolicitacaoId);

            if (solicitacao == null)
                RegistroNaoEncontrado();

            if (viewModel.CancelamentoNFDataProrrogacao != null)
            {
                if (viewModel.CancelamentoNFDataProrrogacao.Value.Date < DateTime.Now.Date)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "A Data de Prorrogação não pode ser menor que a data atual");
                }
            }

            var cancelamentoNF = new SolicitacaoCancelamentoNF(
                solicitacao.Id,
                viewModel.CancelamentoNFTipoPesquisa,
                viewModel.CancelamentoNFTipoPesquisaNumero,
                viewModel.CancelamentoNFLote,
                viewModel.CancelamentoNFNotaFiscalId,
                viewModel.CancelamentoNFNotaFiscal,
                viewModel.CancelamentoNFValorNF,
                viewModel.CancelamentoNFContaId,
                viewModel.CancelamentoNFRazaoSocial,
                viewModel.CancelamentoNFFormaPagamento,
                viewModel.CancelamentoNFDataEmissao,
                viewModel.CancelamentoNFDesconto,
                viewModel.CancelamentoNFValorNovaNF,
                viewModel.CancelamentoValorAPagar,
                viewModel.CancelamentoNFDataProrrogacao,
                User.ObterId(),
                solicitacao);

            var cancelamentoBusca = _solicitacaoRepositorio
                     .ObterCancelamentosNF(solicitacao.Id);

            if (viewModel.CancelamentoNFId == 0)
            {
                if (solicitacao.UnidadeSolicitacao != 1 && solicitacao.UnidadeSolicitacao != 2)
                {
                    if (cancelamentoBusca.Where(c => c.ContaId != viewModel.CancelamentoNFContaId).Any())
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Só é permitido cadastrar Cancelamentos para o mesmo Cliente");
                }

                if (viewModel.CancelamentoNFTipoPesquisa == TipoPesquisa.NF)
                {
                    if (cancelamentoBusca.Where(c => c.NFE == viewModel.CancelamentoNFTipoPesquisaNumero).Any())
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Já existe um registro com a mesma NFE");
                }

                if (cancelamentoBusca.Where(c => c.Lote != viewModel.CancelamentoNFLote).Any())
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Só é permitido cadastrar Cancelamentos para o mesmo Lote");
            }
            else
            {
                if (solicitacao.UnidadeSolicitacao != 1 && solicitacao.UnidadeSolicitacao != 2)
                {
                    if (cancelamentoBusca.Where(c => (c.ContaId != viewModel.CancelamentoNFContaId) && c.Id != viewModel.CancelamentoNFId).Any())
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Só é permitido cadastrar Cancelamentos para o mesmo Cliente");
                }

                if (viewModel.CancelamentoNFTipoPesquisa == TipoPesquisa.NF)
                {
                    if (cancelamentoBusca.Where(c => (c.NFE == viewModel.CancelamentoNFTipoPesquisaNumero) && c.Id != viewModel.CancelamentoNFId).Any())
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Já existe um registro com a mesma NFE");
                }

                if (cancelamentoBusca.Where(c => (c.Lote != viewModel.CancelamentoNFLote) && c.Id != viewModel.CancelamentoNFId).Any())
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Só é permitido cadastrar Cancelamentos para o mesmo Lote");
            }

            if (viewModel.CancelamentoNFId == 0)
            {
                if (!Validar(cancelamentoNF))
                    return RetornarErros();

                _solicitacaoRepositorio.CadastrarCancelamentoNF(cancelamentoNF);
            }
            else
            {
                var solicitacaoCancelamentoNFBusca = _solicitacaoRepositorio.ObterCancelamentoNFPorId(viewModel.CancelamentoNFId);

                if (solicitacaoCancelamentoNFBusca == null)
                    RegistroNaoEncontrado();

                if (!User.IsInRole("Administrador"))
                {
                    if (solicitacaoCancelamentoNFBusca.CriadoPor != User.ObterId())
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "A edição da solicitação é permitida apenas pelo usuário de criação.");
                }

                solicitacaoCancelamentoNFBusca.Alterar(cancelamentoNF);
                solicitacaoCancelamentoNFBusca.SolicitacaoComercial = solicitacao;

                if (!Validar(solicitacaoCancelamentoNFBusca))
                    return RetornarErros();

                _solicitacaoRepositorio.AtualizarCancelamentoNF(solicitacaoCancelamentoNFBusca);
            }

            var cancelamentosNF = _solicitacaoRepositorio.ObterCancelamentosNF(solicitacao.Id);

            return PartialView("_ConsultarCancelamentosNF", cancelamentosNF);
        }

        [HttpGet]
        [UsuarioExternoFilter]
        public ActionResult ObterNotasFiscaisPorTipoSolicitacao(TipoPesquisa tipoPesquisa, TipoOperacao tipoOperacao, string termoPesquisa)
        {
            SolicitacoesUsuarioExternoFiltro filtroUsuarioExterno = null;

            if (ViewBag.UsuarioExternoId != null)
            {
                var clientesVinculados = _usuarioRepositorio.ObterVinculosContas(ViewBag.UsuarioExternoId);

                if (clientesVinculados != null)
                {
                    filtroUsuarioExterno = new SolicitacoesUsuarioExternoFiltro(
                        ViewBag.UsuarioExternoId,
                        User.IsInRole("SolicitacoesCancelamento:Acessar"),
                        User.IsInRole("SolicitacoesDesconto:Acessar"),
                        User.IsInRole("SolicitacoesProrrogacao:Acessar"),
                        User.IsInRole("SolicitacoesRestituicao:Acessar"),
                        clientesVinculados);
                }
            }

            var notasFiscais = _solicitacaoRepositorio
                .ObterNotasFiscaisPorTipoPesquisa(tipoPesquisa, tipoOperacao, termoPesquisa, filtroUsuarioExterno);

            if (notasFiscais.Any())
            {
                if (User.IsInRole("UsuarioExterno"))
                {
                    var existe = false;

                    foreach (var nota in notasFiscais)
                    {
                        foreach (var cnpj in filtroUsuarioExterno.CnpjVinculados)
                        {
                            if (nota.CnpjImportador == cnpj.ContaDocumento)
                            {
                                existe = true;
                                break;
                            }
                        }

                        foreach (var cnpj in filtroUsuarioExterno.CnpjVinculados)
                        {
                            if (nota.CnpjCaptador == cnpj.ContaDocumento)
                            {
                                existe = true;
                            }
                        }
                    }

                    if (!existe)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Usuário não possui permissão para cadastrar este processo");
                    }
                }
            }

            var notas = notasFiscais
                .Select(c => new
                {
                    c.Id,
                    c.NFE,
                    DataEmissao = c.DataEmissao.ToString("dd/MM/yyyy")
                }).ToList();

            return Json(notas, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ObterDetalhesCancelamentoNF(int id)
        {
            var dadoFinanceiro = _solicitacaoRepositorio.ObterCancelamentoNFPorId(id);

            if (dadoFinanceiro != null)
            {
                return Json(new
                {
                    dadoFinanceiro.Id,
                    dadoFinanceiro.TipoPesquisa,
                    dadoFinanceiro.TipoPesquisaNumero,
                    dadoFinanceiro.Lote,
                    dadoFinanceiro.NotaFiscalId,
                    dadoFinanceiro.NFE,
                    dadoFinanceiro.RazaoSocial,
                    dadoFinanceiro.ContaId,
                    dadoFinanceiro.ContaDescricao,
                    dadoFinanceiro.FormaPagamento,
                    DescricaoFormaPagamento = dadoFinanceiro.FormaPagamento.ToName(),
                    Desconto = dadoFinanceiro.Desconto.ToString("n2"),
                    ValorNF = dadoFinanceiro.ValorNF.ToString("n2"),
                    ValorNovaNF = dadoFinanceiro.ValorNovaNF.ToString("n2"),
                    ValorAPagar = dadoFinanceiro.ValorAPagar.ToString("n2"),
                    DataEmissao = dadoFinanceiro.DataEmissao.DataFormatada(),
                    DataProrrogacao = dadoFinanceiro.DataProrrogacao.DataFormatada(),
                }, JsonRequestBehavior.AllowGet);
            }

            return null;
        }

        [HttpGet]
        public ActionResult ObterCancelamentoDefault(int solicitacaoId)
        {
            var registrosCancelamentos = _solicitacaoRepositorio
                .ObterCancelamentosNF(solicitacaoId);

            if (registrosCancelamentos.Count() == 1)
            {
                var cancelamento = registrosCancelamentos.FirstOrDefault();

                var dadoFinanceiro = _solicitacaoRepositorio.ObterCancelamentoNFPorId(cancelamento.Id);

                if (dadoFinanceiro != null)
                {
                    return Json(new
                    {
                        dadoFinanceiro.Id,
                        dadoFinanceiro.TipoPesquisa,
                        dadoFinanceiro.TipoPesquisaNumero,
                        dadoFinanceiro.Lote,
                        dadoFinanceiro.NotaFiscalId,
                        dadoFinanceiro.NFE,
                        dadoFinanceiro.RazaoSocial,
                        dadoFinanceiro.ContaId,
                        dadoFinanceiro.ContaDescricao,
                        dadoFinanceiro.FormaPagamento,
                        DescricaoFormaPagamento = dadoFinanceiro.FormaPagamento.ToName(),
                        Desconto = dadoFinanceiro.Desconto.ToString("n2"),
                        ValorNF = dadoFinanceiro.ValorNF.ToString("n2"),
                        ValorNovaNF = dadoFinanceiro.ValorNovaNF.ToString("n2"),
                        ValorAPagar = dadoFinanceiro.ValorAPagar.ToString("n2"),
                        DataEmissao = dadoFinanceiro.DataEmissao.DataFormatada(),
                        DataProrrogacao = dadoFinanceiro.DataProrrogacao.DataFormatada(),
                    }, JsonRequestBehavior.AllowGet);
                }
            }

            return null;
        }

        [HttpGet]
        public ActionResult ObterDetalhesNotaFiscal(int nfeId, int tipoOperacao)
        {
            NotaFiscal notaFiscal = new NotaFiscal();

            if (tipoOperacao != 3)
                notaFiscal = _notaFiscalRepositorio.ObterDetalhesNotaFiscal(nfeId);
            else
                notaFiscal = _notaFiscalRepositorio.ObterDetalhesNotaFiscalRedex(nfeId);

            if (notaFiscal != null)
            {
                return Json(new
                {
                    notaFiscal.NFE,
                    notaFiscal.Cliente,
                    notaFiscal.FormaPagamento,
                    notaFiscal.Documento,
                    notaFiscal.RPS,
                    notaFiscal.Reserva,
                    notaFiscal.Lote,
                    DescricaoFormaPagamento = notaFiscal.FormaPagamento.ToName(),
                    Valor = notaFiscal.Valor.ToString("n2"),
                    DataEmissao = notaFiscal.DataEmissao.DataFormatada(),
                    DataVencimento = notaFiscal.DataVencimento.DataFormatada()
                }, JsonRequestBehavior.AllowGet);
            }

            return null;
        }

        [HttpPost]
        public ActionResult ExcluirCancelamentoNF(int id)
        {
            var dadoFinanceiro = _solicitacaoRepositorio.ObterCancelamentoNFPorId(id);

            if (dadoFinanceiro == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Registro não encontrado ou já excluído");

            if (!User.IsInRole("Administrador"))
            {
                var usuarioCriacao = _usuarioRepositorio.ObterUsuarioPorId(dadoFinanceiro.CriadoPor);

                if (usuarioCriacao.Login.ToLower() != User.Identity.Name.ToLower())
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"O Cancelamento só pode ser excluído pelo usuário de criação ({usuarioCriacao.Login})");
            }

            _solicitacaoRepositorio.ExcluirCancelamentoNF(id);

            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        private void PopularProrrogacoes(SolicitacaoComercialProrrogacaoViewModel solicitacaoComercialProrrogacaoViewModel)
        {
            var prorrogacoes = _solicitacaoRepositorio.ObterProrrogacoes(solicitacaoComercialProrrogacaoViewModel.ProrrogacaoSolicitacaoId);
            solicitacaoComercialProrrogacaoViewModel.Prorrogacoes = prorrogacoes.ToList();
        }

        [HttpPost]
        public ActionResult CadastrarProrrogacao([Bind(Include = "ProrrogacaoId, ProrrogacaoSolicitacaoId, ProrrogacaoNotaFiscal, ProrrogacaoNotaFiscalId, ProrrogacaoValorNF, ProrrogacaoRazaoSocial, ProrrogacaoContaId, ProrrogacaoVencimentoOriginal, ProrrogacaoDataProrrogacao, ProrrogacaoNumeroProrrogacao, ProrrogacaoIsentarJuros, ProrrogacaoValorJuros, ProrrogacaoValorTotalComJuros, ProrrogacaoObservacoes")] SolicitacaoComercialProrrogacaoViewModel viewModel)
        {
            if (viewModel.ProrrogacaoSolicitacaoId == 0)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var solicitacao = _solicitacaoRepositorio.ObterSolicitacaoPorId(viewModel.ProrrogacaoSolicitacaoId);

            if (solicitacao == null)
                RegistroNaoEncontrado();

            var prorrogacao = new SolicitacaoProrrogacao(
                solicitacao.Id,
                viewModel.ProrrogacaoNotaFiscalId,
                viewModel.ProrrogacaoNotaFiscal,
                viewModel.ProrrogacaoValorNF,
                viewModel.ProrrogacaoRazaoSocial,
                viewModel.ProrrogacaoContaId,
                viewModel.ProrrogacaoVencimentoOriginal,
                viewModel.ProrrogacaoDataProrrogacao,
                viewModel.ProrrogacaoNumeroProrrogacao,
                viewModel.ProrrogacaoIsentarJuros,
                viewModel.ProrrogacaoValorJuros,
                viewModel.ProrrogacaoValorTotalComJuros,
                viewModel.ProrrogacaoObservacoes,
                User.ObterId(),
                solicitacao);

            var prorrogacoesBusca = _solicitacaoRepositorio
                .ObterProrrogacoes(solicitacao.Id);

            if (viewModel.ProrrogacaoId == 0)
            {
                if (prorrogacoesBusca.Where(c => c.NFE == viewModel.ProrrogacaoNotaFiscal).Any())
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Já existe um registro com a mesma NFE");

                if (solicitacao.UnidadeSolicitacao != 1 && solicitacao.UnidadeSolicitacao != 2)
                {
                    if (prorrogacoesBusca.Where(c => c.ContaId != viewModel.ProrrogacaoContaId).Any())
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Só é permitido cadastrar Prorrogações para o mesmo Cliente");
                }
                else
                {
                    if (solicitacao.TipoOperacao != 4)
                    {
                        if (prorrogacoesBusca.Where(c => c.RazaoSocial != viewModel.ProrrogacaoRazaoSocial).Any())
                            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Só é permitido cadastrar Prorrogações para o mesmo Cliente");
                    }
                    else
                    {
                        if (prorrogacoesBusca.Where(c => c.ContaId != viewModel.ProrrogacaoContaId).Any())
                            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Só é permitido cadastrar Prorrogações para o mesmo Cliente");
                    }
                }
            }
            else
            {
                if (prorrogacoesBusca.Where(c => (c.NFE == viewModel.ProrrogacaoNotaFiscal) && c.Id != viewModel.ProrrogacaoId).Any())
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Já existe um registro com a mesma NFE");

                if (solicitacao.UnidadeSolicitacao != 1 && solicitacao.UnidadeSolicitacao != 2)
                {
                    if (prorrogacoesBusca.Where(c => (c.ContaId != viewModel.ProrrogacaoContaId) && c.Id != viewModel.ProrrogacaoId).Any())
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Só é permitido cadastrar Prorrogações para o mesmo Cliente");
                }
                else
                {
                    if (solicitacao.TipoOperacao != 4)
                    {
                        if (prorrogacoesBusca.Where(c => (c.RazaoSocial != viewModel.ProrrogacaoRazaoSocial) && c.Id != viewModel.ProrrogacaoId).Any())
                            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Só é permitido cadastrar Prorrogações para o mesmo Cliente");
                    }
                    else
                    {
                        if (prorrogacoesBusca.Where(c => (c.ContaId != viewModel.ProrrogacaoContaId) && c.Id != viewModel.ProrrogacaoId).Any())
                            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Só é permitido cadastrar Prorrogações para o mesmo Cliente");
                    }
                }
            }

            if (viewModel.ProrrogacaoId == 0)
            {
                if (!Validar(prorrogacao))
                    return RetornarErros();

                _solicitacaoRepositorio.CadastrarProrrogacao(prorrogacao);
            }
            else
            {
                var prorrogacaoBusca = _solicitacaoRepositorio.ObterProrrogacaoPorId(viewModel.ProrrogacaoId);

                if (prorrogacaoBusca == null)
                    RegistroNaoEncontrado();

                if (!User.IsInRole("Administrador"))
                {
                    if (prorrogacaoBusca.CriadoPor != User.ObterId())
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "A edição da solicitação é permitida apenas pelo usuário de criação.");
                }

                prorrogacaoBusca.Alterar(prorrogacao);

                if (!Validar(prorrogacao))
                    return RetornarErros();

                _solicitacaoRepositorio.AtualizarProrrogacao(prorrogacaoBusca);
            }

            var prorrogacoes = _solicitacaoRepositorio.ObterProrrogacoes(solicitacao.Id);

            return PartialView("_ConsultarProrrogacoes", prorrogacoes);
        }

        [HttpGet]
        public ActionResult ObterDetalhesProrrogacao(int id)
        {
            var prorrogacao = _solicitacaoRepositorio.ObterProrrogacaoPorId(id);

            if (prorrogacao != null)
            {
                return Json(new
                {
                    prorrogacao.Id,
                    prorrogacao.SolicitacaoId,
                    prorrogacao.NotaFiscalId,
                    prorrogacao.NFE,
                    prorrogacao.RazaoSocial,
                    prorrogacao.Documento,
                    prorrogacao.NumeroProrrogacao,
                    prorrogacao.IsentarJuros,
                    prorrogacao.Observacoes,
                    prorrogacao.ContaId,
                    prorrogacao.ContaDescricao,
                    ValorNF = prorrogacao.ValorNF.ToString("n2"),
                    ValorJuros = prorrogacao.ValorJuros.ToString("n2"),
                    ValorTotalComJuros = prorrogacao.ValorTotalComJuros.ToString("n2"),
                    VencimentoOriginal = prorrogacao.VencimentoOriginal.DataFormatada(),
                    DataProrrogacao = prorrogacao.DataProrrogacao.DataFormatada(),
                    DataEmissao = prorrogacao.DataEmissao.DataFormatada()
                }, JsonRequestBehavior.AllowGet);
            }

            return null;
        }

        [HttpGet]
        public ActionResult ObterProrrogacaoDefault(int solicitacaoId)
        {
            var registrosProrrogacoes = _solicitacaoRepositorio
                .ObterProrrogacoes(solicitacaoId);

            if (registrosProrrogacoes.Count() == 1)
            {
                var prorrogacao = registrosProrrogacoes.FirstOrDefault();

                var dados = _solicitacaoRepositorio.ObterProrrogacaoPorId(prorrogacao.Id);

                if (dados != null)
                {
                    return Json(new
                    {
                        dados.Id,
                        dados.SolicitacaoId,
                        dados.NotaFiscalId,
                        dados.NFE,
                        dados.RazaoSocial,
                        dados.Documento,
                        dados.NumeroProrrogacao,
                        dados.IsentarJuros,
                        dados.Observacoes,
                        dados.ContaId,
                        dados.ContaDescricao,
                        ValorNF = dados.ValorNF.ToString("n2"),
                        ValorJuros = dados.ValorJuros.ToString("n2"),
                        ValorTotalComJuros = dados.ValorTotalComJuros.ToString("n2"),
                        VencimentoOriginal = dados.VencimentoOriginal.DataFormatada(),
                        DataProrrogacao = dados.DataProrrogacao.DataFormatada(),
                        DataEmissao = dados.DataEmissao.DataFormatada()
                    }, JsonRequestBehavior.AllowGet);
                }
            }

            return null;
        }

        [HttpGet]
        public ActionResult ObterNotasFiscais(int nfe)
        {
            var dadosNFE = _notaFiscalRepositorio
                .ObterNotasFiscais(nfe)
                 .Select(c => new
                 {
                     c.Id,
                     c.NFE,
                     c.StatusNFE,
                     DataEmissao = c.DataEmissao.ToString("dd/MM/yyyy")
                 }).ToList();

            return Json(dadosNFE, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ExcluirProrrogacao(int id)
        {
            var prorrogacao = _solicitacaoRepositorio.ObterProrrogacaoPorId(id);

            if (prorrogacao == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Registro não encontrado ou já excluído");

            if (!User.IsInRole("Administrador"))
            {
                var usuarioCriacao = _usuarioRepositorio.ObterUsuarioPorId(prorrogacao.CriadoPor);

                if (usuarioCriacao.Login.ToLower() != User.Identity.Name.ToLower())
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"A Prorrogação só pode ser excluído pelo usuário de criação ({usuarioCriacao.Login})");
            }

            _solicitacaoRepositorio.ExcluirProrrogacao(id);

            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        [HttpGet]
        public ActionResult ObterProrrogacoes(int solicitacaoId)
        {
            var prorrogacoes = _solicitacaoRepositorio.ObterProrrogacoes(solicitacaoId);

            return PartialView("_ConsultarProrrogacoes", prorrogacoes);
        }

        [HttpGet]
        public ActionResult CalcularJurosProrrogacao(string vencimento, string prorrogacao, string valorNF)
        {
            DateTime dataVencimento = DateTime.MinValue;
            DateTime dataProrrogacao = DateTime.MinValue;
            Double valorNotaFiscal;

            if (!DateTime.TryParse(vencimento, out dataVencimento))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Data de Vencimento inválida");

            if (!DateTime.TryParse(prorrogacao, out dataProrrogacao))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Data de Prorrogação inválida");
            }
            //else
            //{
            //    if (DateTime.Compare(dataProrrogacao, DateTime.Now.Date) < 0)
            //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "A Data de Prorrogação não pode ser menor que a data atual");
            //}

            if (!Double.TryParse(valorNF, out valorNotaFiscal))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Valor de Nota Fiscal inválido");

            var parametros = _parametrosRepositorio.ObterParametros();

            var dias = (dataProrrogacao - dataVencimento).TotalDays;

            //var juros = (parametros.Mora * dias * valorNotaFiscal + (valorNotaFiscal + parametros.Multa));
            var juros = ((parametros.Mora * dias) * valorNotaFiscal) / 100;
            var multa = (valorNotaFiscal * parametros.Multa);

            juros = juros + multa;

            var valorComJuros = valorNotaFiscal + juros;

            return Json(new
            {
                juros = juros.ToString("n2"),
                valorComJuros = valorComJuros.ToString("n2")
            }, JsonRequestBehavior.AllowGet);
        }

        private void PopularRestituicao(SolicitacaoComercialRestituicaoViewModel solicitacaoComercialRestituicaoViewModel)
        {
            var restituicoes = _solicitacaoRepositorio.ObterRestituicoes(solicitacaoComercialRestituicaoViewModel.RestituicaoSolicitacaoId);
            solicitacaoComercialRestituicaoViewModel.Restituicoes = restituicoes.ToList();
        }

        public void PopularBancos(SolicitacaoComercialRestituicaoViewModel solicitacaoComercialRestituicaoViewModel)
        {
            var bancos = _bancoRepositorio.ObterBancos();

            solicitacaoComercialRestituicaoViewModel.Bancos = bancos.ToList();
        }

        [HttpGet]
        public ActionResult ObterServicosFaturamento(int lote, int? seqGr)
        {
            var servicos = new List<ServicoFaturamento>();

            servicos = _servicoFaturamentoRepositorio.ObterServicosPorBL(lote, seqGr).ToList();

            if (servicos.Count == 0)
            {
                servicos = _servicoFaturamentoRepositorio.ObterServicosPreCalculoPorBL(lote).ToList();
            }

            var dadosServicos = servicos
                 .Select(c => new
                 {
                     c.Id,
                     c.Descricao,
                     Valor = c.Valor.ToString("c2")
                 }).ToList();

            return Json(dadosServicos, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ObterServicosFaturamentoMinuta(int minuta)
        {
            var servicos = _minutaRepositorio
                   .ObterServicosPorMinuta(minuta)
                   .Select(c => new
                   {
                       c.Id,
                       c.Descricao,
                       Valor = c.Valor.ToString("c2")
                   }).ToList();

            return Json(servicos, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ObterServicosFaturamentoRedex(string reserva, int seqGr, string display)
        {
            var servicos = new List<ServicoFaturamento>();

            var bookingBusca = _bookingRepositorio.ObterBookingPorReserva(reserva);

            if (bookingBusca != null)
            {
                var clienteFatura = Regex.Match(display, @"\(([^)]*)\)").Groups[1].Value;

                servicos = _servicoFaturamentoRepositorio.ObterServicosRedex(bookingBusca.Id, clienteFatura.ToInt(), seqGr).ToList();

                var dadosServicos = servicos
                    .Select(c => new
                    {
                        c.Id,
                        c.Descricao,
                        Valor = c.Valor.ToString("c2")
                    }).ToList();

                return Json(dadosServicos, JsonRequestBehavior.AllowGet);
            }

            return null;
        }

        private void PopularDescontos(SolicitacaoComercialDescontoViewModel solicitacaoComercialDescontoViewModel)
        {
            var descontos = solicitacaoComercialDescontoViewModel.DescontoTipoOperacaoSolicitacao == 3
                ? _solicitacaoRepositorio.ObterDescontosRedex(solicitacaoComercialDescontoViewModel.DescontoSolicitacaoId)
                : _solicitacaoRepositorio.ObterDescontos(solicitacaoComercialDescontoViewModel.DescontoSolicitacaoId);

            solicitacaoComercialDescontoViewModel.Descontos = descontos.ToList();
        }

        [HttpPost]
        public ActionResult CadastrarRestituicao([Bind(Include = "RestituicaoId, RestituicaoSolicitacaoId, RestituicaoTipoPesquisa, RestituicaoTipoPesquisaNumero, RestituicaoNotaFiscalId, RestituicaoValorNF, RestituicaoRPS, RestituicaoLote, RestituicaoDocumento, RestituicaoFavorecidoId, RestituicaoRazaoSocial, RestituicaoBancoId, RestituicaoAgencia, RestituicaoContaCorrente, RestituicaoFornecedorSAP, RestituicaoDataVencimento, RestituicaoUnidadeSolicitacao, RestituicaoValorAPagar, RestituicaoObservacoes")] SolicitacaoComercialRestituicaoViewModel viewModel)
        {
            if (viewModel.RestituicaoSolicitacaoId == 0)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var solicitacao = _solicitacaoRepositorio.ObterSolicitacaoPorId(viewModel.RestituicaoSolicitacaoId);

            if (solicitacao == null)
                RegistroNaoEncontrado();

            var restituicao = new SolicitacaoRestituicao(
                solicitacao.Id,
                viewModel.RestituicaoTipoPesquisa,
                viewModel.RestituicaoTipoPesquisaNumero,
                viewModel.RestituicaoNotaFiscalId,
                viewModel.RestituicaoValorNF,
                viewModel.RestituicaoRPS,
                viewModel.RestituicaoLote,
                viewModel.RestituicaoDocumento,
                viewModel.RestituicaoFavorecidoId,
                viewModel.RestituicaoRazaoSocial,
                viewModel.RestituicaoBancoId,
                viewModel.RestituicaoAgencia,
                viewModel.RestituicaoContaCorrente,
                viewModel.RestituicaoFornecedorSAP,
                viewModel.RestituicaoValorAPagar,
                viewModel.RestituicaoDataVencimento,
                viewModel.RestituicaoObservacoes,
                User.ObterId(),
                solicitacao);

            if (viewModel.RestituicaoDataVencimento != null)
            {
                if (viewModel.RestituicaoDataVencimento.Value.Date <= DateTime.Now.Date)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "A Data de vencimento não pode ser inferior a data atual.");
            }

            var restituicoes = _solicitacaoRepositorio
                    .ObterRestituicoes(solicitacao.Id);

            if (viewModel.RestituicaoId == 0)
            {
                if (restituicoes.Where(c => c.FavorecidoId != viewModel.RestituicaoFavorecidoId).Any())
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Só é permitido cadastrar Restituições para o mesmo Favorecido");

                if (viewModel.RestituicaoTipoPesquisa == TipoPesquisa.NF)
                {
                    if (restituicoes.Where(c => c.NFE == viewModel.RestituicaoTipoPesquisaNumero).Any())
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Já existe um registro com a mesma NFE");

                    restituicao.NFE = viewModel.RestituicaoTipoPesquisaNumero;
                }
            }
            else
            {
                if (restituicoes.Where(c => c.FavorecidoId != viewModel.RestituicaoFavorecidoId && c.Id != viewModel.RestituicaoId).Any())
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Só é permitido cadastrar Restituições para o mesmo Favorecido");

                if (viewModel.RestituicaoTipoPesquisa == TipoPesquisa.NF)
                {
                    if (restituicoes.Where(c => (c.NFE == viewModel.RestituicaoTipoPesquisaNumero) && c.Id != viewModel.RestituicaoId).Any())
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Já existe um registro com a mesma NFE");

                    restituicao.NFE = viewModel.RestituicaoTipoPesquisaNumero;
                }
            }

            if (viewModel.RestituicaoId == 0)
            {
                if (!Validar(restituicao))
                    return RetornarErros();

                _solicitacaoRepositorio.CadastrarRestituicao(restituicao);
            }
            else
            {
                var restituicaoBusca = _solicitacaoRepositorio.ObterRestituicaoPorId(viewModel.RestituicaoId);

                if (restituicaoBusca == null)
                    RegistroNaoEncontrado();

                if (!User.IsInRole("Administrador"))
                {
                    if (restituicaoBusca.CriadoPor != User.ObterId())
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "A edição da solicitação é permitida apenas pelo usuário de criação.");
                }

                restituicaoBusca.Alterar(restituicao);
                restituicaoBusca.SolicitacaoComercial = solicitacao;

                if (!Validar(restituicaoBusca))
                    return RetornarErros();

                _solicitacaoRepositorio.AtualizarRestituicao(restituicaoBusca);
            }

            restituicoes = _solicitacaoRepositorio.ObterRestituicoes(solicitacao.Id);

            return PartialView("_ConsultarRestituicoes", restituicoes);
        }

        [HttpPost]
        public ActionResult AtualizarResumoRestituicao(int solicitacaoId, decimal valorCobrado, bool habilitaValorDevido, decimal? ValorDevido = 0)
        {
            var solicitacaoBusca = _solicitacaoRepositorio.ObterSolicitacaoPorId(solicitacaoId);

            if (solicitacaoBusca == null)
                RegistroNaoEncontrado();

            var registros = _solicitacaoRepositorio.ObterRestituicoes(solicitacaoId);

            var cidadeSantos = false;

            foreach (var registro in registros)
            {
                var notaBusca = _notaFiscalRepositorio.ObterDetalhesNotaFiscal(registro.NotaFiscalId);

                if (notaBusca != null)
                {
                    if (notaBusca.Cidade == "SANTOS")
                    {
                        cidadeSantos = true;
                        break;
                    }
                }
            }

            if (cidadeSantos)
            {
                solicitacaoBusca.ValorDevido = habilitaValorDevido
                    ? ValorDevido.Value
                    : registros.Sum(c => c.ValorAPagar);
            }
            else
            {
                solicitacaoBusca.ValorDevido = habilitaValorDevido
                    ? ValorDevido.Value
                    : registros.Sum(c => c.ValorNF);
            }

            solicitacaoBusca.ValorCobrado = registros.Any() ? valorCobrado : 0;
            solicitacaoBusca.HabilitaValorDevido = habilitaValorDevido;
            solicitacaoBusca.ValorCredito = solicitacaoBusca.ValorCobrado - solicitacaoBusca.ValorDevido;

            solicitacaoBusca.AlterarResumo(solicitacaoBusca);

            _solicitacaoRepositorio.AtualizarResumoRestituicao(solicitacaoBusca);

            return Json(new
            {
                ValorDevido = solicitacaoBusca.ValorDevido.ToString("n2"),
                ValorCobrado = solicitacaoBusca.ValorCobrado.ToString("n2"),
                ValorCredito = solicitacaoBusca.ValorCredito.ToString("n2")
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ObterValorDevidoRestituicao(int solicitacaoId)
        {
            var restituicoes = _solicitacaoRepositorio.ObterRestituicoes(solicitacaoId);

            if (restituicoes.Any())
            {
                return Json(new
                {
                    ValorDevido = restituicoes.Sum(c => c.ValorNF).ToString("n2")
                }, JsonRequestBehavior.AllowGet);
            }

            return null;
        }

        [HttpGet]
        public ActionResult ObterDetalhesRestituicao(int id)
        {
            var restituicao = _solicitacaoRepositorio.ObterDetalhesRestituicao(id);

            if (restituicao != null)
            {
                return Json(new
                {
                    restituicao.Id,
                    restituicao.SolicitacaoId,
                    restituicao.TipoPesquisa,
                    restituicao.TipoPesquisaNumero,
                    restituicao.NotaFiscalId,
                    restituicao.NFE,
                    restituicao.RPS,
                    restituicao.Lote,
                    restituicao.Documento,
                    restituicao.FavorecidoId,
                    restituicao.FavorecidoDescricao,
                    restituicao.BancoId,
                    restituicao.BancoDescricao,
                    restituicao.Agencia,
                    restituicao.ContaCorrente,
                    restituicao.FornecedorSAP,
                    restituicao.Observacoes,
                    ValorNF = restituicao.ValorNF.ToString("n2"),
                    ValorAPagar = restituicao.ValorAPagar.ToString("n2"),
                    DataVencimento = restituicao.DataVencimento.DataFormatada(),
                    DataEmissao = restituicao.DataEmissao.DataFormatada()
                }, JsonRequestBehavior.AllowGet);
            }

            return null;
        }

        [HttpGet]
        public ActionResult ObterRestituicaoDefault(int solicitacaoId)
        {
            var registrosRestituicoes = _solicitacaoRepositorio
                .ObterRestituicoes(solicitacaoId);

            if (registrosRestituicoes.Count() == 1)
            {
                var restituicao = registrosRestituicoes.FirstOrDefault();

                var dados = _solicitacaoRepositorio.ObterDetalhesRestituicao(restituicao.Id);

                if (dados != null)
                {
                    return Json(new
                    {
                        dados.Id,
                        dados.SolicitacaoId,
                        dados.TipoPesquisa,
                        dados.TipoPesquisaNumero,
                        dados.NotaFiscalId,
                        dados.NFE,
                        dados.RPS,
                        dados.Lote,
                        dados.Documento,
                        dados.FavorecidoId,
                        dados.FavorecidoDescricao,
                        dados.BancoId,
                        dados.BancoDescricao,
                        dados.Agencia,
                        dados.ContaCorrente,
                        dados.FornecedorSAP,
                        dados.Observacoes,
                        ValorNF = dados.ValorNF.ToString("n2"),
                        ValorAPagar = dados.ValorAPagar.ToString("n2"),
                        DataVencimento = dados.DataVencimento.DataFormatada(),
                        DataEmissao = dados.DataEmissao.DataFormatada()
                    }, JsonRequestBehavior.AllowGet);
                }
            }

            return null;
        }

        [HttpPost]
        public ActionResult ExcluirRestituicao(int id)
        {
            var restituicao = _solicitacaoRepositorio.ObterRestituicaoPorId(id);

            if (restituicao == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Registro não encontrado ou já excluído");

            if (!User.IsInRole("Administrador"))
            {
                var usuarioCriacao = _usuarioRepositorio.ObterUsuarioPorId(restituicao.CriadoPor);

                if (usuarioCriacao.Login.ToLower() != User.Identity.Name.ToLower())
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"A Restituição só pode ser excluído pelo usuário de criação ({usuarioCriacao.Login})");
            }

            _solicitacaoRepositorio.ExcluirRestituicao(id);

            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        [HttpGet]
        public ActionResult ObterRestituicoes(int solicitacaoId)
        {
            var restituicoes = _solicitacaoRepositorio.ObterRestituicoes(solicitacaoId);

            return PartialView("_ConsultarRestituicoes", restituicoes);
        }

        [HttpGet]
        [UsuarioExternoFilter]
        public PartialViewResult ConsultarContasPorDescricao(string descricao)
        {
            var resultado = _contaRepositorio.ObterContasPorDescricao(descricao, (int?)ViewBag.UsuarioExternoId);

            return PartialView("_PesquisarContasConsulta", resultado);
        }

        [HttpGet]
        [UsuarioExternoFilter]
        public ActionResult ObterGRS(TipoPesquisa tipoPesquisa, string termoPesquisa)
        {
            var listaGrMinuta = new List<GR>();

            if (tipoPesquisa == TipoPesquisa.LOTE || tipoPesquisa == TipoPesquisa.BL)
            {
                List<GR> grs = new List<GR>();

                if (tipoPesquisa == TipoPesquisa.LOTE)
                {
                    grs = _grRepositorio.ObterGRsPorLote(termoPesquisa.ToInt()).ToList();
                }
                else
                {
                    grs = _grRepositorio.ObterGRsPorBL(termoPesquisa).ToList();
                }

                if (grs.Count == 0)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"Nenhuma GR encontrada para o lote {termoPesquisa}");

                if (grs.Count == 1)
                {
                    var gr = grs.FirstOrDefault();

                    if (gr.StatusGR != "GE")
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"Lote {termoPesquisa} não possui GR com status GERADA");
                }

                if (User.IsInRole("UsuarioExterno"))
                {
                    var clientesVinculados = _usuarioRepositorio.ObterVinculosContas((int)ViewBag.UsuarioExternoId);

                    var existe = false;

                    foreach (var gr in grs)
                    {
                        foreach (var cnpj in clientesVinculados)
                        {
                            if (!string.IsNullOrEmpty(gr.ImportadorCnpj))
                            {
                                if (gr.ImportadorCnpj == cnpj.ContaDocumento)
                                {
                                    existe = true;
                                    break;
                                }
                            }
                        }

                        foreach (var cnpj in clientesVinculados)
                        {
                            if (!string.IsNullOrEmpty(gr.IndicadorCnpj))
                            {
                                if (gr.IndicadorCnpj == cnpj.ContaDocumento)
                                {
                                    existe = true;
                                }
                            }
                        }
                    }

                    if (grs.Any())
                    {
                        if (!existe)
                        {
                            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Usuário não possui permissão para cadastrar este processo");
                        }
                    }
                }

                listaGrMinuta.AddRange(grs);
            }

            if (tipoPesquisa == TipoPesquisa.GR)
            {
                var gr = _grRepositorio.ObterDetalhesGR(termoPesquisa.ToInt());

                if (gr == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"GR {termoPesquisa} não encontrada ou não possui o status gerada");

                if (gr.StatusGR != "GE")
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"GR {termoPesquisa} não possui status GERADA");

                if (User.IsInRole("UsuarioExterno"))
                {
                    var clientesVinculados = _usuarioRepositorio.ObterVinculosContas((int)ViewBag.UsuarioExternoId);

                    var existe = false;

                    foreach (var cnpj in clientesVinculados)
                    {
                        if (!string.IsNullOrEmpty(gr.ImportadorCnpj))
                        {
                            if (gr.ImportadorCnpj == cnpj.ContaDocumento)
                            {
                                existe = true;
                                break;
                            }
                        }

                        if (!string.IsNullOrEmpty(gr.IndicadorCnpj))
                        {
                            if (gr.IndicadorCnpj == cnpj.ContaDocumento)
                            {
                                existe = true;
                            }
                        }
                    }

                    if (!existe)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Usuário não possui permissão para cadastrar este processo");
                    }
                }

                listaGrMinuta.Add(gr);
            }

            var lista = listaGrMinuta.Select(c => new
            {
                c.Id,
                c.SeqGR,
                Display = $"{c.SeqGR} {c.ClienteDescricao}"
            });

            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ObterGRsFaturadas(TipoPesquisa tipoPesquisa, string termoPesquisa)
        {
            var listaGr = new List<GR>();

            if (tipoPesquisa == TipoPesquisa.LOTE)
            {
                var grs = _grRepositorio.ObterGRsFaturadasPorLote(termoPesquisa.ToInt()).ToList();

                if (grs == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"Nenhuma GR encontrada para o lote {termoPesquisa}");

                if (grs.Count == 1)
                {
                    var gr = grs.FirstOrDefault();

                    if (gr.StatusGR != "GE")
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"Lote {termoPesquisa} não possui GR com status GERADA");
                }

                listaGr.AddRange(grs);
            }

            if (tipoPesquisa == TipoPesquisa.GR)
            {
                var gr = _grRepositorio.ObterDetalhesGR(termoPesquisa.ToInt());

                if (gr.FormaPagamento == FormaPagamento.FATURADO)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"Não disponível para Forma de Pagamento {gr.FormaPagamento.ToName()}");

                if (gr == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"GR {termoPesquisa} não encontrada ou não possui o status gerada");

                if (gr.StatusGR != "GE")
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"GR {termoPesquisa} não possui status GERADA");

                listaGr.Add(gr);
            }

            var lista = listaGr.Select(c => new
            {
                c.Id,
                c.SeqGR,
                Display = $"{c.SeqGR} {c.ClienteDescricao}"
            });

            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ObterGRSRedex(TipoPesquisa tipoPesquisa, string termoPesquisa)
        {
            var listaGrMinuta = new List<GR>();

            if (tipoPesquisa == TipoPesquisa.BOOKING)
            {
                var grs = _grRepositorio.ObterGRsRedexPorReserva(termoPesquisa).ToList();

                if (grs == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"Nenhuma GR encontrada para o Booking {termoPesquisa}");

                if (grs.Count == 1)
                {
                    var gr = grs.FirstOrDefault();

                    if (gr.StatusGR != "GE")
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"Booking {termoPesquisa} não possui GR com status GERADA");
                }

                if (User.IsInRole("UsuarioExterno"))
                {
                    var clientesVinculados = _usuarioRepositorio.ObterVinculosContas((int)ViewBag.UsuarioExternoId);

                    var existe = false;

                    foreach (var gr in grs)
                    {
                        foreach (var cnpj in clientesVinculados)
                        {
                            if (gr.ExportadorCnpj == cnpj.ContaDocumento)
                            {
                                existe = true;
                                break;
                            }
                        }
                    }

                    if (!existe)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Usuário não possui permissão para cadastrar este processo");
                    }
                }

                listaGrMinuta.AddRange(grs);
            }

            if (tipoPesquisa == TipoPesquisa.GR)
            {
                var bookingBusca = _bookingRepositorio.ObterBookingPorReserva(termoPesquisa);

                if (bookingBusca == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Booking não encontrado");

                var gr = _grRepositorio.ObterDetalhesGRRedex(bookingBusca.Id, termoPesquisa.ToInt(), 0);

                if (gr == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"GR {termoPesquisa} não encontrada ou não possui o status gerada");

                if (gr.StatusGR != "GE")
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"GR {termoPesquisa} não possui status GERADA");

                if (User.IsInRole("UsuarioExterno"))
                {
                    var clientesVinculados = _usuarioRepositorio.ObterVinculosContas((int)ViewBag.UsuarioExternoId);

                    var existe = false;

                    foreach (var cnpj in clientesVinculados)
                    {
                        if (gr.ExportadorCnpj == cnpj.ContaDocumento)
                        {
                            existe = true;
                            break;
                        }
                    }

                    if (!existe)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Usuário não possui permissão para cadastrar este processo");
                    }
                }

                listaGrMinuta.Add(gr);
            }

            var lista = listaGrMinuta.Select(c => new
            {
                c.Id,
                c.SeqGR,
                Display = $"{c.SeqGR} - {c.ClienteDescricao} ({c.ClienteId})"
            });

            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ObterMinuta(string termoPesquisa)
        {
            var minuta = _minutaRepositorio
                .ObterMinuta(termoPesquisa.ToInt());

            if (minuta == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"Minuta {termoPesquisa} não encontrada ou não possui status PENDENTE");
            }

            if (minuta.Status != "PENDENTE")
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"Não é permitido cadastro de desconto para minutas com status {minuta.Status}");
            }

            var servicos = _minutaRepositorio
                 .ObterServicosPorMinuta(termoPesquisa.ToInt())
                 .Select(c => new
                 {
                     c.Id,
                     c.Descricao,
                     c.Valor
                 }).ToList();

            return Json(new
            {
                minuta.Id,
                minuta.Status,
                minuta.ClienteId,
                minuta.ClienteDescricao,
                Valor = minuta.Valor.ToString("n2"),
                servicosFaturados = servicos
            }, JsonRequestBehavior.AllowGet);
        }

        public List<SolicitacaoComercialMotivo> PopularMotivosPorTipoSolicitacao(TipoSolicitacao tipoSolicitacao)
        {
            var motivos = _motivosRepositorio.ObterSolicitacoesMotivo();

            switch (tipoSolicitacao)
            {
                case TipoSolicitacao.CANCELAMENTO_NF:
                    motivos = motivos.Where(c => c.CancelamentoNF).ToList();
                    break;
                case TipoSolicitacao.DESCONTO:
                    motivos = motivos.Where(c => c.Desconto).ToList();
                    break;
                case TipoSolicitacao.PRORROGACAO_BOLETO:
                    motivos = motivos.Where(c => c.ProrrogacaoBoleto).ToList();
                    break;
                case TipoSolicitacao.RESTITUICAO:
                    motivos = motivos.Where(c => c.Restituicao).ToList();
                    break;
                case TipoSolicitacao.OUTROS:
                    motivos = motivos.Where(c => c.Outros).ToList();
                    break;
                default:
                    break;
            }

            motivos = motivos.Where(c => c.Status == Status.ATIVO);

            return motivos.ToList();
        }

        public ActionResult PopularMotivosPorTipoSolicitacaoJson(TipoSolicitacao tipoSolicitacao)
        {
            var motivos = PopularMotivosPorTipoSolicitacao(tipoSolicitacao)
                .Select(c => new
                {
                    c.Id,
                    c.Descricao
                });

            return Json(motivos, JsonRequestBehavior.AllowGet);
        }

        private List<SolicitacaoComercialOcorrencia> PopularOcorrenciasPorTipoSolicitacao(TipoSolicitacao tipoSolicitacao)
        {
            var ocorrencias = _ocorrenciasRepositorio.ObterSolicitacoesOcorrencia();

            switch (tipoSolicitacao)
            {
                case TipoSolicitacao.CANCELAMENTO_NF:
                    ocorrencias = ocorrencias.Where(c => c.CancelamentoNF).ToList();
                    break;
                case TipoSolicitacao.DESCONTO:
                    ocorrencias = ocorrencias.Where(c => c.Desconto).ToList();
                    break;
                case TipoSolicitacao.PRORROGACAO_BOLETO:
                    ocorrencias = ocorrencias.Where(c => c.ProrrogacaoBoleto).ToList();
                    break;
                case TipoSolicitacao.RESTITUICAO:
                    ocorrencias = ocorrencias.Where(c => c.Restituicao).ToList();
                    break;
                case TipoSolicitacao.OUTROS:
                    ocorrencias = ocorrencias.Where(c => c.Outros).ToList();
                    break;
                default:
                    break;
            }

            ocorrencias = ocorrencias.Where(c => c.Status == Status.ATIVO);

            return ocorrencias.ToList();
        }

        public ActionResult PopularOcorrenciasPorTipoSolicitacaoJson(TipoSolicitacao tipoSolicitacao)
        {
            var ocorrencias = PopularOcorrenciasPorTipoSolicitacao(tipoSolicitacao)
                .Select(c => new
                {
                    c.Id,
                    c.Descricao
                });

            return Json(ocorrencias, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RetornarErros()
        {
            Response.StatusCode = (int)HttpStatusCode.BadRequest;

            return Json(new
            {
                erros = ModelState.Values.SelectMany(v => v.Errors)
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult CadastrarDesconto([Bind(Include = "DescontoId, DescontoSolicitacaoId, TipoPesquisaSolicitacaoDesconto, DescontoTipoPesquisaNumero, DescontoGRMinutaId, DescontoValor, DescontoClienteId, DescontoRazaoSocial, DescontoIndicadorId, ClienteFaturamentoId, DescontoProposta, DescontoVencimentoGR, DescontoFreeTimeGR, DescontoPeriodo, DescontoLote, DescontoReserva, DescontoFormaPagamento, DescontoTipoDesconto, DescontoValorDesconto, DescontoValorDescontoNoServico, DescontoValorDescontoFinal, DescontoTipoDescontoPorServico, DescontoServicoId, DescontoVencimento, DescontoFreeTime, DescontoUnidadeSolicitacao, DescontoValorDescontoComImposto")] SolicitacaoComercialDescontoViewModel viewModel)
        {
            if (viewModel.DescontoSolicitacaoId == 0)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var solicitacao = _solicitacaoRepositorio.ObterSolicitacaoPorId(viewModel.DescontoSolicitacaoId);

            if (solicitacao == null)
                RegistroNaoEncontrado();

            if (viewModel.TipoPesquisaSolicitacaoDesconto == TipoPesquisa.BOOKING)
            {
                if (viewModel.DescontoServicoId > 0)
                {
                    var servicoREDEX = _servicoFaturamentoRepositorio.ObterServicoFaturamentoRedexPorId(viewModel.DescontoServicoId);

                    if (servicoREDEX != null)
                    {
                        viewModel.ServicoId = servicoREDEX.Servico;
                        viewModel.ServicoValor = servicoREDEX.Valor;
                    }
                }
            }
            else
            {
                if (viewModel.DescontoServicoId > 0)
                {
                    var servicoIPA = _servicoFaturamentoRepositorio.ObterServicoFaturamentoPorId(viewModel.DescontoServicoId);

                    if (servicoIPA != null)
                    {
                        viewModel.ServicoId = servicoIPA.Servico;
                        viewModel.ServicoValor = servicoIPA.Valor;
                    }
                }
            }

            var desconto = new SolicitacaoDesconto(
                solicitacao.Id,
                viewModel.TipoPesquisaSolicitacaoDesconto,
                viewModel.DescontoTipoPesquisaNumero,
                viewModel.DescontoValor,
                viewModel.DescontoClienteId,
                viewModel.DescontoRazaoSocial,
                viewModel.DescontoIndicadorId,
                viewModel.DescontoProposta,
                viewModel.DescontoVencimentoGR,
                viewModel.DescontoFreeTimeGR,
                viewModel.DescontoPeriodo,
                viewModel.DescontoLote,
                viewModel.DescontoReserva,
                viewModel.DescontoFormaPagamento,
                viewModel.DescontoTipoDesconto,
                viewModel.DescontoValorDesconto,
                viewModel.DescontoValorDescontoNoServico,
                viewModel.DescontoValorDescontoFinal,
                viewModel.DescontoTipoDescontoPorServico,
                viewModel.DescontoServicoId,
                viewModel.ServicoId,
                viewModel.ServicoValor,
                viewModel.DescontoValorDescontoComImposto,
                viewModel.DescontoVencimento,
                viewModel.DescontoFreeTime,
                User.ObterId());

            if (viewModel.DescontoVencimento != null)
            {
                if (viewModel.DescontoVencimento.Value.Date < DateTime.Now.Date)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "A Data de vencimento não pode ser inferior ou igual a data atual.");
            }

            if (viewModel.DescontoFreeTime != null)
            {
                if (viewModel.DescontoFreeTime.Value.Date < DateTime.Now.Date)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "O Free Time não pode ser inferior ou igual a data atual.");
            }

            if (viewModel.DescontoId == 0)
            {
                bool existeDesconto = false;

                if (viewModel.TipoPesquisaSolicitacaoDesconto == TipoPesquisa.MINUTA)
                {
                    existeDesconto = _solicitacaoRepositorio
                       .ObterDescontos(viewModel.DescontoSolicitacaoId)
                       .Where(c => c.Minuta != viewModel.DescontoGRMinutaId)
                       .Any();

                    desconto.Minuta = viewModel.DescontoGRMinutaId;
                }
                else if (viewModel.TipoPesquisaSolicitacaoDesconto == TipoPesquisa.GR)
                {
                    existeDesconto = _solicitacaoRepositorio
                        .ObterDescontos(viewModel.DescontoSolicitacaoId)
                        .Where(c => c.GR != viewModel.DescontoTipoPesquisaNumero.ToInt())
                        .Any();

                    desconto.SeqGR = viewModel.DescontoGRMinutaId;
                }
                else if (viewModel.TipoPesquisaSolicitacaoDesconto == TipoPesquisa.LOTE)
                {
                    existeDesconto = _solicitacaoRepositorio
                       .ObterDescontos(viewModel.DescontoSolicitacaoId)
                       .Where(c => c.Lote != viewModel.DescontoTipoPesquisaNumero.ToInt())
                       .Any();

                    desconto.Lote = viewModel.DescontoLote;
                    desconto.SeqGR = viewModel.DescontoGRMinutaId;
                }
                else if (viewModel.TipoPesquisaSolicitacaoDesconto == TipoPesquisa.BOOKING)
                {
                    existeDesconto = _solicitacaoRepositorio
                       .ObterDescontos(viewModel.DescontoSolicitacaoId)
                       .Where(c => c.Reserva != viewModel.DescontoReserva)
                       .Any();

                    desconto.SeqGR = viewModel.DescontoGRMinutaId;
                    desconto.ClienteFaturamentoId = viewModel.ClienteFaturamentoId.ToInt();
                }

                if (existeDesconto)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Não é permitido cadastro Lotes / Minutas / Reservas diferentes");

                if (!Validar(desconto))
                    return RetornarErros();

                _solicitacaoRepositorio.CadastrarDesconto(desconto);
            }
            else
            {
                var descontoBusca = _solicitacaoRepositorio.ObterDescontoPorId(viewModel.DescontoId);

                if (descontoBusca == null)
                    RegistroNaoEncontrado();

                bool existeDesconto = false;

                if (viewModel.TipoPesquisaSolicitacaoDesconto == TipoPesquisa.MINUTA)
                {
                    existeDesconto = _solicitacaoRepositorio
                       .ObterDescontos(viewModel.DescontoSolicitacaoId)
                       .Where(c => c.MinutaGRId != viewModel.DescontoGRMinutaId && c.Id != viewModel.DescontoId)
                       .Any();

                    desconto.Minuta = viewModel.DescontoGRMinutaId;
                }
                else if (viewModel.TipoPesquisaSolicitacaoDesconto == TipoPesquisa.GR)
                {
                    existeDesconto = _solicitacaoRepositorio
                        .ObterDescontos(viewModel.DescontoSolicitacaoId)
                        .Where(c => c.GR != viewModel.DescontoTipoPesquisaNumero.ToInt() && c.Id != viewModel.DescontoId)
                        .Any();

                    desconto.SeqGR = viewModel.DescontoGRMinutaId;
                }
                else if (viewModel.TipoPesquisaSolicitacaoDesconto == TipoPesquisa.LOTE)
                {
                    existeDesconto = _solicitacaoRepositorio
                       .ObterDescontos(viewModel.DescontoSolicitacaoId)
                       .Where(c => c.Lote != viewModel.DescontoTipoPesquisaNumero.ToInt() && c.Id != viewModel.DescontoId)
                       .Any();

                    desconto.Lote = viewModel.DescontoLote;
                    desconto.SeqGR = viewModel.DescontoGRMinutaId;
                }
                else if (viewModel.TipoPesquisaSolicitacaoDesconto == TipoPesquisa.BOOKING)
                {
                    existeDesconto = _solicitacaoRepositorio
                      .ObterDescontos(viewModel.DescontoSolicitacaoId)
                      .Where(c => c.Reserva != viewModel.DescontoReserva && c.Id != viewModel.DescontoId)
                      .Any();

                    desconto.SeqGR = viewModel.DescontoGRMinutaId;
                    desconto.ClienteFaturamentoId = viewModel.ClienteFaturamentoId.ToInt();
                }

                if (existeDesconto)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Não é permitido cadastro Lotes / Minutas / Reservas diferentes");

                if (!User.IsInRole("Administrador"))
                {
                    if (descontoBusca.CriadoPor != User.ObterId())
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "A edição da solicitação é permitida apenas pelo usuário de criação.");
                }

                descontoBusca.Alterar(desconto);

                if (!Validar(descontoBusca))
                    return RetornarErros();

                _solicitacaoRepositorio.AtualizarDesconto(descontoBusca);
            }

            var descontos = solicitacao.TipoOperacao == 3
                ? _solicitacaoRepositorio.ObterDescontosRedex(solicitacao.Id)
                : _solicitacaoRepositorio.ObterDescontos(solicitacao.Id);

            return PartialView("_ConsultarDescontos", descontos);
        }

        [HttpGet]
        public ActionResult ObterDetalhesDesconto(int id, int tipoOperacao)
        {
            SolicitacaoDescontoDTO desconto = new SolicitacaoDescontoDTO();

            if (tipoOperacao == 3)
                desconto = _solicitacaoRepositorio.ObterDetalhesDescontoRedex(id);
            else
                desconto = _solicitacaoRepositorio.ObterDetalhesDesconto(id);

            if (desconto != null)
            {
                return Json(new
                {
                    desconto.Id,
                    desconto.TipoPesquisa,
                    desconto.TipoPesquisaNumero,
                    desconto.ClienteId,
                    desconto.RazaoSocial,
                    desconto.ClienteDescricao,
                    desconto.IndicadorId,
                    desconto.ClienteFaturamentoId,
                    desconto.ClienteFaturamentoDescricao,
                    desconto.IndicadorDescricao,
                    desconto.SolicitacaoId,
                    desconto.MinutaGRId,
                    desconto.Proposta,
                    desconto.Periodo,
                    desconto.TipoDesconto,
                    desconto.ServicoFaturadoId,
                    desconto.ServicoDescricao,
                    desconto.Geral,
                    desconto.Lote,
                    desconto.Reserva,
                    desconto.Minuta,
                    desconto.PorServico,
                    desconto.FormaPagamento,
                    desconto.ServicoId,
                    DescricaoFormaPagamento = desconto.FormaPagamento.ToName(),
                    ServicoValor = desconto.ServicoValor.ToString("n2"),
                    DescontoFinal = desconto.DescontoFinal.ToString("n2"),
                    ValorGR = desconto.ValorGR.ToString("n2"),
                    Desconto = desconto.Desconto.ToString("n2"),
                    DescontoNoServico = desconto.DescontoNoServico.ToString("n2"),
                    DescontoComImposto = desconto.DescontoComImposto.ToString("n2"),
                    Vencimento = desconto.Vencimento.DataFormatada(),
                    VencimentoGR = desconto.VencimentoGR.DataFormatada(),
                    FreeTimeGR = desconto.FreeTimeGR.DataFormatada(),
                    FreeTime = desconto.FreeTime.DataFormatada()
                }, JsonRequestBehavior.AllowGet);
            }

            return null;
        }

        [HttpGet]
        public ActionResult ObterDescontoDefault(int solicitacaoId)
        {
            var registrosDescontos = _solicitacaoRepositorio
                .ObterDescontos(solicitacaoId);

            var solicitacaoBusca = _solicitacaoRepositorio.ObterSolicitacaoPorId(solicitacaoId);

            if (registrosDescontos.Count() > 0)
            {
                var desconto = registrosDescontos.FirstOrDefault();

                if (solicitacaoBusca.TipoOperacao == 3)
                    desconto = _solicitacaoRepositorio.ObterDetalhesDescontoRedex(desconto.Id);
                else
                    desconto = _solicitacaoRepositorio.ObterDetalhesDesconto(desconto.Id);

                if (desconto != null)
                {
                    return Json(new
                    {
                        desconto.Id,
                        desconto.TipoPesquisa,
                        desconto.TipoPesquisaNumero,
                        desconto.ClienteId,
                        desconto.ClienteDescricao,
                        desconto.IndicadorId,
                        desconto.IndicadorDescricao,
                        desconto.ClienteFaturamentoId,
                        desconto.ClienteFaturamentoDescricao,
                        desconto.SolicitacaoId,
                        desconto.MinutaGRId,
                        desconto.Proposta,
                        desconto.Periodo,
                        desconto.TipoDesconto,
                        desconto.ServicoFaturadoId,
                        desconto.ServicoDescricao,
                        desconto.Geral,
                        desconto.Lote,
                        desconto.Minuta,
                        desconto.PorServico,
                        desconto.FormaPagamento,
                        desconto.ServicoId,
                        DescricaoFormaPagamento = desconto.FormaPagamento.ToName(),
                        ServicoValor = desconto.ServicoValor.ToString("n2"),
                        DescontoFinal = desconto.DescontoFinal.ToString("n2"),
                        ValorGR = desconto.ValorGR.ToString("n2"),
                        Desconto = desconto.Desconto.ToString("n2"),
                        DescontoNoServico = desconto.DescontoNoServico.ToString("n2"),
                        DescontoComImposto = desconto.DescontoComImposto.ToString("n2"),
                        Vencimento = desconto.Vencimento.DataFormatada(),
                        VencimentoGR = desconto.VencimentoGR.DataFormatada(),
                        FreeTimeGR = desconto.FreeTimeGR.DataFormatada(),
                        FreeTime = desconto.FreeTime.DataFormatada()
                    }, JsonRequestBehavior.AllowGet);
                }
            }

            return null;
        }

        [HttpPost]
        public ActionResult ExcluirDesconto(int id)
        {
            var desconto = _solicitacaoRepositorio.ObterDescontoPorId(id);

            if (desconto == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Registro não encontrado ou já excluído");

            if (!User.IsInRole("Administrador"))
            {
                var usuarioCriacao = _usuarioRepositorio.ObterUsuarioPorId(desconto.CriadoPor);

                if (usuarioCriacao.Externo)
                {
                    if (usuarioCriacao.LoginExterno.ToLower() != User.Identity.Name.ToLower())
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"O Desconto só pode ser excluído pelo usuário de criação ({usuarioCriacao.Login})");
                }
                else
                {
                    if (usuarioCriacao.Login.ToLower() != User.Identity.Name.ToLower())
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"O Desconto só pode ser excluído pelo usuário de criação ({usuarioCriacao.Login})");
                }                
            }

            _solicitacaoRepositorio.ExcluirDesconto(id);

            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        [HttpGet]
        public ActionResult ObterDetalhesGR(TipoPesquisa tipoPesquisa, string lote, string seq_gr, string bl)
        {
            var servicos = new List<ServicoFaturamento>();

            GR gr = new GR();

            if (tipoPesquisa == TipoPesquisa.LOTE)
            {
                gr = _grRepositorio.ObterDetalhesGRPorLote(lote.ToInt(), seq_gr.ToInt());
            }

            if (tipoPesquisa == TipoPesquisa.BL)
            {
                gr = _grRepositorio.ObterDetalhesGRPorBL(bl, seq_gr);
            }

            if (tipoPesquisa == TipoPesquisa.GR)
            {
                gr = _grRepositorio.ObterDetalhesGR(seq_gr.ToInt());
            }

            // Se não encontrar nenhuma GR...

            if (gr == null)
            {
                // Existe pré cálculo?

                GR preCalculo = new GR();

                if (tipoPesquisa == TipoPesquisa.LOTE)
                {
                    preCalculo = _grRepositorio.ObterDetalhesPreCalculoLote(lote.ToInt());
                }

                if (tipoPesquisa == TipoPesquisa.BL)
                {
                    preCalculo = _grRepositorio.ObterDetalhesPreCalculoPorBL(bl);
                }

                if (preCalculo == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"Nenhum registro encontrado para a GR {seq_gr}");
                }

                servicos = _servicoFaturamentoRepositorio.ObterServicosPreCalculoPorBL(preCalculo.Lote).ToList();

                var servicosFaturadosPreCalculo = servicos
                     .Select(c => new ServicoFaturamentoDTO
                     {
                         Id = c.Id,
                         Descricao = c.Descricao,
                         Servico = c.Servico,
                         Valor = c.Valor
                     }).ToList();

                return Json(new
                {
                    preCalculo.SeqGR,
                    preCalculo.ClienteId,
                    preCalculo.ClienteDescricao,
                    preCalculo.IndicadorId,
                    preCalculo.IndicadorDescricao,
                    preCalculo.Proposta,
                    preCalculo.Lote,
                    preCalculo.Periodos,
                    preCalculo.StatusGR,
                    preCalculo.FormaPagamento,
                    DescricaoFormaPagamento = preCalculo.FormaPagamento.ToName(),
                    Valor = preCalculo.Valor.ToString("n2"),
                    Vencimento = preCalculo.Vencimento.DataFormatada(),
                    FreeTime = preCalculo.FreeTime.DataFormatada(),
                    servicosFaturados = servicosFaturadosPreCalculo
                }, JsonRequestBehavior.AllowGet);
            }

            if (gr.StatusGR != "GE")
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"GR {gr.SeqGR} não possui status GERADA");
            }

            servicos = _servicoFaturamentoRepositorio.ObterServicosPorBL(gr.Lote, gr.SeqGR).ToList();

            var servicosFaturados = servicos
                 .Select(c => new ServicoFaturamentoDTO
                 {
                     Id = c.Id,
                     Descricao = c.Descricao,
                     Servico = c.Servico,
                     Valor = c.Valor
                 }).ToList();

            return Json(new
            {
                gr.SeqGR,
                gr.ClienteId,
                gr.ClienteDescricao,
                gr.IndicadorId,
                gr.IndicadorDescricao,
                gr.Proposta,
                gr.Lote,
                gr.Periodos,
                gr.StatusGR,
                gr.FormaPagamento,
                DescricaoFormaPagamento = gr.FormaPagamento.ToName(),
                Valor = gr.Valor.ToString("n2"),
                Vencimento = gr.Vencimento.DataFormatada(),
                FreeTime = gr.FreeTime.DataFormatada(),
                servicosFaturados
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ObterDetalhesGRRedex(TipoPesquisa tipoPesquisa, string reserva, int seq_gr, string display)
        {
            var servicos = new List<ServicoFaturamento>();

            GR gr = new GR();

            var bookingBusca = _bookingRepositorio.ObterBookingPorReserva(reserva);

            if (bookingBusca == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Booking não encontrado");

            var clienteFatura = Regex.Match(display, @"\(([^)]*)\)").Groups[1].Value;

            gr = _grRepositorio.ObterDetalhesGRRedex(bookingBusca.Id, seq_gr, clienteFatura.ToInt());

            if (gr != null)
            {
                if (gr.StatusGR != null && gr.StatusGR != "GE")
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"GR {gr.SeqGR} não possui status GERADA");
                }

                if (seq_gr > 0)
                    gr.SeqGR = seq_gr;

                servicos = _servicoFaturamentoRepositorio.ObterServicosRedex(bookingBusca.Id, gr.ClienteId, gr.SeqGR).ToList();

                var servicosFaturados = servicos
                     .Select(c => new ServicoFaturamentoDTO
                     {
                         Id = c.Id,
                         Descricao = c.Descricao,
                         Servico = c.Servico,
                         Valor = c.Valor,
                         ClienteDescricao = c.ClienteDescricao
                     }).ToList();

                return Json(new
                {
                    gr.SeqGR,
                    gr.ClienteId,
                    gr.ClienteDescricao,
                    gr.IndicadorId,
                    gr.Reserva,
                    gr.IndicadorDescricao,
                    gr.Proposta,
                    gr.Lote,
                    gr.Periodos,
                    gr.StatusGR,
                    gr.FormaPagamento,
                    Valor = gr.Valor.ToString("n2"),
                    Vencimento = gr.Vencimento.DataFormatada(),
                    servicosFaturados
                }, JsonRequestBehavior.AllowGet);
            }

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"Nenhum registro encontrado para reserva {reserva}");
        }

        [HttpGet]
        public ActionResult CalcularDescontoPorServico(int servicoFaturamentoId, TipoDesconto tipoDesconto, string desconto, int lote, int tabelaId, int solicitacaoId, long seqGr = 0)
        {
            var servicoFaturamento = _servicoFaturamentoRepositorio.ObterServicoFaturamentoPorId(servicoFaturamentoId);

            if (servicoFaturamento == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"Serviço Faturado não encontrado");
            }

            decimal valorDesconto = 0;
            decimal valorServico = 0;
            decimal descontoConvertido = 0;
            decimal valorDescontoNoServico = 0;
            decimal valorDescontoPorcentagem = 0;
            decimal totalDescontoComImposto = 0;

            if (!Decimal.TryParse(desconto, out valorDesconto))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Valor do desconto inválido");

            if (!Decimal.TryParse(servicoFaturamento.Valor.ToString(), out valorServico))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Valor do serviço inválido");

            if (valorDesconto > valorServico)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"O valor do desconto ({valorDesconto.ToString("n2")}) não maior que o valor do serviço ({valorServico.ToString("n2")})");

            if (tipoDesconto == TipoDesconto.REAIS)
            {
                descontoConvertido = valorDesconto;
                valorDescontoNoServico = servicoFaturamento.Valor - descontoConvertido;
            }
            else
            {
                valorDescontoPorcentagem = (servicoFaturamento.Valor / 100) * valorDesconto;
                descontoConvertido = servicoFaturamento.Valor - valorDescontoPorcentagem;
                valorDescontoNoServico = descontoConvertido;
                descontoConvertido = valorDescontoPorcentagem;
            }

            var wsCalculo = new CalculoDescontoService(
                lote,
                servicoFaturamento.Servico,
                descontoConvertido,
                tabelaId,
                solicitacaoId,
                seqGr);

            try
            {
                var retornoWs = wsCalculo.CalcularDesconto();

                if (tipoDesconto == TipoDesconto.REAIS)
                {
                    totalDescontoComImposto = (valorDesconto + retornoWs.Imposto);
                }
                else
                {
                    totalDescontoComImposto = (valorDescontoPorcentagem + retornoWs.Imposto);
                }

                return Json(new
                {
                    ValorDescontoNoServico = valorDescontoNoServico.ToString("n2"),
                    ValorFinal = retornoWs.ValorFinal.ToString("n2"),
                    DescontoComImposto = totalDescontoComImposto.ToString("n2")
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [HttpGet]
        public ActionResult CalcularDescontoRedex(int servicoFaturamentoId, TipoDesconto tipoDesconto, string desconto, string reserva, int tabelaId, int solicitacaoId, long seqGr, string display)
        {
            var servicoFaturamento = _servicoFaturamentoRepositorio.ObterServicoFaturamentoRedexPorId(servicoFaturamentoId);

            if (servicoFaturamento == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"Serviço Faturado não encontrado");
            }

            decimal valorDesconto = 0;
            decimal valorServico = 0;
            decimal descontoConvertido = 0;
            decimal valorDescontoNoServico = 0;
            decimal valorDescontoPorcentagem = 0;
            decimal totalDescontoComImposto = 0;

            if (!Decimal.TryParse(desconto, out valorDesconto))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Valor do desconto inválido");

            if (!Decimal.TryParse(servicoFaturamento.Valor.ToString(), out valorServico))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Valor do serviço inválido");

            if (tipoDesconto == TipoDesconto.REAIS)
            {
                descontoConvertido = valorDesconto;
                valorDescontoNoServico = servicoFaturamento.Valor - descontoConvertido;
            }
            else
            {
                valorDescontoPorcentagem = (servicoFaturamento.Valor / 100) * valorDesconto;
                descontoConvertido = servicoFaturamento.Valor - valorDescontoPorcentagem;
                valorDescontoNoServico = descontoConvertido;
                descontoConvertido = valorDescontoPorcentagem;
            }

            var bookingBusca = _bookingRepositorio.ObterBookingPorReserva(reserva);

            var clienteFatura = Regex.Match(display, @"\(([^)]*)\)").Groups[1].Value;

            var wsCalculo = new CalculoDescontoRedexService(
                bookingBusca.Id,
                servicoFaturamento.Servico,
                descontoConvertido,
                tabelaId,
                solicitacaoId,
                seqGr,
                clienteFatura.ToInt());

            try
            {
                var retornoWs = wsCalculo.CalcularDescontoRedex();

                if (tipoDesconto == TipoDesconto.REAIS)
                {
                    totalDescontoComImposto = (valorDesconto + retornoWs.Imposto);
                }
                else
                {
                    totalDescontoComImposto = (valorDescontoPorcentagem + retornoWs.Imposto);
                }

                return Json(new
                {
                    ValorDescontoNoServico = valorDescontoNoServico.ToString("n2"),
                    ValorFinal = retornoWs.ValorFinal.ToString("n2"),
                    DescontoComImposto = totalDescontoComImposto.ToString("n2")
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [HttpGet]
        public ActionResult CalcularDescontoMinutaPorServico(int minuta, TipoDesconto tipoDesconto, int servico, string desconto, int solicitacaoId)
        {
            var servicoFaturamento = _minutaRepositorio.ObterServicoFaturamentoPorId(servico);

            if (servicoFaturamento == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"Serviço Faturado não encontrado");
            }

            decimal valorDesconto = 0;
            decimal valorServico = 0;
            decimal descontoConvertido = 0;
            decimal valorDescontoNoServico = 0;
            decimal valorDescontoPorcentagem = 0;
            decimal totalDescontoComImposto = 0;

            if (!Decimal.TryParse(desconto, out valorDesconto))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Valor do desconto inválido");

            if (!Decimal.TryParse(servicoFaturamento.Valor.ToString(), out valorServico))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Valor do serviço inválido");

            if (tipoDesconto == TipoDesconto.REAIS)
            {
                descontoConvertido = valorDesconto;
                valorDescontoNoServico = servicoFaturamento.Valor - descontoConvertido;
            }
            else
            {
                valorDescontoPorcentagem = (servicoFaturamento.Valor / 100) * valorDesconto;
                descontoConvertido = servicoFaturamento.Valor - valorDescontoPorcentagem;
                valorDescontoNoServico = descontoConvertido;
                descontoConvertido = valorDescontoPorcentagem;
            }

            var wsCalculo = new CalculoDescontoMinutaService(
                minuta,
                servicoFaturamento.Servico,
                descontoConvertido,
                solicitacaoId);

            try
            {
                var retornoWs = wsCalculo.CalcularDesconto();

                if (tipoDesconto == TipoDesconto.REAIS)
                {
                    totalDescontoComImposto = (valorDesconto + retornoWs.Imposto);
                }
                else
                {
                    totalDescontoComImposto = (valorDescontoPorcentagem + retornoWs.Imposto);
                }

                return Json(new
                {
                    ValorDescontoNoServico = valorDescontoNoServico.ToString("n2"),
                    ValorFinal = retornoWs.ValorFinal.ToString("n2"),
                    DescontoComImposto = totalDescontoComImposto.ToString("n2")
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [HttpPost]
        public ActionResult EnviarParaAprovacao(TipoSolicitacao tipoSolicitacao, int solicitacaoId)
        {
            var solicitacaoBusca = _solicitacaoRepositorio.ObterDetalhesSolicitacao(solicitacaoId);

            if (solicitacaoBusca == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Solicitação Comercial não encontrada");

            var token = Autenticador.Autenticar();

            if (token == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Não foi possível se autenticar no serviço de Workflow");

            var workflow = new WorkflowService(token);

            RetornoWorkflow retornoWorkflow = new RetornoWorkflow();

            switch (tipoSolicitacao)
            {
                case TipoSolicitacao.CANCELAMENTO_NF:

                    var registrosCancelamento = _solicitacaoRepositorio.ObterCancelamentosNF(solicitacaoBusca.Id);

                    if (!registrosCancelamento.Any())
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Não há registros de cancelamento cadastrados nesta solicitação");

                    var solicitacaoCancelamentoWorkflow = new SolicitacaoWorkflowDTO()
                    {
                        SolicitacaoId = solicitacaoBusca.Id,
                        Descricao = $"C-{solicitacaoBusca.Id}",
                        TipoSolicitacao = solicitacaoBusca.TipoSolicitacao.ToName(),
                        StatusSolicitacao = solicitacaoBusca.StatusSolicitacao.ToName(),
                        UnidadeSolicitacao = solicitacaoBusca.UnidadeSolicitacaoDescricao,
                        AreaOcorrenciaSolicitacao = solicitacaoBusca.AreaOcorrenciaSolicitacao.ToName(),
                        TipoOperacao = solicitacaoBusca.TipoOperacaoResumida,
                        Ocorrencia = solicitacaoBusca.Ocorrencia,
                        Justificativa = solicitacaoBusca.Justificativa,
                        Motivo = solicitacaoBusca.Motivo,
                        Cliente = solicitacaoBusca.Cliente,
                        CriadoPor = solicitacaoBusca.CriadoPor,
                        ValorNovaNF = registrosCancelamento.Sum(c => c.ValorNovaNF).ToString("n2"),
                        ValorDesconto = registrosCancelamento.Sum(c => c.Desconto).ToString("n2"),
                        Empresa = ObterEmpresaPorUnidadeSolicitacao(solicitacaoBusca.UnidadeSolicitacao),
                        Lote = registrosCancelamento.Select(c => c.Lote).FirstOrDefault(),
                        RazaoSocial = registrosCancelamento.Select(c => c.RazaoSocial).FirstOrDefault().ToString(),
                        Indicador = registrosCancelamento.Select(c => c.Indicador).FirstOrDefault().ToString(),
                        IndicadorDocumento = registrosCancelamento.Select(c => c.IndicadorDocumento).FirstOrDefault().ToString(),
                        Notas = string.Join(",", registrosCancelamento.Select(c => string.Concat(c.NFE, " - Emissão: ", c.DataEmissao.DataFormatada()))),
                        QuantidadeNF = registrosCancelamento.Count(),
                        ValorTotalNF = registrosCancelamento.Sum(c => c.ValorNF).ToString("n2"),
                        User_Externo = User.IsInRole("UsuarioExterno").ToInt()
                    };

                    solicitacaoCancelamentoWorkflow.Substituicao = 0;

                    var dataEmissao = registrosCancelamento
                        .Select(c => c.DataEmissao)
                        .FirstOrDefault()
                        .ToString();

                    if (DateTime.TryParse(dataEmissao, out DateTime dataBase))
                    {
                        if (dataBase.Month != DateTime.Now.Month)
                        {
                            if (dataBase.Month == DateTime.Now.AddMonths(-1).Month)
                            {
                                var parametrosSAP = _parametrosRepositorio.ObterParametrosFatura(solicitacaoBusca.UnidadeSolicitacao);

                                if (parametrosSAP.DiaUtilCancelamentoSAP < DateTime.Now.Day)
                                    solicitacaoCancelamentoWorkflow.Substituicao = 1;
                            }
                            else
                            {
                                solicitacaoCancelamentoWorkflow.Substituicao = 1;
                            }
                        }
                    }

                    retornoWorkflow = workflow.EnviarParaAprovacao(
                        new CadastroWorkflow(Processo.SOLICITACAO_CANCELAMENTO, solicitacaoCancelamentoWorkflow.Empresa, solicitacaoBusca.Id, User.ObterLogin(), User.ObterNome(), User.ObterEmail(), JsonConvert.SerializeObject(solicitacaoCancelamentoWorkflow)));

                    if (retornoWorkflow == null)
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Nenhuma resposta do serviço de Workflow");

                    var solicitacaoWorkflowCancelamento = new EnvioWorkflow(solicitacaoBusca.Id, Processo.SOLICITACAO_CANCELAMENTO, retornoWorkflow.protocolo, retornoWorkflow.mensagem, User.ObterId());
                    _workflowRepositorio.IncluirEnvioAprovacao(solicitacaoWorkflowCancelamento);

                    break;
                case TipoSolicitacao.DESCONTO:

                    var solicitacaoDescontoWorkflow = new SolicitacaoWorkflowDTO()
                    {
                        SolicitacaoId = solicitacaoBusca.Id,
                        Descricao = $"D-{solicitacaoBusca.Id}",
                        TipoSolicitacao = solicitacaoBusca.TipoSolicitacao.ToName(),
                        StatusSolicitacao = solicitacaoBusca.StatusSolicitacao.ToName(),
                        UnidadeSolicitacao = solicitacaoBusca.UnidadeSolicitacaoDescricao,
                        AreaOcorrenciaSolicitacao = solicitacaoBusca.AreaOcorrenciaSolicitacao.ToName(),
                        TipoOperacao = solicitacaoBusca.TipoOperacaoResumida,
                        Ocorrencia = solicitacaoBusca.Ocorrencia,
                        Justificativa = solicitacaoBusca.Justificativa,
                        Motivo = solicitacaoBusca.Motivo,
                        Cliente = solicitacaoBusca.Cliente,
                        CriadoPor = solicitacaoBusca.CriadoPor,
                        Empresa = ObterEmpresaPorUnidadeSolicitacao(solicitacaoBusca.UnidadeSolicitacao),
                        User_Externo = User.IsInRole("UsuarioExterno").ToInt()
                    };

                    bool existeRegistros;

                    if (solicitacaoBusca.OcorrenciaId == 4)
                    {
                        var registrosDescontoEmBoleto = _solicitacaoRepositorio.ObterCancelamentosNF(solicitacaoBusca.Id);

                        existeRegistros = registrosDescontoEmBoleto.Any();

                        if (existeRegistros)
                        {
                            solicitacaoDescontoWorkflow.Lote = registrosDescontoEmBoleto.Select(c => c.Lote).FirstOrDefault();
                            solicitacaoDescontoWorkflow.RazaoSocial = registrosDescontoEmBoleto.Select(c => c.RazaoSocial).FirstOrDefault().ToString();
                            solicitacaoDescontoWorkflow.Notas = string.Join(",", registrosDescontoEmBoleto.Select(c => c.NFE));
                            solicitacaoDescontoWorkflow.ValorTotalNF = registrosDescontoEmBoleto.Sum(c => c.ValorNF).ToString("n2");
                            solicitacaoDescontoWorkflow.ValorNovaNF = registrosDescontoEmBoleto.Sum(c => c.ValorNovaNF).ToString("n2");
                            solicitacaoDescontoWorkflow.Cliente = registrosDescontoEmBoleto.Select(c => c.RazaoSocial).FirstOrDefault().ToString();
                            solicitacaoDescontoWorkflow.ValorDesconto = registrosDescontoEmBoleto.Select(c => c.Desconto).FirstOrDefault().ToString("n2");
                            solicitacaoDescontoWorkflow.DataProrrogacao = registrosDescontoEmBoleto.Select(c => c.DataProrrogacao).FirstOrDefault().ToString();
                            solicitacaoDescontoWorkflow.DataCadastro = registrosDescontoEmBoleto.Select(c => c.DataCadastro).FirstOrDefault().ToString();
                            solicitacaoDescontoWorkflow.DataEmissao = registrosDescontoEmBoleto.Select(c => c.DataEmissao).FirstOrDefault().DataFormatada();
                            solicitacaoDescontoWorkflow.FormaPagamento = registrosDescontoEmBoleto.Select(c => c.FormaPagamento).FirstOrDefault().ToString();
                            solicitacaoDescontoWorkflow.ValorCIF = registrosDescontoEmBoleto.Select(c => c.ValorCIF).FirstOrDefault().ToString("n2");
                        }
                    }
                    else
                    {
                        var registrosDesconto = _solicitacaoRepositorio.ObterDescontos(solicitacaoBusca.Id);

                        existeRegistros = registrosDesconto.Any();

                        if (existeRegistros)
                        {
                            solicitacaoDescontoWorkflow.ValorProcesso = registrosDesconto.Select(c => c.ValorGR).FirstOrDefault().ToString("n2");
                            solicitacaoDescontoWorkflow.Indicador = registrosDesconto.Select(c => c.IndicadorDescricao).FirstOrDefault();
                            solicitacaoDescontoWorkflow.IndicadorDocumento = registrosDesconto.Select(c => c.IndicadorDocumento).FirstOrDefault();
                            solicitacaoDescontoWorkflow.RazaoSocial = registrosDesconto.Select(c => c.RazaoSocial).FirstOrDefault();
                            solicitacaoDescontoWorkflow.DescontoFinal = registrosDesconto.OrderByDescending(c => c.Id).Select(c => c.DescontoFinal).FirstOrDefault().ToString("n2");
                            solicitacaoDescontoWorkflow.Lote = registrosDesconto.Select(c => c.Lote).FirstOrDefault();
                            solicitacaoDescontoWorkflow.GR = registrosDesconto.Select(c => c.GR).FirstOrDefault();
                            solicitacaoDescontoWorkflow.Minuta = registrosDesconto.Select(c => c.Minuta).FirstOrDefault().ToString();
                            solicitacaoDescontoWorkflow.DescontoComImposto = registrosDesconto.Select(c => c.DescontoComImposto).FirstOrDefault().ToString();
                            solicitacaoDescontoWorkflow.ValorCIF = registrosDesconto.Select(c => c.ValorCIF).FirstOrDefault().ToString("n2");
                            solicitacaoDescontoWorkflow.Cliente = registrosDesconto.Select(c => c.RazaoSocial).FirstOrDefault();

                            var valorProcesso = registrosDesconto.Select(c => c.ValorGR).FirstOrDefault();
                            var valorFinalUltimoRegistro = registrosDesconto.OrderByDescending(c => c.Id).Select(c => c.DescontoFinal).FirstOrDefault();

                            solicitacaoDescontoWorkflow.DiferencaValorDesconto = (valorProcesso - valorFinalUltimoRegistro).ToString("n2");
                        }
                    }

                    if (!existeRegistros)
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Não há registros de desconto cadastrados nesta solicitação");

                    retornoWorkflow = workflow.EnviarParaAprovacao(
                        new CadastroWorkflow(Processo.SOLICITACAO_DESCONTO, solicitacaoDescontoWorkflow.Empresa, solicitacaoBusca.Id, User.ObterLogin(), User.ObterNome(), User.ObterEmail(), JsonConvert.SerializeObject(solicitacaoDescontoWorkflow)));

                    if (retornoWorkflow == null)
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Nenhuma resposta do serviço de Workflow");

                    var solicitacaoWorkflowDesconto = new EnvioWorkflow(solicitacaoBusca.Id, Processo.SOLICITACAO_DESCONTO, retornoWorkflow.protocolo, retornoWorkflow.mensagem, User.ObterId());
                    _workflowRepositorio.IncluirEnvioAprovacao(solicitacaoWorkflowDesconto);

                    break;
                case TipoSolicitacao.PRORROGACAO_BOLETO:

                    var registrosProrrogacao = _solicitacaoRepositorio.ObterProrrogacoes(solicitacaoBusca.Id);

                    if (!registrosProrrogacao.Any())
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Não há registros de prorrogação cadastrados nesta solicitação");

                    var solicitacaoProrrogacaoWorkflow = new SolicitacaoWorkflowDTO()
                    {
                        SolicitacaoId = solicitacaoBusca.Id,
                        Descricao = $"P-{solicitacaoBusca.Id}",
                        TipoSolicitacao = solicitacaoBusca.TipoSolicitacao.ToName(),
                        StatusSolicitacao = solicitacaoBusca.StatusSolicitacao.ToName(),
                        UnidadeSolicitacao = solicitacaoBusca.UnidadeSolicitacaoDescricao,
                        AreaOcorrenciaSolicitacao = solicitacaoBusca.AreaOcorrenciaSolicitacao.ToName(),
                        TipoOperacao = solicitacaoBusca.TipoOperacaoResumida,
                        Ocorrencia = solicitacaoBusca.Ocorrencia,
                        Justificativa = solicitacaoBusca.Justificativa,
                        Motivo = solicitacaoBusca.Motivo,
                        Cliente = solicitacaoBusca.Cliente,
                        CriadoPor = solicitacaoBusca.CriadoPor,
                        DataProrrogacao = registrosProrrogacao.Select(c => c.DataProrrogacao).FirstOrDefault().DataFormatada(),
                        Vencimento = registrosProrrogacao.Select(c => c.VencimentoOriginal).FirstOrDefault().DataFormatada(),
                        Empresa = ObterEmpresaPorUnidadeSolicitacao(solicitacaoBusca.UnidadeSolicitacao),
                        Notas = string.Join(",", registrosProrrogacao.Select(c => c.NFE)),
                        RazaoSocial = registrosProrrogacao.Select(c => c.RazaoSocial).FirstOrDefault(),
                        QuantidadeNF = registrosProrrogacao.Count(),
                        ValorTotalNF = registrosProrrogacao.Sum(c => c.ValorNF).ToString("n2"),
                        ValorTotalJuros = registrosProrrogacao.Sum(c => c.ValorJuros).ToString("n2"),
                        isentarjuros = solicitacaoBusca.isentarjuros,
                        User_Externo = User.IsInRole("UsuarioExterno").ToInt()
                    };

                    retornoWorkflow = workflow.EnviarParaAprovacao(
                        new CadastroWorkflow(Processo.SOLICITACAO_PRORROGACAO, solicitacaoProrrogacaoWorkflow.Empresa, solicitacaoBusca.Id, User.ObterLogin(), User.ObterNome(), User.ObterEmail(), JsonConvert.SerializeObject(solicitacaoProrrogacaoWorkflow)));

                    if (retornoWorkflow == null)
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Nenhuma resposta do serviço de Workflow");

                    var solicitacaoWorkflowProrrogacao = new EnvioWorkflow(solicitacaoBusca.Id, Processo.SOLICITACAO_PRORROGACAO, retornoWorkflow.protocolo, retornoWorkflow.mensagem, User.ObterId());
                    _workflowRepositorio.IncluirEnvioAprovacao(solicitacaoWorkflowProrrogacao);

                    break;
                case TipoSolicitacao.RESTITUICAO:

                    var registrosRestituicao = _solicitacaoRepositorio.ObterRestituicoes(solicitacaoBusca.Id);

                    if (!registrosRestituicao.Any())
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Não há registros de restituição cadastrados nesta solicitação");

                    var solicitacaoRestituicaoWorkflow = new SolicitacaoWorkflowDTO()
                    {
                        SolicitacaoId = solicitacaoBusca.Id,
                        Descricao = $"R-{solicitacaoBusca.Id}",
                        TipoSolicitacao = solicitacaoBusca.TipoSolicitacao.ToName(),
                        StatusSolicitacao = solicitacaoBusca.StatusSolicitacao.ToName(),
                        UnidadeSolicitacao = solicitacaoBusca.UnidadeSolicitacaoDescricao,
                        AreaOcorrenciaSolicitacao = solicitacaoBusca.AreaOcorrenciaSolicitacao.ToName(),
                        TipoOperacao = solicitacaoBusca.TipoOperacaoResumida,
                        Ocorrencia = solicitacaoBusca.Ocorrencia,
                        Justificativa = solicitacaoBusca.Justificativa,
                        Motivo = solicitacaoBusca.Motivo,
                        Cliente = solicitacaoBusca.Cliente,
                        CriadoPor = solicitacaoBusca.CriadoPor,
                        Empresa = ObterEmpresaPorUnidadeSolicitacao(solicitacaoBusca.UnidadeSolicitacao),
                        Notas = string.Join(",", registrosRestituicao.Select(c => c.NFE)),
                        RazaoSocial = registrosRestituicao.Select(c => c.FavorecidoDescricao).FirstOrDefault(),
                        QuantidadeNF = registrosRestituicao.Count(),
                        ValorTotalNF = registrosRestituicao.Sum(c => c.ValorNF).ToString("n2"),
                        ValorDevido = solicitacaoBusca.ValorDevido.ToString("n2"),
                        ValorCobrado = solicitacaoBusca.ValorCobrado.ToString("n2"),
                        ValorCredito = solicitacaoBusca.ValorCredito.ToString("n2"),
                        User_Externo = User.IsInRole("UsuarioExterno").ToInt()
                    };

                    retornoWorkflow = workflow.EnviarParaAprovacao(
                        new CadastroWorkflow(Processo.SOLICITACAO_RESTITUICAO, solicitacaoRestituicaoWorkflow.Empresa, solicitacaoBusca.Id, User.ObterLogin(), User.ObterNome(), User.ObterEmail(), JsonConvert.SerializeObject(solicitacaoRestituicaoWorkflow)));

                    if (retornoWorkflow == null)
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Nenhuma resposta do serviço de Workflow");

                    var solicitacaoWorkflowRestituicao = new EnvioWorkflow(solicitacaoBusca.Id, Processo.SOLICITACAO_RESTITUICAO, retornoWorkflow.protocolo, retornoWorkflow.mensagem, User.ObterId());
                    _workflowRepositorio.IncluirEnvioAprovacao(solicitacaoWorkflowRestituicao);

                    break;


                case TipoSolicitacao.OUTROS:

                    var registrosOutros = _solicitacaoRepositorio
                        .ObterAlteracoesFormaPagamento(solicitacaoBusca.Id);

                    if (solicitacaoBusca.OcorrenciaId == 11)
                    {
                        if (!registrosOutros.Any())
                            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Não há registros de alteração de forma de pagamento cadastrados nesta solicitação");
                    }

                    var solicitacaoOutros = new SolicitacaoWorkflowDTO()
                    {
                        SolicitacaoId = solicitacaoBusca.Id,
                        Descricao = $"O-{solicitacaoBusca.Id}",
                        TipoSolicitacao = solicitacaoBusca.TipoSolicitacao.ToName(),
                        StatusSolicitacao = solicitacaoBusca.StatusSolicitacao.ToName(),
                        UnidadeSolicitacao = solicitacaoBusca.UnidadeSolicitacaoDescricao,
                        AreaOcorrenciaSolicitacao = solicitacaoBusca.AreaOcorrenciaSolicitacao.ToName(),
                        TipoOperacao = solicitacaoBusca.TipoOperacaoResumida,
                        Ocorrencia = solicitacaoBusca.Ocorrencia,
                        Justificativa = solicitacaoBusca.Justificativa,
                        Motivo = solicitacaoBusca.Motivo,
                        CriadoPor = solicitacaoBusca.CriadoPor,
                        Empresa = ObterEmpresaPorUnidadeSolicitacao(solicitacaoBusca.UnidadeSolicitacao),
                        Lote = registrosOutros.Select(c => c.Lote).FirstOrDefault(),
                        GR = registrosOutros.Select(c => c.GR).FirstOrDefault(),
                        Valor = registrosOutros.Select(c => c.Valor).FirstOrDefault().ToString("n2"),
                        Indicador = registrosOutros.Select(c => c.Indicador).FirstOrDefault(),
                        Cliente = registrosOutros.Select(c => c.Cliente).FirstOrDefault(),
                        FaturadoContra = registrosOutros.Select(c => c.FaturadoContra).FirstOrDefault(),
                        FaturadoContraDocumento = registrosOutros.Select(c => c.FaturadoContraDocumento).FirstOrDefault(),
                        EmailNotaFiscal = registrosOutros.Select(c => c.EmailNota).FirstOrDefault(),
                        CondicaoPagamento = registrosOutros.Select(c => c.CondicaoPagamentoDescricao).FirstOrDefault(),
                        User_Externo = User.IsInRole("UsuarioExterno").ToInt()
                    };

                    retornoWorkflow = workflow.EnviarParaAprovacao(
                        new CadastroWorkflow(Processo.SOLICITACAO_OUTROS, solicitacaoOutros.Empresa, solicitacaoBusca.Id, User.ObterLogin(), User.ObterNome(), User.ObterEmail(), JsonConvert.SerializeObject(solicitacaoOutros, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore })));

                    if (retornoWorkflow == null)
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Nenhuma resposta do serviço de Workflow");

                    if (retornoWorkflow.sucesso)
                    {
                        var solicitacaoWorkflowOutros = new EnvioWorkflow(solicitacaoBusca.Id, Processo.SOLICITACAO_OUTROS, retornoWorkflow.protocolo, retornoWorkflow.mensagem, User.ObterId());
                        _workflowRepositorio.IncluirEnvioAprovacao(solicitacaoWorkflowOutros);
                    }

                    break;

                default:
                    break;
            }

            if (retornoWorkflow.sucesso == false)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, retornoWorkflow.mensagem);

            _solicitacaoRepositorio.AtualizarStatusSolicitacao(StatusSolicitacao.EM_APROVAVAO, solicitacaoBusca.Id);

            return Json(new
            {
                RedirectUrl = Url.Action(nameof(Atualizar), new { id = solicitacaoBusca.Id })
            });
        }

        [HttpGet]
        public ActionResult ObterHistoricoWorkflow(int id, int idProcesso)
        {
            try
            {
                var solicitacaoBusca = _solicitacaoRepositorio.ObterDetalhesSolicitacao(id);

                if (solicitacaoBusca == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Solicitação Comercial não encontrada");

                var token = Autenticador.Autenticar();

                if (token == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Não foi possível se autenticar no serviço de Workflow");

                var workflow = new WorkflowService(token)
                    .ObterHistoricoWorkflow(id, idProcesso, ObterEmpresaPorUnidadeSolicitacao(solicitacaoBusca.UnidadeSolicitacao));

                return PartialView("_ConsultaHistoricoWorkflow", new HistoricoWorkflowViewModel
                {
                    WorkFlows = workflow.list.SelectMany(c => c.workFlows).ToList()
                });
            }
            catch (Exception)
            {
                return null;
            }
        }

        [HttpPost]
        public ActionResult CadastrarAnexos([Bind(Include = "AnexoSolicitacaoId")] SolicitacaoComercialAnexosViewModel viewModel, HttpPostedFileBase solicitacaoAnexo)
        {
            if (ModelState.IsValid)
            {
                var solicitacaoBusca = _solicitacaoRepositorio.ObterSolicitacaoPorId(viewModel.AnexoSolicitacaoId);

                if (solicitacaoBusca == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Solicitação Comercial não encontrada");

                if (solicitacaoAnexo == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Selecione um arquivo para anexar na Solicitação");

                try
                {
                    IncluirAnexo(solicitacaoBusca.Id, TipoAnexo.OUTROS, solicitacaoAnexo);
                }
                catch (HttpException ex)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ex.Message);
                }

                PopularAnexos(viewModel);

                return PartialView("_AbaAnexosConsulta", viewModel.Anexos);
            }

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Falha ao anexar o arquivo");
        }

        private int IncluirAnexo(int solicitacaoId, TipoAnexo tipoAnexo, HttpPostedFileBase anexo)
        {
            if (anexo != null && anexo.ContentLength > 0)
            {
                var token = Sharepoint.Services.Autenticador.Autenticar();

                if (token == null)
                    throw new HttpException(404, "Não foi possível se autenticar no serviço de Anexos");

                using (var binaryReader = new BinaryReader(anexo.InputStream))
                {
                    byte[] byteArray = binaryReader.ReadBytes(anexo.ContentLength);

                    var dados = new DadosArquivoUpload
                    {
                        Name = anexo.FileName,
                        Extension = Path.GetExtension(anexo.FileName),
                        System = 3,
                        DataArray = Convert.ToBase64String(byteArray)
                    };

                    var retornoUpload = new Sharepoint.Services.AnexosService(token)
                        .EnviarArquivo(dados);

                    var anexoInclusaoId = _anexoRepositorio.IncluirAnexo(
                        new Anexo
                        {
                            IdProcesso = solicitacaoId,
                            Arquivo = anexo.FileName,
                            CriadoPor = User.ObterId(),
                            TipoAnexo = tipoAnexo,
                            TipoDoc = 2,
                            IdArquivo = Converters.GuidToRaw(retornoUpload.Arquivo.id)
                        });

                    return anexoInclusaoId;
                }
            }

            return 0;
        }

        [HttpPost]
        public ActionResult ExcluirAnexo(int id, string idArquivo)
        {
            var anexoBusca = _anexoRepositorio.ObterAnexoPorId(id);

            if (anexoBusca == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Anexo não encontrado ou já excluído");

            if (!User.IsInRole("Administrador"))
            {
                if (anexoBusca.CriadoPor != User.ObterId())
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "O anexo só pode ser excluído pelo usuário de criação");
            }

            if (anexoBusca.TipoAnexo == TipoAnexo.PROPOSTA)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Não é possível excluir um anexo do tipo Proposta");

            try
            {
                var token = Sharepoint.Services.Autenticador.Autenticar();

                if (token == null)
                    throw new HttpException(404, "Não foi possível se autenticar no serviço de Anexos");

                var retornoUpload = new Sharepoint.Services.AnexosService(token)
                    .ExcluirArquivo(idArquivo);

                _anexoRepositorio.ExcluirAnexo(id);

                return new HttpStatusCodeResult(HttpStatusCode.NoContent);
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Falha ao excluir o anexo. Tente novamente.");
            }
        }

        private void PopularAnexos(SolicitacaoComercialAnexosViewModel viewModel)
        {
            var anexos = _solicitacaoRepositorio
                .ObterAnexosDaSolicitacao(viewModel.AnexoSolicitacaoId)
                .ToList();

            viewModel.Anexos = anexos.ToList();
        }

        [HttpGet]
        public ActionResult ObterUsuariosSolicitacao()
        {
            var usuarios = _solicitacaoRepositorio
                .ObterUsuariosSolicitacoes()
                .ToList();

            return Json(usuarios, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult CalcularValorAVista(int lote, string desconto, int notaFiscal)
        {
            if (lote == 0)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Lote não informado.");

            if (!Decimal.TryParse(desconto, out _))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Valor desconto inválido.");

            var detalhesLote = _loteRepositorio.ObterLotePorId(lote);

            NotaFiscal detalhesNota = new NotaFiscal();

            decimal valorImposto = 0;

            if (detalhesLote != null)
            {
                detalhesNota = _notaFiscalRepositorio.ObterDetalhesNotaFiscal(notaFiscal);

                var detalhesImportador = _parceiroRepositorio.ObterDetalhesImportadorPorCnpj(detalhesNota.DocumentoCliente);

                if (detalhesImportador != null)
                {
                    if (detalhesImportador.Cidade == "SANTOS" && detalhesImportador.TipoCliente == "J")
                    {
                        var grs = detalhesNota.GR.Split(',');

                        valorImposto = _servicoFaturamentoRepositorio.ObterValorImposto(lote, grs);
                    }
                }
            }

            return Json(new
            {
                ValorAPagar = detalhesNota.Valor - valorImposto,
                NovoValorAPagar = detalhesNota.Valor - Convert.ToDecimal(desconto) - valorImposto,
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult CalcularValorAVistaRedex(string reserva, string desconto, int notaFiscal)
        {
            decimal valorDesconto = 0;

            if (!Decimal.TryParse(desconto, out valorDesconto))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Valor desconto inválido.");

            var detalhesReserva = _bookingRepositorio.ObterBookingPorReserva(reserva);

            NotaFiscal detalhesNota = new NotaFiscal();

            decimal valorImposto = 0;

            if (detalhesReserva != null)
            {
                detalhesNota = _notaFiscalRepositorio.ObterDetalhesNotaFiscalRedex(notaFiscal);

                if (detalhesNota != null)
                {
                    var detalhesExportador = _parceiroRepositorio.ObterDetalhesExportadorPorCnpj(detalhesNota.DocumentoCliente);

                    if (detalhesExportador.Cidade == "SANTOS" && detalhesExportador.TipoCliente == "J")
                    {
                        var grs = detalhesNota.GR.Split(',');

                        valorImposto = _servicoFaturamentoRepositorio.ObterValorImpostoRedex(detalhesReserva.Id, grs);
                    }
                }
            }

            return Json(new
            {
                ValorAPagar = detalhesNota.Valor - valorImposto,
                NovoValorAPagar = detalhesNota.Valor - valorDesconto - valorImposto,
            }, JsonRequestBehavior.AllowGet);
        }

        public static int ObterEmpresaPorUnidadeSolicitacao(int unidadeSolicitacao)
        {
            switch (unidadeSolicitacao)
            {
                case 1:
                case 2:
                    return 1;
                case 3:
                    return 2;
            }

            return 0;
        }

        [HttpPost]
        public ActionResult CadastrarAlteracaoFormaPagamento([Bind(Include = "AlteracaoFormaPagamentoId, AlteracaoFormaPagamentoSolicitacaoId, AlteracaoFormaPagamentoTipoPesquisa, AlteracaoFormaPagamentoTipoPesquisaNumero, AlteracaoFormaPagamentoLote, AlteracaoFormaPagamentoGrId, AlteracaoFormaPagamentoValor, AlteracaoFormaPagamentoFaturadoContraId, AlteracaoFormaPagamentoFaturadoDescricao, AlteracaoFormaPagamentoCondicaoPagamentoId, AlteracaoFormaPagamentoEncaminharPara")] SolicitacaoComercialAlteracaoFormaPagamentoViewModel viewModel)
        {
            if (viewModel.AlteracaoFormaPagamentoSolicitacaoId == 0)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var solicitacao = _solicitacaoRepositorio.ObterSolicitacaoPorId(viewModel.AlteracaoFormaPagamentoSolicitacaoId);

            if (solicitacao == null)
                RegistroNaoEncontrado();

            var solicitacaoFormaPgto = new SolicitacaoAlteraFormaPagamento(
                viewModel.AlteracaoFormaPagamentoSolicitacaoId,
                viewModel.AlteracaoFormaPagamentoTipoPesquisa,
                viewModel.AlteracaoFormaPagamentoTipoPesquisaNumero,
                viewModel.AlteracaoFormaPagamentoLote,
                viewModel.AlteracaoFormaPagamentoGrId,
                viewModel.AlteracaoFormaPagamentoValor,
                viewModel.AlteracaoFormaPagamentoFaturadoContraId,
                viewModel.AlteracaoFormaPagamentoCondicaoPagamentoId,
                viewModel.AlteracaoFormaPagamentoEncaminharPara,
                User.ObterId());

            if (viewModel.AlteracaoFormaPagamentoId == 0)
            {
                var buscaPorLote = _solicitacaoRepositorio
                    .ObterAlteracoesFormaPagamento(solicitacao.Id)
                    .Where(c => c.Lote != viewModel.AlteracaoFormaPagamentoLote)
                    .Any();

                if (buscaPorLote)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Não é permitido lotes diferentes na mesma solicitação.");

                if (!Validar(solicitacaoFormaPgto))
                    return RetornarErros();

                _solicitacaoRepositorio.CadastrarAlteracaoFormaPgto(solicitacaoFormaPgto);
            }
            else
            {
                var solicitacaoBusca = _solicitacaoRepositorio.ObterAlteracaoFormaPgtoPorId(viewModel.AlteracaoFormaPagamentoId);

                if (solicitacaoBusca == null)
                    RegistroNaoEncontrado();

                var buscaPorLote = _solicitacaoRepositorio
                    .ObterAlteracoesFormaPagamento(solicitacao.Id)
                    .Where(c => c.Lote != viewModel.AlteracaoFormaPagamentoLote && c.Id != viewModel.AlteracaoFormaPagamentoId)
                    .Any();

                if (buscaPorLote)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Não é permitido lotes diferentes na mesma solicitação.");

                if (!User.IsInRole("Administrador"))
                {
                    if (solicitacaoBusca.CriadoPor != User.ObterId())
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "A edição da solicitação é permitida apenas pelo usuário de criação.");
                }

                solicitacaoBusca.Alterar(solicitacaoFormaPgto);

                if (!Validar(solicitacaoBusca))
                    return RetornarErros();

                _solicitacaoRepositorio.AtualizarAlteracaoFormaPgto(solicitacaoBusca);
            }

            var solicitacoes = _solicitacaoRepositorio
                .ObterAlteracoesFormaPagamento(solicitacao.Id)
                .ToList();

            return PartialView("_ConsultarAlteracoesFormasPagamento", solicitacoes);
        }

        [HttpGet]
        public ActionResult ObterDetalhesAlteracaoFormaPgto(int id)
        {
            var dadosSolicitacao = _solicitacaoRepositorio.ObterAlteracaoFormaPgtoPorId(id);

            if (dadosSolicitacao != null)
            {
                return Json(new
                {
                    dadosSolicitacao.Id,
                    dadosSolicitacao.TipoPesquisa,
                    dadosSolicitacao.TipoPesquisaNumero,
                    dadosSolicitacao.Lote,
                    dadosSolicitacao.Gr,
                    dadosSolicitacao.Proposta,
                    dadosSolicitacao.Periodo,
                    dadosSolicitacao.Cliente,
                    dadosSolicitacao.Indicador,
                    dadosSolicitacao.FaturadoContraId,
                    dadosSolicitacao.FaturadoContraDescricao,
                    dadosSolicitacao.EmailNota,
                    dadosSolicitacao.CondicaoPagamentoId,
                    DescricaoFormaPagamento = dadosSolicitacao.FormaPagamento.ToName(),
                    Valor = dadosSolicitacao.Valor.ToString("n2"),
                    FreeTime = dadosSolicitacao.FreeTime.DataFormatada()
                }, JsonRequestBehavior.AllowGet);
            }

            return null;
        }

        [HttpGet]
        public ActionResult ObterSolicitacaoFormaPgtoDefault(int solicitacaoId)
        {
            var registros = _solicitacaoRepositorio
                .ObterAlteracoesFormaPagamento(solicitacaoId);

            if (registros.Count() == 1)
            {
                var solicitacao = registros.FirstOrDefault();

                var dadosSolicitacao = _solicitacaoRepositorio.ObterAlteracaoFormaPgtoPorId(solicitacao.Id);

                if (dadosSolicitacao != null)
                {
                    return Json(new
                    {
                        dadosSolicitacao.Id,
                        dadosSolicitacao.TipoPesquisa,
                        dadosSolicitacao.TipoPesquisaNumero,
                        dadosSolicitacao.Lote,
                        dadosSolicitacao.Gr,
                        dadosSolicitacao.Proposta,
                        dadosSolicitacao.Periodo,
                        dadosSolicitacao.Cliente,
                        dadosSolicitacao.Indicador,
                        dadosSolicitacao.FaturadoContraId,
                        dadosSolicitacao.FaturadoContraDescricao,
                        dadosSolicitacao.EmailNota,
                        dadosSolicitacao.CondicaoPagamentoId,
                        DescricaoFormaPagamento = dadosSolicitacao.FormaPagamento.ToName(),
                        Valor = dadosSolicitacao.Valor.ToString("n2"),
                        FreeTime = dadosSolicitacao.FreeTime.DataFormatada()
                    }, JsonRequestBehavior.AllowGet);
                }
            }

            return null;
        }

        [HttpPost]
        public ActionResult ExcluirSolicitacaoFormaPgto(int id)
        {
            var solicitacao = _solicitacaoRepositorio.ObterAlteracaoFormaPgtoPorId(id);

            if (solicitacao == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Registro não encontrado ou já excluído");

            if (!User.IsInRole("Administrador"))
            {
                var usuarioCriacao = _usuarioRepositorio.ObterUsuarioPorId(solicitacao.CriadoPor);

                if (usuarioCriacao.Login.ToLower() != User.Identity.Name.ToLower())
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"O registro só pode ser excluído pelo usuário de criação ({usuarioCriacao.Login})");
            }

            _solicitacaoRepositorio.ExcluirAlteracoesFormaPagamento(id);

            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }
    }
}