﻿using System.Web.Optimization;

namespace Ecoporto.CRM.Site
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery")
                .Include("~/Content/js/jquery-3.3.1.js",
                         "~/Content/js/jquery.unobtrusive-ajax.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery-url")
               .Include("~/Content/plugins/jquery-url/jquery-url.js"));

            bundles.Add(new ScriptBundle("~/bundles/site")
               .Include("~/Content/plugins/easyAutocomplete/jquery.easy-autocomplete.js", 
                        "~/Content/js/default.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery-validate")
                .Include("~/Content/plugins/jquery-validate/jquery.validate.min.js",
                         "~/Content/plugins/jquery-validate/language/messages_pt_BR.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery-mask")
                .Include("~/Content/plugins/jquery-mask/jquery.mask.js"));

            bundles.Add(new StyleBundle("~/bundles/css")
                .Include("~/Content/css/bootstrap.css",
                         "~/Content/css/estilos.css",
                         "~/Content/css/fontawesome-all.css",
                         "~/Content/plugins/toastr/toastr.css",
                         "~/Content/plugins/easyAutocomplete/easy-autocomplete.css"));

            bundles.Add(new StyleBundle("~/bundles/login")
               .Include("~/Content/css/bootstrap.css",
                        "~/Content/css/login.css"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap")
                .Include("~/Content/js/popper.js",
                         "~/Content/js/bootstrap.js"));

            bundles.Add(new StyleBundle("~/bundles/datatablesCSS")
                .Include("~/Content/plugins/datatables/css/dataTables.bootstrap4.min.css"));

            bundles.Add(new ScriptBundle("~/bundles/datatables")
                .Include("~/Content/plugins/datatables/js/jquery.dataTables.min.js",
                         "~/Content/plugins/datatables/js/dataTables.bootstrap4.min.js",
                         "~/Content/plugins/datatables/js/datetime-moment.js",
                         "~/Content/plugins/datatables/js/dataTables.checkboxes.min.js"));
            
            bundles.Add(new StyleBundle("~/bundles/summernoteCSS")
                .Include("~/Content/plugins/summernote/css/summernote-lite.css"));

            bundles.Add(new ScriptBundle("~/bundles/summernote")
                .Include("~/Content/plugins/summernote/js/summernote-lite.js",
                         "~/Content/plugins/summernote/js/lang/summernote-pt-BR.js",
                         "~/Content/plugins/summernote/js/summernote-cleaner.js"));

            bundles.Add(new ScriptBundle("~/bundles/ckeditor")
               .Include("~/Content/plugins/ckeditor/ckeditor.js",
                        "~/Content/plugins/ckeditor/translations/pt-br.js"));

            bundles.Add(new ScriptBundle("~/bundles/layouts")
                .Include("~/Content/js/layout/default.js"));

            bundles.Add(new ScriptBundle("~/bundles/contatos")
                .Include("~/Content/js/contatos/default.js"));
            
            bundles.Add(new ScriptBundle("~/bundles/auditoria")
                .Include("~/Content/plugins/jsonTree/jsonTree.js")
                .Include("~/Content/plugins/jsonToTable/json-to-table.js")
                .Include("~/Content/js/auditoria/default.js"));

            bundles.Add(new ScriptBundle("~/bundles/solicitacoes")
             .Include("~/Content/js/solicitacoes/default.js"));

            bundles.Add(new ScriptBundle("~/bundles/contas")
               .Include("~/Content/js/contas/contas.js"));

            bundles.Add(new ScriptBundle("~/bundles/toastr")
                .Include("~/Content/plugins/toastr/toastr.js"));

            bundles.Add(new ScriptBundle("~/bundles/oportunidades")
              .Include("~/Content/js/oportunidades/default.js"));

            bundles.Add(new ScriptBundle("~/bundles/oportunidadesAtualizar")
              .Include("~/Content/js/oportunidades/default.js",
                       "~/Content/js/oportunidades/atualizar.js"));

            bundles.Add(new ScriptBundle("~/bundles/moment")
              .Include("~/Content/plugins/moment/moment-with-locales.js"));

            bundles.Add(new StyleBundle("~/bundles/tagsCSS")
               .Include("~/Content/plugins/tags/tagsinput.css"));

            bundles.Add(new ScriptBundle("~/bundles/tags")
                .Include("~/Content/plugins/tags/tagsinput.js"));

            bundles.Add(new StyleBundle("~/bundles/select2CSS")
             .Include("~/Content/plugins/select2/css/select2.css"));

            bundles.Add(new ScriptBundle("~/bundles/select2")
                .Include("~/Content/plugins/select2/js/select2.min.js"));

            BundleTable.EnableOptimizations = true;
        }
    }
}
