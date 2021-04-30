using Ecoporto.CRM.Business.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ecoporto.CRM.Site.Models
{
    public class FaixasPesoViewModel
    {
        public FaixasPesoViewModel()
        {
            FaixasPeso = new List<FaixaPeso>();
        }

        public int Id { get; set; }

        public int FaixaPesoLayoutId { get; set; }

        [Display(Name = "Peso Inicial:")]
        public decimal FaixasPesoValorInicial { get; set; }

        [Display(Name = "Peso Final:")]
        public decimal FaixasPesoValorFinal { get; set; }
        
        [Display(Name = "Preço:")]
        public decimal FaixasPesoPreco { get; set; }

        public IEnumerable<FaixaPeso> FaixasPeso { get; set; }
    }
}