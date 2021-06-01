using Ecoporto.CRM.Business.Enums;
using System;
using System.Web.Mvc;

namespace Ecoporto.CRM.Site.Helpers
{
    public static class FichaFaturamento
    {
        public static MvcHtmlString LinkEnviarFichaParaAprovacao(this HtmlHelper helper, string id, StatusOportunidade statusOportunidade, StatusFichaFaturamento statusFichaFaturamento)
        {
            string btn = string.Empty;
            string disabled = string.Empty;            

            if ((!Enum.IsDefined(typeof(StatusOportunidade), statusOportunidade) || statusOportunidade == StatusOportunidade.ENVIADO_PARA_APROVACAO))
            {
                disabled = "disabled";
            }

            if (statusFichaFaturamento != StatusFichaFaturamento.REJEITADO && statusFichaFaturamento != StatusFichaFaturamento.EM_ANDAMENTO)
            {
                disabled = "disabled";
            }            

            return MvcHtmlString.Create($"<td class=\"campo-acao\"><a href=\"#\" id=\"btn-enviar-ficha-{id}\" onclick=\"enviarFichaFaturamentoParaAprovacao({id})\" class=\"btn btn-warning btn-sm btn-acao {disabled}\"><i class=\"fa fa-check\"></i>&nbsp;Enviar para Aprovação</a></td>");
        }
    }
}