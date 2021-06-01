using Ecoporto.CRM.Business.DTO;
using Ecoporto.CRM.Business.Enums;
using Ecoporto.CRM.Business.Extensions;
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
    public class EquipesOportunidadeController : BaseController
    {
        private readonly IEquipeOportunidadeRepositorio _equipeOportunidadeRepositorio;
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IOportunidadeRepositorio _oportunidadeRepositorio;
        private readonly IVendedorRepositorio _vendedorRepositorio;

        public EquipesOportunidadeController(
            IEquipeOportunidadeRepositorio equipeOportunidadeRepositorio,
            IUsuarioRepositorio usuarioRepositorio,
            IOportunidadeRepositorio oportunidadeRepositorio,
            IVendedorRepositorio vendedorRepositorio,
            ILogger logger) : base(logger)
        {
            _equipeOportunidadeRepositorio = equipeOportunidadeRepositorio;
            _usuarioRepositorio = usuarioRepositorio;
            _oportunidadeRepositorio = oportunidadeRepositorio;
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

            var oportunidade = _oportunidadeRepositorio.ObterOportunidadePorId(id.Value);

            if (oportunidade == null)
                RegistroNaoEncontrado();

            var vendedor = _vendedorRepositorio.ObterVendedorPorId(oportunidade.OportunidadeProposta.VendedorId);

            var viewModel = new EquipeOportunidadeViewModel
            {
                OportunidadeId = oportunidade.Id,
                Proposta = oportunidade.Identificacao,
                Descricao = oportunidade.Descricao,
                StatusOportunidade = oportunidade.StatusOportunidade.ToName(),
                Vendedor = vendedor?.Nome
            };

            var vinculos = _equipeOportunidadeRepositorio.ObterUsuariosVinculados(oportunidade.Id);

            viewModel.Vinculos = vinculos;

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult VincularUsuario(int oportunidadeId, int usuarioId, int acessoConta, int acessoOportunidade, PapelEquipe papel)
        {
            var equipeOportunidade = new EquipeOportunidade(
                oportunidadeId,
                usuarioId,
                acessoConta,
                acessoOportunidade,
                papel);

            if (_equipeOportunidadeRepositorio.VinculoJaExistente(equipeOportunidade))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "O usuário já está vinculado na Oportunidade");
            }

            if (Validar(equipeOportunidade))
            {
                _equipeOportunidadeRepositorio.Vincular(equipeOportunidade);
            }

            var vinculos = _equipeOportunidadeRepositorio.ObterUsuariosVinculados(oportunidadeId);

            return PartialView("_ConsultarUsuariosVinculo", vinculos);
        }

        [HttpPost]
        public ActionResult AtualizarVinculo(int vinculoId, int acessoConta, int acessoOportunidade, PapelEquipe papel)
        {
            var vinculoBusca = _equipeOportunidadeRepositorio.ObterVinculoPorId(vinculoId);

            if (vinculoBusca == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Vínculo não encontrado");

            var equipeConta = new EquipeOportunidade(
                vinculoBusca.OportunidadeId,
                vinculoBusca.UsuarioId,
                acessoConta,
                acessoOportunidade,
                papel);

            equipeConta.Id = vinculoId;

            if (Validar(equipeConta))
            {
                _equipeOportunidadeRepositorio.Atualizar(equipeConta);
            }

            var vinculos = _equipeOportunidadeRepositorio.ObterUsuariosVinculados(vinculoBusca.OportunidadeId);

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
                var vinculo = _equipeOportunidadeRepositorio.ObterVinculoPorId(id);

                if (vinculo == null)
                    RegistroNaoEncontrado();

                _equipeOportunidadeRepositorio.Excluir(vinculo.Id);

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
            var vinculoBusca = _equipeOportunidadeRepositorio.ObterVinculoPorId(id);

            if (vinculoBusca == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Registro não encontrado");

            return Json(new
            {
                vinculoBusca.Id,
                vinculoBusca.OportunidadeId,
                vinculoBusca.UsuarioId,
                vinculoBusca.UsuarioDescricao,
                vinculoBusca.AcessoConta,
                vinculoBusca.AcessoOportunidade,
                vinculoBusca.PapelEquipe
            }, JsonRequestBehavior.AllowGet);
        }
    }
}