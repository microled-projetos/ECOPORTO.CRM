using Dapper;
using Ecoporto.CRM.Business.Enums;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using WsSimuladorCalculoTabelas.Configuracao;
using WsSimuladorCalculoTabelas.Models;

namespace WsSimuladorCalculoTabelas.DAO
{
    public class AnexosDAO
    {
        public int IncluirAnexo(Anexo anexo)
        {
            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
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
            else
            {
                using (SqlConnection con = new SqlConnection(Configuracoes.StringConexao()))
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

                    return con.Query<int>(@"INSERT INTO CRM..TB_CRM_ANEXOS (IdProcesso, Anexo, DataCadastro, CriadoPor, TipoAnexo, Versao, IdFile, TipoDocto) VALUES (@IdProcesso, @Anexo, @DataCadastro, @CriadoPor, @TipoAnexo, @Versao, @IdArquivo, @TipoDocto); SELECT CAST(SCOPE_IDENTITY() AS INT)", parametros).Single();
                }
            }            
        }

        public IEnumerable<Anexo> ObterAnexosOportunidadePorTipo(int processoId, TipoAnexo tipoAnexo)
        {
            using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "ProcessoId", value: processoId, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoAnexo", value: tipoAnexo, direction: ParameterDirection.Input);

                return con.Query<Anexo>(@"SELECT Id, Anexo As Arquivo, IdProcesso, RAWTOHEX(IdFile) As IdArquivo FROM CRM.TB_CRM_ANEXOS WHERE idProcesso = :ProcessoId AND TipoAnexo = :TipoAnexo", parametros);
            }
        }

        public void Excluir(int id)
        {
            using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Id", value: id, direction: ParameterDirection.Input);

                con.Execute(@"DELETE FROM CRM.TB_CRM_ANEXOS WHERE Id = :Id", parametros);
            }
        }
    }
}