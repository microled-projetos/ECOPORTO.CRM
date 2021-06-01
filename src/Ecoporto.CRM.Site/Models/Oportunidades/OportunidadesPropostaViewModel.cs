using Ecoporto.CRM.Business.Enums;
using Ecoporto.CRM.Business.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ecoporto.CRM.Site.Models
{
    public class OportunidadesPropostaViewModel
    {
        public OportunidadesPropostaViewModel()
        {
            Modelos = new List<Modelo>();
            Vendedores = new List<Vendedor>();
            Impostos = new List<Imposto>();
        }

        public int? OportunidadeId { get; set; }

        public StatusOportunidade StatusOportunidade { get; set; }

        public bool OportunidadeCancelada { get; set; }

        [Display(Name = "Tipo Operação")]
        public TipoOperacao TipoOperacao { get; set; }
        
        [Display(Name = "Modelos")]
        public int ModeloId { get; set; }

        [Display(Name = "Forma Pagamento")]
        public FormaPagamento FormaPagamento { get; set; }

        [Display(Name = "Free Time")]
        public int DiasFreeTime { get; set; }

        [Display(Name = "Vendedor")]
        public int VendedorId { get; set; }

        [Display(Name = "Qtde Dias")]
        public int QtdeDias { get; set; }

        public int Validade { get; set; }

        [Display(Name = "Tipo Validade")]
        public TipoValidade TipoValidade { get; set; }
        
        [Display(Name = "Imposto")]
        public int ImpostoId { get; set; }

        [Display(Name = "Início")]
        public DateTime? DataInicio { get; set; }

        [Display(Name = "Término")]
        public DateTime? DataTermino { get; set; }

        [Display(Name = "Lotes")]
        public string Lote { get; set; }

        [Display(Name = "BL's")]
        public string BL { get; set; }

        [Display(Name = "Contêineres")]
        public string Conteiner { get; set; }

        public bool ParametroBL { get; set; }

        public bool ParametroLote { get; set; }

        public bool ParametroConteiner { get; set; }

        public bool ParametroIdTabela { get; set; }

        public bool Acordo { get; set; }

        [Display(Name = "Hub Port")]
        public bool HubPort { get; set; }

        [Display(Name = "Cobrança Especial")]
        public bool CobrancaEspecial { get; set; }

        [Display(Name = "Desova Parc.")]
        public decimal DesovaParcial { get; set; }

        [Display(Name = "% Carga Pátio")]
        public decimal FatorCP { get; set; }

        [Display(Name = "Posic. Isentos")]
        public int PosicIsento { get; set; }

        [Display(Name = "ID Tabela")]
        public int? TabelaReferencia { get; set; }

        public string OrigemClone { get; set; }

        public bool HabilitaAbaFichas { get; set; }

        public bool HabilitaImpostos { get; set; }

        public bool TabelaCanceladaChronos { get; set; }

        public List<Modelo> Modelos { get; set; }        

        public List<Vendedor> Vendedores { get; set; }

        public List<Imposto> Impostos { get; set; }
    }
}