using System.Collections.Generic;

namespace Ecoporto.CRM.Business.Models
{
    public class PermissaoAcessoMenu : Entidade<PermissaoAcessoMenu>
    {        
        public string Descricao { get; set; }

        public string DescricaoCompleta { get; set; }

        public bool Acessar { get; set; }

        public bool Cadastrar { get; set; }

        public bool Atualizar { get; set; }

        public bool Excluir { get; set; }

        public bool Logs { get; set; }

        public bool Dinamico { get; set; }

        public List<PermissaoAcessoMenuCampos> Campos { get; set; }

        public override void Validar()
        {
            ValidationResult = Validate(this);
        }
    }   
}
