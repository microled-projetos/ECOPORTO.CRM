using Dapper;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace WsConsultaSPC
{
    public class AnaliseCreditoRepositorio
    {
        public IEnumerable<PendenciaFinanceiraResponse> ObterPendenciasFinanceiras(string documento)
        {
            using (OracleConnection con = new OracleConnection(Parametros.StringConexao))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Documento", value: documento.Substring(0, 8), direction: ParameterDirection.Input);

                return con.Query<PendenciaFinanceiraResponse>(@"
                    SELECT 
                        NFE As NotaFiscal, 
                        Valor, 
                        Razao As Cliente, 
                        VENCIMENTO_ORIGINAL As Vencimento, 
                        Lote, 
                        Documento, 
                        Tipo 
                    FROM 
                        VW_CRM_PENDENCIAS_FINANCEIRAS 
                    WHERE 
                 SUBSTR(REPLACE(REPLACE(REPLACE(CGCCPF, '.',''), '/', ''), '-', ''), 0, 8) = 
                        SUBSTR(REPLACE(REPLACE(REPLACE(:Documento, '.',''), '/', ''), '-', ''), 0, 8)             
					ORDER BY
						VENCIMENTO_ORIGINAL DESC", parametros);
            }
        }

        public void GravarConsultaSpc(ConsultaSpcResponse consultaSpc, IEnumerable<Conta> contas)
        {
            using (OracleConnection con = new OracleConnection(Parametros.StringConexao))
            {
                con.Open();

                using (var transaction = con.BeginTransaction())
                {
                    var parametros = new DynamicParameters();

                    var sequencia = con.Query<int>("SELECT CRM.SEQ_CRM_SPC_CONSULTAS.NEXTVAL FROM DUAL").Single();

                    parametros.Add(name: "Sequencia", value: sequencia, direction: ParameterDirection.Input);
                    parametros.Add(name: "ContaId", value: consultaSpc.ContaId, direction: ParameterDirection.Input);
					parametros.Add(name: "StatusAnaliseDeCredito", value: consultaSpc.StatusAnaliseDeCredito, direction: ParameterDirection.Input);
					parametros.Add(name: "Protocolo", value: consultaSpc.Protocolo, direction: ParameterDirection.Input);
                    parametros.Add(name: "Validade", value: consultaSpc.Validade, direction: ParameterDirection.Input);
                    parametros.Add(name: "InadimplenteSpc", value: consultaSpc.InadimplenteSpc.ToInt(), direction: ParameterDirection.Input);
                    parametros.Add(name: "InadimplenteEcoporto", value: consultaSpc.InadimplenteEcoporto.ToInt(), direction: ParameterDirection.Input);
                    parametros.Add(name: "Quantidade", value: consultaSpc.Quantidade, direction: ParameterDirection.Input);
                    parametros.Add(name: "Tipo", value: consultaSpc.TipoPessoa, direction: ParameterDirection.Input);
                    parametros.Add(name: "RazaoSocial", value: consultaSpc.RazaoSocial, direction: ParameterDirection.Input);
                    parametros.Add(name: "Fundacao", value: consultaSpc.Fundacao, direction: ParameterDirection.Input);
                    parametros.Add(name: "Nome", value: consultaSpc.Nome, direction: ParameterDirection.Input);
                    parametros.Add(name: "DataNascimento", value: consultaSpc.DataNascimento, direction: ParameterDirection.Input);
                    parametros.Add(name: "CNPJ", value: consultaSpc.CNPJ, direction: ParameterDirection.Input);
                    parametros.Add(name: "CPF", value: consultaSpc.CPF, direction: ParameterDirection.Input);
                    parametros.Add(name: "Atividade", value: consultaSpc.Atividade, direction: ParameterDirection.Input);
                    parametros.Add(name: "Situacao", value: consultaSpc.Situacao, direction: ParameterDirection.Input);
                    parametros.Add(name: "Logradouro", value: consultaSpc.Logradouro, direction: ParameterDirection.Input);
                    parametros.Add(name: "Bairro", value: consultaSpc.Bairro, direction: ParameterDirection.Input);
                    parametros.Add(name: "Cidade", value: consultaSpc.Cidade, direction: ParameterDirection.Input);
                    parametros.Add(name: "Estado", value: consultaSpc.Estado, direction: ParameterDirection.Input);
                    parametros.Add(name: "CEP", value: consultaSpc.CEP, direction: ParameterDirection.Input);
                    parametros.Add(name: "TotalDividaSpc", value: consultaSpc.TotalDividaSpc, direction: ParameterDirection.Input);
                    parametros.Add(name: "TotalDividaEcoporto", value: consultaSpc.TotalDividaEcoporto, direction: ParameterDirection.Input);
                    parametros.Add(name: "ProtestoQuantidade", value: consultaSpc.ProtestoQuantidade, direction: ParameterDirection.Input);
                    parametros.Add(name: "ProtestoData", value: consultaSpc.ProtestoData, direction: ParameterDirection.Input);
                    parametros.Add(name: "ProtestoValorTotal", value: consultaSpc.ProtestoValorTotal, direction: ParameterDirection.Input);
                    parametros.Add(name: "AcaoQuantidade", value: consultaSpc.AcaoQuantidade, direction: ParameterDirection.Input);
                    parametros.Add(name: "AcaoData", value: consultaSpc.AcaoData, direction: ParameterDirection.Input);
                    parametros.Add(name: "AcaoValorTotal", value: consultaSpc.AcaoValorTotal, direction: ParameterDirection.Input);
                    parametros.Add(name: "PendenciaFinancQuantidade", value: consultaSpc.PendenciaFinancQuantidade, direction: ParameterDirection.Input);
                    parametros.Add(name: "PendenciaFinancData", value: consultaSpc.PendenciaFinancData, direction: ParameterDirection.Input);
                    parametros.Add(name: "PendenciaFinancValorTotal", value: consultaSpc.PendenciaFinancValorTotal, direction: ParameterDirection.Input);
                    parametros.Add(name: "ParticipFalenciaQuantidade", value: consultaSpc.ParticipFalenciaQuantidade, direction: ParameterDirection.Input);
                    parametros.Add(name: "ParticipFalenciaData", value: consultaSpc.ParticipFalenciaData, direction: ParameterDirection.Input);
                    parametros.Add(name: "ParticipFalenciaValorTotal", value: consultaSpc.ParticipFalenciaValorTotal, direction: ParameterDirection.Input);
                    parametros.Add(name: "SpcQuantidade", value: consultaSpc.SpcQuantidade, direction: ParameterDirection.Input);
                    parametros.Add(name: "SpcData", value: consultaSpc.SpcData, direction: ParameterDirection.Input);
                    parametros.Add(name: "SpcValorTotal", value: consultaSpc.SpcValorTotal, direction: ParameterDirection.Input);
                    parametros.Add(name: "ChequeSFQuantidade", value: consultaSpc.ChequeSFQuantidade, direction: ParameterDirection.Input);
                    parametros.Add(name: "ChequeSFData", value: consultaSpc.ChequeSFData, direction: ParameterDirection.Input);
                    parametros.Add(name: "ChequeSFValorTotal", value: consultaSpc.ChequeSFValorTotal, direction: ParameterDirection.Input);
                    parametros.Add(name: "ChequeSFCCFQuantidade", value: consultaSpc.ChequeSFCCFQuantidade, direction: ParameterDirection.Input);
                    parametros.Add(name: "ChequeSFCCFData", value: consultaSpc.ChequeSFCCFData, direction: ParameterDirection.Input);
                    parametros.Add(name: "ChequeSFCCFValorTotal", value: consultaSpc.ChequeSFCCFValorTotal, direction: ParameterDirection.Input);
                    parametros.Add(name: "ChequeLojistaQuantidade", value: consultaSpc.ChequeLojistaQuantidade, direction: ParameterDirection.Input);
                    parametros.Add(name: "ChequeLojistaData", value: consultaSpc.ChequeLojistaData, direction: ParameterDirection.Input);
                    parametros.Add(name: "ChequeLojistaValorTotal", value: consultaSpc.ChequeLojistaValorTotal, direction: ParameterDirection.Input);
                    parametros.Add(name: "ChequeCOOutrasQuantidade", value: consultaSpc.ChequeCOOutrasQuantidade, direction: ParameterDirection.Input);
                    parametros.Add(name: "ChequeCOOutrasData", value: consultaSpc.ChequeCOOutrasData, direction: ParameterDirection.Input);
                    parametros.Add(name: "ChequeCOOutrasValorTotal", value: consultaSpc.ChequeCOOutrasValorTotal, direction: ParameterDirection.Input);
                    parametros.Add(name: "ConsultaRealizadaQuantidade", value: consultaSpc.ConsultaRealizadaQuantidade, direction: ParameterDirection.Input);
                    parametros.Add(name: "ConsultaRealizadaData", value: consultaSpc.ConsultaRealizadaData, direction: ParameterDirection.Input);
                    parametros.Add(name: "ConsultaRealizadaValorTotal", value: consultaSpc.ConsultaRealizadaValorTotal, direction: ParameterDirection.Input);
                    parametros.Add(name: "AlertaDocQuantidade", value: consultaSpc.AlertaDocQuantidade, direction: ParameterDirection.Input);
                    parametros.Add(name: "AlertaDocData", value: consultaSpc.AlertaDocData, direction: ParameterDirection.Input);
                    parametros.Add(name: "AlertaDocValorTotal", value: consultaSpc.AlertaDocValorTotal, direction: ParameterDirection.Input);
                    parametros.Add(name: "CreditoConcQuantidade", value: consultaSpc.CreditoConcQuantidade, direction: ParameterDirection.Input);
                    parametros.Add(name: "CreditoConcData", value: consultaSpc.CreditoConcData, direction: ParameterDirection.Input);
                    parametros.Add(name: "CreditoConcValorTotal", value: consultaSpc.CreditoConcValorTotal, direction: ParameterDirection.Input);
                    parametros.Add(name: "ContraOrdemQuantidade", value: consultaSpc.ContraOrdemQuantidade, direction: ParameterDirection.Input);
                    parametros.Add(name: "ContraOrdemData", value: consultaSpc.ContraOrdemData, direction: ParameterDirection.Input);
                    parametros.Add(name: "ContraOrdemValorTotal", value: consultaSpc.ContraOrdemValorTotal, direction: ParameterDirection.Input);
                    parametros.Add(name: "ContraOrdemDFQuantidade", value: consultaSpc.ContraOrdemDFQuantidade, direction: ParameterDirection.Input);
                    parametros.Add(name: "ContraOrdemDFData", value: consultaSpc.ContraOrdemDFData, direction: ParameterDirection.Input);
                    parametros.Add(name: "ContraOrdemDFValorTotal", value: consultaSpc.ContraOrdemDFValorTotal, direction: ParameterDirection.Input);
                    parametros.Add(name: "UsuarioId", value: consultaSpc.UsuarioId, direction: ParameterDirection.Input);

                    con.Execute("DELETE FROM CRM.TB_CRM_SPC_CONSULTA_FILIAL WHERE ConsultaId IN (SELECT Id FROM CRM.TB_CRM_SPC_CONSULTAS WHERE ContaId = :ContaId)", parametros, transaction);
                    con.Execute("DELETE FROM CRM.TB_CRM_SPC_CCF WHERE ConsultaId IN (SELECT Id FROM CRM.TB_CRM_SPC_CONSULTAS WHERE ContaId = :ContaId)", parametros, transaction);
                    con.Execute("DELETE FROM CRM.TB_CRM_SPC_ALERTAS_DOCS WHERE ConsultaId IN (SELECT Id FROM CRM.TB_CRM_SPC_CONSULTAS WHERE ContaId = :ContaId)", parametros, transaction);
                    con.Execute("DELETE FROM CRM.TB_CRM_SPC_HIST_CONSULTAS WHERE ConsultaId IN (SELECT Id FROM CRM.TB_CRM_SPC_CONSULTAS WHERE ContaId = :ContaId)", parametros, transaction);
                    con.Execute("DELETE FROM CRM.TB_CRM_SPC_CONTRA_ORDEM_DOC WHERE ConsultaId IN (SELECT Id FROM CRM.TB_CRM_SPC_CONSULTAS WHERE ContaId = :ContaId)", parametros, transaction);
                    con.Execute("DELETE FROM CRM.TB_CRM_SPC_DETALHES_CH_LOJISTA WHERE ConsultaId IN (SELECT Id FROM CRM.TB_CRM_SPC_CONSULTAS WHERE ContaId = :ContaId)", parametros, transaction);
                    con.Execute("DELETE FROM CRM.TB_CRM_SPC_DETALHES_PEND_FIN WHERE ConsultaId IN (SELECT Id FROM CRM.TB_CRM_SPC_CONSULTAS WHERE ContaId = :ContaId)", parametros, transaction);
                    con.Execute("DELETE FROM CRM.TB_CRM_SPC_DETALHES_SPC WHERE ConsultaId IN (SELECT Id FROM CRM.TB_CRM_SPC_CONSULTAS WHERE ContaId = :ContaId)", parametros, transaction);
                    con.Execute("DELETE FROM CRM.TB_CRM_SPC_CONSULTAS WHERE ContaId = :ContaId", parametros, transaction);

                    con.Execute(@"
							INSERT INTO
								CRM.TB_CRM_SPC_CONSULTAS
									(
										ID,
										CONTAID,
										STATUSANALISEDECREDITO,
										DATACONSULTA,
										VALIDADE,
										PROTOCOLO,
										INADIMPLENTE_SPC,
										INADIMPLENTE_ECOPORTO,
										TIPO,
										RAZAOSOCIAL,
										FUNDACAO,
										NOME,
										DATANASCIMENTO,
										CNPJ,
										CPF,
										ATIVIDADE,
										SITUACAO,
										LOGRADOURO,
										BAIRRO,
										CIDADE,
										ESTADO,
										CEP,
										TOTALDIVIDA_SPC,
										TOTALDIVIDA_ECOPORTO,
										PROTESTO_QTDE,
										PROTESTO_DATA,
										PROTESTO_VALOR_TOTAL,
										ACAO_QTDE,
										ACAO_DATA,
										ACAO_VALOR_TOTAL,
										PEND_FINANC_QTDE,
										PEND_FINANC_DATA,
										PEND_FINANC_TOTAL,
										PART_FALENC_QTDE,
										PART_FALENC_DATA,
										PART_FALENC_VALOR_TOTAL,
										SPC_QTDE,
										SPC_DATA,
										SPC_TOTAL,
										CHEQUE_SF_QTDE,
										CHEQUE_SF_DATA,
										CHEQUE_SF_TOTAL,
										CHEQUE_SF_CCF_QTDE,
										CHEQUE_SF_CCF_DATA,
										CHEQUE_SF_CCF_TOTAL,
										CHEQUE_LOJISTA_QTDE,
										CHEQUE_LOJISTA_DATA,
										CHEQUE_LOJISTA_TOTAL,
										CHEQUE_CO_OUTRAS_QTDE,
										CHEQUE_CO_OUTRAS_DATA,
										CHEQUE_CO_OUTRAS_TOTAL,
										CONSULTA_REALIZADA_QTDE,
										CONSULTA_REALIZADA_DATA,
										CONSULTA_REALIZADA_TOTAL,
										ALERTA_DOC_QTDE,
										ALERTA_DOC_DATA,
										ALERTA_DOC_TOTAL,
										CREDITO_CONCEDIDO_QTDE,
										CREDITO_CONCEDIDO_DATA,
										CREDITO_CONCEDIDO_TOTAL,
										CONTRA_ORDEM_QTDE,
										CONTRA_ORDEM_DATA,
										CONTRA_ORDEM_TOTAL,
										CONTRA_ORDEM_DF_QTDE,
										CONTRA_ORDEM_DF_DATA,
										CONTRA_ORDEM_DF_TOTAL,
										USUARIOID
									) VALUES (
										:Sequencia,
										:ContaId,
										:StatusAnaliseDeCredito,
										SYSDATE,
										:Validade,
										:Protocolo,
										:InadimplenteSpc,
										:InadimplenteEcoporto,
										:Tipo,
										:RazaoSocial,
										:Fundacao,
										:Nome,
										:DataNascimento,
										:CNPJ,
										:CPF,
										:Atividade,
										:Situacao,
										:Logradouro,
										:Bairro,
										:Cidade,
										:Estado,
										:CEP,
										:TotalDividaSpc,
										:TotalDividaEcoporto,
										:ProtestoQuantidade,
										:ProtestoData,
										:ProtestoValorTotal,
										:AcaoQuantidade,
										:AcaoData,
										:AcaoValorTotal,
										:PendenciaFinancQuantidade,
										:PendenciaFinancData,
										:PendenciaFinancValorTotal,
										:ParticipFalenciaQuantidade,
										:ParticipFalenciaData,
										:ParticipFalenciaValorTotal,
										:SpcQuantidade,
										:SpcData,
										:SpcValorTotal,
										:ChequeSFQuantidade,
										:ChequeSFData,
										:ChequeSFValorTotal,
										:ChequeSFCCFQuantidade,
										:ChequeSFCCFData,
										:ChequeSFCCFValorTotal,
										:ChequeLojistaQuantidade,
										:ChequeLojistaData,
										:ChequeLojistaValorTotal,
										:ChequeCOOutrasQuantidade,
										:ChequeCOOutrasData,
										:ChequeCOOutrasValorTotal,
										:ConsultaRealizadaQuantidade,
										:ConsultaRealizadaData,
										:ConsultaRealizadaValorTotal,
										:AlertaDocQuantidade,
										:AlertaDocData,
										:AlertaDocValorTotal,
										:CreditoConcQuantidade,
										:CreditoConcData,
										:CreditoConcValorTotal,
										:ContraOrdemQuantidade,
										:ContraOrdemData,
										:ContraOrdemValorTotal,
										:ContraOrdemDFQuantidade,
										:ContraOrdemDFData,
										:ContraOrdemDFValorTotal,
										:UsuarioId
									)", parametros, transaction);

                    foreach (var item in consultaSpc.DetalhesSpc)
                    {
                        parametros = new DynamicParameters();

                        parametros.Add(name: "Sequencia", value: sequencia, direction: ParameterDirection.Input);
                        parametros.Add(name: "Associado", value: item.Associado, direction: ParameterDirection.Input);
                        parametros.Add(name: "Inclusao", value: item.Inclusao, direction: ParameterDirection.Input);
                        parametros.Add(name: "Vencimento", value: item.Vencimento, direction: ParameterDirection.Input);
                        parametros.Add(name: "Entidade", value: item.Entidade, direction: ParameterDirection.Input);
                        parametros.Add(name: "Contrato", value: item.Contrato, direction: ParameterDirection.Input);
                        parametros.Add(name: "Valor", value: item.Valor, direction: ParameterDirection.Input);

                        con.Execute(@"
								INSERT INTO 
									CRM.TB_CRM_SPC_DETALHES_SPC 
									(
										Id,
										ConsultaId,
										Associado,
										Inclusao,
										Vencimento,
										Entidade,
										Contrato,
										Valor
									) VALUES (
										CRM.SEQ_CRM_SPC_DETALHES_SPC.NEXTVAL,
										:Sequencia,
										:Associado,
										:Inclusao,
										:Vencimento,
										:Entidade,
										:Contrato,
										:Valor
									)", parametros, transaction);
                    }

                    foreach (var item in consultaSpc.DetalhesPendenciasFinanceiras)
                    {
                        parametros = new DynamicParameters();

                        parametros.Add(name: "Sequencia", value: sequencia, direction: ParameterDirection.Input);
                        parametros.Add(name: "Titulo", value: item.Titulo, direction: ParameterDirection.Input);
                        parametros.Add(name: "Avalista", value: item.Avalista.ToInt(), direction: ParameterDirection.Input);
                        parametros.Add(name: "Contrato", value: item.Contrato, direction: ParameterDirection.Input);
                        parametros.Add(name: "Ocorrencia", value: item.Ocorrencia, direction: ParameterDirection.Input);
                        parametros.Add(name: "Filial", value: item.Filial, direction: ParameterDirection.Input);
                        parametros.Add(name: "Origem", value: item.Origem, direction: ParameterDirection.Input);
                        parametros.Add(name: "Moeda", value: item.Moeda, direction: ParameterDirection.Input);
                        parametros.Add(name: "Natureza", value: item.Natureza, direction: ParameterDirection.Input);
                        parametros.Add(name: "Cidade", value: item.Cidade, direction: ParameterDirection.Input);
                        parametros.Add(name: "UF", value: item.UF, direction: ParameterDirection.Input);
                        parametros.Add(name: "Valor", value: item.Valor, direction: ParameterDirection.Input);

                        con.Execute(@"
								INSERT INTO 
									CRM.TB_CRM_SPC_DETALHES_PEND_FIN 
									(
										Id,
										ConsultaId,
										Titulo,
										Avalista,
										Contrato,
										Ocorrencia,
										Filial,
										Origem,
										Moeda,
										Natureza,
										Cidade,
										UF,
										Valor
									) VALUES (
										CRM.SEQ_CRM_SPC_DETALHES_PEND_FIN.NEXTVAL,
										:Sequencia,
										:Titulo,
										:Avalista,
										:Contrato,
										:Ocorrencia,
										:Filial,
										:Origem,
										:Moeda,
										:Natureza,
										:Cidade,
										:UF,
										:Valor
									)", parametros, transaction);
                    }

                    foreach (var item in consultaSpc.DetalhesChequesLojistas)
                    {
                        parametros = new DynamicParameters();

                        parametros.Add(name: "Sequencia", value: sequencia, direction: ParameterDirection.Input);
                        parametros.Add(name: "Associado", value: item.Associado, direction: ParameterDirection.Input);
                        parametros.Add(name: "Entidade", value: item.Entidade, direction: ParameterDirection.Input);
                        parametros.Add(name: "Inclusao", value: item.Inclusao, direction: ParameterDirection.Input);
                        parametros.Add(name: "Descricao", value: item.Descricao, direction: ParameterDirection.Input);
                        parametros.Add(name: "Emissao", value: item.ChequeEmissao, direction: ParameterDirection.Input);
                        parametros.Add(name: "Valor", value: item.ChequeValor, direction: ParameterDirection.Input);
                        parametros.Add(name: "Cidade", value: item.CidadeAssociado, direction: ParameterDirection.Input);

                        con.Execute(@"
								INSERT INTO 
									CRM.TB_CRM_SPC_DETALHES_CH_LOJISTA
									(
										Id,
										ConsultaId,
										Associado,
										Entidade,
										Inclusao,
										Descricao,
										Emissao,
										Valor,
										Cidade
									) VALUES (
										CRM.SEQ_CRM_SPC_DETALHES_CH_LOJ.NEXTVAL,
										:Sequencia,
										:Associado,
										:Entidade,
										:Inclusao,
										:Descricao,
										:Emissao,
										:Valor,
										:Cidade
									)", parametros, transaction);
                    }

                    foreach (var item in consultaSpc.DetalhesContraOrdemDocumentoDiferente)
                    {
                        parametros = new DynamicParameters();

                        parametros.Add(name: "Sequencia", value: sequencia, direction: ParameterDirection.Input);
                        parametros.Add(name: "Documento", value: item.Documento, direction: ParameterDirection.Input);
                        parametros.Add(name: "Inclusao", value: item.Inclusao, direction: ParameterDirection.Input);
                        parametros.Add(name: "Ocorrencia", value: item.Ocorrencia, direction: ParameterDirection.Input);
                        parametros.Add(name: "Origem", value: item.Origem, direction: ParameterDirection.Input);
                        parametros.Add(name: "Informante", value: item.Informante, direction: ParameterDirection.Input);
                        parametros.Add(name: "Descricao", value: item.Descricao, direction: ParameterDirection.Input);

                        con.Execute(@"
								INSERT INTO 
									CRM.TB_CRM_SPC_CONTRA_ORDEM_DOC 
									(
										Id,
										ConsultaId,
										Documento,
										Inclusao,
										Ocorrencia,
										Origem,
										Informante,
										Descricao
									) VALUES (
										CRM.SEQ_CRM_SPC_CONTRA_ORDEM_DOC.NEXTVAL,
										:Sequencia,
										:Documento,
										:Inclusao,
										:Ocorrencia,
										:Origem,
										:Informante,
										:Descricao
									)", parametros, transaction);
                    }

                    foreach (var item in consultaSpc.DetalhesConsultasRealizadas)
                    {
                        parametros = new DynamicParameters();

                        parametros.Add(name: "Sequencia", value: sequencia, direction: ParameterDirection.Input);
                        parametros.Add(name: "Associado", value: item.Associado, direction: ParameterDirection.Input);
                        parametros.Add(name: "DataConsulta", value: item.DataConsulta, direction: ParameterDirection.Input);
                        parametros.Add(name: "Entidade", value: item.Entidade, direction: ParameterDirection.Input);
                        parametros.Add(name: "Cidade", value: item.Cidade, direction: ParameterDirection.Input);
                        parametros.Add(name: "Estado", value: item.Estado, direction: ParameterDirection.Input);

                        con.Execute(@"
								INSERT INTO 
									CRM.TB_CRM_SPC_HIST_CONSULTAS 
									(
										Id,
										ConsultaId,
										Associado,
										DataConsulta,
										Entidade,
										Cidade,
										Estado
									) VALUES (
										CRM.SEQ_CRM_SPC_DETALHES_SPC.NEXTVAL,
										:Sequencia,
										:Associado,
										:DataConsulta,
										:Entidade,
										:Cidade,
										:Estado
									)", parametros, transaction);
                    }

                    foreach (var item in consultaSpc.DetalhesAlertasDocumentos)
                    {
                        parametros = new DynamicParameters();

                        parametros.Add(name: "Sequencia", value: sequencia, direction: ParameterDirection.Input);
                        parametros.Add(name: "Inclusao", value: item.Inclusao, direction: ParameterDirection.Input);
                        parametros.Add(name: "Ocorrencia", value: item.Ocorrencia, direction: ParameterDirection.Input);
                        parametros.Add(name: "Entidade", value: item.Entidade, direction: ParameterDirection.Input);
                        parametros.Add(name: "Motivo", value: item.Motivo, direction: ParameterDirection.Input);
                        parametros.Add(name: "Tipos", value: item.Tipos, direction: ParameterDirection.Input);

                        con.Execute(@"
								INSERT INTO 
									CRM.TB_CRM_SPC_ALERTAS_DOCS 
									(
										Id,
										ConsultaId,
										Inclusao,
										Ocorrencia,
										Entidade,
										Motivo,
										Tipos
									) VALUES (
										CRM.SEQ_CRM_SPC_ALERTAS_DOCS.NEXTVAL,
										:Sequencia,
										:Inclusao,
										:Ocorrencia,
										:Entidade,
										:Motivo,
										:Tipos
									)", parametros, transaction);
                    }

                    foreach (var item in consultaSpc.DetalhesCCF)
                    {
                        parametros = new DynamicParameters();

                        parametros.Add(name: "Sequencia", value: sequencia, direction: ParameterDirection.Input);
                        parametros.Add(name: "Origem", value: item.Origem, direction: ParameterDirection.Input);
                        parametros.Add(name: "UltimoCheque", value: item.DataUltimoCheque, direction: ParameterDirection.Input);
                        parametros.Add(name: "Quantidade", value: item.Quantidade, direction: ParameterDirection.Input);
                        parametros.Add(name: "Motivo", value: item.Motivo, direction: ParameterDirection.Input);

                        con.Execute(@"
								INSERT INTO 
									CRM.TB_CRM_SPC_CCF 
									(
										Id,
										ConsultaId,
										Origem,
										UltimoCheque,
										Quantidade,
										Motivo
									) VALUES (
										CRM.SEQ_CRM_SPC_CCF.NEXTVAL,
										:Sequencia,
										:Origem,
										:UltimoCheque,
										:Quantidade,
										:Motivo
									)", parametros, transaction);
                    }

                    foreach (var conta in contas)
                    {
                        parametros = new DynamicParameters();

                        parametros.Add(name: "ConsultaId", value: sequencia, direction: ParameterDirection.Input);
                        parametros.Add(name: "ContaId", value: conta.Id, direction: ParameterDirection.Input);

                        con.Execute("INSERT INTO CRM.TB_CRM_SPC_CONSULTA_FILIAL (Id, ConsultaId, ContaId) VALUES (CRM.SEQ_CRM_SPC_CCF.NEXTVAL, :ConsultaId, :ContaId)", parametros, transaction);
                    }

                    transaction.Commit();
                }
            }
        }

        public ConsultaSpcResponse ObterConsultaSpc(int contaId)
        {
            using (OracleConnection con = new OracleConnection(Parametros.StringConexao))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "ContaId", value: contaId, direction: ParameterDirection.Input);

                return con.Query<ConsultaSpcResponse>(@"
					SELECT
						ID,
						CONTAID,
						DATACONSULTA,
						STATUSANALISEDECREDITO,
						VALIDADE,
						PROTOCOLO,
						INADIMPLENTE_SPC As InadimplenteSpc,
						INADIMPLENTE_ECOPORTO As InadimplenteEcoporto,
						QUANTIDADE,
						TIPO As TipoPessoa,
						RAZAOSOCIAL,
						FUNDACAO,
						NOME,
						DATANASCIMENTO,
						CNPJ,
						CPF,
						ATIVIDADE,
						SITUACAO,
						LOGRADOURO,
						BAIRRO,
						CIDADE,
						ESTADO,
						CEP,
						TotalDivida_SPC As TotalDividaSpc,
						TotalDivida_Ecoporto As TotalDividaEcoporto,
						PROTESTO_QTDE As ProtestoQuantidade,
						PROTESTO_DATA As ProtestoData,
						PROTESTO_VALOR_TOTAL As ProtestoValorTotal,
						ACAO_QTDE AS AcaoQuantidade,
						ACAO_DATA AS AcaoData,
						ACAO_VALOR_TOTAL AS AcaoValorTotal,
						PEND_FINANC_QTDE AS PendenciaFinancQuantidade,
						PEND_FINANC_DATA AS PendenciaFinancData,
						PEND_FINANC_TOTAL AS PendenciaFinancValorTotal,
						PART_FALENC_QTDE AS ParticipFalenciaQuantidade,
						PART_FALENC_DATA AS ParticipFalenciaData,
						PART_FALENC_VALOR_TOTAL AS ParticipFalenciaValorTotal,
						SPC_QTDE AS SpcQuantidade,
						SPC_DATA AS SpcData,
						SPC_TOTAL AS SpcValorTotal,
						CHEQUE_SF_QTDE AS ChequeSFQuantidade,
						CHEQUE_SF_DATA AS ChequeSFData,
						CHEQUE_SF_TOTAL AS ChequeSFValorTotal,
						CHEQUE_SF_CCF_QTDE AS ChequeSFCCFQuantidade,
						CHEQUE_SF_CCF_DATA AS ChequeSFCCFData,
						CHEQUE_SF_CCF_TOTAL AS ChequeSFCCFValorTotal,
						CHEQUE_LOJISTA_QTDE AS ChequeLojistaQuantidade,
						CHEQUE_LOJISTA_DATA AS ChequeLojistaData,
						CHEQUE_LOJISTA_TOTAL AS ChequeLojistaValorTotal,
						CHEQUE_CO_OUTRAS_QTDE AS ChequeCOOutrasQuantidade,
						CHEQUE_CO_OUTRAS_DATA AS ChequeCOOutrasData,
						CHEQUE_CO_OUTRAS_TOTAL AS ChequeCOOutrasValorTotal,
						CONSULTA_REALIZADA_QTDE AS ConsultaRealizadaQuantidade,
						CONSULTA_REALIZADA_DATA AS ConsultaRealizadaData,
						CONSULTA_REALIZADA_TOTAL AS ConsultaRealizadaValorTotal,
						ALERTA_DOC_QTDE AS AlertaDocQuantidade,
						ALERTA_DOC_DATA AS AlertaDocData,
						ALERTA_DOC_TOTAL AS AlertaDocValorTotal,
						CREDITO_CONCEDIDO_QTDE AS CreditoConcQuantidade,
						CREDITO_CONCEDIDO_DATA AS CreditoConcData,
						CREDITO_CONCEDIDO_TOTAL AS CreditoConcValorTotal,
						CONTRA_ORDEM_QTDE AS ContraOrdemQuantidade,
						CONTRA_ORDEM_DATA AS ContraOrdemData,
						CONTRA_ORDEM_TOTAL AS ContraOrdemValorTotal,
						CONTRA_ORDEM_DF_QTDE AS ContraOrdemDFQuantidade,
						CONTRA_ORDEM_DF_DATA AS ContraOrdemDFData,
						CONTRA_ORDEM_DF_TOTAL AS ContraOrdemDFValorTotal
					FROM
						CRM.TB_CRM_SPC_CONSULTAS
					WHERE
						ContaId=:ContaId ", parametros).FirstOrDefault();
            }
        }

        public IEnumerable<DetalhesSpcDTO> ObterDetalhesSpc(int consultaId)
        {
            using (OracleConnection con = new OracleConnection(Parametros.StringConexao))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "ConsultaId", value: consultaId, direction: ParameterDirection.Input);

                return con.Query<DetalhesSpcDTO>(@"
					SELECT
						Id,
						ConsultaId,
						Associado,
						Inclusao,
						Vencimento,
						Entidade,
						Contrato,
						Valor
					FROM
						CRM.TB_CRM_SPC_DETALHES_SPC
					WHERE
						ConsultaId = :ConsultaId", parametros);
            }
        }

        public IEnumerable<DetalhesPendenciaFinanceiraDTO> ObterDetalhesPendenciasFinanceiras(int consultaId)
        {
            using (OracleConnection con = new OracleConnection(Parametros.StringConexao))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "ConsultaId", value: consultaId, direction: ParameterDirection.Input);

                return con.Query<DetalhesPendenciaFinanceiraDTO>(@"
					SELECT
						Id,
						ConsultaId,
						Titulo,
						Avalista,
						Contrato,
						Ocorrencia,
						Filial,
						Origem,
						Moeda,
						Natureza,
						Cidade,
						UF,
						Valor
					FROM
						CRM.TB_CRM_SPC_DETALHES_PEND_FIN 
					WHERE
						ConsultaId = :ConsultaId", parametros);
            }
        }

        public IEnumerable<DetalhesChequeLojistaDTO> ObterDetalhesChequesLojistas(int consultaId)
        {
            using (OracleConnection con = new OracleConnection(Parametros.StringConexao))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "ConsultaId", value: consultaId, direction: ParameterDirection.Input);

                return con.Query<DetalhesChequeLojistaDTO>(@"
					SELECT
						Id,
						ConsultaId,
						Associado,
						Entidade,
						Inclusao,
						Descricao,
						Emissao,
						Valor,
						Cidade
					FROM
						CRM.TB_CRM_SPC_DETALHES_CH_LOJISTA
					WHERE
						ConsultaId = :ConsultaId", parametros);
            }
        }

        public IEnumerable<ContraOrdemDocumentoDiferenteDTO> ObterDetalhesContraOrdemDocumentoDiferente(int consultaId)
        {
            using (OracleConnection con = new OracleConnection(Parametros.StringConexao))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "ConsultaId", value: consultaId, direction: ParameterDirection.Input);

                return con.Query<ContraOrdemDocumentoDiferenteDTO>(@"
					SELECT
						Id,
						ConsultaId,
						Documento,
						Inclusao,
						Ocorrencia,
						Origem,
						Informante,
						Descricao
					FROM
						CRM.TB_CRM_SPC_CONTRA_ORDEM_DOC
					WHERE
						ConsultaId = :ConsultaId", parametros);
            }
        }

        public IEnumerable<ConsultaRealizadaDTO> ObterDetalhesHistoricoConsultas(int consultaId)
        {
            using (OracleConnection con = new OracleConnection(Parametros.StringConexao))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "ConsultaId", value: consultaId, direction: ParameterDirection.Input);

                return con.Query<ConsultaRealizadaDTO>(@"
					SELECT
						Id,
						ConsultaId,
						Associado,
						DataConsulta,
						Entidade,
						Cidade,
						Estado
					FROM
						CRM.TB_CRM_SPC_HIST_CONSULTAS
					WHERE
						ConsultaId = :ConsultaId", parametros);
            }
        }

        public IEnumerable<AlertaDocumentosDTO> ObterDetalhesAlertasDocumentos(int consultaId)
        {
            using (OracleConnection con = new OracleConnection(Parametros.StringConexao))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "ConsultaId", value: consultaId, direction: ParameterDirection.Input);

                return con.Query<AlertaDocumentosDTO>(@"
					SELECT
						Id,
						ConsultaId,
						Inclusao,
						Ocorrencia,
						Entidade,
						Motivo,
						Tipos
					FROM
						CRM.TB_CRM_SPC_ALERTAS_DOCS
					WHERE
						ConsultaId = :ConsultaId", parametros);
            }
        }

        public IEnumerable<CCFDetalhesDTO> ObterDetalhesCCF(int consultaId)
        {
            using (OracleConnection con = new OracleConnection(Parametros.StringConexao))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "ConsultaId", value: consultaId, direction: ParameterDirection.Input);

                return con.Query<CCFDetalhesDTO>(@"
					SELECT
						Id,
						ConsultaId,
						Origem,
						UltimoCheque,
						Quantidade,
						Motivo
					FROM
						CRM.TB_CRM_SPC_CCF
					WHERE
						ConsultaId = :ConsultaId", parametros);
            }
        }

        public void SolicitarLimiteDeCredito(LimiteCreditoSpcDTO limiteCredito)
        {
            using (OracleConnection con = new OracleConnection(Parametros.StringConexao))
            {
                var parametros = new DynamicParameters();

                parametros.Add(name: "ContaId", value: limiteCredito.ContaId, direction: ParameterDirection.Input);
                parametros.Add(name: "CondicaoPagamentoId", value: limiteCredito.CondicaoPagamentoId, direction: ParameterDirection.Input);
                parametros.Add(name: "LimiteCredito", value: limiteCredito.LimiteCredito, direction: ParameterDirection.Input);
                parametros.Add(name: "Observacoes", value: limiteCredito.Observacoes, direction: ParameterDirection.Input);
                parametros.Add(name: "StatusLimiteCredito", value: limiteCredito.StatusLimiteCredito, direction: ParameterDirection.Input);

                con.Execute("INSERT INTO CRM.TB_CRM_SPC_LIMITE_CREDITO (Id, ContaId, CondicaoPagamentoId, LimiteCredito, Observacoes, StatusLimiteCredito) VALUES (CRM.SEQ_CRM_SPC_LIMITE_CREDITO.NEXTVAL, :ContaId, :CondicaoPagamentoId, :LimiteCredito, :Observacoes, :StatusLimiteCredito)", parametros);
            }
        }

        public IEnumerable<LimiteCreditoSpcDTO> ObterSolicitacoesLimiteDeCredito(int contaId)
        {
            using (OracleConnection con = new OracleConnection(Parametros.StringConexao))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "ContaId", value: contaId, direction: ParameterDirection.Input);

                return con.Query<LimiteCreditoSpcDTO>(@"
					SELECT 
						A.Id, 
						A.ContaId, 
						A.CondicaoPagamentoId, 
						B.DESCPG As CondicaoPagamentoDescricao,
						A.LimiteCredito, 
						A.Observacoes,
						A.StatusLimiteCredito
					FROM 
						CRM.TB_CRM_SPC_LIMITE_CREDITO A 
					INNER JOIN
						FATURA.TB_COND_PGTO B ON A.CondicaoPagamentoId = B.CODCPG
					WHERE 
						A.ContaId = :ContaId", parametros);
            }
        }

        public LimiteCreditoSpcDTO ObterLimiteDeCreditoPorId(int id)
        {
            using (OracleConnection con = new OracleConnection(Parametros.StringConexao))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Id", value: id, direction: ParameterDirection.Input);

                return con.Query<LimiteCreditoSpcDTO>(@"
					SELECT 
						A.Id, 
						A.ContaId, 
						A.CondicaoPagamentoId, 
						B.DESCPG As CondicaoPagamentoDescricao,
						A.LimiteCredito, 
						A.Observacoes 
					FROM 
						CRM.TB_CRM_SPC_LIMITE_CREDITO A 
					INNER JOIN
						FATURA.TB_COND_PGTO B ON A.CondicaoPagamentoId = B.CODCPG
					WHERE 
						A.Id = :Id", parametros).FirstOrDefault();
            }
        }

		public void ExcluirLimiteDeCredito(int id)
		{
			using (OracleConnection con = new OracleConnection(Parametros.StringConexao))
			{
				var parametros = new DynamicParameters();
				parametros.Add(name: "Id", value: id, direction: ParameterDirection.Input);

				con.Execute(@"DELETE FROM CRM.TB_CRM_SPC_LIMITE_CREDITO WHERE Id = :Id", parametros);
			}
		}
		public void AtualizarSPC(int id)
		{
			using (OracleConnection con = new OracleConnection(Parametros.StringConexao))
			{
				var parametros = new DynamicParameters();
				parametros.Add(name: "Id", value: id, direction: ParameterDirection.Input);

				con.Execute(@"DELETE FROM CRM.TB_CRM_SPC_CONSULTAS set STATUSANALISEDECREDITO=2 WHERE Id = :Id", parametros);
			}
		}

		public void AtualizarlimiteDeCredito(int id)
		{
			using (OracleConnection con = new OracleConnection(Parametros.StringConexao))
			{
				var parametros = new DynamicParameters();
				parametros.Add(name: "Id", value: id, direction: ParameterDirection.Input);

				con.Execute(@"UPDATE CRM.TB_CRM_SPC_LIMITE_CREDITO SET StatusLimiteCredito =2  WHERE Id = :Id", parametros);
			}
		}
		public void GravarBlackList()
		{
			using (OracleConnection con = new OracleConnection(Parametros.StringConexao))
			{
				con.Execute(@"update CRM.TB_CRM_contas set  blacklist=1  where SUBSTR(REPLACE(REPLACE(REPLACE(Documento, '.',''), '/', ''), '-', ''), 0, 8)  in(
							select SUBSTR(REPLACE(REPLACE(REPLACE(Documento, '.',''), '/', ''), '-', ''), 0, 8)  from  CRM.TB_CRM_contas where blacklist=1)
								and nvl(blacklist,0)=0");
			}
		}

	}

}
