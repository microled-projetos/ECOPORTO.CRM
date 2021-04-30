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
    public class MotivosController : BaseController
    {
        private readonly IMotivosRepositorio _motivosRepositorio;        

        public MotivosController(
            IMotivosRepositorio motivosRepositorio,            
            ILogger logger) : base(logger)
        {
            _motivosRepositorio = motivosRepositorio;            
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        public void PopularMotivos(SolicitacaoComercialMotivosViewModel solicitacaoComercialMotivosViewModel)
        {
            var motivos = _motivosRepositorio.ObterSolicitacoesMotivo();

            solicitacaoComercialMotivosViewModel.Motivos = motivos.ToList();
        }

        [HttpGet]
        public ActionResult Cadastrar()
        {
            var viewModel = new SolicitacaoComercialMotivosViewModel();

            PopularMotivos(viewModel);

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Cadastrar([Bind(Include = "SolicitacaoId, Descricao, Status, CancelamentoNF, Desconto, Restituicao, ProrrogacaoBoleto, Outros")] SolicitacaoComercialMotivosViewModel viewModel)
        {
            var solicitacaoMotivo = new SolicitacaoComercialMotivo(
                viewModel.Descricao,
                viewModel.CancelamentoNF,
                viewModel.Desconto,
                viewModel.Restituicao,
                viewModel.ProrrogacaoBoleto,
                viewModel.Outros,
                viewModel.Status);

            if (Validar(solicitacaoMotivo))
            {
                _motivosRepositorio.CadastrarMotivo(solicitacaoMotivo);
                TempData["Sucesso"] = true;
            }

            PopularMotivos(viewModel);

            return View(viewModel);
        }

        [HttpGet]
        public ActionResult Atualizar(int? id)
        {
            if (id == null)
                return RedirectToAction(nameof(Index));

            var solicitacaoMotivo = _motivosRepositorio.ObterSolicitacaoMotivoPorId(id.Value);

            if (solicitacaoMotivo == null)
                RegistroNaoEncontrado();

            var viewModel = new SolicitacaoComercialMotivosViewModel
            {
                Id = solicitacaoMotivo.Id,
                Descricao = solicitacaoMotivo.Descricao,
                CancelamentoNF = solicitacaoMotivo.CancelamentoNF,
                Desconto = solicitacaoMotivo.Desconto,
                ProrrogacaoBoleto = solicitacaoMotivo.ProrrogacaoBoleto,
                Restituicao = solicitacaoMotivo.Restituicao,
                Outros = solicitacaoMotivo.Outros,
                Status = solicitacaoMotivo.Status
            };

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Atualizar([Bind(Include = "Id, SolicitacaoId, Descricao, Status, CancelamentoNF, Desconto, Restituicao, ProrrogacaoBoleto, Outros")] SolicitacaoComercialMotivosViewModel viewModel, int? id)
        {
            if (id == null)
                return RedirectToAction(nameof(Index));

            var solicitacaoMotivo = _motivosRepositorio.ObterSolicitacaoMotivoPorId(id.Value);

            if (solicitacaoMotivo == null)
                RegistroNaoEncontrado();

            solicitacaoMotivo.Alterar(new SolicitacaoComercialMotivo(
                viewModel.Descricao,
                viewModel.CancelamentoNF,
                viewModel.Desconto,
                viewModel.Restituicao,
                viewModel.ProrrogacaoBoleto,
                viewModel.Outros,
                viewModel.Status));

            if (Validar(solicitacaoMotivo))
            {
                _motivosRepositorio.AtualizarMotivo(solicitacaoMotivo);
                TempData["Sucesso"] = true;
            }

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Excluir(int id)
        {
            try
            {
                var solicitacaoMotivo = _motivosRepositorio.ObterSolicitacaoMotivoPorId(id);

                if (solicitacaoMotivo == null)
                    RegistroNaoEncontrado();

                _motivosRepositorio.ExcluirMotivo(solicitacaoMotivo.Id);
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }
    }
}