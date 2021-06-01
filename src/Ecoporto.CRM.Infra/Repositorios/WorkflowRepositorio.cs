using Dapper;
using Ecoporto.CRM.Business.Interfaces.Repositorios;
using Ecoporto.CRM.Business.Models;
using Ecoporto.CRM.Infra.Configuracao;
using Ecoporto.CRM.Workflow.Enums;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Ecoporto.CRM.Infra.Repositorios
{
    public class WorkflowRepositorio : IWorkflowRepositorio
    {
        public int IncluirEnvioAprovacao(EnvioWorkflow workflow)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "ProcessoId", value: workflow.ProcessoId, direction: ParameterDirection.Input);
                parametros.Add(name: "ProcessoFilhoId", value: workflow.ProcessoFilhoId, direction: ParameterDirection.Input);
                parametros.Add(name: "Protocolo", value: workflow.Protocolo, direction: ParameterDirection.Input);
                parametros.Add(name: "Mensagem", value: workflow.Mensagem, direction: ParameterDirection.Input);
                parametros.Add(name: "Processo", value: workflow.Processo, direction: ParameterDirection.Input);
                parametros.Add(name: "DataCadastro", value: DateTime.Now, direction: ParameterDirection.Input);
                parametros.Add(name: "CriadoPor", value: workflow.CriadoPor, direction: ParameterDirection.Input);

                parametros.Add(name: "Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                con.Execute(@"INSERT INTO 
                                CRM.TB_CRM_WORKFLOW 
                                (
                                    Id, 
                                    ProcessoId, 
                                    ProcessoFilhoId,
                                    Protocolo, 
                                    Mensagem, 
                                    Processo,
                                    DataCadastro, 
                                    CriadoPor
                                ) VALUES (
                                    CRM.SEQ_CRM_WORKFLOW.NEXTVAL, 
                                    :ProcessoId, 
                                    :ProcessoFilhoId,
                                    :Protocolo,
                                    :Mensagem,
                                    :Processo,
                                    :DataCadastro,
                                    :CriadoPor) RETURNING Id INTO :Id", parametros);

                return parametros.Get<int>("Id");
            }
        }

        public void CancelarEnvioAprovacao(int id)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Id", value: id, direction: ParameterDirection.Input);

                con.Execute(@"DELETE FROM CRM.TB_CRM_WORKFLOW WHERE Id = :Id", parametros);
            }
        }

        public IEnumerable<EnvioWorkflow> ObterAprovacoesPorOportunidade(int oportunidadeId, Processo processo)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "ProcessoId", value: oportunidadeId, direction: ParameterDirection.Input);
                parametros.Add(name: "Processo", value: processo, direction: ParameterDirection.Input);
                               
                 return con.Query<EnvioWorkflow>(@"SELECT A.Id, A.ProcessoId, A.Protocolo, A.Mensagem, A.DataCadastro, A.Processo, NVL(B.Cancelado,0) Cancelado, A.CriadoPor FROM TB_CRM_WORKFLOW A INNER JOIN TB_CRM_OPORTUNIDADES B ON A.ProcessoId = B.Id WHERE A.ProcessoId = :ProcessoId AND A.Processo = :Processo", parametros);
                 
            }
        }

        public IEnumerable<EnvioWorkflow> ObterAprovacoesAnaliseDeCredito(int processoId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "ProcessoId", value: processoId, direction: ParameterDirection.Input);
                parametros.Add(name: "Processo", value: Processo.ANALISE_DE_CREDITO, direction: ParameterDirection.Input);
                
                return con.Query<EnvioWorkflow>(@"   SELECT Id, ProcessoId, Protocolo, Mensagem, DataCadastro, Processo, CriadoPor FROM crm.TB_CRM_WORKFLOW WHERE ProcessoId = :ProcessoId
                  AND Processo = 13
                union all
                  SELECT Id, ProcessoId, Protocolo, Mensagem, DataCadastro, Processo, CriadoPor FROM crm.TB_CRM_WORKFLOW WHERE  
                    Processo = 14 and processoid in ( select id from CRM.TB_CRM_SPC_LIMITE_CREDITO where contaid=:ProcessoId)", parametros);
            }
        }

        public IEnumerable<EnvioWorkflow> ObterAprovacoesLimiteDeCredito(int processoId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "ProcessoId", value: processoId, direction: ParameterDirection.Input);
                parametros.Add(name: "Processo", value: Processo.ANALISE_DE_CREDITO_COND_PGTO , direction: ParameterDirection.Input);

                return con.Query<EnvioWorkflow>(@"SELECT Id, ProcessoId, Protocolo, Mensagem, DataCadastro, Processo, CriadoPor FROM TB_CRM_WORKFLOW WHERE ProcessoId = :ProcessoId AND Processo = :Processo", parametros);
            }
        }

        public int UltimoProtocolo(int oportunidadeId, Processo processo, int processoFilhoId = 0)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "ProcessoId", value: oportunidadeId, direction: ParameterDirection.Input);
                parametros.Add(name: "Processo", value: processo, direction: ParameterDirection.Input);

                if (processoFilhoId > 0)
                {
                    parametros.Add(name: "ProcessoFilhoId", value: processoFilhoId, direction: ParameterDirection.Input);

                    return con.Query<int>(@"SELECT NVL(MAX(Protocolo),0) FROM CRM.TB_CRM_WORKFLOW WHERE ProcessoId = :ProcessoId AND Processo = :Processo AND NVL(ProcessoFilhoId, :ProcessoFilhoId) = :ProcessoFilhoId", parametros).Single();
                }

                return con.Query<int>(@"SELECT NVL(MAX(Protocolo),0) FROM CRM.TB_CRM_WORKFLOW WHERE ProcessoId = :ProcessoId AND Processo = :Processo", parametros).Single();
            }
        }

        public int AcessoSistemaWorkflow(Usuario usuario)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "UsrId", value: usuario.Id, direction: ParameterDirection.Input);
                parametros.Add(name: "TiaCNPJ", value: "00.000.000/0000-00", direction: ParameterDirection.Input);
                parametros.Add(name: "TiaEmpresa", value: 1, direction: ParameterDirection.Input);
                parametros.Add(name: "TiaLogin", value: usuario.LoginWorkflow, direction: ParameterDirection.Input);

                parametros.Add(name: "Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                con.Execute(@"INSERT INTO INTERNET.TB_INT_ACESSO (TIAID, USRID, TIACNPJ, TIAEMPRESA, TIALOGIN) VALUES 
                                    (INTERNET.SEQ_INT_ACESSO.NEXTVAL, :UsrId, :TiaCNPJ, :TiaEmpresa, :TiaLogin) RETURNING TiaId INTO :Id", parametros);

                return parametros.Get<int>("Id");
            }
        }
    }
}
