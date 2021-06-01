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
    public class FaixasPesoRepositorio : IFaixasPesoRepositorio
    {
        public void Cadastrar(FaixaPeso faixa)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "LayoutId", value: faixa.LayoutId, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorInicial", value: faixa.ValorInicial, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorFinal", value: faixa.ValorFinal, direction: ParameterDirection.Input);
                parametros.Add(name: "Preco", value: faixa.Preco, direction: ParameterDirection.Input);

                con.Execute(@"INSERT INTO CRM.TB_CRM_LAYOUT_VL_PESO (Id, LayoutId, ValorInicial, ValorFinal, Preco) VALUES (CRM.SEQ_CRM_LAYOUT_VL_PESO.NEXTVAL, :LayoutId, :ValorInicial, :ValorFinal, :Preco)", parametros);
            }
        }

        public void Atualizar(FaixaPeso faixa)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "LayoutId", value: faixa.LayoutId, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorInicial", value: faixa.ValorInicial, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorFinal", value: faixa.ValorFinal, direction: ParameterDirection.Input);
                parametros.Add(name: "Preco", value: faixa.Preco, direction: ParameterDirection.Input);
                parametros.Add(name: "Id", value: faixa.Id, direction: ParameterDirection.Input);

                con.Execute(@"UPDATE CRM.TB_CRM_LAYOUT_VL_PESO SET ValorInicial = :ValorInicial, ValorFinal = :ValorFinal, Preco = :Preco WHERE Id = :Id", parametros);
            }
        }

        public void Excluir(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                con.Execute(@"DELETE FROM CRM.TB_CRM_LAYOUT_VL_PESO WHERE Id = :id", new { id });
            }
        }

        public IEnumerable<FaixaPeso> ObterFaixasPeso(int layoutId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<FaixaPeso>(@"SELECT Id, LayoutId, ValorInicial, ValorFinal, Preco FROM CRM.TB_CRM_LAYOUT_VL_PESO WHERE LayoutId = :lId ORDER BY Id", new { lId = layoutId });
            }
        }

        public FaixaPeso ObterPorId(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<FaixaPeso>(@"SELECT * FROM CRM.TB_CRM_LAYOUT_VL_PESO WHERE Id = :id", new { id }).FirstOrDefault();
            }
        }
    }
}
