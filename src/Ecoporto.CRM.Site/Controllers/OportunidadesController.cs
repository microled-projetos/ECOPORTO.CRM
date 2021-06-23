using Ecoporto.CRM.Business.DTO;
using Ecoporto.CRM.Business.Enums;
using Ecoporto.CRM.Business.Extensions;
using Ecoporto.CRM.Business.Filtros;
using Ecoporto.CRM.Business.Helpers;
using Ecoporto.CRM.Business.Interfaces.Repositorios;
using Ecoporto.CRM.Business.Interfaces.Servicos;
using Ecoporto.CRM.Business.Models;
using Ecoporto.CRM.Infra.Datatables;
using Ecoporto.CRM.Sharepoint.Models;
using Ecoporto.CRM.Site.Extensions;
using Ecoporto.CRM.Site.Filtros;
using Ecoporto.CRM.Site.Helpers;
using Ecoporto.CRM.Site.Models;
using Ecoporto.CRM.Site.Models.Pdf;
using Ecoporto.CRM.Site.Services;
using Ecoporto.CRM.Workflow.Enums;
using Ecoporto.CRM.Workflow.Models;
using Ecoporto.CRM.Workflow.Services;
using MvcRazorToPdf;
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
using System.Web.Routing;

namespace Ecoporto.CRM.Site.Controllers
{
    [Authorize]
    public class OportunidadesController : BaseController
    {
        // Deixe habilitaDemandaAnaliseDeCredito = false para desabilitar as validações relacionadas a demanda do SPC
        bool habilitaDemandaAnaliseDeCredito = true;

        private readonly IOportunidadeRepositorio _oportunidadeRepositorio;
        private readonly IContaRepositorio _contaRepositorio;
        private readonly IMercadoriaRepositorio _mercadoriaRepositorio;
        private readonly IContatoRepositorio _contatoRepositorio;
        private readonly IImpostoRepositorio _impostoRepositorio;
        private readonly IModeloRepositorio _modeloRepositorio;
        private readonly IVendedorRepositorio _vendedorRepositorio;
        private readonly ILayoutPropostaRepositorio _layoutPropostaRepositorio;
        private readonly ICondicaoPagamentoFaturaRepositorio _condicaoPagamentoFaturaRepositorio;
        private readonly ICidadeRepositorio _cidadeRepositorio;
        private readonly IPremioParceriaRepositorio _premioParceriaRepositorio;
        private readonly IOportunidadeService _oportunidadeService;
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IWorkflowRepositorio _workflowRepositorio;
        private readonly IAnexoRepositorio _anexoRepositorio;
        private readonly IEquipeContaRepositorio _equipeContaRepositorio;
        private readonly ISimuladorRepositorio _simuladorRepositorio;
        private readonly ISimuladorPropostaRepositorio _simuladorPropostaRepositorio;
        private readonly IEmpresaRepositorio _empresaRepositorio;
        private readonly IHubPortRepositorio _hubPortRepositorio;
        private readonly IDocumentoRepositorio _documentoRepositorio;
        private readonly IGrupoAtracacaoRepositorio _grupoAtracacaoRepositorio;
        private readonly IParceiroRepositorio _parceiroRepositorio;
        private readonly IModeloSimuladorRepositorio _modeloSimuladorRepositorio;
        private readonly IMargemRepositorio _margemRepositorio;
        private readonly IParametrosRepositorio _parametrosRepositorio;
        private readonly ILoteRepositorio _loteRepositorio;
        private readonly ITabelasRepositorio _tabelasRepositorio;
        private readonly IEquipesService _equipesService;
        private readonly IAmbienteOracleService _ambienteOracleService;
        private readonly IConcomitanciaTabelaService _concomitanciaTabelaService;
        private readonly IImpostosExcecaoRepositorio _impostosExcecaoRepositorio;
        private readonly IAnaliseCreditoRepositorio _analiseCreditoRepositorio;
        private readonly ILogger _logger;

        public OportunidadesController(
            IOportunidadeRepositorio oportunidadeRepositorio,
            IContaRepositorio contaRepositorio,
            IMercadoriaRepositorio mercadoriaRepositorio,
            IContatoRepositorio contatoRepositorio,
            IImpostoRepositorio impostoRepositorio,
            IModeloRepositorio modeloRepositorio,
            IVendedorRepositorio vendedorRepositorio,
            ILayoutPropostaRepositorio layoutPropostaRepositorio,
            ICondicaoPagamentoFaturaRepositorio condicaoPagamentoFaturaRepositorio,
            ICidadeRepositorio cidadeRepositorio,
            IPremioParceriaRepositorio premioParceriaRepositorio,
            IOportunidadeService oportunidadeService,
            IUsuarioRepositorio usuarioRepositorio,
            IWorkflowRepositorio workflowRepositorio,
            IAnexoRepositorio anexoRepositorio,
            IEquipeContaRepositorio equipeContaRepositorio,
            ISimuladorRepositorio simuladorRepositorio,
            ISimuladorPropostaRepositorio simuladorPropostaRepositorio,
            IEmpresaRepositorio empresaRepositorio,
            IHubPortRepositorio hubPortRepositorio,
            IEquipesService equipesService,
            IDocumentoRepositorio documentoRepositorio,
            IGrupoAtracacaoRepositorio grupoAtracacaoRepositorio,
            IParceiroRepositorio parceiroRepositorio,
            IModeloSimuladorRepositorio modeloSimuladorRepositorio,
            IMargemRepositorio margemRepositorio,
            IParametrosRepositorio parametrosRepositorio,
            ILoteRepositorio loteRepositorio,
            ITabelasRepositorio tabelasRepositorio,
            IAmbienteOracleService ambienteOracleService,
            IConcomitanciaTabelaService concomitanciaTabelaService,
            IImpostosExcecaoRepositorio impostosExcecaoRepositorio,
            IAnaliseCreditoRepositorio analiseCreditoRepositorio,
            ILogger logger) : base(logger)
        {
            _oportunidadeRepositorio = oportunidadeRepositorio;
            _contaRepositorio = contaRepositorio;
            _mercadoriaRepositorio = mercadoriaRepositorio;
            _contatoRepositorio = contatoRepositorio;
            _impostoRepositorio = impostoRepositorio;
            _modeloRepositorio = modeloRepositorio;
            _vendedorRepositorio = vendedorRepositorio;
            _layoutPropostaRepositorio = layoutPropostaRepositorio;
            _condicaoPagamentoFaturaRepositorio = condicaoPagamentoFaturaRepositorio;
            _cidadeRepositorio = cidadeRepositorio;
            _premioParceriaRepositorio = premioParceriaRepositorio;
            _oportunidadeService = oportunidadeService;
            _usuarioRepositorio = usuarioRepositorio;
            _workflowRepositorio = workflowRepositorio;
            _anexoRepositorio = anexoRepositorio;
            _equipeContaRepositorio = equipeContaRepositorio;
            _simuladorRepositorio = simuladorRepositorio;
            _simuladorPropostaRepositorio = simuladorPropostaRepositorio;
            _empresaRepositorio = empresaRepositorio;
            _hubPortRepositorio = hubPortRepositorio;
            _equipesService = equipesService;
            _documentoRepositorio = documentoRepositorio;
            _grupoAtracacaoRepositorio = grupoAtracacaoRepositorio;
            _parceiroRepositorio = parceiroRepositorio;
            _modeloSimuladorRepositorio = modeloSimuladorRepositorio;
            _margemRepositorio = margemRepositorio;
            _parametrosRepositorio = parametrosRepositorio;
            _loteRepositorio = loteRepositorio;
            _tabelasRepositorio = tabelasRepositorio;
            _ambienteOracleService = ambienteOracleService;
            _concomitanciaTabelaService = concomitanciaTabelaService;
            _impostosExcecaoRepositorio = impostosExcecaoRepositorio;
            _analiseCreditoRepositorio = analiseCreditoRepositorio;
            _logger = logger;
        }


        [HttpGet]
        public ActionResult Index(int? id)
        {
            return View();
        }

