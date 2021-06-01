namespace Ecoporto.CRM.Business.DTO
{
    public class SolicitacaoWorkflowDTO
    {
        public int SolicitacaoId { get; set; }

        public string Descricao { get; set; }

        public string TipoSolicitacao { get; set; }

        public string StatusSolicitacao { get; set; }

        public string UnidadeSolicitacao { get; set; }

        public string AreaOcorrenciaSolicitacao { get; set; }

        public string TipoOperacao { get; set; }

        public string Ocorrencia { get; set; }

        public string Motivo { get; set; }

        public string Justificativa { get; set; }

        public string Cliente { get; set; }

        public string CriadoPor { get; set; }

        public string Notas { get; set; }

        public string ValorTotalNF { get; set; }

        public string ValorNovaNF { get; set; }

        public string ValorTotalJuros { get; set; }

        public int QuantidadeNF { get; set; }

        public string ValorDevido { get; set; }

        public string ValorCobrado { get; set; }

        public string ValorCredito { get; set; }

        public string ValorProcesso { get; set; }

        public string DescontoComImposto { get; set; }

        public string Indicador { get; set; }

        public string IndicadorDocumento { get; set; }

        public string DescontoFinal { get; set; }

        public int Lote { get; set; }

        public string Valor { get; set; }

        public string Proposta { get; set; }

        public string Periodo { get; set; }

        public string Vencimento { get; set; }

        public string FreeTime { get; set; }

        public string DataEmissao { get; set; }

        public int GR { get; set; }

        public string Minuta { get; set; }

        public string ValorCIF { get; set; }

        public int Empresa { get; set; }

        public string DiferencaValorDesconto { get; set; }

        public string NFE { get; set; }
        
        public string RazaoSocial { get; set; }

        public string ValorDesconto { get; set; }

        public string DataProrrogacao { get; set; }

        public string DataCadastro { get; set; }

        public string FormaPagamento { get; set; }

        public string CondicaoPagamento { get; set; }

        public int Substituicao { get; set; }

        public string FaturadoContra { get; set; }

        public string FaturadoContraDocumento { get; set; }

        public string EmailNotaFiscal { get; set; }

        public int User_Externo { get; set; }
    }
}
