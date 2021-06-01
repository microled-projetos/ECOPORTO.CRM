using Ecoporto.CRM.Business.Enums;
using Ecoporto.CRM.Business.Interfaces.Repositorios;
using Ecoporto.CRM.Business.Models;
using Ecoporto.CRM.Infra.Datatables;
using Ecoporto.CRM.Site.Filtros;
using Ecoporto.CRM.Site.Models;
using NLog;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace Ecoporto.CRM.Site.Controllers
{
    [Authorize]
    public class ContatosController : BaseController
    {
        private readonly IContatoRepositorio _contatoRepositorio;
        private readonly IEquipeContaRepositorio _equipeContaRepositorio;
        private readonly IContaRepositorio _contaRepositorio;

        public ContatosController(
            IContatoRepositorio contatoRepositorio, IContaRepositorio contaRepositorio, IEquipeContaRepositorio equipeContaRepositorio, ILogger logger) : base(logger)
        {
            _contatoRepositorio = contatoRepositorio;
            _contaRepositorio = contaRepositorio;
            _equipeContaRepositorio = equipeContaRepositorio;
        }

        [HttpGet]
        [CanActivate(Roles = "Contatos:Acessar")]
        public ActionResult Index()
        {
            return View();
        }       

        [HttpGet]
        [UsuarioExternoFilter]
        public PartialViewResult ConsultarContasPorDescricao(string descricao, Segmento? segmento)
        {
            var resultado = _contaRepositorio.ObterContasPorDescricao(descricao, (int?)ViewBag.UsuarioExternoId);

            return PartialView("_PesquisarContasConsulta", resultado);
        }

        private void PopularContas(ContatoViewModel viewModel)
        {
            var conta = _contaRepositorio.ObterContaPorId(viewModel.ContaId);
            viewModel.Contas.Add(conta);
        }

        [HttpGet]
        public ActionResult Cadastrar(int? conta)
        {
            var viewModel = new ContatoViewModel
            {
                Status = Status.ATIVO
            };

            if (conta.HasValue)
            {
                viewModel.TelaContas = true;

                var contaBusca = _contaRepositorio.ObterContaPorId(conta.Value);

                if (contaBusca == null)
                    RegistroNaoEncontrado();

                viewModel.ContaId = conta.Value;
                PopularContas(viewModel);
            }

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Cadastrar([Bind(Include = "Nome, Sobrenome, Telefone, Celular, Email, Cargo, Departamento, DataNascimento, Status, ContaId, TelaContas")] ContatoViewModel viewModel)
        {
            var contato = new Contato(
                viewModel.Nome,
                viewModel.Sobrenome,
                viewModel.Telefone,
                viewModel.Celular,
                viewModel.Email,
                viewModel.Cargo,
                viewModel.Departamento,
                viewModel.DataNascimento,
                viewModel.Status,
                viewModel.ContaId);

            if (Validar(contato))
            {
                contato.Id = _contatoRepositorio.Cadastrar(contato);
                TempData["Sucesso"] = true;

                GravarLogAuditoria(TipoLogAuditoria.INSERT, contato);
            }

            PopularContas(viewModel);

            return View(viewModel);
        }

        [HttpGet]
        [CanActivateVisualizarContatos(Roles = "Contatos:Atualizar")]
        public ActionResult Atualizar(int? id)
        {
            if (id == null)
                return RedirectToAction(nameof(Index));

            var contato = _contatoRepositorio.ObterContatoPorId(id.Value);

            if (contato == null)
                RegistroNaoEncontrado();

            var viewModel = new ContatoViewModel
            {
                Id = contato.Id,
                Nome = contato.Nome,
                Sobrenome = contato.Sobrenome,
                Email = contato.Email,
                Cargo = contato.Cargo,
                Celular = contato.Celular,
                Departamento = contato.Departamento,
                Telefone = contato.Telefone,
                DataNascimento = contato.DataNascimento,
                Status = contato.Status,
                ContaId = contato.ContaId
            };

            PopularContas(viewModel);

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Atualizar([Bind(Include = "Id, Nome, Sobrenome, Telefone, Celular, Email, Cargo, Departamento, DataNascimento, Status, ContaId")] ContatoViewModel viewModel, int? id)
        {
            if (id == null)
                return RedirectToAction(nameof(Index));

            var contato = _contatoRepositorio.ObterContatoPorId(id.Value);

            if (contato == null)
                RegistroNaoEncontrado();

            contato.Alterar(new Contato(
                viewModel.Nome,
                viewModel.Sobrenome,
                viewModel.Telefone,
                viewModel.Celular,
                viewModel.Email,
                viewModel.Cargo,
                viewModel.Departamento,
                viewModel.DataNascimento,
                viewModel.Status,
                viewModel.ContaId));

            if (Validar(contato))
            {
                _contatoRepositorio.Atualizar(contato);
                TempData["Sucesso"] = true;

                GravarLogAuditoria(TipoLogAuditoria.UPDATE, contato);
            }

            PopularContas(viewModel);

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Excluir(int id)
        {
            try
            {
                var contato = _contatoRepositorio.ObterContatoPorId(id);

                if (contato == null)
                    RegistroNaoEncontrado();

                if (!User.IsInRole("Administrador"))
                {                                        
                    var login = User.Identity.Name;

                    var permissoesPorVendedor = _equipeContaRepositorio
                        .ObterPermissoesContaPorVendedor(contato.ContaId, login);

                    var permissoesPorConta = _equipeContaRepositorio
                        .ObterPermissoesContaPorConta(contato.ContaId, login);

                    if (permissoesPorVendedor == null)
                    {
                        if (permissoesPorConta == null)
                        {
                            if (!User.IsInRole("Contatos:Excluir"))
                            {
                                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Usuário não possui permissão para excluir Contatos");
                            }                            
                        }
                    }
                    else
                    {
                        if (permissoesPorVendedor.AcessoConta == 0)
                        {
                            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Usuário não possui permissão para excluir Contatos");
                        }
                    }
                }

                _contatoRepositorio.Excluir(contato.Id);

                GravarLogAuditoria(TipoLogAuditoria.DELETE, contato);
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        [HttpGet]
        public JsonResult ObterContatosPorConta(int contaId)
        {
            var contatos = _contatoRepositorio
                .ObterContatosPorConta(contaId)
                .Select(c => new
                {
                    c.Id,
                    c.NomeCompleto
                });

            return Json(contatos, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [UsuarioExternoFilter]
        public PartialViewResult ObterContatosEContaPorDescricao(string descricao)
        {
            var resultado = _contatoRepositorio.ObterContatosEContaPorDescricao(descricao, (int?)ViewBag.UsuarioExternoId);

            return PartialView("_PesquisarContatosConsulta", resultado);
        }
    }
}