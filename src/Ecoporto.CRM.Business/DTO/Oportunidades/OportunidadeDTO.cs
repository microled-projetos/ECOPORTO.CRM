using Ecoporto.CRM.Business.Enums;
using System;

namespace Ecoporto.CRM.Business.DTO
{
    public class OportunidadeDTO
    {      
        public int Id { get; set; }

        public string Identificacao { get; set; }

        public string Descricao { get; set; }

        public StatusOportunidade StatusOportunidade { get; set; }

        public SucessoNegociacao SucessoNegociacao { get; set; }

        public EstagioNegociacao EstagioNegociacao { get; set; }

        public TipoDeProposta TipoDeProposta { get; set; }

        public TipoServico TipoServico { get; set; }

        public DateTime DataFechamento { get; set; }

        public int TabelaId { get; set; }

        public DateTime DataInicio { get; set; }

        public DateTime DataTermino { get; set; }

        public string CriadoPor { get; set; }

        public int ContaId { get; set; }

        public bool Aprovada { get; set; }

        public int ContatoId { get; set; }

        public decimal Probabilidade { get; set; }

        public ClassificacaoCliente ClassificacaoCliente { get; set; }

        public Segmento Segmento { get; set; }

        public MotivoPerda MotivoPerda { get; set; }

        public TipoNegocio TipoNegocio { get; set; }

        public TipoOperacaoOportunidade TipoOperacaoOportunidade { get; set; }

        public int? RevisaoId { get; set; }

        public int MercadoriaId { get; set; }

        public string Observacao { get; set; }

        public decimal FaturamentoMensalLCL { get; set; }

        public decimal FaturamentoMensalFCL { get; set; }

        public decimal VolumeMensal { get; set; }

        public decimal CIFMedio { get; set; }

        public Boleano PremioParceria { get; set; }

        public DateTime DataCriacao { get; set; }

        public string AlteradoPor { get; set; }

        public DateTime UltimaAlteracao { get; set; }

        public TipoOperacao TipoOperacao { get; set; }

        public int ModeloId { get; set; }

        public FormaPagamento FormaPagamento { get; set; }

        public int DiasFreeTime { get; set; }

        public int QtdeDias { get; set; }

        public int Validade { get; set; }

        public TipoValidade TipoValidade { get; set; }

        public int VendedorId { get; set; }

        public int ImpostoId { get; set; }

        public string ContaDescricao { get; set; }

        public string ContaDocumento { get; set; }

        public string ContaEndereco { get; set; }

        public string ContaCidade { get; set; }

        public string ContaCEP { get; set; }

        public string ContaTelefone { get; set; }

        public string Contato { get; set; }

        public string Revisao { get; set; }

        public string Mercadoria { get; set; }

        public string Modelo { get; set; }

        public string Vendedor { get; set; }

        public string Imposto { get; set; }

        public int TotalLinhas { get; set; }
    }
}
