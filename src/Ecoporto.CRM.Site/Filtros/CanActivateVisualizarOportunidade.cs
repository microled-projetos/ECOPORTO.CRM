using Ecoporto.CRM.Business.Extensions;
using Ecoporto.CRM.Business.Interfaces.Repositorios;
using Ecoporto.CRM.Infra.Repositorios;
using Ecoporto.CRM.Site.Extensions;
using System.Web;
using System.Web.Mvc;

namespace Ecoporto.CRM.Site.Filtros
{
    public class CanActivateVisualizarOportunidade : ActionFilterAttribute
    {
        private readonly IEquipeContaRepositorio _equipeContaRepositorio = new EquipeContaRepositorio();
        private readonly IContaRepositorio _contaRepositorio = new ContaRepositorio();
        private readonly IOportunidadeRepositorio _oportunidadeRepositorio = new OportunidadeRepositorio();
        private readonly IUsuarioRepositorio _usuarioRepositorio = new UsuarioRepositorio();

        public string Roles { get; set; }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            context.Controller.ViewBag.SomenteLeitura = true;
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.HttpContext.User.IsInRole("Administrador"))
                return;

            if (context.RouteData != null && context.RouteData.Values["id"] == null)
                return;

            var oportunidadeId = context.RouteData.Values["id"].ToString().ToInt();

            var oportunidadeBusca = _oportunidadeRepositorio
                .ObterOportunidadePorId(oportunidadeId);

            if (oportunidadeBusca == null)
            {
                context.Result = new RedirectResult("~/Home/Indisponivel");
                return;
            }

            var contaBusca = _contaRepositorio.ObterContaPorId(oportunidadeBusca.ContaId);

            if (contaBusca == null)
            {
                return;
            }

            var login = HttpContext.Current.User.Identity.Name;

            var vendedor = oportunidadeBusca.OportunidadeProposta.VendedorId == 0
                ? contaBusca.VendedorId
                : oportunidadeBusca.OportunidadeProposta.VendedorId;

            var permissoesPorVendedor = _equipeContaRepositorio
                .ObterPermissoesOportunidadePorVendedor(vendedor, login);

            var usuarioBusca = _usuarioRepositorio.ObterUsuarioPorId(HttpContext.Current.User.ObterId());

            // Usuário esta vinculado a equipe do vendedor da oportunidade?

            if (permissoesPorVendedor != null || usuarioBusca.Vendedor)
            {
                if (!usuarioBusca.Vendedor)
                    context.Controller.ViewBag.OportunidadeSomenteLeitura = permissoesPorVendedor.AcessoOportunidade == 0;
                else
                    context.Controller.ViewBag.OportunidadeSomenteLeitura = false;
            }
            else
            {
                // Usuário esta vinculado a equipe da conta relacionada à oportunidade?

                var permissoesPorConta = _equipeContaRepositorio
                    .ObterPermissoesContaPorConta(oportunidadeBusca.ContaId, login);

                if (permissoesPorConta != null)
                {
                    context.Controller.ViewBag.OportunidadeSomenteLeitura = permissoesPorConta.AcessoOportunidade == 0;
                }
                else
                {
                    // Usuário esta vinculado a equipe da oportunidade?

                    var permissoesPorOportunidade = _equipeContaRepositorio.ObterPermissoesPorOportunidade(oportunidadeBusca.Id, login);

                    if (permissoesPorOportunidade != null)
                    {
                        context.Controller.ViewBag.OportunidadeSomenteLeitura = permissoesPorOportunidade.AcessoOportunidade == 0;
                    }
                    else
                    {
                        if (contaBusca == null)
                            context.Controller.ViewBag.OportunidadeSomenteLeitura = null;
                        else
                            context.Controller.ViewBag.OportunidadeSomenteLeitura = true;
                    }
                }
            }
        }
    }
}