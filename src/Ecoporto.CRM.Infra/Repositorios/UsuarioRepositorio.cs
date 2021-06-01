using Dapper;
using Ecoporto.CRM.Business.DTO;
using Ecoporto.CRM.Business.Extensions;
using Ecoporto.CRM.Business.Interfaces.Repositorios;
using Ecoporto.CRM.Business.Models;
using Ecoporto.CRM.Infra.Configuracao;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Ecoporto.CRM.Infra.Repositorios
{
    public class UsuarioRepositorio : IUsuarioRepositorio
    {
        public int Cadastrar(Usuario usuario)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "Login", value: usuario.Login, direction: ParameterDirection.Input);
                parametros.Add(name: "LoginExterno", value: usuario.LoginExterno, direction: ParameterDirection.Input);
                parametros.Add(name: "Senha", value: usuario.Senha, direction: ParameterDirection.Input);
                parametros.Add(name: "LoginWorkflow", value: usuario.LoginWorkflow, direction: ParameterDirection.Input);
                parametros.Add(name: "Nome", value: usuario.Nome, direction: ParameterDirection.Input);
                parametros.Add(name: "Email", value: usuario.Email, direction: ParameterDirection.Input);
                parametros.Add(name: "CPF", value: usuario.CPF, direction: ParameterDirection.Input);
                parametros.Add(name: "CargoId", value: usuario.CargoId, direction: ParameterDirection.Input);
                parametros.Add(name: "Administrador", value: usuario.Administrador.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "Externo", value: usuario.Externo.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "Remoto", value: usuario.Remoto.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "Ativo", value: usuario.Ativo.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "ValidarIP", value: usuario.ValidarIP.ToInt(), direction: ParameterDirection.Input);

                parametros.Add(name: "Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                con.Execute(@"INSERT INTO CRM.TB_CRM_USUARIOS (Id, Login, LoginExterno, Senha, LoginWorkflow, Nome, Email, CPF, CargoId, Administrador, Externo, Remoto, Ativo, ValidarIP) VALUES (CRM.SEQ_CRM_USUARIOS.NEXTVAL, :Login, :LoginExterno, :Senha, :LoginWorkflow, :Nome, :Email, :CPF, :CargoId, :Administrador, :Externo, :Remoto, :Ativo, :ValidarIP) RETURNING Id INTO :Id", parametros);

                return parametros.Get<int>("Id");
            }
        }

        public void Atualizar(Usuario usuario)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                
                parametros.Add(name: "Nome", value: usuario.Nome, direction: ParameterDirection.Input);
                parametros.Add(name: "LoginExterno", value: usuario.LoginExterno, direction: ParameterDirection.Input);
                parametros.Add(name: "Senha", value: usuario.Senha, direction: ParameterDirection.Input);
                parametros.Add(name: "Email", value: usuario.Email, direction: ParameterDirection.Input);
                parametros.Add(name: "CPF", value: usuario.CPF, direction: ParameterDirection.Input);
                parametros.Add(name: "Ativo", value: usuario.Ativo.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "Administrador", value: usuario.Administrador.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "Externo", value: usuario.Externo.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "Remoto", value: usuario.Remoto.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "CargoId", value: usuario.CargoId, direction: ParameterDirection.Input);
                parametros.Add(name: "LoginWorkflow", value: usuario.LoginWorkflow, direction: ParameterDirection.Input);
                parametros.Add(name: "ValidarIP", value: usuario.ValidarIP.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "Id", value: usuario.Id, direction: ParameterDirection.Input);

                con.Execute(@"UPDATE CRM.TB_CRM_USUARIOS SET Nome = :Nome, LoginExterno = :LoginExterno, Senha = :Senha, Email =:Email, CPF = :CPF, Administrador = :Administrador, Externo = :Externo, Remoto = :Remoto, Ativo = :Ativo, CargoId = :CargoId, LoginWorkflow = :LoginWorkflow, ValidarIP = :ValidarIP WHERE Id = :Id", parametros);
            }
        }

        public IEnumerable<UsuarioDTO> ObterUsuarios()
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<UsuarioDTO>(@"
                    SELECT
                        A.Id,
                        A.Login,
                        A.LoginExterno,
                        A.Nome,
                        A.Email,
                        A.Administrador,
                        A.Externo,
                        A.Remoto,
                        A.Ativo,
                        A.ValidarIP,
                        B.Descricao As DescricaoCargo
                    FROM
                        CRM.TB_CRM_USUARIOS A
                    LEFT JOIN
                        CRM.TB_CRM_USUARIOS_CARGOS B ON A.CargoId = B.Id
                    ORDER BY
                        A.Login");
            }
        }

        public Usuario ObterUsuarioPorId(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<Usuario>(@"
                    SELECT 
                        A.Id,
                        A.Login,
                        A.LoginExterno,
                        A.LoginWorkflow,
                        A.Senha,
                        A.Nome,
                        A.Email,
                        A.CPF,
                        A.CargoId,
                        A.Administrador,
                        A.Externo,
                        A.Remoto,
                        A.Ativo,
                        A.Dominio,
                        A.ValidarIP,
                        B.Vendedor    
                    FROM 
                        CRM.TB_CRM_USUARIOS A
                    INNER JOIN
                        CRM.TB_CRM_USUARIOS_CARGOS B ON A.CargoId = B.Id
                    WHERE
                        A.Id = :id", new { id }).FirstOrDefault();
            }
        }

        public void Excluir(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                con.Execute(@"DELETE FROM CRM.TB_CRM_USUARIOS WHERE Id = :id", new { id });
            }
        }

        public Usuario ObterUsuarioPorLogin(string login)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Login", value: login.ToUpper(), direction: ParameterDirection.Input);

                return con.Query<Usuario>($@"
                    SELECT    
                        A.Id,
                        A.Login,
                        A.LoginExterno,                        
                        A.LoginWorkflow,
                        A.Nome,
                        A.CPF,
                        A.Email, 
                        A.Externo,
                        A.Remoto,
                        A.Senha,
                        A.Administrador,
                        A.Ativo,
                        A.ValidarIP,
                        A.CargoId,
                        A.Dominio,
                        B.Vendedor
                    FROM
                        CRM.TB_CRM_USUARIOS A
                    INNER JOIN
                        CRM.TB_CRM_USUARIOS_CARGOS B ON A.CargoId = B.Id
                    WHERE
                        (UPPER(A.Login) = :Login OR UPPER(A.LoginExterno) = :Login)", parametros).FirstOrDefault();
            }
        }

        public Usuario ObterUsuarioPorCPF(string cpf)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "CPF", value: cpf, direction: ParameterDirection.Input);

                return con.Query<Usuario>($@"
                    SELECT    
                        A.Id,
                        A.Login,
                        A.LoginExterno,                        
                        A.LoginWorkflow,
                        A.Nome,
                        A.CPF,
                        A.Email, 
                        A.Externo,
                        A.Remoto,
                        A.Senha,
                        A.Administrador,
                        A.Ativo,
                        A.ValidarIP,
                        A.CargoId,
                        B.Vendedor
                    FROM
                        CRM.TB_CRM_USUARIOS A
                    INNER JOIN
                        CRM.TB_CRM_USUARIOS_CARGOS B ON A.CargoId = B.Id
                    WHERE
                        A.CPF = :CPF", parametros).FirstOrDefault();
            }
        }

        public void VincularConta(int contaId, int usuarioId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "ContaId", value: contaId, direction: ParameterDirection.Input);
                parametros.Add(name: "UsuarioId", value: usuarioId, direction: ParameterDirection.Input);

                con.Execute(@"INSERT INTO CRM.TB_CRM_USUARIOS_CONTAS (Id, UsuarioId, ContaId) VALUES (CRM.SEQ_CRM_USUARIOS_CONTAS.NEXTVAL, :UsuarioId, :ContaId)", parametros);
            }
        }

        public IEnumerable<UsuarioContaDTO> ObterVinculosContas(int usuarioId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "UsuarioId", value: usuarioId, direction: ParameterDirection.Input);

                return con.Query<UsuarioContaDTO>($@"SELECT A.Id, B.Descricao As ContaDescricao, B.Documento As ContaDocumento, A.ContaId FROM CRM.TB_CRM_USUARIOS_CONTAS A INNER JOIN CRM.TB_CRM_CONTAS B ON A.ContaId = B.Id WHERE A.UsuarioId = :UsuarioId ORDER BY B.Descricao", parametros);
            }
        }

        public UsuarioContaDTO ObterVinculoContaPorId(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Id", value: id, direction: ParameterDirection.Input);

                return con.Query<UsuarioContaDTO>($@"SELECT * FROM CRM.TB_CRM_USUARIOS_CONTAS WHERE Id = :Id", parametros).FirstOrDefault();
            }
        }

        public bool ExisteVinculoConta(int contaId, int usuarioId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "ContaId", value: contaId, direction: ParameterDirection.Input);
                parametros.Add(name: "UsuarioId", value: usuarioId, direction: ParameterDirection.Input);

                return con.Query<bool>($@"SELECT * FROM CRM.TB_CRM_USUARIOS_CONTAS WHERE ContaId = :ContaId AND UsuarioId = :UsuarioId", parametros).Any();
            }
        }

        public void ExcluirVinculoConta(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Id", value: id, direction: ParameterDirection.Input);

                con.Execute(@"DELETE FROM CRM.TB_CRM_USUARIOS_CONTAS WHERE Id = :Id", parametros);
            }
        }

        public void AlterarSenha(Usuario usuario)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                
                parametros.Add(name: "Senha", value: usuario.Senha, direction: ParameterDirection.Input);
                parametros.Add(name: "Id", value: usuario.Id, direction: ParameterDirection.Input);

                con.Execute(@"UPDATE CRM.TB_CRM_USUARIOS SET Senha = :Senha WHERE Id = :Id", parametros);
            }
        }

        public IEnumerable<UsuarioIntegracao> ObterUsuariosIntegracao()
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<UsuarioIntegracao>(@"SELECT UsuarioId, AcessoProducao FROM CRM.TB_CRM_USUARIOS_INTEGRACAO");
            }
        }
    }
}
