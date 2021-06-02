using Dapper;
using Oracle.ManagedDataAccess.Client;
using System.Data.SqlClient;
using System.Linq;
using WsSimuladorCalculoTabelas.Configuracao;

namespace WsSimuladorCalculoTabelas.DAO
{
    public class ImpostoDAO
    {
        public decimal ObterTaxaImposto()
        {
            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    return con.Query<decimal>(@"SELECT NVL(SUM(Taxa), 0) / 100 Taxa FROM SGIPA.TB_CAD_IMPOSTOS").Single();
                }
            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuracoes.StringConexao()))
                {
                    return con.Query<decimal>(@"SELECT ISNULL(SUM(Taxa), 0) / 100 Taxa FROM SGIPA..TB_CAD_IMPOSTOS").Single();
                }
            }
        }
    }
}