using Dapper;
using Oracle.ManagedDataAccess.Client;
using System.Linq;

namespace Ecoporto.CRM.IntegraChronosService
{
    public class ModeloDAO
    {       
        public Modelo ObterModeloPorId(int id)
        {
            using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
            {
                return con.Query<Modelo>(@"SELECT * FROM CRM.TB_CRM_MODELO WHERE Id = :mId", new { mId = id }).FirstOrDefault();
            }
        }
    }
}
