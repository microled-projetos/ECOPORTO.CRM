using Ecoporto.CRM.Business.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ecoporto.CRM.Site.Models
{
    public class FaixasBLViewModel
    {
        public FaixasBLViewModel()
        {
            FaixasBL = new List<FaixaBL>();
        }

        public int Id { get; set; }

        public int FaixaBLLayoutId { get; set; }

        [Display(Name = "BL Mínimo:")]
        public int FaixasBLMinimo { get; set; }

        [Display(Name = "BL Máximo:")]
        public int FaixasBLMaximo { get; set; }

        [Display(Name = "Valor Mínimo:")]
        public decimal FaixasBLValorMinimo { get; set; }

        public IEnumerable<FaixaBL> FaixasBL { get; set; }
    }
}