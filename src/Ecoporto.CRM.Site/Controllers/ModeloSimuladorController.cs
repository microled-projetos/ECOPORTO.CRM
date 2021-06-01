using Ecoporto.CRM.Business.Interfaces.Repositorios;
using Ecoporto.CRM.Business.Models;
using Ecoporto.CRM.Site.Models;
using NLog;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace Ecoporto.CRM.Site.Controllers
{
    [Authorize]
    public class ModeloSimuladorController : BaseController
    {
        private readonly ILogger _logger;
        private readonly IModeloSimuladorRepositorio _modeloSimuladorRepositorio;
        private readonly IServicoFaturamentoRepositorio _servicoFaturamentoRepositorio;

        public ModeloSimuladorController(
            IModeloSimuladorRepositorio modeloSimuladorRepositorio,
            IServicoFaturamentoRepositorio servicoFaturamentoRepositorio,
            ILogger logger) : base(logger)
        {
            _modeloSimuladorRepositorio = modeloSimuladorRepositorio;
            _servicoFaturamentoRepositorio = servicoFaturamentoRepositorio;
            _logger = logger;
        }

        [HttpGet]
        public ActionResult Index()
        {
            var modelos = _modeloSimuladorRepositorio.ObterModelosSimulador();

            return View(modelos);
        }

        public void PopularModelos(ModeloSimuladorViewModel modeloSimuladorViewModel)
        {
            var modelos = _modeloSimuladorRepositorio.ObterModelosSimulador();

            modeloSimuladorViewModel.Modelos = modelos.ToList();
        }

        public void PopularServicosIPA(ModeloSimuladorViewModel modeloSimuladorViewModel)
        {
            var servicosIPA = _servicoFaturamentoRepositorio.ObterServicos();

            modeloSimuladorViewModel.ServicosIPA = servicosIPA.ToList();
        }

        public void PopularServicosVinculados(ModeloSimuladorViewModel modeloSimuladorViewModel)
        {
            var modeloBusca = _modeloSimuladorRepositorio.ObterModeloSimuladorPorId(modeloSimuladorViewModel.Id);

            modeloSimuladorViewModel.ServicosVinculados = new int[0];

            if (modeloBusca != null)
            {
                modeloSimuladorViewModel.ServicosVinculados = modeloBusca.ServicoVinculados.ToArray();
            }
        }

        [HttpGet]
        public ActionResult Cadastrar()
        {
            var viewModel = new ModeloSimuladorViewModel();

            PopularModelos(viewModel);
            PopularServicosIPA(viewModel);
            PopularServicosVinculados(viewModel);

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Cadastrar([Bind(Include = "Descricao, Regime, Observacoes, ServicoIPASelecionados")] ModeloSimuladorViewModel viewModel)
        {
            var modeloSimulador = new ModeloSimulador(viewModel.Descricao, viewModel.Observacoes, viewModel.Regime, viewModel.ServicoIPASelecionados);

            if (!Validar(modeloSimulador))
                return RetornarErros();

            try
            {
                _modeloSimuladorRepositorio.Cadastrar(modeloSimulador);
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Falha ao cadastrar o Modelo Simulador");
            }

            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        [HttpGet]
        public ActionResult Atualizar(int? id)
        {
            if (id == null)
                return RedirectToAction(nameof(Index));

            var modeloBusca = _modeloSimuladorRepositorio.ObterModeloSimuladorPorId(id.Value);

            if (modeloBusca == null)
                RegistroNaoEncontrado();

            var viewModel = new ModeloSimuladorViewModel
            {
                Id = modeloBusca.Id,
                Descricao = modeloBusca.Descricao,
                Regime = modeloBusca.Regime,
                Observacoes = modeloBusca.Observacoes
            };

            PopularServicosIPA(viewModel);
            PopularServicosVinculados(viewModel);

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Atualizar([Bind(Include = "Id, Descricao, Regime, Observacoes, ServicoIPASelecionados")] ModeloSimuladorViewModel viewModel, int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Modelo de Simulador não informado");

            var modeloBusca = _modeloSimuladorRepositorio.ObterModeloSimuladorPorId(id.Value);

            if (modeloBusca == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Modelo de Simulador não encontrado ou excluído");

            var modeloSimulador = new ModeloSimulador(viewModel.Descricao, viewModel.Observacoes, viewModel.Regime, viewModel.ServicoIPASelecionados);

            if (!Validar(modeloSimulador))
                return RetornarErros();

            modeloSimulador.Id = modeloBusca.Id;

            _modeloSimuladorRepositorio.Atualizar(modeloSimulador);

            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        [HttpPost]
        public ActionResult Excluir(int id)
        {
            try
            {
                var modeloBusca = _modeloSimuladorRepositorio.ObterModeloSimuladorPorId(id);

                if (modeloBusca == null)
                    RegistroNaoEncontrado();

                _modeloSimuladorRepositorio.Excluir(modeloBusca.Id);
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

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
    }
}