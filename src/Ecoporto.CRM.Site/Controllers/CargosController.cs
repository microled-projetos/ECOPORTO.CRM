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
    public class CargosController : BaseController
    {
        private readonly ICargoRepositorio _cargosRepositorio;
        private readonly IControleAcessoRepositorio _controleAcessoRepositorio;

        public CargosController(
            ICargoRepositorio cargosRepositorio, 
            IControleAcessoRepositorio controleAcessoRepositorio, 
            ILogger logger) : base(logger)
        {
            _cargosRepositorio = cargosRepositorio;
            _controleAcessoRepositorio = controleAcessoRepositorio;
        }

        [HttpGet]
        [CanActivate(Roles = "Cargos:Acessar")]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult Consultar()
        {
            var resultado = _cargosRepositorio
                .ObterCargos()
                .Select(c => new
                {
                    Id = c.Id,
                    Descricao = c.Descricao,
                    Vendedor = c.Vendedor
                });

            return Json(new
            {
                dados = resultado
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public PartialViewResult ConsultarPorDescricao(string descricao)
        {
            var resultado = _cargosRepositorio
                .ObterCargoPorDescricao(descricao);

            return PartialView("_Consulta", resultado);
        }

        [HttpGet]
        [CanActivate(Roles = "Cargos:Cadastrar")]
        public ActionResult Cadastrar()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Cadastrar([Bind(Include = "Descricao, Vendedor")] CargosViewModel viewModel)
        {
            var cargo = new Cargo(viewModel.Descricao, viewModel.Vendedor);

            if (Validar(cargo))
            {
                cargo.Id = _cargosRepositorio.Cadastrar(cargo);

                if (!_controleAcessoRepositorio.ExistePermissaoNoCargo(cargo.Id))
                {
                    var menus = _controleAcessoRepositorio.ObterMenus().ToList();

                    PermissaoAcesso permissaoAcesso = new PermissaoAcesso();

                    foreach (var menu in menus)
                    {
                        var campos = menus
                            .Where(m => m.Id == menu.Id)
                            .SelectMany(c => c.Campos);

                        permissaoAcesso.IncluirPermissaoAcesso(new PermissaoAcesso
                        {
                            MenuId = menu.Id,
                            CargoId = cargo.Id,
                            Acessar = false,
                            Cadastrar = false,
                            Atualizar = false,
                            Excluir = false,
                            Logs = false,
                            Campos = campos
                        });
                    }

                    _controleAcessoRepositorio.AplicarPermissoes(cargo.Id, permissaoAcesso.PermissoesAcesso);
                }

                TempData["Sucesso"] = true;

                GravarLogAuditoria(TipoLogAuditoria.INSERT, cargo);
            }

            return View(viewModel);
        }

        [HttpGet]
        [CanActivate(Roles = "Cargos:Atualizar")]
        public ActionResult Atualizar(int? id)
        {
            if (id == null)
                return RedirectToAction(nameof(Index));

            var cargo = _cargosRepositorio.ObterCargoPorId(id.Value);

            if (cargo == null)
                RegistroNaoEncontrado();

            var viewModel = new CargosViewModel
            {
                Id = cargo.Id,
                Descricao = cargo.Descricao,
                Vendedor = cargo.Vendedor
            };

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Atualizar([Bind(Include = "Id, Descricao, Vendedor")] CargosViewModel viewModel, int? id)
        {
            if (id == null)
                return RedirectToAction(nameof(Index));

            var cargo = _cargosRepositorio.ObterCargoPorId(id.Value);

            if (cargo == null)
                RegistroNaoEncontrado();

            cargo.Alterar(new Cargo(viewModel.Descricao, viewModel.Vendedor));

            if (Validar(cargo))
            {
                _cargosRepositorio.Atualizar(cargo);

                TempData["Sucesso"] = true;

                GravarLogAuditoria(TipoLogAuditoria.UPDATE, cargo);
            }

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Excluir(int id)
        {
            try
            {
                var cargo = _cargosRepositorio.ObterCargoPorId(id);

                if (cargo == null)
                    RegistroNaoEncontrado();

                _cargosRepositorio.Excluir(cargo.Id);

                GravarLogAuditoria(TipoLogAuditoria.DELETE, cargo);
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }
    }
}