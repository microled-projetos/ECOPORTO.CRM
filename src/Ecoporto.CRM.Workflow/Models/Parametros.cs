using System.Configuration;

namespace Ecoporto.CRM.Workflow.Models
{
    internal class Parametros
    {
        public static string BaseUrl 
            => ConfigurationManager.AppSettings["ApiWorkflowUrl"].ToString();

        public static string Usuario
            => ConfigurationManager.AppSettings["ApiWorkflowUsuario"].ToString();

        public static string Senha
            => ConfigurationManager.AppSettings["ApiWorkflowSenha"].ToString();
    }
}
