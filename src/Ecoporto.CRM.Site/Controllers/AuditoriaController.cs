using Ecoporto.CRM.Business.Extensions;
using Ecoporto.CRM.Business.Filtros;
using Ecoporto.CRM.Business.Interfaces.Repositorios;
using Ecoporto.CRM.Infra.Datatables;
using Ecoporto.CRM.Site.Models;
using System.Linq;
using System.Web.Mvc;

namespace Ecoporto.CRM.Site.Controllers
{
    [Authorize]
    public class AuditoriaController : Controller
    {
        private readonly IAuditoriaRepositorio _auditoriaRepositorio;

        public AuditoriaController(IAuditoriaRepositorio auditoriaRepositorio)
        {
            _auditoriaRepositorio = auditoriaRepositorio;
        }

        [HttpGet]
        public ActionResult Timeline(string objeto, int chave, int chavePai = 0)
        {
            var historico = _auditoriaRepositorio.ObterHistorico(objeto, chave);

            return View(new AuditoriaViewModel
            {
                Controller = objeto,
                Chave = chave,
                Historico = historico
            });
        }

        [HttpGet]
        public ActionResult ObterLogsFichasFaturamento(int oportunidadeId)
        {
            var historico = _auditoriaRepositorio.ObterLogsFichasFaturamento(oportunidadeId);

            return PartialView("_ConsultaLogsFichasFaturamento", new AuditoriaViewModel
            {
                Historico = historico
            });
        }

        [HttpGet]
        public PartialViewResult ObterLogsPremiosParceria(int oportunidadeId)
        {
            var historico = _auditoriaRepositorio.ObterLogsPremiosParceria(oportunidadeId);

            return PartialView("_ConsultaLogsPremiosParceria", new AuditoriaViewModel
            {
                Historico = historico
            });
        }

        [HttpGet]
        public PartialViewResult ObterLogsAdendos(int oportunidadeId)
        {
            var historico = _auditoriaRepositorio.ObterLogsAdendos(oportunidadeId);

            return PartialView("_ConsultaLogsAdendos", new AuditoriaViewModel
            {
                Historico = historico
            });
        }

        [HttpGet]
        public PartialViewResult ObterLogsProposta(int oportunidadeId)
        {
            var historico = _auditoriaRepositorio.ObterLogsProposta(oportunidadeId);

            return PartialView("_ConsultaLogsProposta", new AuditoriaViewModel
            {
                Historico = historico
            });
        }

        [HttpGet]
        public PartialViewResult ObterLogsAnexos(int oportunidadeId)
        {
            var historico = _auditoriaRepositorio.ObterLogsAnexos(oportunidadeId);

            return PartialView("_ConsultaLogsAnexos", new AuditoriaViewModel
            {
                Historico = historico
            });
        }

        [HttpGet]
        public ActionResult LogsAcesso()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ObterLogsAcesso(JQueryDataTablesParamViewModel Params, AuditoriaAcessoFiltro filtro)
        {
            var logs = _auditoriaRepositorio
                .ObterLogsAcesso(Params.Pagina, Params.iDisplayLength, filtro, Params.OrderBy, out int totalFiltro)
                .Select(c => new
                {
                    DataHora = c.DataHora.DataHoraFormatada(),
                    c.Login,
                    c.Externo,
                    c.IP,
                    c.Sucesso,
                    c.Mensagem
                });

            var totalRegistros = _auditoriaRepositorio.ObterTotalLogsAcessos();

            var resultado = new
            {
                Params.sEcho,
                iTotalRecords = totalRegistros,
                iTotalDisplayRecords = totalFiltro,
                aaData = logs
            };

            return Json(resultado, JsonRequestBehavior.AllowGet);
        }
    }
}