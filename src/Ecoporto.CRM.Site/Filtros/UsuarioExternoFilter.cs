using Ecoporto.CRM.Business.Interfaces.Repositorios;
using Ecoporto.CRM.Infra.Repositorios;
using Ecoporto.CRM.Site.Extensions;
using System.Web;
using System.Web.Mvc;

namespace Ecoporto.CRM.Site.Filtros
{
    public class UsuarioExternoFilter : ActionFilterAttribute, IActionFilter
    {
        private readonly IUsuarioRepositorio _usuarioRepositorio = new UsuarioRepositorio();
      
        void IActionFilter.OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (HttpContext.Current.User.IsInRole("UsuarioExterno"))
            {
                var usuario = _usuarioRepositorio.ObterUsuarioPorId(HttpContext.Current.User.ObterId());

                if (usuario != null)
                {
                    filterContext.Controller.ViewBag.UsuarioExternoId = usuario.Id;
                }
            }            
        }

        void IActionFilter.OnActionExecuted(ActionExecutedContext filterContext)
        {
            
        }        
    }
}