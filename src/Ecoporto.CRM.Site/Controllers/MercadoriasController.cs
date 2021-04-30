using Ecoporto.CRM.Business.Enums;
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
    public class MercadoriasController : BaseController
    {
        private readonly IMercadoriaRepositorio _mercadoriaRepositorio;
        private readonly ILogger _logger;

        public MercadoriasController(
            IMercadoriaRepositorio mercadoriaRepositorio, ILogger logger) : base(logger)
        {
            _mercadoriaRepositorio = mercadoriaRepositorio;
            _logger = logger;
        }

        [HttpGet]
        [CanActivate(Roles = "Mercadorias:Acessar")]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult Consultar()
        {
            var resultado = _mercadoriaRepositorio
                .ObterMercadorias()
                .Select(c => new
                {
                    Id = c.Id,
                    Descricao = c.Descricao,
                    Status = c.Status
                });

            return Json(new
            {
                dados = resultado
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [CanActivate(Roles = "Mercadorias:Cadastrar")]
        public ActionResult Cadastrar()
        {
            return View(new MercadoriaViewModel
            {
                Status = Status.ATIVO
            });
        }

        [HttpPost]
        public ActionResult Cadastrar([Bind(Include = "Descricao, Status")] MercadoriaViewModel viewModel)
        {
            var mercadoria = new Mercadoria(viewModel.Descricao, viewModel.Status);

            if (Validar(mercadoria))
            {
                mercadoria.Id = _mercadoriaRepositorio.Cadastrar(mercadoria);
                TempData["Sucesso"] = true;

                GravarLogAuditoria(TipoLogAuditoria.INSERT, mercadoria);
            }

            return View(viewModel);
        }

        [HttpGet]
        [CanActivate(Roles = "Mercadorias:Atualizar")]
        public ActionResult Atualizar(int? id)
        {
            if (id == null)
                return RedirectToAction(nameof(Index));

            var mercadoria = _mercadoriaRepositorio.ObterMercadoriaPorId(id.Value);

            if (mercadoria == null)
                RegistroNaoEncontrado();

            var viewModel = new MercadoriaViewModel
            {
                Id = mercadoria.Id,
                Descricao = mercadoria.Descricao,
                Status = mercadoria.Status
            };

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Atualizar([Bind(Include = "Id, Descricao, Status")] MercadoriaViewModel viewModel, int? id)
        {
            if (id == null)
                return RedirectToAction(nameof(Index));

            var mercadoria = _mercadoriaRepositorio.ObterMercadoriaPorId(id.Value);

            if (mercadoria == null)
                RegistroNaoEncontrado();

            mercadoria.Alterar(new Mercadoria(
                viewModel.Descricao,
                viewModel.Status));

            if (Validar(mercadoria))
            {
                _mercadoriaRepositorio.Atualizar(mercadoria);
                TempData["Sucesso"] = true;

                GravarLogAuditoria(TipoLogAuditoria.UPDATE, mercadoria);
            }

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Excluir(int id)
        {
            try
            {
                var mercadoria = _mercadoriaRepositorio.ObterMercadoriaPorId(id);

                if (mercadoria == null)
                    RegistroNaoEncontrado();

                _mercadoriaRepositorio.Excluir(mercadoria.Id);

                GravarLogAuditoria(TipoLogAuditoria.DELETE, mercadoria);
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }        
    }
}