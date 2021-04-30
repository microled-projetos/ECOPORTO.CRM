using Ecoporto.CRM.Business.Interfaces.Repositorios;
using Ecoporto.CRM.Site.Extensions;
using System.Web.Mvc;

namespace Ecoporto.CRM.Site.Controllers
{
    public class WorkflowController : Controller
    {
        private readonly IWorkflowRepositorio _workflowRepositorio;
        private readonly IUsuarioRepositorio _usuarioRepositorio;

        public WorkflowController(IWorkflowRepositorio workflowRepositorio, IUsuarioRepositorio usuarioRepositorio)
        {
            _workflowRepositorio = workflowRepositorio;
            _usuarioRepositorio = usuarioRepositorio;
        }

        public ActionResult Index()
        {
            var usuarioBusca = _usuarioRepositorio.ObterUsuarioPorId(User.ObterId());

            if (usuarioBusca != null)
            {
                ViewBag.Id = _workflowRepositorio.AcessoSistemaWorkflow(usuarioBusca);

                return View();
            }

            return RedirectToAction("Index", "Home");
        }
    }
}