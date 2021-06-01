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
    public class OcorrenciasRepositorio : IOcorrenciasRepositorio
    {
        public void CadastrarOcorrencia(SolicitacaoComercialOcorrencia solicitacaoOcorrencia)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "Descricao", value: solicitacaoOcorrencia.Descricao, direction: ParameterDirection.Input);
                parametros.Add(name: "Status", value: solicitacaoOcorrencia.Status, direction: ParameterDirection.Input);
                parametros.Add(name: "CancelamentoNF", value: solicitacaoOcorrencia.CancelamentoNF.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "Desconto", value: solicitacaoOcorrencia.Desconto.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "Restituicao", value: solicitacaoOcorrencia.Restituicao.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "Outros", value: solicitacaoOcorrencia.Outros.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "ProrrogacaoBoleto", value: solicitacaoOcorrencia.ProrrogacaoBoleto.ToInt(), direction: ParameterDirection.Input);

                con.Execute(@"INSERT INTO CRM.TB_CRM_SOLICITACAO_OCORRENCIAS (Id, Descricao, Status, CancelamentoNF, Desconto, Restituicao, ProrrogacaoBoleto, Outros) VALUES (CRM.SEQ_CRM_SOLICITACAO_OCORRENCIA.NEXTVAL, :Descricao, :Status, :CancelamentoNF, :Desconto, :Restituicao, :ProrrogacaoBoleto, :Outros)", parametros);
            }
        }

        public void AtualizarOcorrencia(SolicitacaoComercialOcorrencia solicitacaoOcorrencia)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "Id", value: solicitacaoOcorrencia.Id, direction: ParameterDirection.Input);
                parametros.Add(name: "Descricao", value: solicitacaoOcorrencia.Descricao, direction: ParameterDirection.Input);
                parametros.Add(name: "Status", value: solicitacaoOcorrencia.Status, direction: ParameterDirection.Input);
                parametros.Add(name: "CancelamentoNF", value: solicitacaoOcorrencia.CancelamentoNF.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "Desconto", value: solicitacaoOcorrencia.Desconto.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "Restituicao", value: solicitacaoOcorrencia.Restituicao.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "Outros", value: solicitacaoOcorrencia.Outros.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "ProrrogacaoBoleto", value: solicitacaoOcorrencia.ProrrogacaoBoleto.ToInt(), direction: ParameterDirection.Input);

                con.Execute(@"UPDATE 
                                CRM.TB_CRM_SOLICITACAO_OCORRENCIAS SET 
                                    Descricao = :Descricao,
                                    Status = :Status,
                                    CancelamentoNF = :CancelamentoNF, 
                                    Desconto = :Desconto, 
                                    Restituicao = :Restituicao, 
                                    ProrrogacaoBoleto = :ProrrogacaoBoleto,
                                    Outros = :Outros
                              WHERE Id = :Id", parametros);
            }
        }

        public void ExcluirOcorrencia(int ocorrenciaSolicitacaoId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Id", value: ocorrenciaSolicitacaoId, direction: ParameterDirection.Input);

                con.Execute(@"DELETE FROM CRM.TB_CRM_SOLICITACAO_OCORRENCIAS WHERE Id = :Id", parametros);
            }
        }

        public IEnumerable<SolicitacaoComercialOcorrencia> ObterSolicitacoesOcorrencia()
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<SolicitacaoComercialOcorrencia>(@"SELECT * FROM CRM.TB_CRM_SOLICITACAO_OCORRENCIAS Order By Descricao");
            }
        }

        public SolicitacaoComercialOcorrencia ObterSolicitacaoOcorrenciaPorId(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<SolicitacaoComercialOcorrencia>(@"SELECT * FROM CRM.TB_CRM_SOLICITACAO_OCORRENCIAS WHERE Id = :smId", new { smId = id }).FirstOrDefault();
            }
        }
    }
}
