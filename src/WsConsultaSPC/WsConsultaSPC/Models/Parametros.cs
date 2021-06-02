using System.Configuration;

namespace WsConsultaSPC
{
    public class Parametros
    {
        public static string StringConexao = ConfigurationManager.ConnectionStrings["StringConexaoOracle"].ConnectionString;
        public static string WsSPCUrl = ConfigurationManager.AppSettings["WsSPCUrl"].ToString();
        public static string WsSPCUsuario = ConfigurationManager.AppSettings["WsSPCUsuario"].ToString();
        public static string WsSPCSenha = ConfigurationManager.AppSettings["WsSPCSenha"].ToString();
        public static string WsSPCProduto = ConfigurationManager.AppSettings["WsSPCProduto"].ToString();

        public decimal DividaSpc { get; set; }
    }
}