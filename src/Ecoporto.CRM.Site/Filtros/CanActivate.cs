using System.Web.Mvc;

namespace Ecoporto.CRM.Site.Filtros
{
    public class CanActivate : ActionFilterAttribute
    {
        public string Roles { get; set; }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.HttpContext.User.IsInRole("Administrador") && !context.HttpContext.User.IsInRole(Roles))
            {
                context.Result = new RedirectResult("~/Home/AcessoNegado");
            }
        }
    }
}