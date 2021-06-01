using Ecoporto.CRM.Business.DTO;
using Ecoporto.CRM.Business.Enums;
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
    public class EquipesVendedorController : BaseController
    {
        private readonly IEquipeVendedorRepositorio _equipeVendedorRepositorio;
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IVendedorRepositorio _vendedorRepositorio;

        public EquipesVendedorController(
            IEquipeVendedorRepositorio equipeVendedorRepositorio,
            IUsuarioRepositorio usuarioRepositorio,
            IVendedorRepositorio vendedorRepositorio,
            ILogger logger) : base(logger)
        {
            _equipeVendedorRepositorio = equipeVendedorRepositorio;
            _usuarioRepositorio = usuarioRepositorio;
            _vendedorRepositorio = vendedorRepositorio;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult Consultar()
        {
            var resultado = _vendedorRepositorio
                .ObterVendedores()
                .Select(c => new
                {
                    c.Id,
                    c.Login,
                    c.Nome
                });

            return Json(new
            {
                dados = resultado
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult EquipeUsuarios(int? id)
        {
            if (id == null)
                return RedirectToAction(nameof(Index));

            var usuario = _usuarioRepositorio.ObterUsuarioPorId(id.Value);

            if (usuario == null)
                RegistroNaoEncontrado();

            var viewModel = new UsuarioViewModel
            {
                Id = usuario.Id,
                Login = usuario.Login,
                Nome = usuario.Nome,
                Email = usuario.Email,
                CargoId = usuario.CargoId,
                Administrador = usuario.Administrador,
                Ativo = usuario.Ativo
            };

            var vinculos = _equipeVendedorRepositorio.ObterUsuariosVinculados(usuario.Id);

            viewModel.Vinculos = vinculos;

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult VincularUsuario(int vendedorId, int usuarioId, int acessoConta, int acessoOportunidade, PapelEquipe papel)
        {    
            var equipeVendedor = new EquipeVendedor(
                vendedorId, 
                usuarioId,
                acessoConta,
                acessoOportunidade,
                papel);

            if (_equipeVendedorRepositorio.VinculoJaExistente(equipeVendedor))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "O usuário já está vinculado no Vendedor");
            }

            if (Validar(equipeVendedor))
            {
                _equipeVendedorRepositorio.Vincular(equipeVendedor);
            }

            var vinculos = _equipeVendedorRepositorio.ObterUsuariosVinculados(vendedorId);

            return PartialView("_ConsultarUsuariosVinculo", vinculos);
        }

        [HttpPost]
        public ActionResult AtualizarVinculo(int vinculoId, int acessoConta, int acessoOportunidade, PapelEquipe papel)
        {
            var vinculoBusca = _equipeVendedorRepositorio.ObterVinculoPorId(vinculoId);

            if (vinculoBusca == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Vínculo não encontrado");

            var equipeConta = new EquipeVendedor(
                vinculoBusca.VendedorId,
                vinculoBusca.UsuarioId,
                acessoConta,
                acessoOportunidade,
                papel);

            equipeConta.Id = vinculoId;

            if (Validar(equipeConta))
            {
                _equipeVendedorRepositorio.Atualizar(equipeConta);
            }

            var vinculos = _equipeVendedorRepositorio.ObterUsuariosVinculados(vinculoBusca.VendedorId);

            return PartialView("_ConsultarUsuariosVinculo", vinculos);
        }

        [HttpGet]
        public PartialViewResult ConsultarUsuariosPorNome(string nome)
        {
            var usuarios = _usuarioRepositorio.ObterUsuarios()
                 .Select(c => new EquipeVendedorUsuarioDTO
                 {
                     Id = c.Id,
                     Login = c.Login,
                     Nome = $"{c.Nome} ({c.Login})"
                 })
                 .Where(c => c.Nome.ToLower().Contains(nome.ToLower()))
                 .OrderBy(c => c.Login)
                 .ToList();

            return PartialView("_PesquisarUsuariosConsulta", usuarios);
        }

        [HttpPost]
        public ActionResult Excluir(int id)
        {
            try
            {
                var vinculo = _equipeVendedorRepositorio.ObterVinculoPorId(id);

                if (vinculo == null)
                    RegistroNaoEncontrado();

                _equipeVendedorRepositorio.Excluir(vinculo.Id);

                GravarLogAuditoria(TipoLogAuditoria.DELETE, vinculo);
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        [HttpGet]
        public ActionResult ObterDetalhesVinculo(int id)
        {
            var vinculoBusca = _equipeVendedorRepositorio.ObterVinculoPorId(id);

            if (vinculoBusca == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Registro não encontrado");

            return Json(new
            {
                vinculoBusca.Id,
                vinculoBusca.VendedorId,
                vinculoBusca.UsuarioId,
                vinculoBusca.UsuarioDescricao,
                vinculoBusca.AcessoConta,
                vinculoBusca.AcessoOportunidade,
                vinculoBusca.PapelEquipe
            }, JsonRequestBehavior.AllowGet);
        }
    }
}