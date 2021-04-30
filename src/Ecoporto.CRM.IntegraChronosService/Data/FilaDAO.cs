using Dapper;
using Ecoporto.CRM.IntegraChronosService.Enums;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Data;

namespace Ecoporto.CRM.IntegraChronosService
{
    public class FilaDAO
    {
        public IEnumerable<Fila> ObterFilaIntegracao()
        {
            using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
            {
                return con.Query<Fila>($"SELECT * FROM TB_CRM_FILA_INTEGRACAO WHERE Status = 1");
            }
        }

        public void AtualizarFila(int id, Status status, string motivo = null)
        {
            using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "Id", value: id, direction: ParameterDirection.Input);
                parametros.Add(name: "Status", value: status, direction: ParameterDirection.Input);
                parametros.Add(name: "Motivo", value: motivo, direction: ParameterDirection.Input);

                con.Execute($"UPDATE TB_CRM_FILA_INTEGRACAO SET Status = :Status, Acao = 1, Motivo = :Motivo, Data_Execucao = SYSDATE WHERE Id = :Id", parametros);
            }
        }
    }
}
