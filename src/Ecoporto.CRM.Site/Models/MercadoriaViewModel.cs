using Ecoporto.CRM.Business.Enums;
using System.ComponentModel.DataAnnotations;

namespace Ecoporto.CRM.Site.Models
{
    public class MercadoriaViewModel
    {       
        public int Id { get; set; }

        [Display(Name = "Descrição")]
        public string Descricao { get; set; }
        
        [Display(Name = "Status")]
        public Status Status { get; set; }   
    }
}