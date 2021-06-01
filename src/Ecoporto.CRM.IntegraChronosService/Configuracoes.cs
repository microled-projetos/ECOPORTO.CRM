using System.Configuration;

namespace Ecoporto.CRM.IntegraChronosService
{
    public static class Configuracoes
    {
        public static string StringConexao()
        {
            return ConfigurationManager.ConnectionStrings["StringConexaoOracle"].ConnectionString;
        }
    }
}