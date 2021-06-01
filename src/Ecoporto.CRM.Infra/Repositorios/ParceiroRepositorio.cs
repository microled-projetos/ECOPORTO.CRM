using Dapper;
using Ecoporto.CRM.Business.Interfaces.Repositorios;
using Ecoporto.CRM.Business.Models;
using Ecoporto.CRM.Infra.Configuracao;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Ecoporto.CRM.Infra.Repositorios
{
    public class ParceiroRepositorio : IParceiroRepositorio
    {
        public Parceiro ObterParceiroPorDocumento(string documento)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<Parceiro>(@"
                        SELECT 
                            FANTASIA As NomeFantasia, 
                            IE As InscricaoEstadual, 
                            LOGRADOURO, 
                            NVL(NUM_END,0) As Numero, 
                            BAIRRO, 
                            CEP, 
                            COMPLEMENTO_END As Complemento, 
                            NVL(ESTADO,0) ESTADO, 
                            CIDADE 
                        FROM 
                            SGIPA.TB_CAD_PARCEIROS 
                        WHERE CGC = :documento", new
                {
                    documento
                }).FirstOrDefault();
            }
        }

        public Parceiro ObterParceiroPorId(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Id", value: id, direction: ParameterDirection.Input);

                return con.Query<Parceiro>(@"
                        SELECT 
                            FANTASIA As NomeFantasia, 
                            IE As InscricaoEstadual, 
                            LOGRADOURO, 
                            NVL(NUM_END,0) As Numero, 
                            BAIRRO, 
                            CEP, 
                            COMPLEMENTO_END As Complemento, 
                            NVL(ESTADO,0) ESTADO, 
                            CIDADE 
                        FROM 
                            SGIPA.TB_CAD_PARCEIROS 
                        WHERE AUTONUM = :Id", parametros).FirstOrDefault();
            }
        }

        public Parceiro ObterDetalhesImportadorPorCnpj(string cnpj)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Cnpj", value: cnpj, direction: ParameterDirection.Input);

                return con.Query<Parceiro>(@"
                        SELECT 
                            DECODE(B.CIDADE_COB,'',B.CIDADE,' ',B.CIDADE,B.CIDADE_COB) AS Cidade, 
                            CASE WHEN LENGTH(REPLACE(REPLACE(REPLACE(REPLACE(NVL(B.CGC,''), '.',''), '/', ''), '-', ''),'_','')) < 11 THEN 'F' ELSE 'J' END AS TipoCliente 
                        FROM 
                            SGIPA.TB_CAD_PARCEIROS A, 
                            SGIPA.TB_CAD_PARCEIROS B 
                        WHERE 
                            B.FLAG_ATIVO = 1 
                        AND 
                            A.CODCLI_FATURA = B.CODCLI_SAP 
                        AND 
                            A.CGC = :Cnpj", parametros).FirstOrDefault();
            }
        }

        public Parceiro ObterDetalhesExportadorPorCnpj(string cnpj)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Cnpj", value: cnpj, direction: ParameterDirection.Input);

                return con.Query<Parceiro>(@"
                        SELECT 
                            DECODE(B.CIDADE_COB,'',B.CIDADE,' ',B.CIDADE,B.CIDADE_COB) AS Cidade, 
                            CASE WHEN LENGTH(REPLACE(REPLACE(REPLACE(REPLACE(NVL(B.CGC,''), '.',''), '/', ''), '-', ''),'_','')) < 11 THEN 'F' ELSE 'J' END AS TipoCliente 
                        FROM 
                            REDEX.TB_CAD_PARCEIROS A, 
                            REDEX.TB_CAD_PARCEIROS B 
                        WHERE 
                            B.FLAG_ATIVO = 1 
                        AND 
                            A.CODCLI_FATURA = B.CODCLI_SAP 
                        AND 
                            A.CGC = :Cnpj", parametros).FirstOrDefault();
            }
        }

        public IEnumerable<Parceiro> ObterArmadoresPorDescricao(string descricao)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();                
                parametros.Add(name: "Criterio", value: "%" + descricao.ToUpper() + "%", direction: ParameterDirection.Input);

                return con.Query<Parceiro>(@"
                    SELECT  
                        DISTINCT 
                            AUTONUM As Id, 
                            RAZAO As RazaoSocial, 
                            DECODE(FANTASIA, NULL, RAZAO, FANTASIA) As NomeFantasia, 
                            NVL(CGC,' ') As Documento 
                    FROM 
                        SGIPA.TB_CAD_PARCEIROS 
                    WHERE 
                        NVL(FLAG_ARMADOR,0) = 1 
                    AND 
                        FLAG_ATIVO = 1 
                    AND 
                        (UPPER(RAZAO) LIKE :Criterio OR CGC LIKE :Criterio)
                    AND
                        ROWNUM < 25", parametros);
            }
        }
    }
}
