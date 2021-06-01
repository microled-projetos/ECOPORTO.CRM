using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Ecoporto.CRM.Site.Helpers
{
    public static class Controls
    {
        public static MvcHtmlString PrivilegedEditorFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, string cssClass, int? maxlength = 524288, bool? readOnly = null)
        {
            var attributos = ObterAtributosTela(html, expression);

            object htmlAttributes = new { @class = cssClass, maxlength };

            if (readOnly != null && readOnly == true)
            {
                htmlAttributes = new
                {
                    @class = cssClass,
                    @readonly = "readonly",
                    maxlength
                };

                return html.EditorFor(expression, new { htmlAttributes });
            }
            else
            {
                if (SemPermissao($"{attributos.Item1}:{attributos.Item2}") && readOnly == null)
                {
                    htmlAttributes = new
                    {
                        @class = cssClass,
                        @readonly = "readonly",
                        maxlength
                    };

                    return html.EditorFor(expression, new { htmlAttributes });
                }
            }

            return html.EditorFor(expression, new { htmlAttributes });
        }

        public static MvcHtmlString PrivilegedTextAreaFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, string cssClass, int qtdrows, bool? readOnly = null)
        {
            var attributos = ObterAtributosTela(html, expression);

            object htmlAttributes = new { @class = cssClass, @rows = qtdrows };

            if (readOnly != null && readOnly == true)
            {
                htmlAttributes = new
                {
                    @class = cssClass,
                    @readonly = "readonly",
                    @rows = qtdrows
                };

                return html.TextAreaFor(expression, htmlAttributes);
            }
            else
            {
                if (SemPermissao($"{attributos.Item1}:{attributos.Item2}") && readOnly == null)
                {
                    htmlAttributes = new
                    {
                        @class = cssClass,
                        @readonly = "readonly",
                        @rows = qtdrows
                    };

                    return html.TextAreaFor(expression, htmlAttributes);
                }
            }

            return html.TextAreaFor(expression, htmlAttributes);
        }

        public static MvcHtmlString PrivilegedDropDownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, IEnumerable<SelectListItem> selectList, string cssClass, string optionLabel = "", bool? readOnly = null)
        {
            var attributos = ObterAtributosTela(html, expression);

            object htmlAttributes = new { @class = cssClass };

            MvcHtmlString ddlf = null;

            if (readOnly != null && readOnly == true)
            {
                htmlAttributes = new
                {
                    @class = cssClass + " select-readonly"
                };

                ddlf = html.DropDownListFor(expression, selectList, optionLabel, htmlAttributes);

                return MvcHtmlString.Create(ddlf.ToString());
            }
            else
            {
                if (SemPermissao($"{attributos.Item1}:{attributos.Item2}") && readOnly == null)
                {
                    htmlAttributes = new
                    {
                        @class = cssClass + " select-readonly"
                    };

                    ddlf = html.DropDownListFor(expression, selectList, optionLabel, htmlAttributes);

                    return MvcHtmlString.Create(ddlf.ToString());
                }
            }

            ddlf = html.DropDownListFor(expression, selectList, optionLabel, htmlAttributes);

            return MvcHtmlString.Create(ddlf.ToString());
        }

        public static MvcHtmlString PrivilegedEnumDropDownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, string cssClass, bool? readOnly = null)
        {
            var attributos = ObterAtributosTela(html, expression);

            object htmlAttributes = new { @class = cssClass };

            MvcHtmlString edplf = null;

            if (readOnly != null && readOnly == true)
            {
                htmlAttributes = new
                {
                    @class = cssClass + " select-readonly"
                };

                edplf = html.EnumDropDownListFor(expression, htmlAttributes);

                return MvcHtmlString.Create(edplf.ToString());

            }
            else
            {
                if (SemPermissao($"{attributos.Item1}:{attributos.Item2}") && readOnly == null)
                {
                    htmlAttributes = new
                    {
                        @class = cssClass + " select-readonly"
                    };

                    edplf = html.EnumDropDownListFor(expression, htmlAttributes);

                    return MvcHtmlString.Create(edplf.ToString());
                }
            }

            edplf = html.EnumDropDownListFor(expression, htmlAttributes);

            return MvcHtmlString.Create(edplf.ToString());
        }

        public static MvcHtmlString PrivilegedCheckBoxFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, string cssClass, bool? readOnly = null, bool? check = false)
        {
            var attributos = ObterAtributosTela(html, expression);

            object htmlAttributes = new { @class = cssClass };

            object hidden = null;

            MvcHtmlString cbf = null;

            Expression<Func<TModel, bool>> expressao;

            if ((readOnly != null && readOnly == true) || (SemPermissao($"{attributos.Item1}:{attributos.Item2}") && readOnly == null))
            {
                htmlAttributes = new
                {
                    @class = "checkbox-readonly"
                };

                if (check.HasValue)
                {
                    if (check.Value)
                    {
                        htmlAttributes = new
                        {
                            @class = "checkbox-readonly",
                            @checked = "checked"
                        };
                    }
                }

                hidden = html.HiddenFor(expression);

                expressao = Expression.Lambda<Func<TModel, bool>>(expression.Body, expression.Parameters);

                cbf = html.CheckBoxFor(expressao, htmlAttributes);

                return MvcHtmlString.Create(string.Concat(cbf.ToString(), hidden?.ToString()));
            }

            if (check.HasValue)
            {
                if (check.Value)
                {
                    htmlAttributes = new
                    {
                        @class = cssClass,
                        @checked = "checked"
                    };
                }
            }

            expressao = Expression.Lambda<Func<TModel, bool>>(expression.Body, expression.Parameters);

            cbf = html.CheckBoxFor(expressao, htmlAttributes);

            return MvcHtmlString.Create(string.Concat(cbf.ToString(), hidden?.ToString()));
        }

        public static Tuple<string, string> ObterAtributosTela<TModel, TValue>(HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
        {
            var tela = html.ViewContext.RouteData.GetRequiredString("controller");
            var controller = ((MemberExpression)expression.Body).Member.Name;

            return new Tuple<string, string>(tela, controller);
        }

        public static bool SemPermissao(string role)
        {
            if (role.Contains("Oportunidades"))
            {
                if (HttpContext.Current.User.IsInRole("Administrador"))
                    return false;

                var campo = role.Replace("Oportunidades:", string.Empty);

                var roles = (!HttpContext.Current.User.IsInRole("OportunidadesIniciais:" + campo) && !HttpContext.Current.User.IsInRole("OportunidadesProposta:" + campo) && !HttpContext.Current.User.IsInRole("OportunidadesFichas:" + campo) && !HttpContext.Current.User.IsInRole("OportunidadesAnexos:" + campo) && !HttpContext.Current.User.IsInRole("OportunidadesPremios:" + campo) && !HttpContext.Current.User.IsInRole("OportunidadesAdendos:" + campo) && !HttpContext.Current.User.IsInRole("OportunidadesSimulador:" + campo));

                return roles;
            }

            if (role.Contains("Solicitacoes"))
            {
                if (HttpContext.Current.User.IsInRole("Administrador"))
                    return false;

                var campo = role.Replace("Solicitacoes:", string.Empty);

                var roles = (!HttpContext.Current.User.IsInRole("Solicitacoes:" + campo)
                 && !HttpContext.Current.User.IsInRole("SolicitacoesCancelamento:" + campo)
                 && !HttpContext.Current.User.IsInRole("SolicitacoesDesconto:" + campo)
                 && !HttpContext.Current.User.IsInRole("SolicitacoesProrrogacao:" + campo)
                 && !HttpContext.Current.User.IsInRole("SolicitacoesRestituicao:" + campo)
                 && !HttpContext.Current.User.IsInRole("SolicitacoesOutros:" + campo)
                 && !HttpContext.Current.User.IsInRole($"OportunidadesIniciais:{campo}_Full")
                 && !HttpContext.Current.User.IsInRole($"OportunidadesProposta:{campo}_Full")
                 && !HttpContext.Current.User.IsInRole($"OportunidadesFichas:{campo}_Full")
                 && !HttpContext.Current.User.IsInRole($"OportunidadesAnexos:{campo}_Full")
                 && !HttpContext.Current.User.IsInRole($"OportunidadesPremios:{campo}_Full")
                 && !HttpContext.Current.User.IsInRole($"OportunidadesAdendos:{campo}_Full")
                 && !HttpContext.Current.User.IsInRole($"Solicitacoes:{campo}_Full")
                 && !HttpContext.Current.User.IsInRole($"SolicitacoesCancelamento:{campo}_Full")
                 && !HttpContext.Current.User.IsInRole($"SolicitacoesDesconto:{campo}_Full")
                 && !HttpContext.Current.User.IsInRole($"SolicitacoesProrrogacao:{campo}_Full")
                 && !HttpContext.Current.User.IsInRole($"SolicitacoesRestituicao:{campo}_Full")
                 && !HttpContext.Current.User.IsInRole($"SolicitacoesOutros:{campo}_Full"));

                return roles;
            }

            return !HttpContext.Current.User.IsInRole("Administrador") && !HttpContext.Current.User.IsInRole(role);
        }

        public static bool AcessoTotal(string campo)
        {
            return
                   HttpContext.Current.User.IsInRole($"OportunidadesIniciais:{campo}_Full")
                || HttpContext.Current.User.IsInRole($"OportunidadesProposta:{campo}_Full")
                || HttpContext.Current.User.IsInRole($"OportunidadesFichas:{campo}_Full")
                || HttpContext.Current.User.IsInRole($"OportunidadesAnexos:{campo}_Full")
                || HttpContext.Current.User.IsInRole($"OportunidadesPremios:{campo}_Full")
                || HttpContext.Current.User.IsInRole($"OportunidadesAdendos:{campo}_Full")
                || HttpContext.Current.User.IsInRole($"Solicitacoes:{campo}_Full")
                || HttpContext.Current.User.IsInRole($"SolicitacoesCancelamento:{campo}_Full")
                || HttpContext.Current.User.IsInRole($"SolicitacoesDesconto:{campo}_Full")
                || HttpContext.Current.User.IsInRole($"SolicitacoesProrrogacao:{campo}_Full")
                || HttpContext.Current.User.IsInRole($"SolicitacoesRestituicao:{campo}_Full")
                || HttpContext.Current.User.IsInRole($"SolicitacoesOutros:{campo}_Full");
        }

        public static MvcHtmlString FullControlEditorFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, string cssClass, int? maxlength = 524288)
        {
            var attributos = ObterAtributosTela(html, expression);

            object htmlAttributes = new { @class = cssClass, maxlength };

            if (AcessoTotal(attributos.Item2))
            {
                htmlAttributes = new
                {
                    @class = cssClass,
                    maxlength
                };
            }
            else
            {
                htmlAttributes = new
                {
                    @class = cssClass,
                    @readonly = "readonly",
                    maxlength
                };
            }

            return html.EditorFor(expression, new { htmlAttributes });
        }

        public static MvcHtmlString FullControlEditorFileFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, string cssClass, int? maxlength = 524288)
        {
            var attributos = ObterAtributosTela(html, expression);

            object htmlAttributes = new { @class = cssClass, maxlength };

            if (AcessoTotal(attributos.Item2))
            {
                htmlAttributes = new
                {
                    @class = cssClass,
                    maxlength,
                    type = "file"
                };
            }
            else
            {
                htmlAttributes = new
                {
                    @class = cssClass,
                    @readonly = "readonly",
                    type = "file",
                    maxlength
                };
            }

            return html.EditorFor(expression, new { htmlAttributes });
        }

        public static MvcHtmlString FullControlDropDownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, IEnumerable<SelectListItem> selectList, string cssClass, string optionLabel = "")
        {
            var attributos = ObterAtributosTela(html, expression);

            object htmlAttributes = new { @class = cssClass };

            MvcHtmlString ddlf = null;

            if (!AcessoTotal(attributos.Item2))
            {
                htmlAttributes = new
                {
                    @class = cssClass + " select-readonly"
                };
            }

            ddlf = html.DropDownListFor(expression, selectList, optionLabel, htmlAttributes);

            return MvcHtmlString.Create(ddlf.ToString());
        }

        public static MvcHtmlString FullControlEnumDropDownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, string cssClass)
        {
            var attributos = ObterAtributosTela(html, expression);

            object htmlAttributes = new { @class = cssClass };

            MvcHtmlString edplf = null;

            if (!AcessoTotal(attributos.Item2))
            {
                htmlAttributes = new
                {
                    @class = cssClass + " select-readonly"
                };
            }

            edplf = html.EnumDropDownListFor(expression, htmlAttributes);

            return MvcHtmlString.Create(edplf.ToString());
        }

        public static MvcHtmlString FullControlRadioButtonFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, bool value)
        {
            var attributos = ObterAtributosTela(html, expression);

            object hidden = null;

            object htmlAttributes = new { };

            Expression<Func<TModel, bool>> expressao;

            MvcHtmlString cbf = null;

            if (!AcessoTotal(attributos.Item2))
            {
                htmlAttributes = new
                {
                    @disabled = "disabled"
                };

                hidden = html.HiddenFor(expression);

                expressao = Expression.Lambda<Func<TModel, bool>>(expression.Body, expression.Parameters);

                cbf = html.RadioButtonFor(expressao, value, htmlAttributes);

                return MvcHtmlString.Create(string.Concat(cbf.ToString(), hidden?.ToString()));
            }

            expressao = Expression.Lambda<Func<TModel, bool>>(expression.Body, expression.Parameters);

            cbf = html.RadioButtonFor(expressao, value, htmlAttributes);

            return MvcHtmlString.Create(string.Concat(cbf.ToString(), hidden?.ToString()));
        }

        public static MvcHtmlString FullControlTextAreaFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, string cssClass, int qtdrows, bool? readOnly = null)
        {
            var attributos = ObterAtributosTela(html, expression);

            object htmlAttributes = new { @class = cssClass, @rows = qtdrows };

            if (!AcessoTotal(attributos.Item2))
            {
                htmlAttributes = new
                {
                    @class = cssClass,
                    @readonly = "readonly",
                    @rows = qtdrows
                };

                return html.TextAreaFor(expression, htmlAttributes);
            }

            return html.TextAreaFor(expression, htmlAttributes);
        }
    }
}