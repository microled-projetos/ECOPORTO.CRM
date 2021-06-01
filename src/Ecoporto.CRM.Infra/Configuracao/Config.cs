using System;
using System.Configuration;

namespace Ecoporto.CRM.Infra.Configuracao
{
    public static class Config
    {
        public static string StringConexao()
        {
            return ConfigurationManager.ConnectionStrings["StringConexaoOracle"].ConnectionString;
        }

        public static string ChaveSecreta() => "XkWuAR2LvIo4QuRt8oSPgFbPAEPdw5Ai";

        public static string Ambiente() => Environment.GetEnvironmentVariable("ASPNET_ENV");
    }
}
