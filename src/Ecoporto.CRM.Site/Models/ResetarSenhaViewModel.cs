using System.ComponentModel.DataAnnotations;

namespace Ecoporto.CRM.Site.Models
{
    public class ResetarSenhaViewModel
    {        
        public int Id { get; set; }

        public string Login { get; set; }
             
        public string CPF { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Nova Senha")]
        public string NovaSenha { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Nova Senha")]
        public string ConfirmacaoNovaSenha { get; set; }
    }
}