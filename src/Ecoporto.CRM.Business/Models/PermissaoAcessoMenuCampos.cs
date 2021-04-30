namespace Ecoporto.CRM.Business.Models
{
    public class PermissaoAcessoMenuCampos : Entidade<PermissaoAcessoMenuCampos>
    {
        public int MenuId { get; set; }

        public string ObjetoId { get; set; }        

        public string ObjetoDescricao { get; set; }

        public bool SomenteLeitura { get; set; }

        public bool Requerido { get; set; }

        public TipoPermissao TipoPermissao { get; set; }

        public override void Validar()
        {            
            ValidationResult = Validate(this);
        }
    }

    public enum TipoPermissao
    {
        SOMENTE_LEITURA,
        LEITURA_ESCRITA,
        ACESSO_TOTAL
    }
}
