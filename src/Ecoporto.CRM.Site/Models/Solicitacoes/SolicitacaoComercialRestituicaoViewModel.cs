using Ecoporto.CRM.Business.DTO;
using Ecoporto.CRM.Business.Enums;
using Ecoporto.CRM.Business.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ecoporto.CRM.Site.Models
{
    public class SolicitacaoComercialRestituicaoViewModel
    {
        public SolicitacaoComercialRestituicaoViewModel()
        {
            RestituicaoNotasFiscais = new List<NotaFiscal>();
            Restituicoes = new List<SolicitacaoRestituicaoDTO>();
            Favorecidos = new List<Conta>();
            Bancos = new List<Banco>();
        }

        public int RestituicaoId { get; set; }

        public int RestituicaoSolicitacaoId { get; set; }

        [Display(Name = "Tipo Pesquisa")]
        public TipoPesquisa RestituicaoTipoPesquisa { get; set; }

        [Display(Name = "Número")]
        public int RestituicaoTipoPesquisaNumero { get; set; }

        [Display(Name = "Nota Fiscal")]
        public int RestituicaoNotaFiscalId { get; set; }

        [Display(Name = "Valor NF")]
        public decimal RestituicaoValorNF { get; set; }

        [Display(Name = "RPS")]
        public int RestituicaoRPS { get; set; }

        [Display(Name = "Razão Social")]
        public string RestituicaoRazaoSocial { get; set; }

        [Display(Name = "Lote")]
        public int RestituicaoLote { get; set; }

        [Display(Name = "Documento")]
        public string RestituicaoDocumento { get; set; }

        [Display(Name = "Favorecido")]
        public int RestituicaoFavorecidoId { get; set; }

        [Display(Name = "Banco")]
        public int RestituicaoBancoId { get; set; }

        [Display(Name = "Valor à Pagar")]
        public decimal RestituicaoValorAPagar { get; set; }

        public string RestituicaoReserva { get; set; }

        [Display(Name = "Agência")]
        public string RestituicaoAgencia { get; set; }

        [Display(Name = "Conta Corrente")]
        public string RestituicaoContaCorrente { get; set; }

        [Display(Name = "Fornecedor SAP")]
        public string RestituicaoFornecedorSAP { get; set; }

        [Display(Name = "Data Vencimento")]
        public DateTime? RestituicaoDataVencimento { get; set; }

        [Display(Name = "Observações")]
        public string RestituicaoObservacoes { get; set; }        

        public int RestituicaoUnidadeSolicitacao { get; set; }

        public int? RestituicaoTipoOperacaoSolicitacao { get; set; }

        public StatusSolicitacao RestituicaoStatusSolicitacao { get; set; }

        public List<NotaFiscal> RestituicaoNotasFiscais { get; set; }

        public List<SolicitacaoRestituicaoDTO> Restituicoes { get; set; }

        public List<Conta> Favorecidos { get; set; }

        public List<Banco> Bancos { get; set; }
    }
}