using Dapper;
using Ecoporto.CRM.Service.Models;
using Ecoporto.CRM.Workflow.Services;
using NLog;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Configuration;
using System.Data;
using System.Linq;
using System.ServiceProcess;
using System.Timers;
using System.Text.RegularExpressions;
using Ecoporto.CRM.Service.Enums;

namespace Ecoporto.CRM.Service
{
    partial class Workflow : ServiceBase
    {
        private Timer timer;

        Logger logger = LogManager.GetCurrentClassLogger();
        string stringConexao = ConfigurationManager.ConnectionStrings["StringConexaoOracle"].ConnectionString;
        string tempo = ConfigurationManager.AppSettings["Tempo"].ToString();

        public Workflow()
        {
            InitializeComponent();
        }

        public void Setup()
        {
            this.timer = new Timer(Convert.ToDouble(tempo))
            {
                AutoReset = true
            };

            this.timer.Elapsed += Run;
            this.timer.Start();

            logger.Info($"{DateTime.Now} - Serviço iniciado. Timer: {tempo}");
        }

        protected override void OnStart(string[] args)
        {
            Setup();
        }

        private void Run(object sender, ElapsedEventArgs e)
        {
            Iniciar();
        }

        public void Iniciar()
        {
            var token = Autenticador.Autenticar();

            if (token == null)
            {
                logger.Error("Não foi possível se autenticar no serviço de Workflow");
                return;
            }

            var workflow = new WorkflowService(token);

            var fila = workflow.ObterFilaWorkflow();

            if (fila == null)
            {
                logger.Info("Não foi possível obter a lista de Workflows");
                return;
            }

            foreach (var wkf in fila.list)
            {
                var idWorkflow = wkf.id_WorkFlow;
                var idProcessoRequisitante = wkf.autonum_Sistema_Processo_Requisitante;
                var idProcesso = wkf.id_Processo;
                logger.Info("Lendo Fila  : IdProcesso" + idProcesso.ToString() + " Status " + wkf.id_Status.ToString() + " ID " + idProcessoRequisitante.ToString());


                if (!Int32.TryParse(idProcessoRequisitante, out _))
                {
                    logger.Error($"O Id (Sistema Processo Requisitante) {idProcessoRequisitante} não é um número válido");
                    continue;
                }

                if (!Int32.TryParse(idProcesso.ToString(), out _))
                {
                    logger.Error($"O Id (Sistema Processo Requisitante) {idProcesso} deve ser um número válido");
                    continue;
                }

                if (!(idProcesso >= 1 && idProcesso <= 14))
                {
                    logger.Error($"O Id do Processo deve ser um valor numérico e estar entre 1-14");
                    continue;
                }

                var idStatus = wkf.id_Status;

                if (idStatus == 2 || idStatus == 3)
                {
                    try
                    {
                        /*
                            Processos:

                            01. OPORTUNIDADE,
                            02. FICHA_FATURAMENTO,
                            03. PREMIO_PARCERIA,
                            04. ADENDO,
                            05. SOLICITACAO_CANCELAMENTO,
                            06. SOLICITACAO_DESCONTO,
                            07. SOLICITACAO_RESTITUICAO,
                            08. SOLICITACAO_PRORROGACAO,
                            09. CANCELAMENTO_OPORTUNIDADE,
                            10. SOLICITACAO_OUTROS
                         */

                        if (idProcesso < 5 || idProcesso == 9)
                        {
                            if (!OportunidadeExistente(idProcessoRequisitante, idProcesso))
                            {
                                switch (idProcesso)
                                {
                                    case 9:
                                        logger.Error("Oportunidade inexistente - autonum_Sistema_Processo_Requisitante: " + idWorkflow);
                                        break;
                                    case 2:
                                        logger.Error("Ficha inexistente - autonum_Sistema_Processo_Requisitante: " + idWorkflow);
                                        break;
                                    case 3:
                                        logger.Error("Prêmio inexistente - autonum_Sistema_Processo_Requisitante: " + idWorkflow);
                                        break;
                                    case 4:
                                        logger.Error("Adendo inexistente - autonum_Sistema_Processo_Requisitante: " + idWorkflow);
                                        break;
                                }

                                continue;
                            }
                        }
                        else
                        {
                            if (idProcesso <= 12)
                            {
                                if (!SolicitacaoExistente(idProcessoRequisitante))
                                {
                                    logger.Error("Solicitação Comercial inexistente - autonum_Sistema_Processo_Requisitante: " + idWorkflow);
                                    continue;
                                }
                            }
                        }

                        AtualizarStatus(idProcessoRequisitante, idWorkflow, idProcesso, idStatus, workflow);
                    }
                    catch (Exception ex)
                    {
                        logger.Error("Falha ao atualizar o Status do registro " + ex.ToString());
                    }
                }
            }

            logger.Info("Processamento concluído");
        }

