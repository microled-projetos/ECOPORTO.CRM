using System.Configuration;

namespace WsSimuladorCalculoTabelas.Configuracao
{
    public static class Configuracoes
    {
        public static string StringConexao()
        {
            return ConfigurationManager.ConnectionStrings["StringConexao"].ConnectionString;
        }

        public static string BancoEmUso()
        {
            return ConfigurationManager.AppSettings["Banco"].ToString();
        }
    }
}