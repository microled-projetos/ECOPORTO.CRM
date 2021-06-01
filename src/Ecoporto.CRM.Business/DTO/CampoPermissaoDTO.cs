namespace Ecoporto.CRM.Business.DTO
{
    public class CampoPermissaoDTO
    {
        public CampoPermissaoDTO(string campo, bool permissao)
        {
            Campo = campo;
            Permissao = permissao;
        }

        public string Campo { get; set; }

        public bool Permissao { get; set; }
    }
}
