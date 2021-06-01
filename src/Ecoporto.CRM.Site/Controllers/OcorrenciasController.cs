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
    public class OcorrenciasController : BaseController
    {
        private readonly IOcorrenciasRepositorio _ocorrenciasRepositorio;        

        public OcorrenciasController(
            IOcorrenciasRepositorio ocorrenciasRepositorio,            
            ILogger logger) : base(logger)
        {
            _ocorrenciasRepositorio = ocorrenciasRepositorio;            
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        public void PopularOcorrencias(SolicitacaoComercialOcorrenciasViewModel solicitacaoComercialOcorrenciasViewModel)
        {
            var ocorrencias = _ocorrenciasRepositorio.ObterSolicitacoesOcorrencia();

            solicitacaoComercialOcorrenciasViewModel.Ocorrencias = ocorrencias.ToList();
        }

        [HttpGet]
        public ActionResult Cadastrar()
        {
            var viewModel = new SolicitacaoComercialOcorrenciasViewModel();

            PopularOcorrencias(viewModel);

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Cadastrar([Bind(Include = "SolicitacaoId, Descricao, CancelamentoNF, Desconto, Restituicao, ProrrogacaoBoleto, Outros, Status")] SolicitacaoComercialOcorrenciasViewModel viewModel)
        {
            var solicitacaoOcorrencia = new SolicitacaoComercialOcorrencia(
                viewModel.Descricao,
                viewModel.CancelamentoNF,
                viewModel.Desconto,
                viewModel.Restituicao,
                viewModel.ProrrogacaoBoleto,
                viewModel.Outros,
                viewModel.Status);

            if (Validar(solicitacaoOcorrencia))
            {
                _ocorrenciasRepositorio.CadastrarOcorrencia(solicitacaoOcorrencia);
                TempData["Sucesso"] = true;
            }

            PopularOcorrencias(viewModel);

            return View(viewModel);
        }

        [HttpGet]
        public ActionResult Atualizar(int? id)
        {
            if (id == null)
                return RedirectToAction(nameof(Index));

            var solicitacaoOcorrencia = _ocorrenciasRepositorio.ObterSolicitacaoOcorrenciaPorId(id.Value);

            if (solicitacaoOcorrencia == null)
                RegistroNaoEncontrado();

            var viewModel = new SolicitacaoComercialOcorrenciasViewModel
            {
                Id = solicitacaoOcorrencia.Id,
                Descricao = solicitacaoOcorrencia.Descricao,
                CancelamentoNF = solicitacaoOcorrencia.CancelamentoNF,
                Desconto = solicitacaoOcorrencia.Desconto,
                ProrrogacaoBoleto = solicitacaoOcorrencia.ProrrogacaoBoleto,
                Restituicao = solicitacaoOcorrencia.Restituicao,
                Outros = solicitacaoOcorrencia.Outros,
                Status = solicitacaoOcorrencia.Status
            };

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Atualizar([Bind(Include = "Id, SolicitacaoId, Descricao, CancelamentoNF, Desconto, Restituicao, ProrrogacaoBoleto, Outros, Status")] SolicitacaoComercialOcorrenciasViewModel viewModel, int? id)
        {
            if (id == null)
                return RedirectToAction(nameof(Index));

            var solicitacaoOcorrencia = _ocorrenciasRepositorio.ObterSolicitacaoOcorrenciaPorId(id.Value);

            if (solicitacaoOcorrencia == null)
                RegistroNaoEncontrado();

            solicitacaoOcorrencia.Alterar(
                new SolicitacaoComercialOcorrencia(
                    viewModel.Descricao,
                    viewModel.CancelamentoNF,
                    viewModel.Desconto,
                    viewModel.Restituicao,
                    viewModel.ProrrogacaoBoleto,
                    viewModel.Outros,
                    viewModel.Status));

            if (Validar(solicitacaoOcorrencia))
            {
                _ocorrenciasRepositorio.AtualizarOcorrencia(solicitacaoOcorrencia);
                TempData["Sucesso"] = true;
            }

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Excluir(int id)
        {
            try
            {
                var solicitacaoOcorrencia = _ocorrenciasRepositorio.ObterSolicitacaoOcorrenciaPorId(id);

                if (solicitacaoOcorrencia == null)
                    RegistroNaoEncontrado();

                _ocorrenciasRepositorio.ExcluirOcorrencia(solicitacaoOcorrencia.Id);
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }
    }
}