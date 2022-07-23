using Ecoporto.CRM.Business.Helpers;
using Ecoporto.CRM.Business.Interfaces.Repositorios;
using Ecoporto.CRM.Business.Interfaces.Servicos;
using Ecoporto.CRM.Business.Models;
using Ecoporto.CRM.Infra.Busca;
using Ecoporto.CRM.Infra.LDAP;
using Ecoporto.CRM.Site.Filtros;
using Ecoporto.CRM.Site.Helpers;
using Ecoporto.CRM.Site.Models;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.DirectoryServices;
using System.Linq;
using System.Runtime.Caching;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Text.RegularExpressions;

namespace Ecoporto.CRM.Site.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IBusca _buscaInterna;
        private readonly ILogger _logger;
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IContaRepositorio _contaRepositorio;
        private readonly IControleAcessoRepositorio _controleAcessoRepositorio;
        private readonly IControleAcessoService _httpService;
        private readonly IRelogioService _relogioService;
        private readonly IAmbienteOracleService _ambienteOracleService;

        public HomeController(
            IBusca buscaInterna,
            ILogger logger,
            IUsuarioRepositorio usuarioRepositorio,
            IContaRepositorio contaRepositorio,
            IControleAcessoRepositorio controleAcessoRepositorio,
            IControleAcessoService httpService,
            IRelogioService relogioService,
            IAmbienteOracleService ambienteOracleService)
        {
            _buscaInterna = buscaInterna;
            _logger = logger;
            _usuarioRepositorio = usuarioRepositorio;
            _contaRepositorio = contaRepositorio;
            _controleAcessoRepositorio = controleAcessoRepositorio;
            _httpService = httpService;
            _relogioService = relogioService;
            _ambienteOracleService = ambienteOracleService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public PartialViewResult ObterMenusDinamicos()
        {
            var menusDinamicos = _controleAcessoRepositorio.ObterMenusDinamicos();

            return PartialView("_MenusDinamicos", menusDinamicos);
        }

        [HttpGet]
        [UsuarioExternoFilter]
        public ActionResult Buscar(string criterio)
        {
            var resultados = _buscaInterna.Buscar(criterio, (int?)ViewBag.UsuarioExternoId);

            return Json(new
            {
                criterio,
                resultados
            }, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            return View(new UsuarioViewModel
            {
                ReturnUrl = returnUrl
            });
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(UsuarioViewModel viewModel)
        {
            try
            {
                var ipUsuario = string.Empty;

                ViewBag.Externo = false;

                if (!ModelState.IsValid)
                    return View(viewModel);

                var usuario = _usuarioRepositorio.ObterUsuarioPorLogin(viewModel.Login);

                if (usuario == null)
                    throw new Exception("Usuário não encontrado");

                ViewBag.Externo = usuario.Externo;

                if (usuario.Externo)
                {
                    try
                    {
                        if (!ValidarAcessoPorIP(usuario.Id, out ipUsuario, usuario.Remoto))
                        {
                            if (usuario.ValidarIP)
                            {
                                _httpService.LogarTentativaAcesso(
                                    usuario.Id,
                                    usuario.Externo,
                                    false,
                                    $"Usuário externo não obteve sucesso no login. Motivo: IP não autorizado",
                                    ipUsuario);

                                throw new Exception($"IP {ipUsuario} não autorizado");
                            }
                        }

                        ValidarAcessoUsuario(viewModel);
                    }
                    catch (Exception ex)
                    {
                        _httpService.LogarTentativaAcesso(
                            usuario.Id,
                            usuario.Externo,
                            false,
                            $"Usuário externo não obteve sucesso no login. Motivo / erro: {ex.Message}",
                            ipUsuario);

                        throw new Exception(ex.Message);
                    }
                }
                else
                {
                    if (!ValidarAcessoPorIP(usuario.Id, out ipUsuario, usuario.Remoto))
                    {
                        var mensagemErro = "Usuário sem permissão para acessar externamente - " + ipUsuario;

                        _httpService.LogarTentativaAcesso(
                            usuario.Id,
                            usuario.Externo,
                            false,
                            mensagemErro,
                            ipUsuario);

                        throw new Exception(mensagemErro);
                    }

                    try

                    {

                        
                       
                       ValidarUsuarioDominio(viewModel, usuario);
                    }
                    catch (Exception ex)
                    {
                        _httpService.LogarTentativaAcesso(
                            usuario.Id,
                            usuario.Externo,
                            false,
                            $"Usuário não obteve sucesso no login. Motivo / erro: {ex.Message}",
                            ipUsuario);

                        throw new Exception(ex.Message);
                    }
                }

                if (usuario.Ativo == false)
                    throw new Exception($"O usuário {usuario.ToString()} está inativo");

                var usuarioJson = JsonConvert.SerializeObject(new
                {
                    usuario.Id,
                    usuario.Nome,
                    Login = usuario.Externo ? usuario.LoginExterno : usuario.Login,
                    Email = usuario.Email ?? string.Empty
                });

                FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
                    1,
                    usuario.ToString(),
                    DateTime.Now,
                    DateTime.Now.AddMinutes(480),
                    viewModel.Lembrar,
                    usuarioJson);

                Response.Cookies.Add(
                    new HttpCookie(
                        FormsAuthentication.FormsCookieName,
                        FormsAuthentication.Encrypt(authTicket)));

                MemoryCache cache = MemoryCache.Default;

                cache.Set(
                    $"U-{usuario.ToString()}.Permissoes",
                    string.Join(",", ObterPermissoesUsuario(usuario)),
                    new DateTimeOffset(DateTime.Now.AddHours(8)));

                if (!string.IsNullOrEmpty(viewModel.ReturnUrl))
                    return Redirect(Server.UrlDecode(viewModel.ReturnUrl));

                _httpService.LogarTentativaAcesso(
                    usuario.Id,
                    usuario.Externo,
                    true,
                    $"Usuário efetuou login com sucesso",
                    ipUsuario);

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ViewBag.Mensagem = ex.Message;
            }

            return View(viewModel);
        }

        private List<string> ObterPermissoesUsuario(Usuario usuario)
        {
            var permissoes = _controleAcessoRepositorio.ObterPermissoes(usuario.CargoId);

            var menus = new List<string>();

            menus.AddRange(permissoes.Where(c => c.Acessar)
                .Select(c => string.Concat(c.Descricao, ":", nameof(c.Acessar))));

            menus.AddRange(permissoes.Where(c => c.Cadastrar)
                .Select(c => string.Concat(c.Descricao, ":", nameof(c.Cadastrar))));

            menus.AddRange(permissoes.Where(c => c.Atualizar)
                .Select(c => string.Concat(c.Descricao, ":", nameof(c.Atualizar))));

            menus.AddRange(permissoes.Where(c => c.Excluir)
                .Select(c => string.Concat(c.Descricao, ":", nameof(c.Excluir))));

            menus.AddRange(permissoes.Where(c => c.Logs)
                .Select(c => string.Concat(c.Descricao, ":", nameof(c.Logs))));

            if (usuario.Administrador)
                menus.Add("Administrador");

            menus.Sort();

            foreach (var menu in permissoes)
            {
                foreach (var campo in menu.Campos
                    .Where(c => (c.TipoPermissao == TipoPermissao.LEITURA_ESCRITA) && c.MenuId == menu.Id))
                {
                    menus.Add($"{menu.Descricao}:{campo.ObjetoId}");
                }

                foreach (var campo in menu.Campos
                    .Where(c => (c.TipoPermissao == TipoPermissao.ACESSO_TOTAL) && c.MenuId == menu.Id))
                {
                    menus.Add($"{menu.Descricao}:{campo.ObjetoId}_Full");
                }
            }

            if (permissoes.SelectMany(c => c.Campos.Where(d => d.TipoPermissao == TipoPermissao.ACESSO_TOTAL)).Any())
                menus.Add("OportunidadeFullControll");

            foreach (var menu in permissoes)
            {
                foreach (var campo in menu.Campos
                    .Where(c => (c.TipoPermissao == TipoPermissao.ACESSO_TOTAL) && c.MenuId == 9))
                {
                    if (campo.ObjetoId == "Anexo")
                    {
                        if (!menus.Contains("OportunidadeAnexosFullControll"))
                            menus.Add("OportunidadeAnexosFullControll");
                    }                    
                }
            }

            if (usuario.Externo)
                menus.Add("UsuarioExterno");

            if (_ambienteOracleService.Ambiente() == Business.Enums.AmbienteOracle.DEV)
                menus.Add("BaseDesenvolvimento");
            else
                menus.Add("BaseProducao");

            return menus;
        }

        private static void ValidarUsuarioDominio(UsuarioViewModel viewModel, Usuario usuario)
        {
            var servidorLDAP = string.Empty;

            switch (usuario.Dominio)
            {
                case 1:
                    servidorLDAP = ConfigurationManager.AppSettings["EcoportoLDAP"].ToString();
                    break;
                case 2:
                    servidorLDAP = ConfigurationManager.AppSettings["EcopatioLDAP"].ToString();
                    break;
                case 3:
                    servidorLDAP = ConfigurationManager.AppSettings["EcorodoviasLDAP"].ToString();
                    break;
            }

            var activeDirectory = new ActiveDirectoryService(servidorLDAP, viewModel.Login, viewModel.Senha);

            var directorySearcher = activeDirectory.DirectorySearcher();
            directorySearcher.Filter = "(SAMAccountName=" + viewModel.Login + ")";

            SearchResult searchResult = directorySearcher.FindOne();
        }

        private static void ValidarAcessoUsuario(UsuarioViewModel viewModel)
        {
            using (var ws = new WsControleSenha.Criptografia())
            {
                var resultado = ws.ValidarUsuario("CRM.TB_CRM_USUARIOS", viewModel.Login, Criptografia.Encriptar(viewModel.Senha));

                if (resultado != "AUTORIZADO")
                {
                    throw new Exception(resultado);
                }
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult TrocarSenha(string login)
        {
            return View(new AlterarSenhaViewModel
            {
                Login = string.IsNullOrEmpty(login)
                    ? User.Identity.Name
                    : login
            });
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult TrocarSenha(AlterarSenhaViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            var usuario = _usuarioRepositorio.ObterUsuarioPorLogin(viewModel.Login);

            if (usuario != null)
            {
                if (!usuario.Externo)
                {
                    ModelState.AddModelError(string.Empty, "Opção disponível apenas para usuários Externos.");
                    return View(viewModel);
                }

                if (usuario.Ativo == false)
                    ModelState.AddModelError(string.Empty, $"O usuário {usuario.ToString()} está inativo");

                if (!usuario.Autenticar(viewModel.Login, viewModel.SenhaAtual))
                    ModelState.AddModelError(string.Empty, $"A senha atual não confere");

                if (viewModel.NovaSenha != viewModel.ConfirmacaoNovaSenha)
                    ModelState.AddModelError(string.Empty, $"O campo Nova Senha e Confirmação não são iguais");

                if (ModelState.IsValid)
                {
                    using (var ws = new WsControleSenha.Criptografia())
                    {
                        var resultado = ws.alterarSenha(viewModel.NovaSenha, "CRM.TB_CRM_USUARIOS", usuario.LoginExterno);

                        if (resultado != "OK")
                        {
                            ModelState.AddModelError(string.Empty, resultado);
                            return View(viewModel);
                        }

                        usuario.AlterarSenha(viewModel.NovaSenha);

                        _usuarioRepositorio.AlterarSenha(usuario);

                        TempData["Sucesso"] = true;
                    }
                }

                return View(viewModel);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Usuário não encontrado");
            }

            return View(viewModel);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult ResetarSenha()
        {
            return View(new ResetarSenhaViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult ResetarSenha(ResetarSenhaViewModel viewModel)
        {
            var usuario = _usuarioRepositorio.ObterUsuarioPorCPF(viewModel.CPF);

            if (usuario == null)
            {
                ModelState.AddModelError(string.Empty, "Usuário não encontrado");
                return View(viewModel);
            }

            if (!usuario.Externo)
            {
                ModelState.AddModelError(string.Empty, "Opção disponível apenas para usuários Externos.");
                return View(viewModel);
            }

            if (usuario.Ativo == false)
                ModelState.AddModelError(string.Empty, $"O usuário {usuario.ToString()} está inativo");

            if (viewModel.NovaSenha != viewModel.ConfirmacaoNovaSenha)
                ModelState.AddModelError(string.Empty, $"O campo Nova Senha e Confirmação não são iguais");

            if (ModelState.IsValid)
            {
                using (var ws = new WsControleSenha.Criptografia())
                {
                    var resultado = ws.ValidarUsuarioCPF("CRM.TB_CRM_USUARIOS", usuario.LoginExterno, usuario.CPF);

                    if (resultado != "AUTORIZADO")
                    {
                        ModelState.AddModelError(string.Empty, resultado);
                        return View(viewModel);
                    }

                    resultado = ws.alterarSenha(viewModel.NovaSenha, "CRM.TB_CRM_USUARIOS", usuario.LoginExterno);

                    if (resultado != "OK")
                    {
                        ModelState.AddModelError(string.Empty, resultado);
                        return View(viewModel);
                    }

                    usuario.AlterarSenha(viewModel.NovaSenha);

                    _usuarioRepositorio.AlterarSenha(usuario);

                    TempData["Sucesso"] = true;
                }
            }

            return View(viewModel);
        }

        public ActionResult Erro(string ticket, string exception)
        {
            return View(new ErroViewModel
            {
                Data = DateTime.Now,
                Ticket = ticket,
                Exception = exception
            });
        }

        public ActionResult Indisponivel()
        {
            return View();
        }

        public ActionResult AcessoNegado()
        {
            return View();
        }

        public ActionResult NaoEncontrado()
        {
            return View();
        }

        private bool ValidarAcessoPorIP(int usuarioId, out string ipUsuario, bool remoto)
        {
            // Acessando internamente
            if (!Internet.AcessoViaInternet(out ipUsuario) || remoto)
            {
                return true;
            }

            var vinculosContas = _usuarioRepositorio
                .ObterVinculosContas(usuarioId);

            if (!vinculosContas.Any())
                throw new Exception("Seu usuário não está vinculado a nenhuma Conta");

            var ips = new List<ControleAcessoConta>();

            foreach (var vinculo in vinculosContas)
                ips.AddRange(_contaRepositorio.ObterVinculosIPs(vinculo.ContaId));

            foreach (var ip in ips)
            {
                if (Internet.ContemIP(ip.IPInicial, ip.IPFinal, ipUsuario))
                    return true;
            }

            return false;
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();

            HttpCookie aCookie;
            string cookieName;
            int limit = Request.Cookies.Count;
            for (int i = 0; i < limit; i++)
            {
                cookieName = Request.Cookies[i].Name;
                aCookie = new HttpCookie(cookieName);
                aCookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(aCookie);
            }

            return RedirectToAction("Index", "Home");
        }
    }
}