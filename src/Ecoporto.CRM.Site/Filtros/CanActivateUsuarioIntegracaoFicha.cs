using Ecoporto.CRM.Business.Enums;
using Ecoporto.CRM.Business.Extensions;
using Ecoporto.CRM.Business.Interfaces.Repositorios;
using Ecoporto.CRM.Infra.Repositorios;
using Ecoporto.CRM.Site.Extensions;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ecoporto.CRM.Site.Filtros
{
    public class CanActivateUsuarioIntegracaoFicha : ActionFilterAttribute
    {
        private readonly IOportunidadeRepositorio _oportunidadeRepositorio = new OportunidadeRepositorio();
        private readonly IModeloRepositorio _modeloRepositorio = new ModeloRepositorio();
        private readonly IUsuarioRepositorio _usuarioRepositorio = new UsuarioRepositorio();
        private readonly IParametrosRepositorio _parametrosRepositorio = new ParametrosRepositorio();

        public string Roles { get; set; }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var oportunidadeId = 0;

            if (context.RouteData != null && context.RouteData.Values["id"] != null)
            {
                oportunidadeId = context.RouteData.Values["id"].ToString().ToInt();
            }
            else
            {
                if (context.HttpContext.Request.Headers["Referer"] != null)
                {
                    var referer = context.HttpContext.Request.Headers["Referer"].Split('/');

                    oportunidadeId = referer[referer.Length-1].ToInt();
                }
            }
            
            var usuarioIntegracao = _usuarioRepositorio
                .ObterUsuariosIntegracao()
                .Where(c => c.UsuarioId == HttpContext.Current.User.ObterId()).FirstOrDefault();

            var parametros = _parametrosRepositorio.ObterParametros();

            if (usuarioIntegracao == null)
            {
                context.Controller.ViewBag.HabilitaBotaoIntegracaoFicha = false;
            }
            else
            {
                if (HttpContext.Current.User.IsInRole("BaseDesenvolvimento"))
                {
                    context.Controller.ViewBag.HabilitaBotaoIntegracaoFicha = parametros.IntegraChronos && usuarioIntegracao != null;
                }
                else
                {
                    if (usuarioIntegracao != null)
                    {
                        context.Controller.ViewBag.HabilitaBotaoIntegracaoFicha = parametros.IntegraChronos && usuarioIntegracao.AcessoProducao;
                    }
                }
            }            
        }
    }
}