        public void AtualizarStatus(string id, int idWorkflow, int idProcesso, int status, WorkflowService workflow)
        {
            string SQL = string.Empty;
            string SQLL = string.Empty;
            string SQLFichas = string.Empty;
            string SQLFichasRevisao = string.Empty;
            int oportunidadeId = 0;
            int fichaRevisadaId = 0;

            SQLL = "";
            logger.Info("Lendo Fila  : IdProcesso" + idProcesso.ToString() +" Status "+ status.ToString() +" ID "+id.ToString() );
            // Oportunidade
            if (idProcesso == 1 || idProcesso == 9)
            {
                // Aprovada
                if (status == 2)
                {
                    var revisao = OportunidadeRevisada(id);

                    if (!string.IsNullOrEmpty(revisao))
                    {
                        if (Int32.TryParse(revisao, out int resultado))
                        {
                            if (resultado > 0)
                                AtualizarOportunidadeRevisada(resultado);
                        }
                    }

                    // StatusOportunidade = 1 (Ativa)
                    // StatusOportunidade = 2 (Cancelado)
                    // SucessoNegociacao = 4 (Ganho)
                    // EstagioNegociacao = 4 (Ganho)
                    //SQL = "UPDATE CRM.TB_CRM_OPORTUNIDADES SET Aprovada = 1, StatusOportunidade = 1, SucessoNegociacao = 4 WHERE Id = :Id AND StatusOportunidade <> 2";

                    SQL = @"UPDATE CRM.TB_CRM_OPORTUNIDADES 
                                SET Aprovada = CASE WHEN Cancelado = 1 THEN 0 ELSE 1 END, 
                                    StatusOportunidade = CASE WHEN Cancelado = 1 THEN 2 ELSE 1 END, 
                                    SucessoNegociacao = CASE WHEN Cancelado = 1 THEN SucessoNegociacao ELSE 4 END,
                                    EstagioNegociacao = CASE WHEN Cancelado = 1 THEN EstagioNegociacao ELSE 4 END
                            WHERE Id = :Id";

                    // StatusFichaFaturamento = 3 (Aprovado)
                    SQLFichas = "UPDATE TB_CRM_OPORTUNIDADE_FICHA_FAT SET StatusFichaFaturamento = 3 WHERE OportunidadeId = :Id AND StatusFichaFaturamento = 2";

                    ObterNumeroDeTabela(id, workflow);
                }

                // Rejeitada
                if (status == 3)
                {
                    // StatusOportunidade = 1 (Recusado)
                    SQL = "UPDATE CRM.TB_CRM_OPORTUNIDADES " +
                        "       SET StatusOportunidade = CASE WHEN Cancelado = 1 THEN 1 ELSE 4 END, " +
                        "           Cancelado = CASE WHEN Cancelado = 1 THEN 0 ELSE Cancelado END, TabelaId = NULL " +
                        "  WHERE Id = :Id";

                    // StatusFichaFaturamento = 4 (Rejeitado)
                    SQLFichas = "UPDATE TB_CRM_OPORTUNIDADE_FICHA_FAT SET StatusFichaFaturamento = 4 WHERE OportunidadeId = :Id AND StatusFichaFaturamento = 2";
                }
            }

            // Fichas de Faturamento
            if (idProcesso == 2)
            {
                var fichaBusca = ObterFichaFaturamentoPorId(id);

                // Aprovada
                if (status == 2)
                {
                    // StatusFichaFaturamento = 3 (Aprovado)
                    SQL = "UPDATE CRM.TB_CRM_OPORTUNIDADE_FICHA_FAT SET StatusFichaFaturamento = 3 WHERE Id = :Id";

                    if (fichaBusca != null)
                    {
                        if (fichaBusca.RevisaoId > 0)
                        {
                            //SQLFichasRevisao
                            fichaRevisadaId = fichaBusca.RevisaoId;

                            SQLFichasRevisao = "UPDATE CRM.TB_CRM_OPORTUNIDADE_FICHA_FAT SET StatusFichaFaturamento = 6 WHERE Id = :FichaRevisaoId";
                        }
                    }
                }

                // Rejeitada
                if (status == 3)
                {
                    // StatusFichaFaturamento = 3 (Rejeitado)
                    SQL = "UPDATE CRM.TB_CRM_OPORTUNIDADE_FICHA_FAT SET StatusFichaFaturamento = 4 WHERE Id = :Id";

                    if (fichaBusca != null)
                    {
                        if (fichaBusca.RevisaoId > 0)
                        {
                            if (fichaBusca.RevisaoId > 0)
                            {
                                //SQLFichasRevisao
                                fichaRevisadaId = fichaBusca.RevisaoId;

                                SQLFichasRevisao = "UPDATE CRM.TB_CRM_OPORTUNIDADE_FICHA_FAT SET StatusFichaFaturamento = 6 WHERE Id = :FichaRevisaoId";
                            }
                        }
                    }
                }
            }

            // Prêmios de Parceria
            if (idProcesso == 3)
            {
                // Aprovada
                if (status == 2)
                {
                    var revisao = PremioRevisado(id);

                    if (!string.IsNullOrEmpty(revisao))
                    {
                        if (Int32.TryParse(revisao, out int resultado))
                        {
                            if (resultado > 0)
                                AtualizarPremioRevisado(resultado);
                        }
                    }

                    // StatusPremioParceria = 3 (Cadastrado)
                    SQL = "UPDATE CRM.TB_CRM_OPORTUNIDADE_PREMIOS SET StatusPremioParceria = CASE WHEN Cancelado = 1 THEN 6 ELSE 3 END WHERE Id = :Id";

                }

                // Rejeitada
                if (status == 3)
                {
                    // StatusPremioParceria = 5 (Rejeitado)
                    // StatusPremioParceria = 3 (Cadastro)

                    SQL = "UPDATE CRM.TB_CRM_OPORTUNIDADE_PREMIOS " +
                       "       SET StatusPremioParceria = CASE WHEN Cancelado = 1 THEN 3 ELSE 5 END, " +
                       "           Cancelado = CASE WHEN Cancelado = 1 THEN 0 ELSE Cancelado END " +
                       "  WHERE Id = :Id";
                }
            }

            // Adendos
            if (idProcesso == 4)
            {
                var adendoBusca = ObterAdendoPorId(id);
              
                // Aprovada
                if (status == 2)
                {
                    // StatusAdendo = 4 (Aprovado)
                    SQL = "UPDATE CRM.TB_CRM_OPORTUNIDADE_ADENDOS SET StatusAdendo = 4 WHERE Id = :Id";
                    AtualizarAdendo(id);

                    if (adendoBusca != null)
                    {
                        oportunidadeId = adendoBusca.OportunidadeId;

                        if (adendoBusca.TipoAdendo == 2 || adendoBusca.TipoAdendo == 3)
                        {
                            if (adendoBusca.TipoAdendo == 2)
                            {
                                var formaPagamentoAdendo = ObterFormaPagamentoAdendo(adendoBusca.Id);

                                if (formaPagamentoAdendo == FormaPagamentoAdendo.FATURADO)
                                {
                                    SQLFichas = "UPDATE TB_CRM_OPORTUNIDADE_FICHA_FAT SET StatusFichaFaturamento = 3 WHERE OportunidadeId = :OportunidadeId AND StatusFichaFaturamento = 2";
                                }
                            }

                            if (adendoBusca.TipoAdendo == 3)
                            {
                                var temficha = TemFichaFFaturamento(adendoBusca.Id, oportunidadeId);
                                if (temficha > 0)
                                {
                                    SQLFichas = "UPDATE TB_CRM_OPORTUNIDADE_FICHA_FAT SET StatusFichaFaturamento = 3 WHERE OportunidadeId = :OportunidadeId AND StatusFichaFaturamento = 2";
                                }
                                else
                                {
                                    SQLFichas = "";
                                }
                            }
                        }
                    }                    
                }

                // Rejeitada
                if (status == 3)
                {                    
                    // StatusAdendo = 3 (Rejeitado)
                    SQL = "UPDATE CRM.TB_CRM_OPORTUNIDADE_ADENDOS SET StatusAdendo = 3 WHERE Id = :Id";

                    if (adendoBusca != null)
                    {
                        oportunidadeId = adendoBusca.OportunidadeId;

                        if (adendoBusca.TipoAdendo == 2 || adendoBusca.TipoAdendo == 3) 
                        {
                            if (adendoBusca.TipoAdendo == 2)
                            {
                                var formaPagamentoAdendo = ObterFormaPagamentoAdendo(adendoBusca.Id);

                                if (formaPagamentoAdendo == FormaPagamentoAdendo.FATURADO)
                                {
                                    SQLFichas = "UPDATE TB_CRM_OPORTUNIDADE_FICHA_FAT SET StatusFichaFaturamento = 4 WHERE OportunidadeId = :OportunidadeId AND StatusFichaFaturamento = 2";
                                }
                            }
                            
                            if (adendoBusca.TipoAdendo == 3)
                            {
                                var temficha = TemFichaFFaturamento(adendoBusca.Id, oportunidadeId);
                                if (temficha > 0)
                                {
                                    SQLFichas = "UPDATE TB_CRM_OPORTUNIDADE_FICHA_FAT SET StatusFichaFaturamento = 4 WHERE OportunidadeId = :OportunidadeId AND StatusFichaFaturamento = 2";
                                }
                                else
                                {
                                    SQLFichas = "";
                                }
                            }
                        }
                    }
                }
            }

            // Solicitações
            if (idProcesso == 5 || idProcesso == 6 || idProcesso == 7 || idProcesso == 8 || idProcesso == 10)
            {
                // Aprovada
                if (status == 2)
                {
                    // StatusSolicitacao = 3 (Aprovado)
                    SQL = "UPDATE CRM.TB_CRM_SOLICITACOES SET StatusSolicitacao = 3 WHERE Id = :Id";
                }

                // Rejeitada
                if (status == 3)
                {
                    // StatusSolicitacao = 4 (Rejeitado)
                    SQL = "UPDATE CRM.TB_CRM_SOLICITACOES SET StatusSolicitacao = 4 WHERE Id = :Id";
                }
            }

            //Carnelos em 30/12/20 - Consulta SPC
            if (idProcesso == 13)
            {
                logger.Info("Atualizando 13  IdProcesso" + idProcesso.ToString() + " Status " + status.ToString() + " ID " + id.ToString());

                if (status == 2)
                {
                    SQL = "UPDATE CRM.TB_CRM_SPC_CONSULTAS SET StatusAnaliseDeCredito = 3 WHERE ContaId = :Id";

                    SQLL = "UPDATE CRM.TB_CRM_SPC_LIMITE_CREDITO SET StatusLimiteCredito = 3 WHERE ContaId = :Id";

                }

                if (status == 3)
                {
                    SQL = "UPDATE CRM.TB_CRM_SPC_CONSULTAS SET validade= ADD_MONTHS(TRUNC(dataconsulta), 3),StatusAnaliseDeCredito = 4 WHERE ContaId = :Id";

                   SQLL = "UPDATE CRM.TB_CRM_SPC_LIMITE_CREDITO SET StatusLimiteCredito = 4 WHERE ContaId = :Id";
                }

            }

            //Carnelos em 30/12/20 - Limite de Credito
            if (idProcesso == 14)
            {
                logger.Info("Atualizando 14  IdProcesso" + idProcesso.ToString() + " Status " + status.ToString() + " ID " + id.ToString());
                if (status == 2)
                {
                    SQL = "UPDATE CRM.TB_CRM_SPC_LIMITE_CREDITO SET StatusLimiteCredito = 3 WHERE Id = :Id";
                }

                if (status == 3)
                {
                    SQL = "UPDATE CRM.TB_CRM_SPC_LIMITE_CREDITO SET StatusLimiteCredito = 4 WHERE Id = :Id";
                }

            }



            if (!string.IsNullOrEmpty(SQL))
            {
                using (OracleConnection con = new OracleConnection(stringConexao))
                {
                    var parametros = new DynamicParameters();

                    parametros.Add(name: "Id", value: id, direction: ParameterDirection.Input);
                    parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);
                    parametros.Add(name: "FichaRevisaoId", value: fichaRevisadaId, direction: ParameterDirection.Input);
                    
                    con.Execute(SQL, parametros);

                    if (!string.IsNullOrEmpty(SQLFichas))
                    {
                        con.Execute(SQLFichas, parametros);
                    }

                    if (!string.IsNullOrEmpty(SQLFichasRevisao))
                    {
                        con.Execute(SQLFichasRevisao, parametros);
                    }
                  
                    if (!string.IsNullOrEmpty(SQLL))
                        {
                            con.Execute(SQLL, parametros);
                        }
                }

                workflow.EnviarRetornoRegistroAtualizado(idWorkflow);

                logger.Info("Status atualizado com sucesso");
            }
        }

