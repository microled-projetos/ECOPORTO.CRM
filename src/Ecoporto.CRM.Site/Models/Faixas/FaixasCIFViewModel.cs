using Ecoporto.CRM.Business.Enums;
using Ecoporto.CRM.Business.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ecoporto.CRM.Site.Models
{
    public class FaixasCIFViewModel
    {
        public FaixasCIFViewModel()
        {
            FaixasCIF = new List<FaixaCIF>();
        }

        public int Id { get; set; }

        public int FaixaCIFLayoutId { get; set; }

        [Display(Name = "CIF Min:")]
        public decimal FaixasCIFMinimo { get; set; }

        [Display(Name = "CIF Máx:")]
        public decimal FaixasCIFMaximo { get; set; }

        [Display(Name = "Margem:")]
        public Margem FaixasCIFMargem { get; set; }

        [Display(Name = "Valor 20:")]
        public decimal FaixasCIFValor20 { get; set; }

        [Display(Name = "Valor 40:")]
        public decimal FaixasCIFValor40 { get; set; }

        [Display(Name = "Descrição:")]
        public string FaixasCIFDescricao { get; set; }

        public IEnumerable<FaixaCIF> FaixasCIF { get; set; }
    }
}