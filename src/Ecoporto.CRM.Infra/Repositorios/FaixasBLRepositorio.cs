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
    public class FaixasBLRepositorio : IFaixasBLRepositorio
    {
        public void Cadastrar(FaixaBL faixa)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "LayoutId", value: faixa.LayoutId, direction: ParameterDirection.Input);
                parametros.Add(name: "BLMinimo", value: faixa.BLMinimo, direction: ParameterDirection.Input);
                parametros.Add(name: "BLMaximo", value: faixa.BLMaximo, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorMinimo", value: faixa.ValorMinimo, direction: ParameterDirection.Input);

                con.Execute(@"INSERT INTO CRM.TB_CRM_LAYOUT_VL_MINIMO_BL (Id, LayoutId, BLMinimo, BLMaximo, ValorMinimo) VALUES (CRM.SEQ_CRM_LAYOUT_VL_MINIMO_BL.NEXTVAL, :LayoutId, :BLMinimo, :BLMaximo, :ValorMinimo)", parametros);
            }
        }

        public void Atualizar(FaixaBL faixa)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "BLMinimo", value: faixa.BLMinimo, direction: ParameterDirection.Input);
                parametros.Add(name: "BLMaximo", value: faixa.BLMaximo, direction: ParameterDirection.Input);
                parametros.Add(name: "ValorMinimo", value: faixa.ValorMinimo, direction: ParameterDirection.Input);
                parametros.Add(name: "Id", value: faixa.Id, direction: ParameterDirection.Input);

                con.Execute(@"UPDATE CRM.TB_CRM_LAYOUT_VL_MINIMO_BL SET BLMinimo = :BLMinimo, BLMaximo = :BLMaximo, ValorMinimo = :ValorMinimo WHERE Id = :Id", parametros);
            }
        }

        public void Excluir(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                con.Execute(@"DELETE FROM CRM.TB_CRM_LAYOUT_VL_MINIMO_BL WHERE Id = :id", new { id });
            }
        }

        public IEnumerable<FaixaBL> ObterFaixasBL(int layoutId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<FaixaBL>(@"SELECT Id, LayoutId, BLMinimo, BLMaximo, ValorMinimo FROM CRM.TB_CRM_LAYOUT_VL_MINIMO_BL WHERE LayoutId = :lId ORDER BY Id", new { lId = layoutId });
            }
        }

        public FaixaBL ObterPorId(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<FaixaBL>(@"SELECT * FROM CRM.TB_CRM_LAYOUT_VL_MINIMO_BL WHERE Id = :id", new { id }).FirstOrDefault();
            }
        }

        public void CadastrarFaixasPeso(FaixaPeso faixa)
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

        public void ExcluirFaixaPeso(int id)
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

        public FaixaPeso ObterFaixaPesoPorId(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<FaixaPeso>(@"SELECT * FROM CRM.TB_CRM_LAYOUT_VL_PESO WHERE Id = :id", new { id }).FirstOrDefault();
            }
        }
    }
}
