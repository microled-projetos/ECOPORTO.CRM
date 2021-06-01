using Ecoporto.CRM.Business.Helpers;
using Ecoporto.CRM.Business.Interfaces.Repositorios;
using Ecoporto.CRM.Infra.Configuracao;
using Ecoporto.CRM.Site.Extensions;
using Microsoft.Reporting.WebForms;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Net;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace Ecoporto.CRM.Site.Controllers
{
    [AllowAnonymous]
    public class ReportsController : Controller
    {
        private readonly IUsuarioRepositorio _usuarioRepositorio;

        public ReportsController(IUsuarioRepositorio usuarioRepositorio)
        {
            _usuarioRepositorio = usuarioRepositorio;
        }

        private string ServidorRelatoriosUrl
            => ConfigurationManager.AppSettings["ServidorRelatoriosUrl"].ToString();

        private string ServidorRelatoriosDiretorio
            => ConfigurationManager.AppSettings["ServidorRelatoriosDiretorio"].ToString();

        private string ServidorRelatoriosUsuario
           => ConfigurationManager.AppSettings["ServidorRelatoriosUsuario"].ToString();

        private string ServidorRelatoriosSenha
           => ConfigurationManager.AppSettings["ServidorRelatoriosSenha"].ToString();

        private string ServidorRelatoriosDominio
           => ConfigurationManager.AppSettings["ServidorRelatoriosDominio"].ToString();

        [ValidateInput(false)]
        public ActionResult Index(string relatorio, int? validaUsuario = 0)
        {           
            ReportViewer reportViewer = new ReportViewer
            {
                ProcessingMode = ProcessingMode.Remote,
                SizeToReportContent = true,
                AsyncRendering = false,
                Width = Unit.Percentage(100)
            };
            
            reportViewer.ServerReport.ReportServerCredentials = new CustomReportCredentials(ServidorRelatoriosUsuario, ServidorRelatoriosSenha, ServidorRelatoriosDominio);
            reportViewer.ServerReport.ReportServerUrl = new Uri(ServidorRelatoriosUrl);
            reportViewer.ServerReport.ReportPath = $"/{ServidorRelatoriosDiretorio}/{relatorio}";

            if (validaUsuario == 1)
            {
                ReportParameter usuarioLogado = new ReportParameter("strUser");
                usuarioLogado.Values.Add(User.ObterId().ToString());
                reportViewer.ServerReport.SetParameters(usuarioLogado);
            }
            
            ViewBag.ReportViewer = reportViewer;

            return View();
        }
    }

    public class CustomReportCredentials : IReportServerCredentials
    {
        private string _UserName;
        private string _PassWord;
        private string _DomainName;

        public CustomReportCredentials(string UserName, string PassWord, string DomainName)
        {
            _UserName = UserName;
            _PassWord = PassWord;
            _DomainName = DomainName;
        }

        public System.Security.Principal.WindowsIdentity ImpersonationUser
        {
            get { return null; }
        }

        public ICredentials NetworkCredentials
        {
            get { return new NetworkCredential(_UserName, _PassWord, _DomainName); }
        }

        public bool GetFormsCredentials(out Cookie authCookie, out string user,
         out string password, out string authority)
        {
            authCookie = null;
            user = password = authority = null;
            return false;
        }
    }
}