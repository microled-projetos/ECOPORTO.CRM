using Ecoporto.CRM.Business.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ecoporto.CRM.Site.Models
{
    public class FaixasVolumeViewModel
    {
        public FaixasVolumeViewModel()
        {
            FaixasVolume = new List<FaixaVolume>();
        }

        public int Id { get; set; }

        public int FaixaVolumeLayoutId { get; set; }

        [Display(Name = "Volume Inicial:")]
        public decimal FaixasVolumeValorInicial { get; set; }

        [Display(Name = "Volume Final:")]
        public decimal FaixasVolumeValorFinal { get; set; }
        
        [Display(Name = "Preço:")]
        public decimal FaixasVolumePreco { get; set; }

        public IEnumerable<FaixaVolume> FaixasVolume { get; set; }
    }
}