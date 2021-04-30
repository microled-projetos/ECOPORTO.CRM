using Dapper;
using Oracle.ManagedDataAccess.Client;
using System.Linq;
using WsSimuladorCalculoTabelas.Configuracao;
using WsSimuladorCalculoTabelas.Models;

namespace WsSimuladorCalculoTabelas.DAO
{
    public class ParametrosDAO
    {
        public Parametros ObterParametros()
        {
            using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
            {
                return con.Query<Parametros>(@"SELECT AnexarSimulador, AtualizaValorTicket FROM CRM.TB_CRM_PARAMETROS").FirstOrDefault();
            }
        }
    }
}