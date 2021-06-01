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
    public class FaixasCIFRepositorio : IFaixasCIFRepositorio
    {
        public void Cadastrar(FaixaCIF faixa)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "LayoutId", value: faixa.LayoutId, direction: ParameterDirection.Input);
                parametros.Add(name: "Minimo", value: faixa.Minimo, direction: ParameterDirection.Input);
                parametros.Add(name: "Maximo", value: faixa.Maximo, direction: ParameterDirection.Input);
                parametros.Add(name: "Margem", value: faixa.Margem, direction: ParameterDirection.Input);
                parametros.Add(name: "Valor20", value: faixa.Valor20, direction: ParameterDirection.Input);
                parametros.Add(name: "Valor40", value: faixa.Valor40, direction: ParameterDirection.Input);
                parametros.Add(name: "Descricao", value: faixa.Descricao, direction: ParameterDirection.Input);

                con.Execute(@"INSERT INTO CRM.TB_CRM_LAYOUT_VL_CIF_BL (Id, LayoutId, Minimo, Maximo, Margem, Valor20, Valor40, Descricao) VALUES (CRM.SEQ_CRM_LAYOUT_VL_CIF_BL.NEXTVAL, :LayoutId, :Minimo, :Maximo, :Margem, :Valor20, :Valor40, :Descricao)", parametros);
            }
        }

        public void Atualizar(FaixaCIF faixa)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "Minimo", value: faixa.Minimo, direction: ParameterDirection.Input);
                parametros.Add(name: "Maximo", value: faixa.Maximo, direction: ParameterDirection.Input);
                parametros.Add(name: "Margem", value: faixa.Margem, direction: ParameterDirection.Input);
                parametros.Add(name: "Valor20", value: faixa.Valor20, direction: ParameterDirection.Input);
                parametros.Add(name: "Valor40", value: faixa.Valor40, direction: ParameterDirection.Input);
                parametros.Add(name: "Descricao", value: faixa.Descricao, direction: ParameterDirection.Input);
                parametros.Add(name: "Id", value: faixa.Id, direction: ParameterDirection.Input);

                con.Execute(@"UPDATE CRM.TB_CRM_LAYOUT_VL_CIF_BL SET Minimo = :Minimo, Maximo = :Maximo, Margem = :Margem, Valor20 = :Valor20, Valor40 = :Valor40, Descricao = :Descricao WHERE Id = :Id", parametros);
            }
        }

        public void Excluir(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                con.Execute(@"DELETE FROM CRM.TB_CRM_LAYOUT_VL_CIF_BL WHERE Id = :id", new { id });
            }
        }

        public IEnumerable<FaixaCIF> ObterFaixasCIF(int layoutId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<FaixaCIF>(@"SELECT Id, LayoutId, Minimo, Maximo, Margem, Valor20, Valor40, Descricao FROM CRM.TB_CRM_LAYOUT_VL_CIF_BL WHERE LayoutId = :lId ORDER BY Id", new { lId = layoutId });
            }
        }

        public FaixaCIF ObterPorId(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<FaixaCIF>(@"SELECT * FROM CRM.TB_CRM_LAYOUT_VL_CIF_BL WHERE Id = :id", new { id }).FirstOrDefault();
            }
        }
    }
}
