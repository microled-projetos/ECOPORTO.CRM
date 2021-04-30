using Dapper;
using Ecoporto.CRM.Business.Utils;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using System.Threading.Tasks;

namespace Ecoporto.CRM.Infra.Auditoria
{
    public enum TipoAuditoria
    {
        INSERT,
        UPDATE,
        DELETE
    }

    public sealed class Logger
    {
        public async static Task Auditar(object objeto, TipoAuditoria tipoAuditoria, string usuario)
        {
            var tipoObjeto = objeto.GetType().Name;

            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "TipoObjeto", value: objeto, direction: ParameterDirection.Input);
                parametros.Add(name: "Objeto", value: tipoAuditoria, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoAuditoria", value: tipoAuditoria, direction: ParameterDirection.Input);
                parametros.Add(name: "Usuario", value: tipoAuditoria, direction: ParameterDirection.Input);
                parametros.Add(name: "Data", value: DateTime.Now, direction: ParameterDirection.Input);

                await con.QueryAsync(@"INSERT INTO CRM.TB_CRM_MERCADORIAS (Id, Descricao, Status) VALUES (CRM.SEQ_CRM_MERCADORIAS.NEXTVAL, :Descricao, :Status)", parametros);
            }
        }
    }    
}
