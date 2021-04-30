using Dapper;
using Ecoporto.CRM.Business.Extensions;
using Ecoporto.CRM.Business.Interfaces.Repositorios;
using Ecoporto.CRM.Business.Interfaces.Servicos;
using Ecoporto.CRM.Business.Models;
using Ecoporto.CRM.Business.Models.Oportunidades;
using Ecoporto.CRM.Infra.Configuracao;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;

namespace Ecoporto.CRM.Infra.Services
{
    public class ConcomitanciaTabelaService : IConcomitanciaTabelaService
    {
        private readonly IOportunidadeRepositorio _oportunidadeRepositorio;

        public ConcomitanciaTabelaService(IOportunidadeRepositorio oportunidadeRepositorio)
        {
            _oportunidadeRepositorio = oportunidadeRepositorio;
        }

        public IEnumerable<OportunidadeTabelaConcomitante> ObtemPropostasDuplicadasCRM(Oportunidade oportunidade)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "OportunidadeId", value: oportunidade.Id, direction: ParameterDirection.Input);
                parametros.Add(name: "DataInicio", value: oportunidade.OportunidadeProposta.DataInicio ?? DateTime.Now.AddDays(-360), direction: ParameterDirection.Input);
                parametros.Add(name: "DataTermino", value: oportunidade.OportunidadeProposta?.DataTermino ?? DateTime.Now.AddDays(1000), direction: ParameterDirection.Input);

                var filtro = string.Empty;

                if (oportunidade.RevisaoId.HasValue)
                {
                    if (oportunidade.RevisaoId.Value > 0)
                    {
                        filtro += " AND A.ID <> :RevisaoId ";
                        parametros.Add(name: "RevisaoId", value: oportunidade.RevisaoId, direction: ParameterDirection.Input);
                    }
                }

                return con.Query<OportunidadeTabelaConcomitante>($@"
                        SELECT 
                            DISTINCT 
                                A.Id, 
                                A.Identificacao, 
                                A.ImportadorId, 
                                A.DespachanteId, 
                                A.ColoaderId, 
                                A.CoColoaderId, 
                                A.CoColoader2Id, 
                                A.Segmento,
                                A.CNPJ_IMPORTADOR As CnpjImportador,
                                A.CNPJ_IMPORTADOR As CnpjDespachante,
                                A.CNPJ_COLOADER As CnpjColoader,
                                A.CNPJ_COCOLOADER As CnpjCoColoader,
                                A.CNPJ_COCOLOADER2 As CnpjCoColoader2
                        FROM 
                            VW_PROPOSTAS_CONCOMITANTES A 
                        INNER JOIN 
                            VW_PROPOSTAS_CONCOMITANTES B ON 
                                    A.IMPORTADORID = B.ImportadorId 
                                AND A.DespachanteId = B.DespachanteId AND A.ColoaderId = B.ColoaderId 
                                    AND A.CoColoaderId = B.CoColoaderId AND A.CoColoader2Id = B.CoColoader2Id 
                                        AND A.NvoccId = B.NvoccId AND B.Acordo = 0 
                                            AND  A.HubPort = B.HubPort 
                                            AND A.Acordo = B.Acordo 
                                            AND  A.COBRANCAESPECIAL=B.COBRANCAESPECIAL
                                            AND  A.TIPOSERVICO=B.TIPOSERVICO
                        AND 
                            (A.DataTermino IS NULL OR A.DataTermino >= SYSDATE) 
                       AND
                            (A.DataInicio <= :DataTermino or B.Datatermino is null) 
                        AND 
                            A.Id <> :OportunidadeId AND A.EmpresaId = 1  
                        AND 
                            B.ID = :OportunidadeId {filtro}", parametros);
            }
        }

        public IEnumerable<OportunidadeTabelaConcomitante> ObtemTabelasDuplicadasChronos(int oportunidadeId)
        {
            using (var con = new OracleConnection(Config.StringConexao()))
            {               
                using (OracleCommand cmd = new OracleCommand("PR_CONCOMITANCIA_CRM", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(new OracleParameter
                    {
                        OracleDbType = OracleDbType.Int32,
                        Direction = ParameterDirection.Input,
                        Value = oportunidadeId
                    });

                    cmd.Parameters.Add(new OracleParameter
                    {
                        OracleDbType = OracleDbType.RefCursor,
                        Direction = ParameterDirection.Output
                    });

                    con.Open();
                    OracleDataReader dr = cmd.ExecuteReader();

                    var lista = new List<OportunidadeTabelaConcomitante>();

                    while (dr.Read())
                    {
                        lista.Add(new OportunidadeTabelaConcomitante
                        {
                            Id = dr["Id"].ToString().ToInt(),
                            ImportadorId = dr["ImportadorId"].ToString().ToInt(),
                            DespachanteId = dr["DespachanteId"].ToString().ToInt(),
                            ColoaderId = dr["ColoaderId"].ToString().ToInt(),
                            CoColoaderId = dr["CoColoaderId"].ToString().ToInt(),
                            CoColoader2Id = dr["CoColoader2Id"].ToString().ToInt(),
                            CnpjImportador = dr["CnpjImportador"].ToString(),
                            CnpjDespachante = dr["CnpjDespachante"].ToString(),
                            CnpjColoader = dr["CnpjColoader"].ToString(),
                            CnpjCoColoader = dr["CnpjCoColoader"].ToString(),
                            CnpjCoColoader2 = dr["CnpjCoColoader2"].ToString(),
                        });
                    }

                    try
                    {
                        con.Execute($"DROP TABLE CRM.TB_{oportunidadeId.ToString()}");
                    }
                    catch (Exception ex)
                    {

                    }

                    return lista;
                }                
            }
        }
    }
}
