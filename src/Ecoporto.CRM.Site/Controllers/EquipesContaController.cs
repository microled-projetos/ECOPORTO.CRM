using Ecoporto.CRM.Business.DTO;
using Ecoporto.CRM.Business.Enums;
using Ecoporto.CRM.Business.Interfaces.Repositorios;
using Ecoporto.CRM.Business.Models;
using Ecoporto.CRM.Infra.Datatables;
using Ecoporto.CRM.Site.Models;
using NLog;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace Ecoporto.CRM.Site.Controllers
{
    [Authorize]
    public class EquipesContaController : BaseController
    {
        private readonly IEquipeContaRepositorio _equipeContaRepositorio;
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IContaRepositorio _contaRepositorio;
        private readonly IVendedorRepositorio _vendedorRepositorio;

        public EquipesContaController(
            IEquipeContaRepositorio equipeContaRepositorio,
            IUsuarioRepositorio usuarioRepositorio,
            IContaRepositorio contaRepositorio,
            IVendedorRepositorio vendedorRepositorio,
            ILogger logger) : base(logger)
        {
            _equipeContaRepositorio = equipeContaRepositorio;
            _usuarioRepositorio = usuarioRepositorio;
            _contaRepositorio = contaRepositorio;
            _vendedorRepositorio = vendedorRepositorio;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult EquipeUsuarios(int? id)
        {
            if (id == null)
                return RedirectToAction(nameof(Index));

            var conta = _contaRepositorio.ObterContaPorId(id.Value);

            if (conta == null)
                RegistroNaoEncontrado();

            var vendedor = _vendedorRepositorio.ObterVendedorPorId(conta.VendedorId);

            if (vendedor == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Esta conta não possui nenhum Vendedor vinculado");

            var viewModel = new EquipeContaViewModel
            {
                ContaId = conta.Id,
                Descricao = conta.Descricao,
                Documento = conta.Documento,
                VendedorId = vendedor.Id,
                VendedorNome = vendedor.Nome
            };

            var vinculos = _equipeContaRepositorio.ObterUsuariosVinculados(conta.Id);

            viewModel.Vinculos = vinculos;

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult VincularUsuario(int contaId, int usuarioId, int acessoConta, int acessoOportunidade, PapelEquipe papel)
        {
            var equipeConta = new EquipeConta(
                contaId,
                usuarioId,
                acessoConta,
                acessoOportunidade,
                papel);

            if (_equipeContaRepositorio.VinculoJaExistente(equipeConta))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "O usuário já está vinculado na Conta");
            }

            if (Validar(equipeConta))
            {
                _equipeContaRepositorio.Vincular(equipeConta);
            }

            var vinculos = _equipeContaRepositorio.ObterUsuariosVinculados(contaId);

            return PartialView("_ConsultarUsuariosVinculo", vinculos);
        }

        [HttpPost]
        public ActionResult AtualizarVinculo(int vinculoId, int acessoConta, int acessoOportunidade, PapelEquipe papel)
        {
            var vinculoBusca = _equipeContaRepositorio.ObterVinculoPorId(vinculoId);

            if (vinculoBusca == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Vínculo não encontrado");

            var equipeConta = new EquipeConta(
                vinculoBusca.ContaId,
                vinculoBusca.UsuarioId,
                acessoConta,
                acessoOportunidade,
                papel);

            equipeConta.Id = vinculoId;

            if (Validar(equipeConta))
            {
                _equipeContaRepositorio.Atualizar(equipeConta);
            }

            var vinculos = _equipeContaRepositorio.ObterUsuariosVinculados(vinculoBusca.ContaId);

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
                 .Where(c => c.Nome.Contains(nome))
                 .OrderBy(c => c.Login)
                 .ToList();

            return PartialView("_PesquisarUsuariosConsulta", usuarios);
        }

        [HttpPost]
        public ActionResult Excluir(int id)
        {
            try
            {
                var vinculo = _equipeContaRepositorio.ObterVinculoPorId(id);

                if (vinculo == null)
                    RegistroNaoEncontrado();

                _equipeContaRepositorio.Excluir(vinculo.Id);

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
            var vinculoBusca = _equipeContaRepositorio.ObterVinculoPorId(id);

            if (vinculoBusca == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Registro não encontrado");

            return Json(new
            {
                vinculoBusca.Id,
                vinculoBusca.ContaId,
                vinculoBusca.UsuarioId,
                vinculoBusca.UsuarioDescricao,
                vinculoBusca.AcessoConta,
                vinculoBusca.AcessoOportunidade,
                vinculoBusca.PapelEquipe
            }, JsonRequestBehavior.AllowGet);
        }
    }
}