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
    public class ServicoRepositorio : IServicoRepositorio
    {
        public int Cadastrar(Servico servico)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                con.Open();

                using (var transaction = con.BeginTransaction())
                {
                    var parametros = new DynamicParameters();

                    parametros.Add(name: "Descricao", value: servico.Descricao, direction: ParameterDirection.Input);
                    parametros.Add(name: "Status", value: servico.Status, direction: ParameterDirection.Input);
                    parametros.Add(name: "RecintoAlfandegado", value: servico.RecintoAlfandegado.ToInt(), direction: ParameterDirection.Input);
                    parametros.Add(name: "Operador", value: servico.Operador.ToInt(), direction: ParameterDirection.Input);
                    parametros.Add(name: "Redex", value: servico.Redex.ToInt(), direction: ParameterDirection.Input);

                    parametros.Add(name: "Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    con.Execute(@"INSERT INTO CRM.TB_CRM_SERVICOS (Id, Descricao, Status, RecintoAlfandegado, Operador, Redex) VALUES 
                                    (CRM.SEQ_CRM_SERVICOS.NEXTVAL,:Descricao, :Status, :RecintoAlfandegado, :Operador, :Redex) RETURNING Id INTO :Id", parametros);

                    var id = parametros.Get<int>("Id");

                    foreach (var servicoVinculado in servico.ServicosVinculados)
                    {
                        parametros = new DynamicParameters();

                        parametros.Add(name: "ServicoId", value: id, direction: ParameterDirection.Input);
                        parametros.Add(name: "ServicoFaturamentoId", value: servicoVinculado.Id, direction: ParameterDirection.Input);

                        con.Execute(@"INSERT INTO CRM.TB_CRM_SERVICO_IPA (Id, ServicoId, ServicoFaturamentoId) VALUES (CRM.SEQ_CRM_SERVICO_IPA.NEXTVAL, :ServicoId, :ServicoFaturamentoId)", parametros);
                    }
                    
                    transaction.Commit();

                    return id;
                }
            }
        }

        public void Atualizar(Servico servico)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                con.Open();

                using (var transaction = con.BeginTransaction())
                {
                    var parametros = new DynamicParameters();

                    parametros.Add(name: "Descricao", value: servico.Descricao, direction: ParameterDirection.Input);
                    parametros.Add(name: "Status", value: servico.Status, direction: ParameterDirection.Input);
                    parametros.Add(name: "RecintoAlfandegado", value: servico.RecintoAlfandegado.ToInt(), direction: ParameterDirection.Input);
                    parametros.Add(name: "Operador", value: servico.Operador.ToInt(), direction: ParameterDirection.Input);
                    parametros.Add(name: "Redex", value: servico.Redex.ToInt(), direction: ParameterDirection.Input);
                    parametros.Add(name: "Id", value: servico.Id, direction: ParameterDirection.Input);

                    con.Execute(@"UPDATE CRM.TB_CRM_SERVICOS SET Descricao = :Descricao, Status = :Status, RecintoAlfandegado = :RecintoAlfandegado, Operador = :Operador, Redex = :Redex WHERE Id = :Id", parametros);

                    con.Execute(@"DELETE FROM CRM.TB_CRM_SERVICO_IPA WHERE ServicoId = :sId", new { sId = servico.Id });

                    foreach (var servicoVinculado in servico.ServicosVinculados)
                    {
                        parametros = new DynamicParameters();

                        parametros.Add(name: "ServicoId", value: servico.Id, direction: ParameterDirection.Input);
                        parametros.Add(name: "ServicoFaturamentoId", value: servicoVinculado.Id, direction: ParameterDirection.Input);

                        con.Execute(@"INSERT INTO CRM.TB_CRM_SERVICO_IPA (Id, ServicoId, ServicoFaturamentoId) VALUES (CRM.SEQ_CRM_SERVICO_IPA.NEXTVAL, :ServicoId, :ServicoFaturamentoId)", parametros);
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

                using (var transaction = con.BeginTransaction())
                {
                    con.Execute(@"DELETE FROM CRM.TB_CRM_SERVICO_IPA WHERE ServicoId = :sId", new { sId = id });
                    con.Execute(@"DELETE FROM CRM.TB_CRM_SERVICOS WHERE Id = :sId", new { sId = id });

                    transaction.Commit();
                }                
            }
        }

        public IEnumerable<Servico> ObterServicos()
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<Servico>(@"SELECT * FROM CRM.TB_CRM_SERVICOS ORDER BY Descricao");
            }
        }

        public Servico ObterServicoPorId(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var servico = con.Query<Servico>(@"
                        SELECT Id, Descricao, Status, RecintoAlfandegado, Operador, Redex FROM CRM.TB_CRM_SERVICOS WHERE Id = :id", new { id }).FirstOrDefault();

                if (servico != null)
                {
                    var servicosVinculados = ObterServicosVinculados(servico.Id);

                    foreach (var servicoVinculado in servicosVinculados)
                        servico.AdicionarServicoVinculado(servicoVinculado);
                }

                return servico;
            }
        }

        public IEnumerable<ServicoFaturamento> ObterServicosVinculados(int servicoId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<ServicoFaturamento>(@"SELECT B.AUTONUM AS ID, B.DESCR AS DESCRICAO FROM CRM.TB_CRM_SERVICO_IPA A INNER JOIN SGIPA.TB_SERVICOS_IPA B ON A.ServicoFaturamentoId = B.AUTONUM WHERE A.ServicoId = :sId ORDER BY B.DESCR", new { sId = servicoId });
            }
        }
   
        public Servico ObterServicoPorDescricao(string descricao, int? id = 0)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var SQL = id == null
                    ? "SELECT * FROM CRM.TB_CRM_SERVICOS WHERE Descricao = :descricao"
                    : "SELECT * FROM CRM.TB_CRM_SERVICOS WHERE Descricao = :descricao AND Id <> :id";

                return con.Query<Servico>(SQL, new { descricao, id }).FirstOrDefault();
            }
        }

        public IEnumerable<Servico> ObterServicosPorDescricao(string descricao)
        {
            var criterio = descricao.ToUpper() + "%";

            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<Servico>("SELECT * FROM CRM.TB_CRM_SERVICOS WHERE UPPER(Descricao) LIKE :criterio AND ROWNUM < 300", new { criterio });
            }
        }
    }
}