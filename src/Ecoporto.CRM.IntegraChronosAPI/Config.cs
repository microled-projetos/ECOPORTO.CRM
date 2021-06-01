using System;
using System.Configuration;

namespace Ecoporto.CRM.IntegraChronosAPI
{
    public static class Config
    {
        public static string StringConexao()
        {
            return ConfigurationManager.ConnectionStrings["StringConexaoOracle"].ConnectionString;
        }

        public static string SecretUserString() => "crm_integracao";

        public static string SecretKeyString() => "5^EQ@WB48Qye?4>X";        
    }
}
