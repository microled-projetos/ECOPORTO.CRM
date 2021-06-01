using System.Web.Mvc;

namespace Ecoporto.CRM.Site.Helpers
{
    public static class Workflow
    {       
        public static MvcHtmlString StatusWorkflow(this HtmlHelper helper, int status)
        {
            string span = string.Empty;

            switch (status)
            {
                case 1:
                    span = "<span class=\"btn btn-warning btn-sm btn-acao\"><i class=\"fa fa-clock\"></i>&nbsp;Pendente</span>";
                    break;
                case 2:
                    span = "<span class=\"btn btn-success btn-sm btn-acao\"><i class=\"fa fa-check\"></i>&nbsp;Aprovado</span>";
                    break;
                case 3:
                    span = "<span class=\"btn btn-danger btn-sm btn-acao\"><i class=\"fa fa-trash\"></i>&nbsp;Rejeitado</span>";
                    break;
                default:
                    break;
            }

            return MvcHtmlString.Create(span);           
        }

        public static MvcHtmlString DescricaoStatusWorkflow(this HtmlHelper helper, int status)
        {
            string texto = string.Empty;

            switch (status)
            {
                case 1:
                    texto = "Pendente";
                    break;
                case 2:
                    texto = "Aprovado";
                    break;
                case 3:
                    texto = "Rejeitado";
                    break;
                default:
                    break;
            }

            return MvcHtmlString.Create(texto);
        }
    }
}