using Dapper;
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
    public class CargoRepositorio : ICargoRepositorio
    {
        public int Cadastrar(Cargo cargo)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "Descricao", value: cargo.Descricao, direction: ParameterDirection.Input);
                parametros.Add(name: "Vendedor", value: cargo.Vendedor.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                con.Execute(@"INSERT INTO CRM.TB_CRM_USUARIOS_CARGOS (Id, Descricao, Vendedor) VALUES (CRM.SEQ_CRM_USUARIOS_CARGOS.NEXTVAL, :Descricao, :Vendedor) RETURNING Id INTO :Id", parametros);

                return parametros.Get<int>("Id");
            }
        }

        public void Atualizar(Cargo cargo)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "Descricao", value: cargo.Descricao, direction: ParameterDirection.Input);
                parametros.Add(name: "Vendedor", value: cargo.Vendedor.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "Id", value: cargo.Id, direction: ParameterDirection.Input);

                con.Execute(@"UPDATE CRM.TB_CRM_USUARIOS_CARGOS SET Descricao = :Descricao, Vendedor = :Vendedor WHERE Id = :Id", parametros);
            }
        }

        public IEnumerable<Cargo> ObterCargos()
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<Cargo>(@"SELECT * FROM CRM.TB_CRM_USUARIOS_CARGOS ORDER BY DESCRICAO");
            }
        }

        public Cargo ObterCargoPorId(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<Cargo>(@"SELECT * FROM CRM.TB_CRM_USUARIOS_CARGOS WHERE Id = :mId", new { mId = id }).FirstOrDefault();
            }
        }

        public void Excluir(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                con.Open();

                var parametros = new DynamicParameters();
                parametros.Add(name: "CargoId", value: id, direction: ParameterDirection.Input);

                using (var transaction = con.BeginTransaction())
                {
                    try
                    {
                        con.Execute(@"DELETE FROM CRM.TB_CRM_USUARIOS_PERMISSOES WHERE CargoId = :CargoId", parametros);
                        con.Execute(@"DELETE FROM CRM.TB_CRM_USUARIOS_CARGOS WHERE Id = :CargoId", parametros);
                    }
                    catch
                    {
                        transaction.Rollback();
                    }
                    
                    transaction.Commit();
                }
            }            
        }

        public IEnumerable<Cargo> ObterCargoPorDescricao(string descricao)
        {
            var criterio = "%" + descricao.ToUpper() + "%";

            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<Cargo>($@"SELECT * FROM CRM.TB_CRM_USUARIOS_CARGOS WHERE UPPER(Descricao) LIKE :criterio", new { criterio });
            }
        }
    }
}
