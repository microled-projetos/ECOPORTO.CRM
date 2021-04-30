using System.Linq;
using System.Web.Mvc;
using Ecoporto.CRM.Business.Extensions;

namespace Ecoporto.CRM.Site.Helpers
{
    public static class Validation
    {
        public static MvcHtmlString ValidarFormulario(this HtmlHelper helper, ModelStateDictionary modelState, bool update)
        {
            var erros = modelState.Values
                .SelectMany(x => x.Errors)
                .ToList();

            var sucesso = helper.ViewContext.TempData["Sucesso"]?
                .ToString()
                .ToBoolean();

            var acao = update ? "atualizado" : "cadastrado";

            var msgSucesso = $@"<div 
                                    class='alert alert-success' 
                                    role='alert'>Registro <strong>{ acao }</strong> com sucesso!
                                    <button type='button' class='close' data-dismiss='alert' aria-label='Close'><span aria-hidden='true'>&times;</span></button>
                                </div>";

            var itens = string.Empty;

            foreach (var erro in erros)
            {
                itens += $"<li>{ erro.ErrorMessage }</li>";
            }

            var msgErro = $@"<div class='alert alert-danger' role='alert'>
                                <ul>
                                    {itens}
                                </ul>
                             </div> ";

            var devolve = string.Empty;

            if (erros.Count() == 0 && sucesso.GetValueOrDefault())
                return MvcHtmlString.Create(msgSucesso);

            if (erros.ToArray().Length > 0 && !sucesso.GetValueOrDefault())
                return MvcHtmlString.Create(msgErro);

            return MvcHtmlString.Create(string.Empty);

        }
    }
}