using Dapper;
using Oracle.ManagedDataAccess.Client;
using System.Data.SqlClient;
using System.Linq;
using WsSimuladorCalculoTabelas.Configuracao;
using WsSimuladorCalculoTabelas.Models;

namespace WsSimuladorCalculoTabelas.DAO
{
    public class ContaDAO
    {
        public Conta ObterContaPorId(int id)
        {
            //te
            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    return con.Query<Conta>(@"SELECT * FROM CRM.TB_CRM_CONTAS WHERE Id = :id", new { id }).FirstOrDefault();
                }
            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuracoes.StringConexao()))
                {
                    return con.Query<Conta>(@"SELECT * FROM CRM..TB_CRM_CONTAS WHERE Id = @id", new { id }).FirstOrDefault();
                }
            }
        }

        public Conta ObterContaPorDocumento(string documento)
        {
            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    return con.Query<Conta>(@"SELECT * FROM CRM.TB_CRM_CONTAS WHERE Documento = :documento", new { documento }).FirstOrDefault();
                }
            }
            else
            {
                using (SqlConnection con = new SqlConnection(Configuracoes.StringConexao()))
                {
                    return con.Query<Conta>(@"SELECT * FROM CRM..TB_CRM_CONTAS WHERE Documento = @documento", new { documento }).FirstOrDefault();
                }
            }
        }
    }
}