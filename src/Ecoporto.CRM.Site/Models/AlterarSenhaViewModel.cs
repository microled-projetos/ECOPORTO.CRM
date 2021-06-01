using System.ComponentModel.DataAnnotations;

namespace Ecoporto.CRM.Site.Models
{
    public class AlterarSenhaViewModel
    {        
        public int Id { get; set; }

        public string Login { get; set; }
     
        [DataType(DataType.Password)]
        [Display(Name = "Senha Atual")]
        public string SenhaAtual { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Nova Senha")]
        public string NovaSenha { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmação Nova Senha")]
        public string ConfirmacaoNovaSenha { get; set; }      
    }
}