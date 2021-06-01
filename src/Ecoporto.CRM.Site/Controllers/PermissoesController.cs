using Ecoporto.CRM.Business.Enums;
using Ecoporto.CRM.Business.Interfaces.Repositorios;
using Ecoporto.CRM.Business.Models;
using Ecoporto.CRM.Site.Filtros;
using Ecoporto.CRM.Site.Models;
using NLog;
using System.Linq;
using System.Web.Mvc;

namespace Ecoporto.CRM.Site.Controllers
{
    [Authorize]
    public class PermissoesController : BaseController
    {
        private readonly ICargoRepositorio _cargoRepositorio;
        private readonly IControleAcessoRepositorio _controleAcessoRepositorio;
        private readonly ILogger _logger;

        public PermissoesController(
            ICargoRepositorio cargoRepositorio,
            IControleAcessoRepositorio controleAcessoRepositorio,
            ILogger logger) : base(logger)
        {
            _cargoRepositorio = cargoRepositorio;
            _controleAcessoRepositorio = controleAcessoRepositorio;
            _logger = logger;
        }

        private void PopularCargos(PermissaoViewModel permissaoViewModel)
        {
            permissaoViewModel.Cargos = _cargoRepositorio
                .ObterCargos()
                .ToList();
        }

        private void PopularMenus(PermissaoViewModel permissaoViewModel)
        {
            permissaoViewModel.Menus = _controleAcessoRepositorio
                .ObterMenus()
                .Where(c => c.Dinamico == false)
                .ToList();
        }

        private void PopularPermissoes(PermissaoViewModel permissaoViewModel)
        {
            permissaoViewModel.Menus = _controleAcessoRepositorio
                .ObterPermissoes(permissaoViewModel.CargoId)
                .ToList();
        }

        [HttpGet]
        [CanActivate(Roles = "Administrador")]
        public ActionResult Cadastrar(int? cargoId)
        {
            if (!cargoId.HasValue)
                return RedirectToAction("Index", "Cargos");

            var cargo = _cargoRepositorio.ObterCargoPorId(cargoId.Value);

            if (cargo == null)
                RegistroNaoEncontrado();
            
            PermissaoViewModel viewModel = new PermissaoViewModel
            {
                CargoId = cargo.Id
            };

            PopularCargos(viewModel);
            PopularPermissoes(viewModel);

            if (viewModel.Menus.Count > 0)
            {                              
                return View(viewModel);
            }

            PopularMenus(viewModel);

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Cadastrar([Bind(Include = "CargoId, Menus")] PermissaoViewModel viewModel)
        {
            var cargo = _cargoRepositorio.ObterCargoPorId(viewModel.CargoId);

            if (cargo == null)
                RegistroNaoEncontrado();

            PermissaoAcesso permissaoAcesso = new PermissaoAcesso();

            foreach (var menu in viewModel.Menus)
            {
                var campos = viewModel.Menus
                    .Where(m => m.Id == menu.Id)
                        .SelectMany(c => c.Campos);

                permissaoAcesso.IncluirPermissaoAcesso(new PermissaoAcesso
                {
                    MenuId = menu.Id,
                    CargoId = viewModel.CargoId,
                    Acessar = menu.Acessar,
                    Cadastrar = menu.Cadastrar,
                    Atualizar = menu.Atualizar,
                    Excluir = menu.Excluir,
                    Logs = menu.Logs,
                    Campos = campos
                });
            }

            if (Validar(permissaoAcesso))
            {
                _controleAcessoRepositorio.AplicarPermissoes(viewModel.CargoId, permissaoAcesso.PermissoesAcesso);

                GravarLogAuditoria(TipoLogAuditoria.INSERT, permissaoAcesso);

                TempData["Sucesso"] = true;               
            }
            
            PopularCargos(viewModel);
            PopularPermissoes(viewModel);

            return View(viewModel);
        }
    }
}