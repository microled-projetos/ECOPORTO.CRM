using Ecoporto.CRM.Business.Enums;
using Ecoporto.CRM.Business.Extensions;
using Ecoporto.CRM.Business.Interfaces.Repositorios;
using Ecoporto.CRM.Business.Models;
using Ecoporto.CRM.Site.Filtros;
using Ecoporto.CRM.Site.Models;
using NLog;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace Ecoporto.CRM.Site.Controllers
{
    [Authorize]
    public class ModelosController : BaseController
    {
        private readonly IModeloRepositorio _modeloRepositorio;
        private readonly IImpostoRepositorio _impostoRepositorio;
        private readonly IVendedorRepositorio _vendedorRepositorio;
        private readonly IModeloSimuladorRepositorio _modeloSimuladorRepositorio;
        private readonly ILayoutRepositorio _layoutRepositorio;

        public ModelosController(
            IModeloRepositorio modeloRepositorio,
            IImpostoRepositorio impostoRepositorio,
            IVendedorRepositorio vendedorRepositorio,
            ILayoutRepositorio layoutRepositorio,
            IModeloSimuladorRepositorio modeloSimuladorRepositorio,
            ILogger logger) : base(logger)
        {
            _modeloRepositorio = modeloRepositorio;
            _impostoRepositorio = impostoRepositorio;
            _vendedorRepositorio = vendedorRepositorio;
            _layoutRepositorio = layoutRepositorio;
            _modeloSimuladorRepositorio = modeloSimuladorRepositorio;
        }

        [HttpGet]
        [CanActivate(Roles = "Modelos:Acessar")]
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult Consultar()
        {
            var modelos = _modeloRepositorio.ObterModelos().Select(c => new
            {
                Id = c.Id,
                Descricao = c.Descricao,
                TipoOperacao = c.TipoOperacao.ToName(),
                FormaPagamento = c.FormaPagamento?.ToName(),
                DataCadastro = c.DataCadastro.ToString("dd/MM/yyyy"),
                Status = c.Status,
                DataInatividade = c.DataInatividade?.ToString("dd/MM/yyyy")
            });

            return Json(new
            {
                dados = modelos
            }, JsonRequestBehavior.AllowGet);
        }

        private void PopularVendedores(ModeloViewModel viewModel)
        {
            viewModel.Vendedores = _vendedorRepositorio
                .ObterVendedores()
                .ToList();
        }

        private void PopularImpostos(ModeloViewModel viewModel)
        {
            viewModel.Impostos = _impostoRepositorio
                .ObterImpostos()
                .ToList();
        }

        private void PopularModelosSimulador(ModeloViewModel viewModel)
        {
            viewModel.ModelosSimulador = _modeloSimuladorRepositorio
              .ObterModelosSimulador()
              .ToList();
        }

        private void PopularVinculosModelosSimulador(ModeloViewModel viewModel)
        {
            viewModel.ModelosSimuladorVinculados = _modeloRepositorio
              .ObterModelosSimuladorVinculados(viewModel.Id)
              .ToList();
        }

        [HttpGet]
        [CanActivate(Roles = "Modelos:Cadastrar")]
        public ActionResult Cadastrar()
        {
            var viewModel = new ModeloViewModel
            {
                Status = Status.ATIVO,
                TipoValidade = TipoValidade.ANOS
            };

            PopularVendedores(viewModel);
            PopularImpostos(viewModel);

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Cadastrar([Bind(Include = "TipoOperacao, Descricao, Status, FormaPagamento, DiasFreeTime, QtdeDias, Validade, TipoValidade, VendedorId, ImpostoId, Acordo, Escalonado, ParametroBL, ParametroLote, ParametroConteiner, ParametroIdTabela, HubPort, CobrancaEspecial, DesovaParcial, FatorCP, PosicIsento, TipoServico")] ModeloViewModel viewModel)
        {
            var modeloBusca = _modeloRepositorio.ObterModeloPorDescricao(viewModel.Descricao);

            if (modeloBusca != null)
                ModelState.AddModelError(string.Empty, "Já existe um modelo cadastrado com a mesma descrição.");

            if (ModelState.IsValid)
            {
                var modelo = new Modelo(
                    viewModel.TipoOperacao,
                    viewModel.Descricao,
                    viewModel.Status,
                    viewModel.FormaPagamento,
                    viewModel.DiasFreeTime,
                    viewModel.QtdeDias,
                    viewModel.Validade,
                    viewModel.TipoValidade,
                    viewModel.TipoServico,
                    viewModel.VendedorId,
                    viewModel.ImpostoId,
                    viewModel.Acordo,
                    viewModel.Escalonado,
                    viewModel.ParametroLote,
                    viewModel.ParametroBL,
                    viewModel.ParametroConteiner,
                    viewModel.ParametroIdTabela,
                    viewModel.HubPort,
                    viewModel.CobrancaEspecial,
                    viewModel.IntegraChronos,
                    viewModel.Simular,
                    viewModel.DesovaParcial,
                    viewModel.FatorCP,
                    viewModel.PosicIsento);

                if (Validar(modelo))
                {
                    modelo.Id = _modeloRepositorio.Cadastrar(modelo);
                    TempData["Sucesso"] = true;

                    GravarLogAuditoria(TipoLogAuditoria.INSERT, modelo);
                }
            }

            PopularVendedores(viewModel);
            PopularImpostos(viewModel);

            return View(viewModel);
        }

        [HttpGet]
        [CanActivate(Roles = "Modelos:Atualizar")]
        public ActionResult Atualizar(int? id)
        {
            if (id == null)
                return RedirectToAction(nameof(Index));

            var modelo = _modeloRepositorio.ObterModeloPorId(id.Value);

            if (modelo == null)
                RegistroNaoEncontrado();

            var viewModel = new ModeloViewModel
            {
                Id = modelo.Id,
                Descricao = modelo.Descricao,
                DiasFreeTime = modelo.DiasFreeTime,
                QtdeDias = modelo.QtdeDias,
                Status = modelo.Status,
                TipoOperacao = modelo.TipoOperacao,
                Validade = modelo.Validade,
                TipoValidade = modelo.TipoValidade,
                FormaPagamento = modelo.FormaPagamento,
                ImpostoId = modelo.ImpostoId,
                VendedorId = modelo.VendedorId,
                Acordo = modelo.Acordo,
                Escalonado = modelo.Escalonado,
                ParametroBL = modelo.ParametroBL,
                ParametroLote = modelo.ParametroLote,
                ParametroIdTabela = modelo.ParametroIdTabela,
                ParametroConteiner = modelo.ParametroConteiner,
                HubPort = modelo.HubPort,
                CobrancaEspecial = modelo.CobrancaEspecial,
                DesovaParcial = modelo.DesovaParcial,
                FatorCP = modelo.FatorCP,
                PosicIsento = modelo.PosicIsento,
                TipoServico = modelo.TipoServico,
                IntegraChronos = modelo.IntegraChronos,
                Simular = modelo.Simular
            };

            PopularVendedores(viewModel);
            PopularImpostos(viewModel);
            PopularModelosSimulador(viewModel);
            PopularVinculosModelosSimulador(viewModel);

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Atualizar([Bind(Include = "Id, TipoOperacao, Descricao, Status, FormaPagamento, DiasFreeTime, QtdeDias, Validade, TipoValidade, VendedorId, ImpostoId, Acordo, Escalonado, ParametroBL, ParametroLote, ParametroConteiner, ParametroIdTabela, HubPort, CobrancaEspecial, DesovaParcial, FatorCP, PosicIsento, TipoServico, IntegraChronos, Simular")] ModeloViewModel viewModel, int? id)
        {
            if (id == null)
                return RedirectToAction(nameof(Index));

            var modelo = _modeloRepositorio.ObterModeloPorId(id.Value);

            if (modelo == null)
                RegistroNaoEncontrado();

            var modeloBusca = _modeloRepositorio.ObterModeloPorDescricao(viewModel.Descricao, modelo.Id);

            if (modeloBusca != null)
                ModelState.AddModelError(string.Empty, "Já existe um modelo cadastrado com a mesma descrição.");

            if (ModelState.IsValid)
            {
                modelo.Alterar(new Modelo(
                    viewModel.TipoOperacao,
                    viewModel.Descricao,
                    viewModel.Status,
                    viewModel.FormaPagamento,
                    viewModel.DiasFreeTime,
                    viewModel.QtdeDias,
                    viewModel.Validade,
                    viewModel.TipoValidade,
                    viewModel.TipoServico,
                    viewModel.VendedorId,
                    viewModel.ImpostoId,
                    viewModel.Acordo,
                    viewModel.Escalonado,
                    viewModel.ParametroLote,
                    viewModel.ParametroBL,
                    viewModel.ParametroConteiner,
                    viewModel.ParametroIdTabela,
                    viewModel.HubPort,
                    viewModel.CobrancaEspecial,
                    viewModel.IntegraChronos,
                    viewModel.Simular,
                    viewModel.DesovaParcial,
                    viewModel.FatorCP,
                    viewModel.PosicIsento));

                if (Validar(modelo))
                {
                    _modeloRepositorio.Atualizar(modelo);
                    TempData["Sucesso"] = true;

                    GravarLogAuditoria(TipoLogAuditoria.UPDATE, modelo);
                }
            }

            PopularVendedores(viewModel);
            PopularImpostos(viewModel);
            PopularModelosSimulador(viewModel);
            PopularVinculosModelosSimulador(viewModel);

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Excluir(int id)
        {
            try
            {
                var modelo = _modeloRepositorio.ObterModeloPorId(id);

                if (modelo == null)
                    RegistroNaoEncontrado();

                var layout = _layoutRepositorio.ObterLayoutPorModelo(modelo.Id);

                if (layout != null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "O modelo já está sendo utilizado em um layout");

                _modeloRepositorio.Excluir(modelo.Id);

                GravarLogAuditoria(TipoLogAuditoria.DELETE, modelo);
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        [HttpGet]
        public ActionResult Importar()
        {
            var modelos = _modeloRepositorio.ObterModelos();

            ViewBag.Modelos = modelos.Select(c => new Modelo
            {
                Id = c.Id,
                Descricao = c.Descricao
            });

            return View();
        }

        [HttpPost]
        public ActionResult Importar(ModeloViewModel viewModel)
        {
            if (viewModel.Id == 0)
                ModelState.AddModelError(string.Empty, "Nenhum modelo selecionado");

            var modeloBusca = _modeloRepositorio.ObterModeloPorId(viewModel.Id);

            if (modeloBusca == null)
                RegistroNaoEncontrado();

            var modelo = new Modelo(
                  modeloBusca.TipoOperacao,
                  viewModel.Descricao,
                  modeloBusca.Status,
                  modeloBusca.FormaPagamento,
                  modeloBusca.DiasFreeTime,
                  modeloBusca.QtdeDias,
                  modeloBusca.Validade,
                  modeloBusca.TipoValidade,
                  modeloBusca.TipoServico,
                  modeloBusca.VendedorId,
                  modeloBusca.ImpostoId,
                  modeloBusca.Acordo,
                  modeloBusca.Escalonado,
                  modeloBusca.ParametroLote,
                  modeloBusca.ParametroBL,
                  modeloBusca.ParametroConteiner,
                  modeloBusca.ParametroIdTabela,
                  modeloBusca.HubPort,
                  modeloBusca.CobrancaEspecial,
                  modeloBusca.IntegraChronos,
                  modeloBusca.Simular,
                  modeloBusca.DesovaParcial,
                  modeloBusca.FatorCP,
                  modeloBusca.PosicIsento);

            Validar(modelo);

            if (modelo.Valido)
            {
                var id = _modeloRepositorio.Cadastrar(modelo);
                _layoutRepositorio.ImportarLayout(id, modeloBusca.Id);

                TempData["Sucesso"] = true;
            }

            var modelos = _modeloRepositorio.ObterModelos();

            ViewBag.Modelos = modelos.Select(c => new Modelo
            {
                Id = c.Id,
                Descricao = c.Descricao
            });

            return View(viewModel);
        }

        [HttpGet]
        public JsonResult ObterModelosPorTipoOperacao(TipoOperacao tipo)
        {
            var modelos = _modeloRepositorio
                .ObterModelosPorTipoOperacao(tipo)
                .Select(c => new
                {
                    Id = c.Id,
                    Descricao = c.Descricao
                });

            return Json(modelos, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ObterModeloPorId(int id)
        {
            var modeloBusca = _modeloRepositorio
                .ObterModeloPorId(id);

            var modelo = new
            {
                modeloBusca.FormaPagamento,
                modeloBusca.DiasFreeTime,
                modeloBusca.VendedorId,
                modeloBusca.QtdeDias,
                modeloBusca.Validade,
                modeloBusca.TipoValidade,
                modeloBusca.TipoServico,
                modeloBusca.ImpostoId,
                modeloBusca.Acordo,
                modeloBusca.Escalonado,
                modeloBusca.ParametroLote,
                modeloBusca.ParametroBL,
                modeloBusca.ParametroConteiner,
                modeloBusca.ParametroIdTabela,
                modeloBusca.HubPort,
                modeloBusca.CobrancaEspecial,
                modeloBusca.DesovaParcial,
                modeloBusca.FatorCP,
                modeloBusca.PosicIsento
            };

            return Json(modelo, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult IncluirModeloSimulador(int modeloSimuladorId, int id)
        {
            if (modeloSimuladorId == 0)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Selecione o Segmento");

            var modeloSimuladorBusca = _modeloSimuladorRepositorio.ObterModeloSimuladorPorId(modeloSimuladorId);

            if (modeloSimuladorBusca == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Modelo de Simulador não encontrado");

            var modeloBusca = _modeloRepositorio.ObterModeloPorId(id);

            if (modeloBusca == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Modelo não encontrado");

            _modeloRepositorio.CadastrarModeloSimulador(new ModeloSimulador
            {
                Id = modeloSimuladorId,
                ModeloId = modeloBusca.Id
            });

            var modelos = _modeloRepositorio
                .ObterModelosSimuladorVinculados(modeloBusca.Id)
                .ToList();

            return PartialView("_ModelosSimuladorConsulta", modelos);
        }

        [HttpPost]
        public ActionResult ExcluirModeloSimulador(int id)
        {
            var modeloSimuladorBusca = _modeloSimuladorRepositorio.ObterVinculoSimuladorPorId(id);

            if (modeloSimuladorBusca == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Modelo de Simulador não encontrado");

            _modeloRepositorio.ExcluirModeloSimulador(id);

            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }
    }
}