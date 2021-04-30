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
    public class ModeloSimuladorRepositorio : IModeloSimuladorRepositorio
    {
        public int Cadastrar(ModeloSimulador modeloSimulador)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                con.Open();

                var parametros = new DynamicParameters();

                parametros.Add(name: "Descricao", value: modeloSimulador.Descricao, direction: ParameterDirection.Input);
                parametros.Add(name: "Regime", value: modeloSimulador.Regime, direction: ParameterDirection.Input);
                parametros.Add(name: "Observacoes", value: modeloSimulador.Observacoes, direction: ParameterDirection.Input);
                
                parametros.Add(name: "Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                using (var transaction = con.BeginTransaction())
                {
                    con.Execute(@"INSERT INTO CRM.TB_CRM_SIMULADOR_MODELO (Id, Descricao, Regime, Observacoes) VALUES (CRM.SEQ_CRM_SIMULADOR_MODELO.NEXTVAL, :Descricao, :Regime, :Observacoes) RETURNING Id INTO :Id", parametros);

                    var modeloSimuladorId = parametros.Get<int>("Id");

                    foreach (var servico in modeloSimulador.ServicoIPA)
                    {
                        parametros = new DynamicParameters();

                        parametros.Add(name: "ModeloSimuladorId", value: modeloSimuladorId, direction: ParameterDirection.Input);
                        parametros.Add(name: "ServicoId", value: servico, direction: ParameterDirection.Input);

                        con.Execute(@"INSERT INTO CRM.TB_CRM_SIMULADOR_SERVICOS (Id, ModeloSimuladorId, ServicoId) VALUES (CRM.SEQ_CRM_SIMULADOR_SERVICOS.NEXTVAL, :ModeloSimuladorId, :ServicoId)", parametros);
                    }

                    transaction.Commit();

                    return modeloSimuladorId;
                }
            }
        }

        public void Atualizar(ModeloSimulador modeloSimulador)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                con.Open();

                var parametros = new DynamicParameters();

                parametros.Add(name: "Descricao", value: modeloSimulador.Descricao, direction: ParameterDirection.Input);
                parametros.Add(name: "Regime", value: modeloSimulador.Regime, direction: ParameterDirection.Input);
                parametros.Add(name: "Observacoes", value: modeloSimulador.Observacoes, direction: ParameterDirection.Input);
                
                parametros.Add(name: "Id", value: modeloSimulador.Id, direction: ParameterDirection.Input);

                using (var transaction = con.BeginTransaction())
                {
                    con.Execute(@"UPDATE CRM.TB_CRM_SIMULADOR_MODELO SET Descricao = :Descricao, Regime = :Regime, Observacoes = :Observacoes WHERE Id = :Id", parametros);

                    con.Execute(@"DELETE FROM CRM.TB_CRM_SIMULADOR_SERVICOS WHERE ModeloSimuladorId = :Id", parametros);

                    foreach (var servico in modeloSimulador.ServicoIPA)
                    {
                        parametros = new DynamicParameters();

                        parametros.Add(name: "ModeloSimuladorId", value: modeloSimulador.Id, direction: ParameterDirection.Input);
                        parametros.Add(name: "ServicoId", value: servico, direction: ParameterDirection.Input);

                        con.Execute(@"INSERT INTO CRM.TB_CRM_SIMULADOR_SERVICOS (Id, ModeloSimuladorId, ServicoId) VALUES (CRM.SEQ_CRM_SIMULADOR_SERVICOS.NEXTVAL, :ModeloSimuladorId, :ServicoId)", parametros);
                    }

                    transaction.Commit();
                }
            }
        }

        public void Excluir(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                con.Open();

                var parametros = new DynamicParameters();
                parametros.Add(name: "Id", value: id, direction: ParameterDirection.Input);

                using (var transaction = con.BeginTransaction())
                {
                    con.Execute(@"DELETE FROM CRM.TB_CRM_SIMULADOR_SERVICOS WHERE ModeloSimuladorId = :Id", parametros);
                    con.Execute(@"DELETE FROM CRM.TB_CRM_SIMULADOR_MODELO WHERE Id = :Id", parametros);

                    transaction.Commit();
                }
            }
        }

        public ModeloSimulador ObterModeloSimuladorPorId(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Id", value: id, direction: ParameterDirection.Input);

                var modelo = con.Query<ModeloSimulador>(@"SELECT Id, Descricao, Regime, Observacoes FROM TB_CRM_SIMULADOR_MODELO WHERE Id = :Id", parametros).FirstOrDefault();

                if (modelo != null)
                {
                    var servicosVinculados = con.Query<int>(@"SELECT ServicoId FROM CRM.TB_CRM_SIMULADOR_SERVICOS WHERE ModeloSimuladorId = :Id", parametros);

                    foreach (var vinculo in servicosVinculados)
                    {
                        modelo.ServicoVinculados.Add(vinculo);
                    }
                }

                return modelo;
            }
        }

        public IEnumerable<ModeloSimulador> ObterModelosSimulador()
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<ModeloSimulador>(@"SELECT Id, Descricao, Regime, Observacoes FROM TB_CRM_SIMULADOR_MODELO ORDER BY Id");
            }
        }

        public VinculoModeloSimuladoDTO ObterVinculoSimuladorPorId(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Id", value: id, direction: ParameterDirection.Input);

                return con.Query<VinculoModeloSimuladoDTO>(@"SELECT Id, ModeloId, ModeloSimuladorId FROM TB_CRM_MODELO_SIMULADOR WHERE Id = :Id", parametros).FirstOrDefault();
            }
        }
    }
}
