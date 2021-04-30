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
    public class MercadoriaRepositorio : IMercadoriaRepositorio
    {
        public int Cadastrar(Mercadoria mercadoria)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "Descricao", value: mercadoria.Descricao, direction: ParameterDirection.Input);
                parametros.Add(name: "Status", value: mercadoria.Status, direction: ParameterDirection.Input);

                parametros.Add(name: "Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                con.Execute(@"INSERT INTO CRM.TB_CRM_MERCADORIAS (Id, Descricao, Status) VALUES (CRM.SEQ_CRM_MERCADORIAS.NEXTVAL, :Descricao, :Status) RETURNING Id INTO :Id", parametros);

                return parametros.Get<int>("Id");
            }
        }

        public void Atualizar(Mercadoria mercadoria)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "Descricao", value: mercadoria.Descricao, direction: ParameterDirection.Input);
                parametros.Add(name: "Status", value: mercadoria.Status, direction: ParameterDirection.Input);
                parametros.Add(name: "Id", value: mercadoria.Id, direction: ParameterDirection.Input);

                con.Execute(@"UPDATE CRM.TB_CRM_MERCADORIAS SET Descricao = :Descricao, Status = :Status WHERE Id = :Id", parametros);
            }
        }

        public IEnumerable<Mercadoria> ObterMercadorias()
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<Mercadoria>(@"SELECT * FROM CRM.TB_CRM_MERCADORIAS WHERE Status = 1 ORDER BY DESCRICAO");
            }
        }

        public Mercadoria ObterMercadoriaPorId(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<Mercadoria>(@"SELECT * FROM CRM.TB_CRM_MERCADORIAS WHERE Id = :mId", new { mId = id }).FirstOrDefault();
            }
        }

        public void Excluir(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                con.Execute(@"DELETE FROM CRM.TB_CRM_MERCADORIAS WHERE Id = :mId", new { mId = id });
            }
        }

        public IEnumerable<Mercadoria> ObterMercadoriaPorDescricao(string descricao)
        {
            var criterio = "%" + descricao.ToUpper() + "%";

            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<Mercadoria>($@"SELECT * FROM CRM.TB_CRM_MERCADORIAS WHERE Status = 1 AND UPPER(Descricao) LIKE :criterio AND ROWNUM < 100", new { criterio });
            }
        }
    }
}
