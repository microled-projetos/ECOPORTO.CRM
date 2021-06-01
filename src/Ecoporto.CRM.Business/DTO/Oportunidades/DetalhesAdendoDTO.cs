namespace Ecoporto.CRM.Business.DTO
{
    public class DetalhesAdendoDTO
    {
        public string Id { get; set; }
        public string OportunidadeDescricao { get; set; }
        public string TipoAdendo { get; set; }
        public string StatusAdendo { get; set; }
        public string Vendedor { get; set; }
        public string FormaPagamento { get; set; }        
        public string SubClientesInclusao { get; set; }
        public string SubClientesExclusao { get; set; }
        public string GruposCnpjInclusao { get; set; }
        public string GruposCnpjExclusao { get; set; }
        public string CriadoPor { get; set; }
    }
}
