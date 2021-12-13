using Dapper;
using Oracle.ManagedDataAccess.Client;
using System.Data.SqlClient;
using System.Linq;
using WsSimuladorCalculoTabelas.Configuracao;

namespace WsSimuladorCalculoTabelas.DAO
{
    public class ImpostoDAO
    {
        public decimal ObterTaxaImposto( int TipoImposto )
        {
            string Filtro = "";
                switch (TipoImposto)
            {
                case 1:
                    Filtro = "";
                    break;
                case 2:
                    Filtro = " where autonum in(0) ";
                    break;
                case 3:
                    Filtro = "";
                    break;
                case 4:
                    Filtro = " where autonum in(2, 3) " ;
                    break;
                case 5:
                    Filtro = " where autonum in(1, 2) ";
                    break;
                case 6:
                    Filtro = " where autonum in(1, 3) ";
                    break;
                case 7:
                    Filtro = " where autonum in(2) ";
                    break;
                case 8:
                    Filtro = " where autonum in(3) ";
                    break;
                case 9:
                    Filtro = " where autonum in(1) ";
                    break;
               }
                           
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