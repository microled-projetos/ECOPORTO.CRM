using Dapper;
using Ecoporto.CRM.Business.DTO;
using Ecoporto.CRM.Business.Interfaces.Repositorios;
using Ecoporto.CRM.Business.Models;
using Ecoporto.CRM.Infra.Configuracao;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Ecoporto.CRM.Infra.Repositorios
{
    public class ContatoRepositorio : IContatoRepositorio
    {        
        public int Cadastrar(Contato contato)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                
                parametros.Add(name: "Nome", value: contato.Nome, direction: ParameterDirection.Input);
                parametros.Add(name: "Sobrenome", value: contato.Sobrenome, direction: ParameterDirection.Input);
                parametros.Add(name: "Telefone", value: contato.Telefone, direction: ParameterDirection.Input);
                parametros.Add(name: "Celular", value: contato.Celular, direction: ParameterDirection.Input);
                parametros.Add(name: "Email", value: contato.Email, direction: ParameterDirection.Input);
                parametros.Add(name: "Cargo", value: contato.Cargo, direction: ParameterDirection.Input);
                parametros.Add(name: "Departamento", value: contato.Departamento, direction: ParameterDirection.Input);
                parametros.Add(name: "DataNascimento", value: contato.DataNascimento, direction: ParameterDirection.Input);
                parametros.Add(name: "Status", value: contato.Status, direction: ParameterDirection.Input);
                parametros.Add(name: "ContaId", value: contato.ContaId, direction: ParameterDirection.Input);

                parametros.Add(name: "Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                con.Execute(@"INSERT INTO CRM.TB_CRM_CONTATOS 
                                  ( 
                                     Id, 
                                     Nome, 
                                     Sobrenome, 
                                     Telefone,                 
                                     Celular, 
                                     Email, 
                                     Cargo, 
                                     Departamento, 
                                     DataNascimento,                                                 
                                     Status,
                                     ContaId
                                  ) VALUES ( 
                                     CRM.SEQ_CRM_CONTATOS.NEXTVAL, 
                                     :Nome, 
                                     :Sobrenome, 
                                     :Telefone, 
                                     :Celular, 
                                     :Email, 
                                     :Cargo, 
                                     :Departamento, 
                                     :DataNascimento,                 
                                     :Status,
                                     :ContaId
                                  ) RETURNING Id INTO :Id", parametros);

                return parametros.Get<int>("Id");
            }
        }

        public void Atualizar(Contato contato)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "Nome", value: contato.Nome, direction: ParameterDirection.Input);
                parametros.Add(name: "Sobrenome", value: contato.Sobrenome, direction: ParameterDirection.Input);
                parametros.Add(name: "Telefone", value: contato.Telefone, direction: ParameterDirection.Input);
                parametros.Add(name: "Celular", value: contato.Celular, direction: ParameterDirection.Input);
                parametros.Add(name: "Email", value: contato.Email, direction: ParameterDirection.Input);
                parametros.Add(name: "Cargo", value: contato.Cargo, direction: ParameterDirection.Input);
                parametros.Add(name: "Departamento", value: contato.Departamento, direction: ParameterDirection.Input);
                parametros.Add(name: "DataNascimento", value: contato.DataNascimento, direction: ParameterDirection.Input);
                parametros.Add(name: "Status", value: contato.Status, direction: ParameterDirection.Input);
                parametros.Add(name: "ContaId", value: contato.ContaId, direction: ParameterDirection.Input);
                parametros.Add(name: "Id", value: contato.Id, direction: ParameterDirection.Input);

                con.Execute(@"UPDATE CRM.TB_CRM_CONTATOS 
                                SET                            
                                    Nome = :Nome, 
                                    Sobrenome = :Sobrenome, 
                                    Telefone = :Telefone,                 
                                    Celular = :Celular, 
                                    Email = :Email, 
                                    Cargo = :Cargo, 
                                    Departamento = :Departamento, 
                                    DataNascimento = :DataNascimento,                                                 
                                    Status = :Status,
                                    ContaId = :ContaId
                            WHERE Id = :Id", parametros);
            }
        }

        public void Excluir(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                con.Execute(@"DELETE FROM CRM.TB_CRM_CONTATOS WHERE Id = :id", new { id });
            }
        }

        public IEnumerable<Contato> ObterTodosContatos()
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<Contato>(@"SELECT Id, Nome, SobreNome FROM CRM.TB_CRM_CONTATOS ORDER BY Nome");
            }
        }

        public IEnumerable<Contato> ObterContatosPorConta(int contaId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<Contato>(@"SELECT Id, Nome, SobreNome, Telefone, Celular, Email, Cargo, Departamento, DataNascimento, Status, ContaId FROM CRM.TB_CRM_CONTATOS WHERE ContaId = :contaId", new { contaId });
            }
        }

        public IEnumerable<Contato> ObterContatosPorDescricao(string descricao, int? usuarioId)
        {
            var criterio = descricao.ToUpper() + "%";

            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Criterio", value: criterio, direction: ParameterDirection.Input);

                var filtroSQL = string.Empty;

                if (usuarioId.HasValue)
                {
                    parametros.Add(name: "UsuarioId", value: usuarioId, direction: ParameterDirection.Input);
                    filtroSQL = " AND ContaId IN (SELECT ContaId FROM TB_CRM_USUARIOS_CONTAS WHERE UsuarioId = :UsuarioId) ";
                }                

                return con.Query<Contato>($@"
                    SELECT 
                        Id, 
                        Nome, 
                        SobreNome, 
                        Telefone, 
                        Celular, 
                        Email, 
                        Cargo, 
                        Departamento, 
                        DataNascimento, 
                        Status, 
                        ContaId 
                    FROM 
                        CRM.TB_CRM_CONTATOS 
                    WHERE 
                        (UPPER(NOME) || ' ' || UPPER(SOBRENOME) LIKE :Criterio) {filtroSQL}
                    AND 
                        ROWNUM < 300", parametros);
            }
        }

        public IEnumerable<ContatoDTO> ObterContatosEContaPorDescricao(string descricao, int? usuarioId)
        {
            var criterio = descricao.ToUpper() + "%";

            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Criterio", value: criterio, direction: ParameterDirection.Input);

                var filtroSQL = string.Empty;

                if (usuarioId.HasValue)
                {
                    parametros.Add(name: "UsuarioId", value: usuarioId, direction: ParameterDirection.Input);
                    filtroSQL = " AND A.ContaId IN (SELECT ContaId FROM TB_CRM_USUARIOS_CONTAS WHERE UsuarioId = :UsuarioId) ";
                }

                return con.Query<ContatoDTO>($@"
                    SELECT
                        A.Id,
                        A.Nome,
                        A.Sobrenome,
                        A.Telefone,
                        A.Celular,
                        A.Email,
                        A.Cargo,
                        A.Departamento,
                        A.Datanascimento,
                        A.Status,
                        A.ContaId,
                        B.Descricao as ContaDescricao,
                        B.Documento as ContaDocumento
                    FROM
                        CRM.TB_CRM_CONTATOS A
                    INNER JOIN
                        CRM.TB_CRM_CONTAS B ON A.ContaId = B.ID
                    WHERE
                        (UPPER(A.NOME) || ' ' || UPPER(A.SOBRENOME) LIKE :Criterio) {filtroSQL}
                    AND
                        ROWNUM < 200", parametros);
            }
        }

        public Contato ObterContatoPorId(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var result = con.Query<Contato, Conta, Contato>(@"
                    SELECT 
                        A.Id, 
                        A.Nome, 
                        A.SobreNome, 
                        A.Telefone, 
                        A.Celular, 
                        A.Email, 
                        A.Cargo, 
                        A.Departamento, 
                        TO_CHAR(A.DataNascimento, 'DD/MM/YYYY') DataNascimento, 
                        A.Status, 
                        A.ContaId,
                        B.Id,
                        B.Descricao
                    FROM 
                        CRM.TB_CRM_CONTATOS A
                    INNER JOIN
                        CRM.TB_CRM_CONTAS B ON A.ContaId = B.Id
                    WHERE 
                        A.Id = :id
                ", (c, ct) =>
                {                    
                    if (ct != null)
                        c.Conta = ct;

                    return c;

                }, new { id }, splitOn: "Id").FirstOrDefault();

                return result;
            }
        }        
    }
}
