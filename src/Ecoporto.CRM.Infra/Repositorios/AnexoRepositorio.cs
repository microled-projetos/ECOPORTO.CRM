using Dapper;
using Ecoporto.CRM.Business.DTO;
using Ecoporto.CRM.Business.Enums;
using Ecoporto.CRM.Business.Interfaces.Repositorios;
using Ecoporto.CRM.Business.Models;
using Ecoporto.CRM.Infra.Configuracao;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Ecoporto.CRM.Infra.Repositorios
{
    public class AnexoRepositorio : IAnexoRepositorio
    {
        public int IncluirAnexo(Anexo anexo)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "IdProcesso", value: anexo.IdProcesso, direction: ParameterDirection.Input);
                parametros.Add(name: "Anexo", value: anexo.Arquivo, direction: ParameterDirection.Input);
                parametros.Add(name: "DataCadastro", value: DateTime.Now, direction: ParameterDirection.Input);
                parametros.Add(name: "CriadoPor", value: anexo.CriadoPor, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoAnexo", value: anexo.TipoAnexo, direction: ParameterDirection.Input);
                parametros.Add(name: "Versao", value: anexo.Versao, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoDocto", value: anexo.TipoDoc, direction: ParameterDirection.Input);
                parametros.Add(name: "IdArquivo", value: anexo.IdArquivo.ToString().Replace("-", "").ToUpper(), direction: ParameterDirection.Input);

                parametros.Add(name: "Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                con.Execute(@"INSERT INTO CRM.TB_CRM_ANEXOS (Id, IdProcesso, Anexo, DataCadastro, CriadoPor, TipoAnexo, Versao, IdFile, TipoDocto) VALUES (CRM.SEQ_CRM_ANEXOS.NEXTVAL, :IdProcesso, :Anexo, :DataCadastro, :CriadoPor, :TipoAnexo, :Versao, :IdArquivo, :TipoDocto) RETURNING Id INTO :Id", parametros);

                return parametros.Get<int>("Id");
            }
        }

        public void ExcluirAnexo(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                con.Execute(@"DELETE FROM CRM.TB_CRM_ANEXOS WHERE Id = :id", new { id });
            }
        }

        public void ExcluirAnexosOportunidadePorTipo(int processoId, TipoAnexo tipoAnexo)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "ProcessoId", value: processoId, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoAnexo", value: tipoAnexo, direction: ParameterDirection.Input);

                con.Execute(@"DELETE FROM CRM.TB_CRM_ANEXOS WHERE idProcesso = :ProcessoId AND TipoAnexo = :TipoAnexo", parametros);
            }
        }

        public Anexo ObterAnexoPorId(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<Anexo>(@"SELECT * FROM CRM.TB_CRM_ANEXOS WHERE Id = :id", new { id }).FirstOrDefault();
            }
        }

        public AnexosDTO ObterDetalhesAnexo(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<AnexosDTO>(@"
                    SELECT
                        A.Id,
                        A.IdProcesso,
                        A.Anexo,
                        A.DataCadastro,
                        A.TipoAnexo,
                        A.Versao,
                        RAWTOHEX(A.IdFile) IdFile,
                        B.Login As CriadoPor,
                        DECODE(A.TipoAnexo, 1, 'Ficha Faturamento', 2, 'Cancelamento', 3, 'Prêmio Parceria', 4, 'Proposta', 5, 'Outros', 6, 'Solicitação') As TipoAnexoDescricao
                    FROM
                        CRM.TB_CRM_ANEXOS A
                    INNER JOIN
                        CRM.TB_CRM_USUARIOS B ON A.CriadoPor = B.Id
                    WHERE
                        A.Id = :id", new { id }).FirstOrDefault();
            }
        }

        public IEnumerable<AnexosDTO> ObterAnexosPorOportunidade(int oportunidadeId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "IdProcesso", value: oportunidadeId, direction: ParameterDirection.Input);

                return con.Query<AnexosDTO>(@"
                    SELECT 
                        Id, 
                        IdProcesso, 
                        Anexo, 
                        DataCadastro, 
                        TipoAnexo, 
                        Versao, 
                        RAWTOHEX(IdFile) IdFile 
                    FROM 
                        CRM.TB_CRM_ANEXOS, 
                        (
                            SELECT 
                                COUNT(1) CONTAR 
                            FROM 
                                CRM.TB_CRM_ANEXOS 
                            WHERE   
                                IdProcesso = :IdProcesso 
                            AND 
                                (UPPER(ANEXO) LIKE '%.DOC') AND  TipoDocto = 1) B  
                    WHERE 
                        IdProcesso = :IdProcesso 
                    AND 
                        ((UPPER(ANEXO) LIKE '%.DOC' AND B.CONTAR > 0) OR (UPPER(ANEXO) LIKE '%.PDF' AND B.CONTAR = 0))
                    AND 
                        TipoDocto = 1       


                    UNION ALL

                    SELECT 
                        Id, 
                        IdProcesso, 
                        Anexo, 
                        DataCadastro, 
                        TipoAnexo, 
                        Versao, 
                        RAWTOHEX(IdFile) IdFile 
                    FROM 
                        TB_CRM_ANEXOS 
                    WHERE 
                        IdProcesso = :IdProcesso
                    AND 
                        (UPPER(ANEXO) NOT LIKE '%FICHA%' AND UPPER(ANEXO) NOT LIKE '%FATURA%' AND UPPER(ANEXO) like '%.XLS%')
                    AND 
                        TipoDocto = 1", parametros);
            }
        }
    }
}
