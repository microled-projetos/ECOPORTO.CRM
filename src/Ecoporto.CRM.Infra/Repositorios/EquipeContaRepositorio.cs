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
    public class EquipeContaRepositorio : IEquipeContaRepositorio
    {
        public IEnumerable<EquipeContaUsuarioDTO> ObterUsuariosVinculados(int contaId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "ContaId", value: contaId, direction: ParameterDirection.Input);

                return con.Query<EquipeContaUsuarioDTO>(@"SELECT A.Id, B.Login, B.Nome, A.AcessoConta, A.AcessoOportunidade, A.PapelEquipe FROM CRM.TB_CRM_EQUIPES_CONTA A INNER JOIN CRM.TB_CRM_USUARIOS B ON A.UsuarioId = B.Id WHERE A.ContaId = :ContaId ORDER BY B.Nome", parametros);
            }
        }

        public void Vincular(EquipeConta equipe)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "ContaId", value: equipe.ContaId, direction: ParameterDirection.Input);
                parametros.Add(name: "UsuarioId", value: equipe.UsuarioId, direction: ParameterDirection.Input);
                parametros.Add(name: "AcessoConta", value: equipe.AcessoConta, direction: ParameterDirection.Input);
                parametros.Add(name: "AcessoOportunidade", value: equipe.AcessoOportunidade, direction: ParameterDirection.Input);
                parametros.Add(name: "PapelEquipe", value: equipe.PapelEquipe, direction: ParameterDirection.Input);

                con.Execute(@"INSERT INTO CRM.TB_CRM_EQUIPES_CONTA (Id, ContaId, UsuarioId, AcessoConta, AcessoOportunidade, PapelEquipe) VALUES (CRM.SEQ_CRM_EQUIPES_CONTA.NEXTVAL, :ContaId, :UsuarioId, :AcessoConta, :AcessoOportunidade, :PapelEquipe)", parametros);
            }
        }

        public void Atualizar(EquipeConta equipe)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "AcessoConta", value: equipe.AcessoConta, direction: ParameterDirection.Input);
                parametros.Add(name: "AcessoOportunidade", value: equipe.AcessoOportunidade, direction: ParameterDirection.Input);
                parametros.Add(name: "PapelEquipe", value: equipe.PapelEquipe, direction: ParameterDirection.Input);
                parametros.Add(name: "Id", value: equipe.Id, direction: ParameterDirection.Input);

                con.Execute(@"UPDATE CRM.TB_CRM_EQUIPES_CONTA SET AcessoConta = :AcessoConta, AcessoOportunidade = :AcessoOportunidade, PapelEquipe = :PapelEquipe WHERE Id = :Id", parametros);
            }
        }

        public EquipeConta ObterVinculoPorId(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Id", value: id, direction: ParameterDirection.Input);

                return con.Query<EquipeConta>(@"
                        SELECT
                            A.Id,
                            A.ContaId,
                            A.UsuarioId,
                            B.Nome || ' (' || B.Login || ')'  As UsuarioDescricao,
                            A.AcessoConta,
                            A.AcessoOportunidade,
                            A.PapelEquipe
                        FROM
                            CRM.TB_CRM_EQUIPES_CONTA A
                        INNER JOIN
                            CRM.TB_CRM_USUARIOS B ON A.UsuarioId = B.Id
                        WHERE
                            A.Id = :Id", parametros).FirstOrDefault();
            }
        }

        public bool VinculoJaExistente(EquipeConta equipe)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "ContaId", value: equipe.ContaId, direction: ParameterDirection.Input);
                parametros.Add(name: "UsuarioId", value: equipe.UsuarioId, direction: ParameterDirection.Input);

                return con.Query<EquipeConta>(@"SELECT * FROM CRM.TB_CRM_EQUIPES_CONTA WHERE ContaId = :ContaId AND UsuarioId = :UsuarioId", parametros).Any();
            }
        }

        public void Excluir(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                con.Execute(@"DELETE FROM CRM.TB_CRM_EQUIPES_CONTA WHERE Id = :mId", new { mId = id });
            }
        }

        public EquipeConta ObterPermissoesContaPorVendedor(int contaId, string login)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "ContaId", value: contaId, direction: ParameterDirection.Input);
                parametros.Add(name: "Login", value: login, direction: ParameterDirection.Input);

                return con.Query<EquipeConta>(@"
                    SELECT
                        B.Id,
                        A.Id As ContaId,
                        B.UsuarioId,
                        B.AcessoConta, 
                        B.AcessoOportunidade 
                    FROM 
                        CRM.TB_CRM_CONTAS A 
                    INNER JOIN 
                        CRM.TB_CRM_EQUIPES_VENDEDOR B ON A.VendedorId = B.VendedorId 
                    INNER JOIN 
                        CRM.TB_CRM_USUARIOS C ON B.UsuarioId = C.Id 
                    WHERE 
                        A.Id = :ContaId 
                    AND 
                        C.Login = :Login", parametros).FirstOrDefault();
            }
        }

        public EquipeConta ObterPermissoesContaPorConta(int contaId, string login)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "ContaId", value: contaId, direction: ParameterDirection.Input);
                parametros.Add(name: "Login", value: login, direction: ParameterDirection.Input);

                return con.Query<EquipeConta>(@"
                    SELECT
                        B.Id,
                        A.Id As ContaId,
                        B.UsuarioId,
                        B.AcessoConta, 
                        B.AcessoOportunidade 
                    FROM 
                        CRM.TB_CRM_CONTAS A 
                    INNER JOIN 
                        CRM.TB_CRM_EQUIPES_CONTA B ON B.ContaId = A.Id
                    INNER JOIN 
                        CRM.TB_CRM_USUARIOS C ON B.UsuarioId = C.Id  
                    WHERE 
                        A.Id = :ContaId 
                    AND 
                        C.Login = :Login", parametros).FirstOrDefault();
            }
        }

        public EquipeConta ObterPermissoesOportunidadePorVendedor(int vendedorId, string login)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "VendedorId", value: vendedorId, direction: ParameterDirection.Input);
                parametros.Add(name: "Login", value: login, direction: ParameterDirection.Input);

                return con.Query<EquipeConta>(@"
                    SELECT
                        B.Id,
                        A.ContaId,
                        B.UsuarioId,
                        B.AcessoConta, 
                        B.AcessoOportunidade 
                    FROM 
                        CRM.TB_CRM_OPORTUNIDADES A
                    INNER JOIN 
                        CRM.TB_CRM_EQUIPES_VENDEDOR B ON A.VendedorId = B.VendedorId 
                    INNER JOIN
                        CRM.TB_CRM_CONTAS C ON A.ContaId = C.Id
                    INNER JOIN 
                        CRM.TB_CRM_USUARIOS D ON B.UsuarioId = D.Id 
                    WHERE 
                        A.VendedorId = :VendedorId
                    AND 
                        D.Login = :Login", parametros).FirstOrDefault();
            }
        }

        public EquipeOportunidade ObterPermissoesPorOportunidade(int oportunidadeId, string login)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);
                parametros.Add(name: "Login", value: login, direction: ParameterDirection.Input);

                return con.Query<EquipeOportunidade>(@"
                    SELECT
                        A.Id,
                        A.OportunidadeId,
                        A.UsuarioId,
                        A.AcessoConta, 
                        A.AcessoOportunidade 
                    FROM 
                        CRM.TB_CRM_EQUIPES_OPORTUNIDADE A                        
                    INNER JOIN 
                        CRM.TB_CRM_USUARIOS B ON A.UsuarioId = B.Id 
                    WHERE 
                        A.OportunidadeId = :OportunidadeId
                    AND 
                        B.Login = :Login", parametros).FirstOrDefault();
            }
        }
    }
}
