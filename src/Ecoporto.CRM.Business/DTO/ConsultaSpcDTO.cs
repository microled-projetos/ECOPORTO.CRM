using Ecoporto.CRM.Business.Enums;
using System;
using System.Collections.Generic;

namespace Ecoporto.CRM.Business.DTO
{
    public class LimiteCreditoSpcDTO
    {
        public int Id { get; set; }

        public int ContaId { get; set; }

        public string CondicaoPagamentoId { get; set; }

        public string CondicaoPagamentoDescricao { get; set; }

        public decimal LimiteCredito { get; set; }
 
        public string Observacoes { get; set; }
        public decimal TotalDividaSpc { get; set; }

        public decimal TotalDividaEcoporto { get; set; }

        public bool InadimplenteSpc { get; set; }

        public bool InadimplenteEcoporto { get; set; }

        public StatusLimiteCredito StatusLimiteCredito { get; set; }
    }

    public class DetalhesSpcDTO
    {
        public int ConsultaId { get; set; }

        public string Associado { get; set; }

        public DateTime Inclusao { get; set; }

        public DateTime Vencimento { get; set; }

        public string Entidade { get; set; }

        public string Contrato { get; set; }

        public decimal Valor { get; set; }
    }

    public class ContraOrdemDocumentoDiferenteDTO
    {
        public string Documento { get; set; }

        public DateTime Inclusao { get; set; }

        public DateTime Ocorrencia { get; set; }

        public string Origem { get; set; }

        public string Informante { get; set; }

        public string Descricao { get; set; }
    }

    public class ConsultaRealizadaDTO
    {
        public string Associado { get; set; }

        public DateTime DataConsulta { get; set; }

        public string Entidade { get; set; }

        public string Cidade { get; set; }

        public string Estado { get; set; }
    }

    public class CCFDetalhesDTO
    {
        public string Origem { get; set; }

        public DateTime DataUltimoCheque { get; set; }

        public int Quantidade { get; set; }

        public string Motivo { get; set; }
    }

    public class AlertaDocumentosDTO
    {
        public DateTime Inclusao { get; set; }

        public DateTime Ocorrencia { get; set; }

        public string Entidade { get; set; }

        public string Motivo { get; set; }
        
        public string Tipos { get; set; }
    }

    public class DetalhesChequeLojistaDTO
    {
        public int ConsultaId { get; set; }

        public string Associado { get; set; }

        public string Entidade { get; set; }

        public DateTime Inclusao { get; set; }

        public string Descricao { get; set; }

        public DateTime? ChequeEmissao { get; set; }

        public decimal? ChequeValor { get; set; }

        public string CidadeAssociado { get; set; }
    }

    public class DetalhesPendenciaFinanceiraDTO
    {
        public int ConsultaId { get; set; }

        public string Titulo { get; set; }

        public bool Avalista { get; set; }

        public string Contrato { get; set; }

        public DateTime Ocorrencia { get; set; }

        public string Filial { get; set; }

        public string Origem { get; set; }

        public string Moeda { get; set; }

        public string Natureza { get; set; }

        public string Cidade { get; set; }

        public string UF { get; set; }

        public decimal Valor { get; set; }
    }

    public class ConsultaSpcDTO
    {
        public int Id { get; set; }

        public StatusAnaliseDeCredito StatusAnaliseDeCredito { get; set; }

        public int ContaId { get; set; }

        public bool Restricao { get; set; }

        public string CondicaoPagamento { get; set; }

        public decimal LimiteCredito { get; set; }

        public TipoPessoa Tipo { get; set; }

        public string DataConsulta { get; set; }

        public DateTime Validade { get; set; }

        public string Protocolo { get; set; }

        public decimal TotalDividaSpc { get; set; }

        public decimal TotalDividaEcoporto { get; set; }

        public bool InadimplenteSpc { get; set; }

        public bool InadimplenteEcoporto { get; set; }

        public int Quantidade { get; set; }

        public decimal LimiteNegociado { get; set; }

        public int PrazoSolicitado { get; set; }

        public bool CreditoAprovado { get; set; }

        public string RazaoSocial { get; set; }

        public DateTime? Fundacao { get; set; }

