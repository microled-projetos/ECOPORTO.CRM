using Ecoporto.CRM.Business.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ecoporto.CRM.Site.Models
{
    public class PermissaoViewModel
    {
        public PermissaoViewModel()
        {
            Cargos = new List<Cargo>();
        }

        [Display(Name = "Cargo")]
        public int CargoId { get; set; }

        [Display(Name = "Menus")]
        public List<PermissaoAcessoMenu> Menus { get; set; }

        public List<Cargo> Cargos { get; set; }
    }
}