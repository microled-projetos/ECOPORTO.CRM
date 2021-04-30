using Ecoporto.CRM.Business.Enums;
using Ecoporto.CRM.Business.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ecoporto.CRM.Site.Models
{
    public class SolicitacaoComercialOcorrenciasViewModel
    {
        public SolicitacaoComercialOcorrenciasViewModel()
        {
            Ocorrencias = new List<SolicitacaoComercialOcorrencia>();
        }

        public int Id { get; set; }
        
        public string Descricao { get; set; }

        [Display(Name = "Cancelamento NF")]
        public bool CancelamentoNF { get; set; }

        [Display(Name = "Desconto")]
        public bool Desconto { get; set; }

        [Display(Name = "Restituição")]
        public bool Restituicao { get; set; }

        [Display(Name = "Prorrogação Boleto")]
        public bool ProrrogacaoBoleto { get; set; }

        [Display(Name = "Outros")]
        public bool Outros { get; set; }

        public Status Status { get; set; }

        public List<SolicitacaoComercialOcorrencia> Ocorrencias { get; set; }
    }
}