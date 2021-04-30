using Ecoporto.CRM.Business.Extensions;
using Ecoporto.CRM.Business.Interfaces.Repositorios;
using Ecoporto.CRM.Infra.Repositorios;
using Ecoporto.CRM.Site.Extensions;
using System.Web;
using System.Web.Mvc;

namespace Ecoporto.CRM.Site.Filtros
{
    public class CanActivateOportunidadeUsuarioExterno : ActionFilterAttribute
    {
        private readonly IOportunidadeRepositorio oportunidadeRepositorio = new OportunidadeRepositorio();
        private readonly IUsuarioRepositorio _usuarioRepositorio = new UsuarioRepositorio();

        public string Roles { get; set; }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            context.Controller.ViewBag.SomenteLeitura = true;
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.HttpContext.User.IsInRole("UsuarioExterno"))
            {
                if (context.RouteData != null && context.RouteData.Values["id"] == null)
                    return;

                var oportunidadeId = context.RouteData.Values["id"].ToString().ToInt();

                var oportunidade = oportunidadeRepositorio.ObterOportunidadePorId(oportunidadeId);

                if (oportunidade != null)
                {
                    if (!_usuarioRepositorio.ExisteVinculoConta(oportunidade.ContaId, HttpContext.Current.User.ObterId()))
                    {
                        context.Result = new RedirectResult("~/Home/AcessoNegado");
                    }
                }
            }
        }
    }
}