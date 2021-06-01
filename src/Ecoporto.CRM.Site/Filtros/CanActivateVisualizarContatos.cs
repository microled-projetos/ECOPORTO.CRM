using Ecoporto.CRM.Business.Extensions;
using Ecoporto.CRM.Business.Interfaces.Repositorios;
using Ecoporto.CRM.Infra.Repositorios;
using System.Web;
using System.Web.Mvc;

namespace Ecoporto.CRM.Site.Filtros
{
    public class CanActivateVisualizarContatos : ActionFilterAttribute
    {
        private readonly IEquipeContaRepositorio _equipeContaRepositorio = new EquipeContaRepositorio();
        private readonly IContaRepositorio _contaRepositorio = new ContaRepositorio();

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

            if (HttpContext.Current.Request.QueryString["conta"] == null)
            {
                context.Result = new RedirectResult("~/Home/Indisponivel");
                return;
            }

            var contaId = HttpContext.Current.Request.QueryString["conta"].ToString().ToInt();

            var contaBusca = _contaRepositorio
                .ObterContaPorId(contaId);

            if (contaBusca == null)
            {
                context.Result = new RedirectResult("~/Home/Indisponivel");
                return;
            }

            var login = HttpContext.Current.User.Identity.Name;

            var permissoesPorVendedor = _equipeContaRepositorio
                .ObterPermissoesOportunidadePorVendedor(contaBusca.VendedorId, login);

            // Usuário esta vinculado a equipe do vendedor da oportunidade?

            if (permissoesPorVendedor != null)
            {
                context.Controller.ViewBag.ContatoSomenteLeitura = permissoesPorVendedor.AcessoConta == 0;
            }
            else
            {
                // Usuário esta vinculado a equipe da conta relacionada à oportunidade?

                var permissoesPorConta = _equipeContaRepositorio
                    .ObterPermissoesContaPorConta(contaBusca.Id, login);

                if (permissoesPorConta != null)
                {
                    context.Controller.ViewBag.ContatoSomenteLeitura = permissoesPorConta.AcessoConta == 0;
                }
                else
                {
                    context.Controller.ViewBag.ContatoSomenteLeitura = true;
                }
            }
        }
    }
}