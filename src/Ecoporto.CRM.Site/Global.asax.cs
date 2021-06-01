using Ecoporto.CRM.Site.App_Start;
using Ecoporto.CRM.Site.Helpers;
using NLog;
using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Ecoporto.CRM.Site
{
    public class MvcApplication : HttpApplication
    {       
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            ModelBinders.Binders.Add(
               typeof(decimal), new DecimalModelBinder());
            ModelBinders.Binders.Add(
                typeof(decimal?), new DecimalModelBinder());

            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RazorViewEngine());
           
            LogManager.Configuration = NLogConfig.Configure();

            if (!Directory.Exists(HttpContext.Current.Server.MapPath($"~/App_Data")))
            {
                throw new Exception("O diretório App_Data não foi encontrado");
            }
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var logger = DependencyResolver.Current.GetService<ILogger>();

            Exception exception = Server.GetLastError();

            var httpContext = ((HttpApplication)sender).Context;

            httpContext.Response.Clear();
            httpContext.ClearError();
            httpContext.Response.TrySkipIisCustomErrors = true;

            if (exception is HttpException)
            {
                var httpException = (HttpException)exception;
                var status = httpException.GetHttpCode();

                if (status == 404)
                {
                    httpContext.Response.Redirect("~/");
                }
            }
            else
            {
                var ticket = Guid.NewGuid();
                GlobalDiagnosticsContext.Set("ticket", ticket);
                logger.Error(exception, exception.Message);

                var rota = new RouteData();

                rota.Values["controller"] = "Home";
                rota.Values["action"] = "Erro";
                rota.Values["ticket"] = ticket.ToString();
                rota.Values["exception"] = exception.Message;

                httpContext.Response.RedirectToRoute("Default", rota.Values);
            }
        }

        protected void Application_AuthenticateRequest(Object sender, EventArgs e)
        {            
            Identity.Autenticar();
        }
    }
}
