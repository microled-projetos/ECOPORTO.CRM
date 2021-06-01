using Dapper;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Ecoporto.CRM.IntegraChronosService
{
    public class OportunidadeDAO
    {
        public Oportunidade ObterOportunidadePorId(int id)
        {
            using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Id", value: id, direction: ParameterDirection.Input);

                return con.Query<Oportunidade>(@"SELECT * FROM CRM.VW_CRM_OPORTUNIDADES WHERE Id = :Id", parametros).FirstOrDefault();
            }
        }

        public OportunidadeAdendo ObterAdendoPorId(int id)
        {
            using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
            {
                return con.Query<OportunidadeAdendo>(@"
                        SELECT
                            Id,
                            OportunidadeId,
                            TipoAdendo,
                            CriadoPor,
                            StatusAdendo,
                            DataCadastro                       
                        FROM
                            CRM.TB_CRM_OPORTUNIDADE_ADENDOS
                        WHERE
                            Id = :id", new { id }).FirstOrDefault();
            }
        }
    }
}