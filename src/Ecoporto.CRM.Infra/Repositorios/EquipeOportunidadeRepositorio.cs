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
    public class EquipeOportunidadeRepositorio : IEquipeOportunidadeRepositorio
    {
        public IEnumerable<EquipeOportunidadeUsuarioDTO> ObterUsuariosVinculados(int oportunidadeId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);

                return con.Query<EquipeOportunidadeUsuarioDTO>(@"SELECT A.Id, B.Login, B.Nome, A.AcessoConta, A.AcessoOportunidade, A.PapelEquipe FROM CRM.TB_CRM_EQUIPES_OPORTUNIDADE A INNER JOIN CRM.TB_CRM_USUARIOS B ON A.UsuarioId = B.Id WHERE A.OportunidadeId = :OportunidadeId ORDER BY B.Nome", parametros);
            }
        }

        public void Vincular(EquipeOportunidade equipe)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "OportunidadeId", value: equipe.OportunidadeId, direction: ParameterDirection.Input);
                parametros.Add(name: "UsuarioId", value: equipe.UsuarioId, direction: ParameterDirection.Input);
                parametros.Add(name: "AcessoConta", value: equipe.AcessoConta, direction: ParameterDirection.Input);
                parametros.Add(name: "AcessoOportunidade", value: equipe.AcessoOportunidade, direction: ParameterDirection.Input);
                parametros.Add(name: "PapelEquipe", value: equipe.PapelEquipe, direction: ParameterDirection.Input);

                con.Execute(@"INSERT INTO CRM.TB_CRM_EQUIPES_OPORTUNIDADE (Id, OportunidadeId, UsuarioId, AcessoConta, AcessoOportunidade, PapelEquipe) VALUES (CRM.SEQ_CRM_EQUIPES_OPORTUNIDADE.NEXTVAL, :OportunidadeId, :UsuarioId, :AcessoConta, :AcessoOportunidade, :PapelEquipe)", parametros);
            }
        }

        public void Atualizar(EquipeOportunidade equipe)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "AcessoConta", value: equipe.AcessoConta, direction: ParameterDirection.Input);
                parametros.Add(name: "AcessoOportunidade", value: equipe.AcessoOportunidade, direction: ParameterDirection.Input);
                parametros.Add(name: "PapelEquipe", value: equipe.PapelEquipe, direction: ParameterDirection.Input);
                parametros.Add(name: "Id", value: equipe.Id, direction: ParameterDirection.Input);

                con.Execute(@"UPDATE CRM.TB_CRM_EQUIPES_OPORTUNIDADE SET AcessoConta = :AcessoConta, AcessoOportunidade = :AcessoOportunidade, PapelEquipe = :PapelEquipe WHERE Id = :Id", parametros);
            }
        }

        public EquipeOportunidade ObterVinculoPorId(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Id", value: id, direction: ParameterDirection.Input);

                return con.Query<EquipeOportunidade>(@"
                        SELECT
                            A.Id,
                            A.OportunidadeId,
                            A.UsuarioId,
                            B.Nome || ' (' || B.Login || ')'  As UsuarioDescricao,
                            A.AcessoConta,
                            A.AcessoOportunidade,
                            A.PapelEquipe
                        FROM
                            CRM.TB_CRM_EQUIPES_OPORTUNIDADE A
                        INNER JOIN
                            CRM.TB_CRM_USUARIOS B ON A.UsuarioId = B.Id
                        WHERE
                            A.Id = :Id", parametros).FirstOrDefault();
            }
        }

        public bool VinculoJaExistente(EquipeOportunidade equipe)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "OportunidadeId", value: equipe.OportunidadeId, direction: ParameterDirection.Input);
                parametros.Add(name: "UsuarioId", value: equipe.UsuarioId, direction: ParameterDirection.Input);

                return con.Query<EquipeOportunidade>(@"SELECT * FROM CRM.TB_CRM_EQUIPES_OPORTUNIDADE WHERE OportunidadeId = :OportunidadeId AND UsuarioId = :UsuarioId", parametros).Any();
            }
        }

        public void Excluir(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                con.Execute(@"DELETE FROM CRM.TB_CRM_EQUIPES_OPORTUNIDADE WHERE Id = :mId", new { mId = id });
            }
        }
    }
}
