using Dapper;
using Ecoporto.CRM.Infra.Configuracao;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Data;

namespace Ecoporto.CRM.Infra.Busca
{
    public class BuscaInterna : IBusca
    {
        public IEnumerable<BuscaInternaResultado> Buscar(string criterio, int? usuarioId)
        {
            criterio = "%" + criterio.Trim().ToUpper() + "%";

            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "Criterio", value: criterio, direction: ParameterDirection.Input);

                // É um usuário externo

                if (usuarioId.HasValue)
                {
                    parametros.Add(name: "UsuarioId", value: usuarioId, direction: ParameterDirection.Input);

                    var SQL = @"
                        
                        SELECT
                            TERMO, MENU, CHAVE
                        FROM
                            (
                                SELECT 
                                    DISTINCT 
                                        SUBSTR(TERMO, 0, 50) TERMO, 
                                        MENU, 
                                        CHAVE 
                                FROM 
                                    CRM.TB_CRM_BUSCA 
                                WHERE 
                                    TRIM(UPPER(TERMO)) LIKE :Criterio 
                                AND
                                    UPPER(MENU) <> 'CONTAS' AND UPPER(MENU) <> 'OPORTUNIDADES'

                                UNION ALL

                                SELECT 
                                    DISTINCT 
                                        SUBSTR(A.TERMO, 0, 50) TERMO, 
                                        A.MENU, 
                                        A.CHAVE 
                                FROM 
                                    CRM.TB_CRM_BUSCA A
                                INNER JOIN
                                    CRM.TB_CRM_CONTAS B ON A.Chave = B.Id
                                WHERE 
                                    TRIM(UPPER(A.TERMO)) LIKE :Criterio 
                                AND
                                    UPPER(MENU) = 'CONTAS'
                                AND
                                    B.Id IN (SELECT ContaId FROM TB_CRM_USUARIOS_CONTAS WHERE UsuarioId = :UsuarioId)

                                UNION ALL

                                SELECT 
                                    DISTINCT 
                                        SUBSTR(A.TERMO, 0, 50) TERMO, 
                                        A.MENU, 
                                        A.CHAVE 
                                FROM 
                                    CRM.TB_CRM_BUSCA A
                                INNER JOIN
                                    CRM.TB_CRM_OPORTUNIDADES B ON A.Chave = B.Id
                                LEFT JOIN
                                    TB_CRM_OPORTUNIDADE_CLIENTES C ON B.Id = C.OportunidadeId
                                LEFT JOIN
                                    TB_CRM_CONTAS D ON C.ContaId = D.Id        
                                LEFT JOIN
                                    TB_CRM_OPORTUNIDADE_GRUPO_CNPJ E ON B.Id = E.OportunidadeId
                                LEFT JOIN
                                    TB_CRM_CONTAS F ON C.ContaId = F.Id        
                                LEFT  JOIN
                                    TB_CRM_OPORTUNIDADE_ADENDOS G ON B.Id = G.OportunidadeId
                                LEFT JOIN
                                    TB_CRM_ADENDO_SUB_CLIENTE H ON G.Id = H.AdendoId
                                LEFT JOIN
                                    TB_CRM_CONTAS I ON H.SubClienteId = I.Id
                                LEFT JOIN
                                    TB_CRM_ADENDO_GRUPO_CNPJ J ON G.Id = J.AdendoId
                                LEFT JOIN
                                    TB_CRM_CONTAS K ON J.GrupoCnpjId = K.Id
                                WHERE             
                                    UPPER(MENU) = 'OPORTUNIDADES'
                                AND
                                    B.ContaId IN (SELECT ContaId FROM TB_CRM_USUARIOS_CONTAS WHERE UsuarioId = :UsuarioId)
                                AND
                                    (TRIM(UPPER(A.TERMO)) LIKE :Criterio
                                        OR TRIM(UPPER(D.Descricao)) LIKE :Criterio
                                            OR TRIM(UPPER(D.Documento)) LIKE :Criterio
                                                OR TRIM(UPPER(F.Descricao)) LIKE :Criterio 
                                                    OR TRIM(UPPER(F.Documento)) LIKE :Criterio 
                                                        OR TRIM(UPPER(I.Descricao)) LIKE :Criterio
                                                            OR TRIM(UPPER(I.Documento)) LIKE :Criterio
                                                                OR TRIM(UPPER(K.Descricao)) LIKE :Criterio
                                                                    OR TRIM(UPPER(K.Documento)) LIKE :Criterio)
                            ) WHERE 
                                ROWNUM < 10";

                    return con.Query<BuscaInternaResultado>(SQL, parametros);
                }

                return con.Query<BuscaInternaResultado>(@"
                    SELECT 
                        DISTINCT 
                            SUBSTR(TERMO, 0, 50) TERMO, 
                            MENU, 
                            CHAVE 
                    FROM 
                        CRM.TB_CRM_BUSCA 
                    WHERE 
                        TRIM(UPPER(TERMO)) LIKE :Criterio 
                    AND 
                        ROWNUM < 10", parametros);
            }
        }
    }
}
