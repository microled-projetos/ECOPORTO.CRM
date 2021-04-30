using System.Configuration;

namespace Ecoporto.CRM.Sharepoint.Models
{
    internal class Parametros
    {
        public static string BaseUrl 
            => ConfigurationManager.AppSettings["ApiAnexosUrl"].ToString();

        public static string Usuario
           => ConfigurationManager.AppSettings["ApiAnexosUsuario"].ToString();

        public static string Senha
            => ConfigurationManager.AppSettings["ApiAnexosSenha"].ToString();
    }
}
