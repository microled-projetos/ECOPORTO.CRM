using Dapper;
using Ecoporto.CRM.Business.Interfaces.Repositorios;
using Ecoporto.CRM.Infra.Configuracao;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;

namespace Ecoporto.CRM.Infra.Repositorios
{
    public class MargemRepositorio : IMargemRepositorio
    {
        public IEnumerable<string> ObterMargens()
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<string>(@"SELECT DISTINCT MARGEM As Descricao FROM SGIPA.DTE_TB_ARMAZENS WHERE MARGEM IS NOT NULL UNION ALL SELECT 'SVAR' FROM DUAL ORDER BY 1");
            }
        }
    }
}
