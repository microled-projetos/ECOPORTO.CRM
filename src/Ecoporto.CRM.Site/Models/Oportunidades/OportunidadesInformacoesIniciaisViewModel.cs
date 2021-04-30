using Ecoporto.CRM.Business.DTO;
using Ecoporto.CRM.Business.Enums;
using Ecoporto.CRM.Business.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ecoporto.CRM.Site.Models
{
    public class OportunidadesInformacoesIniciaisViewModel
    {
        public OportunidadesInformacoesIniciaisViewModel()
        {
            Empresas = new List<Empresa>();
            Contatos = new List<Contato>();
            Revisoes = new List<Oportunidade>();
            Mercadorias = new List<Mercadoria>();
            Contas = new List<Conta>();
            SubClientes = new List<ClientePropostaDTO>();
            ClientesGrupoCNPJ = new List<ClientePropostaDTO>();
        }

        public int Id { get; set; }

        public int CriadoPor { get; set; }

        [Display(Name = "Proposta")]
        public string Identificacao { get; set; }

        [Display(Name = "Conta")]
        public int ContaId { get; set; }

        [Display(Name = "Empresa")]
        public int EmpresaId { get; set; }

        [Display(Name = "Conta")]
        public int SubClienteContaId { get; set; }

        [Display(Name = "Conta")]
        public int GrupoCNPJContaId { get; set; }

        public bool Aprovada { get; set; }

        public bool ConsultaTabela { get; set; }

        [Display(Name = "Descrição")]
        public string Descricao { get; set; }

        [Display(Name = "Fechamento")]
        public DateTime? DataFechamento { get; set; }       

        [Display(Name = "ID Tabela")]
        public int? TabelaId { get; set; }

        [Display(Name = "Contato")]
        public int ContatoId { get; set; }

        [Display(Name = "% Prob.")]
        public decimal Probabilidade { get; set; }

        [Display(Name = "Sucesso Negociação")]
        public SucessoNegociacao SucessoNegociacao { get; set; }

        [Display(Name = "Class. Cliente")]
        public ClassificacaoCliente ClassificacaoCliente { get; set; }

        [Display(Name = "Segmento")]
        public SegmentoSubCliente SubClienteSegmento { get; set; }

        [Display(Name = "Segmento")]
        public Segmento Segmento { get; set; }

        [Display(Name = "Estágio Negociação")]
        public EstagioNegociacao EstagioNegociacao { get; set; }

        [Display(Name = "Status")]
        public StatusOportunidade StatusOportunidade { get; set; }

        [Display(Name = "Status")]
        public StatusFichaFaturamento StatusFichaFaturamento { get; set; }

        [Display(Name = "Motivo Perda")]
        public MotivoPerda MotivoPerda { get; set; }

        [Display(Name = "Tipo Prop.")]
        public TipoDeProposta TipoDeProposta { get; set; }

        [Display(Name = "Tipo Serviço")]
        public TipoServico TipoServico { get; set; }

        [Display(Name = "Tipo Negócio")]
        public TipoNegocio TipoNegocio { get; set; }

        [Display(Name = "Tipo Operação")]
        public TipoOperacao TipoOperacao { get; set; }

        [Display(Name = "Tipo Oper.")]
        public TipoOperacaoOportunidade TipoOperacaoOportunidade { get; set; }

        [Display(Name = "Revisão")]
        public int? RevisaoId { get; set; }

        public string RevisaoProposta { get; set; }

        [Display(Name = "Mercadoria")]
        public int MercadoriaId { get; set; }        

        [Display(Name = "Observações")]
        public string Observacao { get; set; }

        [Display(Name = "Fat. LCL")]
        public decimal FaturamentoMensalLCL { get; set; }

        [Display(Name = "Fat. FCL")]
        public decimal FaturamentoMensalFCL { get; set; }

        [Display(Name = "Vol. Mensal")]
        public decimal VolumeMensal { get; set; }

        [Display(Name = "CIF Médio")]
        public decimal CIFMedio { get; set; }

        public bool Cancelado { get; set; }

        [Display(Name = "Prêmio")]
        public Boleano PremioParceria { get; set; }

        public string OrigemClone { get; set; }

        [Display(Name = "Data Cancelamento")]
        public DateTime? DataCancelamentoOportunidade { get; set; }

        public bool PermiteAlterarDataCancelamento { get; set; }

        public string Mensagem { get; set; }
        public List<Empresa> Empresas { get; set; }

        public List<Conta> Contas { get; set; }

        public List<Contato> Contatos { get; set; }

        public List<Mercadoria> Mercadorias { get; set; }

        public List<Oportunidade> Revisoes { get; set; }

        public List<ClientePropostaDTO> SubClientes { get; set; }

        public List<ClientePropostaDTO> ClientesGrupoCNPJ { get; set; }
    }
}