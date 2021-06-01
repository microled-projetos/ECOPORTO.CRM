using Dapper;
using Ecoporto.CRM.Business.DTO;
using Ecoporto.CRM.Business.Interfaces.Repositorios;
using Ecoporto.CRM.Business.Models;
using Ecoporto.CRM.Infra.Configuracao;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Ecoporto.CRM.Infra.Repositorios
{
    public class LoteRepositorio : ILoteRepositorio
    {
        public Lote ObterLotePorId(int lote)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Lote", value: lote, direction: ParameterDirection.Input);

                return con.Query<Lote>(@"
                    SELECT 
                        A.AUTONUM As Id, 
                        NVL(A.TIPO_DOCUMENTO,0) As TipoDocumento, 
                        A.Importador As ImportadorId, 
                        A.Despachante As DespachanteId, 
                        B.Razao As ImportadorDescricao, 
                        C.Razao As DespachanteDescricao,
                        NVL(A.FLAG_ATIVO, 0) AS Ativo,
                        A.NUMERO
                    FROM 
                        SGIPA.TB_BL A 
                    LEFT JOIN 
                        SGIPA.TB_CAD_PARCEIROS B ON A.Importador = B.AUTONUM 
                    LEFT JOIN 
                        SGIPA.TB_CAD_PARCEIROS C ON A.Despachante = C.AUTONUM
                    WHERE
                        A.AUTONUM = :Lote", parametros).FirstOrDefault();
            }
        }

        public Lote ExisteAverbacao(int oportunidadeId, DateTime? dataCancelamento)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);
                parametros.Add(name: "DataCancelamento", value: dataCancelamento, direction: ParameterDirection.Input);

                return con.Query<Lote>(@"
                    SELECT 
                        AUTONUM As NumeroLote, 
                        NVL(TIPO_DOCUMENTO,0) As TipoDocumento, 
                        Importador As ImportadorId, 
                        Despachante As DespachanteId
                    FROM 
                        SGIPA.TB_BL BL
                    INNER JOIN 
                        CRM.TB_CRM_OPORTUNIDADES OP ON OP.TABELAID = BL.AUTONUM_LISTA
                    WHERE 
                        OP.ID = :OportunidadeId
                    AND 
                        BL.DT_FIM_AVERBA > :DataCancelamento", parametros).FirstOrDefault();
            }
        }

        public IEnumerable<LotesMasterDTO> ObterLotesMaster(int lote)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Lote", value: lote, direction: ParameterDirection.Input);

                return con.Query<LotesMasterDTO>(@"
                    WITH LNIVEL (
                        LOTE,
                        NUMERO,
                        FLAG_ATIVO,
                        MASTER
                    ) AS (
                        SELECT
                            AUTONUM LOTE,
                            NUMERO,FLAG_ATIVO,
                            BL_MASTER MASTER
                        FROM
                            SGIPA.TB_BL
                        WHERE
                            AUTONUM = :Lote
                        UNION ALL
                        SELECT
                            M.AUTONUM LOTE,
                            M.NUMERO,M.FLAG_ATIVO,
                            BL_MASTER MASTER
                        FROM
                            SGIPA.TB_BL M
                            INNER JOIN LNIVEL C ON M.BL_MASTER = C.LOTE
                    )
                    SELECT
                        LOTE,
                        NUMERO,
                        MASTER
                    FROM
                        LNIVEL WHERE FLAG_ATIVO = 1
                    GROUP BY
                        LOTE,
                        NUMERO,
                        FLAG_ATIVO,
                        MASTER
                    ORDER BY
                        LOTE", parametros);
            }
        }
    }
}
