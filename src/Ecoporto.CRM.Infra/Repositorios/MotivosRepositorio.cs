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
    public class MotivosRepositorio : IMotivosRepositorio
    {
        public void CadastrarMotivo(SolicitacaoComercialMotivo solicitacaoMotivo)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "Descricao", value: solicitacaoMotivo.Descricao, direction: ParameterDirection.Input);
                parametros.Add(name: "Status", value: solicitacaoMotivo.Status, direction: ParameterDirection.Input);
                parametros.Add(name: "CancelamentoNF", value: solicitacaoMotivo.CancelamentoNF.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "Desconto", value: solicitacaoMotivo.Desconto.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "Restituicao", value: solicitacaoMotivo.Restituicao.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "Outros", value: solicitacaoMotivo.Outros.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "ProrrogacaoBoleto", value: solicitacaoMotivo.ProrrogacaoBoleto.ToInt(), direction: ParameterDirection.Input);

                con.Execute(@"INSERT INTO CRM.TB_CRM_SOLICITACAO_MOTIVOS (Id, Descricao, Status, CancelamentoNF, Desconto, Restituicao, ProrrogacaoBoleto, Outros) VALUES (CRM.SEQ_CRM_SOLICITACAO_MOTIVOS.NEXTVAL, :Descricao, :Status, :CancelamentoNF, :Desconto, :Restituicao, :ProrrogacaoBoleto, :Outros)", parametros);
            }
        }

        public void AtualizarMotivo(SolicitacaoComercialMotivo solicitacaoMotivo)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "Id", value: solicitacaoMotivo.Id, direction: ParameterDirection.Input);
                parametros.Add(name: "Descricao", value: solicitacaoMotivo.Descricao, direction: ParameterDirection.Input);
                parametros.Add(name: "Status", value: solicitacaoMotivo.Status, direction: ParameterDirection.Input);
                parametros.Add(name: "CancelamentoNF", value: solicitacaoMotivo.CancelamentoNF.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "Desconto", value: solicitacaoMotivo.Desconto.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "Restituicao", value: solicitacaoMotivo.Restituicao.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "Outros", value: solicitacaoMotivo.Outros.ToInt(), direction: ParameterDirection.Input);
                parametros.Add(name: "ProrrogacaoBoleto", value: solicitacaoMotivo.ProrrogacaoBoleto.ToInt(), direction: ParameterDirection.Input);

                con.Execute(@"UPDATE 
                                CRM.TB_CRM_SOLICITACAO_MOTIVOS SET 
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

        public void ExcluirMotivo(int motivoSolicitacaoId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Id", value: motivoSolicitacaoId, direction: ParameterDirection.Input);

                con.Execute(@"DELETE FROM CRM.TB_CRM_SOLICITACAO_MOTIVOS WHERE Id = :Id", parametros);
            }
        }

        public IEnumerable<SolicitacaoComercialMotivo> ObterSolicitacoesMotivo()
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<SolicitacaoComercialMotivo>(@"SELECT * FROM CRM.TB_CRM_SOLICITACAO_MOTIVOS ORDER BY Descricao");
            }
        }

        public SolicitacaoComercialMotivo ObterSolicitacaoMotivoPorId(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                return con.Query<SolicitacaoComercialMotivo>(@"SELECT * FROM CRM.TB_CRM_SOLICITACAO_MOTIVOS WHERE Id = :smId", new { smId = id }).FirstOrDefault();
            }
        }
    }
}
