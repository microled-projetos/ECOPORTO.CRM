using Dapper;
using Ecoporto.CRM.Business.DTO;
using Ecoporto.CRM.Business.Enums;
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
    public class ModeloRepositorio : IModeloRepositorio
    {
        public int Cadastrar(Modelo modelo)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "TipoOperacao", value: modelo.TipoOperacao, direction: ParameterDirection.Input);
                parametros.Add(name: "Descricao", value: modelo.Descricao, direction: ParameterDirection.Input);
                parametros.Add(name: "Status", value: modelo.Status, direction: ParameterDirection.Input);
                parametros.Add(name: "FormaPagamento", value: modelo.FormaPagamento, direction: ParameterDirection.Input);
                parametros.Add(name: "QtdeDias", value: modelo.QtdeDias, direction: ParameterDirection.Input);
                parametros.Add(name: "Validade", value: modelo.Validade, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoValidade", value: modelo.TipoValidade, direction: ParameterDirection.Input);
                parametros.Add(name: "DiasFreeTime", value: modelo.DiasFreeTime, direction: ParameterDirection.Input);
                parametros.Add(name: "VendedorId", value: modelo.VendedorId, direction: ParameterDirection.Input);
                parametros.Add(name: "ImpostoId", value: modelo.ImpostoId, direction: ParameterDirection.Input);
                parametros.Add(name: "Acordo", value: modelo.Acordo.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "Escalonado", value: modelo.Escalonado.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "ParametroBL", value: modelo.ParametroBL.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "ParametroConteiner", value: modelo.ParametroConteiner.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "ParametroLote", value: modelo.ParametroLote.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "ParametroIdTabela", value: modelo.ParametroIdTabela.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "HubPort", value: modelo.HubPort.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "CobrancaEspecial", value: modelo.CobrancaEspecial.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "DesovaParcial", value: modelo.DesovaParcial, direction: ParameterDirection.Input);
                parametros.Add(name: "FatorCP", value: modelo.FatorCP, direction: ParameterDirection.Input);
                parametros.Add(name: "PosicIsento", value: modelo.PosicIsento, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoServico", value: modelo.TipoServico, direction: ParameterDirection.Input);
                
                parametros.Add(name: "Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                con.Execute(@"INSERT INTO CRM.TB_CRM_MODELO 
                            ( 
                                Id, 
                                TipoOperacao, 
                                Descricao, 
                                Status,                 
                                FormaPagamento, 
                                QtdeDias, 
                                Validade, 
                                TipoValidade, 
                                DiasFreeTime,                                                 
                                VendedorId, 
                                ImpostoId,
                                Acordo,
                                Escalonado,
                                ParametroBL,
                                ParametroConteiner,
                                ParametroLote,
                                ParametroIdTabela,
                                DataCadastro,
                                HubPort,
                                CobrancaEspecial,
                                DesovaParcial,
                                FatorCP,
                                PosicIsento,
                                TipoServico
                            ) VALUES ( 
                                CRM.SEQ_CRM_MODELOS.NEXTVAL, 
                                :TipoOperacao, 
                                :Descricao, 
                                :Status, 
                                :FormaPagamento, 
                                :QtdeDias, 
                                :Validade, 
                                :TipoValidade, 
                                :DiasFreeTime,                 
                                :VendedorId, 
                                :ImpostoId,
                                :Acordo,
                                :Escalonado,
                                :ParametroBL,
                                :ParametroConteiner,
                                :ParametroLote,
                                :ParametroIdTabela,
                                SYSDATE,
                                :HubPort,
                                :CobrancaEspecial,
                                :DesovaParcial,
                                :FatorCP,
                                :PosicIsento,
                                :TipoServico
                            ) RETURNING Id INTO :Id", parametros);

                return parametros.Get<int>("Id");
            }
        }

        public void Atualizar(Modelo modelo)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "TipoOperacao", value: modelo.TipoOperacao, direction: ParameterDirection.Input);
                parametros.Add(name: "Descricao", value: modelo.Descricao, direction: ParameterDirection.Input);
                parametros.Add(name: "Status", value: modelo.Status, direction: ParameterDirection.Input);
                parametros.Add(name: "FormaPagamento", value: modelo.FormaPagamento, direction: ParameterDirection.Input);
                parametros.Add(name: "QtdeDias", value: modelo.QtdeDias, direction: ParameterDirection.Input);
                parametros.Add(name: "Validade", value: modelo.Validade, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoValidade", value: modelo.TipoValidade, direction: ParameterDirection.Input);
                parametros.Add(name: "DiasFreeTime", value: modelo.DiasFreeTime, direction: ParameterDirection.Input);
                parametros.Add(name: "VendedorId", value: modelo.VendedorId, direction: ParameterDirection.Input);
                parametros.Add(name: "ImpostoId", value: modelo.ImpostoId, direction: ParameterDirection.Input);
                parametros.Add(name: "DataInatividade", value: modelo.DataInatividade, direction: ParameterDirection.Input);
                parametros.Add(name: "Acordo", value: modelo.Acordo.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "Escalonado", value: modelo.Escalonado.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "ParametroBL", value: modelo.ParametroBL.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "ParametroConteiner", value: modelo.ParametroConteiner.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "ParametroLote", value: modelo.ParametroLote.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "ParametroIdTabela", value: modelo.ParametroIdTabela.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "HubPort", value: modelo.HubPort.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "CobrancaEspecial", value: modelo.CobrancaEspecial.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "DesovaParcial", value: modelo.DesovaParcial, direction: ParameterDirection.Input);
                parametros.Add(name: "FatorCP", value: modelo.FatorCP, direction: ParameterDirection.Input);
                parametros.Add(name: "PosicIsento", value: modelo.PosicIsento, direction: ParameterDirection.Input);
                parametros.Add(name: "TipoServico", value: modelo.TipoServico, direction: ParameterDirection.Input);
                parametros.Add(name: "IntegraChronos", value: modelo.IntegraChronos.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "Simular", value: modelo.Simular.ToInt(), direction: ParameterDirection.Input);

                parametros.Add(name: "Id", value: modelo.Id, direction: ParameterDirection.Input);

                con.Execute(@"UPDATE CRM.TB_CRM_MODELO 
                                    SET                                
                                        TipoOperacao = :TipoOperacao, 
                                        Descricao = :Descricao, 
                                        Status = :Status,                 
                                        FormaPagamento = :FormaPagamento, 
                                        QtdeDias = :QtdeDias, 
                                        Validade = :Validade, 
                                        TipoValidade = :TipoValidade, 
                                        DiasFreeTime = :DiasFreeTime,                                                 
                                        VendedorId = :VendedorId,
                                        ImpostoId = :ImpostoId,
                                        DataInatividade = :DataInatividade,
                                        Acordo = :Acordo,
                                        Escalonado = :Escalonado,
                                        ParametroBL = :ParametroBL,
                                        ParametroConteiner = :ParametroConteiner,
                                        ParametroLote = :ParametroLote,
                                        ParametroIdTabela = :ParametroIdTabela,
                                        HubPort = :HubPort,
                                        CobrancaEspecial = :CobrancaEspecial,
                                        DesovaParcial = :DesovaParcial,
                                        FatorCP = :FatorCP,
                                        PosicIsento = :PosicIsento,
                                        TipoServico = :TipoServico,
                                        IntegraChronos = :IntegraChronos,
                                        Simular = :Simular
                                    WHERE Id = :Id", parametros);
            }
        }

        public IEnumerable<Modelo> ObterModelos()
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<Modelo>(@"SELECT * FROM CRM.TB_CRM_MODELO ORDER BY DESCRICAO");
            }
        }

        public IEnumerable<Modelo> ObterModelosAtivos()
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<Modelo>(@"SELECT * FROM CRM.TB_CRM_MODELO WHERE STATUS = 1 ORDER BY DESCRICAO");
            }
        }

        public Modelo ObterModeloPorId(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<Modelo>(@"SELECT * FROM CRM.TB_CRM_MODELO WHERE Id = :mId", new { mId = id }).FirstOrDefault();
            }
        }

        public void Excluir(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                con.Execute(@"DELETE FROM CRM.TB_CRM_MODELO WHERE Id = :mId", new { mId = id });
            }
        }

        public string ObterValorPorCampo(string campo, int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<string>($@"SELECT {campo} FROM CRM.VW_CRM_MODELOS WHERE Id = :mId", new { mId = id }).FirstOrDefault();
            }
        }

        public Modelo ObterModeloPorDescricao(string descricao, int? id = 0)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var SQL = id == null
                    ? "SELECT * FROM CRM.TB_CRM_MODELO WHERE Descricao = :descricao"
                    : "SELECT * FROM CRM.TB_CRM_MODELO WHERE Descricao = :descricao AND Id <> :id";

                return con.Query<Modelo>(SQL, new { descricao, id }).FirstOrDefault();
            }
        }

        public IEnumerable<Modelo> ObterModelosPorDescricao(string descricao)
        {
            var criterio = descricao.ToUpper() + "%";

            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<Modelo>("SELECT * FROM CRM.TB_CRM_MODELO WHERE UPPER(Descricao) LIKE :criterio AND ROWNUM < 300", new { criterio });
            }
        }

        public IEnumerable<Modelo> ObterModelosPorTipoOperacao(TipoOperacao tipo)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<Modelo>(@"SELECT Id, Descricao FROM CRM.TB_CRM_MODELO WHERE TipoOperacao = :tipo AND Status = 1 ORDER BY DESCRICAO", new { tipo });
            }
        }

        public IEnumerable<VinculoModeloSimuladoDTO> ObterModelosSimuladorVinculados(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "ModeloId", value: id, direction: ParameterDirection.Input);

                return con.Query<VinculoModeloSimuladoDTO>(@"SELECT A.Id, C.Id As ModeloSimuladorId, C.Descricao FROM CRM.TB_CRM_MODELO_SIMULADOR A INNER JOIN CRM.TB_CRM_MODELO B ON A.MODELOID = B.ID INNER JOIN CRM.TB_CRM_SIMULADOR_MODELO C ON A.ModeloSimuladorId = C.Id WHERE A.ModeloId = :ModeloId", parametros);
            }
        }

        public void CadastrarModeloSimulador(ModeloSimulador modeloSimulador)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "ModeloId", value: modeloSimulador.ModeloId, direction: ParameterDirection.Input);
                parametros.Add(name: "ModeloSimuladorId", value: modeloSimulador.Id, direction: ParameterDirection.Input);

                con.Execute(@"INSERT INTO CRM.TB_CRM_MODELO_SIMULADOR (Id, ModeloId, ModeloSimuladorId) VALUES (CRM.SEQ_CRM_MODELO_SIMULADOR.NEXTVAL, :ModeloId, :ModeloSimuladorId)", parametros);
            }
        }

        public void ExcluirModeloSimulador(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Id", value: id, direction: ParameterDirection.Input);

                con.Execute(@"DELETE FROM CRM.TB_CRM_MODELO_SIMULADOR WHERE Id = :Id", parametros);
            }
        }
    }
}
