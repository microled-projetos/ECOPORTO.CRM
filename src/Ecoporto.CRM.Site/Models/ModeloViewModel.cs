using Ecoporto.CRM.Business.DTO;
using Ecoporto.CRM.Business.Enums;
using Ecoporto.CRM.Business.Extensions;
using Ecoporto.CRM.Business.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ecoporto.CRM.Site.Models
{
    public class ModeloViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Tipo de Operação")]
        public TipoOperacao TipoOperacao { get; set; }        

        [Display(Name = "Descrição")]
        public string Descricao { get; set; }

        [Required]
        public Status Status { get; set; }
            
        [Display(Name = "Free Time")]
        public int DiasFreeTime { get; set; }

        [Display(Name = "Qtde Dias")]
        public int QtdeDias { get; set; }

        public int Validade { get; set; }

        [Display(Name = "Tipo Validade")]
        public TipoValidade TipoValidade { get; set; }

        [Display(Name = "Vendedor")]
        public int? VendedorId { get; set; }

        [Display(Name = "Imposto")]
        public int ImpostoId { get; set; }

        public bool Acordo { get; set; }

        [Display(Name = "Integra Chronos")]
        public bool IntegraChronos { get; set; }

        [Display(Name = "Simular")]
        public bool Simular { get; set; }

        [Display(Name = "Parâmetro BL")]
        public bool ParametroBL { get; set; }

        [Display(Name = "Parâmetro ID Tabela")]
        public bool ParametroIdTabela { get; set; }

        public bool Escalonado { get; set; }

        [Display(Name = "Parâmetro Lote")]
        public bool ParametroLote { get; set; }

        [Display(Name = "Parâmetro Contêiner")]
        public bool ParametroConteiner { get; set; }

        [Display(Name = "Forma Pagamento")]
        public FormaPagamento? FormaPagamento { get; set; }

        [Display(Name = "Hub Port")]
        public bool HubPort { get; set; }

        [Display(Name = "Cobrança Especial")]
        public bool CobrancaEspecial { get; set; }

        [Display(Name = "Desova Parc.")]
        public decimal DesovaParcial { get; set; }

        [Display(Name = "Tipo Serviço")]
        public TipoServico TipoServico { get; set; }

        [Display(Name = "% Carga Pátio")]
        public decimal FatorCP { get; set; }

        [Display(Name = "Nº de Posic. Isentos")]
        public int PosicIsento { get; set; }

        [Display(Name = "Modelo Simulador")]
        public int ModeloSimuladorId { get; set; }

        public List<ModeloSimulador> ModelosSimulador { get; set; }

        public List<VinculoModeloSimuladoDTO> ModelosSimuladorVinculados { get; set; }

        public List<Vendedor> Vendedores { get; set; }

        public List<Imposto> Impostos { get; set; }

        public string DescricaoFormaPagamento 
            => FormaPagamento?.ToName();        

        public string DescricaoTipoOperacao 
            => TipoOperacao.ToName();

        public string DescricaoStatus
            => Status.ToName();

        public string DescricaoTipoValidade
            => TipoValidade.ToName();
    }
}