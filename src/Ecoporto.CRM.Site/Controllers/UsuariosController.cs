using Ecoporto.CRM.Business.Enums;
using Ecoporto.CRM.Business.Helpers;
using Ecoporto.CRM.Business.Interfaces.Repositorios;
using Ecoporto.CRM.Business.Models;
using Ecoporto.CRM.Infra.LDAP;
using Ecoporto.CRM.Site.Filtros;
using Ecoporto.CRM.Site.Models;
using NLog;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace Ecoporto.CRM.Site.Controllers
{
    [Authorize]
    public class UsuariosController : BaseController
    {
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly ICargoRepositorio _cargoRepositorio;
        private readonly IContaRepositorio _contaRepositorio;

        public UsuariosController(
            IUsuarioRepositorio usuarioRepositorio,
            ICargoRepositorio cargoRepositorio,
            IContaRepositorio contaRepositorio,
            ILogger logger) : base(logger)
        {
            _usuarioRepositorio = usuarioRepositorio;
            _cargoRepositorio = cargoRepositorio;
            _contaRepositorio = contaRepositorio;
        }

        [HttpGet]
        [CanActivate(Roles = "Usuarios:Acessar")]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult Consultar()
        {
            var usuarios = _usuarioRepositorio
                .ObterUsuarios()
                .Select(c => new
                {
                    Id = c.Id,
                    Login = c.Login != null ? c.Login : c.LoginExterno,
                    Nome = c.Nome,
                    Email = c.Email,
                    DescricaoCargo = c.DescricaoCargo,
                    Ativo = c.Ativo,
                    Administrador = c.Administrador,
                    Externo = c.Externo
                });

            return Json(new
            {
                dados = usuarios
            }, JsonRequestBehavior.AllowGet);
        }

        public void PopularCargos(UsuarioViewModel viewModel)
        {
            viewModel.Cargos = _cargoRepositorio
                .ObterCargos()
                .ToList();
        }

        public void PopularContas(UsuarioViewModel viewModel)
        {
            viewModel.Contas = _usuarioRepositorio
                .ObterVinculosContas(viewModel.Id)
                .ToList();
        }

        public IEnumerable<Usuario> PopularUsuarios(string dominio)
        {
            var activeDirectory = new ActiveDirectoryService(
                ConfigurationManager.AppSettings[$"{dominio}LDAP"].ToString(),
                ConfigurationManager.AppSettings["UsuarioLDAP"].ToString(),
                ConfigurationManager.AppSettings["SenhaLDAP"].ToString());

            return activeDirectory
                .ObterUsuarios()
                .Select(c => new Usuario
                {
                    Id = c.Id,
                    Login = c.Login,
                    Nome = $"{c.Nome} ({c.Login})",
                    Email = c.Email
                })
                .OrderBy(c => c.Login)
                .ToList();
        }

        [HttpGet]
        [CanActivate(Roles = "Usuarios:Cadastrar")]
        public ActionResult Cadastrar()
        {
            var viewModel = new UsuarioViewModel();

            PopularCargos(viewModel);

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Cadastrar([Bind(Include = "Login, LoginExterno, Senha, LoginWorkflow, Nome, Email, CPF, Administrador, Externo, Remoto, Ativo, CargoId, ValidarIP")] UsuarioViewModel viewModel)
        {
            var login = (viewModel.Externo
                ? viewModel.LoginExterno
                : viewModel.Login) ?? string.Empty;

            var usuarioBusca = _usuarioRepositorio.ObterUsuarioPorLogin(login);

            if (usuarioBusca != null)
                ModelState.AddModelError(string.Empty, "Já existe um usuário cadastrado utilizando este login.");

            if (ModelState.IsValid)
            {
                var usuario = new Usuario(
                    viewModel.Login,
                    viewModel.LoginExterno,
                    Criptografia.Encriptar(viewModel.Senha),
                    viewModel.LoginWorkflow,
                    viewModel.Nome,
                    viewModel.Email,
                    viewModel.CPF,
                    viewModel.CargoId,
                    viewModel.Administrador,
                    viewModel.Externo,
                    viewModel.Remoto,
                    viewModel.Ativo,
                    viewModel.ValidarIP);

                if (Validar(usuario))
                {
                    usuario.Id = _usuarioRepositorio.Cadastrar(usuario);

                    if (viewModel.Externo)
                    {
                        using (var ws = new WsControleSenha.Criptografia())
                        {
                            var resultado = ws.alterarSenha(viewModel.Senha, "CRM.TB_CRM_USUARIOS", viewModel.LoginExterno);

                            if (resultado != "OK")
                            {
                                _usuarioRepositorio.Excluir(usuario.Id);

                                ModelState.AddModelError(string.Empty, resultado);

                                PopularCargos(viewModel);

                                return View(viewModel);
                            }
                        }
                    }

                    GravarLogAuditoria(TipoLogAuditoria.INSERT, usuario);

                    return RedirectToAction(nameof(Atualizar), new { usuario.Id });
                }
            }

            PopularCargos(viewModel);

            return View(viewModel);
        }

        [HttpGet]
        [CanActivate(Roles = "Usuarios:Atualizar")]
        public ActionResult Atualizar(int? id)
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
                LoginExterno = usuario.LoginExterno,
                LoginWorkflow = usuario.LoginWorkflow,
                Nome = usuario.Nome,
                CPF = usuario.CPF,
                Email = usuario.Email,
                CargoId = usuario.CargoId,
                Administrador = usuario.Administrador,
                Externo = usuario.Externo,
                Remoto = usuario.Remoto,
                Ativo = usuario.Ativo,
                ValidarIP = usuario.ValidarIP
            };

            PopularContas(viewModel);
            PopularCargos(viewModel);

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Atualizar([Bind(Include = "Id, Login, LoginExterno, Senha, LoginWorkflow, Nome, Email, CPF, CargoId, Administrador, Externo, Remoto, Ativo, CargoId, ValidarIP")] UsuarioViewModel viewModel, int? id)
        {
            if (id == null)
                return RedirectToAction(nameof(Index));

            var usuarioBusca = _usuarioRepositorio.ObterUsuarioPorId(id.Value);

            if (usuarioBusca == null)
                RegistroNaoEncontrado();

            if (ModelState.IsValid)
            {
                var usuario = new Usuario(
                    viewModel.Login,
                    viewModel.LoginExterno,
                    viewModel.Senha,
                    viewModel.LoginWorkflow,
                    viewModel.Nome,
                    viewModel.Email,
                    viewModel.CPF,
                    viewModel.CargoId,
                    viewModel.Administrador,
                    viewModel.Externo,
                    viewModel.Remoto,
                    viewModel.Ativo,
                    viewModel.ValidarIP)
                {
                    Id = usuarioBusca.Id
                };

                if (viewModel.Externo)
                {
                    if (!string.IsNullOrEmpty(usuarioBusca.Senha) && string.IsNullOrEmpty(viewModel.Senha))
                    {
                        usuario.Senha = usuarioBusca.Senha;
                    }
                }

                if (Validar(usuario))
                {
                    var usuarioExistente = _usuarioRepositorio.ObterUsuarioPorLogin(viewModel.Externo
                        ? viewModel.LoginExterno
                        : viewModel.Login);

                    if (usuarioExistente != null && usuarioExistente.Id != usuarioBusca.Id)
                        ModelState.AddModelError(string.Empty, "Já existe um usuário cadastrado utilizando este login.");

                    if (viewModel.Externo && !string.IsNullOrEmpty(viewModel.Senha))
                    {
                        if (Criptografia.Encriptar(viewModel.Senha) != usuarioBusca.Senha)
                        {
                            using (var ws = new WsControleSenha.Criptografia())
                            {
                                var resultado = ws.alterarSenha(viewModel.Senha, "CRM.TB_CRM_USUARIOS", viewModel.LoginExterno);

                                if (resultado != "OK")
                                {
                                    ModelState.AddModelError(string.Empty, resultado);

                                    PopularContas(viewModel);
                                    PopularCargos(viewModel);

                                    return View(viewModel);
                                }

                                usuario.Senha = Criptografia.Encriptar(viewModel.Senha);
                                _usuarioRepositorio.AlterarSenha(usuario);
                            }
                        }
                    }

                    _usuarioRepositorio.Atualizar(usuario);

                    TempData["Sucesso"] = true;

                    GravarLogAuditoria(TipoLogAuditoria.UPDATE, usuario);
                }
            }

            PopularContas(viewModel);
            PopularCargos(viewModel);

            return View(viewModel);
        }

        [HttpGet]
        public PartialViewResult ConsultarUsuariosPorNome(string nome, string dominio)
        {
            var usuarios = PopularUsuarios(dominio)
                .Where(c => c.Nome.ToLower().Contains(nome.ToLower()))
                .ToList();

            return PartialView("_PesquisarUsuariosConsulta", usuarios);
        }

        [HttpGet]
        public ActionResult ObterDetalhesDoUsuario(string login, string dominio)
        {
            var usuarios = PopularUsuarios(dominio);

            var usuarioBusca = usuarios
                .Where(c => c.Login == login)
                .FirstOrDefault();

            if (usuarioBusca != null)
            {
                return Json(new
                {
                    usuarioBusca.Nome,
                    usuarioBusca.Email
                }, JsonRequestBehavior.AllowGet);
            }

            return null;
        }

        [HttpGet]
        [UsuarioExternoFilter]
        public PartialViewResult ConsultarContasPorDescricao(string descricao)
        {
            var resultado = _contaRepositorio.ObterContasPorDescricao(descricao, (int?)ViewBag.UsuarioExternoId);

            return PartialView("_PesquisarContasConsulta", resultado);
        }

        [HttpPost]
        public ActionResult VincularConta(int contaId, int usuarioId)
        {
            var contaBusca = _contaRepositorio.ObterContaPorId(contaId);

            if (contaBusca == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Conta não encontrada ou já excluída");

            var usuarioBusca = _usuarioRepositorio.ObterUsuarioPorId(usuarioId);

            if (usuarioBusca == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Usuário não encontrado");

            _usuarioRepositorio.VincularConta(contaBusca.Id, usuarioBusca.Id);

            var vinculos = _usuarioRepositorio.ObterVinculosContas(usuarioBusca.Id);

            return PartialView("_ConsultaVinculoContas", vinculos);
        }

        [HttpPost]
        public ActionResult ExcluirVinculoConta(int id)
        {
            var vinculoBusca = _usuarioRepositorio.ObterVinculoContaPorId(id);

            if (vinculoBusca == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Vínculo não encontrado ou já excluído");

            _usuarioRepositorio.ExcluirVinculoConta(id);

            var vinculos = _usuarioRepositorio.ObterVinculosContas(vinculoBusca.UsuarioId);

            return PartialView("_ConsultaVinculoContas", vinculos);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult ObterDetalhesDoUsuarioExterno(string login)
        {
            var usuarioBusca = _usuarioRepositorio.ObterUsuarioPorLogin(login);

            if (usuarioBusca != null)
            {
                return Json(new
                {
                    usuarioBusca.Nome,
                    usuarioBusca.Email,
                    usuarioBusca.Externo,
                    usuarioBusca.LoginExterno
                }, JsonRequestBehavior.AllowGet);
            }

            return null;
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult ObterUsuarioPorCPF(string cpf)
        {
            var usuarioBusca = _usuarioRepositorio.ObterUsuarioPorCPF(cpf);

            if (usuarioBusca != null)
            {
                return Json(new
                {
                    usuarioBusca.Nome,
                    usuarioBusca.Email,
                    usuarioBusca.Externo,
                    usuarioBusca.LoginExterno
                }, JsonRequestBehavior.AllowGet);
            }

            return null;
        }
    }
}