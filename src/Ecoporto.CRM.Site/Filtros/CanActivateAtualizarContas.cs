using Ecoporto.CRM.Business.Extensions;
using Ecoporto.CRM.Business.Interfaces.Repositorios;
using Ecoporto.CRM.Infra.Repositorios;
using System.Web;
using System.Web.Mvc;

namespace Ecoporto.CRM.Site.Filtros
{
    public class CanActivateAtualizarContas : ActionFilterAttribute
    {
        private readonly IEquipeContaRepositorio _equipeContaRepositorio = new EquipeContaRepositorio();

        public string Roles { get; set; }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            context.Controller.ViewBag.SomenteLeitura = true;           
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.HttpContext.User.IsInRole("Administrador"))
                return;

            if (context.HttpContext.User.IsInRole("UsuarioExterno"))
                return;

            if (context.RouteData != null && context.RouteData.Values["id"] == null)
                return;

            var contaId = context.RouteData.Values["id"].ToString().ToInt();

            var login = HttpContext.Current.User.Identity.Name;

            var permissoesPorVendedor = _equipeContaRepositorio
                .ObterPermissoesContaPorVendedor(contaId, login);

            var permissoesPorConta = _equipeContaRepositorio
                .ObterPermissoesContaPorConta(contaId, login);

            if (permissoesPorVendedor == null)
            {
                if (permissoesPorConta == null)
                {
                    if (context.HttpContext.User.IsInRole(Roles))
                    {
                        context.Controller.ViewBag.ContaSomenteLeitura = null;
                        context.Controller.ViewBag.OportunidadeSomenteLeitura = null;
                    }
                    else
                    {
                        context.Result = new RedirectResult("~/Home/AcessoNegado");
                    }                    
                }
                else
                {
                    context.Controller.ViewBag.ContaSomenteLeitura = permissoesPorConta.AcessoConta == 0;
                    context.Controller.ViewBag.OportunidadeSomenteLeitura = permissoesPorConta.AcessoOportunidade == 0;
                }
            }
            else
            {
                context.Controller.ViewBag.ContaSomenteLeitura = permissoesPorVendedor.AcessoConta == 0;
                context.Controller.ViewBag.OportunidadeSomenteLeitura = permissoesPorVendedor.AcessoOportunidade == 0;
            }       
        }
    }
}