        [HttpGet]
        [UsuarioExternoFilter]
        public ActionResult Consultar(JQueryDataTablesParamViewModel Params, OportunidadesFiltro filtro)
        {
            if (!string.IsNullOrWhiteSpace(Params.sSearch))
            {
                if (StringHelpers.IsInteger(Params.sSearch))
                {
                    filtro.Identificacao = Params.sSearch.ToInt();
                }
                else
                {
                    filtro.Descricao = Params.sSearch;
                }
            }

            var oportunidades = _oportunidadeRepositorio
                .ObterOportunidades(Params.Pagina, Params.iDisplayLength, filtro, Params.OrderBy, out int totalFiltro, (int?)ViewBag.UsuarioExternoId)
                .Select(c => new
                {
                    c.Id,
                    c.Identificacao,
                    c.Descricao,
                    StatusOportunidade = c.StatusOportunidade.ToName(),
                    SucessoNegociacao = c.SucessoNegociacao.ToName(),
                    EstagioNegociacao = c.EstagioNegociacao.ToName(),
                    TipoDeProposta = c.TipoDeProposta.ToName(),
                    TipoServico = c.TipoServico.ToName(),
                    c.DataFechamento,
                    c.TabelaId,
                    c.Vendedor,
                    DataInicio = c.DataInicio.DataFormatada(),
                    DataTermino = c.DataTermino.DataFormatada(),
                    c.CriadoPor,
                    c.DataCriacao
                });

            var totalRegistros = _oportunidadeRepositorio.ObterTotalOportunidades();

            var resultado = new
            {
                Params.sEcho,
                iTotalRecords = totalRegistros,
                iTotalDisplayRecords = totalFiltro,
                aaData = oportunidades
            };

            return Json(resultado, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [UsuarioExternoFilter]
        public PartialViewResult ConsultarPorDescricao(string descricao)
        {
            var resultado = _oportunidadeRepositorio
                .ObterOportunidadesPorDescricao(descricao, (int?)ViewBag.UsuarioExternoId);

            return PartialView("_AbaInformacoesIniciaisPesquisaOportunidadesConsulta", resultado);
        }

        [HttpGet]
        [UsuarioExternoFilter]
        public PartialViewResult ConsultarContasPorDescricao(string descricao, Segmento? segmento)
        {
            IEnumerable<Conta> resultado = new List<Conta>();

            if (segmento.HasValue)
                resultado = _contaRepositorio.ObterContasImportadoresPorDescricao(descricao);
            else
                resultado = _contaRepositorio.ObterContasPorDescricao(descricao, (int?)ViewBag.UsuarioExternoId);

            return PartialView("_PesquisarContasConsulta", resultado);
        }

        [HttpGet]
        [UsuarioExternoFilter]
        public PartialViewResult ConsultarClientesFontePagadora(string descricao)
        {
            var resultado = _contaRepositorio.ObterContasPorDescricao(descricao, (int?)ViewBag.UsuarioExternoId);

            return PartialView("_AbaFichaFaturamentoPesquisaClientesConsulta", resultado);
        }

        private void PopularContas(OportunidadesInformacoesIniciaisViewModel viewModel)
        {
            var conta = _contaRepositorio.ObterContaPorId(viewModel.ContaId);

            // Session["FontePagadoraId"] = viewModel.ContaId;
            // Session["RazaoSocial"] = viewModel.Descricao;

            if (conta != null)
            {
                viewModel.Contas.Add(new Conta
                {
                    Id = conta.Id,
                    Descricao = $"{conta.Descricao} ({conta.Documento})"
                });
            }
        }

        private void PopularRevisao(OportunidadesInformacoesIniciaisViewModel viewModel)
        {
            if (viewModel.RevisaoId.HasValue)
            {
                var revisao = _oportunidadeRepositorio.ObterOportunidadePorId(viewModel.RevisaoId.Value);
                viewModel.Revisoes.Add(revisao);
            }
        }

        private void PopularEmpresas(OportunidadesInformacoesIniciaisViewModel viewModel)
        {
            viewModel.Empresas = _empresaRepositorio
                .ObterEmpresas()
                .ToList();
        }

        private void PopularMercadorias(OportunidadesInformacoesIniciaisViewModel viewModel)
        {
            viewModel.Mercadorias = _mercadoriaRepositorio
                .ObterMercadorias()
                .ToList();
        }

        private void PopularVendedores(OportunidadesPropostaViewModel viewModel)
        {
            viewModel.Vendedores = _vendedorRepositorio
                .ObterVendedores()
                .ToList();
        }

        private void PopularVendedoresAdendo(OportunidadesAdendosViewModel viewModel)
        {
            viewModel.Vendedores = _vendedorRepositorio
                .ObterVendedores()
                .ToList();
        }

        private void PopularImpostos(OportunidadesPropostaViewModel viewModel)
        {
            viewModel.Impostos = _impostoRepositorio
                .ObterImpostos()
                .ToList();
        }

        private void PopularModelos(OportunidadesPropostaViewModel viewModel)
        {
            viewModel.Modelos = _modeloRepositorio
                .ObterModelosPorTipoOperacao(viewModel.TipoOperacao)
                .ToList();
        }

        private void PopularSubClientes(OportunidadesInformacoesIniciaisViewModel viewModel)
        {
            viewModel.SubClientes = _oportunidadeRepositorio
                .ObterSubClientes(viewModel.Id)
                .ToList();
        }

        private void PopularClientesGrupoCNPJ(OportunidadesInformacoesIniciaisViewModel viewModel)
        {
            viewModel.ClientesGrupoCNPJ = _oportunidadeRepositorio
                .ObterClientesGrupoCNPJ(viewModel.Id)
                .ToList();
        }

        private void PopularClientesDaProposta(OportunidadesAdendosViewModel viewModel)
        {
            viewModel.SubClientes = _oportunidadeRepositorio
                .ObterSubClientesDaProposta(viewModel.AdendoOportunidadeId)
                .ToList();
        }

        private void PopularClientesGrupoCNPJ(OportunidadesAdendosViewModel viewModel)
        {
            viewModel.ClientesGrupoCNPJ = _oportunidadeRepositorio
                .ObterClientesGrupoCNPJ(viewModel.AdendoOportunidadeId)
                .ToList();
        }

        private void PopularClientesDaProposta(OportunidadesFichaFaturamentoViewModel viewModel)
        {
            viewModel.ClientesProposta = _oportunidadeRepositorio
                .ObterSubClientesDaProposta(viewModel.OportunidadeId)
                .Where(c => c.Segmento == SegmentoSubCliente.IMPORTADOR)
                .ToList();
        }

        private void PopularCondicoesPagamento(OportunidadesFichaFaturamentoViewModel viewModel)
        {
            viewModel.CondicoesPagamentoFaturamento = _condicaoPagamentoFaturaRepositorio
                .ObterCondicoesPagamento()
                .ToList();
        }

        private void PopularFichasFaturamento(OportunidadesFichaFaturamentoViewModel viewModel)
        {
            viewModel.FichasFaturamento = _oportunidadeRepositorio
                .ObterFichasFaturamento(viewModel.OportunidadeId)
                .ToList();
        }

        private void PopularAnexos(OportunidadesAnexosViewModel viewModel)
        {
            viewModel.Anexos = ObterAnexos(viewModel.AnexoOportunidadeId);
        }

        [HttpGet]
        public ActionResult ObterAnexosOportunidade(int oportunidadeId)
        {
            return PartialView("_AbaAnexosConsulta", ObterAnexos(oportunidadeId));
        }

        public List<AnexosDTO> ObterAnexos(int oportunidadeId)
        {
            var usuarioExterno = User.IsInRole("UsuarioExterno");

            var anexos = _oportunidadeRepositorio
                .ObterAnexosDaOportunidade(oportunidadeId, usuarioExterno);

            if (!User.IsInRole("OportunidadesPremios:Acessar"))
            {
                anexos = anexos.Where(c => c.TipoAnexo != TipoAnexo.PREMIO_PARCERIA);
            }

            return anexos.ToList();
        }

        private void PopularNotas(OportunidadesNotasViewModel viewModel)
        {
            var anexos = _oportunidadeRepositorio
                .ObterNotasDaOportunidade(viewModel.NotaOportunidadeId)
                .ToList();

            viewModel.Notas = anexos.ToList();
        }

        private void PopularContatos(OportunidadesInformacoesIniciaisViewModel viewModel)
        {
            viewModel.Contatos = _contatoRepositorio
                .ObterContatosPorConta(viewModel.ContaId)
                .ToList();
        }

        private void PopularPremiosParceria(OportunidadesPremioParceriaViewModel viewModel)
        {
            viewModel.Premios = _premioParceriaRepositorio
                .ObterPremiosParceriaPorOportunidade(viewModel.PremioParceriaOportunidadeId)
                .ToList();
        }

        private void PopularAdendos(OportunidadesAdendosViewModel viewModel)
        {
            viewModel.Adendos = _oportunidadeRepositorio
                .ObterAdendos(viewModel.AdendoOportunidadeId)
                .ToList();
        }

        private bool ExisteAdendoFaturadoEmAberto(int oportunidadeId)
        {
            var adendoBusca = _oportunidadeRepositorio
                .ObterAdendos(oportunidadeId)
                .Where(c => (c.StatusAdendo == StatusAdendo.ABERTO || c.StatusAdendo == StatusAdendo.ENVIADO) && c.TipoAdendo == TipoAdendo.FORMA_PAGAMENTO)
                .FirstOrDefault();

            if (adendoBusca != null)
            {
                var adendoFormaPgtoBusca = _oportunidadeRepositorio.ObterAdendoFormaPagamento(adendoBusca.Id);

                if (adendoFormaPgtoBusca != null)
                {
                    return adendoFormaPgtoBusca.FormaPagamento == FormaPagamento.FATURADO;
                }
            }

            return false;
        }

        [HttpPost]
        public PartialViewResult FiltrarAdendos(AdendosFiltro filtro)
        {
            var adendos = _oportunidadeRepositorio.ObterAdendos(filtro);

            return PartialView("_AbaAdendosConsulta", adendos);
        }

        [HttpGet]
        [CanActivate(Roles = "OportunidadesIniciais:Cadastrar")]
        [CanActivateVisualizarOportunidade(Roles = "OportunidadesIniciais:Cadastrar")]
        public ActionResult Cadastrar(int? conta)
        {
            var informacoesIniciaisVieModel = new OportunidadesInformacoesIniciaisViewModel();
            informacoesIniciaisVieModel.PremioParceria = Boleano.SIM;

            if (conta.HasValue)
            {
                var contaBusca = _contaRepositorio.ObterContaPorId(conta.Value);

                if (contaBusca == null)
                    RegistroNaoEncontrado();

                informacoesIniciaisVieModel.ContaId = conta.Value;
            }

            PopularEmpresas(informacoesIniciaisVieModel);
            PopularContas(informacoesIniciaisVieModel);
            PopularContatos(informacoesIniciaisVieModel);
            PopularMercadorias(informacoesIniciaisVieModel);

            return View(new OportunidadesViewModel
            {
                OportunidadesInformacoesIniciaisViewModel = informacoesIniciaisVieModel
            });
        }

        [HttpGet]
        [CanActivate(Roles = "OportunidadesIniciais:Atualizar")]
        [CanActivateVisualizarOportunidade]
        [CanActivateOportunidadeUsuarioExterno]
        [CanActivateUsuarioIntegracao]
        [CanActivateUsuarioIntegracaoAdendo]
        [CanActivateUsuarioIntegracaoFicha]
        public ActionResult Atualizar(int? id)
        {
            if (id == null)
                return RedirectToAction(nameof(Index));

            var oportunidade = _oportunidadeRepositorio.ObterOportunidadePorId(id.Value);

            Session["TipoConsultaOportunidade"] = true;

            if (oportunidade == null)
                RegistroNaoEncontrado();

            var viewModelInformacoesIniciais = new OportunidadesInformacoesIniciaisViewModel
            {
                Id = oportunidade.Id,
                Identificacao = oportunidade.Identificacao,
                ContaId = oportunidade.ContaId,
                EmpresaId = oportunidade.EmpresaId,
                Aprovada = oportunidade.Aprovada,
                ConsultaTabela = oportunidade.ConsultaTabela,
                Descricao = oportunidade.Descricao,
                DataFechamento = oportunidade.DataFechamento,
                TabelaId = oportunidade.TabelaId,
                ContatoId = oportunidade.ContatoId,
                Probabilidade = oportunidade.Probabilidade,
                SucessoNegociacao = oportunidade.SucessoNegociacao,
                ClassificacaoCliente = oportunidade.ClassificacaoCliente,
                Segmento = oportunidade.Segmento,
                Cancelado = oportunidade.Cancelado,
                EstagioNegociacao = oportunidade.EstagioNegociacao,
                StatusOportunidade = oportunidade.StatusOportunidade,
                MotivoPerda = oportunidade.MotivoPerda,
                TipoDeProposta = oportunidade.TipoDeProposta,
                TipoServico = oportunidade.TipoServico,
                TipoNegocio = oportunidade.TipoNegocio,
                TipoOperacaoOportunidade = oportunidade.TipoOperacaoOportunidade,
                RevisaoId = oportunidade.RevisaoId,
                MercadoriaId = oportunidade.MercadoriaId,
                Observacao = oportunidade.Observacao,
                FaturamentoMensalLCL = oportunidade.FaturamentoMensalLCL,
                FaturamentoMensalFCL = oportunidade.FaturamentoMensalFCL,
                VolumeMensal = oportunidade.VolumeMensal,
                CIFMedio = oportunidade.CIFMedio,
                PremioParceria = oportunidade.PremioParceria,
                CriadoPor = oportunidade.CriadoPor,
                OrigemClone = oportunidade.OrigemClone,
                DataCancelamentoOportunidade = oportunidade.DataCancelamento,
                PermiteAlterarDataCancelamento = oportunidade.PermiteAlterarDataCancelamento
            };

            var usuario = _usuarioRepositorio.ObterUsuarioPorId(oportunidade.CriadoPor);

            if (usuario != null)
            {
                ViewBag.UsuarioCriacao = usuario.Login;
                ViewBag.DataCriacao = oportunidade.DataCriacao.DataHoraFormatada();
            }

            if (viewModelInformacoesIniciais.RevisaoId.HasValue)
            {
                var revisaoProposta = _oportunidadeRepositorio
                    .ObterOportunidadePorId(viewModelInformacoesIniciais.RevisaoId.Value);

                viewModelInformacoesIniciais.RevisaoProposta = revisaoProposta.Identificacao;
            }

            if (viewModelInformacoesIniciais.DataFechamento == null)
            {
                viewModelInformacoesIniciais.DataFechamento = oportunidade.DataCriacao.AddDays(15);
            }

            var tabelaCanceladaChronos = _oportunidadeRepositorio.TabelaCanceladaChronos(oportunidade.TabelaId ?? 0);

            PopularEmpresas(viewModelInformacoesIniciais);
            PopularContas(viewModelInformacoesIniciais);
            PopularRevisao(viewModelInformacoesIniciais);
            PopularMercadorias(viewModelInformacoesIniciais);
            PopularContatos(viewModelInformacoesIniciais);
            PopularSubClientes(viewModelInformacoesIniciais);
            PopularClientesGrupoCNPJ(viewModelInformacoesIniciais);

            var viewModelProposta = new OportunidadesPropostaViewModel
            {
                OportunidadeId = oportunidade.Id,
                StatusOportunidade = oportunidade.StatusOportunidade,
                OportunidadeCancelada = oportunidade.Cancelado,
                TipoOperacao = oportunidade.OportunidadeProposta.TipoOperacao,
                ModeloId = oportunidade.OportunidadeProposta.ModeloId,
                FormaPagamento = oportunidade.OportunidadeProposta.FormaPagamento,
                DiasFreeTime = oportunidade.OportunidadeProposta.DiasFreeTime,
                QtdeDias = oportunidade.OportunidadeProposta.QtdeDias,
                Validade = oportunidade.OportunidadeProposta.Validade,
                TipoValidade = oportunidade.OportunidadeProposta.TipoValidade,
                ImpostoId = oportunidade.OportunidadeProposta.ImpostoId,
                DataInicio = oportunidade.OportunidadeProposta.DataInicio,
                DataTermino = oportunidade.OportunidadeProposta.DataTermino,
                Acordo = oportunidade.OportunidadeProposta.Acordo,
                ParametroBL = oportunidade.OportunidadeProposta.ParametroBL,
                ParametroLote = oportunidade.OportunidadeProposta.ParametroLote,
                ParametroConteiner = oportunidade.OportunidadeProposta.ParametroConteiner,
                ParametroIdTabela = oportunidade.OportunidadeProposta.ParametroIdTabela,
                BL = oportunidade.OportunidadeProposta.BL,
                Lote = oportunidade.OportunidadeProposta.Lote,
                Conteiner = oportunidade.OportunidadeProposta.Conteiner,
                CobrancaEspecial = oportunidade.OportunidadeProposta.CobrancaEspecial,
                FatorCP = oportunidade.OportunidadeProposta.FatorCP,
                PosicIsento = oportunidade.OportunidadeProposta.PosicIsento,
                DesovaParcial = oportunidade.OportunidadeProposta.DesovaParcial,
                HubPort = oportunidade.OportunidadeProposta.HubPort,
                OrigemClone = oportunidade.OrigemClone,
                TabelaReferencia = oportunidade.OportunidadeProposta.TabelaReferencia,
                TabelaCanceladaChronos = tabelaCanceladaChronos
            };

            PopularImpostos(viewModelProposta);
            PopularVendedores(viewModelProposta);

            if (viewModelProposta.VendedorId == 0)
            {
                viewModelProposta.VendedorId = oportunidade.VendedorId;
            }

            var modelo = _modeloRepositorio
                .ObterModeloPorId(oportunidade.OportunidadeProposta.ModeloId);

            if (modelo != null)
            {
                viewModelProposta.ParametroBL = modelo.ParametroBL;
                viewModelProposta.ParametroLote = modelo.ParametroLote;
                viewModelProposta.ParametroConteiner = modelo.ParametroConteiner;
                viewModelProposta.ParametroIdTabela = modelo.ParametroIdTabela;
            }

            if (!Enum.IsDefined(typeof(TipoValidade), viewModelProposta.TipoValidade))
            {
                viewModelProposta.TipoValidade = TipoValidade.MESES;
            }

            PopularModelos(viewModelProposta);

            if (!viewModelProposta.Modelos.Contains(modelo)) // Modelo que foi inativo após vinculo na oportunidade
                viewModelProposta.Modelos.Add(modelo);

            var viewModelSimulador = new SimuladorPropostaViewModel
            {
                SimuladorPropostaOportunidadeId = oportunidade.Id
            };

            viewModelSimulador.SimuladorPropostaTiposDocumentos = _documentoRepositorio
                   .ObterTiposDocumentos().ToList();

            viewModelSimulador.SimuladorPropostaMargens = _margemRepositorio
                .ObterMargens().ToList();

            viewModelSimulador.SimuladorPropostaGruposAtracacao = _grupoAtracacaoRepositorio
                .ObterGruposAtracacao().ToList();

            viewModelSimulador.SimuladorPropostaModelos = _modeloRepositorio
                .ObterModelosSimuladorVinculados(viewModelProposta.ModeloId)
                .ToList();

            viewModelSimulador.SimuladorPropostaCif = oportunidade.CIFMedio;

            viewModelSimulador.SimuladoresCadastrados = _simuladorPropostaRepositorio
                .ObterParametrosSimulador(oportunidade.Id)
                .ToList();

            var viewModelFichaFaturamento = new OportunidadesFichaFaturamentoViewModel
            {
                OportunidadeId = oportunidade.Id,
                StatusFichaFaturamento = StatusFichaFaturamento.EM_ANDAMENTO,
                StatusOportunidade = oportunidade.StatusOportunidade
            };

            PopularCondicoesPagamento(viewModelFichaFaturamento);
            PopularClientesDaProposta(viewModelFichaFaturamento);
            PopularFichasFaturamento(viewModelFichaFaturamento);

            var viewModelAnexos = new OportunidadesAnexosViewModel
            {
                AnexoOportunidadeId = oportunidade.Id,
            };

            PopularAnexos(viewModelAnexos);

            var viewModelNotas = new OportunidadesNotasViewModel
            {
                NotaOportunidadeId = oportunidade.Id,
            };

            PopularNotas(viewModelNotas);

            var viewModelAnexosNotas = new OportunidadesAnexosNotasViewModel
            {
                OportunidadesAnexosViewModel = viewModelAnexos,
                OportunidadesNotasViewModel = viewModelNotas
            };

            ViewBag.AnexosNotasProposta = oportunidade.Identificacao;

            var viewPremioParceria = new OportunidadesPremioParceriaViewModel
            {
                PremioParceriaOportunidadeId = oportunidade.Id,
                PremioParceriaContaId = oportunidade.ContaId
            };

            PopularPremiosParceria(viewPremioParceria);

            var viewModelAdendos = new OportunidadesAdendosViewModel
            {
                AdendoOportunidadeId = oportunidade.Id
            };

            PopularVendedoresAdendo(viewModelAdendos);
            PopularClientesDaProposta(viewModelAdendos);
            PopularClientesGrupoCNPJ(viewModelAdendos);
            PopularAdendos(viewModelAdendos);

            var oportunidadeViewModel = new OportunidadesViewModel
            {
                Id = oportunidade.Id,
                OportunidadesInformacoesIniciaisViewModel = viewModelInformacoesIniciais,
                OportunidadesViewModelProposta = viewModelProposta,
                OportunidadesSimuladorViewModel = viewModelSimulador,
                OportunidadesFichaFaturamentoViewModel = viewModelFichaFaturamento,
                OportunidadesAnexosNotasViewModel = viewModelAnexosNotas,
                OportunidadesPremioParceriaViewModel = viewPremioParceria,
                OportunidadesAdendosViewModel = viewModelAdendos
            };

            viewModelProposta.HabilitaAbaFichas = ExisteAdendoFaturadoEmAberto(oportunidade.Id);

            return View(oportunidadeViewModel);
        }

        private bool ExistemImpostosExcecao(int oportunidadeId)
        {
            return _oportunidadeRepositorio.ExistemImpostosExcecao(oportunidadeId);
        }

        [HttpGet]
        public ActionResult ObterHistoricoWorkflow(int id, int idProcesso)
        {
            var token = Autenticador.Autenticar();

            if (token == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Não foi possível se autenticar no serviço de Workflow");

            Oportunidade oportunidade = null;

            if (idProcesso == 1)
            {
                oportunidade = _oportunidadeRepositorio.ObterOportunidadePorId(id);

                idProcesso = oportunidade.Cancelado ? 9 : idProcesso;
            }
            else if (idProcesso == 2)
            {
                var fichaBusca = _oportunidadeRepositorio.ObterFichaFaturamentoPorId(id);

                if (fichaBusca != null)
                {
                    oportunidade = _oportunidadeRepositorio.ObterOportunidadePorId(fichaBusca.OportunidadeId);
                }
            }
            else if (idProcesso == 3)
            {
                var premioBusca = _premioParceriaRepositorio.ObterPremioParceriaPorId(id);

                if (premioBusca != null)
                {
                    oportunidade = _oportunidadeRepositorio.ObterOportunidadePorId(premioBusca.OportunidadeId);
                }
            }
            else if (idProcesso == 4)
            {
                var adendoBusca = _oportunidadeRepositorio.ObterAdendoPorId(id);

                if (adendoBusca != null)
                {
                    oportunidade = _oportunidadeRepositorio.ObterOportunidadePorId(adendoBusca.OportunidadeId);
                }
            }

            var workflow = new WorkflowService(token)
                .ObterHistoricoWorkflow(id, idProcesso, oportunidade.EmpresaId);

            if (workflow != null)
            {
                try
                {
                    if (idProcesso == 1)
                    {
                        if (oportunidade != null)
                        {
                            if (!oportunidade.TabelaId.HasValue)
                                ObterNumeroDeTabela(workflow, id);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.Info($"Histórico Workflow {ex.Message}");
                }

                return PartialView("_ConsultaHistoricoWorkflow", new HistoricoWorkflowViewModel
                {
                    WorkFlows = workflow
                        .list.SelectMany(c => c.workFlows).ToList()
                });
            }

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Não foi possível obter o histórico do workflow");
        }

        private void ObterNumeroDeTabela(RetornoHistoricoWorkflow workflow, int oportunidadeId)
        {
            var ultimoAprovador = workflow.list
                .SelectMany(c => c.workFlows)
                .Select(c => c.ultimo_Aprovador_Usuario_Login)
                .FirstOrDefault();

            if (ultimoAprovador == null)
                return;

            var comentario = workflow.list
                .SelectMany(c => c.workFlows)
                .Select(c => c.etapas
                        .Select(f => f.aprovacoes
                            .Where(g => g.usuario_Aprovador_Login == ultimoAprovador.ToString())
                            .FirstOrDefault())
                                .Select(d => d?.comentario)
                                .FirstOrDefault())
                            .FirstOrDefault();

            if (comentario != null)
            {
                var ocorrencias = Regex.Matches(comentario.ToString(), @"(?<=\*)[^}]*(?=\*)");

                if (ocorrencias.Count > 0)
                {
                    var capture = ocorrencias.Cast<Match>().FirstOrDefault();
                    var tabela = capture.Value;

                    _oportunidadeRepositorio.AtualizarNumeroTabela(tabela, oportunidadeId);
                }
            }
        }

        [HttpPost]
        public ActionResult CadastrarInformacoesIniciais([Bind(Include = "Identificacao, ContaId, EmpresaId, Aprovada, ConsultaTabela, Descricao, SucessoNegociacao, DataFechamento, ContatoId, EstagioNegociacao, ClassificacaoCliente, Probabilidade, Segmento, RevisaoId, TipoOperacaoOportunidade, TipoNegocio, TipoDeProposta, TipoServico, MotivoPerda, StatusOportunidade, PremioParceria, FaturamentoMensalLCL, FaturamentoMensalFCL, VolumeMensal, CIFMedio, MercadoriaId, Observacao")] OportunidadesInformacoesIniciaisViewModel viewModel)
        {
            if (viewModel.SucessoNegociacao == SucessoNegociacao.GANHO)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "A oportunidade deve ser aprovada antes de ser finalizada");

            var oportunidade = new Oportunidade(
                viewModel.Identificacao,
                viewModel.ContaId,
                viewModel.EmpresaId,
                viewModel.Aprovada,
                viewModel.ConsultaTabela,
                viewModel.Descricao,
                viewModel.DataFechamento,
                viewModel.TabelaId,
                viewModel.ContatoId,
                viewModel.Probabilidade,
                viewModel.SucessoNegociacao,
                viewModel.ClassificacaoCliente,
                viewModel.Segmento,
                viewModel.EstagioNegociacao,
                viewModel.StatusOportunidade,
                viewModel.MotivoPerda,
                viewModel.TipoDeProposta,
                viewModel.TipoServico,
                viewModel.TipoNegocio,
                viewModel.TipoOperacaoOportunidade,
                viewModel.RevisaoId,
                viewModel.MercadoriaId,
                viewModel.Observacao,
                viewModel.FaturamentoMensalLCL,
                viewModel.FaturamentoMensalFCL,
                viewModel.VolumeMensal,
                viewModel.CIFMedio,
                viewModel.PremioParceria,
                User.ObterId());

            if (!Validar(oportunidade))
                return RetornarErros();

            if (viewModel.ContaId > 0)
            {
                var contaBusca = _contaRepositorio.ObterContaPorId(viewModel.ContaId);

                if (contaBusca != null)
                {
                    oportunidade.Conta = contaBusca;
                }
            }

            oportunidade.Id = _oportunidadeRepositorio.Cadastrar(oportunidade);

            var detalhesOportunidade = _oportunidadeRepositorio.ObterDetalhesOportunidade(oportunidade.Id);

            GravarLogAuditoria(TipoLogAuditoria.INSERT, detalhesOportunidade);

            return Json(new
            {
                RedirectUrl = Url.Action(nameof(Atualizar), new { oportunidade.Id })
            });
        }

        [HttpPost]
        public ActionResult AtualizarInformacoesIniciais([Bind(Include = "Id, Identificacao, ContaId, EmpresaId, TabelaId, Aprovada, ConsultaTabela, Descricao, SucessoNegociacao, DataFechamento, ContatoId, EstagioNegociacao, ClassificacaoCliente, Probabilidade, Segmento, RevisaoId, TipoOperacaoOportunidade, TipoNegocio, TipoDeProposta, TipoServico, MotivoPerda, StatusOportunidade, PremioParceria, FaturamentoMensalLCL, FaturamentoMensalFCL, VolumeMensal, CIFMedio, MercadoriaId, Observacao")] OportunidadesInformacoesIniciaisViewModel viewModel, int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            if(viewModel.TipoNegocio == TipoNegocio.REVISAO_AJUSTE && viewModel.RevisaoId == null) 
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Campo Revisão é obrigatório quando o tipo de Negócio for igual a REVISÃO");
            };

            var oportunidadeBusca = _oportunidadeRepositorio.ObterOportunidadePorId(id.Value);

            if (oportunidadeBusca == null)
                RegistroNaoEncontrado();

            if (!User.IsInRole("OportunidadesIniciais:Atualizar"))
            {
                if (!_equipesService.ValidarEquipeOportunidade(oportunidadeBusca.Id))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Usuário não possui permissão para edição da Oportunidade (Equipes)");
                }
            }

            if (!User.IsInRole("OportunidadeFullControll"))
            {
                if (viewModel.SucessoNegociacao == SucessoNegociacao.GANHO && oportunidadeBusca.StatusOportunidade != StatusOportunidade.ATIVA)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "A oportunidade deve ser aprovada antes de ser finalizada");
            }

            var oportunidade = new Oportunidade(
                viewModel.Identificacao,
                viewModel.ContaId,
                viewModel.EmpresaId,
                viewModel.Aprovada,
                viewModel.ConsultaTabela,
                viewModel.Descricao,
                viewModel.DataFechamento,
                viewModel.TabelaId,
                viewModel.ContatoId,
                viewModel.Probabilidade,
                viewModel.SucessoNegociacao,
                viewModel.ClassificacaoCliente,
                viewModel.Segmento,
                viewModel.EstagioNegociacao,
                viewModel.StatusOportunidade,
                viewModel.MotivoPerda,
                viewModel.TipoDeProposta,
                viewModel.TipoServico,
                viewModel.TipoNegocio,
                viewModel.TipoOperacaoOportunidade,
                viewModel.RevisaoId,
                viewModel.MercadoriaId,
                viewModel.Observacao,
                viewModel.FaturamentoMensalLCL,
                viewModel.FaturamentoMensalFCL,
                viewModel.VolumeMensal,
                viewModel.CIFMedio,
                viewModel.PremioParceria,
                viewModel.CriadoPor);

            // Solicitação Jéssica - 03/12/2018 
            // Poderia fazer uma tratativa para não fazer estas validações (aba informação inicial e proposta) caso o status da oportunidade for igual Ativa/ Vencida/ Cancelada/ Revisada?

            if (oportunidadeBusca.StatusOportunidade != StatusOportunidade.ATIVA
              && oportunidadeBusca.StatusOportunidade != StatusOportunidade.VENCIDO
                  && oportunidadeBusca.StatusOportunidade != StatusOportunidade.CANCELADA
                      && oportunidadeBusca.StatusOportunidade != StatusOportunidade.REVISADA)
            {
                if (!Validar(oportunidade))
                    return RetornarErros();
            }

            if (User.IsInRole($"OportunidadesIniciais:{nameof(StatusOportunidade)}_Full"))
            {
                if (viewModel.StatusOportunidade == StatusOportunidade.CANCELADA)
                {
                    oportunidade.StatusOportunidade = StatusOportunidade.CANCELADA;
                    oportunidade.Cancelado = true;
                    oportunidade.DataCancelamento = DateTime.Now;
                }
                else
                {
                    oportunidade.StatusOportunidade = viewModel.StatusOportunidade;
                    oportunidade.Cancelado = false;
                    oportunidade.DataCancelamento = null;
                }
            }
            else
            {
                if (viewModel.StatusOportunidade == StatusOportunidade.ATIVA || oportunidadeBusca.StatusOportunidade == StatusOportunidade.CANCELADA)
                {
                    oportunidade.Cancelado = false;
                    oportunidade.DataCancelamento = null;
                }
            }

            try
            {
                oportunidadeBusca.AlterarInformacoesIniciais(oportunidade);

                _oportunidadeRepositorio.Atualizar(oportunidadeBusca);

                var detalhesOportunidade = _oportunidadeRepositorio.ObterDetalhesOportunidade(oportunidadeBusca.Id);

                GravarLogAuditoria(TipoLogAuditoria.UPDATE, detalhesOportunidade);

                return Json(new
                {
                    RedirectUrl = Url.Action(nameof(Atualizar), new { oportunidadeBusca.Id })
                });
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Falha ao atualizar a oportunidade");
            }
        }

        public ActionResult ExisteConcomitancia(int oportunidadeId)
        {
            var parametros = _parametrosRepositorio.ObterParametros();

            if (parametros.ValidaConcomitancia)
            {
                var oportunidadeBusca = _oportunidadeRepositorio.ObterOportunidadePorId(oportunidadeId);

                if (oportunidadeBusca == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Oportunidade não encontrada");

                var concomitancia = ValidarConcomitancia(oportunidadeBusca, parametros.CriarAdendoExclusaoCliente);

                if (concomitancia.Concomitante)
                {
                    return Json(new
                    {
                        Existe = true,
                        Mensagem = concomitancia.Mensagem,
                        Bloqueia = parametros.CriarAdendoExclusaoCliente == false,
                        RedirectUrl = string.Empty
                    }, JsonRequestBehavior.AllowGet);
                }
            }

            return Json(new
            {
                Existe = false
            }, JsonRequestBehavior.AllowGet);
        }

        public OportunidadeConcomitancia ValidarConcomitancia(Oportunidade oportunidade, bool criarAdendoExclusao)
        {
            string mensagemRetorno = string.Empty;
            bool bloqueiaEnvio = false;
            bool concomitanteCRM = false;
            bool concomitanteChronos = false;

            var propostasConcomitantes = _concomitanciaTabelaService.ObtemPropostasDuplicadasCRM(oportunidade);

            if (propostasConcomitantes.Any())
            {
                concomitanteCRM = true;

                mensagemRetorno += $"<br />Proposta(s) concomitante(s): ";

                foreach (var concomitante in propostasConcomitantes)
                {
                    mensagemRetorno += $"<strong>{concomitante.Identificacao}</strong> <br />";

                    concomitante.CnpjImportador = concomitante.CnpjImportador?.Replace("Z", string.Empty);
                    concomitante.CnpjDespachante = concomitante.CnpjDespachante?.Replace("Z", string.Empty);
                    concomitante.CnpjColoader = concomitante.CnpjColoader?.Replace("Z", string.Empty);
                    concomitante.CnpjCoColoader = concomitante.CnpjCoColoader?.Replace("Z", string.Empty);
                    concomitante.CnpjCoColoader2 = concomitante.CnpjCoColoader2?.Replace("Z", string.Empty);

                    var props = new[] { concomitante.CnpjImportador, concomitante.CnpjDespachante, concomitante.CnpjColoader, concomitante.CnpjCoColoader, concomitante.CnpjCoColoader2 };

                    if (props.Length > 0)
                    {
                        mensagemRetorno += $"({string.Join(", ", props.Where(s => !string.IsNullOrEmpty(s) && s != "Z").Distinct())})";
                    }

                    mensagemRetorno += $"<br />";
                }

                mensagemRetorno += $"<br />";
            }

            var propostasConcomitantesChronos = _concomitanciaTabelaService
                .ObtemTabelasDuplicadasChronos(oportunidade.Id)
                .Where(c => c.Id != oportunidade.TabelaId && c.Id != oportunidade.TabelaRevisadaId); // Desconsidera a Tabela informada na Proposta e a Tabela informada na Proposta Revisada            

            foreach (var tabelaChronosConcomitante in propostasConcomitantesChronos)
            {
                if (tabelaChronosConcomitante.Id == oportunidade.TabelaId)
                    continue;

                concomitanteChronos = true;

                var tabelaChronosBusca = _tabelasRepositorio.ObterTabelaChronosPorId(tabelaChronosConcomitante.Id);

                if (tabelaChronosBusca == null)
                    throw new Exception($"Tabela ID {tabelaChronosConcomitante.Id} não encontrada");

                if (tabelaChronosConcomitante.ImportadorId > 0)
                {
                    bloqueiaEnvio = !_tabelasRepositorio.ExisteParceiroNoGrupo(tabelaChronosConcomitante.Id, tabelaChronosConcomitante.ImportadorId, "I");

                    if (bloqueiaEnvio == false)
                    {
                        var oportunidadeAdendoBusca = _oportunidadeRepositorio.ObterOportunidadePorTabela(tabelaChronosConcomitante.Id);

                        if (oportunidadeAdendoBusca != null)
                        {
                            var contaAdendoBusca = _contaRepositorio.ObterContaPorDocumento(tabelaChronosConcomitante.CnpjImportador);

                            if (contaAdendoBusca == null)
                                throw new Exception($"Conta {tabelaChronosConcomitante.CnpjImportador} não encontrada na Base CRM");

                            if (criarAdendoExclusao)
                            {
                                if (!_oportunidadeRepositorio.ExisteAdendoSubCliente(oportunidadeAdendoBusca.Id, contaAdendoBusca.Id, AdendoAcao.EXCLUSAO))
                                {
                                    var adendoId = _oportunidadeRepositorio
                                        .IncluirAdendo(new OportunidadeAdendo
                                        {
                                            OportunidadeId = oportunidadeAdendoBusca.Id,
                                            TipoAdendo = TipoAdendo.EXCLUSAO_SUB_CLIENTE,
                                            StatusAdendo = StatusAdendo.ABERTO,
                                            CriadoPor = User.ObterId(),
                                            DataCadastro = DateTime.Now
                                        });

                                    _oportunidadeRepositorio.IncluirAdendoSubCliente(new OportunidadeAdendoSubCliente
                                    {
                                        AdendoId = adendoId,
                                        Acao = AdendoAcao.EXCLUSAO,
                                        Segmento = (int)SegmentoSubCliente.IMPORTADOR,
                                        ClienteId = contaAdendoBusca.Id
                                    });
                                }
                            }
                        }

                        continue;
                    }
                }

                if (tabelaChronosConcomitante.CoColoader2Id > 0)
                {
                    bloqueiaEnvio = !_tabelasRepositorio.ExisteParceiroNoGrupo(tabelaChronosConcomitante.Id, tabelaChronosConcomitante.CoColoader2Id, "B");

                    if (bloqueiaEnvio == false)
                    {
                        var oportunidadeAdendoBusca = _oportunidadeRepositorio.ObterOportunidadePorTabela(tabelaChronosConcomitante.Id);

                        if (oportunidadeAdendoBusca != null)
                        {
                            var contaAdendoBusca = _contaRepositorio.ObterContaPorDocumento(tabelaChronosConcomitante.CnpjCoColoader2);

                            if (contaAdendoBusca == null)
                                throw new Exception($"Conta {tabelaChronosConcomitante.CnpjCoColoader2} não encontrada na Base CRM");

                            if (criarAdendoExclusao)
                            {
                                if (!_oportunidadeRepositorio.ExisteAdendoSubCliente(oportunidadeAdendoBusca.Id, contaAdendoBusca.Id, AdendoAcao.EXCLUSAO))
                                {
                                    var adendoId = _oportunidadeRepositorio
                                        .IncluirAdendo(new OportunidadeAdendo
                                        {
                                            OportunidadeId = oportunidadeAdendoBusca.Id,
                                            TipoAdendo = TipoAdendo.EXCLUSAO_SUB_CLIENTE,
                                            StatusAdendo = StatusAdendo.ABERTO,
                                            CriadoPor = User.ObterId(),
                                            DataCadastro = DateTime.Now
                                        });

                                    _oportunidadeRepositorio.IncluirAdendoSubCliente(new OportunidadeAdendoSubCliente
                                    {
                                        AdendoId = adendoId,
                                        Acao = AdendoAcao.EXCLUSAO,
                                        Segmento = (int)SegmentoSubCliente.CO_COLOADER2,
                                        ClienteId = contaAdendoBusca.Id
                                    });
                                }
                            }
                        }

                        continue;
                    }
                }

                if (tabelaChronosConcomitante.CoColoaderId > 0)
                {
                    bloqueiaEnvio = !_tabelasRepositorio.ExisteParceiroNoGrupo(tabelaChronosConcomitante.Id, tabelaChronosConcomitante.CoColoaderId, "U");

                    if (bloqueiaEnvio == false)
                    {
                        var oportunidadeAdendoBusca = _oportunidadeRepositorio.ObterOportunidadePorTabela(tabelaChronosConcomitante.Id);

                        if (oportunidadeAdendoBusca != null)
                        {
                            var contaAdendoBusca = _contaRepositorio.ObterContaPorDocumento(tabelaChronosConcomitante.CnpjCoColoader);

                            if (contaAdendoBusca == null)
                                throw new Exception($"Conta {tabelaChronosConcomitante.CnpjCoColoader} não encontrada na Base CRM");

                            if (criarAdendoExclusao)
                            {
                                if (!_oportunidadeRepositorio.ExisteAdendoSubCliente(oportunidadeAdendoBusca.Id, contaAdendoBusca.Id, AdendoAcao.EXCLUSAO))
                                {
                                    var adendoId = _oportunidadeRepositorio
                                        .IncluirAdendo(new OportunidadeAdendo
                                        {
                                            OportunidadeId = oportunidadeAdendoBusca.Id,
                                            TipoAdendo = TipoAdendo.EXCLUSAO_SUB_CLIENTE,
                                            StatusAdendo = StatusAdendo.ABERTO,
                                            CriadoPor = User.ObterId(),
                                            DataCadastro = DateTime.Now
                                        });

                                    _oportunidadeRepositorio.IncluirAdendoSubCliente(new OportunidadeAdendoSubCliente
                                    {
                                        AdendoId = adendoId,
                                        Acao = AdendoAcao.EXCLUSAO,
                                        Segmento = (int)SegmentoSubCliente.CO_COLOADER1,
                                        ClienteId = contaAdendoBusca.Id
                                    });
                                }
                            }
                        }

                        continue;
                    }
                }

                if (tabelaChronosConcomitante.ColoaderId > 0)
                {
                    bloqueiaEnvio = !_tabelasRepositorio.ExisteParceiroNoGrupo(tabelaChronosConcomitante.Id, tabelaChronosConcomitante.ColoaderId, "S");

                    if (bloqueiaEnvio == false)
                    {
                        var oportunidadeAdendoBusca = _oportunidadeRepositorio.ObterOportunidadePorTabela(tabelaChronosConcomitante.Id);

                        if (oportunidadeAdendoBusca != null)
                        {
                            var contaAdendoBusca = _contaRepositorio.ObterContaPorDocumento(tabelaChronosConcomitante.CnpjColoader);

                            if (contaAdendoBusca == null)
                                throw new Exception($"Conta {tabelaChronosConcomitante.CnpjColoader} não encontrada na Base CRM");

                            if (criarAdendoExclusao)
                            {
                                if (!_oportunidadeRepositorio.ExisteAdendoSubCliente(oportunidadeAdendoBusca.Id, contaAdendoBusca.Id, AdendoAcao.EXCLUSAO))
                                {
                                    var adendoId = _oportunidadeRepositorio
                                        .IncluirAdendo(new OportunidadeAdendo
                                        {
                                            OportunidadeId = oportunidadeAdendoBusca.Id,
                                            TipoAdendo = TipoAdendo.EXCLUSAO_SUB_CLIENTE,
                                            StatusAdendo = StatusAdendo.ABERTO,
                                            CriadoPor = User.ObterId(),
                                            DataCadastro = DateTime.Now
                                        });

                                    _oportunidadeRepositorio.IncluirAdendoSubCliente(new OportunidadeAdendoSubCliente
                                    {
                                        AdendoId = adendoId,
                                        Acao = AdendoAcao.EXCLUSAO,
                                        Segmento = (int)SegmentoSubCliente.COLOADER,
                                        ClienteId = contaAdendoBusca.Id
                                    });
                                }
                            }
                        }

                        continue;
                    }
                }

                if (tabelaChronosConcomitante.DespachanteId > 0)
                {
                    bloqueiaEnvio = !_tabelasRepositorio.ExisteParceiroNoGrupo(tabelaChronosConcomitante.Id, tabelaChronosConcomitante.DespachanteId, "D");

                    if (bloqueiaEnvio == false)
                    {
                        var oportunidadeAdendoBusca = _oportunidadeRepositorio.ObterOportunidadePorTabela(tabelaChronosConcomitante.Id);

                        if (oportunidadeAdendoBusca != null)
                        {
                            var contaAdendoBusca = _contaRepositorio.ObterContaPorDocumento(tabelaChronosConcomitante.CnpjDespachante);

                            if (contaAdendoBusca == null)
                                throw new Exception($"Conta {tabelaChronosConcomitante.CnpjDespachante} não encontrada na Base CRM");

                            if (criarAdendoExclusao)
                            {
                                if (!_oportunidadeRepositorio.ExisteAdendoSubCliente(oportunidadeAdendoBusca.Id, contaAdendoBusca.Id, AdendoAcao.EXCLUSAO))
                                {
                                    var adendoId = _oportunidadeRepositorio
                                        .IncluirAdendo(new OportunidadeAdendo
                                        {
                                            OportunidadeId = oportunidadeAdendoBusca.Id,
                                            TipoAdendo = TipoAdendo.EXCLUSAO_SUB_CLIENTE,
                                            StatusAdendo = StatusAdendo.ABERTO,
                                            CriadoPor = User.ObterId(),
                                            DataCadastro = DateTime.Now
                                        });

                                    _oportunidadeRepositorio.IncluirAdendoSubCliente(new OportunidadeAdendoSubCliente
                                    {
                                        AdendoId = adendoId,
                                        Acao = AdendoAcao.EXCLUSAO,
                                        Segmento = (int)SegmentoSubCliente.DESPACHANTE,
                                        ClienteId = contaAdendoBusca.Id
                                    });
                                }
                            }
                        }

                        continue;
                    }
                }
            }

            if (concomitanteChronos)
            {
                mensagemRetorno += $"<br />Tabela(s) Concomitantes: ";

                foreach (var concomitante in propostasConcomitantesChronos)
                {
                    mensagemRetorno += $"<strong>{concomitante.Id}</strong> <br />";

                    concomitante.CnpjImportador = concomitante.CnpjImportador?.Replace("Z", string.Empty);
                    concomitante.CnpjDespachante = concomitante.CnpjDespachante?.Replace("Z", string.Empty);
                    concomitante.CnpjColoader = concomitante.CnpjColoader?.Replace("Z", string.Empty);
                    concomitante.CnpjCoColoader = concomitante.CnpjCoColoader?.Replace("Z", string.Empty);
                    concomitante.CnpjCoColoader2 = concomitante.CnpjCoColoader2?.Replace("Z", string.Empty);

                    var props = new[] { concomitante.CnpjImportador, concomitante.CnpjDespachante, concomitante.CnpjColoader, concomitante.CnpjCoColoader, concomitante.CnpjCoColoader2 };

                    if (props.Length > 0)
                    {
                        mensagemRetorno += $"({string.Join(", ", props.Where(s => !string.IsNullOrEmpty(s) && s != "Z").Distinct())})";
                    }

                    mensagemRetorno += $"<br />";
                }

                mensagemRetorno += $"<br />";
            }

            return new OportunidadeConcomitancia
            {
                Concomitante = concomitanteCRM || concomitanteChronos,
                Mensagem = mensagemRetorno,
                Bloqueia = true,
                RedirectUrl = string.Empty
            };
        }

        [HttpGet]
        public ActionResult ValidarOportunidadeConcomitante(int oportunidadeId)
        {
            bool concomitante = false;
            bool bloqueia = false;
            string mensagem = string.Empty;

            var parametros = _parametrosRepositorio.ObterParametros();

            if (parametros.ValidaConcomitancia)
            {
                var oportunidadeBusca = _oportunidadeRepositorio.ObterOportunidadePorId(oportunidadeId);

                if (oportunidadeBusca == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Oportunidade não encontrada");

                var concomitancia = ValidarConcomitancia(oportunidadeBusca, parametros.CriarAdendoExclusaoCliente);

                if (concomitancia.Concomitante)
                {
                    concomitante = true;
                    mensagem = concomitancia.Mensagem;
                    bloqueia = parametros.CriarAdendoExclusaoCliente == false;
                }
            }

            return Json(new
            {
                Existe = concomitante,
                Mensagem = mensagem,
                Bloqueia = bloqueia,
                RedirectUrl = string.Empty
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult EnviarOportunidadeParaAprovacao(int id)
        {
            try
            {
                var parametros = _parametrosRepositorio.ObterParametros();

                var oportunidadeBusca = _oportunidadeRepositorio.ObterOportunidadePorId(id);

                if (oportunidadeBusca == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Oportunidade não encontrada");

                var modeloBusca = _modeloRepositorio.ObterModeloPorId(oportunidadeBusca.OportunidadeProposta.ModeloId);

                if (modeloBusca == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Não existe Modelo vinculado na Proposta");

                if (oportunidadeBusca.OportunidadeProposta.FormaPagamento == FormaPagamento.FATURADO && oportunidadeBusca.StatusOportunidade != StatusOportunidade.CANCELADA)
                {
                    var fichasFaturamento = _oportunidadeRepositorio.ObterFichasFaturamento(oportunidadeBusca.Id);

                    if (!fichasFaturamento.Any())
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Ficha de Faturamento não cadastrada");

                    var existeFichaGeral = fichasFaturamento.Any(c => c.ContaId == oportunidadeBusca.ContaId);


                    if (existeFichaGeral == false)
                    {
                        var subClientes = _oportunidadeRepositorio
                            .ObterSubClientes(oportunidadeBusca.Id);

                        var clienteSemFicha = false;

                        foreach (var subCliente in subClientes)
                        {
                            if (subCliente.Segmento == SegmentoSubCliente.IMPORTADOR)
                            {
                                var clienteTemFicha = fichasFaturamento.Where(c => c.ContaId == subCliente.ContaId).Any();

                                if (clienteTemFicha == false)
                                    clienteSemFicha = true;
                            }
                        }

                        if (clienteSemFicha)
                            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"Existem clientes sem ficha de faturamento");
                    }
                }

                if (!User.IsInRole("OportunidadesIniciais:EnviarOportunidadeParaAprovacao"))
                {
                    if (!_equipesService.ValidarEquipeOportunidade(oportunidadeBusca.Id))
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Usuário não possui permissão para edição da Oportunidade (Equipes)");
                    }
                }



                if (Enum.IsDefined(typeof(StatusOportunidade), oportunidadeBusca.StatusOportunidade) && oportunidadeBusca.StatusOportunidade != StatusOportunidade.RECUSADO && oportunidadeBusca.StatusOportunidade != StatusOportunidade.CANCELADA)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"Oportunidades com Status {oportunidadeBusca.StatusOportunidade.ToName()} não podem ser enviadas para aprovação");

                if (string.IsNullOrEmpty(oportunidadeBusca.SallesId))
                {
                    var proposta = oportunidadeBusca.OportunidadeProposta;
                    proposta.OportunidadeId = oportunidadeBusca.Id;

                    proposta.Validar();

                    if (proposta.Invalido)
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"A oportunidade não possui nenhuma proposta válida");

                    if (proposta.FormaPagamento == FormaPagamento.FATURADO && oportunidadeBusca.StatusOportunidade != StatusOportunidade.CANCELADA)
                    {
                        var fichas = _oportunidadeRepositorio.ObterFichasFaturamento(oportunidadeBusca.Id);

                        if (!fichas.Any())
                            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"A oportunidade não possui ficha(s) de faturamento cadastrada(s)");

                        var estrangeiro = _analiseCreditoRepositorio.VerificarSeEstrangeiro(oportunidadeBusca.ContaId);
                        if (estrangeiro == 0)
                        {
                            if (_oportunidadeRepositorio.ObterStatusCondPgto(oportunidadeBusca.Id) != 3)
                            {
                                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"Condição de Pagamento sem aprovação ");

                            }
                        }
                    }
                }

                try
                {
                    ValidarRegrasRevisao(oportunidadeBusca);
                }
                catch (Exception ex)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ex.Message);
                }

                var aprovacoes = _workflowRepositorio.ObterAprovacoesPorOportunidade(oportunidadeBusca.Id, Processo.OPORTUNIDADE);

                if (aprovacoes.Any() && oportunidadeBusca.StatusOportunidade == StatusOportunidade.ENVIADO_PARA_APROVACAO)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Já existe uma aprovação pendente para esta Oportunidade");

                if (oportunidadeBusca.Cancelado && oportunidadeBusca.StatusOportunidade != StatusOportunidade.CANCELADA)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Já existe uma aprovação de cancelamento pendente para esta Oportunidade");

                var oportunidadeDetalhes = _oportunidadeRepositorio.ObterDetalhesOportunidade(id);

                var anexosOportunidade = _anexoRepositorio
                    .ObterAnexosPorOportunidade(oportunidadeBusca.Id).Select(c => c.IdFile);

                var campos = new
                {
                    oportunidadeId = oportunidadeDetalhes.Id,
                    oportunidadeDetalhes.Descricao,
                    oportunidadeDetalhes.Identificacao,
                    ContaId = oportunidadeDetalhes.Conta,
                    ContaCnpj = oportunidadeDetalhes.ContaDocumento,
                    ContatoId = oportunidadeDetalhes.Contato,
                    MercadoriaId = oportunidadeDetalhes.Mercadoria,
                    oportunidadeDetalhes.Aprovada,
                    oportunidadeDetalhes.DataFechamento,
                    oportunidadeDetalhes.TabelaId,
                    oportunidadeDetalhes.Probabilidade,
                    oportunidadeDetalhes.SucessoNegociacao,
                    oportunidadeDetalhes.ClassificacaoCliente,
                    oportunidadeDetalhes.Segmento,
                    oportunidadeDetalhes.EstagioNegociacao,
                    oportunidadeDetalhes.StatusOportunidade,
                    oportunidadeDetalhes.MotivoPerda,
                    oportunidadeDetalhes.TipoDeProposta,
                    oportunidadeDetalhes.TipoServico,
                    oportunidadeDetalhes.TipoNegocio,
                    oportunidadeDetalhes.TipoOperacaoOportunidade,
                    oportunidadeDetalhes.RevisaoId,
                    oportunidadeDetalhes.Observacao,
                    oportunidadeDetalhes.FaturamentoMensalLCL,
                    oportunidadeDetalhes.FaturamentoMensalFCL,
                    oportunidadeDetalhes.VolumeMensal,
                    oportunidadeDetalhes.CIFMedio,
                    oportunidadeDetalhes.PremioParceria,
                    oportunidadeDetalhes.CriadoPor,
                    oportunidadeDetalhes.TipoOperacao,
                    oportunidadeDetalhes.Vendedor,
                    oportunidadeDetalhes.FormaPagamento,
                    oportunidadeDetalhes.Modelo,
                    DataInicio = oportunidadeDetalhes.DataInicio.DataFormatada(),
                    DataTermino = oportunidadeDetalhes.DataTermino.DataFormatada(),
                    IdFile = string.Join(",", anexosOportunidade)
                };

                Processo processo = oportunidadeBusca.StatusOportunidade != StatusOportunidade.CANCELADA
                    ? Processo.OPORTUNIDADE
                    : Processo.CANCELAMENTO_OPORTUNIDADE;

                var token = Autenticador.Autenticar();

                if (token == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Não foi possível se autenticar no serviço de Workflow");

                var workflow = new WorkflowService(token);

                var retornoWorkflow = workflow.EnviarParaAprovacao(
                    new CadastroWorkflow(
                        processo,
                        oportunidadeBusca.EmpresaId,
                        oportunidadeBusca.Id,
                        User.ObterLogin(),
                        User.ObterNome(),
                        User.ObterEmail(),
                        JsonConvert.SerializeObject(campos)));

                if (retornoWorkflow == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Nenhuma resposta do serviço de Workflow");

                if (retornoWorkflow.sucesso == false)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, retornoWorkflow.mensagem);

                var workFlowId = _workflowRepositorio.IncluirEnvioAprovacao(
                    new EnvioWorkflow(
                        oportunidadeBusca.Id,
                        Processo.OPORTUNIDADE,
                        retornoWorkflow.protocolo,
                        retornoWorkflow.mensagem,
                        User.ObterId()));

                if (oportunidadeBusca.StatusOportunidade != StatusOportunidade.CANCELADA)
                {
                    _oportunidadeRepositorio.AtualizarStatusOportunidade(StatusOportunidade.ENVIADO_PARA_APROVACAO, oportunidadeBusca.Id);

                    var fichasFaturamento = _oportunidadeRepositorio.ObterFichasFaturamento(oportunidadeBusca.Id);

                    foreach (var ficha in fichasFaturamento)
                    {
                        if (ficha.StatusFichaFaturamento == StatusFichaFaturamento.EM_ANDAMENTO || ficha.StatusFichaFaturamento == StatusFichaFaturamento.REJEITADO)
                        {
                            _oportunidadeRepositorio.AtualizarStatusFichaFaturamento(StatusFichaFaturamento.EM_APROVACAO, ficha.Id);
                        }
                    }
                }

                if (oportunidadeBusca.StatusOportunidade == StatusOportunidade.CANCELADA)
                {
                    _oportunidadeRepositorio.PermiteAlterarDataCancelamento(oportunidadeBusca.Id, false);
                }

                if (parametros.GerarPdfProposta)
                {
                    if ((!Enum.IsDefined(typeof(StatusOportunidade), oportunidadeBusca.StatusOportunidade)) || oportunidadeBusca.StatusOportunidade == StatusOportunidade.ENVIADO_PARA_APROVACAO || oportunidadeBusca.StatusOportunidade == StatusOportunidade.RECUSADO)
                    {
                        if (oportunidadeBusca.OportunidadeProposta?.TipoOperacao == TipoOperacao.RA)
                        {
                            string url = Url.Action(nameof(PropostaOnline), "Oportunidades", new RouteValueDictionary(new { oportunidadeBusca.Id }), "http");

                            new PropostaPdfHelper(_anexoRepositorio, oportunidadeBusca, true, url)
                                .GerarPropostaPdf();
                        }
                    }
                }

                if (modeloBusca.Simular && oportunidadeBusca.StatusOportunidade != StatusOportunidade.CANCELADA)
                {
                    var parametrosSimulador = _simuladorPropostaRepositorio
                        .ObterParametrosSimulador(oportunidadeBusca.Id)
                        .FirstOrDefault();

                    if (parametrosSimulador == null)
                    {
                        parametrosSimulador = new OportunidadeParametrosSimuladorDTO();

                        var modelosSimulador = _modeloRepositorio
                            .ObterModelosSimuladorVinculados(oportunidadeBusca.OportunidadeProposta.ModeloId)
                            .ToList();

                        if (modelosSimulador.Any())
                        {
                            var modeloSimulador = modelosSimulador.First();

                            var parametro = new SimuladorPropostaParametros(
                                oportunidadeBusca.Id,
                                modeloSimulador.ModeloSimuladorId,
                                oportunidadeBusca.TipoServico.ToName(),
                                null,
                                0,
                                5,
                                5,
                                1,
                                null,
                                0,
                                0,
                                string.Empty,
                                oportunidadeBusca.CIFMedio,
                                User.ObterId());

                            parametrosSimulador.Id = _simuladorPropostaRepositorio.CadastrarParametrosSimulador(parametro);
                            parametrosSimulador.ModeloId = modeloSimulador.ModeloSimuladorId;
                        }
                    }

                    if (parametrosSimulador.Id > 0)
                    {
                        using (var wsSimulador = new WsSimulador.SimuladorCalculo())
                        {
                            wsSimulador.Timeout = 900000;

                            wsSimulador.SimuladorOportunidade(oportunidadeBusca.Id, parametrosSimulador.Id, parametrosSimulador.ModeloId, User.ObterId());
                        }
                    }
                }

                return Json(new
                {
                    Existe = false,
                    Mensagem = string.Empty,
                    Bloqueia = false,
                    RedirectUrl = Url.Action(nameof(Atualizar), new { oportunidadeBusca.Id })
                });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult GerarPropostaPdfExterno(int id, string hash)
        {
            bool sucesso = true;
            string mensagem = string.Empty;

            if (string.IsNullOrEmpty(hash))
            {
                mensagem = "Hash não informado";

                sucesso = false;
            }

            if (hash != "da39a3ee5e6b4b0d3255bfef95601890afd80709")
            {
                mensagem = "Hash inválido";

                sucesso = false;
            }

            if (sucesso)
            {
                try
                {
                    var oportunidadeBusca = _oportunidadeRepositorio.ObterOportunidadePorId(id);

                    //   if ((!Enum.IsDefined(typeof(StatusOportunidade), oportunidadeBusca.StatusOportunidade)) || oportunidadeBusca.StatusOportunidade == StatusOportunidade.ENVIADO_PARA_APROVACAO || oportunidadeBusca.StatusOportunidade == StatusOportunidade.RECUSADO)
                    //   {
                    if (oportunidadeBusca.OportunidadeProposta?.TipoOperacao == TipoOperacao.RA)
                    {
                        string url = Url.Action(nameof(PropostaOnline), "Oportunidades", new RouteValueDictionary(new { oportunidadeBusca.Id }), "http");

                        new PropostaPdfHelper(_anexoRepositorio, oportunidadeBusca, true, url, true)
                            .GerarPropostaPdf();

                        mensagem = "PDF gerado com sucesso!";
                    }
                    else
                    {
                        sucesso = false;
                        mensagem = $"O Tipo de Operação não é RA";
                    }
                    //  }
                    //   else
                    //   {
                    //    sucesso = false;
                    //   mensagem = $"O Status {oportunidadeBusca.StatusOportunidade.ToName()} não permite geração de PDF";
                    // }
                }
                catch (Exception ex)
                {
                    sucesso = false;
                    mensagem = ex.Message;
                }
            }

            return Json(new
            {
                sucesso,
                mensagem
            });
        }

        [HttpPost]
        public ActionResult AtualizarInformacoesProposta([Bind(Include = "OportunidadeId, ModeloId, TipoOperacao, FormaPagamento, DiasFreeTime, VendedorId, QtdeDias, Validade, TipoValidade, ImpostoId, DataInicio, DataTermino, Acordo, ParametroLote, ParametroBL, ParametroConteiner, ParametroIdTabela, Lote, BL, Conteiner, HubPort, CobrancaEspecial, DesovaParcial, FatorCP, PosicIsento, TabelaReferencia")] OportunidadesPropostaViewModel viewModel)
        {
            var id = viewModel.OportunidadeId;

            if (!id.HasValue)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var oportunidadeBusca = _oportunidadeRepositorio.ObterOportunidadePorId(id.Value);

            if (oportunidadeBusca == null)
                RegistroNaoEncontrado();

            if (!User.IsInRole("OportunidadesProposta:Atualizar"))
            {
                if (!_equipesService.ValidarEquipeOportunidade(oportunidadeBusca.Id))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Usuário não possui permissão para edição da Oportunidade (Equipes)");
                }
            }

            var impostoOriginal = oportunidadeBusca.OportunidadeProposta.ImpostoId;

            var proposta = new OportunidadeProposta
            {
                OportunidadeId = oportunidadeBusca.Id,
                ModeloId = viewModel.ModeloId,
                FormaPagamento = viewModel.FormaPagamento,
                DiasFreeTime = viewModel.DiasFreeTime,
                VendedorId = viewModel.VendedorId,
                QtdeDias = viewModel.QtdeDias,
                Validade = viewModel.Validade,
                TipoValidade = viewModel.TipoValidade,
                ImpostoId = viewModel.ImpostoId,
                TipoOperacao = viewModel.TipoOperacao,
                DataInicio = viewModel.DataInicio,
                DataTermino = viewModel.DataTermino,
                Acordo = viewModel.Acordo,
                ParametroBL = viewModel.ParametroBL,
                ParametroLote = viewModel.ParametroLote,
                ParametroConteiner = viewModel.ParametroConteiner,
                ParametroIdTabela = viewModel.ParametroIdTabela,
                BL = viewModel.ParametroBL ? viewModel.BL : string.Empty,
                Lote = viewModel.ParametroLote ? viewModel.Lote : string.Empty,
                Conteiner = viewModel.ParametroConteiner ? viewModel.Conteiner : string.Empty,
                HubPort = viewModel.HubPort,
                CobrancaEspecial = viewModel.CobrancaEspecial,
                DesovaParcial = viewModel.DesovaParcial,
                FatorCP = viewModel.FatorCP,
                PosicIsento = viewModel.PosicIsento,
                TabelaReferencia = viewModel.TabelaReferencia
            };

            if (!Validar(proposta))
                return RetornarErros();

            var modeloBusca = _modeloRepositorio
                .ObterModeloPorId(viewModel.ModeloId);

            if (modeloBusca != null)
            {
                if (modeloBusca.Acordo != viewModel.Acordo)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "O Modelo da Oportunidade exige o tipo Acordo");

                if (modeloBusca.ParametroLote && string.IsNullOrEmpty(viewModel.Lote))
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "O Modelo da Oportunidade exige que seja informado um ou mais Lotes");

                if (modeloBusca.ParametroBL && string.IsNullOrEmpty(viewModel.BL))
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "O Modelo da Oportunidade exige que seja informado um ou mais BLs");

                if (modeloBusca.ParametroConteiner && string.IsNullOrEmpty(viewModel.Conteiner))
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "O Modelo da Oportunidade exige que seja informado um ou mais Contêineres");

