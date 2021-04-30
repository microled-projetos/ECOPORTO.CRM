using Dapper;
using Ecoporto.CRM.Business.DTO;
using Ecoporto.CRM.Business.Extensions;
using Ecoporto.CRM.Business.Interfaces.Repositorios;
using Ecoporto.CRM.Infra.Configuracao;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Ecoporto.CRM.Infra.Repositorios
{
    public class ImpostosExcecaoRepositorio : IImpostosExcecaoRepositorio
    {     
        public IEnumerable<ImpostosExcecaoDTO> ObterServicos(int modeloId, int oportunidadeId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "ModeloId", value: modeloId, direction: ParameterDirection.Input);
                parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);

                return con.Query<ImpostosExcecaoDTO>(@"
                    SELECT 
                        DISTINCT 
                            ServicoId, 
                            Servico, 
                            Id, 
                            Tipo 
                    FROM 
                        (
                            SELECT 
                                    A.Id AS ServicoId, 
                                    A.DESCRICAO AS Servico,
                                    (SELECT Id FROM CRM.TB_CRM_OPORTUNIDADES_IMPOSTOS WHERE ModeloId = C.ModeloId AND SERVICOID = C.ServicoId AND OportunidadeId = C.OportunidadeId) As Id,
                                    (SELECT TIPO FROM CRM.TB_CRM_OPORTUNIDADES_IMPOSTOS WHERE ModeloId = C.ModeloId AND SERVICOID = C.ServicoId AND OportunidadeId = C.OportunidadeId) As Tipo
                                FROM 
                                    CRM.TB_CRM_SERVICOS A
                                INNER JOIN
                                    CRM.TB_CRM_SERVICO_IPA B ON A.Id = B.ServicoId
                                INNER JOIN
                                    CRM.TB_CRM_OPORTUNIDADE_LAYOUT C ON A.Id = C.ServicoId
                                WHERE 
                                    C.ModeloId = :ModeloId AND C.OportunidadeId = :OportunidadeId
                                ORDER BY 
                                    C.LINHA
                        ) 
                            ORDER BY Servico
                    ", parametros);
            }
        }

        public ImpostosExcecaoDTO ObterPorId(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Id", value: id, direction: ParameterDirection.Input);

                return con.Query<ImpostosExcecaoDTO>(@"
                    SELECT
                        Id,
                        ServicoId,
                        ModeloId,
                        OportunidadeId,
                        PIS,
                        ISS,
                        COFINS,
                        ValorPIS,
                        ValorISS,
                        ValorCOFINS,
                        Tipo
                    FROM
                        TB_CRM_OPORTUNIDADES_IMPOSTOS
                    WHERE
                        Id = :Id", parametros).FirstOrDefault();
            }
        }

        public void GravarServicos(ImpostosExcecaoDTO model)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                con.Open();

                var parametros = new DynamicParameters();

                using (var transaction = con.BeginTransaction())
                {
                    foreach (var servicoId in model.ServicosSelecionados)
                    {
                        parametros = new DynamicParameters();

                        parametros.Add(name: "ModeloId", value: model.ModeloId, direction: ParameterDirection.Input);
                        parametros.Add(name: "OportunidadeId", value: model.OportunidadeId, direction: ParameterDirection.Input);
                        parametros.Add(name: "ServicoId", value: servicoId, direction: ParameterDirection.Input);
                        parametros.Add(name: "PIS", value: model.PIS.ToInt(), direction: ParameterDirection.Input);
                        parametros.Add(name: "ISS", value: model.ISS.ToInt(), direction: ParameterDirection.Input);
                        parametros.Add(name: "COFINS", value: model.COFINS.ToInt(), direction: ParameterDirection.Input);
                        parametros.Add(name: "ValorPIS", value: model.ValorPIS, direction: ParameterDirection.Input);
                        parametros.Add(name: "ValorISS", value: model.ValorISS, direction: ParameterDirection.Input);
                        parametros.Add(name: "ValorCOFINS", value: model.ValorCOFINS, direction: ParameterDirection.Input);
                        parametros.Add(name: "Tipo", value: model.Tipo, direction: ParameterDirection.Input);

                        con.Execute(@"DELETE FROM CRM.TB_CRM_OPORTUNIDADES_IMPOSTOS WHERE ModeloId = :ModeloId AND OportunidadeId = :OportunidadeId AND ServicoId = :ServicoId", parametros, transaction);

                        con.Execute(@"INSERT INTO CRM.TB_CRM_OPORTUNIDADES_IMPOSTOS (ID, MODELOID, OPORTUNIDADEID, SERVICOID, PIS, ISS, COFINS, VALORPIS, VALORISS, VALORCOFINS, TIPO) VALUES (CRM.SEQ_CRM_OPORTUNIDADES_IMPOSTOS.NEXTVAL, :ModeloId, :OportunidadeId, :ServicoId, :PIS, :ISS, :COFINS, :ValorPIS, :ValorISS, :ValorCOFINS, :Tipo)", parametros, transaction);
                    }

                    parametros = new DynamicParameters();
                    parametros.Add(name: "OportunidadeId", value: model.OportunidadeId, direction: ParameterDirection.Input);

                    con.Execute(@"UPDATE CRM.TB_CRM_OPORTUNIDADES SET IMPOSTOID = 3 WHERE Id = :OportunidadeId", parametros, transaction);

                    transaction.Commit();
                }
            }
        }

        public void Excluir(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Id", value: id, direction: ParameterDirection.Input);

                con.Execute(@"DELETE FROM CRM.TB_CRM_OPORTUNIDADES_IMPOSTOS WHERE Id = :Id", parametros);
            }
        }

        public void ExcluirTodosDaOportunidade(int oportunidadeId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);

                con.Execute(@"DELETE FROM CRM.TB_CRM_OPORTUNIDADES_IMPOSTOS WHERE OportunidadeId = :OportunidadeId", parametros);
            }
        }
    }
}