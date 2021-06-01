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
    public class ServicosController : BaseController
    {
        private readonly IServicoRepositorio _servicoRepositorio;
        private readonly IServicoFaturamentoRepositorio _servicoFaturamentoRepositorio;
        private readonly ILayoutRepositorio _layoutRepositorio;

        public ServicosController(
            IServicoRepositorio servicoRepositorio,
            IServicoFaturamentoRepositorio servicoFaturamentoRepositorio,
            ILayoutRepositorio layoutRepositorio,
            ILogger logger) : base(logger)
        {
            _servicoRepositorio = servicoRepositorio;
            _servicoFaturamentoRepositorio = servicoFaturamentoRepositorio;
            _layoutRepositorio = layoutRepositorio;
        }

        [HttpGet]
        [CanActivate(Roles = "Servicos:Acessar")]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult Consultar()
        {
            var servicos = _servicoRepositorio
                .ObterServicos()
                .Select(c => new
                {
                    Id = c.Id,
                    Descricao = c.Descricao,
                    Status = c.Status,
                    RecintoAlfandegado = c.RecintoAlfandegado,
                    Operador = c.Operador,
                    Redex = c.Redex
                });

            return Json(new
            {
                dados = servicos
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [CanActivate(Roles = "Servicos:Cadastrar")]
        public ActionResult Cadastrar()
        {
            var viewModel = new ServicoViewModel
            {
                Status = Status.ATIVO
            };

            PopularServicos(viewModel);

            return View(viewModel);
        }

        private void PopularServicos(ServicoViewModel viewModel)
        {
            viewModel.ServicosFaturamento = _servicoFaturamentoRepositorio
                .ObterServicos()
                .ToList();
        }

        private void PopularServicosVinculados(ServicoViewModel viewModel)
        {
            var servicos = viewModel.ServicosSelecionados ?? viewModel.ServicosVinculados.Select(c => c.Id).ToArray();

            if (servicos.Length > 0)
                viewModel.ServicosVinculados = _servicoFaturamentoRepositorio
                    .ObterServicos(servicos)
                    .ToList();
        }

        [HttpPost]
        public ActionResult Cadastrar([Bind(Include = "Descricao, Status, RecintoAlfandegado, Operador, Redex, ServicosSelecionados")] ServicoViewModel viewModel)
        {
            var servicoBusca = _servicoRepositorio.ObterServicoPorDescricao(viewModel.Descricao);

            if (servicoBusca != null)
                ModelState.AddModelError(string.Empty, "Já existe um serviço cadastrado com a mesma descrição.");

            if (ModelState.IsValid)
            {
                var servico = new Servico(
                    viewModel.Descricao,
                    viewModel.Status,
                    viewModel.RecintoAlfandegado,
                    viewModel.Operador,
                    viewModel.Redex);

                foreach (var servicoSelecionado in viewModel.ServicosSelecionados)
                    servico.AdicionarServicoVinculado(new ServicoFaturamento(servicoSelecionado));

                if (Validar(servico))
                {
                    servico.Id = _servicoRepositorio.Cadastrar(servico);
                    TempData["Sucesso"] = true;

                    GravarLogAuditoria(TipoLogAuditoria.INSERT, servico);
                }
            }

            PopularServicos(viewModel);
            PopularServicosVinculados(viewModel);

            return View(viewModel);
        }

        [HttpGet]
        [CanActivate(Roles = "Servicos:Atualizar")]
        public ActionResult Atualizar(int? id)
        {
            if (id == null)
                return RedirectToAction(nameof(Index));

            var servico = _servicoRepositorio.ObterServicoPorId(id.Value);

            if (servico == null)
                RegistroNaoEncontrado();

            var viewModel = new ServicoViewModel
            {
                Id = servico.Id,
                Descricao = servico.Descricao,
                Status = servico.Status,
                RecintoAlfandegado = servico.RecintoAlfandegado,
                Operador = servico.Operador,
                Redex = servico.Redex,
                ServicosVinculados = servico.ServicosVinculados
            };

            PopularServicos(viewModel);
            PopularServicosVinculados(viewModel);

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Atualizar([Bind(Include = "Id, Descricao, Status, RecintoAlfandegado, Operador, Redex, ServicosSelecionados")] ServicoViewModel viewModel, int? id)
        {
            if (id == null)
                return RedirectToAction(nameof(Index));

            var servico = _servicoRepositorio.ObterServicoPorId(id.Value);

            if (servico == null)
                RegistroNaoEncontrado();

            var servicoBusca = _servicoRepositorio.ObterServicoPorDescricao(viewModel.Descricao, servico.Id);

            if (servicoBusca != null)
                ModelState.AddModelError(string.Empty, "Já existe um serviço cadastrado com a mesma descrição.");

            if (ModelState.IsValid)
            {
                servico.Alterar(new Servico(
                    viewModel.Descricao,
                    viewModel.Status,
                    viewModel.RecintoAlfandegado,
                    viewModel.Operador,
                    viewModel.Redex
                ));

                servico.ServicosVinculados.Clear();

                foreach (var servicoSelecionado in viewModel.ServicosSelecionados)
                    servico.AdicionarServicoVinculado(new ServicoFaturamento(servicoSelecionado));

                if (Validar(servico))
                {
                    _servicoRepositorio.Atualizar(servico);
                    TempData["Sucesso"] = true;

                    GravarLogAuditoria(TipoLogAuditoria.UPDATE, servico);
                }
            }

            PopularServicos(viewModel);
            PopularServicosVinculados(viewModel);

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Excluir(int id)
        {
            try
            {
                var servico = _servicoRepositorio.ObterServicoPorId(id);

                if (servico == null)
                    RegistroNaoEncontrado();

                var layout = _layoutRepositorio.ObterLayoutPorServico(servico.Id);

                if (layout != null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "O serviço já está sendo utilizado em um layout");
                
                _servicoRepositorio.Excluir(servico.Id);

                GravarLogAuditoria(TipoLogAuditoria.DELETE, servico);
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }        
    }
}