        public string Nome { get; set; }

        public string Nacionalidade { get; set; }

        public DateTime? DataNascimento { get; set; }

        public string CNPJ { get; set; }

        public string CPF { get; set; }

        public string Atividade { get; set; }

        public string Situacao { get; set; }

        public string Logradouro { get; set; }

        public string Bairro { get; set; }

        public string Cidade { get; set; }

        public string Estado { get; set; }

        public int CEP { get; set; }

        public int ProtestoQuantidade { get; set; }

        public DateTime? ProtestoData { get; set; }

        public decimal? ProtestoValorTotal { get; set; }

        public int AcaoQuantidade { get; set; }

        public DateTime? AcaoData { get; set; }

        public decimal? AcaoValorTotal { get; set; }

        public int PendenciaFinancQuantidade { get; set; }

        public DateTime? PendenciaFinancData { get; set; }

        public decimal? PendenciaFinancValorTotal { get; set; }

        public int ParticipFalenciaQuantidade { get; set; }

        public DateTime? ParticipFalenciaData { get; set; }

        public decimal? ParticipFalenciaValorTotal { get; set; }

        public int SpcQuantidade { get; set; }

        public DateTime? SpcData { get; set; }

        public decimal? SpcValorTotal { get; set; }

        public int ChequeSFQuantidade { get; set; }

        public DateTime? ChequeSFData { get; set; }

        public decimal? ChequeSFValorTotal { get; set; }

        public int ChequeSFCCFQuantidade { get; set; }

        public DateTime? ChequeSFCCFData { get; set; }

        public decimal? ChequeSFCCFValorTotal { get; set; }

        public int ChequeLojistaQuantidade { get; set; }

        public DateTime? ChequeLojistaData { get; set; }

        public decimal? ChequeLojistaValorTotal { get; set; }

        public int ChequeCOOutrasQuantidade { get; set; }

        public DateTime? ChequeCOOutrasData { get; set; }

        public decimal? ChequeCOOutrasValorTotal { get; set; }

        public int ConsultaRealizadaQuantidade { get; set; }

        public DateTime? ConsultaRealizadaData { get; set; }

        public decimal? ConsultaRealizadaValorTotal { get; set; }

        public int AlertaDocQuantidade { get; set; }

        public DateTime? AlertaDocData { get; set; }

        public decimal? AlertaDocValorTotal { get; set; }

        public int CreditoConcQuantidade { get; set; }

        public DateTime? CreditoConcData { get; set; }

        public decimal? CreditoConcValorTotal { get; set; }

        public int ContraOrdemQuantidade { get; set; }

        public DateTime? ContraOrdemData { get; set; }

        public decimal? ContraOrdemValorTotal { get; set; }

        public int ContraOrdemDFQuantidade { get; set; }

        public DateTime? ContraOrdemDFData { get; set; }

        public decimal? ContraOrdemDFValorTotal { get; set; }

        public List<DetalhesSpcDTO> DetalhesSpc { get; set; } = new List<DetalhesSpcDTO>();

        public List<DetalhesPendenciaFinanceiraDTO> DetalhesPendenciasFinanceiras { get; set; } = new List<DetalhesPendenciaFinanceiraDTO>();

        public List<DetalhesChequeLojistaDTO> DetalhesChequesLojistas { get; set; } = new List<DetalhesChequeLojistaDTO>();

        public List<ContraOrdemDocumentoDiferenteDTO> DetalhesContraOrdemDocumentoDiferente { get; set; } = new List<ContraOrdemDocumentoDiferenteDTO>();

        public List<ConsultaRealizadaDTO> DetalhesConsultasRealizadas { get; set; } = new List<ConsultaRealizadaDTO>();

        public List<AlertaDocumentosDTO> DetalhesAlertasDocumentos { get; set; } = new List<AlertaDocumentosDTO>();

        public List<CCFDetalhesDTO> DetalhesCCF { get; set; } = new List<CCFDetalhesDTO>();

        public string XML { get; set; }

        public int UsuarioId { get; set; }
        public string Observacoes { get; set; }
        public string CondicaoPagamentoDescricao { get; set; }
    }
}
