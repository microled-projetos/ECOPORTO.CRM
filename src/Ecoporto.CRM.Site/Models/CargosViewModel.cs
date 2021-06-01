using System.ComponentModel.DataAnnotations;

namespace Ecoporto.CRM.Site.Models
{
    public class CargosViewModel
    {     
        public int Id { get; set; }

        [Display(Name = "Descrição")]
        public string Descricao { get; set; }

        [Display(Name = "Vendedor")]
        public bool Vendedor { get; set; }
    }
}