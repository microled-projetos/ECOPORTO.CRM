using Ecoporto.CRM.Business.Enums;
using System;
using System.Web.Mvc;

namespace Ecoporto.CRM.Site.Helpers
{
    public static class PremioParceria
    {
        public static MvcHtmlString LinkEnviarPremioParaAprovacao(this HtmlHelper helper, int id, StatusOportunidade statusOportunidade, StatusPremioParceria statusPremioParceria)
        {
            string btn = string.Empty;
            string disabled = string.Empty;

            if (statusOportunidade != StatusOportunidade.ATIVA && statusOportunidade != StatusOportunidade.VENCIDO && statusOportunidade != StatusOportunidade.CANCELADA && statusOportunidade != StatusOportunidade.REVISADA)
            {
                disabled = "disabled";
            }

            if (statusPremioParceria == StatusPremioParceria.EM_APROVACAO || statusPremioParceria == StatusPremioParceria.CADASTRADO || statusPremioParceria == StatusPremioParceria.REVISADO)
            {
                disabled = "disabled";
            }
            
            return MvcHtmlString.Create($"<td class=\"campo-acao\"><a href=\"#\" id=\"btn-enviar-premio-{id}\" onclick=\"enviarPremioParceriaParaAprovacao({id})\" class=\"btn btn-warning btn-sm btn-acao {disabled}\"><i class=\"fa fa-check\"></i>&nbsp;Enviar para Aprovação</a></td>");
        }
    }
}