using Dapper;
using Ecoporto.CRM.IntegraChronosAPI.Models;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Ecoporto.CRM.IntegraChronosAPI.Data
{
    public class FilaRepository
    {
        public async Task<int> Cadastrar(Processo model)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "Id_Processo", value: model.Id_Processo, direction: ParameterDirection.Input);
                parametros.Add(name: "Tipo_Processo", value: model.Tipo_Processo, direction: ParameterDirection.Input);
                parametros.Add(name: "Id_Workflow", value: model.Id_Workflow, direction: ParameterDirection.Input);
                parametros.Add(name: "Id_Etapa", value: model.Id_Etapa, direction: ParameterDirection.Input);
                parametros.Add(name: "Acao", value: model.Acao, direction: ParameterDirection.Input);

                parametros.Add(name: "Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await con.ExecuteAsync(@"INSERT INTO CRM.TB_CRM_FILA_INTEGRACAO (Id, Id_Processo, Tipo_Processo, Id_Workflow, Id_Etapa, Acao, Status) VALUES (CRM.SEQ_CRM_FILA_INTEGRACAO.NEXTVAL, :Id_Processo, :Tipo_Processo, :Id_Workflow, :Id_Etapa, :Acao, 1) RETURNING Id INTO :Id", parametros);

                return parametros.Get<int>("Id");
            }
        }

        public async Task<Processo> Consultar(int protocolo)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Id", value: protocolo, direction: ParameterDirection.Input);

                var result = await con.QueryAsync<Processo>(@"SELECT Id, Id_Processo, Tipo_Processo, Id_Workflow, Id_Etapa, Acao, Status, Motivo, Data_Execucao FROM CRM.TB_CRM_FILA_INTEGRACAO WHERE Id = :Id", parametros);

                return result.FirstOrDefault();
            }
        }

        public async Task<Processo> ConsultarPorProcesso(Processo model)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "Id_Processo", value: model.Id_Processo, direction: ParameterDirection.Input);
                parametros.Add(name: "Tipo_Processo", value: model.Tipo_Processo, direction: ParameterDirection.Input);
                parametros.Add(name: "Id_Workflow", value: model.Id_Workflow, direction: ParameterDirection.Input);
                parametros.Add(name: "Id_Etapa", value: model.Id_Etapa, direction: ParameterDirection.Input);
                parametros.Add(name: "Acao", value: model.Acao, direction: ParameterDirection.Input);

                var result = await con.QueryAsync<Processo>(@"SELECT Id FROM CRM.TB_CRM_FILA_INTEGRACAO WHERE Id_Processo = :Id_Processo AND Tipo_Processo = :Tipo_Processo AND Id_WorkFlow = : Id_Workflow AND Id_Etapa = :Id_Etapa AND Acao = :Acao AND Status = 1", parametros);

                return result.FirstOrDefault();
            }
        }

        public async Task<bool> ExisteProcesso(Processo model)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "Id_Processo", value: model.Id_Processo, direction: ParameterDirection.Input);
                parametros.Add(name: "Tipo_Processo", value: model.Tipo_Processo, direction: ParameterDirection.Input);

                var tabela = string.Empty;

                switch (model.Tipo_Processo)
                {
                    case 1:
                    case 9:
                        tabela = "CRM.TB_CRM_OPORTUNIDADES";                                           
                        break;
                    case 2:
                        tabela = "CRM.TB_CRM_OPORTUNIDADE_FICHA_FAT";
                        break;
                    case 3:
                        tabela = "CRM.TB_CRM_OPORTUNIDADE_PREMIOS";
                        break;
                    case 4:
                        tabela = "CRM.TB_CRM_OPORTUNIDADE_ADENDOS";
                        break;
                    case 5:
                        tabela = "CRM.TB_CRM_SOLICITACAO_CANCEL_NF";
                        break;
                    case 6:
                        tabela = "CRM.TB_CRM_SOLICITACAO_DESCONTO";
                        break;
                    case 7:
                        tabela = "CRM.TB_CRM_SOLICITACAO_RESTITUICAO";
                        break;
                    case 8:
                        tabela = "CRM.TB_CRM_SOLICITACAO_PRORROGACAO";
                        break;
                    case 10:
                        tabela = "CRM.TB_CRM_SOLICITACAO_FORMA_PGTO";
                        break;
                }

                var result = await con.QueryAsync<int>($"SELECT COUNT(1) FROM {tabela} WHERE ID = :Id_Processo", parametros);

                return result.Any();
            }
        }
    }
}