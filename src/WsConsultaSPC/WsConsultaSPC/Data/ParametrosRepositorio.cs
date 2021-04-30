using Dapper;
using Oracle.ManagedDataAccess.Client;
using System.Linq;

namespace WsConsultaSPC
{
    public class ParametrosRepositorio 
    {
        public Parametros ObterParametros()
        {
            using (OracleConnection con = new OracleConnection(Parametros.StringConexao))
            {
                return con.Query<Parametros>(@"SELECT DividaSpc FROM CRM.TB_CRM_PARAMETROS").FirstOrDefault();
            }
        }
    }
}