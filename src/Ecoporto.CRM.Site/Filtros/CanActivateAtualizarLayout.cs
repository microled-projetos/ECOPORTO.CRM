using Ecoporto.CRM.Business.Enums;
using Ecoporto.CRM.Business.Extensions;
using Ecoporto.CRM.Infra.Repositorios;
using System.Web.Mvc;

namespace Ecoporto.CRM.Site.Filtros
{
    public class CanActivateAtualizarLayout : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.RouteData == null || context.RouteData.Values["id"] == null)
                return;

            if (context.HttpContext.Request.QueryString["oportunidadeId"] == null)
                return;

            var layoutId = context.RouteData.Values["id"].ToString().ToInt();

            var _layoutRepositorio = new LayoutRepositorio(true);

            var layoutBusca = _layoutRepositorio.ObterLayoutPorId(layoutId);

            if (layoutBusca != null)
            {
                if (layoutBusca.TipoRegistro == TipoRegistro.CONDICAO_INICIAL)
                {
                    if (!(context.HttpContext.User.IsInRole("Administrador") || context.HttpContext.User.IsInRole("OportunidadesProposta:PropostaCondicoesIniciais")))
                    {
                        context.Result = new RedirectResult("~/Home/AcessoNegado");
                    }
                }

                if (layoutBusca.TipoRegistro == TipoRegistro.CONDICAO_GERAL)
                {
                    if (!(context.HttpContext.User.IsInRole("Administrador") || context.HttpContext.User.IsInRole("OportunidadesProposta:PropostaCondicoesGerais")))
                    {
                        context.Result = new RedirectResult("~/Home/AcessoNegado");
                    }
                }

                if (layoutBusca.TipoRegistro != TipoRegistro.CONDICAO_INICIAL && layoutBusca.TipoRegistro != TipoRegistro.CONDICAO_GERAL)
                {
                    if (!(context.HttpContext.User.IsInRole("Administrador") || context.HttpContext.User.IsInRole("Layouts:Atualizar")))
                    {
                        context.Result = new RedirectResult("~/Home/AcessoNegado");
                    }
                }
            }
        }
    }
}