        public TipoAdendo ObterTipoAdendo(string adendoId)
        {
            using (OracleConnection con = new OracleConnection(stringConexao))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "AdendoId", value: adendoId, direction: ParameterDirection.Input);

                return con.Query<TipoAdendo>("SELECT TipoAdendo FROM CRM.TB_CRM_ADENDOS WHERE Id = :AdendoId", parametros).FirstOrDefault();
            }
        }

        public FormaPagamentoAdendo ObterFormaPagamentoAdendo(int adendoId)
        {
            using (OracleConnection con = new OracleConnection(stringConexao))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "AdendoId", value: adendoId, direction: ParameterDirection.Input);

                return con.Query<FormaPagamentoAdendo>("SELECT FormaPagamento FROM CRM.TB_CRM_ADENDO_FORMA_PAGAMENTO WHERE AdendoId = :AdendoId", parametros).FirstOrDefault();
            }
        }

            public FormaPagamentoAdendo TemFichaFFaturamento(int adendoId , int  Id)
            {
                using (OracleConnection con = new OracleConnection(stringConexao))
                {
                    var parametros = new DynamicParameters();
                    parametros.Add(name: "AdendoId", value: adendoId, direction: ParameterDirection.Input);
                    parametros.Add(name: "OportunidadeId", value: Id, direction: ParameterDirection.Input);
 
                return con.Query<FormaPagamentoAdendo>(@"SELECT count(1) contar

                        FROM
                            CRM.TB_CRM_ADENDO_SUB_CLIENTE A
                        INNER JOIN
                            CRM.TB_CRM_OPORTUNIDADE_ADENDOS B ON A.AdendoId = B.Id
                        INNER JOIN
                            CRM.TB_CRM_CONTAS C ON A.SubClienteId = C.Id
                        INNER JOIN
                            CRM.TB_CRM_OPORTUNIDADE_FICHA_FAT D ON D.CONTAID = C.ID AND  D.OportunidadeId = :OportunidadeId 
                        WHERE
                           A.Acao = 1 and   B.OportunidadeId = :OportunidadeId AND 
                            AdendoId = :AdendoId", parametros).FirstOrDefault();
                }

        }

        public bool OportunidadeExistente(string id, int idProcesso)
        {
            string tabela = string.Empty;
            var retorno = 0;

            switch (idProcesso)
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
            }

            if (!string.IsNullOrEmpty(tabela))
            {
                using (OracleConnection con = new OracleConnection(stringConexao))
                {
                    var parametros = new DynamicParameters();
                    parametros.Add(name: "Id", value: id, direction: ParameterDirection.Input);

                    retorno = con.Query<int>($"SELECT COUNT(1) FROM {tabela} WHERE ID = :Id", parametros).FirstOrDefault();
                }
            }

            return Convert.ToInt32(retorno) > 0;
        }

        public bool SolicitacaoExistente(string id)
        {
            var retorno = 0;

            using (OracleConnection con = new OracleConnection(stringConexao))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Id", value: id, direction: ParameterDirection.Input);

                retorno = con.Query<int>($"SELECT COUNT(1) FROM CRM.TB_CRM_SOLICITACOES WHERE ID = :Id", parametros).FirstOrDefault();
            }

            return Convert.ToInt32(retorno) > 0;
        }

        public string OportunidadeRevisada(string idOportunidade)
        {
            var resultado = 0;

            using (OracleConnection con = new OracleConnection(stringConexao))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Id", value: idOportunidade, direction: ParameterDirection.Input);

                resultado = con.Query<int>("SELECT NVL(RevisaoId,0) RevisaoId FROM CRM.TB_CRM_OPORTUNIDADES WHERE Id = :Id", parametros).FirstOrDefault();
            }

            if (Convert.ToInt32(resultado) > 0)
                logger.Info($"Oportunidade revisada: Id: {idOportunidade}");

            return resultado.ToString();
        }

        public string PremioRevisado(string idPremio)
        {
            var resultado = 0;

            using (OracleConnection con = new OracleConnection(stringConexao))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Id", value: idPremio, direction: ParameterDirection.Input);

                resultado = con.Query<int>("SELECT NVL(PremioRevisaoId,0) PremioRevisaoId FROM CRM.TB_CRM_OPORTUNIDADE_PREMIOS WHERE Id = :Id", parametros).FirstOrDefault();
            }

            if (Convert.ToInt32(resultado) > 0)
                logger.Info($"Prêmio revisado: Id: {idPremio}");

            return resultado.ToString();
        }

        public void AtualizarOportunidadeRevisada(int idOportunidade)
        {
            var resultado = 0;

            using (OracleConnection con = new OracleConnection(stringConexao))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Id", value: idOportunidade, direction: ParameterDirection.Input);

                resultado = con.Execute("UPDATE CRM.TB_CRM_OPORTUNIDADES SET StatusOportunidade = 3 WHERE Id = :Id", parametros);
            }

            if (Convert.ToInt32(resultado) > 0)
                logger.Info($"Oportunidade revisada: Id: {idOportunidade}");
        }

        public void AtualizarPremioRevisado(int idPremio)
        {
            var resultado = 0;

            using (OracleConnection con = new OracleConnection(stringConexao))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Id", value: idPremio, direction: ParameterDirection.Input);

                resultado = con.Execute("UPDATE CRM.TB_CRM_OPORTUNIDADE_PREMIOS SET StatusPremioParceria = 4 WHERE Id = :Id", parametros);
            }

            if (Convert.ToInt32(resultado) > 0)
                logger.Info($"Prêmio revisado: Id: {idPremio}");
        }
        
        public void AtualizarFormaPagamentoOportunidade(int oportunidadeId, FormaPagamentoAdendo formaPagamento)
        {
            using (OracleConnection con = new OracleConnection(stringConexao))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);
                parametros.Add(name: "FormaPagamento", value: formaPagamento, direction: ParameterDirection.Input);

                con.Execute("UPDATE CRM.TB_CRM_OPORTUNIDADE SET FormaPagamento = :FormaPagamento WHERE Id = :OportunidadeId", parametros);
            }
        }

        public void AtualizarAdendo(string id)
        {
            var parametros = new DynamicParameters();
            parametros.Add(name: "AdendoId", value: id, direction: ParameterDirection.Input);

            using (OracleConnection con = new OracleConnection(stringConexao))
            {
                var adendo = con.Query<OportunidadeAdendo>("SELECT Id, TipoAdendo, OportunidadeId FROM CRM.TB_CRM_OPORTUNIDADE_ADENDOS WHERE Id = :AdendoId", parametros).FirstOrDefault();

                if (adendo != null)
                {
                    switch (adendo.TipoAdendo)
                    {
                        case 1:

                            parametros = new DynamicParameters();
                            parametros.Add(name: "AdendoId", value: adendo.Id, direction: ParameterDirection.Input);

                            var vendedorId = con.Query<int>("SELECT VendedorId FROM CRM.TB_CRM_ADENDO_VENDEDOR WHERE AdendoId = :AdendoId", parametros).FirstOrDefault();

                            if (Int32.TryParse(vendedorId.ToString(), out int resultadoVendedor))
                            {
                                if (resultadoVendedor > 0)
                                {
                                    parametros.Add(name: "VendedorId", value: resultadoVendedor, direction: ParameterDirection.Input);
                                    parametros.Add(name: "OportunidadeId", value: adendo.OportunidadeId, direction: ParameterDirection.Input);

                                    con.Execute(@"UPDATE CRM.TB_CRM_OPORTUNIDADES SET VendedorId = :VendedorId WHERE Id = :OportunidadeId", parametros);
                                }
                            }

                            break;

                        case 2:

                            parametros = new DynamicParameters();
                            parametros.Add(name: "AdendoId", value: adendo.Id, direction: ParameterDirection.Input);

                            var formaPagamento = con.Query<int>("SELECT FormaPagamento FROM CRM.TB_CRM_ADENDO_FORMA_PAGAMENTO WHERE AdendoId = :AdendoId", parametros).FirstOrDefault();

                            if (Int32.TryParse(formaPagamento.ToString(), out int resultadoFormaPagamento))
                            {
                                if (resultadoFormaPagamento > 0)
                                {
                                    parametros.Add(name: "FormaPagamento", value: resultadoFormaPagamento, direction: ParameterDirection.Input);
                                    parametros.Add(name: "OportunidadeId", value: adendo.OportunidadeId, direction: ParameterDirection.Input);

                                    con.Execute(@"UPDATE CRM.TB_CRM_OPORTUNIDADES SET FormaPagamento = :FormaPagamento WHERE Id = :OportunidadeId", parametros);
                                }
                            }

                            break;

                        case 3:

                            parametros = new DynamicParameters();
                            parametros.Add(name: "AdendoId", value: adendo.Id, direction: ParameterDirection.Input);
                            parametros.Add(name: "OportunidadeId", value: adendo.OportunidadeId, direction: ParameterDirection.Input);

                            con.Execute(@"
                                    INSERT INTO 
                                        CRM.TB_CRM_OPORTUNIDADE_CLIENTES 
                                            (
                                                Id, 
                                                Segmento, 
                                                ContaId, 
                                                OportunidadeId, 
                                                CriadoPor
                                            ) 
                                            SELECT 
                                                CRM.SEQ_CRM_OPORTUNIDADE_CLIENTES.NEXTVAL, 
                                                NVL(A.Segmento, 1) Segmento,
                                                A.SubClienteId, 
                                                B.OportunidadeId, 
                                                B.CriadoPor 
                                            FROM 
                                                CRM.TB_CRM_ADENDO_SUB_CLIENTE A
                                            INNER JOIN
                                                CRM.TB_CRM_OPORTUNIDADE_ADENDOS B ON A.AdendoId = B.Id
                                            WHERE 
                                                A.AdendoId = :AdendoId AND A.Acao = 1
                                            AND
                                                B.OportunidadeId = :OportunidadeId", parametros);
                            break;

                        case 4:

                            parametros = new DynamicParameters();
                            parametros.Add(name: "AdendoId", value: adendo.Id, direction: ParameterDirection.Input);
                            parametros.Add(name: "OportunidadeId", value: adendo.OportunidadeId, direction: ParameterDirection.Input);

                            con.Execute(@"
                                    DELETE FROM
                                        CRM.TB_CRM_OPORTUNIDADE_CLIENTES 
                                    WHERE 
                                        ContaId IN (                                            
                                                SELECT SubClienteId                                                
                                            FROM 
                                                CRM.TB_CRM_ADENDO_SUB_CLIENTE
                                            WHERE 
                                                AdendoId = :AdendoId AND Acao = 2
                                        ) AND OportunidadeId = :OportunidadeId", parametros);
                            break;

                        case 5:

                            parametros = new DynamicParameters();
                            parametros.Add(name: "AdendoId", value: adendo.Id, direction: ParameterDirection.Input);
                            parametros.Add(name: "OportunidadeId", value: adendo.OportunidadeId, direction: ParameterDirection.Input);

                            con.Execute(@"
                                    INSERT INTO 
                                        CRM.TB_CRM_OPORTUNIDADE_GRUPO_CNPJ
                                            (
                                                Id, 
                                                ContaId, 
                                                OportunidadeId, 
                                                CriadoPor
                                            ) 
                                            SELECT 
                                                CRM.SEQ_CRM_OPORTUNIDADE_GRUPOCNPJ.NEXTVAL, 
                                                A.GrupoCNPJId, 
                                                B.OportunidadeId, 
                                                B.CriadoPor 
                                        FROM 
                                            CRM.TB_CRM_ADENDO_GRUPO_CNPJ A
                                        INNER JOIN
                                            CRM.TB_CRM_OPORTUNIDADE_ADENDOS B ON A.AdendoId = B.Id
                                        WHERE 
                                            A.AdendoId = :AdendoId AND A.Acao = 1
                                        AND 
                                            B.OportunidadeId = :OportunidadeId", parametros);
                            break;

                        case 6:

                            parametros = new DynamicParameters();
                            parametros.Add(name: "AdendoId", value: adendo.Id, direction: ParameterDirection.Input);
                            parametros.Add(name: "OportunidadeId", value: adendo.OportunidadeId, direction: ParameterDirection.Input);

                            con.Execute(@"
                                    DELETE FROM
                                        CRM.TB_CRM_OPORTUNIDADE_GRUPO_CNPJ 
                                    WHERE 
                                        ContaId IN (                                            
                                                SELECT GrupoCnpjId                                                
                                            FROM 
                                                CRM.TB_CRM_ADENDO_GRUPO_CNPJ
                                            WHERE 
                                                AdendoId = :AdendoId AND Acao = 2
                                        ) AND OportunidadeId = :OportunidadeId", parametros);
                            break;
                        default:
                            break;
                    }
                }
            }
        }


        public OportunidadeAdendo ObterAdendoPorId(string id)
        {
            var parametros = new DynamicParameters();
            parametros.Add(name: "AdendoId", value: id, direction: ParameterDirection.Input);

            using (OracleConnection con = new OracleConnection(stringConexao))
            {
                return con.Query<OportunidadeAdendo>("SELECT Id, TipoAdendo, OportunidadeId FROM CRM.TB_CRM_OPORTUNIDADE_ADENDOS WHERE Id = :AdendoId", parametros).FirstOrDefault();                
            }
        }

        public OportunidadeFichaFaturamento ObterFichaFaturamentoPorId(string id)
        {
            var parametros = new DynamicParameters();
            parametros.Add(name: "FichaFaturamentoId", value: id, direction: ParameterDirection.Input);

            using (OracleConnection con = new OracleConnection(stringConexao))
            {
                return con.Query<OportunidadeFichaFaturamento>("SELECT Id, OportunidadeId, ContaId, RevisaoId FROM TB_CRM_OPORTUNIDADE_FICHA_FAT WHERE Id = :FichaFaturamentoId", parametros).FirstOrDefault();
            }
        }


        private void ObterNumeroDeTabela(string oportunidadeId, WorkflowService workflowService)
        {
            try
            {
                using (OracleConnection con = new OracleConnection(stringConexao))
                {
                    var parametros = new DynamicParameters();
                    parametros.Add(name: "Id", value: oportunidadeId, direction: ParameterDirection.Input);

                    var empresa = con.Query<int>($"SELECT NVL(MAX(EmpresaId),0) EmpresaId FROM CRM.TB_CRM_OPORTUNIDADES WHERE ID = :Id", parametros).FirstOrDefault();

                    if (empresa > 0)
                    {
                        var workflow = workflowService.ObterHistoricoWorkflow(Convert.ToInt32(oportunidadeId), 1, empresa);

                        if (workflow != null)
                        {
                            var ultimoAprovador = workflow.list
                                   .SelectMany(c => c.workFlows)
                                   .Select(c => c.ultimo_Aprovador_Usuario_Login)
                                   .FirstOrDefault();

                            if (ultimoAprovador == null)
                                return;

                            var comentario = workflow.list
                                .SelectMany(c => c.workFlows)
                                .Select(c => c.etapas
                                        .Select(f => f.aprovacoes
                                            .Where(g => g.usuario_Aprovador_Login == ultimoAprovador.ToString())
                                            .FirstOrDefault())
                                                .Select(d => d?.comentario)
                                                .FirstOrDefault())
                                            .FirstOrDefault();

                            if (comentario != null)
                            {
                                var ocorrencias = Regex.Matches(comentario.ToString(), @"(?<=\*)[^}]*(?=\*)");

                                if (ocorrencias.Count > 0)
                                {
                                    var capture = ocorrencias.Cast<Match>().FirstOrDefault();
                                    var tabela = capture.Value;

                                    parametros = new DynamicParameters();

                                    parametros.Add(name: "TabelaId", value: tabela, direction: ParameterDirection.Input);
                                    parametros.Add(name: "OportunidadeId", value: oportunidadeId, direction: ParameterDirection.Input);

                                    con.Execute(@"UPDATE CRM.TB_CRM_OPORTUNIDADES SET TabelaId = :TabelaId WHERE Id = :OportunidadeId", parametros);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Falha ao atualizar o ID da Tabela - " + ex.ToString());
            }
        }

        protected override void OnStop()
        {
            this.timer.Stop();
            this.timer = null;

            logger.Info($"{DateTime.Now} - Serviço parado...");
        }
    }


}
