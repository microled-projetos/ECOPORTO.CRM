using Ecoporto.CRM.Business.DTO;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ecoporto.CRM.Site.Models
{
    public class EquipeContaViewModel
    {
        public EquipeContaViewModel()
        {
            Vinculos = new List<EquipeContaUsuarioDTO>();
        }

        public int ContaId { get; set; }

        [Display(Name = "Descrição")]
        public string Descricao { get; set; }

        [Display(Name = "Documento (CPF/CNPJ)")]
        public string Documento { get; set; }

        public int VendedorId { get; set; }

        [Display(Name = "Vendedor")]
        public string VendedorNome { get; set; }

        public IEnumerable<EquipeContaUsuarioDTO> Vinculos { get; set; }
    }
}