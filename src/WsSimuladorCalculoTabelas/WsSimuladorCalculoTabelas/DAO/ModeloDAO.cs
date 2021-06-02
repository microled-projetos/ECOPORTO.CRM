using Dapper;
using Oracle.ManagedDataAccess.Client;
using System.Linq;
using WsSimuladorCalculoTabelas.Configuracao;
using WsSimuladorCalculoTabelas.Models;

namespace WsSimuladorCalculoTabelas.DAO
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