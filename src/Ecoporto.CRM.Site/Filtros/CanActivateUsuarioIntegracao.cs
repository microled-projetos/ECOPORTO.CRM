using Ecoporto.CRM.Business.Enums;
using Ecoporto.CRM.Business.Extensions;
using Ecoporto.CRM.Business.Interfaces.Repositorios;
using Ecoporto.CRM.Infra.Repositorios;
using Ecoporto.CRM.Site.Extensions;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ecoporto.CRM.Site.Filtros
{
    public class CanActivateUsuarioIntegracao : ActionFilterAttribute
    {
        private readonly IOportunidadeRepositorio _oportunidadeRepositorio = new OportunidadeRepositorio();
        private readonly IModeloRepositorio _modeloRepositorio = new ModeloRepositorio();
        private readonly IUsuarioRepositorio _usuarioRepositorio = new UsuarioRepositorio();

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

                    oportunidadeId = referer[referer.Length - 1].ToInt();
                }
            }

            var oportunidadeBusca = _oportunidadeRepositorio
                .ObterOportunidadePorId(oportunidadeId);

            if (oportunidadeBusca == null)
            {
                throw new Exception($"Oportunidade ({oportunidadeId}) não encontrada");
            }

            var usuarioIntegracao = _usuarioRepositorio
                .ObterUsuariosIntegracao()
                .Where(c => c.UsuarioId == HttpContext.Current.User.ObterId()).FirstOrDefault();

            if (usuarioIntegracao == null)
            {
                context.Controller.ViewBag.HabilitaBotaoIntegracaoProposta = false;
            }
            else
            {
                var modeloBusca = _modeloRepositorio.ObterModeloPorId(oportunidadeBusca.OportunidadeProposta.ModeloId);

                if (modeloBusca == null)
                {
                    context.Controller.ViewBag.HabilitaBotaoIntegracaoProposta = false;
                }
                else
                {
                    var permiteIntegracao = modeloBusca.IntegraChronos;

                    var usuarioHabilitadoIntegracaoProducao = usuarioIntegracao.AcessoProducao;

                    var oportunidadeEnviadaAprovacao = oportunidadeBusca.StatusOportunidade == StatusOportunidade.ENVIADO_PARA_APROVACAO;

                    if (HttpContext.Current.User.IsInRole("BaseDesenvolvimento"))
                    {
                        context.Controller.ViewBag.HabilitaBotaoIntegracaoProposta = permiteIntegracao && oportunidadeEnviadaAprovacao;
                    }
                    else
                    {
                        context.Controller.ViewBag.HabilitaBotaoIntegracaoProposta = permiteIntegracao && oportunidadeEnviadaAprovacao && usuarioHabilitadoIntegracaoProducao;
                    }
                }                
            }
        }
    }
}