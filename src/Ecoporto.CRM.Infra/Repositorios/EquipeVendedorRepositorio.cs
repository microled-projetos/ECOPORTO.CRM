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
    public class EquipeVendedorRepositorio : IEquipeVendedorRepositorio
    {
        public IEnumerable<EquipeVendedorUsuarioDTO> ObterUsuariosVinculados(int vendedorId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "VendedorId", value: vendedorId, direction: ParameterDirection.Input);

                return con.Query<EquipeVendedorUsuarioDTO>(@"SELECT A.Id, B.Login, B.Nome, A.AcessoConta, A.AcessoOportunidade, A.PapelEquipe FROM CRM.TB_CRM_EQUIPES_VENDEDOR A INNER JOIN CRM.TB_CRM_USUARIOS B ON A.UsuarioId = B.Id WHERE A.VendedorId = :VendedorId ORDER BY B.Nome", parametros);
            }
        }

        public void Vincular(EquipeVendedor equipe)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "VendedorId", value: equipe.VendedorId, direction: ParameterDirection.Input);
                parametros.Add(name: "UsuarioId", value: equipe.UsuarioId, direction: ParameterDirection.Input);
                parametros.Add(name: "AcessoConta", value: equipe.AcessoConta, direction: ParameterDirection.Input);
                parametros.Add(name: "AcessoOportunidade", value: equipe.AcessoOportunidade, direction: ParameterDirection.Input);
                parametros.Add(name: "PapelEquipe", value: equipe.PapelEquipe, direction: ParameterDirection.Input);

                con.Execute(@"INSERT INTO CRM.TB_CRM_EQUIPES_VENDEDOR (Id, VendedorId, UsuarioId, AcessoConta, AcessoOportunidade, PapelEquipe) VALUES (CRM.SEQ_CRM_EQUIPES_VENDEDOR.NEXTVAL, :VendedorId, :UsuarioId, :AcessoConta, :AcessoOportunidade, :PapelEquipe)", parametros);
            }
        }

        public void Atualizar(EquipeVendedor equipe)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "AcessoConta", value: equipe.AcessoConta, direction: ParameterDirection.Input);
                parametros.Add(name: "AcessoOportunidade", value: equipe.AcessoOportunidade, direction: ParameterDirection.Input);
                parametros.Add(name: "PapelEquipe", value: equipe.PapelEquipe, direction: ParameterDirection.Input);
                parametros.Add(name: "Id", value: equipe.Id, direction: ParameterDirection.Input);

                con.Execute(@"UPDATE CRM.TB_CRM_EQUIPES_VENDEDOR SET AcessoConta = :AcessoConta, AcessoOportunidade = :AcessoOportunidade, PapelEquipe = :PapelEquipe WHERE Id = :Id", parametros);
            }
        }

        public EquipeVendedor ObterVinculoPorId(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Id", value: id, direction: ParameterDirection.Input);

                return con.Query<EquipeVendedor>(@"
                        SELECT
                            A.Id,
                            A.VendedorId,
                            A.UsuarioId,
                            B.Nome || ' (' || B.Login || ')'  As UsuarioDescricao,
                            A.AcessoConta,
                            A.AcessoOportunidade,
                            A.PapelEquipe
                        FROM
                            CRM.TB_CRM_EQUIPES_VENDEDOR A
                        INNER JOIN
                            CRM.TB_CRM_USUARIOS B ON A.UsuarioId = B.Id
                        WHERE
                            A.Id = :Id", parametros).FirstOrDefault();
            }
        }

        public bool VinculoJaExistente(EquipeVendedor equipe)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "VendedorId", value: equipe.VendedorId, direction: ParameterDirection.Input);
                parametros.Add(name: "UsuarioId", value: equipe.UsuarioId, direction: ParameterDirection.Input);

                return con.Query<EquipeVendedor>(@"SELECT * FROM CRM.TB_CRM_EQUIPES_VENDEDOR WHERE VendedorId = :VendedorId AND UsuarioId = :UsuarioId", parametros).Any();
            }
        }

        public void Excluir(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                con.Execute(@"DELETE FROM CRM.TB_CRM_EQUIPES_VENDEDOR WHERE Id = :mId", new { mId = id });
            }
        }
    }
}
