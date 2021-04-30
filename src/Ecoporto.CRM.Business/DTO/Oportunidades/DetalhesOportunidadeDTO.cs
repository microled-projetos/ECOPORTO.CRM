using System;

namespace Ecoporto.CRM.Business.DTO
{
    public class DetalhesOportunidadeDTO
    {
        public virtual int Id { get; set; }

        public virtual string Identificacao { get; set; }

        public virtual string Descricao { get; set; }

        public virtual string StatusOportunidade { get; set; }

        public virtual string SucessoNegociacao { get; set; }

        public virtual string EstagioNegociacao { get; set; }

        public virtual string TipoDeProposta { get; set; }

        public virtual string TipoServico { get; set; }

        public virtual DateTime DataFechamento { get; set; }

        public virtual int TabelaId { get; set; }

        public virtual DateTime DataInicio { get; set; }

        public virtual DateTime DataTermino { get; set; }

        public virtual int CriadoPor { get; set; }

        public virtual int ContaId { get; set; }

        public virtual bool Aprovada { get; set; }

        public virtual int ContatoId { get; set; }

        public virtual decimal Probabilidade { get; set; }

        public virtual string ClassificacaoCliente { get; set; }

        public virtual string Segmento { get; set; }

        public virtual string MotivoPerda { get; set; }

        public virtual string TipoNegocio { get; set; }

        public virtual string TipoOperacaoOportunidade { get; set; }

        public virtual int? RevisaoId { get; set; }

        public virtual int MercadoriaId { get; set; }

        public virtual string Observacao { get; set; }

        public virtual decimal FaturamentoMensalLCL { get; set; }

        public virtual decimal FaturamentoMensalFCL { get; set; }

        public virtual decimal VolumeMensal { get; set; }

        public virtual decimal CIFMedio { get; set; }

        public virtual string PremioParceria { get; set; }

        public virtual DateTime DataCriacao { get; set; }

        public virtual string AlteradoPor { get; set; }

        public virtual DateTime UltimaAlteracao { get; set; }

        public virtual string TipoOperacao { get; set; }

        public virtual int ModeloId { get; set; }

        public virtual string FormaPagamento { get; set; }

        public virtual int DiasFreeTime { get; set; }

        public virtual int QtdeDias { get; set; }

        public virtual int Validade { get; set; }

        public virtual string TipoValidade { get; set; }

        public virtual int VendedorId { get; set; }

        public virtual int ImpostoId { get; set; }

        public virtual string Conta { get; set; }

        public virtual string ContaDocumento { get; set; }

        public virtual string ContaEndereco { get; set; }

        public virtual string ContaCidade { get; set; }

        public virtual string ContaCEP { get; set; }

        public virtual string ContaTelefone { get; set; }

        public virtual string Contato { get; set; }

        public virtual string Revisao { get; set; }

        public virtual string Mercadoria { get; set; }

        public virtual string Modelo { get; set; }

        public virtual string Vendedor { get; set; }

        public virtual string Imposto { get; set; }
    }
}
