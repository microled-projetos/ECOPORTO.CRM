using Dapper;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace WsConsultaSPC
{
    public class ContaRepositorio
    {
        public Conta ObterContaPorDocumento(string documento)
        {
            using (OracleConnection con = new OracleConnection(Parametros.StringConexao))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Documento", value: documento, direction: ParameterDirection.Input);

                return con.Query<Conta>(@"SELECT * FROM CRM.TB_CRM_CONTAS WHERE REPLACE(REPLACE(REPLACE(Documento,'.',''),'/',''),'-','') = :Documento", parametros).FirstOrDefault();
            }
        }

        public Conta ObterContaPorId(int id)
        {
            using (OracleConnection con = new OracleConnection(Parametros.StringConexao))
            {
                return con.Query<Conta>(@"SELECT * FROM CRM.TB_CRM_CONTAS WHERE Id = :id", new { id }).FirstOrDefault();
            }
        }

        public IEnumerable<Conta> ObterContasPorRaizDocumento(string documento)
        {
            using (OracleConnection con = new OracleConnection(Parametros.StringConexao))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Documento", value: documento.Substring(0, 8), direction: ParameterDirection.Input);

                return con.Query<Conta>($@"
                    SELECT 
                        Id, 
                        Descricao, 
                        Documento, 
                        NomeFantasia, 
                        SituacaoCadastral, 
                        VendedorId, 
                        Segmento, 
                        ClassificacaoFiscal                       
                    FROM 
                        CRM.TB_CRM_CONTAS
                    WHERE 
                        SUBSTR(REPLACE(REPLACE(REPLACE(Documento, '.',''), '/', ''), '-', ''), 0, 8) = :Documento", parametros);
            }
        }
    }
}
