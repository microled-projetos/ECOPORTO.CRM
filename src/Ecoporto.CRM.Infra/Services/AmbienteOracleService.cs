using Dapper;
using Ecoporto.CRM.Business.Enums;
using Ecoporto.CRM.Business.Interfaces.Servicos;
using Ecoporto.CRM.Infra.Configuracao;
using Oracle.ManagedDataAccess.Client;
using System.Linq;

namespace Ecoporto.CRM.Infra.Services
{
    public class AmbienteOracleService : IAmbienteOracleService
    {
        public AmbienteOracle Ambiente()
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<AmbienteOracle>(@"SELECT DECODE(global_name, 'PATIODEV', 1, 2) As Ambiente FROM global_name").FirstOrDefault();
            }
        }
    }
}