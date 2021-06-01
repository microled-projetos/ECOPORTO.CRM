using System;

namespace Ecoporto.CRM.IntegraChronosService
{
    public class Oportunidade
    {
        public int Id { get; set; }

        public string Identificacao { get; set; }

        public string Descricao { get; set; }

        public string StatusOportunidade { get; set; }

        public string SucessoNegociacao { get; set; }

        public int TipoServicoModelo { get; set; }

        public string EstagioNegociacao { get; set; }

        public string TipoDeProposta { get; set; }

        public string TipoServico { get; set; }

        public DateTime DataFechamento { get; set; }

        public int TabelaId { get; set; }

        public DateTime DataInicio { get; set; }

        public DateTime DataTermino { get; set; }

        public DateTime DataCancelamento { get; set; }

        public int CriadoPor { get; set; }

        public int ContaId { get; set; }

        public bool Aprovada { get; set; }

        public int ContatoId { get; set; }

        public decimal Probabilidade { get; set; }

        public string ClassificacaoCliente { get; set; }

        public string Segmento { get; set; }

        public string MotivoPerda { get; set; }

        public string TipoNegocio { get; set; }

        public string TipoOperacaoOportunidade { get; set; }

        public int? RevisaoId { get; set; }

        public int MercadoriaId { get; set; }

        public string Observacao { get; set; }

        public decimal FaturamentoMensalLCL { get; set; }

        public decimal FaturamentoMensalFCL { get; set; }

        public decimal VolumeMensal { get; set; }

        public decimal CIFMedio { get; set; }

        public string PremioParceria { get; set; }

        public DateTime DataCriacao { get; set; }

        public string AlteradoPor { get; set; }

        public DateTime UltimaAlteracao { get; set; }

        public string FormaPagamento { get; set; }

        public int DiasFreeTime { get; set; }

        public int QtdeDias { get; set; }

        public int Validade { get; set; }

        public bool Cancelado { get; set; }

        public string TipoValidade { get; set; }

        public int VendedorId { get; set; }

        public string VendedorCpf { get; set; }

        public int ImpostoId { get; set; }

        public string Conta { get; set; }

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

        public bool Acordo { get; set; }

        public bool HubPort { get; set; }

        public bool CobrancaEspecial { get; set; }

        public decimal DesovaParcial { get; set; }

        public decimal FatorCP { get; set; }

        public int PosicIsento { get; set; }

        public int TabelaReferencia { get; set; }

        public string ParametroIdTabela { get; set; }

        public int? ModeloId { get; set; }
    }
}