                if (modeloBusca.ParametroIdTabela && !viewModel.TabelaReferencia.HasValue)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "O Modelo da Oportunidade exige que seja informado o ID da Tabela");
            }

            if (viewModel.ImpostoId == 3)
            {
                if (!ExistemImpostosExcecao(oportunidadeBusca.Id))
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Não existem exceções de impostos cadastradas para esta oportunidade.");
            }

            if (viewModel.DataInicio.HasValue && viewModel.DataTermino.HasValue)
            {
                if (viewModel.TipoValidade == TipoValidade.ANOS)
                {
                    TimeSpan span = viewModel.DataTermino.Value.AddDays(1) - viewModel.DataInicio.Value;

                    //// Ano Bisexto
                    //if (DateTime.IsLeapYear(viewModel.DataInicio.Value.Year) || DateTime.IsLeapYear(viewModel.DataTermino.Value.Year))
                    //{
                    //    span.Subtract(TimeSpan.FromDays(1));
                    //}

                    if (viewModel.Validade != span.Days / 365)
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"O intervalo de Datas não pode ser inferior ou superior a {viewModel.Validade} ano(s)");
                }
            }

            if (oportunidadeBusca.OportunidadeProposta.ModeloId != viewModel.ModeloId)
                ImportarModeloLayout(oportunidadeBusca.Id, proposta.ModeloId);

            TipoValidade tipoValidade = proposta.TipoValidade;

            proposta.DataInicio = viewModel.DataInicio == null
                ? DateTime.Now
                : viewModel.DataInicio;

            if (viewModel.DataTermino == null)
            {
                proposta.DataTermino = AtualizarDataTermino(oportunidadeBusca, proposta.DataInicio.Value, tipoValidade, proposta.Validade);
            }

            oportunidadeBusca.OportunidadeProposta = proposta;

            if (viewModel.ImpostoId != 3)
            {
                _impostosExcecaoRepositorio.ExcluirTodosDaOportunidade(oportunidadeBusca.Id);
            }

            _oportunidadeRepositorio.AtualizarProposta(proposta);

            var detalhesProposta = _oportunidadeRepositorio.ObterDetalhesProposta(oportunidadeBusca.Id);

            GravarLogAuditoria(TipoLogAuditoria.UPDATE, detalhesProposta, oportunidadeBusca.Id);

            return Json(new
            {
                hash = nameof(proposta),
                redirectUrl = Url.Action(nameof(Atualizar), new { oportunidadeBusca.Id })
            });
        }

        public List<string> PermiteInclusaoFicha(Oportunidade oportunidade)
        {
            List<string> mensagens = new List<string>();

            if (oportunidade?.OportunidadeProposta == null)
                mensagens.Add("A oportunidade não possui nenhum Modelo inculado");

            if (oportunidade.OportunidadeProposta.FormaPagamento == FormaPagamento.AVISTA)
            {
                var adendosOportunidade = _oportunidadeRepositorio.ObterAdendos(oportunidade.Id);

                var ultimoAdendo = adendosOportunidade
                    .OrderByDescending(c => c.Id)
                    .FirstOrDefault(c => c.TipoAdendo == TipoAdendo.FORMA_PAGAMENTO);

                if (ultimoAdendo != null)
                {
                    var formaPagamentoAdendo = _oportunidadeRepositorio.ObterAdendoFormaPagamento(ultimoAdendo.Id);

                    if (formaPagamentoAdendo != null)
                    {
                        if (formaPagamentoAdendo.FormaPagamento != FormaPagamento.FATURADO)
                            mensagens.Add("Não foi encontrado nenhum adendo de Forma Pagamento Faturado");
                    }
                }
                else
                {
                    mensagens.Add("Inclusão de Fichas de Faturamento não permitida");
                }
            }

            return mensagens;
        }

        private bool BloqueiaAlteracaoFichas(StatusOportunidade statusOportunidade)
        {
            return (statusOportunidade == StatusOportunidade.ENVIADO_PARA_APROVACAO || statusOportunidade == StatusOportunidade.CANCELADA || statusOportunidade == StatusOportunidade.VENCIDO);
        }

        private bool BloqueiaAlteracaoFichas(StatusFichaFaturamento statusFichaFaturamento)
        {
            return (statusFichaFaturamento == StatusFichaFaturamento.APROVADO || statusFichaFaturamento == StatusFichaFaturamento.REVISADA || statusFichaFaturamento == StatusFichaFaturamento.EM_APROVACAO);
        }

        [HttpGet]
        public ActionResult PermiteAlterarCamposFicha(int oportunidadeId, int fichaId)
        {
            bool permiteAlteracoes = true;
            bool permiteSalvarFichas = false;

            var oportunidadeBusca = _oportunidadeRepositorio.ObterOportunidadePorId(oportunidadeId);

            if (BloqueiaAlteracaoFichas(oportunidadeBusca.StatusOportunidade))
            {
                permiteAlteracoes = false;
            }

            var fichaFaturamentoBusca = _oportunidadeRepositorio.ObterFichaFaturamentoPorId(fichaId);

            if (BloqueiaAlteracaoFichas(fichaFaturamentoBusca.StatusFichaFaturamento))
            {
                permiteAlteracoes = false;
            }

            permiteSalvarFichas = User.IsInRole("OportunidadesFichas:Cadastrar");

            List<CampoPermissaoDTO> campos = new List<CampoPermissaoDTO>();

            campos.Add(new CampoPermissaoDTO("revisarFichaFaturamento", User.IsInRole("OportunidadesFichas:RevisarFichaFaturamento") || User.IsInRole("OportunidadesFichas:RevisarFichaFaturamento_Full")));
            campos.Add(new CampoPermissaoDTO("enviarParaAprovacao", User.IsInRole("OportunidadesFichas:EnviarParaAprovacao") || User.IsInRole("OportunidadesFichas:EnviarParaAprovacao_Full")));
            campos.Add(new CampoPermissaoDTO("clientePropostaSelecionadoId", User.IsInRole("OportunidadesFichas:ClientePropostaSelecionadoId") || User.IsInRole("OportunidadesFichas:ClientePropostaSelecionadoId_Full")));
            campos.Add(new CampoPermissaoDTO("fontePagadoraId", User.IsInRole("OportunidadesFichas:FontePagadoraId") || User.IsInRole("OportunidadesFichas:FontePagadoraId_Full")));
            campos.Add(new CampoPermissaoDTO("condicaoPagamentoPorDia", User.IsInRole("OportunidadesFichas:CondicaoPagamentoPorDia") || User.IsInRole("OportunidadesFichas:CondicaoPagamentoPorDia_Full")));
            campos.Add(new CampoPermissaoDTO("diaUtil", User.IsInRole("OportunidadesFichas:DiaUtil") || User.IsInRole("OportunidadesFichas:DiaUtil_Full")));
            campos.Add(new CampoPermissaoDTO("entregaEletronica", User.IsInRole("OportunidadesFichas:EntregaEletronica") || User.IsInRole("OportunidadesFichas:EntregaEletronica_Full")));
            campos.Add(new CampoPermissaoDTO("entregaManual", User.IsInRole("OportunidadesFichas:EntregaManual") || User.IsInRole("OportunidadesFichas:EntregaManual_Full")));
            campos.Add(new CampoPermissaoDTO("correiosComum", User.IsInRole("OportunidadesFichas:CorreiosComum") || User.IsInRole("OportunidadesFichas:CorreiosComum_Full")));
            campos.Add(new CampoPermissaoDTO("ultimoDiaDoMes", User.IsInRole("OportunidadesFichas:UltimoDiaDoMes") || User.IsInRole("OportunidadesFichas:UltimoDiaDoMes_Full")));
            campos.Add(new CampoPermissaoDTO("faturadoContraId", User.IsInRole("OportunidadesFichas:FaturadoContraId") || User.IsInRole("OportunidadesFichas:FaturadoContraId_Full")));
            campos.Add(new CampoPermissaoDTO("diasSemana", User.IsInRole("OportunidadesFichas:DiasSemana") || User.IsInRole("OportunidadesFichas:DiasSemana_Full")));
            campos.Add(new CampoPermissaoDTO("diasFaturamento", User.IsInRole("OportunidadesFichas:DiasFaturamento") || User.IsInRole("OportunidadesFichas:DiasFaturamento_Full")));
            campos.Add(new CampoPermissaoDTO("dataCorte", User.IsInRole("OportunidadesFichas:DataCorte") || User.IsInRole("OportunidadesFichas:DataCorte_Full")));
            campos.Add(new CampoPermissaoDTO("emailFaturamento", User.IsInRole("OportunidadesFichas:EmailFaturamento") || User.IsInRole("OportunidadesFichas:EmailFaturamento_Full")));
            campos.Add(new CampoPermissaoDTO("condicaoPagamentoFaturamentoId", User.IsInRole("OportunidadesFichas:CondicaoPagamentoFaturamentoId") || User.IsInRole("OportunidadesFichas:CondicaoPagamentoFaturamentoId_Full")));
            campos.Add(new CampoPermissaoDTO("observacoesFaturamento", User.IsInRole("OportunidadesFichas:ObservacoesFaturamento") || User.IsInRole("OportunidadesFichas:ObservacoesFaturamento_Full")));
            campos.Add(new CampoPermissaoDTO("anexoFaturamento", User.IsInRole("OportunidadesFichas:AnexoFaturamento") || User.IsInRole("OportunidadesFichas:AnexoFaturamento_Full")));
            campos.Add(new CampoPermissaoDTO("pesquisarClienteFontePagadora", User.IsInRole("OportunidadesFichas:PesquisarClienteFontePagadora") || User.IsInRole("OportunidadesFichas:PesquisarClienteFontePagadora_Full")));
            campos.Add(new CampoPermissaoDTO("adicionarDiasSemanaFaturamento", User.IsInRole("OportunidadesFichas:AdicionarDiasSemanaFaturamento") || User.IsInRole("OportunidadesFichas:AdicionarDiasSemanaFaturamento_Full")));
            campos.Add(new CampoPermissaoDTO("adicionarDiasFaturamento", User.IsInRole("OportunidadesFichas:AdicionarDiasFaturamento") || User.IsInRole("OportunidadesFichas:AdicionarDiasFaturamento_Full")));
            campos.Add(new CampoPermissaoDTO("novaFichaFaturamento", User.IsInRole("OportunidadesFichas:NovaFichaFaturamento") || User.IsInRole("OportunidadesFichas:NovaFichaFaturamento_Full")));
            campos.Add(new CampoPermissaoDTO("recallFichaFaturamento", User.IsInRole("OportunidadesFichas:RecallFichaFaturamento") || User.IsInRole("OportunidadesFichas:RecallFichaFaturamento_Full")));

            return Json(new
            {
                permiteAlteracoes,
                permiteSalvarFichas,
                admin = false,
                campos = campos.ToList()
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Teste(string consultaspc)
        {
            if (consultaspc != null)
            {
                var id = Session["FontePagadoraId"];

                return RedirectToAction("ConsultarSpc", "AnaliseCredito", new { id = id });
            }
            return RedirectToAction("Index", "Oportunidades");
        }

        [HttpPost]
        [CanActivateUsuarioIntegracaoFicha]
        public ActionResult CadastrarFichaFaturamento([Bind(Include = "FichaFaturamentoId, OportunidadeId, StatusFichaFaturamento, FaturadoContraId, ClientesPropostaSelecionados, ClientePropostaSelecionadoId, DiasSemana, DiasFaturamento, DataCorte, CondicaoPagamentoFaturamentoId, EmailFaturamento, ObservacoesFaturamento, ContasSelecionadas, FontePagadoraId, CondicaoPagamentoPorDia, CondicaoPagamentoPorDiaSemana, DiaUtil, UltimoDiaDoMes, EntregaManual, EntregaEletronica, EntregaManualSedex, FichaRevisaoId")] OportunidadesFichaFaturamentoViewModel viewModel, HttpPostedFileBase anexoFaturamento)
        {
            if (viewModel.OportunidadeId == 0)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var oportunidadeBusca = _oportunidadeRepositorio.ObterOportunidadePorId(viewModel.OportunidadeId);

            if (oportunidadeBusca == null)
                RegistroNaoEncontrado();

            if (!User.IsInRole("OportunidadesFichas:Atualizar"))
            {
                if (!_equipesService.ValidarEquipeOportunidade(oportunidadeBusca.Id))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Usuário não possui permissão para edição da Oportunidade (Equipes)");
                }
            }

            if (oportunidadeBusca.StatusOportunidade == StatusOportunidade.ENVIADO_PARA_APROVACAO || oportunidadeBusca.StatusOportunidade == StatusOportunidade.CANCELADA || oportunidadeBusca.StatusOportunidade == StatusOportunidade.VENCIDO)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "O Status da Oportunidade não permite cadastrar Fichas de Faturamento");
            }
            #region ANALISE DE CREDITO

            var adendoforma = _analiseCreditoRepositorio.buscaformaadendo(oportunidadeBusca.Id);
            bool entrada = false;
            if (oportunidadeBusca.OportunidadeProposta.FormaPagamento == FormaPagamento.FATURADO) { entrada = true; }
            if (adendoforma > 0)
            {
                entrada = true;
            }

            if (habilitaDemandaAnaliseDeCredito && (entrada))
            {
                var fontePagadoraId = viewModel.FontePagadoraId;

                var estrangeiro = _analiseCreditoRepositorio.VerificarSeEstrangeiro(fontePagadoraId);

                if (estrangeiro == 0)
                {
                    var spcBusca = _analiseCreditoRepositorio.ObterConsultaSpc(fontePagadoraId);
                    if (spcBusca == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Fonte Pagadora não possui análise de crédito ou análise pendente aprovação.");
                    }
                    else
                    {
                        if (spcBusca.Validade < DateTime.Now)
                        {
                            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Análise de Crédito vencida para a Fonte Pagadora");
                        }


                        if (spcBusca.InadimplenteEcoporto || spcBusca.InadimplenteSpc)
                        {
                            if (spcBusca.StatusAnaliseDeCredito != StatusAnaliseDeCredito.APROVADO)
                            {
                                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Fonte Pagadora conta como inadimplente. Análise de Crédito não possui aprovação.");
                            }
                        }


                        if (_oportunidadeRepositorio.ObterStatusCondPgtoNew(viewModel.FontePagadoraId, viewModel.CondicaoPagamentoFaturamentoId) != 3)
                        {
                            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"Condição de Pagamento sem aprovação ");

                        }
                        var fatFCL = oportunidadeBusca.FaturamentoMensalFCL;
                        var fatLCL = oportunidadeBusca.FaturamentoMensalLCL;

                        //var valorLiteCreditoBase = (fatFCL / (fatLCL == 0 ? 1 : fatFCL)) * 12;

                        //if (valorLiteCreditoBase > 200_000)
                        //{
                        //    var limitesDeCredito = _analiseCreditoRepositorio.ObterSolicitacoesLimiteDeCredito(fontePagadoraId);

                        //    if (limitesDeCredito.Any())
                        //    {
                        //        var limitesDeCreditoSemAprovacao = limitesDeCredito.Where(c => c.StatusLimiteCredito != StatusLimiteCredito.APROVADO).Any();

                        //        if (limitesDeCreditoSemAprovacao)
                        //        {
                        //            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "A Análise de Crédito possui limites de crédito sem aprovação");
                        //        }
                        //    }
                        //}
                    }
                }
                

                
            }
            #endregion FIM ANALISE DE CREDITO
            var ficha = new OportunidadeFichaFaturamento
            {
                OportunidadeId = oportunidadeBusca.Id,
                StatusFichaFaturamento = StatusFichaFaturamento.EM_ANDAMENTO,
                FaturadoContraId = viewModel.FaturadoContraId,
                DiasSemana = viewModel.DiasSemana,
                DiasFaturamento = viewModel.DiasFaturamento,
                DataCorte = viewModel.DataCorte,
                CondicaoPagamentoFaturamentoId = viewModel.CondicaoPagamentoFaturamentoId,
                EmailFaturamento = viewModel.EmailFaturamento,
                ObservacoesFaturamento = viewModel.ObservacoesFaturamento,
                ContasSelecionadas = viewModel.ClientePropostaSelecionadoId,
                FontePagadoraId = viewModel.FontePagadoraId,
                CondicaoPagamentoPorDia = viewModel.CondicaoPagamentoPorDia,
                CondicaoPagamentoPorDiaSemana = viewModel.CondicaoPagamentoPorDiaSemana,
                EntregaManual = viewModel.EntregaManual,
                EntregaManualSedex = viewModel.EntregaManualSedex,
                DiaUtil = viewModel.DiaUtil,
                UltimoDiaDoMes = viewModel.UltimoDiaDoMes,
                RevisaoId = viewModel.FichaRevisaoId,
                EntregaEletronica = viewModel.EntregaEletronica
            };

            var existeAnaliseDeCreditoPendente = _oportunidadeRepositorio
                .AnaliseDeCreditoPendente(viewModel.FontePagadoraId);

            var existeLimiteDeCreditoPendente = _oportunidadeRepositorio
                .LimiteDeCreditoPendente(viewModel.FontePagadoraId);

            if (existeAnaliseDeCreditoPendente > 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"Analise de Credito pendente para esta conta");
            }
            if (existeAnaliseDeCreditoPendente > 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"Limite de credito pendente para esta conta");
            }

            ficha.Id = viewModel.FichaFaturamentoId;

            if (!Validar(ficha))
                return RetornarErros();

            var fichasCadastradas = _oportunidadeRepositorio
                .ObterFichasFaturamento(oportunidadeBusca.Id);

            if (ficha.Id == 0 || viewModel.FichaRevisaoId != 0)
            {
                var permiteInclusaoFicha = PermiteInclusaoFicha(oportunidadeBusca);

                if (permiteInclusaoFicha.Any())
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, permiteInclusaoFicha.First());

                try
                {
                    if (viewModel.FichaRevisaoId == 0)
                    {
                        if (ficha.ContasSelecionadas != null)
                        {
                            foreach (var contaSelecionada in ficha.ContasSelecionadas)
                            {
                                var fichasEmAndamento = fichasCadastradas
                                    .Where(c => c.ContaId == contaSelecionada);

                                if (fichasEmAndamento.Any())
                                {
                                    var contaBusca = _contaRepositorio.ObterContaPorId(contaSelecionada);

                                    if (contaBusca != null)
                                    {
                                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"Já existe uma Ficha em andamento para o cliente {contaBusca.Descricao}");
                                    }
                                }
                            }
                        }
                        else
                        {
                            var fichasEmAndamento = fichasCadastradas
                                .Where(c => c.ContaId == oportunidadeBusca.ContaId);

                            if (fichasEmAndamento.Any())
                                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"Já existe uma Ficha cadastrada para o cliente {oportunidadeBusca.Conta.Descricao}");
                        }
                    }
                    else
                    {
                        var fichaRevisao = fichasCadastradas
                            .Where(c => c.RevisaoId == viewModel.FichaRevisaoId.ToString()
                                && ficha.ContasSelecionadas.Contains(c.ContaId)).Any();

                        if (fichaRevisao)
                            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"Já existe uma Ficha em revisão em Andamento");
                    }

                    var demandaFontePagadora = true;

                    if (demandaFontePagadora)
                    {
                        if (!string.IsNullOrEmpty(viewModel.DiasSemana) && !string.IsNullOrEmpty(viewModel.DiasFaturamento))
                        {
                            ModelState.AddModelError(string.Empty, "É permitido inserir o campo Dias Faturamento (por dia da semana) OU dia útil");

                            return RetornarErros();
                        }

                        var regras = 0;

                        if (!string.IsNullOrEmpty(viewModel.CondicaoPagamentoPorDiaSemana))
                        {
                            regras += 1;
                        }

                        if (!string.IsNullOrEmpty(viewModel.CondicaoPagamentoPorDia))
                        {
                            regras += 1;
                        }

                        if (viewModel.UltimoDiaDoMes)
                        {
                            regras += 1;
                        }

                        if (regras == 3)
                        {
                            ModelState.AddModelError(string.Empty, "É permitido inserir o campo Condição Pagamento por dia de semana ou dia vencimento ou indicativo ultimo dia do mês");

                            return RetornarErros();
                        }

                        if (viewModel.EntregaEletronica && string.IsNullOrEmpty(viewModel.EmailFaturamento))
                        {
                            ModelState.AddModelError(string.Empty, "Informe o endereço de email para entrega eletrônica");

                            return RetornarErros();
                        }
                    }

                    if (anexoFaturamento != null)
                    {
                        if (anexoFaturamento.ContentLength == 0)
                            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "É obrigatório o anexo da Ficha de Faturamento");

                        ficha.AnexoFaturamentoId = IncluirAnexo(oportunidadeBusca.Id, TipoAnexo.FICHA_FATURAMENTO, anexoFaturamento);
                    }

                    if (viewModel.FichaRevisaoId == 0 && ficha.ContasSelecionadas != null)
                    {
                        foreach (var contaSelecionada in ficha.ContasSelecionadas)
                        {
                            var fichaBusca = fichasCadastradas.Where(c => c.ContaId == contaSelecionada).FirstOrDefault();

                            if (fichaBusca != null)
                            {
                                var contaBusca = _contaRepositorio.ObterContaPorId(fichaBusca.ContaId);

                                if (contaBusca != null)
                                {
                                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"Ficha já cadastrada para o Cliente: {contaBusca.Descricao}");
                                }
                            }
                        }
                    }

                    if (ficha.ContasSelecionadas != null)
                    {
                        foreach (var contaSelecionada in ficha.ContasSelecionadas)
                        {
                            ficha.ContaId = contaSelecionada;
                            ficha.Id = _oportunidadeRepositorio.IncluirFichaFaturamento(ficha);

                            var fichaBusca = _oportunidadeRepositorio.ObterDetalhesFichaFaturamento(ficha.Id);

                            GravarLogAuditoria(TipoLogAuditoria.INSERT, fichaBusca, fichaBusca.OportunidadeId);
                        }
                    }
                    else
                    {
                        var oportunidade = _oportunidadeRepositorio.ObterOportunidadePorId(viewModel.OportunidadeId);

                        ficha.ContaId = oportunidade.ContaId;
                        ficha.Id = _oportunidadeRepositorio.IncluirFichaFaturamento(ficha);

                        var fichaBusca = _oportunidadeRepositorio.ObterDetalhesFichaFaturamento(ficha.Id);

                        GravarLogAuditoria(TipoLogAuditoria.INSERT, fichaBusca, oportunidade.Id);
                    }
                }
                catch (HttpException ex)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ex.Message);
                }
            }
            else
            {
                try
                {
                    var permiteInclusaoFicha = PermiteInclusaoFicha(oportunidadeBusca);

                    if (permiteInclusaoFicha.Any())
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, permiteInclusaoFicha.First());

                    if (ficha.Id > 0)
                    {
                        var fichaFaturamentoBusca = _oportunidadeRepositorio.ObterFichaFaturamentoPorId(ficha.Id);

                        if (fichaFaturamentoBusca == null)
                            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Ficha Faturamento não encontrada ou já excluída");

                        if (fichaFaturamentoBusca.StatusFichaFaturamento != StatusFichaFaturamento.EM_ANDAMENTO && fichaFaturamentoBusca.StatusFichaFaturamento != StatusFichaFaturamento.REJEITADO)
                            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"Não é permitido a atualização de fichas de faturamento com o Status {fichaFaturamentoBusca.StatusFichaFaturamento.ToName()}");

                        if (anexoFaturamento != null)
                        {
                            ficha.AnexoFaturamentoId = IncluirAnexo(oportunidadeBusca.Id, TipoAnexo.FICHA_FATURAMENTO, anexoFaturamento);
                        }
                        else
                        {
                            ficha.AnexoFaturamentoId = fichaFaturamentoBusca.AnexoFaturamentoId;
                        }

                        var demandaFontePagadora = true;

                        if (demandaFontePagadora)
                        {
                            if (!string.IsNullOrEmpty(viewModel.DiasSemana) && !string.IsNullOrEmpty(viewModel.DiasFaturamento))
                            {
                                ModelState.AddModelError(string.Empty, "É permitido inserir o campo Dias Faturamento (por dia da semana) OU dia útil");

                                return RetornarErros();
                            }

                            var regras = 0;

                            if (!string.IsNullOrEmpty(viewModel.CondicaoPagamentoPorDiaSemana))
                            {
                                regras += 1;
                            }

                            if (!string.IsNullOrEmpty(viewModel.CondicaoPagamentoPorDia))
                            {
                                regras += 1;
                            }

                            if (viewModel.UltimoDiaDoMes)
                            {
                                regras += 1;
                            }

                            if (regras == 3)
                            {
                                ModelState.AddModelError(string.Empty, "É permitido inserir o campo Condição Pagamento por dia de semana ou dia vencimento ou indicativo ultimo dia do mês");

                                return RetornarErros();
                            }

                            if (viewModel.EntregaEletronica && string.IsNullOrEmpty(viewModel.EmailFaturamento))
                            {
                                ModelState.AddModelError(string.Empty, "Informe o endereço de email para entrega eletrônica");

                                return RetornarErros();
                            }
                        }
                    }

                    _oportunidadeRepositorio.AtualizarFichaFaturamento(ficha);

                    var fichaBusca = _oportunidadeRepositorio.ObterDetalhesFichaFaturamento(ficha.Id);

                    GravarLogAuditoria(TipoLogAuditoria.UPDATE, fichaBusca, fichaBusca.OportunidadeId);
                }
                catch (HttpException ex)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ex.Message);
                }
            }

            PopularFichasFaturamento(viewModel);

            return PartialView("_AbaFichaFaturamentoConsultaFichas", viewModel.FichasFaturamento);
        }

        [HttpPost]
        public ActionResult ExcluirFichaFaturamento(int id)
        {
            var fichaFaturamentoBusca = _oportunidadeRepositorio.ObterFichaFaturamentoPorId(id);

            if (fichaFaturamentoBusca == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Ficha de Faturamento não encontrada");

            var oportunidadeBusca = _oportunidadeRepositorio.ObterOportunidadePorId(fichaFaturamentoBusca.OportunidadeId);

            if (oportunidadeBusca == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Oportunidade não encontrada");

            if (!User.IsInRole("OportunidadesFichas:Excluir"))
            {
                if (!_equipesService.ValidarEquipeOportunidade(oportunidadeBusca.Id))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Usuário não possui permissão para edição da Oportunidade (Equipes)");
                }
            }

            if (fichaFaturamentoBusca.StatusFichaFaturamento != StatusFichaFaturamento.EM_ANDAMENTO && fichaFaturamentoBusca.StatusFichaFaturamento != StatusFichaFaturamento.REJEITADO)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"Não é permitido a atualização de Fichas De Faturamento com o Status {fichaFaturamentoBusca.StatusFichaFaturamento.ToName()}");

            var fichaBusca = _oportunidadeRepositorio.ObterDetalhesFichaFaturamento(id);

            GravarLogAuditoria(TipoLogAuditoria.DELETE, fichaBusca, fichaBusca.OportunidadeId);

            _oportunidadeRepositorio.ExcluirFichaFaturamento(id);

            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        [HttpGet]
        public ActionResult ObterDetalhesFichaFaturamento(int id)
        {
            var fichaFaturamento = _oportunidadeRepositorio.ObterFichaFaturamentoPorId(id);

            if (fichaFaturamento == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Ficha de Faturamento não encontrada");

            var detalhesFicha = _oportunidadeRepositorio.ObterDetalhesFichaFaturamento(id);

            return Json(new
            {
                detalhesFicha.Id,
                detalhesFicha.FaturadoContraId,
                detalhesFicha.FaturadoContra,
                detalhesFicha.FaturadoContraDocumento,
                detalhesFicha.ContaId,
                detalhesFicha.Cliente,
                detalhesFicha.ClienteDocumento,
                detalhesFicha.DiasSemana,
                detalhesFicha.DiasSemanaLiterais,
                detalhesFicha.DiasFaturamento,
                detalhesFicha.DataCorte,
                detalhesFicha.EmailFaturamento,
                detalhesFicha.CondicaoPagamentoId,
                detalhesFicha.ObservacoesFaturamento,
                detalhesFicha.IdFile,
                detalhesFicha.UltimoDiaDoMes,
                detalhesFicha.DiaUtil,
                detalhesFicha.FontePagadoraId,
                detalhesFicha.FontePagadora,
                detalhesFicha.FontePagadoraDocumento,
                detalhesFicha.CondicaoPagamentoPorDia,
                detalhesFicha.CondicaoPagamentoPorDiaSemana,
                detalhesFicha.EntregaEletronica,
                detalhesFicha.EntregaManual,
                detalhesFicha.EntregaManualSedex,
                detalhesFicha.CorreioComum,
                detalhesFicha.CorreioSedex
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult EnviarFichaFaturamentoParaAprovacao(int id)
        {
            var fichaFaturamentoBusca = _oportunidadeRepositorio.ObterFichaFaturamentoPorId(id);

            if (fichaFaturamentoBusca == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Ficha Faturamento não encontrada");

            var oportunidadeBusca = _oportunidadeRepositorio.ObterOportunidadePorId(fichaFaturamentoBusca.OportunidadeId);

            if (oportunidadeBusca == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Oportunidade não encontrada");

            if (!User.IsInRole("OportunidadesFichas:EnviarParaAprovacao"))
            {
                if (!_equipesService.ValidarEquipeOportunidade(oportunidadeBusca.Id))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Usuário não possui permissão para edição da Oportunidade (Equipes)");
                }
            }

            //ANALISE DE CREDITO 
            if (habilitaDemandaAnaliseDeCredito && oportunidadeBusca.OportunidadeProposta.FormaPagamento == FormaPagamento.FATURADO)
            {
                var fontePagadoraId = fichaFaturamentoBusca.FontePagadoraId;

                var spcBusca = _analiseCreditoRepositorio.ObterConsultaSpc(fontePagadoraId);

                if (spcBusca == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Fonte Pagadora não possui análise de crédito ou análise pendente aprovação.");
                }
                else
                {
                    if (spcBusca.Validade < DateTime.Now)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Análise de Crédito vencida para a Fonte Pagadora");
                    }


                    if (spcBusca.InadimplenteEcoporto || spcBusca.InadimplenteSpc)
                    {
                        if (spcBusca.StatusAnaliseDeCredito != StatusAnaliseDeCredito.APROVADO)
                        {
                            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Fonte Pagadora conta como inadimplente. Análise de Crédito não possui aprovação.");
                        }
                    }

                    var limitesDeCreditom = _analiseCreditoRepositorio.ObterSolicitacoesLimiteDeCredito(fontePagadoraId);

                    if (_oportunidadeRepositorio.ObterStatusCondPgtoNew(fontePagadoraId, fichaFaturamentoBusca.CondicaoPagamentoFaturamentoId) != 3)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"Condição de Pagamento sem aprovação ");

                    }
                    var fatFCL = oportunidadeBusca.FaturamentoMensalFCL;
                    var fatLCL = oportunidadeBusca.FaturamentoMensalLCL;

                    //var valorLiteCreditoBase = (fatLCL / (fatFCL == 0 ? 1 : fatLCL)) * 12;

                    //if (valorLiteCreditoBase > 200_000)
                    //{
                    //    var limitesDeCredito = _analiseCreditoRepositorio.ObterSolicitacoesLimiteDeCredito(fontePagadoraId);

                    //    if (limitesDeCredito.Any())
                    //    {
                    //        var limitesDeCreditoSemAprovacao = limitesDeCredito.Where(c => c.StatusLimiteCredito != StatusLimiteCredito.APROVADO).Any();

                    //        if (limitesDeCreditoSemAprovacao)
                    //        {
                    //            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "A Análise de Crédito possui limites de crédito sem aprovação");
                    //        }
                    //    }
                    //}
                }
            }
            var adendos = _oportunidadeRepositorio.ObterAdendos(oportunidadeBusca.Id);

            var adendosFormaPgtoEmAberto = adendos
                .Where(c => c.TipoAdendo == TipoAdendo.FORMA_PAGAMENTO && c.StatusAdendo == StatusAdendo.ABERTO);

            var existeAdendoFormaPgtoFaturadoEmAberto = false;

            foreach (var adendo in adendosFormaPgtoEmAberto)
            {
                var adendoFormaPgto = _oportunidadeRepositorio.ObterAdendoFormaPagamento(adendo.Id);

                if (adendoFormaPgto.FormaPagamento == FormaPagamento.FATURADO)
                {
                    existeAdendoFormaPgtoFaturadoEmAberto = true;
                    break;
                }
            }

            if (existeAdendoFormaPgtoFaturadoEmAberto)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Envio permitido via Adendo");

            var adendosSubClienteEmAberto = adendos
               .Where(c => c.TipoAdendo == TipoAdendo.INCLUSAO_SUB_CLIENTE && c.StatusAdendo == StatusAdendo.ABERTO);

            var existeAdendoSubClienteEmAberto = false;

            foreach (var adendo in adendosSubClienteEmAberto)
            {
                var adendoSubCliente = _oportunidadeRepositorio
                    .ObterAdendoSubClientesInclusao(adendo.Id);

                existeAdendoSubClienteEmAberto = adendoSubCliente.Any(c => c.SubClienteId == fichaFaturamentoBusca.ContaId);

                if (existeAdendoSubClienteEmAberto)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Envio permitido via Adendo");
            }

            var aprovacoes = _workflowRepositorio.ObterAprovacoesPorOportunidade(oportunidadeBusca.Id, Processo.FICHA_FATURAMENTO);

            if (aprovacoes.Any() && fichaFaturamentoBusca.StatusFichaFaturamento == StatusFichaFaturamento.EM_APROVACAO)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Já existe uma aprovação pendente para esta Ficha de Faturamento");

            var token = Autenticador.Autenticar();

            if (token == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Não foi possível se autenticar no serviço de Workflow");

            var workflow = new WorkflowService(token);

            var conta = _contaRepositorio.ObterContaPorId(oportunidadeBusca.ContaId);

            CondicaoPagamentoFatura condicaoPagamentoFatura = null;

            if (fichaFaturamentoBusca.CondicaoPagamentoFaturamentoId != null)
                condicaoPagamentoFatura = _condicaoPagamentoFaturaRepositorio.ObterCondicoPagamentoPorId(fichaFaturamentoBusca.CondicaoPagamentoFaturamentoId);

            var oportunidadeDetalhes = _oportunidadeRepositorio.ObterDetalhesOportunidade(oportunidadeBusca.Id);

            var demandaFichaFaturamento = true;

            dynamic campos = null;

            if (demandaFichaFaturamento)
            {
                if (!string.IsNullOrEmpty(fichaFaturamentoBusca.CondicaoPagamentoPorDiaSemana))
                {
                    var condicaoPagamentoPorDiaSemana = fichaFaturamentoBusca.CondicaoPagamentoPorDiaSemana.Split(',');

                    if (condicaoPagamentoPorDiaSemana.Any())
                    {
                        fichaFaturamentoBusca.CondicaoPagamentoPorDiaSemana = string.Empty;

                        var diasSemanaPorExtenso = condicaoPagamentoPorDiaSemana.Select(c => DiasSemanaHelper.DiaSemanaPorExtenso(c.ToInt()));

                        fichaFaturamentoBusca.CondicaoPagamentoPorDiaSemana = string.Join(",", diasSemanaPorExtenso);
                    }
                }

                campos = new
                {
                    OportunidadeId = oportunidadeBusca.Id,
                    Descricao = $"F-{fichaFaturamentoBusca.Id}",
                    DescricaoOportunidade = oportunidadeBusca.Descricao,
                    StatusFichaFaturamento = fichaFaturamentoBusca.StatusFichaFaturamento.ToName(),
                    CondicaoPagamentoFaturamentoId = condicaoPagamentoFatura?.Descricao,
                    ContaId = oportunidadeDetalhes.Conta,
                    DataInicio = oportunidadeDetalhes.DataInicio.DataFormatada(),
                    DataTermino = oportunidadeDetalhes.DataTermino.DataFormatada(),
                    oportunidadeDetalhes.TabelaId,
                    fichaFaturamentoBusca.FaturadoContraDescricao,
                    fichaFaturamentoBusca.DiasSemana,
                    fichaFaturamentoBusca.DiasFaturamento,
                    fichaFaturamentoBusca.DataCorte,
                    fichaFaturamentoBusca.EmailFaturamento,
                    fichaFaturamentoBusca.ObservacoesFaturamento,
                    fichaFaturamentoBusca.AnexoFaturamento,
                    oportunidadeDetalhes.TipoOperacao,
                    fichaFaturamentoBusca.FontePagadoraDescricao,
                    fichaFaturamentoBusca.CondicaoPagamentoPorDia,
                    fichaFaturamentoBusca.CondicaoPagamentoPorDiaSemana,
                    fichaFaturamentoBusca.EntregaEletronica,
                    fichaFaturamentoBusca.EntregaManual,
                    fichaFaturamentoBusca.EntregaManualSedex,
                    fichaFaturamentoBusca.CorreioComum,
                    fichaFaturamentoBusca.CorreioSedex,
                    StatusOportunidade = oportunidadeDetalhes.StatusOportunidade,
                    FormaPagamento = oportunidadeDetalhes.FormaPagamento,
                };
            }
            else
            {
                campos = new
                {
                    OportunidadeId = oportunidadeBusca.Id,
                    Descricao = $"F-{fichaFaturamentoBusca.Id}",
                    DescricaoOportunidade = oportunidadeBusca.Descricao,
                    StatusFichaFaturamento = fichaFaturamentoBusca.StatusFichaFaturamento.ToName(),
                    CondicaoPagamentoFaturamentoId = condicaoPagamentoFatura?.Descricao,
                    ContaId = oportunidadeDetalhes.Conta,
                    DataInicio = oportunidadeDetalhes.DataInicio.DataFormatada(),
                    DataTermino = oportunidadeDetalhes.DataTermino.DataFormatada(),
                    oportunidadeDetalhes.TabelaId,
                    fichaFaturamentoBusca.FaturadoContraId,
                    fichaFaturamentoBusca.DiasSemana,
                    fichaFaturamentoBusca.DiasFaturamento,
                    fichaFaturamentoBusca.DataCorte,
                    fichaFaturamentoBusca.EmailFaturamento,
                    fichaFaturamentoBusca.ObservacoesFaturamento,
                    fichaFaturamentoBusca.AnexoFaturamento,
                    oportunidadeDetalhes.TipoOperacao
                };
            }

            var retorno = workflow.EnviarParaAprovacao(
                new CadastroWorkflow(Processo.FICHA_FATURAMENTO, oportunidadeBusca.EmpresaId, fichaFaturamentoBusca.Id, User.ObterLogin(), User.ObterNome(), User.ObterEmail(), JsonConvert.SerializeObject(campos)));

            if (retorno == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Nenhuma resposta do serviço de Workflow");

            if (retorno.sucesso == false)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, retorno.mensagem);

            _workflowRepositorio.IncluirEnvioAprovacao(new EnvioWorkflow(oportunidadeBusca.Id, fichaFaturamentoBusca.Id, Processo.FICHA_FATURAMENTO, retorno.protocolo, retorno.mensagem, User.ObterId()));

            _oportunidadeRepositorio.AtualizarStatusFichaFaturamento(StatusFichaFaturamento.EM_APROVACAO, fichaFaturamentoBusca.Id);

            GravarLogAuditoria(TipoLogAuditoria.OUTROS, fichaFaturamentoBusca, mensagem: "Ficha de Faturamento enviada para Aprovação");

            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        [HttpPost]
        public ActionResult Excluir(int id)
        {
            try
            {
                var oportunidadeBusca = _oportunidadeRepositorio.ObterOportunidadePorId(id);

                if (oportunidadeBusca == null)
                    RegistroNaoEncontrado();

                if (!User.IsInRole("OportunidadesIniciais:Excluir"))
                {
                    if (!_equipesService.ValidarEquipeOportunidade(oportunidadeBusca.Id))
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Usuário não possui permissão para edição da Oportunidade (Equipes)");
                    }
                }

                if (Enum.IsDefined(typeof(SucessoNegociacao), oportunidadeBusca.SucessoNegociacao) && (oportunidadeBusca.SucessoNegociacao == SucessoNegociacao.GANHO || oportunidadeBusca.SucessoNegociacao == SucessoNegociacao.PERDIDO))
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"Exclusão não permitida para Sucesso Negociação igual a {oportunidadeBusca.SucessoNegociacao.ToName()}");

                _oportunidadeRepositorio.Excluir(oportunidadeBusca.Id);

                GravarLogAuditoria(TipoLogAuditoria.DELETE, oportunidadeBusca);
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        [HttpPost]
        public ActionResult ImportarModeloLayout(int id = 0, int modeloId = 0)
        {
            var oportunidadeBusca = _oportunidadeRepositorio.ObterOportunidadePorId(id);

            if (oportunidadeBusca == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Oportunidade não encontrada");

            var modeloBusca = _modeloRepositorio.ObterModeloPorId(modeloId);

            if (modeloBusca == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Modelo não encontrado");

            _layoutPropostaRepositorio.LimparCamposAlterados(oportunidadeBusca.Id);

            _oportunidadeService.ImportarLayoutNaOportunidade(oportunidadeBusca.Id, modeloBusca.Id);

            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        [HttpPost]
        public ActionResult IncluirSubCliente(int segmento, int contaId, int oportunidadeId)
        {
            if (segmento == 0)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Selecione o Segmento");

            var contaBusca = _contaRepositorio.ObterContaPorId(contaId);

            if (contaBusca == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Conta não encontrada");

            var oportunidadeBusca = _oportunidadeRepositorio.ObterOportunidadePorId(oportunidadeId);

            if (oportunidadeBusca == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Oportunidade não encontrada");

            if (!User.IsInRole("OportunidadesIniciais:AdicionarSubCliente"))
            {
                if (!_equipesService.ValidarEquipeOportunidade(oportunidadeBusca.Id))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Usuário não possui permissão para edição da Oportunidade (Equipes)");
                }
            }

            if (oportunidadeBusca.SucessoNegociacao == SucessoNegociacao.GANHO || oportunidadeBusca.SucessoNegociacao == SucessoNegociacao.PERDIDO)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Operação não permitida em Oportunidades Ganha / Perdida");

            var existeSubCliente = _oportunidadeRepositorio.ObterSubClientes(oportunidadeBusca.Id)
                .Where(c => c.ContaId == contaId)
                .Any();

            if (existeSubCliente)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Sub Cliente já incluído na Oportunidade");
            }

            _oportunidadeRepositorio.IncluirSubCliente(segmento, contaId, oportunidadeId, User.ObterId());

            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        public OportunidadeConcomitancia ValidarConcomitanciaAdendo(Oportunidade oportunidade, OportunidadeAdendo adendo)
        {
            var concomitante = false;
            string mensagemRetorno = string.Empty;

            var propostasConcomitantes = _concomitanciaTabelaService.ObtemPropostasDuplicadasCRM(oportunidade);

            var propostas = new List<int>();
            var listaCnpjs = new List<string>();

            if (propostasConcomitantes.Any())
            {
                //concomitante = true;

                //var propostas = string.Join(", ", propostasConcomitantes.Select(c => c.Identificacao).ToList().Distinct());

                //mensagemRetorno += $"Propostas concomitantes: {propostas} <br />";

                concomitante = true;

                mensagemRetorno += $"<br />Proposta(s) concomitante(s): ";

                foreach (var proposta in propostasConcomitantes)
                {
                    if (!propostas.Contains(proposta.Identificacao))
                    {
                        propostas.Add(proposta.Identificacao);

                        mensagemRetorno += $"<strong>{proposta.Identificacao}</strong> <br />";

                        foreach (var cnpj in propostasConcomitantes.Where(c => c.Identificacao == proposta.Identificacao))
                        {
                            cnpj.CnpjImportador = cnpj.CnpjImportador?.Replace("Z", string.Empty);
                            cnpj.CnpjDespachante = cnpj.CnpjDespachante?.Replace("Z", string.Empty);
                            cnpj.CnpjColoader = cnpj.CnpjColoader?.Replace("Z", string.Empty);
                            cnpj.CnpjCoColoader = cnpj.CnpjCoColoader?.Replace("Z", string.Empty);
                            cnpj.CnpjCoColoader2 = cnpj.CnpjCoColoader2?.Replace("Z", string.Empty);

                            if (!string.IsNullOrEmpty(cnpj.CnpjImportador))
                            {
                                if (!listaCnpjs.Contains(cnpj.CnpjImportador))
                                    listaCnpjs.Add(cnpj.CnpjImportador);
                            }

                            if (!string.IsNullOrEmpty(cnpj.CnpjDespachante))
                            {
                                if (!listaCnpjs.Contains(cnpj.CnpjDespachante))
                                    listaCnpjs.Add(cnpj.CnpjDespachante);
                            }

                            if (!string.IsNullOrEmpty(cnpj.CnpjColoader))
                            {
                                if (!listaCnpjs.Contains(cnpj.CnpjColoader))
                                    listaCnpjs.Add(cnpj.CnpjColoader);
                            }

                            if (!string.IsNullOrEmpty(cnpj.CnpjCoColoader))
                            {
                                if (!listaCnpjs.Contains(cnpj.CnpjCoColoader))
                                    listaCnpjs.Add(cnpj.CnpjCoColoader);
                            }

                            if (!string.IsNullOrEmpty(cnpj.CnpjCoColoader2))
                            {
                                if (!listaCnpjs.Contains(cnpj.CnpjCoColoader2))
                                    listaCnpjs.Add(cnpj.CnpjCoColoader2);
                            }
                        }

                        if (listaCnpjs.Any())
                        {
                            mensagemRetorno += $"({string.Join(", ", listaCnpjs.Where(s => !string.IsNullOrEmpty(s)).Distinct())})";
                        }

                        mensagemRetorno += $"<br />";
                    }
                }

                mensagemRetorno += $"<br />";
            }

            var propostasConcomitantesChronos = _concomitanciaTabelaService
                .ObtemTabelasDuplicadasChronos(oportunidade.Id)
                .Where(c => c.Id != oportunidade.TabelaId); // Desconsidera a Tabela informada na proposta

            if (propostasConcomitantesChronos.Any())
            {
                propostas.Clear();
                listaCnpjs.Clear();

                mensagemRetorno += $"<br />Tabela(s) Concomitantes: ";

                foreach (var tabela in propostasConcomitantesChronos)
                {
                    if (!propostas.Contains(tabela.Id))
                    {
                        propostas.Add(tabela.Id);

                        mensagemRetorno += $"<strong>{tabela.Id}</strong> <br />";

                        foreach (var cnpj in propostasConcomitantesChronos.Where(c => c.Id == tabela.Id))
                        {
                            cnpj.CnpjImportador = cnpj.CnpjImportador?.Replace("Z", string.Empty);
                            cnpj.CnpjDespachante = cnpj.CnpjDespachante?.Replace("Z", string.Empty);
                            cnpj.CnpjColoader = cnpj.CnpjColoader?.Replace("Z", string.Empty);
                            cnpj.CnpjCoColoader = cnpj.CnpjCoColoader?.Replace("Z", string.Empty);
                            cnpj.CnpjCoColoader2 = cnpj.CnpjCoColoader2?.Replace("Z", string.Empty);

                            if (!string.IsNullOrEmpty(cnpj.CnpjImportador))
                            {
                                if (!listaCnpjs.Contains(cnpj.CnpjImportador))
                                    listaCnpjs.Add(cnpj.CnpjImportador);
                            }

                            if (!string.IsNullOrEmpty(cnpj.CnpjDespachante))
                            {
                                if (!listaCnpjs.Contains(cnpj.CnpjDespachante))
                                    listaCnpjs.Add(cnpj.CnpjDespachante);
                            }

                            if (!string.IsNullOrEmpty(cnpj.CnpjColoader))
                            {
                                if (!listaCnpjs.Contains(cnpj.CnpjColoader))
                                    listaCnpjs.Add(cnpj.CnpjColoader);
                            }

                            if (!string.IsNullOrEmpty(cnpj.CnpjCoColoader))
                            {
                                if (!listaCnpjs.Contains(cnpj.CnpjCoColoader))
                                    listaCnpjs.Add(cnpj.CnpjCoColoader);
                            }

                            if (!string.IsNullOrEmpty(cnpj.CnpjCoColoader2))
                            {
                                if (!listaCnpjs.Contains(cnpj.CnpjCoColoader2))
                                    listaCnpjs.Add(cnpj.CnpjCoColoader2);
                            }
                        }

                        if (listaCnpjs.Any())
                        {
                            mensagemRetorno += $"({string.Join(", ", listaCnpjs.Where(s => !string.IsNullOrEmpty(s)).Distinct())})";
                        }

                        mensagemRetorno += $"<br />";
                    }
                }

                mensagemRetorno += $"<br />";
            }

            return new OportunidadeConcomitancia
            {
                Concomitante = concomitante,
                Mensagem = mensagemRetorno,
                RedirectUrl = Url.Action(nameof(Atualizar), new { id = oportunidade.Id })
            };
        }

        public JsonResult ValidarConcomitanciaAdendosCliente(int oportunidadeId, int adendoId)
        {
            var parametros = _parametrosRepositorio.ObterParametros();

            if (parametros.ValidaConcomitancia)
            {
                var oportunidadeBusca = _oportunidadeRepositorio.ObterOportunidadePorId(oportunidadeId);

                if (oportunidadeBusca == null)
                    throw new Exception("Oportunidade não encontrada");

                var adendoBusca = _oportunidadeRepositorio.ObterAdendoPorId(adendoId);

                if (adendoBusca == null)
                    throw new Exception("Adendo não encontrado");

                var concomitancia = ValidarConcomitanciaAdendo(oportunidadeBusca, adendoBusca);

                return Json(new
                {
                    Existe = concomitancia.Concomitante,
                    Mensagem = concomitancia.Mensagem,
                    RedirectUrl = Url.Action(nameof(Atualizar), new { id = oportunidadeId })
                }, JsonRequestBehavior.AllowGet);
            }

            return null;
        }

        [HttpPost]
        public ActionResult ExcluirSubCliente(int id)
        {
            var subClienteBusca = _oportunidadeRepositorio.ObterSubClientePorId(id);

            if (subClienteBusca == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Sub Cliente não encontrado");

            var oportunidadeBusca = _oportunidadeRepositorio.ObterOportunidadePorId(subClienteBusca.OportunidadeId);

            if (!User.IsInRole("OportunidadesIniciais:ExcluirSubCliente"))
            {
                if (!_equipesService.ValidarEquipeOportunidade(oportunidadeBusca.Id))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Usuário não possui permissão para edição da Oportunidade (Equipes)");
                }
            }

            if (oportunidadeBusca.StatusOportunidade == StatusOportunidade.ENVIADO_PARA_APROVACAO || oportunidadeBusca.SucessoNegociacao == SucessoNegociacao.GANHO || oportunidadeBusca.SucessoNegociacao == SucessoNegociacao.PERDIDO)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Exclusão não permitida");

            _oportunidadeRepositorio.ExcluirSubCliente(id);

            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        [HttpGet]
        public PartialViewResult ConsultarSubClientes(int oportunidadeId)
        {
            var subClientes = _oportunidadeRepositorio.ObterSubClientes(oportunidadeId);

            return PartialView("_AbaInformacoesIniciaisConsultaSubClientes", subClientes);
        }

        [HttpGet]
        public JsonResult ConsultarSubClientesJson(int oportunidadeId)
        {
            var subClientes = _oportunidadeRepositorio.ObterSubClientes(oportunidadeId)
                 .Select(c => new
                 {
                     c.Id,
                     c.ContaId,
                     c.Descricao,
                     c.Documento,
                     c.CriadoPor,
                     DataCriacao = c.DataCriacao.DataFormatada(),
                     Segmento = c.Segmento.ToName()
                 });

            return Json(new
            {
                dados = subClientes
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ConsultarSubClientesFichaFaturamentoJson(int oportunidadeId)
        {
            var subClientes = _oportunidadeRepositorio.ObterSubClientesDaProposta(oportunidadeId)
                .Where(c => c.Segmento == SegmentoSubCliente.IMPORTADOR)
                .Select(c => new
                {
                    c.Id,
                    c.ContaId,
                    c.Descricao,
                    c.Documento,
                    c.CriadoPor,
                    DataCriacao = c.DataCriacao.DataFormatada(),
                    Segmento = c.Segmento.ToName()
                });

            return Json(new
            {
                dados = subClientes
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ConsultarContaDaOportunidadeJson(int oportunidadeId)
        {
            var contas = _oportunidadeRepositorio.ObterContasDaOportunidade(oportunidadeId)
                .Select(c => new
                {
                    c.Id,
                    c.Descricao,
                    c.Documento,
                    DataCriacao = c.DataCriacao.DataFormatada()
                });

            return Json(new { dados = contas }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult IncluirClienteGrupoCNPJ(int contaId, int oportunidadeId)
        {
            var contaBusca = _contaRepositorio.ObterContaPorId(contaId);

            if (contaBusca == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Conta não encontrada");

            var oportunidadeBusca = _oportunidadeRepositorio.ObterOportunidadePorId(oportunidadeId);

            if (oportunidadeBusca == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Oportunidade não encontrada");

            if (!User.IsInRole("OportunidadesIniciais:AdicionarClienteGrupoCNPJ"))
            {
                if (!_equipesService.ValidarEquipeOportunidade(oportunidadeBusca.Id))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Usuário não possui permissão para edição da Oportunidade (Equipes)");
                }
            }

            if (oportunidadeBusca.SucessoNegociacao == SucessoNegociacao.GANHO || oportunidadeBusca.SucessoNegociacao == SucessoNegociacao.PERDIDO)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Operação não permitida em Oportunidades Ganha / Perdida");

            var existeSubCliente = _oportunidadeRepositorio.ObterClientesGrupoCNPJ(oportunidadeBusca.Id)
                .Where(c => c.ContaId == contaId)
                .Any();

            if (existeSubCliente)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Sub Cliente já incluído na Oportunidade");
            }

            _oportunidadeRepositorio.IncluirClienteGrupoCNPJ(contaId, oportunidadeId, User.ObterId());

            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        [HttpPost]
        public ActionResult ExcluirClienteGrupoCNPJ(int id)
        {
            var clienteGrupoBusca = _oportunidadeRepositorio.ObterClienteGrupoCNPJPorId(id);

            if (clienteGrupoBusca == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Cliente não encontrado");

            var oportunidadeBusca = _oportunidadeRepositorio.ObterOportunidadePorId(clienteGrupoBusca.OportunidadeId);

            if (!User.IsInRole("OportunidadesIniciais:ExcluirClienteGrupoCNPJ"))
            {
                if (!_equipesService.ValidarEquipeOportunidade(oportunidadeBusca.Id))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Usuário não possui permissão para edição da Oportunidade (Equipes)");
                }
            }

            if (oportunidadeBusca.StatusOportunidade == StatusOportunidade.ENVIADO_PARA_APROVACAO || oportunidadeBusca.SucessoNegociacao == SucessoNegociacao.GANHO || oportunidadeBusca.SucessoNegociacao == SucessoNegociacao.PERDIDO)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Exclusão não permitida");

            _oportunidadeRepositorio.ExcluirClienteGrupoCNPJ(id);

            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        [HttpGet]
        public PartialViewResult ConsultarClientesGrupoCNPJ(int oportunidadeId)
        {
            var clientesGrupo = _oportunidadeRepositorio.ObterClientesGrupoCNPJ(oportunidadeId);

            return PartialView("_AbaInformacoesIniciaisConsultaGrupoCNPJ", clientesGrupo);
        }

        [HttpGet]
        public ActionResult ConsultarClientesGrupoCNPJJson(int oportunidadeId)
        {
            var clientesGrupo = _oportunidadeRepositorio.ObterClientesGrupoCNPJ(oportunidadeId)
                 .Select(c => new
                 {
                     c.Id,
                     c.Descricao,
                     c.Documento,
                     c.CriadoPor,
                     DataCriacao = c.DataCriacao.DataFormatada()
                 });

            return Json(new
            {
                dados = clientesGrupo
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult CadastrarAnexos([Bind(Include = "AnexoOportunidadeId")] OportunidadesAnexosViewModel viewModel, HttpPostedFileBase anexo)
        {
            if (ModelState.IsValid)
            {
                var oportunidadeBusca = _oportunidadeRepositorio.ObterOportunidadePorId(viewModel.AnexoOportunidadeId);

                if (oportunidadeBusca == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Oportunidade não encontrada");

                if (oportunidadeBusca.StatusOportunidade == StatusOportunidade.ENVIADO_PARA_APROVACAO)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"Não é permitido cadastrar anexos quando Status for {oportunidadeBusca.StatusOportunidade.ToName()}");

                //if (!User.IsInRole("OportunidadesAnexos:Cadastrar"))
                //{
                //    if (!_equipesService.ValidarEquipeOportunidade(oportunidadeBusca.Id))
                //    {
                //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Usuário não possui permissão para edição da Oportunidade (Equipes)");
                //    }
                //}

                if (anexo == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Selecione um arquivo para anexar a Oportunidade");

                try
                {
                    IncluirAnexo(oportunidadeBusca.Id, TipoAnexo.OUTROS, anexo);
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

        public List<string> ArquivosPermitidos => new List<string>
        {
            ".pdf",".xls",".xlsx",".doc",".docx",".jpg",".jpeg",".png",".gif",".msg",".tif",".txt"
        };

        private int IncluirAnexo(int oportunidadeId, TipoAnexo tipoAnexo, HttpPostedFileBase anexo)
        {
            if (anexo != null && anexo.ContentLength > 0)
            {
                if (anexo.ContentLength > 5242880)
                    throw new HttpException(500, "O tamanho do arquivo não deve ultrapassar 5MB");

                if (!ArquivosPermitidos.Contains(new FileInfo(anexo.FileName).Extension.ToLower()))
                    throw new HttpException(500, "Extensão de arquivo não permitida");

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

                    if (!retornoUpload.success)
                        throw new HttpException(500, "Retorno API anexos: " + retornoUpload.message);

                    var anexoInclusaoId = _anexoRepositorio.IncluirAnexo(
                        new Anexo
                        {
                            IdProcesso = oportunidadeId,
                            Arquivo = anexo.FileName,
                            CriadoPor = User.ObterId(),
                            TipoAnexo = tipoAnexo,
                            TipoDoc = 1,
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

            var oportunidadeBusca = _oportunidadeRepositorio.ObterOportunidadePorId(anexoBusca.IdProcesso);

            if (oportunidadeBusca == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Oportunidade não encontrada");

            if (!User.IsInRole("Administrador") && !User.IsInRole("OportunidadeAnexosFullControll"))
            {
                if (anexoBusca.CriadoPor != User.ObterId())
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "O anexo só pode ser excluído pelo usuário de criação");
            }

            if (anexoBusca.TipoAnexo == TipoAnexo.PROPOSTA || anexoBusca.TipoAnexo == TipoAnexo.RELATORIO_SIMULADOR)
            {
                if (oportunidadeBusca.StatusOportunidade == StatusOportunidade.ATIVA || oportunidadeBusca.StatusOportunidade == StatusOportunidade.ENVIADO_PARA_APROVACAO)
                {
                    if (!User.IsInRole("OportunidadeAnexosFullControll"))
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"Exclusão de arquivo não permitida para Status {oportunidadeBusca.StatusOportunidade.ToName()}");
                }

                if (oportunidadeBusca.StatusOportunidade == StatusOportunidade.VENCIDO || oportunidadeBusca.StatusOportunidade == StatusOportunidade.REVISADA || oportunidadeBusca.StatusOportunidade == StatusOportunidade.CANCELADA)
                {
                    if (User.IsInRole("Administrador") || User.IsInRole("OportunidadeAnexosFullControll"))
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"Não é possível excluir anexos para Oportunidade {oportunidadeBusca.StatusOportunidade.ToName()}");
                }
            }

            try
            {
                var token = Sharepoint.Services.Autenticador.Autenticar();

                if (token == null)
                    throw new HttpException(404, "Não foi possível se autenticar no serviço de Anexos");

                var arquivoId = Converters.RawToGuid(idArquivo);

                var retornoUpload = new Sharepoint.Services.AnexosService(token)
                    .ExcluirArquivo(arquivoId);

                if (!retornoUpload.success)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, retornoUpload.message);
                }

                var detalhesAnexo = _anexoRepositorio.ObterDetalhesAnexo(id);

                _anexoRepositorio.ExcluirAnexo(id);

                GravarLogAuditoria(TipoLogAuditoria.DELETE, detalhesAnexo, detalhesAnexo.IdProcesso);

                return new HttpStatusCodeResult(HttpStatusCode.NoContent);
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Falha ao excluir o anexo. Tente novamente.");
            }
        }

        [HttpPost]
        public ActionResult CadastrarNotas([Bind(Include = "NotaId, NotaOportunidadeId, Nota, NotaDescricao")] OportunidadesNotasViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var oportunidadeBusca = _oportunidadeRepositorio.ObterOportunidadePorId(viewModel.NotaOportunidadeId);

                if (oportunidadeBusca == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Oportunidade não encontrada");

                var nota = new OportunidadeNotas(viewModel.NotaOportunidadeId, viewModel.Nota, viewModel.NotaDescricao, User.ObterId());

                if (!Validar(nota))
                    return RetornarErros();

                if (viewModel.NotaId == 0)
                {
                    _oportunidadeRepositorio.IncluirNota(nota);
                }
                else
                {
                    var notaBusca = _oportunidadeRepositorio.ObterNotaPorId(viewModel.NotaId);

                    if (notaBusca == null)
                    {
                        RegistroNaoEncontrado();
                    }

                    notaBusca.Alterar(nota);

                    _oportunidadeRepositorio.AtualizarNota(notaBusca);
                }

                PopularNotas(viewModel);

                return PartialView("_AbaNotasConsulta", viewModel.Notas);
            }

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Falha ao anexar o arquivo");
        }

        [HttpPost]
        public ActionResult ExcluirNota(int id)
        {
            try
            {
                var notaBusca = _oportunidadeRepositorio.ObterNotaPorId(id);

                if (notaBusca == null)
                {
                    RegistroNaoEncontrado();
                }

                if (!User.IsInRole("Administrador"))
                {
                    if (notaBusca.CriadoPor != User.ObterId())
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "O anexo só pode ser excluído pelo usuário de criação");
                }

                _oportunidadeRepositorio.ExcluirNota(notaBusca.Id);

                GravarLogAuditoria(TipoLogAuditoria.DELETE, notaBusca);
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        public ActionResult ObterDetalhesNota(int id)
        {
            var notaBusca = _oportunidadeRepositorio.ObterNotaPorId(id);

            if (notaBusca == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Nota não encontrada");

            return Json(new
            {
                notaBusca.Id,
                notaBusca.Nota,
                notaBusca.Descricao
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ObterNotasPorDescricao(string descricao, int oportunidadeId)
        {
            var resultado = _oportunidadeRepositorio.ObterNotaPorDescricao(descricao, oportunidadeId);

            return PartialView("_AbaNotasConsulta", resultado);
        }

        [HttpPost]
        public ActionResult CancelarOportunidade(int id, string dataCancelamento)
        {
            var oportunidadeBusca = _oportunidadeRepositorio.ObterOportunidadePorId(id);

            if (oportunidadeBusca == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Oportunidade não encontrada");

            if (!User.IsInRole("OportunidadesIniciais:CancelarOportunidade"))
            {
                if (!_equipesService.ValidarEquipeOportunidade(oportunidadeBusca.Id))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Usuário não possui permissão para edição da Oportunidade (Equipes)");
                }
            }

            if (oportunidadeBusca.Cancelado)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "A Oportunidade já está cancelada");

            if (!Enum.IsDefined(typeof(StatusOportunidade), oportunidadeBusca.StatusOportunidade))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"Operação não permitida apenas para o status atual da Oportunidade");
            else
            {
                if (oportunidadeBusca.StatusOportunidade != StatusOportunidade.ATIVA && oportunidadeBusca.StatusOportunidade != StatusOportunidade.REVISADA && oportunidadeBusca.StatusOportunidade != StatusOportunidade.VENCIDO)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"Operação não permitida apenas para Oportunidades com status {oportunidadeBusca.StatusOportunidade.ToName()}");
            }

            if (!DateTimeHelpers.IsDate(dataCancelamento))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Data inválida!");

            oportunidadeBusca.DataCancelamento = Convert.ToDateTime(dataCancelamento);

            var averbacao = _loteRepositorio.ExisteAverbacao(oportunidadeBusca.Id, oportunidadeBusca.DataCancelamento);

            if (averbacao != null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"Existe processo Averbado {averbacao.NumeroLote} após a data informada");
            }

            _oportunidadeRepositorio.AtualizarOportunidadeCancelada(oportunidadeBusca);

            var detalhesOportunidade = _oportunidadeRepositorio.ObterDetalhesOportunidade(oportunidadeBusca.Id);

            _oportunidadeRepositorio.PermiteAlterarDataCancelamento(oportunidadeBusca.Id, true);

            GravarLogAuditoria(TipoLogAuditoria.UPDATE, detalhesOportunidade);

            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        [HttpGet]
        public Proposta MontaLayout(int id)
        {
            var layoutService = new LayoutService(_layoutPropostaRepositorio, _modeloRepositorio, _oportunidadeRepositorio);

            var linhas = _layoutPropostaRepositorio.ObterLayouts(id, true).ToList();
            var linhasSemCondicoes = linhas.Where(c => c.TipoRegistro != TipoRegistro.CONDICAO_INICIAL && c.TipoRegistro != TipoRegistro.CONDICAO_GERAL).ToList();

            var condicoesIniciais = layoutService.ObterCondicoesIniciais(linhas, id);
            var layout = layoutService.MontarLayout(linhasSemCondicoes, id);
            var condicoesGerais = layoutService.ObterCondicoesGerais(linhas);

            var proposta = new Proposta
            {
                CondicoesIniciais = condicoesIniciais?.ToString(),
                Tabela = layout,
                CondicoesGerais = condicoesGerais?.ToString()
            };

            return proposta;
        }

        [HttpGet]
        public ActionResult ExisteLayoutNaProposta(int oportunidadeId)
        {
            var resultado = _oportunidadeRepositorio.ExisteLayoutNaProposta(oportunidadeId);

            if (!resultado)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Não existe nenhum Layout na Proposta");

            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        [HttpGet]
        public string MontaPreview(int id)
        {
            var layout = MontaLayout(id);

            return layout.ToString();
        }

        [HttpPost]
        public ActionResult CadastrarPremiosParceria([Bind(Include = "PremioParceriaId, PremioParceriaOportunidadeId, StatusPremioParceria, Favorecido1, Favorecido2, Favorecido3, Instrucao, PremioParceriaContatoId, PremioReferenciaId, TipoServicoPremioParceria, PremioRevisaoId, Observacoes, UrlPremio, DataUrlPremio, EmailFavorecido1, EmailFavorecido2, EmailFavorecido3, ModalidadesSelecionadas")] OportunidadesPremioParceriaViewModel viewModel, HttpPostedFileBase anexoPremio)
        {
            if (viewModel.PremioParceriaOportunidadeId == 0)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Oportunidade não informada");

            var oportunidadeBusca = _oportunidadeRepositorio.ObterOportunidadePorId(viewModel.PremioParceriaOportunidadeId);

            if (oportunidadeBusca == null)
                RegistroNaoEncontrado();

            if (oportunidadeBusca.SucessoNegociacao != SucessoNegociacao.GANHO)
            {
                if (Enum.IsDefined(typeof(StatusOportunidade), oportunidadeBusca.StatusOportunidade) && oportunidadeBusca.StatusOportunidade != StatusOportunidade.ENVIADO_PARA_APROVACAO)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Disponível apenas se o Sucesso da Negociação for igual a Ganho");
                }
            }

            var premioParceria = new OportunidadePremioParceria
            {
                OportunidadeId = oportunidadeBusca.Id,
                StatusPremioParceria = StatusPremioParceria.EM_ANDAMENTO,
                Favorecido1 = viewModel.Favorecido1,
                Favorecido2 = viewModel.Favorecido2,
                Favorecido3 = viewModel.Favorecido3,
                Instrucao = viewModel.Instrucao,
                ContatoId = viewModel.PremioParceriaContatoId,
                PremioReferenciaId = viewModel.PremioReferenciaId,
                TipoServicoPremioParceria = viewModel.TipoServicoPremioParceria,
                Observacoes = viewModel.Observacoes,
                UrlPremio = viewModel.UrlPremio,
                PremioRevisaoId = viewModel.PremioRevisaoId,
                DataUrlPremio = viewModel.DataUrlPremio,
                EmailFavorecido1 = viewModel.EmailFavorecido1?.Trim(),
                EmailFavorecido2 = viewModel.EmailFavorecido2?.Trim(),
                EmailFavorecido3 = viewModel.EmailFavorecido3?.Trim(),
                CriadoPor = User.ObterId()
            };

            premioParceria.Id = viewModel.PremioParceriaId;

            if (!Validar(premioParceria))
                return RetornarErros();

            foreach (var modalidade in viewModel.ModalidadesSelecionadas)
                premioParceria.AdicionarModalidade(modalidade);

            var premiosEmAndamento = _premioParceriaRepositorio
                .ObterPremiosParceriaPorOportunidade(oportunidadeBusca.Id)
                .Where(c => c.StatusPremioParceria == StatusPremioParceria.EM_ANDAMENTO);

            if (viewModel.PremioParceriaId == 0 || viewModel.PremioRevisaoId != 0)
            {
                if (premiosEmAndamento.Any())
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Já existe um prêmio parceria em andamento");
                }

                if (!User.IsInRole("OportunidadesPremios:Cadastrar"))
                {
                    if (!_equipesService.ValidarEquipeOportunidade(oportunidadeBusca.Id))
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Usuário não possui permissão para edição da Oportunidade (Equipes)");
                    }
                }

                // TODO: verificar
                premioParceria.AnexoId = IncluirAnexo(oportunidadeBusca.Id, TipoAnexo.PREMIO_PARCERIA, anexoPremio);

                if (_premioParceriaRepositorio.ExistePremioParceria(premioParceria))
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Já existe um prêmio parceria em processo de aprovação");

                premioParceria.Id = _premioParceriaRepositorio.Cadastrar(premioParceria);

                var premioParceriaBusca = _premioParceriaRepositorio.ObterDetalhesPremioParceria(premioParceria.Id);

                GravarLogAuditoria(TipoLogAuditoria.INSERT, premioParceriaBusca, premioParceriaBusca.OportunidadeId);
            }
            else
            {
                if (!User.IsInRole("OportunidadesPremios:Cadastrar"))
                {
                    if (!_equipesService.ValidarEquipeOportunidade(oportunidadeBusca.Id))
                    {
                        if (!User.IsInRole("OportunidadesPremios:DataUrlPremio") && !User.IsInRole("OportunidadesPremios:UrlPremio"))
                        {
                            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Usuário não possui permissão para edição da Oportunidade (Equipes)");
                        }
                    }
                }

                var premioParceriaBusca = _premioParceriaRepositorio.ObterPremioParceriaPorId(premioParceria.Id);

                if (premioParceriaBusca == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Prêmio Parceria não encontrado ou já excluído");

                if (premioParceriaBusca.StatusPremioParceria != StatusPremioParceria.EM_ANDAMENTO && premioParceriaBusca.StatusPremioParceria != StatusPremioParceria.REJEITADO)
                {
                    if (!User.IsInRole("OportunidadesPremios:DataUrlPremio") && !User.IsInRole("OportunidadesPremios:UrlPremio"))
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"Não é permitido a atualização de prêmios com o Status {premioParceriaBusca.StatusPremioParceria.ToName()}");
                    }
                }

                if (anexoPremio != null)
                {
                    premioParceria.AnexoId = IncluirAnexo(oportunidadeBusca.Id, TipoAnexo.PREMIO_PARCERIA, anexoPremio);
                }
                else
                {
                    premioParceria.AnexoId = premioParceriaBusca.AnexoId;
                }

                _premioParceriaRepositorio.Atualizar(premioParceria);

                var detalhesPremio = _premioParceriaRepositorio.ObterDetalhesPremioParceria(premioParceria.Id);

                GravarLogAuditoria(TipoLogAuditoria.UPDATE, detalhesPremio, detalhesPremio.OportunidadeId);
            }

            PopularPremiosParceria(viewModel);

            return PartialView("_AbaPremioParceriaConsulta", viewModel.Premios);
        }

        [HttpGet]
        public ActionResult ConsultarPremiosReferencia(string descricao)
        {
            var resultado = _premioParceriaRepositorio.ObterPremiosParceriaPorDescricao(descricao);

            return PartialView("_PesquisarPremiosConsulta", resultado);
        }

        [HttpGet]
        public ActionResult ObterModalidadesPremioParceria(TipoServicoPremioParceria tipoServico)
        {
            var modalidades = Enum.GetValues(typeof(ModalidadesComissionamento)).Cast<ModalidadesComissionamento>();

            if (tipoServico == TipoServicoPremioParceria.EXPORTACAO)
            {
                modalidades = modalidades
                    .Where(c => c == ModalidadesComissionamento.PACOTE ||
                        c == ModalidadesComissionamento.CONTEINER);
            }
            else if (tipoServico == TipoServicoPremioParceria.LTL_EXPORTACAO)
            {
                modalidades = new List<ModalidadesComissionamento>();
            }

            return Json(new
            {
                modalidades = modalidades.Select(c => new
                {
                    Id = c,
                    Descricao = c.ToName()
                }),
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ExcluirPremioParceria(int id)
        {
            var premioParceriaBusca = _premioParceriaRepositorio.ObterPremioParceriaPorId(id);

            if (premioParceriaBusca == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Prêmio Parceria não encontrado");

            var oportunidadeBusca = _oportunidadeRepositorio.ObterOportunidadePorId(premioParceriaBusca.OportunidadeId);

            if (oportunidadeBusca == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Oportunidade não encontrada");

            if (!User.IsInRole("OportunidadesPremios:Excluir"))
            {
                if (!_equipesService.ValidarEquipeOportunidade(oportunidadeBusca.Id))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Usuário não possui permissão para edição da Oportunidade (Equipes)");
                }
            }

            if (premioParceriaBusca.StatusPremioParceria != StatusPremioParceria.EM_ANDAMENTO)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"Não é permitido a exclusão de prêmios com o Status {premioParceriaBusca.StatusPremioParceria.ToName()}");

            var detalhesPremio = _premioParceriaRepositorio.ObterDetalhesPremioParceria(id);

            GravarLogAuditoria(TipoLogAuditoria.DELETE, detalhesPremio, detalhesPremio.OportunidadeId);

            _premioParceriaRepositorio.Excluir(id);

            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        [HttpPost]
        public ActionResult EnviarPremioParceriaParaAprovacao(int id)
        {
            var premioParceriaBusca = _premioParceriaRepositorio.ObterPremioParceriaPorId(id);

            if (premioParceriaBusca == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Prêmio Parceria não encontrado");

            var oportunidadeBusca = _oportunidadeRepositorio.ObterOportunidadePorId(premioParceriaBusca.OportunidadeId);

            if (oportunidadeBusca == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Oportunidade não encontrada");

            if (!User.IsInRole("OportunidadesPremios:EnviarParaAprovacao"))
            {
                if (!_equipesService.ValidarEquipeOportunidade(oportunidadeBusca.Id))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Usuário não possui permissão para edição da Oportunidade (Equipes)");
                }
            }

            var aprovacoes = _workflowRepositorio.ObterAprovacoesPorOportunidade(oportunidadeBusca.Id, Processo.PREMIO_PARCERIA);

            if (aprovacoes.Any() && premioParceriaBusca.StatusPremioParceria == StatusPremioParceria.EM_APROVACAO)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Já existe uma aprovação pendente para este Prêmio Parceria");

            var token = Autenticador.Autenticar();

            if (token == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Não foi possível se autenticar no serviço de Workflow");

            var workflow = new WorkflowService(token);

            var premioDetalhes = _premioParceriaRepositorio.ObterDetalhesPremioParceria(premioParceriaBusca.Id);

            var campos = new
            {
                Descricao = $"P-{premioDetalhes.Id}",
                DescricaoOportunidade = oportunidadeBusca.Descricao,
                ContaDescricao = oportunidadeBusca.Conta.Descricao,
                premioDetalhes.OportunidadeId,
                ContatoId = premioDetalhes.DescricaoContato,
                StatusPremioParceria = premioDetalhes.DescricaoStatusPremioParceria,
                Instrucao = premioDetalhes.DescricaoInstrucao,
                TipoServicoPremioParceria = premioDetalhes.DescricaoServicoPremioParceria,
                Favorecido1 = premioDetalhes.DescricaoFavorecido1,
                Favorecido2 = premioDetalhes.DescricaoFavorecido2,
                Favorecido3 = premioDetalhes.DescricaoFavorecido3,
                PremioReferenciaId = $"P-{premioDetalhes.PremioReferenciaId}",
                premioDetalhes.Observacoes,
                premioDetalhes.Anexo,
                premioDetalhes.UrlPremio,
                DataUrlPremio = premioDetalhes.DataUrlPremio.DataFormatada(),
                premioDetalhes.EmailFavorecido1,
                premioDetalhes.EmailFavorecido2,
                premioDetalhes.EmailFavorecido3,
                premioDetalhes.CriadoPor,
                TipoOperacao = premioDetalhes.DescricaoTipoOperacao
            };

            RetornoWorkflow retorno;

            Processo processo = premioDetalhes.StatusPremioParceria == StatusPremioParceria.CANCELADO
                ? Processo.CANCELAMENTO_PREMIO
                : Processo.PREMIO_PARCERIA;

            retorno = workflow.EnviarParaAprovacao(
                new CadastroWorkflow(processo, oportunidadeBusca.EmpresaId, premioParceriaBusca.Id, User.ObterLogin(), User.ObterNome(), User.ObterEmail(), JsonConvert.SerializeObject(campos)));

            if (retorno == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Nenhuma resposta do serviço de Workflow");

            if (retorno.sucesso == false)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, retorno.mensagem);

            _workflowRepositorio.IncluirEnvioAprovacao(new EnvioWorkflow(oportunidadeBusca.Id, processo, retorno.protocolo, retorno.mensagem, User.ObterId()));

            _premioParceriaRepositorio.AtualizarStatusPremioParceria(StatusPremioParceria.EM_APROVACAO, premioParceriaBusca.Id);

            GravarLogAuditoria(TipoLogAuditoria.OUTROS, premioParceriaBusca, mensagem: "Prêmio Parceria enviado para Aprovação");

            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        [HttpPost]
        public ActionResult CancelarPremioParceria(int id)
        {
            var premioParceriaBusca = _premioParceriaRepositorio.ObterPremioParceriaPorId(id);

            if (premioParceriaBusca == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Prêmio Parceria não encontrado");

            var oportunidadeBusca = _oportunidadeRepositorio.ObterOportunidadePorId(premioParceriaBusca.OportunidadeId);

            if (oportunidadeBusca == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Oportunidade não encontrada");

            if (!User.IsInRole("OportunidadesPremios:CancelarPremioParceria"))
            {
                if (!_equipesService.ValidarEquipeOportunidade(oportunidadeBusca.Id))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Usuário não possui permissão para edição da Oportunidade (Equipes)");
                }
            }

            if (premioParceriaBusca.StatusPremioParceria != StatusPremioParceria.CADASTRADO)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Disponível apenas para Status igual a Cadastrado");

            _premioParceriaRepositorio.CancelarPremioParceria(premioParceriaBusca.Id);

            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        [HttpGet]
        public ActionResult ObterPremioParceriaAtivo(int oportunidadeId)
        {
            if (!User.IsInRole("OportunidadesPremios:RevisarPremioParceria"))
            {
                if (!_equipesService.ValidarEquipeOportunidade(oportunidadeId))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Usuário não possui permissão para edição da Oportunidade (Equipes)");
                }
            }

            var premios = _premioParceriaRepositorio.ObterPremiosParceriaPorOportunidade(oportunidadeId);

            var filtro = premios.Where(c => c.StatusPremioParceria == StatusPremioParceria.CADASTRADO).FirstOrDefault();

            return Json(new
            {
                filtro?.Id
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ObterFichaFaturamentoAtiva(int oportunidadeId)
        {
            if (!User.IsInRole("OportunidadesFichas:RevisarFichaFaturamento"))
            {
                if (!_equipesService.ValidarEquipeOportunidade(oportunidadeId))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Usuário não possui permissão para edição da Oportunidade (Equipes)");
                }
            }

            var fichas = _oportunidadeRepositorio.ObterFichasFaturamento(oportunidadeId);

            var filtro = fichas.Where(c => c.StatusFichaFaturamento == StatusFichaFaturamento.APROVADO);

            var multipos = false;
            dynamic fichasAtivas = null;

            if (filtro.Count() > 1)
            {
                multipos = true;

                fichasAtivas = filtro.Select(c => new
                {
                    Id = c.Id,
                    Cliente = $"{c.Cliente} ({c.ClienteDocumento})"
                });
            }
            else
            {
                var ficha = filtro.FirstOrDefault();

                if (ficha != null)
                {
                    fichasAtivas = new
                    {
                        Id = ficha.Id,
                        Cliente = $"{ficha.Cliente} ({ficha.ClienteDocumento})"
                    };
                }
            }

            return Json(new
            {
                fichasAtivas,
                multipos
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ExcluirAdendo(int id)
        {
            var adendoBusca = _oportunidadeRepositorio.ObterAdendoPorId(id);

            if (adendoBusca == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Adendo não encontrado ou já excluído");

            var oportunidadeBusca = _oportunidadeRepositorio.ObterOportunidadePorId(adendoBusca.OportunidadeId);

            if (oportunidadeBusca == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Oportunidade não encontrada");

            if (!User.IsInRole("OportunidadesAdendos:Excluir"))
            {
                if (!_equipesService.ValidarEquipeOportunidade(oportunidadeBusca.Id))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Usuário não possui permissão para edição da Oportunidade (Equipes)");
                }
            }

            if (adendoBusca.StatusAdendo != StatusAdendo.ABERTO && adendoBusca.StatusAdendo != StatusAdendo.REJEITADO)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"Exclusão permitida apenas para Status igual a {adendoBusca.StatusAdendo.ToName()}");

            if (adendoBusca.TipoAdendo == TipoAdendo.FORMA_PAGAMENTO)
            {
                var fichas = _oportunidadeRepositorio.ObterFichasFaturamento(oportunidadeBusca.Id);

                var fichasEmAndamento = fichas.Where(c => c.StatusFichaFaturamento == StatusFichaFaturamento.EM_ANDAMENTO || c.StatusFichaFaturamento == StatusFichaFaturamento.EM_APROVACAO).Any();

                if (fichasEmAndamento)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"Existem Fichas de Faturamento em andamento");
            }

            if (adendoBusca.TipoAdendo == TipoAdendo.INCLUSAO_SUB_CLIENTE)
            {
                if (oportunidadeBusca.OportunidadeProposta.FormaPagamento == FormaPagamento.FATURADO)
                {
                    var subClientesAdendo = _oportunidadeRepositorio.ObterAdendoSubClientesInclusao(adendoBusca.Id);

                    foreach (var subCliente in subClientesAdendo)
                    {
                        var fichaBusca = _oportunidadeRepositorio
                            .ObterFichaFaturamentoPorClienteCnpj(subCliente.Documento, oportunidadeBusca.Id);

                        if (fichaBusca != null)
                        {
                            _oportunidadeRepositorio.ExcluirFichaFaturamento(fichaBusca.Id);
                        }
                    }
                }
            }

            _oportunidadeRepositorio.ExcluirAdendo(id);

            GravarLogAuditoria(TipoLogAuditoria.DELETE, adendoBusca);

            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        [HttpGet]
        public ActionResult ObterDetalhesPremioParceria(int id)
        {
            var premioParceriaBusca = _premioParceriaRepositorio.ObterDetalhesPremioParceria(id);

            if (premioParceriaBusca == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Prêmio Parceria não encontrado");

            var modalidades = _premioParceriaRepositorio
                .ObterModalidades(premioParceriaBusca.Id)
                .Select(c => new
                {
                    Id = c.Modalidade.ToValue(),
                    Descricao = c.Modalidade.ToName()
                });

            return Json(new
            {
                premioParceriaBusca.Id,
                premioParceriaBusca.OportunidadeId,
                premioParceriaBusca.StatusOportunidade,
                premioParceriaBusca.ContatoId,
                premioParceriaBusca.DescricaoContato,
                premioParceriaBusca.PremioReferenciaId,
                premioParceriaBusca.StatusPremioParceria,
                premioParceriaBusca.TipoServicoPremioParceria,
                premioParceriaBusca.Favorecido1,
                premioParceriaBusca.DescricaoFavorecido1,
                premioParceriaBusca.DocumentoFavorecido1,
                premioParceriaBusca.Favorecido2,
                premioParceriaBusca.DescricaoFavorecido2,
                premioParceriaBusca.DocumentoFavorecido2,
                premioParceriaBusca.Favorecido3,
                premioParceriaBusca.DescricaoFavorecido3,
                premioParceriaBusca.DocumentoFavorecido3,
                premioParceriaBusca.Instrucao,
                premioParceriaBusca.DataCadastro,
                premioParceriaBusca.CriadoPor,
                premioParceriaBusca.EmailFavorecido1,
                premioParceriaBusca.EmailFavorecido2,
                premioParceriaBusca.EmailFavorecido3,
                premioParceriaBusca.AnexoId,
                premioParceriaBusca.Observacoes,
                premioParceriaBusca.UrlPremio,
                premioParceriaBusca.DataUrlPremio,
                ModalidadesSelecionadas = modalidades
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ClonarOportunidade([Bind(Include = "CloneOportunidadeSelecionada, CloneSubClientesSelecionados, CloneGruposCNPJSelecionados, CloneContaOportunidadeSelecionada, Descricao")] OportunidadesClonarPropostaViewModel viewModel)
        {
            if (viewModel.CloneOportunidadeSelecionada == 0)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Selecione uma Oportunidade");

            if (string.IsNullOrEmpty(viewModel.Descricao))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Informe a Descrição da nova Oportunidade");

            var oportunidade = _oportunidadeRepositorio.ObterOportunidadePorId(viewModel.CloneOportunidadeSelecionada);

            if (oportunidade == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Oportunidade não encontrada");

            if (oportunidade.OportunidadeProposta == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "A oportunidade selecionada não possui um Modelo vinculado");

            var novaOportunidade = _oportunidadeService.ClonarOportunidade(
                new ClonarOportunidadeDTO(
                    viewModel.Descricao,
                    viewModel.CloneOportunidadeSelecionada,
                    User.ObterId(),
                    oportunidade.OportunidadeProposta.FormaPagamento,
                    viewModel.CloneSubClientesSelecionados,
                    viewModel.CloneGruposCNPJSelecionados),
                viewModel.CloneContaOportunidadeSelecionada);

            oportunidade = _oportunidadeRepositorio.ObterOportunidadePorId(novaOportunidade);

            var detalhesOportunidade = _oportunidadeRepositorio.ObterDetalhesOportunidade(oportunidade.Id);

            GravarLogAuditoria(TipoLogAuditoria.INSERT, detalhesOportunidade);

            return Json(new
            {
                novaOportunidade
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ClonarProposta(int oportunidadeOrigem, int oportunidadeDestino)
        {
            if (oportunidadeOrigem == 0)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Selecione uma Oportunidade");

            if (oportunidadeDestino == 0)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Selecione uma Oportunidade");

            if (oportunidadeOrigem == oportunidadeDestino)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"Você selecionou a mesma Proposta para clonagem. Escolha uma diferente.");

            var oportunidadeOrigemBusca = _oportunidadeRepositorio.ObterOportunidadePorId(oportunidadeOrigem);

            if (oportunidadeOrigemBusca == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Oportunidade não encontrada");

            var oportunidadeDestinoBusca = _oportunidadeRepositorio.ObterOportunidadePorId(oportunidadeDestino);

            if (oportunidadeDestinoBusca == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Oportunidade não encontrada");

            if (oportunidadeOrigemBusca.OportunidadeProposta.ModeloId != oportunidadeDestinoBusca.OportunidadeProposta.ModeloId)
            {
                var modeloBusca = _modeloRepositorio.ObterModeloPorId(oportunidadeDestinoBusca.OportunidadeProposta.ModeloId);

                if (modeloBusca == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Modelo não encontrado");

                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"O Modelo da Oportunidade deve ser igual a {modeloBusca.Descricao}");
            }

            _oportunidadeService.ClonarProposta(oportunidadeOrigem, oportunidadeDestino, oportunidadeDestinoBusca.OportunidadeProposta.FormaPagamento);

            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        public ActionResult RetornarErros()
        {
            Response.StatusCode = (int)HttpStatusCode.BadRequest;

            return Json(new
            {
                erros = ModelState.Values.SelectMany(v => v.Errors)
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PdfCancelamento(int id)
        {
            var oportunidade = _oportunidadeRepositorio.ObterOportunidadePorId(id);

            if (oportunidade == null)
                throw new Exception("Oportunidade não encontrada");

            var conta = _contaRepositorio.ObterContaPorId(oportunidade.ContaId);

            if (conta == null)
                throw new Exception("A oportunidade não possui uma Conta vinculada");

            if (conta.CidadeId.HasValue)
                conta.Cidade = _cidadeRepositorio.ObterCidadePorId(conta.CidadeId.Value);

            var clientesGrupo = _oportunidadeRepositorio.ObterClientesGrupoCNPJ(oportunidade.Id);

            var cancelamento = new CancelamentoOportunidadeDTO
            {
                IdentificacaoOportunidade = oportunidade.Identificacao,
                ContaBairro = conta.Bairro,
                ContaCEP = conta.CEP,
                ContaCidade = conta.Cidade?.Descricao,
                ContaDescricao = conta.Descricao,
                ContaDocumento = conta.Documento,
                ContaEstado = conta.Estado.ToString(),
                ContaLogradouro = conta.Logradouro,
                ContaNumero = conta.Numero,
                DataCancelamento = oportunidade.DataCancelamento,
                ClientesGrupo = clientesGrupo
            };

            var nomeArquivo = $"Cancelamento-Proposta-{oportunidade.Identificacao}.pdf";

            _anexoRepositorio.ExcluirAnexosOportunidadePorTipo(oportunidade.Id, TipoAnexo.CANCELAMENTO);

            SalvarAnexo(TipoArquivo.PDF, TipoAnexo.CANCELAMENTO, oportunidade, cancelamento, "PdfCancelamento", nomeArquivo);

            return new PdfActionResult(cancelamento);
        }

        public ActionResult AtualizarDataCancelamento(int id, string dataCancelamento)
        {
            var oportunidade = _oportunidadeRepositorio.ObterOportunidadePorId(id);

            if (oportunidade == null)
                throw new Exception("Oportunidade não encontrada");

            if (DateTimeHelpers.IsDate(dataCancelamento))
            {
                if (Convert.ToDateTime(dataCancelamento) != oportunidade.DataCancelamento)
                {
                    oportunidade.DataCancelamento = Convert.ToDateTime(dataCancelamento);
                    _oportunidadeRepositorio.AtualizarDataCancelamento(oportunidade);
                }
            }

            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        private void ValidarRegrasRevisao(Oportunidade oportunidade)
        {
            var revisaoId = oportunidade.RevisaoId ?? 0;

            if (revisaoId == 0)
                return;

            var oportunidadeRevisada = _oportunidadeRepositorio.ObterOportunidadePorId(revisaoId);

            if (oportunidade.OportunidadeProposta.Acordo == false)
            {
                var oportunidadesRevisadas = _oportunidadeRepositorio
                    .ObterOportunidadePorRevisaoId(revisaoId)
                    .Where(c => c.Id != oportunidade.Id);

                if (oportunidadesRevisadas.Where(c => c.SucessoNegociacao != SucessoNegociacao.PERDIDO).Any())
                {
                    throw new Exception($"Já existe uma revisão para a oportunidade revisada {oportunidade.Identificacao}, que são: {string.Join(",", oportunidadesRevisadas.Select(c => c.Identificacao))}");
                }
            }

            if (oportunidade.ContaId != oportunidadeRevisada.ContaId)
            {
                throw new Exception($"Cliente da oportunidade não corresponder ao da revisada");
            }

            if (oportunidade.Segmento != oportunidadeRevisada.Segmento)
            {
                var segmentoAgente = (oportunidade.Segmento == Segmento.FREIGHT_FORWARDER && oportunidadeRevisada.Segmento == Segmento.AGENTE) || (oportunidade.Segmento == Segmento.AGENTE && oportunidadeRevisada.Segmento == Segmento.FREIGHT_FORWARDER);

                if (!segmentoAgente)
                    throw new Exception($"Segmento não corresponde ao segmento da oportunidade revisada");
            }

            var subClientesOportunidade = _oportunidadeRepositorio.ObterSubClientes(oportunidade.Id);
            var subClientesOportunidadeRevisada = _oportunidadeRepositorio.ObterSubClientes(oportunidadeRevisada.Id);

            if (subClientesOportunidade.Any() && subClientesOportunidadeRevisada.Any())
            {
                var segmentosSubClientesOportunidade = subClientesOportunidade
                    .Select(c => c.Segmento).Distinct();

                var segmentosSubClientesOportunidadeRevisada = subClientesOportunidadeRevisada
                    .Select(c => c.Segmento).Distinct();

                var diferencaDeSegmentos =
                    segmentosSubClientesOportunidade.Except(segmentosSubClientesOportunidadeRevisada);

                if (diferencaDeSegmentos.Any())
                {
                    throw new Exception($"Oportunidade não possui os mesmos segmentos de sub clientes da oportunidade revisada ou vice versa.");
                }

                var naoPossuiSubCliente = false;
                var segmentoQueNaoPossuiSubCliente = SegmentoSubCliente.IMPORTADOR;

                foreach (var segmento in subClientesOportunidadeRevisada.Select(c => c.Segmento).Distinct())
                {
                    var subClientes = subClientesOportunidadeRevisada.Where(c => c.Segmento == segmento);

                    foreach (var subCliente in subClientes)
                    {
                        if (subClientesOportunidade.Any(c => c.Segmento == segmento))
                            break;
                        else
                        {
                            naoPossuiSubCliente = true;
                            segmentoQueNaoPossuiSubCliente = segmento;

                            break;
                        }
                    }
                }

                if (naoPossuiSubCliente)
                {
                    throw new Exception($"Oportunidade não possui cliente da oportunidade revisada no segmento {segmentoQueNaoPossuiSubCliente.ToName()}.");
                }
            }

            if (oportunidade.OportunidadeProposta.TipoOperacao != oportunidadeRevisada.OportunidadeProposta.TipoOperacao)
            {
                throw new Exception($"Tipo Operação não corresponde ao tipo operação da oportunidade revisada");
            }

            if (oportunidade.EmpresaId != oportunidadeRevisada.EmpresaId)
            {
                throw new Exception($"Empresa não corresponde a empresa da oportunidade revisada");
            }

            if (!oportunidade.OportunidadeProposta.DataInicio.HasValue || !oportunidade.OportunidadeProposta.DataTermino.HasValue)
            {
                throw new Exception($"A proposta não possui data de início / término");
            }

            if (oportunidade.OportunidadeProposta.DataInicio.Value <= oportunidadeRevisada.OportunidadeProposta.DataInicio.Value)
            {
                throw new Exception($"Oportunidade possui a data início menor ou igual a oportunidade revisada");
            }

            if ((oportunidade.OportunidadeProposta.DataInicio.Value - oportunidadeRevisada.OportunidadeProposta.DataInicio.Value).Days == 1)
            {
                throw new Exception($"Oportunidade possui data início D+1 da oportunidade revisada");
            }

            if ((oportunidade.OportunidadeProposta.DataInicio.Value - oportunidadeRevisada.OportunidadeProposta.DataTermino.Value).Days >= 2)
            {
                throw new Exception($"Oportunidade possui data inicio maior que a data fim da oportunidade revisada");
            }

            if (oportunidade.OportunidadeProposta.Acordo != oportunidadeRevisada.OportunidadeProposta.Acordo)
            {
                throw new Exception($"Oportunidade não possui indicativo Acordo correspondente a oportunidade revisada");
            }

            if (oportunidade.OportunidadeProposta.HubPort != oportunidadeRevisada.OportunidadeProposta.HubPort)
            {
                throw new Exception($"Oportunidade não possui indicativo HubPort correspondente a oportunidade revisada");
            }

            if (oportunidade.OportunidadeProposta.CobrancaEspecial != oportunidadeRevisada.OportunidadeProposta.CobrancaEspecial)
            {
                throw new Exception($"Oportunidade não possui indicativo Cobrança Especial correspondente a oportunidade revisada");
            }
        }

        [UsuarioExternoFilter]
        public ActionResult GerarProposta(int id)
        {
            string mensagem = string.Empty;

            var oportunidadeBusca = _oportunidadeRepositorio.ObterOportunidadePorId(id);

            if (oportunidadeBusca == null)
                throw new Exception("Oportunidade não encontrada");

            if (oportunidadeBusca.OportunidadeProposta == null)
                throw new Exception("Nenhuma Proposta encontrada na Oportunidade");

            if (!User.IsInRole("OportunidadesProposta:GerarProposta"))
            {
                if (!_equipesService.ValidarEquipeOportunidade(oportunidadeBusca.Id))
                {
                    throw new Exception("Usuário não possui permissão para edição da Oportunidade (Equipes)");
                }
            }

            try
            {
                ValidarRegrasRevisao(oportunidadeBusca);
            }
            catch (Exception ex)
            {
                mensagem = ex.Message;
            }

            var parametros = _parametrosRepositorio.ObterParametros();

            if (parametros.ValidaConcomitancia)
            {
                var concomitancia = ValidarConcomitancia(oportunidadeBusca, parametros.CriarAdendoExclusaoCliente);

                if (concomitancia.Concomitante)
                {
                    return Json(new
                    {
                        Existe = true,
                        Mensagem = concomitancia.Mensagem,
                        Bloqueia = parametros.CriarAdendoExclusaoCliente == false,
                        RedirectUrl = string.Empty
                    }, JsonRequestBehavior.AllowGet);
                }
            }

            AtualizarProposta(oportunidadeBusca);

            AtualizarReferenciaOportunidade(oportunidadeBusca);

            ExcluirArquivosPropostaTemporarios();

            new PropostaWordHelper(oportunidadeBusca)
                .GerarPropostaWord();

            string url = Url.Action(nameof(PropostaOnline), "Oportunidades", new RouteValueDictionary(new { oportunidadeBusca.Id }), "http");

            new PropostaPdfHelper(_anexoRepositorio, oportunidadeBusca, false, url)
                .GerarPropostaPdf();

            var detalhesProposta = _oportunidadeRepositorio.ObterDetalhesProposta(oportunidadeBusca.Id);

            GravarLogAuditoria(TipoLogAuditoria.UPDATE, detalhesProposta, oportunidadeBusca.Id, "Proposta Gerada");

            return PartialView("_DownloadProposta", new OportunidadeDownloadPropostaViewModel
            {
                OportunidadeId = oportunidadeBusca.Id,
                Mensagem = mensagem
            });
        }

        private void AtualizarProposta(Oportunidade oportunidadeBusca)
        {
            oportunidadeBusca.OportunidadeProposta.OportunidadeId = oportunidadeBusca.Id;

            if (!oportunidadeBusca.OportunidadeProposta.DataInicio.HasValue)
            {
                oportunidadeBusca.OportunidadeProposta.DataInicio = DateTime.Now;
            }

            _oportunidadeRepositorio.AtualizarProposta(oportunidadeBusca.OportunidadeProposta);
        }

        private void AtualizarReferenciaOportunidade(Oportunidade oportunidadeBusca)
        {
            var referencias = Regex.Matches(oportunidadeBusca?.Observacao ?? string.Empty, @"(?<=\!)[^}]*(?=\!)");

            if (referencias.Count > 0)
            {
                var capture = referencias.Cast<Match>().FirstOrDefault();
                oportunidadeBusca.Referencia = capture.Value;
            }
            else
            {
                oportunidadeBusca.Referencia = string.Empty;
            }

            _oportunidadeRepositorio.AtualizarReferencia(oportunidadeBusca.Referencia, oportunidadeBusca.Id);
        }

        public void ExcluirArquivosPropostaTemporarios()
        {
            var dataArquivo = DateTime.Now.AddMinutes(-30);

            try
            {
                var caminho = Server.MapPath("~/App_Data/");

                var arquivos = new DirectoryInfo(caminho).GetFiles();

                var arquivosParaExclusao = arquivos
                    .Where(a => a.LastWriteTime <= dataArquivo)
                    .ToList();

                if (arquivosParaExclusao.Count() == 0)
                    return;

                foreach (var arquivo in arquivosParaExclusao)
                    System.IO.File.Delete(arquivo.FullName);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
            }
        }

        public ActionResult PropostaPdf(int id)
        {
            var oportunidadeBusca = _oportunidadeRepositorio.ObterOportunidadePorId(id);

            if (oportunidadeBusca == null)
                throw new Exception("Oportunidade não encontrada");

            if (oportunidadeBusca.OportunidadeProposta == null)
                throw new Exception("Nenhuma Proposta encontrada na Oportunidade");

            if (!User.IsInRole("OportunidadesProposta:GerarProposta"))
            {
                if (!_equipesService.ValidarEquipeOportunidade(oportunidadeBusca.Id))
                {
                    throw new Exception("Usuário não possui permissão para edição da Oportunidade (Equipes)");
                }
            }

            var tabela = MontaLayout(oportunidadeBusca.Id);

            tabela.Identificacao = oportunidadeBusca.Identificacao;
            tabela.TipoServico = oportunidadeBusca.TipoServico;
            tabela.Vigencia = DateTime.Now;

            oportunidadeBusca.OportunidadeProposta.OportunidadeId = oportunidadeBusca.Id;
            oportunidadeBusca.OportunidadeProposta.DataInicio = DateTime.Now;

            var referencias = Regex.Matches(oportunidadeBusca.Observacao, @"(?<=\!)[^}]*(?=\!)");

            if (referencias.Count > 0)
            {
                var capture = referencias.Cast<Match>().FirstOrDefault();
                tabela.Referencia = capture.Value;

                _oportunidadeRepositorio.AtualizarReferencia(tabela.Referencia, oportunidadeBusca.Id);
            }

            _oportunidadeRepositorio.AtualizarProposta(oportunidadeBusca.OportunidadeProposta);

            var detalhesProposta = _oportunidadeRepositorio.ObterDetalhesProposta(oportunidadeBusca.Id);
            GravarLogAuditoria(TipoLogAuditoria.UPDATE, detalhesProposta, oportunidadeBusca.Id, "Proposta Gerada");

            tabela.Identificacao = oportunidadeBusca.Identificacao;
            tabela.Referencia = oportunidadeBusca.Referencia;
            tabela.TipoServico = oportunidadeBusca.TipoServico;
            tabela.DataInicio = oportunidadeBusca.OportunidadeProposta?.DataInicio ?? DateTime.Now;
            tabela.Vigencia = DateTime.Now;

            return new PdfActionResult(tabela);
        }

        [HttpGet]
        public ActionResult ConfirmarGeracaoProposta()
        {
            return PartialView("_ConfirmarGeracaoProposta", new OportunidadesViewModel());
        }

        [HttpGet]
        public FileResult DownloadArquivoProposta(int id, TipoArquivo formato)
        {
            var oportunidadeBusca = _oportunidadeRepositorio.ObterOportunidadePorId(id);

            if (oportunidadeBusca == null)
                throw new Exception("Oportunidade não encontrada");

            if (!User.IsInRole("Administrador"))
            {
                if (formato == TipoArquivo.WORD)
                {
                    if (!User.IsInRole("OportunidadesProposta:btnGerarPropostaWord"))
                    {
                        throw new Exception("Você não tem permissão para gerar propostas no formato Word");
                    }
                }

                if (formato == TipoArquivo.PDF)
                {
                    if (!User.IsInRole("OportunidadesProposta:btnGerarPropostaPdf"))
                    {
                        throw new Exception("Você não tem permissão para gerar propostas no formato Pdf");
                    }
                }
            }

            string arquivo = string.Empty;
            string caminho = string.Empty;
            string contentType = string.Empty;

            arquivo = $"{oportunidadeBusca.Descricao}-{oportunidadeBusca.Identificacao}";

            if (formato == TipoArquivo.PDF)
            {
                arquivo = $"{arquivo.CorrigirNomeArquivo()}.pdf";
                contentType = "application/pdf";
            }

            if (formato == TipoArquivo.WORD)
            {
                arquivo = $"{arquivo.CorrigirNomeArquivo()}.doc";
                contentType = "application/ms-word";
            }

            caminho = Server.MapPath($"~/App_Data/{arquivo}");

            var bytes = System.IO.File.ReadAllBytes(caminho);

            return File(bytes, contentType, arquivo);
        }

        private static DateTime AtualizarDataTermino(Oportunidade oportunidadeBusca, DateTime dataInicio, TipoValidade tipoValidade, int validade)
        {
            DateTime dataTermino;

            if (tipoValidade == TipoValidade.DIAS)
            {
                if (validade == 365)
                {
                    dataTermino = dataInicio.AddYears(1).AddDays(-1);
                }
                else
                {
                    dataTermino = dataInicio.AddDays(validade);
                }
            }
            else if (tipoValidade == TipoValidade.MESES)
            {
                if (validade == 12)
                {
                    dataTermino = dataInicio.AddYears(1).AddDays(-1);
                }
                else
                {
                    dataTermino = dataInicio.AddMonths(validade);
                }
            }
            else
            {
                dataTermino = dataInicio.AddYears(validade).AddDays(-1);
            }

            return dataTermino;
        }

        private void SalvarAnexo(TipoArquivo tipoArquivo, TipoAnexo tipoAnexo, Oportunidade oportunidade, object objeto, string view, string nomeArquivo, int versao = 0)
        {
            byte[] pdfBytes = ControllerContext.GeneratePdf(objeto, view);

            if (pdfBytes != null && pdfBytes.Length > 0)
            {
                var token = Sharepoint.Services.Autenticador.Autenticar();

                if (token == null)
                    throw new HttpException(404, "Não foi possível se autenticar no serviço de Anexos");

                var dados = new DadosArquivoUpload
                {
                    Name = nomeArquivo,
                    Extension = Path.GetExtension(nomeArquivo),
                    System = 3,
                    DataArray = Convert.ToBase64String(pdfBytes)
                };

                var retornoUpload = new Sharepoint.Services.AnexosService(token)
                    .EnviarArquivo(dados);

                _anexoRepositorio.IncluirAnexo(
                    new Anexo
                    {
                        IdProcesso = oportunidade.Id,
                        Arquivo = nomeArquivo,
                        CriadoPor = User.ObterId(),
                        TipoAnexo = tipoAnexo,
                        TipoDoc = 1,
                        IdArquivo = Converters.GuidToRaw(retornoUpload.Arquivo.id),
                        Versao = versao
                    });
            }
        }

        [AllowAnonymous]
        public ActionResult PropostaOnline(int id)
        {
            var oportunidadeBusca = _oportunidadeRepositorio.ObterOportunidadePorId(id);

            if (oportunidadeBusca == null)
                throw new Exception("Oportunidade não encontrada");

            if (oportunidadeBusca.OportunidadeProposta == null)
                throw new Exception("Nenhuma Proposta encontrada na Oportunidade");

            var tabela = MontaLayout(oportunidadeBusca.Id);

            tabela.Identificacao = oportunidadeBusca.Identificacao;
            tabela.TipoServico = oportunidadeBusca.TipoServico;
            tabela.DataInicio = oportunidadeBusca.OportunidadeProposta?.DataInicio ?? DateTime.Now;
            tabela.Vigencia = DateTime.Now;

            return View(tabela);
        }

        public ActionResult PropostaComVinculoTabela(int id)
        {
            var oportunidadeBusca = _oportunidadeRepositorio.ObterOportunidadePorId(id);

            if (oportunidadeBusca == null)
                throw new Exception("Oportunidade não encontrada");

            if (oportunidadeBusca.OportunidadeProposta == null)
                throw new Exception("Nenhuma Proposta encontrada na Oportunidade");

            return View(new Proposta
            {
                Identificacao = oportunidadeBusca.Identificacao,
                TipoServico = oportunidadeBusca.TipoServico,
                DataInicio = oportunidadeBusca.OportunidadeProposta?.DataInicio ?? DateTime.Now,
                Vigencia = DateTime.Now,
                IdTabelaVinculada = oportunidadeBusca.OportunidadeProposta?.TabelaReferencia
            });
        }

        public ActionResult ValidarInclusaoSubCliente(int oportunidadeId, SegmentoSubCliente segmento, int subClienteId)
        {
            var oportunidadeBusca = _oportunidadeRepositorio.ObterOportunidadePorId(oportunidadeId);

            if (oportunidadeBusca == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Oportunidade não encontrada");

            var subClienteInclusao = _contaRepositorio.ObterContaPorId(subClienteId);

            if (subClienteInclusao == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Sub Cliente não encontrado");

            var subClientesOportunidade = _oportunidadeRepositorio.ObterSubClientes(oportunidadeId);

            string mensagem = "Inclusão não permitida, pois este segmento não consta na proposta";

            if (oportunidadeBusca.Segmento != Segmento.DESPACHANTE && oportunidadeBusca.TipoDeProposta != TipoDeProposta.GERAL)
            {
                if (subClientesOportunidade.Any())
                {
                    if (!subClientesOportunidade.Any(c => c.Segmento == segmento))
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, mensagem);
                }
                else
                {
                    switch (segmento)
                    {
                        case SegmentoSubCliente.IMPORTADOR:

                            if (oportunidadeBusca.Segmento != Segmento.IMPORTADOR)
                                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, mensagem);

                            break;
                        case SegmentoSubCliente.DESPACHANTE:

                            if (oportunidadeBusca.Segmento != Segmento.DESPACHANTE)
                                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, mensagem);

                            break;
                        case SegmentoSubCliente.COLOADER:
                        case SegmentoSubCliente.CO_COLOADER1:
                        case SegmentoSubCliente.CO_COLOADER2:

                            if (oportunidadeBusca.Segmento != Segmento.COLOADER)
                                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, mensagem);

                            break;
                    }
                }
            }

            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        [HttpPost]
        [CanActivateUsuarioIntegracaoAdendo]
        public ActionResult CadastrarAdendos([Bind(Include = "AdendoOportunidadeId, AdendoVendedorId, TipoAdendo, StatusAdendo, AdendoFormaPagamento, AdendoSegmento, AdendoContaId, AdendoSubClienteId, AdendoClienteGrupoCNPJId, AdendosClientesGrupoInclusao, AdendosClientesGrupoExclusao, AdendosSubClientesInclusao, AdendosSubClientesExclusao")] OportunidadesAdendosViewModel viewModel, HttpPostedFileBase anexoFormaPagamento, HttpPostedFileBase anexoExclusaoSubClientes, HttpPostedFileBase anexoExclusaoGrupoCNPJ)
        {
            bool habilitaAbaFichas = false;

            if (viewModel.AdendoOportunidadeId == 0)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Oportunidade não informada");

            var oportunidadeBusca = _oportunidadeRepositorio.ObterOportunidadePorId(viewModel.AdendoOportunidadeId);

            if (oportunidadeBusca == null)
                RegistroNaoEncontrado();

            if (!User.IsInRole("OportunidadesAdendos:Cadastrar"))
            {
                if (!_equipesService.ValidarEquipeOportunidade(oportunidadeBusca.Id))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Usuário não possui permissão para edição da Oportunidade (Equipes)");
                }
            }

            if (oportunidadeBusca.StatusOportunidade != StatusOportunidade.ATIVA && oportunidadeBusca.StatusOportunidade != StatusOportunidade.REVISADA)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Disponível apenas se o Status da Oportunidade for igual a Ativa / Revisada");

            if (viewModel.AdendoVendedorId > 0)
            {
                if (viewModel.AdendoVendedorId == oportunidadeBusca.OportunidadeProposta?.VendedorId)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "O Vendedor informado é o mesmo da Oportunidade.");
            }

            if (viewModel.AdendoFormaPagamento > 0)
            {
                if (viewModel.AdendoFormaPagamento == oportunidadeBusca.OportunidadeProposta?.FormaPagamento)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "A Forma de Pagamento informada é a mesma da Oportunidade");
            }

            if (viewModel.TipoAdendo == TipoAdendo.FORMA_PAGAMENTO)
            {
                var adendoBusca = _oportunidadeRepositorio.ObterAdendos(oportunidadeBusca.Id)
                    .Where(c => c.TipoAdendo == TipoAdendo.FORMA_PAGAMENTO && (c.StatusAdendo == StatusAdendo.ABERTO || c.StatusAdendo == StatusAdendo.ENVIADO || c.StatusAdendo == StatusAdendo.REJEITADO))
                    .FirstOrDefault();

                if (adendoBusca != null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"Já existe um Adendo de {adendoBusca.TipoAdendo.ToName()} com status {adendoBusca.StatusAdendo.ToName()}");
            }

            //if (viewModel.AdendoFormaPagamento == FormaPagamento.FATURADO && (anexoFormaPagamento == null || anexoFormaPagamento.ContentLength == 0))
            //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "É necessário anexar o arquivo da ficha");

            var adendo = new OportunidadeAdendo
            {
                OportunidadeId = oportunidadeBusca.Id,
                TipoAdendo = viewModel.TipoAdendo,
                StatusAdendo = StatusAdendo.ABERTO,
                CriadoPor = User.ObterId(),
                DataCadastro = DateTime.Now
            };

            if (!Validar(adendo))
                return RetornarErros();

            var fichasFaturamento = _oportunidadeRepositorio.ObterFichasFaturamento(oportunidadeBusca.Id);

            if (viewModel.TipoAdendo == TipoAdendo.INCLUSAO_SUB_CLIENTE)
            {
                if (oportunidadeBusca.OportunidadeProposta.FormaPagamento == FormaPagamento.FATURADO && oportunidadeBusca.StatusOportunidade != StatusOportunidade.CANCELADA)
                {
                    var existeFichaGeral = fichasFaturamento.Any(c => c.ContaId == oportunidadeBusca.ContaId && c.StatusFichaFaturamento != StatusFichaFaturamento.REVISADA);

                    if (existeFichaGeral == false)
                    {
                        List<ClientePropostaDTO> subClientes = new List<ClientePropostaDTO>();

                        foreach (var subCliente in viewModel.AdendosSubClientesInclusao)
                        {
                            var registro = subCliente.Split(':');
                            var subClienteId = registro[0].ToInt();
                            var segmentoSubCliente = (SegmentoSubCliente)registro[1].ToInt();

                            if (subClienteId > 0)
                            {
                                var subClienteBusca = _contaRepositorio.ObterContaPorId(subClienteId);

                                if (subClienteBusca != null)
                                {
                                    subClientes.Add(new ClientePropostaDTO
                                    {
                                        ContaId = subClienteBusca.Id,
                                        Descricao = subClienteBusca.Descricao,
                                        Documento = subClienteBusca.Documento,
                                        Segmento = segmentoSubCliente,
                                    });
                                }
                            }
                        }

                        ClientePropostaDTO clienteSemFicha = null;

                        foreach (var subCliente in subClientes)
                        {
                            if (subCliente.Segmento == SegmentoSubCliente.IMPORTADOR)
                            {
                                var clienteTemFicha = fichasFaturamento.Where(c => c.ContaId == subCliente.ContaId).Any();

                                if (clienteTemFicha == false)
                                {
                                    clienteSemFicha = subCliente;

                                    break;
                                }
                            }
                        }

                        if (clienteSemFicha != null)
                        {
                            Response.StatusCode = 400;

                            return Json(new
                            {
                                AdendoId = adendo.Id,
                                OportunidadeId = oportunidadeBusca.Id,
                                SubClienteId = clienteSemFicha.ContaId,
                                DescricaoSubCliente = clienteSemFicha.Descricao,
                                DocumentoSubCliente = clienteSemFicha.Documento,
                                BloqueiaInclusaoSubCliente = true
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }

                foreach (var subCliente in viewModel.AdendosSubClientesInclusao)
                {
                    var registro = subCliente.Split(':');
                    var subClienteId = registro[0].ToInt();

                    if (subClienteId > 0)
                    {
                        var existeSubCliente = _oportunidadeRepositorio
                            .ObterSubClientes(viewModel.AdendoOportunidadeId)
                            .Any(c => c.ContaId == subClienteId);

                        var conta = _contaRepositorio.ObterContaPorId(subClienteId);

                        if (conta == null)
                            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"Sub Cliente inexistente");

                        if (existeSubCliente)
                            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"O Sub Cliente {conta.Descricao} já existe na Oportunidade");

                        if (_oportunidadeRepositorio.ExisteAdendoSubCliente(oportunidadeBusca.Id, subClienteId, AdendoAcao.INCLUSAO))
                            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"Já existe um Adendo para o Sub Cliente {conta.Descricao} cadastrado ou pendente de aprovação");
                    }
                }
            }

            if (viewModel.TipoAdendo == TipoAdendo.EXCLUSAO_SUB_CLIENTE)
            {
                foreach (var subCliente in viewModel.AdendosSubClientesExclusao)
                {
                    var registro = subCliente.Split(':');
                    var subClienteId = registro[0].ToInt();

                    var subClienteBusca = _oportunidadeRepositorio.ObterSubClientePorContaEOportunidade(subClienteId, oportunidadeBusca.Id);

                    if (subClienteBusca == null)
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"Sub Cliente inexistente");

                    var outrosSubClientesMesmoSegmento = _oportunidadeRepositorio.ObterSubClientes(oportunidadeBusca.Id)
                        .Where(c => c.Segmento == subClienteBusca.Segmento && c.Id != subClienteBusca.Id)
                        .Any();

                    if (outrosSubClientesMesmoSegmento == false)
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"O Sub Cliente {subClienteBusca.Descricao} ({subClienteBusca.Documento}) é o único {subClienteBusca.Segmento.ToName()} da Oportunidade. Exclusão não permitida!");

                    if (_oportunidadeRepositorio.ExisteAdendoSubCliente(oportunidadeBusca.Id, subClienteId, AdendoAcao.EXCLUSAO))
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"Já existe um Adendo de exclusão para o Sub Cliente {subClienteBusca.Descricao} ({subClienteBusca.Documento}) cadastrado ou pendente de aprovação");
                }
            }

            if (viewModel.TipoAdendo == TipoAdendo.INCLUSAO_GRUPO_CNPJ)
            {
                foreach (var clienteGrupo in viewModel.AdendosClientesGrupoInclusao)
                {
                    var existeClienteGrupo = _oportunidadeRepositorio
                        .ObterClientesGrupoCNPJ(viewModel.AdendoOportunidadeId)
                        .Any(c => c.ContaId == clienteGrupo);

                    var conta = _contaRepositorio.ObterContaPorId(clienteGrupo);

                    if (conta == null)
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"Cliente inexistente");

                    if (existeClienteGrupo)
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"O Cliente {conta.Descricao} já existe na Oportunidade");

                    if (_oportunidadeRepositorio.ExisteAdendoClienteGrupoCNPJ(oportunidadeBusca.Id, clienteGrupo, AdendoAcao.INCLUSAO))
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"Já existe um Adendo para o Cliente {conta.Descricao} cadastrado ou pendente de aprovação");
                }
            }

            if (viewModel.TipoAdendo == TipoAdendo.EXCLUSAO_GRUPO_CNPJ)
            {
                foreach (var clienteGrupo in viewModel.AdendosClientesGrupoExclusao)
                {
                    var conta = _contaRepositorio.ObterContaPorId(clienteGrupo);

                    if (conta == null)
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"Cliente inexistente");

                    if (_oportunidadeRepositorio.ExisteAdendoSubCliente(oportunidadeBusca.Id, clienteGrupo, AdendoAcao.EXCLUSAO))
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"Já existe um Adendo de exclusão para o Cliente {conta.Descricao} cadastrado ou pendente de aprovação");
                }
            }

            var adendoId = _oportunidadeRepositorio.IncluirAdendo(adendo);

            if (viewModel.TipoAdendo == TipoAdendo.ALTERACAO_VENDEDOR)
            {
                var adendoVendedor = new OportunidadeAdendoVendedor
                {
                    AdendoId = adendoId,
                    VendedorId = viewModel.AdendoVendedorId
                };

                if (!Validar(adendoVendedor))
                {
                    _oportunidadeRepositorio.ExcluirAdendo(adendoId);
                    return RetornarErros();
                }

                adendoVendedor.Id = _oportunidadeRepositorio.IncluirAdendoVendedor(adendoVendedor);
            }

            if (viewModel.TipoAdendo == TipoAdendo.FORMA_PAGAMENTO)
            {
                var adendoFormaPagamento = new OportunidadeAdendoFormaPagamento
                {
                    AdendoId = adendoId,
                    FormaPagamento = viewModel.AdendoFormaPagamento
                };

                if (!Validar(adendoFormaPagamento))
                {
                    _oportunidadeRepositorio.ExcluirAdendo(adendoId);
                    return RetornarErros();
                }

                adendoFormaPagamento.AnexoId = IncluirAnexo(oportunidadeBusca.Id, TipoAnexo.OUTROS, anexoFormaPagamento);

                adendoFormaPagamento.Id = _oportunidadeRepositorio.IncluirAdendoFormaPagamento(adendoFormaPagamento);

                habilitaAbaFichas = adendoFormaPagamento.FormaPagamento == FormaPagamento.FATURADO;
            }

            if (viewModel.TipoAdendo == TipoAdendo.INCLUSAO_GRUPO_CNPJ)
            {
                var adendoGrupoCNPJ = new OportunidadeAdendoGrupoCNPJ
                {
                    AdendoId = adendoId,
                    Acao = AdendoAcao.INCLUSAO
                };

                adendoGrupoCNPJ.AdicionarClientesGrupoCNPJ(
                    viewModel.AdendosClientesGrupoInclusao.ToList());

                if (!Validar(adendoGrupoCNPJ))
                {
                    _oportunidadeRepositorio.ExcluirAdendo(adendoId);
                    return RetornarErros();
                }

                foreach (var grupo in adendoGrupoCNPJ.Clientes)
                {
                    adendoGrupoCNPJ.ClienteId = grupo;

                    adendoGrupoCNPJ.Id = _oportunidadeRepositorio.IncluirAdendoGruposCNPJ(adendoGrupoCNPJ);
                }
            }

            if (viewModel.TipoAdendo == TipoAdendo.EXCLUSAO_GRUPO_CNPJ)
            {
                var adendoGrupoCNPJ = new OportunidadeAdendoGrupoCNPJ
                {
                    AdendoId = adendoId,
                    Acao = AdendoAcao.EXCLUSAO
                };

                adendoGrupoCNPJ.AdicionarClientesGrupoCNPJ(
                    viewModel.AdendosClientesGrupoExclusao.ToList());

                if (!Validar(adendoGrupoCNPJ))
                {
                    _oportunidadeRepositorio.ExcluirAdendo(adendoId);
                    return RetornarErros();
                }

                adendoGrupoCNPJ.AnexoId = IncluirAnexo(oportunidadeBusca.Id, TipoAnexo.OUTROS, anexoExclusaoGrupoCNPJ);

                foreach (var grupo in adendoGrupoCNPJ.Clientes)
                {
                    adendoGrupoCNPJ.ClienteId = grupo;
                    _oportunidadeRepositorio.IncluirAdendoGruposCNPJ(adendoGrupoCNPJ);
                }
            }

            if (viewModel.TipoAdendo == TipoAdendo.INCLUSAO_SUB_CLIENTE)
            {
                var adendoSubCliente = new OportunidadeAdendoSubCliente
                {
                    AdendoId = adendoId,
                    Acao = AdendoAcao.INCLUSAO
                };

                adendoSubCliente.AdicionarSubClientes(
                    viewModel.AdendosSubClientesInclusao.ToList());

                if (!Validar(adendoSubCliente))
                {
                    _oportunidadeRepositorio.ExcluirAdendo(adendoId);
                    return RetornarErros();
                }

                foreach (var cliente in adendoSubCliente.Clientes)
                {
                    var registro = cliente.Split(':');

                    if (registro.Length == 2)
                    {
                        var subClienteId = registro[0].ToInt();
                        var segmento = registro[1].ToInt();

                        adendoSubCliente.ClienteId = subClienteId;
                        adendoSubCliente.Segmento = segmento;

                        adendoSubCliente.Id = _oportunidadeRepositorio.IncluirAdendoSubCliente(adendoSubCliente);
                    }
                }
            }

            if (viewModel.TipoAdendo == TipoAdendo.EXCLUSAO_SUB_CLIENTE)
            {
                var adendoSubCliente = new OportunidadeAdendoSubCliente
                {
                    AdendoId = adendoId,
                    Acao = AdendoAcao.EXCLUSAO
                };

                adendoSubCliente.AdicionarSubClientes(
                    viewModel.AdendosSubClientesExclusao.ToList());

                if (!Validar(adendoSubCliente))
                {
                    _oportunidadeRepositorio.ExcluirAdendo(adendoId);
                    return RetornarErros();
                }

                adendoSubCliente.AnexoId = IncluirAnexo(oportunidadeBusca.Id, TipoAnexo.OUTROS, anexoExclusaoSubClientes);

                foreach (var cliente in adendoSubCliente.Clientes)
                {
                    var registro = cliente.Split(':');

                    if (registro.Length == 2)
                    {
                        var subClienteId = registro[0].ToInt();
                        var segmento = registro[1].ToInt();

                        adendoSubCliente.ClienteId = subClienteId;
                        adendoSubCliente.Segmento = segmento;

                        _oportunidadeRepositorio.IncluirAdendoSubCliente(adendoSubCliente);
                    }
                }
            }

            var detalhesAdendo = _oportunidadeRepositorio.ObterDetalhesAdendo(adendoId);

            GravarLogAuditoria(TipoLogAuditoria.INSERT, detalhesAdendo, adendo.OportunidadeId);

            PopularAdendos(viewModel);

            return Json(new
            {
                AdendoId = adendoId,
                OportunidadeId = oportunidadeBusca.Id,
                HabilitaAbaFichas = habilitaAbaFichas
            }, JsonRequestBehavior.AllowGet);
        }

        [CanActivateUsuarioIntegracaoAdendo]
        public PartialViewResult ObterAdendosOportunidade(int oportunidadeId)
        {
            var oportunidadeBusca = _oportunidadeRepositorio.ObterOportunidadePorId(oportunidadeId);

            if (oportunidadeBusca == null)
                throw new Exception("Oportunidade não encontrada");

            var viewModel = new OportunidadesAdendosViewModel();
            viewModel.AdendoOportunidadeId = oportunidadeId;

            PopularAdendos(viewModel);

            return PartialView("_AbaAdendosConsulta", viewModel.Adendos);
        }

        [HttpGet]
        public ActionResult DetalhesAdendo(int id)
        {
            var adendoBusca = _oportunidadeRepositorio.ObterAdendoPorId(id);

            if (adendoBusca == null)
                RegistroNaoEncontrado();

            var viewModel = new OportunidadesAdendosViewModel
            {
                Id = adendoBusca.Id,
                TipoAdendo = adendoBusca.TipoAdendo,
                StatusAdendo = adendoBusca.StatusAdendo
            };

            if (adendoBusca.TipoAdendo == TipoAdendo.ALTERACAO_VENDEDOR)
            {
                var adendoVendedor = _oportunidadeRepositorio.ObterAdendoVendedor(id);

                PopularVendedoresAdendo(viewModel);

                if (adendoVendedor != null)
                    viewModel.AdendoVendedorId = adendoVendedor.VendedorId;
            }

            if (adendoBusca.TipoAdendo == TipoAdendo.FORMA_PAGAMENTO)
            {
                var adendoFormaPagamento = _oportunidadeRepositorio.ObterAdendoFormaPagamento(id);

                if (adendoFormaPagamento != null)
                    viewModel.AdendoFormaPagamento = adendoFormaPagamento.FormaPagamento;
            }

            if (adendoBusca.TipoAdendo == TipoAdendo.INCLUSAO_GRUPO_CNPJ)
            {
                var gruposCNPJ = _oportunidadeRepositorio.ObterAdendoGruposCNPJ(id, AdendoAcao.INCLUSAO);

                viewModel.ClientesGrupoCNPJ = gruposCNPJ.ToList();
            }

            if (adendoBusca.TipoAdendo == TipoAdendo.EXCLUSAO_GRUPO_CNPJ)
            {
                var gruposCNPJ = _oportunidadeRepositorio.ObterAdendoGruposCNPJ(id, AdendoAcao.EXCLUSAO);

                viewModel.ClientesGrupoCNPJ = gruposCNPJ.ToList();
            }

            if (adendoBusca.TipoAdendo == TipoAdendo.INCLUSAO_SUB_CLIENTE)
            {
                var subClientes = _oportunidadeRepositorio.ObterAdendoSubClientesInclusao(id);

                viewModel.SubClientes = subClientes.ToList();
            }

            if (adendoBusca.TipoAdendo == TipoAdendo.EXCLUSAO_SUB_CLIENTE)
            {
                var subClientes = _oportunidadeRepositorio.ObterAdendoSubClientesExclusao(id);

                viewModel.SubClientes = subClientes.ToList();
            }

            return View(viewModel);
        }

        [HttpGet]
        public ActionResult AtualizarAdendo(int id)
        {
            var adendoBusca = _oportunidadeRepositorio.ObterAdendoPorId(id);

            if (adendoBusca == null)
                RegistroNaoEncontrado();

            var viewModel = new OportunidadesAdendosViewModel
            {
                AdendoOportunidadeId = adendoBusca.OportunidadeId,
                TipoAdendo = adendoBusca.TipoAdendo,
                StatusAdendo = adendoBusca.StatusAdendo
            };

            PopularVendedoresAdendo(viewModel);
            PopularClientesDaProposta(viewModel);
            PopularClientesGrupoCNPJ(viewModel);

            if (adendoBusca.TipoAdendo == TipoAdendo.ALTERACAO_VENDEDOR)
            {
                var adendoVendedor = _oportunidadeRepositorio.ObterAdendoVendedor(id);

                PopularVendedoresAdendo(viewModel);

                if (adendoVendedor != null)
                {
                    viewModel.Id = adendoVendedor.Id;
                    viewModel.AdendoVendedorId = adendoVendedor.VendedorId;
                }
            }

            if (adendoBusca.TipoAdendo == TipoAdendo.FORMA_PAGAMENTO)
            {
                var adendoFormaPagamento = _oportunidadeRepositorio.ObterAdendoFormaPagamento(id);

                if (adendoFormaPagamento != null)
                {
                    viewModel.Id = adendoFormaPagamento.Id;
                    viewModel.AdendoFormaPagamento = adendoFormaPagamento.FormaPagamento;
                }
            }

            if (adendoBusca.TipoAdendo == TipoAdendo.INCLUSAO_GRUPO_CNPJ)
            {
                var gruposCNPJ = _oportunidadeRepositorio.ObterAdendoGruposCNPJ(id, AdendoAcao.INCLUSAO);

                viewModel.ClientesGrupoCNPJ = gruposCNPJ.ToList();
            }

            if (adendoBusca.TipoAdendo == TipoAdendo.EXCLUSAO_GRUPO_CNPJ)
            {
                var gruposCNPJ = _oportunidadeRepositorio.ObterClientesGrupoCNPJ(adendoBusca.OportunidadeId);

                viewModel.ClientesGrupoCNPJ = gruposCNPJ.ToList();
            }

            if (adendoBusca.TipoAdendo == TipoAdendo.INCLUSAO_SUB_CLIENTE)
            {
                var subClientes = _oportunidadeRepositorio.ObterAdendoSubClientesInclusao(id);

                viewModel.SubClientes = subClientes.ToList();
            }

            if (adendoBusca.TipoAdendo == TipoAdendo.EXCLUSAO_SUB_CLIENTE)
            {
                var subClientes = _oportunidadeRepositorio.ObterSubClientesEdicaoAdendo(adendoBusca.OportunidadeId, AdendoAcao.EXCLUSAO);

                viewModel.SubClientes = subClientes.ToList();
            }

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult AtualizarAdendo([Bind(Include = "Id, AdendoOportunidadeId, AdendoVendedorId, TipoAdendo, StatusAdendo, AdendoFormaPagamento, AdendoSegmento, AdendoContaId, AdendoSubClienteId, AdendoClienteGrupoCNPJId, AdendosClientesGrupoInclusao, AdendosClientesGrupoExclusao, AdendosSubClientesInclusao, AdendosSubClientesExclusao")] OportunidadesAdendosViewModel viewModel, HttpPostedFileBase anexoFormaPagamento, HttpPostedFileBase anexoExclusaoSubClientes, HttpPostedFileBase anexoExclusaoGrupoCNPJ)
        {
            if (viewModel.Id == 0)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Adendo não informado");

            var oportunidadeBusca = _oportunidadeRepositorio.ObterOportunidadePorId(viewModel.AdendoOportunidadeId);

            if (oportunidadeBusca == null)
                RegistroNaoEncontrado();

            if (!User.IsInRole("OportunidadesAdendos:Atualizar"))
            {
                if (!_equipesService.ValidarEquipeOportunidade(oportunidadeBusca.Id))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Usuário não possui permissão para edição da Oportunidade (Equipes)");
                }
            }

            if (oportunidadeBusca.StatusOportunidade != StatusOportunidade.ATIVA && oportunidadeBusca.StatusOportunidade != StatusOportunidade.REVISADA)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Disponível apenas se o Status da Oportunidade for igual a Ativa / Revisada");

            if (viewModel.AdendoVendedorId > 0)
            {
                if (viewModel.AdendoVendedorId == oportunidadeBusca.OportunidadeProposta?.VendedorId)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "O Vendedor informado é o mesmo da Oportunidade.");
            }

            if (viewModel.AdendoFormaPagamento > 0)
            {
                if (viewModel.AdendoFormaPagamento == oportunidadeBusca.OportunidadeProposta?.FormaPagamento)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "A Forma de Pagamento informada é a mesma da Oportunidade");
            }

            if (viewModel.TipoAdendo == TipoAdendo.ALTERACAO_VENDEDOR)
            {
                var adendoVendedorBusca = _oportunidadeRepositorio.ObterAdendoVendedor(viewModel.Id);

                if (adendoVendedorBusca == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Adendo de Vendedor não encontrado");
                }

                adendoVendedorBusca.VendedorId = viewModel.AdendoVendedorId;

                if (!Validar(adendoVendedorBusca))
                    return RetornarErros();

                _oportunidadeRepositorio.AtualizarAdendoVendedor(adendoVendedorBusca);

                var detalhesAdendo = _oportunidadeRepositorio.ObterDetalhesAdendo(adendoVendedorBusca.AdendoId);

                GravarLogAuditoria(TipoLogAuditoria.UPDATE, detalhesAdendo, oportunidadeBusca.Id);
            }

            if (viewModel.TipoAdendo == TipoAdendo.FORMA_PAGAMENTO)
            {
                var adendoFormaPagamentoBusca = _oportunidadeRepositorio.ObterAdendoFormaPagamento(viewModel.Id);

                if (adendoFormaPagamentoBusca == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Adendo de Forma Pagamento não encontrado");
                }

                adendoFormaPagamentoBusca.FormaPagamento = viewModel.AdendoFormaPagamento;

                if (!Validar(adendoFormaPagamentoBusca))
                    return RetornarErros();

                adendoFormaPagamentoBusca.AnexoId = IncluirAnexo(oportunidadeBusca.Id, TipoAnexo.OUTROS, anexoFormaPagamento);

                _oportunidadeRepositorio.AtualizarAdendoFormaPagamento(adendoFormaPagamentoBusca);

                var detalhesAdendo = _oportunidadeRepositorio.ObterDetalhesAdendo(adendoFormaPagamentoBusca.AdendoId);

                GravarLogAuditoria(TipoLogAuditoria.UPDATE, detalhesAdendo, oportunidadeBusca.Id);
            }

            if (viewModel.TipoAdendo == TipoAdendo.INCLUSAO_GRUPO_CNPJ)
            {
                foreach (var clienteGrupo in viewModel.AdendosClientesGrupoInclusao)
                {
                    var existeClienteGrupo = _oportunidadeRepositorio
                        .ObterClientesGrupoCNPJ(viewModel.AdendoOportunidadeId)
                        .Any(c => c.ContaId == clienteGrupo);

                    if (existeClienteGrupo)
                    {
                        var conta = _contaRepositorio.ObterContaPorId(clienteGrupo);

                        if (conta != null)
                            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"O Sub Cliente {conta.Descricao} já existe na Oportunidade");
                    }
                }

                _oportunidadeRepositorio.ExcluirGruposCNPJDoAdendo(viewModel.Id, AdendoAcao.INCLUSAO);

                var adendoGrupoCNPJ = new OportunidadeAdendoGrupoCNPJ
                {
                    AdendoId = viewModel.Id,
                    Acao = AdendoAcao.INCLUSAO
                };

                adendoGrupoCNPJ.AdicionarClientesGrupoCNPJ(
                    viewModel.AdendosClientesGrupoInclusao.ToList());

                if (!Validar(adendoGrupoCNPJ))
                    return RetornarErros();

                foreach (var grupo in adendoGrupoCNPJ.Clientes)
                {
                    adendoGrupoCNPJ.ClienteId = grupo;
                    _oportunidadeRepositorio.IncluirAdendoGruposCNPJ(adendoGrupoCNPJ);
                }

                var detalhesAdendo = _oportunidadeRepositorio.ObterDetalhesAdendo(adendoGrupoCNPJ.AdendoId);

                GravarLogAuditoria(TipoLogAuditoria.UPDATE, detalhesAdendo, oportunidadeBusca.Id);
            }

            if (viewModel.TipoAdendo == TipoAdendo.EXCLUSAO_GRUPO_CNPJ)
            {
                _oportunidadeRepositorio.ExcluirGruposCNPJDoAdendo(viewModel.Id, AdendoAcao.EXCLUSAO);

                var adendoGrupoCNPJ = new OportunidadeAdendoGrupoCNPJ
                {
                    AdendoId = viewModel.Id,
                    Acao = AdendoAcao.EXCLUSAO
                };

                adendoGrupoCNPJ.AdicionarClientesGrupoCNPJ(
                    viewModel.AdendosClientesGrupoExclusao.ToList());

                if (!Validar(adendoGrupoCNPJ))
                    return RetornarErros();

                foreach (var grupo in adendoGrupoCNPJ.Clientes)
                {
                    adendoGrupoCNPJ.ClienteId = grupo;

                    adendoGrupoCNPJ.Id = _oportunidadeRepositorio.IncluirAdendoGruposCNPJ(adendoGrupoCNPJ);
                    GravarLogAuditoria(TipoLogAuditoria.INSERT, adendoGrupoCNPJ);
                }

                var detalhesAdendo = _oportunidadeRepositorio.ObterDetalhesAdendo(adendoGrupoCNPJ.AdendoId);

                GravarLogAuditoria(TipoLogAuditoria.UPDATE, detalhesAdendo, oportunidadeBusca.Id);
            }

            if (viewModel.TipoAdendo == TipoAdendo.INCLUSAO_SUB_CLIENTE)
            {
                foreach (var subCliente in viewModel.AdendosSubClientesInclusao)
                {
                    var registro = subCliente.Split(':');
                    var subClienteId = registro[0].ToInt();

                    if (subClienteId > 0)
                    {
                        var existeSubCliente = _oportunidadeRepositorio
                            .ObterSubClientes(viewModel.AdendoOportunidadeId)
                            .Any(c => c.ContaId == subClienteId && c.AdendoId != viewModel.Id);

                        var conta = _contaRepositorio.ObterContaPorId(subClienteId);

                        if (conta == null)
                            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"Sub Cliente inexistente");

                        if (existeSubCliente)
                            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"O Sub Cliente {conta.Descricao} já existe na Oportunidade");
                    }
                }

                _oportunidadeRepositorio.ExcluirSubClientesDoAdendo(viewModel.Id, AdendoAcao.INCLUSAO);

                var adendoSubCliente = new OportunidadeAdendoSubCliente
                {
                    AdendoId = viewModel.Id,
                    Acao = AdendoAcao.INCLUSAO
                };

                adendoSubCliente.AdicionarSubClientes(
                    viewModel.AdendosSubClientesInclusao.ToList());

                if (!Validar(adendoSubCliente))
                    return RetornarErros();

                foreach (var subCliente in adendoSubCliente.Clientes)
                {
                    var registro = subCliente.Split(':');
                    var subClienteId = registro[0].ToInt();
                    var segmento = registro[1].ToInt();

                    if (subClienteId > 0)
                    {
                        adendoSubCliente.ClienteId = subClienteId;
                        adendoSubCliente.Segmento = segmento;

                        _oportunidadeRepositorio.IncluirAdendoSubCliente(adendoSubCliente);
                    }
                }

                var subClientes = _oportunidadeRepositorio.ObterAdendoSubClientesInclusao(viewModel.Id);

                viewModel.SubClientes = subClientes.ToList();

                var detalhesAdendo = _oportunidadeRepositorio.ObterDetalhesAdendo(adendoSubCliente.AdendoId);

                GravarLogAuditoria(TipoLogAuditoria.UPDATE, detalhesAdendo, oportunidadeBusca.Id);
            }

            if (viewModel.TipoAdendo == TipoAdendo.EXCLUSAO_SUB_CLIENTE)
            {
                var adendoSubCliente = new OportunidadeAdendoSubCliente
                {
                    AdendoId = viewModel.Id,
                    Acao = AdendoAcao.EXCLUSAO
                };

                adendoSubCliente.AdicionarSubClientes(
                    viewModel.AdendosSubClientesExclusao.ToList());

                if (!Validar(adendoSubCliente))
                    return RetornarErros();

                adendoSubCliente.AnexoId = IncluirAnexo(oportunidadeBusca.Id, TipoAnexo.OUTROS, anexoExclusaoSubClientes);

                _oportunidadeRepositorio.ExcluirSubClientesDoAdendo(viewModel.Id, AdendoAcao.EXCLUSAO);

                foreach (var cliente in adendoSubCliente.Clientes)
                {
                    var registro = cliente.Split(':');

                    if (registro.Length == 2)
                    {
                        var subClienteId = registro[0].ToInt();
                        var segmento = registro[1].ToInt();

                        adendoSubCliente.ClienteId = subClienteId;
                        adendoSubCliente.Segmento = segmento;

                        _oportunidadeRepositorio.IncluirAdendoSubCliente(adendoSubCliente);
                    }
                }

                var subClientes = _oportunidadeRepositorio.ObterAdendoSubClientesExclusao(viewModel.Id);

                viewModel.SubClientes = subClientes.ToList();

                var detalhesAdendo = _oportunidadeRepositorio.ObterDetalhesAdendo(adendoSubCliente.AdendoId);

                GravarLogAuditoria(TipoLogAuditoria.UPDATE, detalhesAdendo, oportunidadeBusca.Id);
            }

            PopularAdendos(viewModel);
            PopularVendedoresAdendo(viewModel);

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult EnviarAdendoParaAprovacao(int id)
        {
            bool habilitaAbaFichas = false;

            var adendoBusca = _oportunidadeRepositorio.ObterAdendoPorId(id);

            if (adendoBusca == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Adendo não encontrado");

            var oportunidadeBusca = _oportunidadeRepositorio.ObterOportunidadePorId(adendoBusca.OportunidadeId);

            if (oportunidadeBusca == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Oportunidade não encontrada");

            if (!User.IsInRole("OportunidadesAdendos:EnviarParaAprovacao"))
            {
                if (!_equipesService.ValidarEquipeOportunidade(oportunidadeBusca.Id))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Usuário não possui permissão para edição da Oportunidade (Equipes)");
                }
            }

            if (string.IsNullOrEmpty(oportunidadeBusca.SallesId))
            {
                if (oportunidadeBusca.OportunidadeProposta == null || oportunidadeBusca.OportunidadeProposta.ModeloId == 0)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "A Oportunidade não possui nenhum Modelo Vinculado");
            }

            var modelo = _modeloRepositorio.ObterModeloPorId(oportunidadeBusca.OportunidadeProposta.ModeloId);

            var aprovacoes = _workflowRepositorio.ObterAprovacoesPorOportunidade(oportunidadeBusca.Id, Processo.ADENDO);

            if (aprovacoes.Any() && adendoBusca.StatusAdendo == StatusAdendo.ENVIADO)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Já existe uma aprovação pendente para este Adendo");


            IEnumerable<dynamic> clientes = null;

            if (adendoBusca.TipoAdendo == TipoAdendo.INCLUSAO_SUB_CLIENTE)
            {
                clientes = _oportunidadeRepositorio
                    .ObterAdendoSubClientesInclusao(adendoBusca.Id)
                    .Select(c => $"{c.Descricao} ({ c.Documento})");

                var parametros = _parametrosRepositorio.ObterParametros();

                if (parametros.ValidaConcomitancia)
                {
                    var concomitancia = ValidarConcomitanciaAdendo(oportunidadeBusca, adendoBusca);

                    if (concomitancia.Concomitante)
                    {
                        return Json(new
                        {
                            Bloqueia = true,
                            Concomitante = concomitancia.Concomitante,
                            Mensagem = concomitancia.Mensagem,
                            RedirectUrl = Url.Action(nameof(Atualizar), new { id = oportunidadeBusca.Id })
                        }, JsonRequestBehavior.AllowGet);
                    }
                }

                if (oportunidadeBusca.OportunidadeProposta.FormaPagamento == FormaPagamento.FATURADO)
                {
                    var subClientesAdendo = _oportunidadeRepositorio.ObterAdendoSubClientesInclusao(adendoBusca.Id);

                    foreach (var subCliente in subClientesAdendo)
                    {
                        var fichaBusca = _oportunidadeRepositorio
                            .ObterFichaFaturamentoPorClienteCnpj(subCliente.Documento, oportunidadeBusca.Id);

                        if (fichaBusca != null)
                        {
                            _oportunidadeRepositorio.AtualizarStatusFichaFaturamento(StatusFichaFaturamento.EM_APROVACAO, fichaBusca.Id);
                        }
                    }
                }
            }

            if (adendoBusca.TipoAdendo == TipoAdendo.INCLUSAO_GRUPO_CNPJ)
            {
                clientes = _oportunidadeRepositorio
                    .ObterAdendoGruposCNPJ(adendoBusca.Id, AdendoAcao.INCLUSAO)
                    .Select(c => $"{c.Descricao} ({ c.Documento})");
            }

            if (adendoBusca.TipoAdendo == TipoAdendo.EXCLUSAO_SUB_CLIENTE)
            {
                clientes = _oportunidadeRepositorio
                    .ObterAdendoSubClientesExclusao(adendoBusca.Id)
                    .Select(c => $"{c.Descricao} ({ c.Documento})");
            }

            if (adendoBusca.TipoAdendo == TipoAdendo.EXCLUSAO_GRUPO_CNPJ)
            {
                clientes = _oportunidadeRepositorio
                    .ObterAdendoGruposCNPJ(adendoBusca.Id, AdendoAcao.EXCLUSAO)
                    .Select(c => $"{c.Descricao} ({ c.Documento})");
            }

            if (adendoBusca.TipoAdendo == TipoAdendo.FORMA_PAGAMENTO)
            {
                var fichasFaturamento = _oportunidadeRepositorio.ObterFichasFaturamento(oportunidadeBusca.Id);

                var adendoFormaPagamento = _oportunidadeRepositorio
                    .ObterAdendoFormaPagamento(adendoBusca.Id)
                    .FormaPagamento;

                if (adendoFormaPagamento == FormaPagamento.FATURADO)
                {
                    if (!fichasFaturamento.Any())
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Ficha de Faturamento não cadastrada");


                    var existeFichaGeral = fichasFaturamento.Any(c => c.ContaId == oportunidadeBusca.ContaId);

                    if (existeFichaGeral == false)
                    {
                        var subClientes = _oportunidadeRepositorio
                            .ObterSubClientes(oportunidadeBusca.Id);

                        var clienteSemFicha = false;

                        foreach (var subCliente in subClientes)
                        {
                            if (subCliente.Segmento == SegmentoSubCliente.IMPORTADOR)
                            {
                                var clienteTemFicha = fichasFaturamento.Where(c => c.ContaId == subCliente.ContaId).Any();

                                if (clienteTemFicha == false)
                                    clienteSemFicha = true;
                            }
                        }

                        if (clienteSemFicha)
                            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"Existem clientes sem ficha de faturamento");
                    }
                }
                //

                foreach (var ficha in fichasFaturamento)
                {
                    if (ficha.StatusFichaFaturamento == StatusFichaFaturamento.EM_ANDAMENTO || ficha.StatusFichaFaturamento == StatusFichaFaturamento.REJEITADO)
                    {
                        _oportunidadeRepositorio.AtualizarStatusFichaFaturamento(StatusFichaFaturamento.EM_APROVACAO, ficha.Id);
                    }
                }

                var lst = new List<string>
                {
                    adendoFormaPagamento.ToName()
                };

                clientes = lst;

                habilitaAbaFichas = adendoFormaPagamento == FormaPagamento.FATURADO;
            }

            if (adendoBusca.TipoAdendo == TipoAdendo.ALTERACAO_VENDEDOR)
            {
                var adendoVendedor = _oportunidadeRepositorio
                    .ObterAdendoVendedor(adendoBusca.Id).VendedorId;

                var vendedor = _vendedorRepositorio.ObterVendedorPorId(adendoVendedor);

                var lst = new List<string>
                {
                    vendedor.Nome
                };

                clientes = lst;
            }

            var oportunidadeDetalhes = _oportunidadeRepositorio.ObterDetalhesOportunidade(oportunidadeBusca.Id);

            var campos = new
            {
                OportunidadeId = oportunidadeBusca.Id,
                Descricao = $"A-{adendoBusca.Id}",
                DescricaoOportunidade = oportunidadeBusca.Descricao,
                oportunidadeDetalhes.Conta,
                oportunidadeDetalhes.TabelaId,
                TipoAdendo = adendoBusca.TipoAdendo.ToName(),
                StatusAdendo = adendoBusca.StatusAdendo.ToName(),
                adendoBusca.DataCadastro,
                adendoBusca.CriadoPor,
                TipoOperacao = oportunidadeBusca.OportunidadeProposta.TipoOperacao.ToString(),
                Clientes = clientes != null ? string.Join(",", clientes) : string.Empty
            };

            var token = Autenticador.Autenticar();

            if (token == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Não foi possível se autenticar no serviço de Workflow");

            var workflow = new WorkflowService(token);

            var retorno = workflow.EnviarParaAprovacao(
                new CadastroWorkflow(Processo.ADENDO, oportunidadeBusca.EmpresaId, adendoBusca.Id, User.ObterLogin(), User.ObterNome(), User.ObterEmail(), JsonConvert.SerializeObject(campos)));

            if (retorno == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Nenhuma resposta do serviço de Workflow");

            if (retorno.sucesso == false)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, retorno.mensagem);

            _workflowRepositorio.IncluirEnvioAprovacao(new EnvioWorkflow(oportunidadeBusca.Id, adendoBusca.Id, Processo.ADENDO, retorno.protocolo, retorno.mensagem, User.ObterId()));

            _oportunidadeRepositorio.AtualizarStatusAdendo(StatusAdendo.ENVIADO, adendoBusca.Id);

            GravarLogAuditoria(TipoLogAuditoria.OUTROS, adendoBusca, mensagem: "Adendo enviado para Aprovação");

            return Json(new
            {
                Bloqueia = false,
                Mensagem = "Adendo enviado para Aprovação",
                HabilitaAbaFichas = habilitaAbaFichas
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ValidarEquipeConta(int contaId, bool subCliente)
        {
            if (subCliente)
                return new HttpStatusCodeResult(HttpStatusCode.NoContent);

            var permissoesPorConta = _equipeContaRepositorio
                .ObterPermissoesContaPorConta(contaId, User.Identity.Name);

            var conta = _contaRepositorio.ObterContaPorId(contaId);

            if (!User.IsInRole("Administrador") || !User.IsInRole("UsuarioExterno"))
            {
                if (permissoesPorConta != null)
                {
                    if (permissoesPorConta.AcessoOportunidade == 0)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Usuário não possui acesso à conta relacionada à oportunidade. Operação não permitida");
                    }
                }
                else
                {
                    if (conta.VendedorId != User.ObterId())
                    {
                        var permissoesPorVendedor = _equipeContaRepositorio
                            .ObterPermissoesContaPorVendedor(contaId, User.Identity.Name);

                        if (permissoesPorVendedor == null || permissoesPorVendedor.AcessoOportunidade == 0)
                        {
                            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Usuário não possui acesso à conta relacionada à oportunidade. Operação não permitida");
                        }
                    }
                }
            }

            return Json(new
            {
                sucesso = true
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ValidarPermissaoSPC(int contaId, string razaoSocial, bool subCliente)
        {
            int tamanho = razaoSocial.Length;

            string cnpj = razaoSocial.Substring(tamanho - 19, 18).Replace(")", "");
            //achei
            Session["RazaoSocial"] = razaoSocial;
            Session["FontePagadoraId"] = contaId;
            Session["Cnpj"] = cnpj;


            if (subCliente)
                return new HttpStatusCodeResult(HttpStatusCode.NoContent);

            var permissoesPorConta = _equipeContaRepositorio
                .ObterPermissoesContaPorConta(contaId, User.Identity.Name);

            var conta = _contaRepositorio.ObterContaPorId(contaId);

            if (!User.IsInRole("Administrador") || !User.IsInRole("UsuarioExterno"))
            {
                if (permissoesPorConta != null)
                {
                    if (permissoesPorConta.AcessoOportunidade == 0)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Usuário não possui acesso à conta relacionada à oportunidade. Operação não permitida");
                    }
                }
                else
                {
                    if (conta.VendedorId != User.ObterId())
                    {
                        var permissoesPorVendedor = _equipeContaRepositorio
                            .ObterPermissoesContaPorVendedor(contaId, User.Identity.Name);

                        if (permissoesPorVendedor == null || permissoesPorVendedor.AcessoOportunidade == 0)
                        {
                            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Usuário não possui acesso à conta relacionada à oportunidade. Operação não permitida");
                        }
                    }
                }
            }

            return Json(new
            {
                sucesso = true
            }, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult ObterUsuariosOportunidade()
        {
            var usuarios = _oportunidadeRepositorio
                .ObterUsuariosOportunidade()
                .ToList();

            return Json(usuarios, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public int ExisteModeloNaOportunidade(int id)
        {
            var resultado = _oportunidadeRepositorio
                .ObterOportunidadePorId(id).OportunidadeProposta?.ModeloId;

            return resultado.Value;
        }

        [HttpGet]
        public bool ExisteCancelamento(int id)
        {
            var resultado = _oportunidadeRepositorio
                .ObterOportunidadePorId(id);

            return resultado.Cancelado;
        }

        public ActionResult EdicaoValores(int? id)
        {
            if (id == null)
                return RedirectToAction(nameof(Index));

            var oportunidadeBusca = _oportunidadeRepositorio.ObterOportunidadePorId(id.Value);

            if (oportunidadeBusca == null)
                throw new Exception("Oportunidade não encontrada");

            ILayoutBase repositorio = _layoutPropostaRepositorio;

            var layout = repositorio.ObterLayoutEdicaoProposta(id.Value).ToList();

            var clientesHubPort = _hubPortRepositorio.ObterClientesHubPort();

            foreach (var item in layout)
                item.ClientesHubPort = clientesHubPort;

            var alteracoes = _layoutPropostaRepositorio.ObterAlteracoesProposta(oportunidadeBusca.Id).ToList();

            foreach (var valor in layout)
            {
                foreach (var alteracao in alteracoes.Where(c => c.Linha == valor.Linha).ToList())
                {
                    var nomePropriedadeAlterada = alteracao.Propriedade;

                    var propertyInfo = valor.GetType().GetProperty(nomePropriedadeAlterada);

                    if (propertyInfo != null)
                    {
                        if (!propertyInfo.PropertyType.IsEnum && !propertyInfo.PropertyType.IsGenericType)
                        {
                            ObjetoHelpers.GetValue(valor, nomePropriedadeAlterada, out object valorPropriedadeAlterada);
                            propertyInfo.SetValue(valor, $"<span class='valor-proposta-alterado'>{valorPropriedadeAlterada}</span>", null);
                        }
                    }
                }
            }

            bool apenasLeitura = !(!Enum.IsDefined(typeof(StatusOportunidade), oportunidadeBusca.StatusOportunidade) || oportunidadeBusca.StatusOportunidade == StatusOportunidade.RECUSADO);

            if (!User.IsInRole("OportunidadesProposta:btnEditarProposta"))
            {
                apenasLeitura = true;
            }

            return View(nameof(EdicaoValores), new EdicaoValoresViewModel
            {
                OportunidadeId = oportunidadeBusca.Id,
                Identificacao = oportunidadeBusca.Identificacao,
                Valores = layout,
                ApenasLeitura = apenasLeitura
            });
        }

        [HttpPost]
        public ActionResult ExportarTabelas(int id, string lotes)
        {
            var oportunidadeBusca = _oportunidadeRepositorio.ObterOportunidadePorId(id);
            //proposta - 66779
            var message = "";
            if (string.IsNullOrEmpty(lotes)==false)
             {
                var lotesArr = Array.ConvertAll(lotes.Split(','), int.Parse);
            
                Dictionary<int, bool> lotesVerificados = new Dictionary<int, bool>();

                foreach (var lote in lotesArr)
                {
                    var resultado = _loteRepositorio.ValidarLoteNotaFiscal(lote);
                    if (resultado)
                    {
                        var cancelamento = _loteRepositorio.ValidarLoteCancelamentoNotaFiscal(lote);
                        lotesVerificados.Add(lote, cancelamento);
                        // > Se não: 
                        //apresenta mensagem de aviso que o lote possui NF e não permite a integração (onde seguiremos com o rejeite do acordo).
                    };
                }

                foreach (KeyValuePair<int, bool> item in lotesVerificados)
                {
                    if (item.Value == false)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Lote " + item.Key + ", Possue NF e não permite a integração");
                    };
                }
                foreach (KeyValuePair<int, bool> item in lotesVerificados)
                {
                    if (item.Value == true)
                    {
                        message = "Lote: " + item.Key + " possue nota de cancelamento: ";
                    };
                }
            }
            if (oportunidadeBusca == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Oportunidade não encontrada");

            if (oportunidadeBusca.OportunidadeProposta?.ModeloId == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Nenhum Modelo vinculado na Oportunidade");

            var modeloBusca = _modeloRepositorio.ObterModeloPorId(oportunidadeBusca.OportunidadeProposta.ModeloId);

            if (modeloBusca == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Modelo inexistente");

            if (modeloBusca.IntegraChronos == false)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Modelo não configurado para integração");

            using (var wsIntegraChronos = new WsIntegraChronos.IntegraChronos())
            {
                wsIntegraChronos.Timeout = 900000;

                var response = new WsIntegraChronos.Response();

                using (var ws = new WsIntegraChronos.IntegraChronos())
                {
                    response = ws.ExportarTabelas(oportunidadeBusca.Id, User.ObterId());

                    if (response.Sucesso == false)
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, response.Mensagem);

                    return PartialView("_ModalTabelaCobrancaExportada", new OportunidadesInformacoesIniciaisViewModel
                    {
                        TabelaId = response.TabelaId,
                        Mensagem = message
                    });
                }
            }
        }

        [HttpPost]
        public ActionResult ExportarTabelaCancelada(int id)
        {
            var oportunidadeBusca = _oportunidadeRepositorio.ObterOportunidadePorId(id);

            if (oportunidadeBusca == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Oportunidade não encontrada");

            if (oportunidadeBusca.StatusOportunidade != StatusOportunidade.CANCELADA)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Disponível apenas para Oportunidades com Status Cancelada");

            if (oportunidadeBusca.Cancelado == false)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "A Oportunidade não está Cancelada");

            using (var wsIntegraChronos = new WsIntegraChronos.IntegraChronos())
            {
                wsIntegraChronos.Timeout = 900000;

                var response = new WsIntegraChronos.Response();

                using (var ws = new WsIntegraChronos.IntegraChronos())
                {
                    response = ws.CancelarTabela(oportunidadeBusca.Id, User.ObterId());

                    if (response.Sucesso == false)
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, response.Mensagem);

                    return PartialView("_ModalTabelaCobrancaCancelada", new OportunidadesInformacoesIniciaisViewModel
                    {
                        TabelaId = response.TabelaId,
                        Mensagem = response.Mensagem
                    });
                }
            }
        }

        [HttpPost]
        public ActionResult IntegraAdendoChronos(int id)
        {
            var usuarioIntegracao = _usuarioRepositorio
                .ObterUsuariosIntegracao()
                .Where(c => c.UsuarioId == User.ObterId()).FirstOrDefault();

            if (usuarioIntegracao == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"Usuário não cadastrado para funcionalidade de Integração");

            //31/07/2020
            //if (_ambienteOracleService.Ambiente() == AmbienteOracle.PROD)
            //{
            //    if (usuarioIntegracao.AcessoProducao == false)
            //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"Funcionalidade não disponível para ambiente de {_ambienteOracleService.Ambiente().ToName()}");
            //}

            using (var ws = new WsIntegraChronos.IntegraChronos())
            {
                var adendoBusca = _oportunidadeRepositorio.ObterAdendoPorId(id);

                if (adendoBusca == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Adendo não encontrado / excluído");

                var response = ws.IntregrarAdendosChronos(adendoBusca.OportunidadeId, adendoBusca.Id);

                if (response.Sucesso == false)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, response.Mensagem);

                return PartialView("_ModalIntegracaoChronos", new OportunidadesIntegracaoChronosViewModel
                {
                    Descricao = response.Mensagem
                });
            }
        }

        [HttpPost]
        public ActionResult IntegraFichaChronos(int id)
        {
            var usuarioIntegracao = _usuarioRepositorio
                .ObterUsuariosIntegracao()
                .Where(c => c.UsuarioId == User.ObterId()).FirstOrDefault();

            if (usuarioIntegracao == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"Usuário não cadastrado para funcionalidade de Integração");

            var fichaBusca = _oportunidadeRepositorio.ObterFichaFaturamentoPorId(id);

            if (fichaBusca == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Ficha não encontrada / excluída");

            if (fichaBusca.StatusFichaFaturamento == StatusFichaFaturamento.APROVADO || fichaBusca.StatusFichaFaturamento == StatusFichaFaturamento.REVISADA)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Status não permitido para integração");

            var adendos = _oportunidadeRepositorio.ObterAdendos(fichaBusca.OportunidadeId);

            var adendosFormaPgtoEmAberto = adendos
                .Where(c => c.TipoAdendo == TipoAdendo.FORMA_PAGAMENTO && (c.StatusAdendo == StatusAdendo.ABERTO || c.StatusAdendo == StatusAdendo.ENVIADO));

            var existeAdendoFormaPgtoFaturadoEmAberto = false;

            foreach (var adendo in adendosFormaPgtoEmAberto)
            {
                var adendoFormaPgto = _oportunidadeRepositorio.ObterAdendoFormaPagamento(adendo.Id);

                if (adendoFormaPgto.FormaPagamento == FormaPagamento.FATURADO)
                {
                    existeAdendoFormaPgtoFaturadoEmAberto = true;
                    break;
                }
            }

            if (existeAdendoFormaPgtoFaturadoEmAberto)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Envio permitido via Adendo");

            using (var ws = new WsIntegraChronos.IntegraChronos())
            {
                var response = ws.IntregrarFichasChronos(fichaBusca.OportunidadeId, fichaBusca.Id);

                if (response.Sucesso == false)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, response.Mensagem);

                return PartialView("_ModalIntegracaoChronos", new OportunidadesIntegracaoChronosViewModel
                {
                    Descricao = response.Mensagem
                });
            }
        }

        [HttpGet]
        public ActionResult ObterLotesProposta(string lotes)
        {
            if (string.IsNullOrWhiteSpace(lotes))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Lotes não informados");

            var lotesArr = Array.ConvertAll(lotes.Split(','), int.Parse);

            var lotesLista = new List<string>();
            var blsLista = new List<string>();

            foreach (var lote in lotesArr)
            {
                var lotesResultado = _loteRepositorio.ObterLotesMaster(lote);

                lotesLista.AddRange(lotesResultado.Select(c => c.Lote));
                blsLista.AddRange(lotesResultado.Select(c => c.Numero));
            }

            return Json(new
            {
                lotesLista,
                blsLista
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ValidarLoteProposta(string lote)
        {
            var loteBusca = _loteRepositorio.ObterLotePorId(lote.ToInt());

            if (loteBusca == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Lote não encontrado");

            //if (loteBusca.Ativo == false)
            //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Este Lote não está ativo");

            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        [HttpGet]
        public ActionResult ObterBLsPorId(string lote)
        {
            var loteBusca = _loteRepositorio.ObterLotePorId(lote.ToInt());

            if (loteBusca != null)
            {
                return Json(loteBusca.Numero, JsonRequestBehavior.AllowGet);
            }

            return null;
        }
    }
}